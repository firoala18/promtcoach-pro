using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using ProjectsWebApp.Models;

namespace ProjectsWebApp.DataAccsess.Services.Calsses
{
    public sealed class AiProviderResolver : IAiProviderResolver
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _cfg;

        private const string OpenAIBase = "https://api.openai.com/v1";
        private const string KisskiBase = "https://chat-ai.academiccloud.de/v1";
        private const string GeminiBase = "gemini://v1";

        public AiProviderResolver(ApplicationDbContext db, IConfiguration cfg)
            => (_db, _cfg) = (db, cfg);

        public async Task<AiProviderResolution> ResolveChatAsync(string? userId, string? modelOverride, CancellationToken ct)
        {
            // Load global record
            ApiKeySetting? globalRec = null;
            try { globalRec = await _db.ApiKeySettings.OrderByDescending(x => x.UpdatedAt).FirstOrDefaultAsync(ct); } catch { }

            // Determine user's latest group
            string groupName = "Ohne Gruppe";
            try
            {
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    var g = await _db.UserGroupMemberships
                        .Where(m => m.UserId == userId)
                        .OrderByDescending(m => m.CreatedAt)
                        .Select(m => m.Group)
                        .FirstOrDefaultAsync(ct);
                    if (!string.IsNullOrWhiteSpace(g)) groupName = g.Trim();
                }
            }
            catch { }

            // Check if any owner of this group has ApiManager role
            bool hasPrivilegedOwner = false;
            try
            {
                var ownerIds = await _db.DozentGroupOwnerships
                    .Where(o => o.Group == groupName)
                    .Select(o => o.DozentUserId)
                    .Distinct()
                    .ToListAsync(ct);
                if (ownerIds.Count > 0)
                {
                    var roleNames = await _db.UserRoles
                        .Where(ur => ownerIds.Contains(ur.UserId))
                        .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .ToListAsync(ct);
                    hasPrivilegedOwner = roleNames.Any(n => string.Equals(n, "ApiManager", StringComparison.OrdinalIgnoreCase));
                }
            }
            catch { hasPrivilegedOwner = false; }

            // Check if the current user has a privileged role (Admin / SuperAdmin / Coach / ApiManager)
            bool isPrivilegedUser = false;
            try
            {
                if (!string.IsNullOrWhiteSpace(userId))
                {
                    var myRoles = await _db.UserRoles
                        .Where(ur => ur.UserId == userId)
                        .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .ToListAsync(ct);

                    isPrivilegedUser = myRoles.Any(n =>
                        string.Equals(n, "Admin", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(n, "SuperAdmin", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(n, "Coach", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(n, "ApiManager", StringComparison.OrdinalIgnoreCase));
                }
            }
            catch
            {
                isPrivilegedUser = false;
            }

            // Helper: normalize OpenAI aliases
            static string NormalizeOpenAI(string m)
            {
                if (string.IsNullOrWhiteSpace(m)) return "gpt-4o-mini";
                var t = m.Trim();
                if (string.Equals(t, "gpt-5-instant", StringComparison.OrdinalIgnoreCase)) return "gpt-5";
                if (string.Equals(t, "gpt5", StringComparison.OrdinalIgnoreCase) || string.Equals(t, "gpt_5", StringComparison.OrdinalIgnoreCase)) return "gpt-5";
                if (string.Equals(t, "4o-mini", StringComparison.OrdinalIgnoreCase)) return "gpt-4o-mini";
                if (string.Equals(t, "4o", StringComparison.OrdinalIgnoreCase)) return "gpt-4o";
                return t;
            }

            // Determine preferred provider and key/model
            // Final rule set:
            //  - Wenn es mindestens einen Besitzer mit Rolle "ApiManager" gibt, werden IMMER die
            //    GLOBALEN API-Einstellungen verwendet (Gruppen-Keys werden ignoriert).
            //  - Wenn es KEINEN ApiManager-Besitzer gibt, aber ein gültiger Gruppen‑API‑Key
            //    (GroupApiKeySetting + ActiveProvider + Key) existiert, wird dieser Gruppen‑Key verwendet.
            //  - Wenn es weder einen ApiManager‑Besitzer noch einen gültigen Gruppen‑API‑Key gibt,
            //    wird KEIN Fallback auf globale Keys erlaubt -> AI ist für diese Gruppe deaktiviert.

            GroupApiKeySetting? grecord = null;
            bool hasValidGroupKey = false;
            string provider = string.Empty;

            // Special case: if user has no real group ("Ohne Gruppe"), skip group-level gating
            bool isNoGroup = string.Equals(groupName, "Ohne Gruppe", StringComparison.OrdinalIgnoreCase);

            // Admin/SuperAdmin/Coach/ApiManager should always be able to fall back to
            // the global API configuration, so we only enforce group gating for
            // non-privileged users.
            if (!isNoGroup && !isPrivilegedUser)
            {
                try
                {
                    grecord = await _db.Set<GroupApiKeySetting>()
                        .Where(g => g.Group == groupName)
                        .OrderByDescending(g => g.UpdatedAt)
                        .FirstOrDefaultAsync(ct);
                    if (grecord != null && !string.IsNullOrWhiteSpace(grecord.ActiveProvider))
                    {
                        provider = grecord.ActiveProvider.Trim().ToLowerInvariant();
                        if (provider == "openai" && !string.IsNullOrWhiteSpace(grecord.OpenAIKey))
                            hasValidGroupKey = true;
                        else if (provider == "kisski" && !string.IsNullOrWhiteSpace(grecord.KisskiApiKey))
                            hasValidGroupKey = true;
                        else if (provider == "gemini" && !string.IsNullOrWhiteSpace(grecord.GeminiApiKey))
                            hasValidGroupKey = true;
                    }
                }
                catch
                {
                    grecord = null;
                    hasValidGroupKey = false;
                    provider = string.Empty;
                }

                // 1) Wenn es KEINEN ApiManager‑Besitzer gibt, aber einen gültigen Gruppen‑Key,
                //    wird der Gruppen‑Key verwendet.
                if (!hasPrivilegedOwner && hasValidGroupKey && grecord != null)
                {
                    if (provider == "openai")
                    {
                        var model = NormalizeOpenAI(!string.IsNullOrWhiteSpace(modelOverride)
                            ? modelOverride!
                            : (string.IsNullOrWhiteSpace(grecord.OpenAIModel)
                                ? (globalRec?.OpenAIModel ?? _cfg["OpenAI:ModelId"] ?? "gpt-4o-mini")
                                : grecord.OpenAIModel!));
                        if (!(string.Equals(model, "gpt-4o-mini", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(model, "gpt-4o", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(model, "gpt-5", StringComparison.OrdinalIgnoreCase)))
                            model = "gpt-4o-mini";
                        return new AiProviderResolution("openai", OpenAIBase, grecord.OpenAIKey!, model);
                    }
                    if (provider == "kisski")
                    {
                        var model = !string.IsNullOrWhiteSpace(modelOverride)
                            ? modelOverride!
                            : (string.IsNullOrWhiteSpace(grecord.KisskiModel)
                                ? (globalRec?.KisskiModel ?? _cfg["Kisski:ModelId"] ?? "meta-llama-3.1-8b-instruct")
                                : grecord.KisskiModel!);
                        return new AiProviderResolution("kisski", KisskiBase, grecord.KisskiApiKey!, model);
                    }
                    if (provider == "gemini")
                    {
                        var model = !string.IsNullOrWhiteSpace(modelOverride)
                            ? modelOverride!
                            : (string.IsNullOrWhiteSpace(grecord.GeminiModel)
                                ? (globalRec?.GeminiModel ?? _cfg["Gemini:ModelId"] ?? "gemini-2.5-flash")
                                : grecord.GeminiModel!);
                        if (!(string.Equals(model, "gemini-2.5-pro", StringComparison.OrdinalIgnoreCase)
                              || string.Equals(model, "gemini-2.5-flash", StringComparison.OrdinalIgnoreCase)))
                            model = "gemini-2.5-flash";
                        return new AiProviderResolution("gemini", GeminiBase, grecord.GeminiApiKey!, model);
                    }
                }

                // 2) Wenn es weder einen ApiManager‑Besitzer noch einen gültigen Gruppen‑Key gibt,
                //    ist AI für diese Gruppe gesperrt (kein Fallback auf global).
                if (!hasPrivilegedOwner && !hasValidGroupKey)
                {
                    throw new InvalidOperationException("Keine gültige API‑Konfiguration für diese Gruppe: Es ist kein Gruppen‑API‑Key gesetzt und kein ApiManager als Besitzer hinterlegt.");
                }
            }

            // Fallback to global
            var prov = (globalRec?.ActiveProvider ?? string.Empty).Trim().ToLowerInvariant();
            if (prov == "openai")
            {
                var key = (globalRec?.OpenAIKey ?? _cfg["OpenAI:ApiKey"]) ?? string.Empty;
                var model = NormalizeOpenAI(!string.IsNullOrWhiteSpace(modelOverride) ? modelOverride! : (globalRec?.OpenAIModel ?? _cfg["OpenAI:ModelId"] ?? "gpt-4o-mini"));
                if (!(string.Equals(model, "gpt-4o-mini", StringComparison.OrdinalIgnoreCase) || string.Equals(model, "gpt-4o", StringComparison.OrdinalIgnoreCase) || string.Equals(model, "gpt-5", StringComparison.OrdinalIgnoreCase)))
                    model = "gpt-4o-mini";
                return new AiProviderResolution("openai", OpenAIBase, key, model);
            }
            if (prov == "kisski")
            {
                var key = (globalRec?.KisskiApiKey ?? _cfg["Kisski:ApiKey"]) ?? string.Empty;
                var model = !string.IsNullOrWhiteSpace(modelOverride)
                    ? modelOverride!
                    : (globalRec?.KisskiModel ?? _cfg["Kisski:ModelId"] ?? "meta-llama-3.1-8b-instruct");
                return new AiProviderResolution("kisski", KisskiBase, key, model);
            }
            if (prov == "gemini")
            {
                var key = (globalRec?.GeminiApiKey ?? _cfg["Gemini:ApiKey"]) ?? string.Empty;
                var model = !string.IsNullOrWhiteSpace(modelOverride) ? modelOverride! : (globalRec?.GeminiModel ?? _cfg["Gemini:ModelId"] ?? "gemini-2.5-flash");
                if (!(string.Equals(model, "gemini-2.5-pro", StringComparison.OrdinalIgnoreCase) || string.Equals(model, "gemini-2.5-flash", StringComparison.OrdinalIgnoreCase)))
                    model = "gemini-2.5-flash";
                return new AiProviderResolution("gemini", GeminiBase, key, model);
            }

            // Default preference OpenAI→Kisski→Gemini
            if (!string.IsNullOrWhiteSpace(globalRec?.OpenAIKey ?? _cfg["OpenAI:ApiKey"]))
            {
                var key = (globalRec?.OpenAIKey ?? _cfg["OpenAI:ApiKey"]) ?? string.Empty;
                var model = NormalizeOpenAI(!string.IsNullOrWhiteSpace(modelOverride) ? modelOverride! : (globalRec?.OpenAIModel ?? _cfg["OpenAI:ModelId"] ?? "gpt-4o-mini"));
                if (!(string.Equals(model, "gpt-4o-mini", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(model, "gpt-4o", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(model, "gpt-5", StringComparison.OrdinalIgnoreCase)))
                    model = "gpt-4o-mini";
                return new AiProviderResolution("openai", OpenAIBase, key, model);
            }
            if (!string.IsNullOrWhiteSpace(globalRec?.KisskiApiKey ?? _cfg["Kisski:ApiKey"]))
            {
                var key = (globalRec?.KisskiApiKey ?? _cfg["Kisski:ApiKey"]) ?? string.Empty;
                var model = !string.IsNullOrWhiteSpace(modelOverride)
                    ? modelOverride!
                    : (globalRec?.KisskiModel ?? _cfg["Kisski:ModelId"] ?? "meta-llama-3.1-8b-instruct");
                return new AiProviderResolution("kisski", KisskiBase, key, model);
            }
            if (!string.IsNullOrWhiteSpace(globalRec?.GeminiApiKey ?? _cfg["Gemini:ApiKey"]))
            {
                var key = (globalRec?.GeminiApiKey ?? _cfg["Gemini:ApiKey"]) ?? string.Empty;
                var model = !string.IsNullOrWhiteSpace(modelOverride) ? modelOverride! : (globalRec?.GeminiModel ?? _cfg["Gemini:ModelId"] ?? "gemini-2.5-flash");
                if (!(string.Equals(model, "gemini-2.5-pro", StringComparison.OrdinalIgnoreCase) || string.Equals(model, "gemini-2.5-flash", StringComparison.OrdinalIgnoreCase)))
                    model = "gemini-2.5-flash";
                return new AiProviderResolution("gemini", GeminiBase, key, model);
            }

            throw new InvalidOperationException("Keine gültige API‑Konfiguration gefunden.");
        }

        public async Task<AiProviderResolution> ResolveGroupChatAsync(string groupName, string? modelOverride, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(groupName))
                throw new ArgumentException("Group name is required.", nameof(groupName));

            var normGroup = groupName.Trim();

            // Load global record (for model defaults only, never for keys)
            ApiKeySetting? globalRec = null;
            try { globalRec = await _db.ApiKeySettings.OrderByDescending(x => x.UpdatedAt).FirstOrDefaultAsync(ct); } catch { }

            // Check if any owner of this group has ApiManager role
            bool hasPrivilegedOwner = false;
            try
            {
                var ownerIds = await _db.DozentGroupOwnerships
                    .Where(o => o.Group == normGroup)
                    .Select(o => o.DozentUserId)
                    .Distinct()
                    .ToListAsync(ct);
                if (ownerIds.Count > 0)
                {
                    var roleNames = await _db.UserRoles
                        .Where(ur => ownerIds.Contains(ur.UserId))
                        .Join(_db.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .ToListAsync(ct);
                    hasPrivilegedOwner = roleNames.Any(n => string.Equals(n, "ApiManager", StringComparison.OrdinalIgnoreCase));
                }
            }
            catch { hasPrivilegedOwner = false; }

            if (hasPrivilegedOwner)
            {
                // Gruppe hat einen ApiManager‑Besitzer → auf globale Konfiguration zurückfallen
                return await ResolveChatAsync(null, modelOverride, ct);
            }

            GroupApiKeySetting? grecord = null;
            bool hasValidGroupKey = false;
            string provider = string.Empty;

            try
            {
                grecord = await _db.Set<GroupApiKeySetting>()
                    .Where(g => g.Group == normGroup)
                    .OrderByDescending(g => g.UpdatedAt)
                    .FirstOrDefaultAsync(ct);
                if (grecord != null && !string.IsNullOrWhiteSpace(grecord.ActiveProvider))
                {
                    provider = grecord.ActiveProvider.Trim().ToLowerInvariant();
                    if (provider == "openai" && !string.IsNullOrWhiteSpace(grecord.OpenAIKey))
                        hasValidGroupKey = true;
                    else if (provider == "kisski" && !string.IsNullOrWhiteSpace(grecord.KisskiApiKey))
                        hasValidGroupKey = true;
                    else if (provider == "gemini" && !string.IsNullOrWhiteSpace(grecord.GeminiApiKey))
                        hasValidGroupKey = true;
                }
            }
            catch
            {
                grecord = null;
                hasValidGroupKey = false;
                provider = string.Empty;
            }

            if (!hasValidGroupKey || grecord == null)
            {
                // Kein gültiger Gruppen‑Key konfiguriert → auf globale Konfiguration zurückfallen
                return await ResolveChatAsync(null, modelOverride, ct);
            }

            static string NormalizeOpenAI(string m)
            {
                if (string.IsNullOrWhiteSpace(m)) return "gpt-4o-mini";
                var t = m.Trim();
                if (string.Equals(t, "gpt-5-instant", StringComparison.OrdinalIgnoreCase)) return "gpt-5";
                if (string.Equals(t, "gpt5", StringComparison.OrdinalIgnoreCase) || string.Equals(t, "gpt_5", StringComparison.OrdinalIgnoreCase)) return "gpt-5";
                if (string.Equals(t, "4o-mini", StringComparison.OrdinalIgnoreCase)) return "gpt-4o-mini";
                if (string.Equals(t, "4o", StringComparison.OrdinalIgnoreCase)) return "gpt-4o";
                return t;
            }

            if (provider == "openai")
            {
                var model = NormalizeOpenAI(!string.IsNullOrWhiteSpace(modelOverride)
                    ? modelOverride!
                    : (string.IsNullOrWhiteSpace(grecord.OpenAIModel)
                        ? (globalRec?.OpenAIModel ?? _cfg["OpenAI:ModelId"] ?? "gpt-4o-mini")
                        : grecord.OpenAIModel!));
                if (!(string.Equals(model, "gpt-4o-mini", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(model, "gpt-4o", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(model, "gpt-5", StringComparison.OrdinalIgnoreCase)))
                    model = "gpt-4o-mini";
                return new AiProviderResolution("openai", OpenAIBase, grecord.OpenAIKey!, model);
            }
            if (provider == "kisski")
            {
                var model = !string.IsNullOrWhiteSpace(modelOverride)
                    ? modelOverride!
                    : (string.IsNullOrWhiteSpace(grecord.KisskiModel)
                        ? (globalRec?.KisskiModel ?? _cfg["Kisski:ModelId"] ?? "meta-llama-3.1-8b-instruct")
                        : grecord.KisskiModel!);
                return new AiProviderResolution("kisski", KisskiBase, grecord.KisskiApiKey!, model);
            }
            if (provider == "gemini")
            {
                var model = !string.IsNullOrWhiteSpace(modelOverride)
                    ? modelOverride!
                    : (string.IsNullOrWhiteSpace(grecord.GeminiModel)
                        ? (globalRec?.GeminiModel ?? _cfg["Gemini:ModelId"] ?? "gemini-2.5-flash")
                        : grecord.GeminiModel!);
                if (!(string.Equals(model, "gemini-2.5-pro", StringComparison.OrdinalIgnoreCase)
                      || string.Equals(model, "gemini-2.5-flash", StringComparison.OrdinalIgnoreCase)))
                    model = "gemini-2.5-flash";
                return new AiProviderResolution("gemini", GeminiBase, grecord.GeminiApiKey!, model);
            }

            throw new InvalidOperationException("Unbekannter Provider für Gruppen‑API‑Key.");
        }

        public async Task<string> ResolveEmbeddingsKeyAsync(CancellationToken ct)
        {
            try
            {
                var key = await _db.ApiKeySettings
                    .OrderByDescending(x => x.UpdatedAt)
                    .Select(x => x.OpenAIEmbeddingsKey ?? x.OpenAIKey)
                    .FirstOrDefaultAsync(ct);
                if (!string.IsNullOrWhiteSpace(key)) return key!;
            }
            catch { }
            var cfgKey = _cfg["OpenAI:EmbeddingsApiKey"] ?? _cfg["OpenAI:ApiKey"];
            if (!string.IsNullOrWhiteSpace(cfgKey)) return cfgKey!;
            var env = Environment.GetEnvironmentVariable("OpenAI__EmbeddingsApiKey")
                      ?? Environment.GetEnvironmentVariable("OpenAI__ApiKey")
                      ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(env)) return env;
            throw new InvalidOperationException("OpenAI‑Embeddings‑Key fehlt.");
        }
    }
}
