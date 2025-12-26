using Dto;
using Dto.PromptFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using OpenAI;
using System.ClientModel;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using ProjectsWebApp.Utility;
using System.Text;
using System.Net.Http;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace ProjectsWebApp.Areas.User.Controllers
{
    [Area("User")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public sealed class PromptTechniqueController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _cfg;
        private readonly IAiProviderResolver _resolver;
        private readonly IUserActivityLogger _activityLogger;

        public PromptTechniqueController(ApplicationDbContext db,
                                         IConfiguration cfg,
                                         IAiProviderResolver resolver,
                                         IUserActivityLogger activityLogger)
            => (_db, _cfg, _resolver, _activityLogger) = (db, cfg, resolver, activityLogger);

        public sealed record GenerateRequest(PromptFormDto Form, bool UseKiAssistant, string? ModelId, string? GroupName);
        public sealed record GenerateResponse(string Html);

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GenerateRequest req, CancellationToken ct)
        {
            if (req?.Form is null) return BadRequest("Ungültige Eingabe");

            // Global feature toggle
            if (!FeatureFlags.EnablePromptTechnique)
                return Forbid("Prompt‑Techniken wurden deaktiviert.");

            var uid = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var resolved = await _resolver.ResolveChatAsync(uid, req.ModelId, ct);
            var baseUrl = resolved.BaseUrl;
            var apiKey = resolved.ApiKey;
            var modelId = resolved.ModelId;
            if (string.IsNullOrWhiteSpace(apiKey)) return BadRequest("API‑Konfiguration fehlt.");

            // Load dynamic system preamble from DB if available (with group override)
            var latestSystem = _db.PromptAiSettings
                                    .Where(x => x.SystemPreamble != null && x.SystemPreamble != "")
                                    .OrderByDescending(x => x.UpdatedAt)
                                    .Select(x => x.SystemPreamble)
                                    .FirstOrDefault();
            string? latestKi = null;
            try
            {
                latestKi = _db.PromptAiSettings
                              .Where(x => x.KiAssistantSystemPrompt != null && x.KiAssistantSystemPrompt != "")
                              .OrderByDescending(x => x.UpdatedAt)
                              .Select(x => x.KiAssistantSystemPrompt)
                              .FirstOrDefault();
            }
            catch { }
            string? groupOverrideSystem = null;
            string? groupOverrideKi = null;
            string? groupOverrideUserInstruction = null;
            try
            {
                // Prefer explicit group from request; fall back to latest membership
                string? groupName = (req.GroupName ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(groupName) && !string.IsNullOrWhiteSpace(uid))
                {
                    var grp = await _db.UserGroupMemberships
                                       .Where(m => m.UserId == uid)
                                       .OrderByDescending(m => m.CreatedAt)
                                       .Select(m => m.Group)
                                       .FirstOrDefaultAsync(ct);
                    groupName = string.IsNullOrWhiteSpace(grp) ? "Ohne Gruppe" : grp!.Trim();
                }

                if (!string.IsNullOrWhiteSpace(groupName))
                {
                    var gp = await _db.GroupPromptAiSettings
                                      .Where(g => g.Group == groupName)
                                      .OrderByDescending(g => g.UpdatedAt)
                                      .FirstOrDefaultAsync(ct);
                    var useGlobal = gp?.UseGlobal ?? true;
                    if (!useGlobal && gp != null)
                    {
                        if (!string.IsNullOrWhiteSpace(gp.SystemPreamble)) groupOverrideSystem = gp.SystemPreamble;
                        if (!string.IsNullOrWhiteSpace(gp.KiAssistantSystemPrompt)) groupOverrideKi = gp.KiAssistantSystemPrompt;
                        if (!string.IsNullOrWhiteSpace(gp.UserInstructionText)) groupOverrideUserInstruction = gp.UserInstructionText;
                    }
                }
            }
            catch { }

            var defaultTechniqueSys = """

Du bist ein Experte für KI und Prompt Engineering. Extrahiere aus instruktiven Eingaben wissenschaftlich fundierte Prompt-Techniken, die direkt in einem LLM angewendet werden können, um präzise Ergebnisse zu erzielen.
Ausgabeformat:
(Prompt Technique in English): Deutsche Anweisung. <br/>

Richtlinien:

- Verwende keine Aufzählungen oder Nummerierungen. 
- Die Prompt Techniken sollen in ihrem offiziellen englischen Fachbegriff stehen, während die zugehörige Beschreibung auf Deutsch formuliert wird. 
- Jede Zeile beschreibt eine einzelne, sofort anwendbare Prompt Technik.
""";
            var sys = (req.UseKiAssistant && !string.IsNullOrWhiteSpace(groupOverrideKi))
                ? groupOverrideKi!
                : (req.UseKiAssistant && !string.IsNullOrWhiteSpace(latestKi))
                    ? latestKi!
                    : (!string.IsNullOrWhiteSpace(groupOverrideSystem) ? groupOverrideSystem! : (!string.IsNullOrWhiteSpace(latestSystem) ? latestSystem! : defaultTechniqueSys));


            var sb = new StringBuilder();
            sb.AppendLine($"#Title: {req.Form.Titel}");
            sb.AppendLine($"#Thema: {req.Form.Thema}");
            sb.AppendLine($"#Ziele: {req.Form.Ziele}");
            sb.AppendLine($"#Beschreibung: {req.Form.Beschreibung}");
            if (req.Form.Schlüsselbegriffe != null && req.Form.Schlüsselbegriffe.Length > 0)
                sb.AppendLine($"#Schlüsselbegriffe: {string.Join(", ", req.Form.Schlüsselbegriffe)}");

            // Optional dynamic user instruction from PromptTypeGuidances (use Text type as global default)
            var dynamicUser = _db.PromptTypeGuidances
                                 .Where(x => x.Type == PromptType.Text)
                                 .OrderByDescending(x => x.UpdatedAt)
                                 .Select(x => x.GuidanceText)
                                 .FirstOrDefault();
            var userInstruction = !string.IsNullOrWhiteSpace(groupOverrideUserInstruction)
                ? groupOverrideUserInstruction
                : (!string.IsNullOrWhiteSpace(dynamicUser)
                ? dynamicUser
                : """
Convert the following input into a structured and visually decorated HTML list
of scientific prompt techniques, following academic prompt-engineering frameworks.
""");

            // Combine admin user instruction with current form input into a single user message
            var user = new StringBuilder()
                .AppendLine(userInstruction.Trim())
                .AppendLine()
                .AppendLine("INPUT:")
                .AppendLine(sb.ToString())
                .ToString();

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(sys),
                new UserChatMessage(user)
            };

            if (baseUrl.StartsWith("gemini://", StringComparison.OrdinalIgnoreCase))
            {
                var http = new HttpClient();
                var model = string.IsNullOrWhiteSpace(modelId) ? "gemini-2.5-flash" : modelId;
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";
                var payload = new
                {
                    contents = new[]
                    {
                        new { role = "user", parts = new[] { new { text = sys + "\n\n" + user } } }
                    },
                    generationConfig = new { temperature = 0, topP = 1 }
                };
                var json = JsonSerializer.Serialize(payload);
                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                var resp = await http.PostAsync(url, content, ct);
                var respText = await resp.Content.ReadAsStringAsync(ct);
                if (!resp.IsSuccessStatusCode) return BadRequest($"Gemini API Fehler: {(int)resp.StatusCode} {resp.ReasonPhrase} -> {respText}");
                using var doc = JsonDocument.Parse(respText);
                var htmlGem = doc.RootElement
                                  .GetProperty("candidates")[0]
                                  .GetProperty("content")
                                  .GetProperty("parts")[0]
                                  .GetProperty("text")
                                  .GetString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(htmlGem)) return BadRequest("Leere Antwort");
                var htmlTrimmed = htmlGem.Trim();

                // Analytics: log prompt generation (Gemini)
                try
                {
                    if (ProjectsWebApp.Utility.FeatureFlags.EnableAnalytics)
                    {
                        // For analytics we always use the latest real group membership
                        // of the user, independent of the System‑Prompt selection.
                        string? analyticsGroup = null;
                        if (!string.IsNullOrWhiteSpace(uid))
                        {
                            try
                            {
                                analyticsGroup = await _db.UserGroupMemberships
                                                          .Where(m => m.UserId == uid)
                                                          .OrderByDescending(m => m.CreatedAt)
                                                          .Select(m => m.Group)
                                                          .FirstOrDefaultAsync(ct);
                                analyticsGroup = string.IsNullOrWhiteSpace(analyticsGroup) ? null : analyticsGroup.Trim();
                            }
                            catch { analyticsGroup = null; }
                        }

                        await _activityLogger.LogAsync(
                            uid ?? string.Empty,
                            string.IsNullOrWhiteSpace(analyticsGroup) ? null : analyticsGroup,
                            "prompt_generate",
                            null,
                            new { provider = "gemini", modelId, useKiAssistant = req.UseKiAssistant },
                            ct);
                    }
                }
                catch { /* analytics must never break main flow */ }

                return Ok(new GenerateResponse(htmlTrimmed));
            }

            ChatCompletionOptions options;
            if (modelId.StartsWith("gpt-5", StringComparison.OrdinalIgnoreCase))
            {
                options = new ChatCompletionOptions();
            }
            else
            {
                options = new ChatCompletionOptions { Temperature = 0.1f, TopP = 1f };
            }

            OpenAIClient openai;
            if (baseUrl.StartsWith("https://chat-ai.academiccloud.de", StringComparison.OrdinalIgnoreCase))
            {
                openai = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions { Endpoint = new Uri(baseUrl) });
            }
            else
            {
                // For OpenAI public API, omit Endpoint to avoid potential double "/v1" path issues.
                openai = new OpenAIClient(new ApiKeyCredential(apiKey));
            }
            var client = openai.GetChatClient(modelId);
            if (baseUrl.Contains("chat-ai.academiccloud.de", StringComparison.OrdinalIgnoreCase))
            {
                var attempts = new List<string> { modelId, _cfg["Kisski:ModelId"] ?? string.Empty, "meta-llama-3.1-8b-instruct", "gemma-3-27b-it", "openai-gpt-oss-120b" }
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
                Exception? lastErr = null;
                foreach (var m in attempts)
                {
                    try
                    {
                        using var http = new HttpClient();
                        http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                        var endpoint = baseUrl.TrimEnd('/') + "/chat/completions";
                        var kisskiMessages = messages.Select(x => new {
                            role = x is SystemChatMessage ? "system" : (x is AssistantChatMessage ? "assistant" : "user"),
                            content = x.Content?.FirstOrDefault()?.Text ?? string.Empty
                        });
                        var payload = new { model = m, messages = kisskiMessages, temperature = options.Temperature ?? 0 };
                        var json = JsonSerializer.Serialize(payload);
                        using var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var resp = await http.PostAsync(endpoint, content, ct);
                        var text = await resp.Content.ReadAsStringAsync(ct);
                        if (resp.StatusCode == System.Net.HttpStatusCode.NotFound)
                        {
                            var altBase = baseUrl.EndsWith("/v1", StringComparison.OrdinalIgnoreCase) ? baseUrl.Substring(0, baseUrl.Length - 3) : (baseUrl.TrimEnd('/') + "/v1");
                            var altEndpoint = altBase.TrimEnd('/') + "/chat/completions";
                            using var content2 = new StringContent(json, Encoding.UTF8, "application/json");
                            resp = await http.PostAsync(altEndpoint, content2, ct);
                            text = await resp.Content.ReadAsStringAsync(ct);
                        }
                        if (!resp.IsSuccessStatusCode) { lastErr = new InvalidOperationException($"Kisski error {(int)resp.StatusCode}: {text}"); continue; }
                        using var doc = JsonDocument.Parse(text);
                        var html = doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(html)) { lastErr = new InvalidOperationException("Leere Antwort"); continue; }

                        // Analytics: log prompt generation (Kisski / chat-ai.academiccloud.de)
                        try
                        {
                            if (ProjectsWebApp.Utility.FeatureFlags.EnableAnalytics)
                            {
                                string? analyticsGroup = null;
                                if (!string.IsNullOrWhiteSpace(uid))
                                {
                                    try
                                    {
                                        analyticsGroup = await _db.UserGroupMemberships
                                                                  .Where(mm => mm.UserId == uid)
                                                                  .OrderByDescending(mm => mm.CreatedAt)
                                                                  .Select(mm => mm.Group)
                                                                  .FirstOrDefaultAsync(ct);
                                        analyticsGroup = string.IsNullOrWhiteSpace(analyticsGroup) ? null : analyticsGroup.Trim();
                                    }
                                    catch { analyticsGroup = null; }
                                }

                                await _activityLogger.LogAsync(
                                    uid ?? string.Empty,
                                    string.IsNullOrWhiteSpace(analyticsGroup) ? null : analyticsGroup,
                                    "prompt_generate",
                                    null,
                                    new { provider = baseUrl, modelId = m, useKiAssistant = req.UseKiAssistant },
                                    ct);
                            }
                        }
                        catch { /* analytics must never break main flow */ }

                        return Ok(new GenerateResponse(html.Trim()));
                    }
                    catch (Exception ex) { lastErr = ex; continue; }
                }
                return BadRequest(lastErr?.Message ?? "Unbekannter Fehler bei Kisski");
            }
            else
            {
                var result = await client.CompleteChatAsync(messages, options, cancellationToken: ct);
                var html = result.Value?.Content?.FirstOrDefault()?.Text?.Trim() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(html)) return BadRequest("Leere Antwort vom Modell");

                // Analytics: log prompt generation (OpenAI/Kisski etc.)
                try
                {
                    if (ProjectsWebApp.Utility.FeatureFlags.EnableAnalytics)
                    {
                        string? analyticsGroup = null;
                        if (!string.IsNullOrWhiteSpace(uid))
                        {
                            try
                            {
                                analyticsGroup = await _db.UserGroupMemberships
                                                          .Where(m => m.UserId == uid)
                                                          .OrderByDescending(m => m.CreatedAt)
                                                          .Select(m => m.Group)
                                                          .FirstOrDefaultAsync(ct);
                                analyticsGroup = string.IsNullOrWhiteSpace(analyticsGroup) ? null : analyticsGroup.Trim();
                            }
                            catch { analyticsGroup = null; }
                        }

                        await _activityLogger.LogAsync(
                            uid ?? string.Empty,
                            string.IsNullOrWhiteSpace(analyticsGroup) ? null : analyticsGroup,
                            "prompt_generate",
                            null,
                            new { provider = baseUrl, modelId, useKiAssistant = req.UseKiAssistant },
                            ct);
                    }
                }
                catch { /* analytics must never break main flow */ }

                return Ok(new GenerateResponse(html));
            }
        }
    }
}
