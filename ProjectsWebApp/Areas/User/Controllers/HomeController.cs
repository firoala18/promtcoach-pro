using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI;
using System.ClientModel;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;
using ProjectsWebApp.Models.ViewModels;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using System.Diagnostics;
using System.Drawing.Printing;
using ProjectsWebApp.Utility;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace ProjectsWebApp.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _cfg;
        private readonly IAiProviderResolver _resolver;
        private readonly IUserActivityLogger _activityLogger;

        public HomeController(ILogger<HomeController> logger,
                              IUnitOfWork unitOfWork,
                              ApplicationDbContext db,
                              UserManager<IdentityUser> userManager,
                              IConfiguration cfg,
                              IAiProviderResolver resolver,
                              IUserActivityLogger activityLogger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _db = db;
            _userManager = userManager;
            _cfg = cfg;
            _resolver = resolver;
            _activityLogger = activityLogger;
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAssistantReflektion(int id, string? reflektion)
        {
            var repo = _unitOfWork.GetRepository<Assistant>();
            var model = repo.GetFirstOrDefault(a => a.Id == id);
            if (model == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            bool allowed = User.IsInRole(Utility.SD.Role_Admin)
                           || User.IsInRole(Utility.SD.Role_Dozent)
                           || User.IsInRole("Coach")
                           || string.Equals(model.CreatedByUserId, currentUserId, StringComparison.Ordinal);
            if (!allowed) return Forbid();

            string? r = string.IsNullOrWhiteSpace(reflektion) ? null : reflektion.Trim();
            const int MaxRefl = 4000;
            if (r != null && r.Length > MaxRefl) r = r.Substring(0, MaxRefl);

            model.Reflektion = r;
            try
            {
                repo.Update(model);
                _unitOfWork.Save();
                return Json(new { ok = true });
            }
            catch (DbUpdateException ex)
            {
                Response.StatusCode = 400;
                return Json(new { ok = false, error = "Speichern der Reflektion fehlgeschlagen.", detail = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAssistantAvatar(int id, string? avatarUrl, IFormFile? avatarFile)
        {
            var repo = _unitOfWork.GetRepository<Assistant>();
            var model = repo.GetFirstOrDefault(a => a.Id == id);
            if (model == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            bool allowed = User.IsInRole(Utility.SD.Role_Admin)
                           || User.IsInRole(Utility.SD.Role_Dozent)
                           || User.IsInRole("Coach")
                           || string.Equals(model.CreatedByUserId, currentUserId, StringComparison.Ordinal);
            if (!allowed) return Forbid();

            string? finalAvatar = null;
            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "assistants");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(avatarFile.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                await using var stream = System.IO.File.Create(filePath);
                await avatarFile.CopyToAsync(stream);
                finalAvatar = $"~/uploads/assistants/{fileName}";
            }
            else if (!string.IsNullOrWhiteSpace(avatarUrl))
            {
                var av = avatarUrl.Trim();
                if (!av.StartsWith("http", StringComparison.OrdinalIgnoreCase) && av.StartsWith("/"))
                    av = "~" + av;
                finalAvatar = av;
            }

            if (string.IsNullOrWhiteSpace(finalAvatar))
            {
                Response.StatusCode = 400;
                return Json(new { ok = false, error = "Keine Avatar-Quelle angegeben." });
            }

            model.AvatarUrl = finalAvatar;
            repo.Update(model);
            _unitOfWork.Save();

            string resolved = finalAvatar;
            try { resolved = Url.Content(finalAvatar); } catch { }
            return Json(new { ok = true, url = resolved });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAssistantAbout(int id, string? goals, string? description)
        {
            var repo = _unitOfWork.GetRepository<Assistant>();
            var model = repo.GetFirstOrDefault(a => a.Id == id);
            if (model == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            bool allowed = User.IsInRole(Utility.SD.Role_Admin)
                           || User.IsInRole(Utility.SD.Role_Dozent)
                           || User.IsInRole("Coach")
                           || string.Equals(model.CreatedByUserId, currentUserId, StringComparison.Ordinal);
            if (!allowed) return Forbid();

            // Normalize inputs
            string? g = string.IsNullOrWhiteSpace(goals) ? null : goals.Trim();
            string? d = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            // Clamp lengths to avoid DB errors (Goals is annotated with MaxLength(2048))
            const int MaxGoals = 2048;
            const int MaxDesc = 4000; // conservative cap; schema may be smaller. Consider migration to nvarchar(max).
            if (g != null && g.Length > MaxGoals) g = g.Substring(0, MaxGoals);
            if (d != null && d.Length > MaxDesc) d = d.Substring(0, MaxDesc);

            model.Goals = g;
            model.Description = d;

            try
            {
                repo.Update(model);
                _unitOfWork.Save();
                return Json(new { ok = true });
            }
            catch (DbUpdateException ex)
            {
                // Return a clear client error; frontend can inform the user
                Response.StatusCode = 400;
                return Json(new { ok = false, error = "Speichern fehlgeschlagen (DB). Bitte Textlänge prüfen.", detail = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAssistantOwnerLicense(int id, string? name, string? authorName, string? licenses)
        {
            var repo = _unitOfWork.GetRepository<Assistant>();
            var model = repo.GetFirstOrDefault(a => a.Id == id);
            if (model == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            bool allowed = User.IsInRole(Utility.SD.Role_Admin)
                           || User.IsInRole(Utility.SD.Role_Dozent)
                           || User.IsInRole("Coach")
                           || string.Equals(model.CreatedByUserId, currentUserId, StringComparison.Ordinal);
            if (!allowed) return Forbid();

            if (!string.IsNullOrWhiteSpace(name))
            {
                model.Name = name.Trim();
            }

            model.AuthorName = string.IsNullOrWhiteSpace(authorName) ? null : authorName.Trim();
            model.Licenses = string.IsNullOrWhiteSpace(licenses) ? null : licenses.Trim();
            repo.Update(model);
            _unitOfWork.Save();
            return Json(new { ok = true });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)]
        [RequestSizeLimit(500_000_000)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadAssistantEmbedding(int id, [FromForm] IFormFile? txtFile, CancellationToken ct)
        {
            if (txtFile == null || txtFile.Length == 0)
            {
                var f = Request?.Form?.Files?.FirstOrDefault();
                if (f != null) txtFile = f;
            }
            if (txtFile == null || txtFile.Length == 0)
            {
                TempData["error"] = "Bitte eine Textdatei auswählen.";
                return RedirectToAction(nameof(AssistantChat), new { id });
            }

            var repo = _unitOfWork.GetRepository<Assistant>();
            var model = repo.GetFirstOrDefault(a => a.Id == id);
            if (model == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            bool allowed = string.Equals(model.CreatedByUserId, currentUserId, StringComparison.Ordinal);
            if (!allowed) return Forbid();

            var isTxt = string.Equals(Path.GetExtension(txtFile.FileName), ".txt", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(txtFile.ContentType, "text/plain", StringComparison.OrdinalIgnoreCase);
            if (!isTxt)
            {
                TempData["error"] = "Nur .txt Dateien werden unterstützt.";
                return RedirectToAction(nameof(AssistantChat), new { id });
            }

            string text;
            using (var sr = new StreamReader(txtFile.OpenReadStream()))
            {
                text = await sr.ReadToEndAsync();
            }
            text = (text ?? string.Empty).Trim();
            if (text.Length == 0)
            {
                TempData["error"] = "Die Datei ist leer.";
                return RedirectToAction(nameof(AssistantChat), new { id });
            }

            var apiKey = await _resolver.ResolveEmbeddingsKeyAsync(ct);

            var chunks = SplitIntoChunks(text, maxTokensPerChunk: 6000);
            if (chunks.Count == 0)
            {
                TempData["error"] = "Text konnte nicht in gültige Chunks aufgeteilt werden.";
                return RedirectToAction(nameof(AssistantChat), new { id });
            }

            try
            {
                var embClient = new EmbeddingClient("text-embedding-3-small", apiKey);
                var embRepo = _unitOfWork.GetRepository<AssistantEmbedding>();

                int stored = 0;
                foreach (var (chunk, index) in chunks.Select((c, i) => (c, i)))
                {
                    var resp = await embClient.GenerateEmbeddingAsync(chunk, cancellationToken: ct);
                    var vector = resp.Value.ToFloats();
                    var json = JsonSerializer.Serialize(vector);

                    embRepo.Add(new AssistantEmbedding
                    {
                        AssistantId = id,
                        SourceFileName = $"{Path.GetFileNameWithoutExtension(txtFile.FileName)}_teil{index + 1}{Path.GetExtension(txtFile.FileName)}",
                        Content = chunk,
                        EmbeddingJson = json,
                        UploadedAtUtc = DateTime.UtcNow
                    });
                    stored++;
                }
                _unitOfWork.Save();

                TempData["success"] = stored == 1
                    ? "Embedding gespeichert."
                    : $"{stored} Embeddings als Teile gespeichert (Text wurde wegen Länge aufgeteilt).";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Embedding fehlgeschlagen: {ex.Message}";
            }

            return RedirectToAction(nameof(AssistantChat), new { id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAssistantEmbedding(int id, int embId)
        {
            var aRepo = _unitOfWork.GetRepository<Assistant>();
            var model = aRepo.GetFirstOrDefault(a => a.Id == id);
            if (model == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            bool allowed = string.Equals(model.CreatedByUserId, currentUserId, StringComparison.Ordinal);
            if (!allowed) return Forbid();

            try
            {
                var repo = _unitOfWork.GetRepository<AssistantEmbedding>();
                var item = repo.GetFirstOrDefault(e => e.Id == embId);
                if (item == null || item.AssistantId != id)
                {
                    TempData["error"] = "Embedding nicht gefunden.";
                    return RedirectToAction(nameof(AssistantChat), new { id });
                }
                repo.Remove(item);
                _unitOfWork.Save();
                TempData["success"] = "Embedding gelöscht.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Löschen fehlgeschlagen: {ex.Message}";
            }
            return RedirectToAction(nameof(AssistantChat), new { id });
        }

        [Authorize]
        [HttpGet]
        public IActionResult DownloadAssistantEmbedding(int id, int embId)
        {
            var repo = _unitOfWork.GetRepository<AssistantEmbedding>();
            var item = repo.GetFirstOrDefault(e => e.Id == embId);
            if (item == null || item.AssistantId != id)
                return NotFound();

            var name = string.IsNullOrWhiteSpace(item.SourceFileName)
                ? $"assistant_{id}_embedding_{embId}.txt"
                : item.SourceFileName;
            var content = item.Content ?? string.Empty;
            var bytes = Encoding.UTF8.GetBytes(content);
            return File(bytes, "text/plain", name);
        }

        private static List<string> SplitIntoChunks(string text, int maxTokensPerChunk)
        {
            var chunks = new List<string>();
            if (string.IsNullOrWhiteSpace(text)) return chunks;

            int maxCharsPerChunk = maxTokensPerChunk * 4;
            var paragraphs = text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.None);
            var current = new StringBuilder();
            int currentLength = 0;

            foreach (var para in paragraphs)
            {
                var trimmed = para.Trim();
                if (trimmed.Length == 0) continue;

                if (currentLength + trimmed.Length + 4 > maxCharsPerChunk)
                {
                    if (current.Length > 0)
                    {
                        chunks.Add(current.ToString().Trim());
                        current.Clear();
                        currentLength = 0;
                    }
                    if (trimmed.Length > maxCharsPerChunk)
                    {
                        for (int i = 0; i < trimmed.Length; i += maxCharsPerChunk)
                        {
                            var part = trimmed.Substring(i, Math.Min(maxCharsPerChunk, trimmed.Length - i));
                            chunks.Add(part);
                        }
                        continue;
                    }
                }

                if (current.Length > 0) current.AppendLine().AppendLine();
                current.Append(trimmed);
                currentLength += trimmed.Length + (current.Length > trimmed.Length ? 4 : 0);
            }

            if (current.Length > 0) chunks.Add(current.ToString().Trim());
            return chunks;
        }

        [Authorize]
        [HttpPost]
        public async Task AssistantReplyStream(int id, [FromBody] ChatRequestDto body, CancellationToken ct)
        {
            var assistant = _unitOfWork
                .GetRepository<Assistant>()
                .GetFirstOrDefault(a => a.Id == id);
            if (assistant == null)
            {
                Response.StatusCode = 404;
                return;
            }

            // Group-based access control (using AssistantGroups table)
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var isDozent = User.IsInRole("Dozent");
            var isCoach = User.IsInRole("Coach");
            var currentUserId = _userManager.GetUserId(User);
            if (!isAdmin && assistant.CreatedByUserId != currentUserId && !(assistant?.IsGlobal ?? false))
            {
                // Get groups the assistant is published to
                HashSet<string> assistantGroups = new(StringComparer.OrdinalIgnoreCase);
                HashSet<int> assistantGroupIds = new();
                try
                {
                    var agList = _db.AssistantGroups.Where(ag => ag.AssistantId == id).ToList();
                    foreach (var ag in agList)
                    {
                        if (ag.GroupId.HasValue) assistantGroupIds.Add(ag.GroupId.Value);
                        if (!string.IsNullOrWhiteSpace(ag.Group)) assistantGroups.Add(ag.Group.Trim());
                    }
                }
                catch { }

                // Get user's groups
                HashSet<string> userGroups = new(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var mems = _db.UserGroupMemberships
                                   .Where(m => m.UserId == currentUserId)
                                   .Select(m => m.Group)
                                   .AsEnumerable()
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .Select(s => s!.Trim());
                    foreach (var g in mems) userGroups.Add(g);
                }
                catch { }

                // For Dozent/Coach: also include owned groups
                if (isDozent || isCoach)
                {
                    try
                    {
                        var owned = _db.DozentGroupOwnerships
                                       .Where(o => o.DozentUserId == currentUserId)
                                       .Select(o => o.Group)
                                       .AsEnumerable()
                                       .Where(s => !string.IsNullOrWhiteSpace(s))
                                       .Select(s => s.Trim());
                        foreach (var g in owned) userGroups.Add(g);
                    }
                    catch { }
                }

                // Check if assistant is published to any of user's groups
                bool allowed = false;

                // Check by group name
                foreach (var ug in userGroups)
                {
                    if (assistantGroups.Contains(ug))
                    {
                        allowed = true;
                        break;
                    }
                }

                // Also check by group ID
                if (!allowed && assistantGroupIds.Count > 0)
                {
                    try
                    {
                        var userGroupIds = _db.Groups
                            .AsEnumerable()
                            .Where(g => g?.Name != null && userGroups.Contains(g.Name.Trim()))
                            .Select(g => g.Id)
                            .ToHashSet();
                        allowed = assistantGroupIds.Any(agId => userGroupIds.Contains(agId));
                    }
                    catch { }
                }

                if (!allowed)
                {
                    Response.StatusCode = 403;
                    return;
                }
            }

            // Resolve provider via centralized resolver
            string baseUrl = string.Empty;
            string? apiKey = null;
            string modelId = string.Empty;
            try
            {
                // Admins/SuperAdmins: bypass group-level gating and use global API configuration
                var resolverUserId = isAdmin ? null : currentUserId;
                var resolved = await _resolver.ResolveChatAsync(resolverUserId, null, ct);
                baseUrl = resolved.BaseUrl;
                apiKey = resolved.ApiKey;
                modelId = resolved.ModelId;
            }
            catch { }
            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
            {
                Response.StatusCode = 500;
                await Response.WriteAsync("API-Konfiguration fehlt (BaseUrl, ApiKey).", ct);
                return;
            }
            if (baseUrl.Contains("api.openai.com", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(modelId, "gpt-5-instant", StringComparison.OrdinalIgnoreCase)) modelId = "gpt-5";
                else if (string.Equals(modelId, "4o-mini", StringComparison.OrdinalIgnoreCase)) modelId = "gpt-4o-mini";
                else if (string.Equals(modelId, "4o", StringComparison.OrdinalIgnoreCase)) modelId = "gpt-4o";
                if (modelId.StartsWith("openai-", StringComparison.OrdinalIgnoreCase)) modelId = _cfg["OpenAI:ModelId"] ?? "gpt-4o-mini";
            }
            _logger.LogInformation("[AssistantReplyStream] Using endpoint: {Endpoint} with model: {Model}", baseUrl, modelId);

            // Analytics: log assistant streaming chat call
            try
            {
                if (ProjectsWebApp.Utility.FeatureFlags.EnableAnalytics)
                {
                    string? userGroup = null;
                    try
                    {
                        userGroup = _db.UserGroupMemberships
                                       .Where(m => m.UserId == currentUserId)
                                       .OrderByDescending(m => m.CreatedAt)
                                       .Select(m => m.Group)
                                       .FirstOrDefault();
                    }
                    catch { }

                    await _activityLogger.LogAsync(
                        currentUserId ?? string.Empty,
                        string.IsNullOrWhiteSpace(userGroup) ? null : (userGroup!.Trim()),
                        "assistant_chat_stream",
                        null,
                        new { assistantId = id, baseUrl, modelId },
                        ct);
                }
            }
            catch { /* analytics must never break main flow */ }

            // Build retrieval-augmented context from stored embeddings
            var ragContext = await BuildRagContextAsync(id, body, ct);
            var messages = new List<ChatMessage> { new SystemChatMessage(assistant.SystemPrompt) };
            if (!string.IsNullOrWhiteSpace(ragContext))
            {
                messages.Add(new SystemChatMessage($"Kontext (aus Wissensbasis):\n{ragContext}"));
            }
            if (body?.Messages != null)
            {
                foreach (var m in body.Messages)
                {
                    var content = m?.Content ?? string.Empty;
                    if (string.Equals(m?.Role, "assistant", StringComparison.OrdinalIgnoreCase))
                        messages.Add(new AssistantChatMessage(content));
                    else
                        messages.Add(new UserChatMessage(content));
                }
            }

            Response.Headers["Content-Type"] = "text/plain; charset=utf-8";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["X-Accel-Buffering"] = "no"; // Nginx buffering off (if any)

            try
            {
                if (baseUrl.StartsWith("gemini://", StringComparison.OrdinalIgnoreCase))
                {
                    var url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelId}:streamGenerateContent?alt=sse&key={Uri.EscapeDataString(apiKey)}";
                    using var http = new HttpClient();
                    var sb = new StringBuilder();
                    sb.AppendLine(assistant.SystemPrompt);
                    if (!string.IsNullOrWhiteSpace(ragContext)) sb.AppendLine($"\nKontext (aus Wissensbasis):\n{ragContext}");
                    var lastUser = body?.Messages?.LastOrDefault(m => !string.Equals(m?.Role, "assistant", StringComparison.OrdinalIgnoreCase))?.Content ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(lastUser)) sb.AppendLine($"\n{lastUser}");
                    var payload = new
                    {
                        contents = new[]
                        {
                            new { role = "user", parts = new[] { new { text = sb.ToString() } } }
                        }
                    };
                    var json = JsonSerializer.Serialize(payload);
                    using var req = new HttpRequestMessage(HttpMethod.Post, url)
                    {
                        Content = new StringContent(json, Encoding.UTF8, "application/json")
                    };
                    req.Headers.Accept.Clear();
                    req.Headers.Accept.ParseAdd("text/event-stream");
                    var resp = await http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
                    if (!resp.IsSuccessStatusCode)
                    {
                        var respTextErr = await resp.Content.ReadAsStringAsync(ct);
                        _logger.LogError("[AssistantReplyStream] Gemini error: {Status} {Body}", resp.StatusCode, respTextErr);
                        return;
                    }
                    await using var stream = await resp.Content.ReadAsStreamAsync(ct);
                    using var reader = new StreamReader(stream, Encoding.UTF8);
                    var eventBuf = new StringBuilder();
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        if (line is null) break;
                        if (line.StartsWith("event:", StringComparison.OrdinalIgnoreCase)) continue;
                        if (line.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                        {
                            var eventData = line.Substring(5).Trim();
                            if (eventData == "[DONE]") break;
                            eventBuf.Append(eventData);
                        }
                        else if (line.Length == 0)
                        {
                            if (eventBuf.Length > 0)
                            {
                                var jsonLine = eventBuf.ToString();
                                eventBuf.Clear();
                                try
                                {
                                    using var d = JsonDocument.Parse(jsonLine);
                                    var root = d.RootElement;
                                    if (root.TryGetProperty("candidates", out var cands) && cands.ValueKind == JsonValueKind.Array)
                                    {
                                        var cand0 = cands.EnumerateArray().FirstOrDefault();
                                        if (cand0.ValueKind != JsonValueKind.Undefined && cand0.TryGetProperty("content", out var content) && content.TryGetProperty("parts", out var parts) && parts.ValueKind == JsonValueKind.Array)
                                        {
                                            foreach (var p in parts.EnumerateArray())
                                            {
                                                if (p.ValueKind != JsonValueKind.Undefined && p.TryGetProperty("text", out var t))
                                                {
                                                    var delta = t.GetString() ?? string.Empty;
                                                    if (!string.IsNullOrEmpty(delta))
                                                    {
                                                        await Response.WriteAsync(delta, ct);
                                                        await Response.Body.FlushAsync(ct);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
                else if (baseUrl.Contains("api.openai.com", StringComparison.OrdinalIgnoreCase) && modelId.StartsWith("gpt-5", StringComparison.OrdinalIgnoreCase))
                {
                    var url = baseUrl.TrimEnd('/') + "/responses";
                    using var http = new HttpClient();
                    http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                    var sb3 = new StringBuilder();
                    sb3.AppendLine(assistant.SystemPrompt);
                    if (!string.IsNullOrWhiteSpace(ragContext)) sb3.AppendLine($"\nKontext (aus Wissensbasis):\n{ragContext}");
                    var lastUser3 = body?.Messages?.LastOrDefault(m => !string.Equals(m?.Role, "assistant", StringComparison.OrdinalIgnoreCase))?.Content ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(lastUser3)) sb3.AppendLine($"\n{lastUser3}");
                    var payload3 = new
                    {
                        model = modelId,
                        input = sb3.ToString(),
                        reasoning = new { effort = "minimal" },
                        text = new { verbosity = "low" }
                    };
                    var json3 = JsonSerializer.Serialize(payload3);
                    using var req3 = new StringContent(json3, Encoding.UTF8, "application/json");
                    var resp3 = await http.PostAsync(url, req3, ct);
                    var respText3 = await resp3.Content.ReadAsStringAsync(ct);
                    if (!resp3.IsSuccessStatusCode)
                    {
                        _logger.LogError("[AssistantReplyStream] OpenAI Responses error: {Status} {Body}", resp3.StatusCode, respText3);
                        return;
                    }
                    string outText = string.Empty;
                    try
                    {
                        using var doc3 = JsonDocument.Parse(respText3);
                        if (doc3.RootElement.TryGetProperty("output_text", out var ot))
                            outText = ot.GetString() ?? string.Empty;
                        else if (doc3.RootElement.TryGetProperty("output", out var outputArr) && outputArr.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var el in outputArr.EnumerateArray())
                            {
                                if (el.ValueKind != JsonValueKind.Undefined && el.TryGetProperty("content", out var cont) && cont.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var part in cont.EnumerateArray())
                                    {
                                        if (part.ValueKind != JsonValueKind.Undefined && part.TryGetProperty("text", out var t)) outText += t.GetString() ?? string.Empty;
                                    }
                                }
                            }
                        }
                        else if (doc3.RootElement.TryGetProperty("content", out var rootContent) && rootContent.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var part in rootContent.EnumerateArray())
                            {
                                if (part.ValueKind != JsonValueKind.Undefined && part.TryGetProperty("text", out var t)) outText += t.GetString() ?? string.Empty;
                            }
                        }
                        else if (doc3.RootElement.TryGetProperty("response", out var respEl))
                        {
                            if (respEl.TryGetProperty("output_text", out var ot2)) outText = ot2.GetString() ?? string.Empty;
                            else if (respEl.TryGetProperty("content", out var cont2) && cont2.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var p in cont2.EnumerateArray())
                                {
                                    if (p.ValueKind != JsonValueKind.Undefined && p.TryGetProperty("text", out var t)) outText += t.GetString() ?? string.Empty;
                                }
                            }
                        }
                        else if (doc3.RootElement.TryGetProperty("choices", out var choices) && choices.ValueKind == JsonValueKind.Array)
                        {
                            var c = choices.EnumerateArray().FirstOrDefault();
                            if (c.ValueKind != JsonValueKind.Undefined && c.TryGetProperty("message", out var msg) && msg.TryGetProperty("content", out var chText))
                                outText = chText.GetString() ?? string.Empty;
                        }
                    }
                    catch { }
                    if (!string.IsNullOrEmpty(outText))
                    {
                        await Response.WriteAsync(outText, ct);
                        await Response.Body.FlushAsync(ct);
                    }
                }
                else
                {
                    _logger.LogInformation("[AssistantReplyStream] Creating OpenAIClient with endpoint: {Endpoint} and model: {Model}", baseUrl, modelId);
                    var openai = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions { Endpoint = new Uri(baseUrl) });
                    var client = openai.GetChatClient(modelId);
                    ChatCompletionOptions streamOpts = modelId.StartsWith("gpt-5", StringComparison.OrdinalIgnoreCase)
                        ? new ChatCompletionOptions()
                        : new ChatCompletionOptions { Temperature = 0.7f, TopP = 1f };
                    var stream = client.CompleteChatStreamingAsync(messages, streamOpts, cancellationToken: ct);

                    await foreach (var update in stream.WithCancellation(ct))
                    {
                        foreach (var part in update.ContentUpdate)
                        {
                            var delta = part.Text;
                            if (!string.IsNullOrEmpty(delta))
                            {
                                await Response.WriteAsync(delta, ct);
                                await Response.Body.FlushAsync(ct);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Assistant streaming failed for AssistantId={Id}", id);
                if (ex is System.ClientModel.ClientResultException cre)
                {
                    _logger.LogError("[AssistantReplyStream] Kisski API error: Status={Status} Message={Message}", cre.Status, cre.Message);
                }
            }
        }



        public IActionResult Index()
        {

            // Fetch all projects
            var projectList = _unitOfWork.Project.GetAll(includeProperties: "Images")
                                                 .Where(p => !p.IsVirtuellesKlassenzimmer)
                                                 .Where(p => p.IsEnabled)
                                                 .ToList();

            // Fetch all slider items dynamically
            var sliderItems = _unitOfWork.SliderItem
    .GetAll()
    .Where(s => !s.IsForVirtuellesKlassenzimmer)
    .ToList();

            // Fetch all categories, fachgruppen, etc., for mapping
            var categories = _unitOfWork.GetRepository<Category>().GetAll().OrderBy(c => c.DisplayOrder).ToDictionary(c => c.Id, c => c.Name);
            var fachgruppen = _unitOfWork.GetRepository<Fachgruppen>().GetAll().OrderBy(c => c.DisplayOrder).ToDictionary(f => f.Id, f => f.Name);
            var techAnforderungen = _unitOfWork.GetRepository<TechAnforderung>().GetAll().OrderBy(c => c.DisplayOrder).ToDictionary(t => t.Id, t => t.Name);
            var fakultaeten = _unitOfWork.GetRepository<Fakultaet>().GetAll().OrderBy(c => c.DisplayOrder).ToDictionary(f => f.Id, f => f.Name);

            // Populate ViewBag with distinct values for dropdowns
            ViewBag.Categories = categories.Values.ToList();
            ViewBag.Fachgruppen = fachgruppen.Values.ToList();
            ViewBag.Fakultaet = fakultaeten.Values.ToList();
            ViewBag.TechAnforderungen = techAnforderungen.Values.ToList();
            ViewBag.SliderItems = sliderItems; // Pass slider items to the View

            // Map category, fachgruppen, etc., names to each project for display
            foreach (var project in projectList)
            {
                project.CategoryIds = GetNamesFromIds(project.CategoryIds, categories);
                project.FachgruppenIds = GetNamesFromIds(project.FachgruppenIds, fachgruppen);
                project.FakultaetIds = GetNamesFromIds(project.FakultaetIds, fakultaeten);
                project.TechAnforderungIds = GetNamesFromIds(project.TechAnforderungIds, techAnforderungen);
            }

            return View(projectList);
        }



        public IActionResult KIBarDetails(int id)
        {
            var project = _unitOfWork.MakerSpaceProject.Get(u => u.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        public IActionResult Uebersicht()
        {
            var content = _unitOfWork.GetRepository<UebersichtContent>().Get(u => u.Id == 1);
            if (content == null)
            {
                content = new UebersichtContent { ContentHtml = "Kein Inhalt verfügbar." };
            }
            return View(content);
        }


        public IActionResult KIBar()
        {
            var allProjects = _unitOfWork.MakerSpaceProject.GetAll()
                .Select(p => new MakerSpaceProject
                {
                    Id = p.Id,
                    DisplayOrder = p.DisplayOrder,
                    Title = p.Title,
                    Tags = p.Tags,
                    Top = p.Top,
                    download = p.download,
                    lesezeichen = p.lesezeichen,
                    tutorial = p.tutorial,
                    netzwerk = p.netzwerk,
                    events = p.events,
                    Forschung = p.Forschung,
                    ITRecht = p.ITRecht,
                    Beitraege = p.Beitraege,
                    ProjectUrl = p.ProjectUrl,
                    ImageUrl = p.ImageUrl,
                    Description = p.Description
                })
                .ToList();

            var tags = allProjects.SelectMany(p => p.Tags?.Split(',') ?? new string[0])
                                  .Select(t => t.Trim())
                                  .Distinct()
                                  .OrderBy(t => t)
                                  .ToList();

            var description = _unitOfWork.MakerSpaceDescription.GetAll().FirstOrDefault();

            var email = _unitOfWork.GetRepository<ContactEmail>().Get(e => e.Id == 1)?.Email ?? "h.seehagen-marx@uni-wuppertal.de";

            ViewBag.Tags = tags;
            ViewBag.MakerSpaceDescription = description;
            ViewBag.ContactEmail = email;
            return View(allProjects);
        }

        // Areas/User/Controllers/HomeController.cs

        [Authorize]
        public IActionResult Bibliothek(string? group = null, bool mine = false)
        {
            var all = _unitOfWork.GetRepository<PromptTemplate>()
                                  .GetAll()
                                  .ToList();

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("Coach");
            // Prepare group dropdown for everyone: Admin/Coach see all; others see their allowed ones
            {
                List<string> allGroups;
                try
                {
                    if (isAdmin)
                    {
                        // Admin/Coach: show only existing groups from Groups table
                        allGroups = _db.Groups
                                       .AsEnumerable()
                                       .Select(g => g?.Name?.Trim())
                                       .Where(s => !string.IsNullOrWhiteSpace(s))
                                       .Select(s => s!)
                                       .Concat(new[] { "Ohne Gruppe" })
                                       .Distinct(StringComparer.OrdinalIgnoreCase)
                                       .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                                       .ToList();
                    }
                    else
                    {
                        var uid = _userManager.GetUserId(User);
                        var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                        try
                        {
                            var myGroups = _db.UserGroupMemberships
                                              .Where(m => m.UserId == uid)
                                              .Select(m => m.Group)
                                              .AsEnumerable()
                                              .Where(s => !string.IsNullOrWhiteSpace(s))
                                              .Select(s => s!.Trim());
                            foreach (var g in myGroups) set.Add(g);
                        }
                        catch { }
                        if (User.IsInRole("Dozent"))
                        {
                            foreach (var g in _db.DozentGroupOwnerships.Where(o => o.DozentUserId == uid).Select(o => o.Group).AsEnumerable().Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!.Trim())) set.Add(g);
                        }

                        // Intersect with existing Groups to filter out deleted ones
                        HashSet<string> existing;
                        try
                        {
                            existing = _db.Groups.AsEnumerable()
                                        .Select(g => g?.Name?.Trim())
                                        .Where(s => !string.IsNullOrWhiteSpace(s))
                                        .Select(s => s!)
                                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                        }
                        catch { existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase); }

                        allGroups = set.Where(s => existing.Contains(s))
                                       .Distinct(StringComparer.OrdinalIgnoreCase)
                                       .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                                       .ToList();
                    }
                }
                catch { allGroups = new List<string> { "Ohne Gruppe" }; }
                ViewBag.AllGroups = allGroups;
                ViewBag.SelectedGroup = string.IsNullOrWhiteSpace(group) ? null : group;
                ViewBag.MineSelected = mine;
            }
            if (isAdmin)
            {
                var selected = (group ?? string.Empty).Trim();
                IEnumerable<PromptTemplate> list = all;

                if (mine)
                {
                    var currentUserId = _userManager.GetUserId(User);
                    var me = _userManager.Users.FirstOrDefault(u => u.Id == currentUserId);
                    var currentEmail = me?.Email ?? string.Empty;
                    var currentUserName = me?.UserName ?? (User?.Identity?.Name ?? string.Empty);
                    list = list.Where(t =>
                        t.UserId == currentUserId ||
                        (!string.IsNullOrWhiteSpace(t.Autorin) && (
                            t.Autorin.Contains(currentEmail, StringComparison.OrdinalIgnoreCase) ||
                            t.Autorin.Contains(currentUserName, StringComparison.OrdinalIgnoreCase)
                        ))
                    );
                }
                else if (!string.IsNullOrWhiteSpace(selected))
                {
                    // Admin filter: if a specific group is selected, show ONLY prompts explicitly associated to that group
                    HashSet<int> assocIds;
                    try
                    {
                        int? gid = null;
                        try
                        {
                            gid = _db.Groups
                                     .AsEnumerable()
                                     .FirstOrDefault(g => !string.IsNullOrWhiteSpace(g.Name) && string.Equals(g.Name.Trim(), selected, StringComparison.OrdinalIgnoreCase))?.Id;
                        }
                        catch { gid = null; }

                        assocIds = new HashSet<int>(_db.PromptTemplateGroups
                            .AsEnumerable()
                            .Where(pg => (gid != null && pg.GroupId == gid)
                                         || (!string.IsNullOrWhiteSpace(pg.Group) && string.Equals(pg.Group.Trim(), selected, StringComparison.OrdinalIgnoreCase)))
                            .Select(pg => pg.PromptTemplateId));
                    }
                    catch { assocIds = new HashSet<int>(); }

                    if (string.Equals(selected, "Ohne Gruppe", StringComparison.OrdinalIgnoreCase))
                    {
                        // Ohne Gruppe: handle in the dedicated branch below
                    }
                    else
                    {
                        list = list.Where(t => assocIds.Contains(t.Id));
                    }
                }

                var finalList = list.OrderByDescending(t => t.CreatedAt).ToList();

                // Admins: indicator for all prompts that have any active share link
                HashSet<int> sharedPromptIds;
                try
                {
                    var ids = finalList.Select(t => t.Id).ToList();
                    if (ids.Count == 0)
                    {
                        sharedPromptIds = new HashSet<int>();
                    }
                    else
                    {
                        var now = DateTime.UtcNow;
                        sharedPromptIds = _db.PromptShareLinks
                            .Where(l => ids.Contains(l.PromptTemplateId)
                                        && l.IsActive
                                        && (!l.ExpiresAtUtc.HasValue || l.ExpiresAtUtc.Value > now))
                            .Select(l => l.PromptTemplateId)
                            .Distinct()
                            .ToHashSet();
                    }
                }
                catch
                {
                    sharedPromptIds = new HashSet<int>();
                }

                ViewBag.SharedPromptIds = sharedPromptIds;
                return View(finalList);
            }

            var isDozent = User.IsInRole("Dozent");
            var myId = _userManager.GetUserId(User);

            // Determine allowed groups for non-admins (all memberships + owned if Dozent)
            var allowedGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var myGroups = _db.UserGroupMemberships
                                   .Where(m => m.UserId == myId)
                                   .Select(m => m.Group)
                                   .AsEnumerable()
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .Select(s => s!.Trim());
                foreach (var g in myGroups) allowedGroups.Add(g);
            }
            catch { }
            if (allowedGroups.Count == 0) allowedGroups.Add("Ohne Gruppe");

            if (isDozent)
            {
                try
                {
                    var owned = _db.DozentGroupOwnerships
                                   .Where(o => o.DozentUserId == myId)
                                   .Select(o => o.Group)
                                   .AsEnumerable()
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .Select(s => s!.Trim());
                    foreach (var g in owned) allowedGroups.Add(g);
                }
                catch { }
            }

            // Normalize allowed groups for the view and routing
            var allowedGroupList = allowedGroups
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                .ToList();
            ViewBag.AllGroups = allowedGroupList;

            // Non-admins: always land on a concrete group (no aggregated "Alle Gruppen" view)
            if (!mine && string.IsNullOrWhiteSpace(group) && allowedGroupList.Count > 0)
            {
                var first = allowedGroupList.First();
                return RedirectToAction("Bibliothek", new { group = first, mine });
            }
            ViewBag.SelectedGroup = string.IsNullOrWhiteSpace(group) ? null : group;

            // Build latest-group lookup and allowed creator user IDs
            var groupLookup = new Dictionary<string, string?>(StringComparer.Ordinal);
            try
            {
                groupLookup = _db.UserGroupMemberships
                                 .GroupBy(m => m.UserId)
                                 .Select(g => new
                                 {
                                     UserId = g.Key,
                                     Group = g.OrderByDescending(x => x.CreatedAt).Select(x => x.Group).FirstOrDefault()
                                 })
                                 .AsEnumerable()
                                 .ToDictionary(x => x.UserId, x => x.Group);
            }
            catch { }

            var allowedUserIds = new HashSet<string>(StringComparer.Ordinal);
            foreach (var kv in groupLookup)
            {
                var g = string.IsNullOrWhiteSpace(kv.Value) ? "Ohne Gruppe" : kv.Value!.Trim();
                if (allowedGroups.Contains(g)) allowedUserIds.Add(kv.Key);
            }

            // Also include group owners of allowed groups
            try
            {
                var ownerIds = _db.DozentGroupOwnerships
                                  .AsEnumerable()
                                  .Where(o => !string.IsNullOrWhiteSpace(o.Group) && allowedGroups.Contains(o.Group.Trim()))
                                  .Select(o => o.DozentUserId)
                                  .Where(id => !string.IsNullOrWhiteSpace(id));
                foreach (var id in ownerIds) allowedUserIds.Add(id!);
            }
            catch { }

            List<PromptTemplate> filtered;
            if (mine)
            {
                // Show ONLY my prompts regardless of group filters; match by UserId or Autorin contains my username/email
                var me = _userManager.Users.FirstOrDefault(u => u.Id == myId);
                var currentEmail = me?.Email ?? string.Empty;
                var currentUserName = me?.UserName ?? (User?.Identity?.Name ?? string.Empty);
                filtered = all
                    .Where(t =>
                        (t.UserId == myId) ||
                        (!string.IsNullOrWhiteSpace(t.Autorin) && (
                            t.Autorin.Contains(currentEmail, StringComparison.OrdinalIgnoreCase) ||
                            t.Autorin.Contains(currentUserName, StringComparison.OrdinalIgnoreCase)
                        ))
                    )
                    .OrderByDescending(t => t.CreatedAt)
                    .ToList();
            }
            else
            {
                var selected = (group ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(selected) && !string.Equals(selected, "Ohne Gruppe", StringComparison.OrdinalIgnoreCase))
                {
                    // restrict to selected group if it's within allowedGroups
                    if (allowedGroups.Contains(selected))
                    {
                        HashSet<int> assocIds;
                        try
                        {
                            int? gid = null;
                            try
                            {
                                gid = _db.Groups
                                         .AsEnumerable()
                                         .FirstOrDefault(g => !string.IsNullOrWhiteSpace(g.Name) && string.Equals(g.Name.Trim(), selected, StringComparison.OrdinalIgnoreCase))?.Id;
                            }
                            catch { gid = null; }

                            assocIds = new HashSet<int>(_db.PromptTemplateGroups
                                .AsEnumerable()
                                .Where(pg => (gid != null && pg.GroupId == gid)
                                             || (!string.IsNullOrWhiteSpace(pg.Group) && string.Equals(pg.Group.Trim(), selected, StringComparison.OrdinalIgnoreCase)))
                                .Select(pg => pg.PromptTemplateId));
                        }
                        catch { assocIds = new HashSet<int>(); }

                        filtered = all
                            .Where(t => assocIds.Contains(t.Id))
                            .OrderByDescending(t => t.CreatedAt)
                            .ToList();
                    }
                    else
                    {
                        // selected group not allowed → show none
                        filtered = new List<PromptTemplate>();
                    }
                }
                else if (!string.IsNullOrWhiteSpace(selected) && string.Equals(selected, "Ohne Gruppe", StringComparison.OrdinalIgnoreCase))
                {
                    // Ohne Gruppe: no association entry and creators without a group
                    Dictionary<string, string?> latestGroupByUser;
                    try
                    {
                        latestGroupByUser = _db.UserGroupMemberships
                            .GroupBy(m => m.UserId)
                            .Select(g => new { UserId = g.Key, Group = g.OrderByDescending(x => x.CreatedAt).Select(x => x.Group).FirstOrDefault() })
                            .AsEnumerable()
                            .ToDictionary(x => x.UserId, x => x.Group);
                    }
                    catch { latestGroupByUser = new Dictionary<string, string?>(); }
                    var noGroupCreators = latestGroupByUser
                        .Where(kv => string.IsNullOrWhiteSpace(kv.Value))
                        .Select(kv => kv.Key)
                        .ToHashSet(StringComparer.Ordinal);

                    HashSet<int> withAssocIds;
                    try { withAssocIds = new HashSet<int>(_db.PromptTemplateGroups.Select(pg => pg.PromptTemplateId)); }
                    catch { withAssocIds = new HashSet<int>(); }
                    filtered = all
                        .Where(t => !withAssocIds.Contains(t.Id) && (!string.IsNullOrWhiteSpace(t.UserId) && noGroupCreators.Contains(t.UserId)))
                        .OrderByDescending(t => t.CreatedAt)
                        .ToList();
                }
                else
                {
                    // default: any prompt by allowed creators OR associated to any allowed group (via GroupId or legacy Group name)
                    HashSet<int> assocAllowedIds;
                    try
                    {
                        HashSet<int> allowedIds;
                        try
                        {
                            allowedIds = _db.Groups
                                .AsEnumerable()
                                .Where(g => !string.IsNullOrWhiteSpace(g.Name) && allowedGroups.Contains(g.Name.Trim()))
                                .Select(g => g.Id)
                                .ToHashSet();
                        }
                        catch { allowedIds = new HashSet<int>(); }

                        assocAllowedIds = new HashSet<int>(_db.PromptTemplateGroups
                            .AsEnumerable()
                            .Where(pg => (pg.GroupId != null && allowedIds.Contains(pg.GroupId.Value))
                                         || (!string.IsNullOrWhiteSpace(pg.Group) && allowedGroups.Contains(pg.Group.Trim())))
                            .Select(pg => pg.PromptTemplateId));
                    }
                    catch { assocAllowedIds = new HashSet<int>(); }

                    filtered = all
                        .Where(t => assocAllowedIds.Contains(t.Id) || (!string.IsNullOrWhiteSpace(t.UserId) && allowedUserIds.Contains(t.UserId)))
                        .OrderByDescending(t => t.CreatedAt)
                        .ToList();
                }
            }
            // Dozenten: indicator only for prompts they themselves shared
            HashSet<int> sharedPromptIdsNonAdmin = new HashSet<int>();
            if (isDozent)
            {
                try
                {
                    var ids = filtered.Select(t => t.Id).ToList();
                    if (ids.Count > 0)
                    {
                        var now = DateTime.UtcNow;
                        sharedPromptIdsNonAdmin = _db.PromptShareLinks
                            .Where(l => ids.Contains(l.PromptTemplateId)
                                        && l.IsActive
                                        && (!l.ExpiresAtUtc.HasValue || l.ExpiresAtUtc.Value > now)
                                        && l.CreatedByUserId == myId)
                            .Select(l => l.PromptTemplateId)
                            .Distinct()
                            .ToHashSet();
                    }
                }
                catch
                {
                    sharedPromptIdsNonAdmin = new HashSet<int>();
                }
            }

            ViewBag.MineSelected = mine;
            ViewBag.SharedPromptIds = sharedPromptIdsNonAdmin;
            return View(filtered);
        }

        [Authorize]
        public IActionResult Assistenten(string? group = null)
        {
            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var isDozent = User.IsInRole("Dozent");
            var isCoach = User.IsInRole("Coach");

            if (isAdmin)
            {
                var repo = _unitOfWork.GetRepository<Assistant>();
                var all = repo.GetAll().ToList();

                HashSet<int> sharedAssistantIds;
                try
                {
                    var now = DateTime.UtcNow;
                    sharedAssistantIds = _db.AssistantShareLinks
                        .Where(l => l.IsActive && (!l.ExpiresAtUtc.HasValue || l.ExpiresAtUtc.Value > now))
                        .Select(l => l.AssistantId)
                        .Distinct()
                        .ToHashSet();
                }
                catch
                {
                    sharedAssistantIds = new HashSet<int>();
                }

                // Prepare group dropdown (Admins only) from Groups table + "Ohne Gruppe"
                List<string> allGroups;
                try
                {
                    allGroups = _db.Groups
                                   .AsEnumerable()
                                   .Select(g => g?.Name?.Trim())
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .Select(s => s!)
                                   .Concat(new[] { "Ohne Gruppe" })
                                   .Distinct(StringComparer.OrdinalIgnoreCase)
                                   .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                                   .ToList();
                }
                catch { allGroups = new List<string> { "Ohne Gruppe" }; }
                ViewBag.AllGroups = allGroups;
                ViewBag.CanPublishToMultipleGroups = allGroups != null && allGroups.Count > 1;
                ViewBag.SelectedGroup = string.IsNullOrWhiteSpace(group) ? null : group;
                ViewBag.SharedAssistantIds = sharedAssistantIds;

                var selected = (group ?? string.Empty).Trim();
                if (!string.IsNullOrWhiteSpace(selected))
                {
                    if (string.Equals(selected, "Ohne Gruppe", StringComparison.OrdinalIgnoreCase))
                    {
                        // Assistants with no group associations
                        HashSet<int> withGroups;
                        try { withGroups = _db.AssistantGroups.Select(ag => ag.AssistantId).ToHashSet(); }
                        catch { withGroups = new HashSet<int>(); }

                        var filtered = all
                            .Where(a => !withGroups.Contains(a.Id))
                            .OrderByDescending(a => a.CreatedAt)
                            .ToList();
                        return View(filtered);
                    }
                    else
                    {
                        // Assistants associated to the selected group (by GroupId or legacy Group string)
                        int? selectedGroupId = null;
                        try
                        {
                            var grp = _db.Groups
                                         .AsEnumerable()
                                         .FirstOrDefault(g => string.Equals(g?.Name?.Trim(), selected, StringComparison.OrdinalIgnoreCase));
                            if (grp != null) selectedGroupId = grp.Id;
                        }
                        catch { }

                        HashSet<int> matchById;
                        try { matchById = (selectedGroupId == null) ? new HashSet<int>() : _db.AssistantGroups.Where(ag => ag.GroupId == selectedGroupId).Select(ag => ag.AssistantId).ToHashSet(); }
                        catch { matchById = new HashSet<int>(); }

                        HashSet<int> matchByLegacy;
                        try { matchByLegacy = _db.AssistantGroups.Where(ag => ag.Group != null && ag.Group.Trim() == selected).Select(ag => ag.AssistantId).ToHashSet(); }
                        catch { matchByLegacy = new HashSet<int>(); }

                        var allow = new HashSet<int>(matchById);
                        foreach (var id in matchByLegacy) allow.Add(id);

                        var filtered = all
                            .Where(a => allow.Contains(a.Id))
                            .OrderByDescending(a => a.CreatedAt)
                            .ToList();
                        return View(filtered);
                    }
                }

                return View(all.OrderByDescending(a => a.CreatedAt).ToList());
            }

            // Build lookup: latest group per creator
            var latestByCreator = _db.UserGroupMemberships
                .GroupBy(m => m.UserId)
                .Select(g => new { UserId = g.Key, Group = g.OrderByDescending(x => x.CreatedAt).Select(x => x.Group).FirstOrDefault() });

            if (isDozent || isCoach)
            {
                // Dozent sees only assistants published to their owned/member groups, plus their own
                HashSet<string> owned = new(StringComparer.OrdinalIgnoreCase);
                HashSet<string> memberGroups = new(StringComparer.OrdinalIgnoreCase);
                try
                {
                    owned = _db.DozentGroupOwnerships
                               .Where(o => o.DozentUserId == currentUserId)
                               .Select(o => o.Group)
                               .AsEnumerable()
                               .Where(s => !string.IsNullOrWhiteSpace(s))
                               .Select(s => s.Trim())
                               .ToHashSet(StringComparer.OrdinalIgnoreCase);
                    // also allow all of the Dozent's own membership groups
                    var mems = _db.UserGroupMemberships
                                   .Where(m => m.UserId == currentUserId)
                                   .Select(m => m.Group)
                                   .AsEnumerable()
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .Select(s => s!.Trim());
                    foreach (var g in mems) memberGroups.Add(g);
                }
                catch { }

                // Combine owned + member groups
                var allAllowedGroupNames = owned.Concat(memberGroups).Distinct(StringComparer.OrdinalIgnoreCase).ToList();

                // Build groups dropdown (Dozent/Coach): union of owned + memberships, intersect with existing groups
                List<string> existingGroups;
                try { existingGroups = _db.Groups.AsEnumerable().Select(g => g?.Name?.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!).Distinct(StringComparer.OrdinalIgnoreCase).ToList(); }
                catch { existingGroups = new List<string>(); }
                var allowedGroups = allAllowedGroupNames
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim())
                    .Where(s => existingGroups.Contains(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                ViewBag.AllGroups = allowedGroups;
                ViewBag.CanPublishToMultipleGroups = allowedGroups != null && allowedGroups.Count > 1;
                // Non‑admins must always have a concrete group selected (no global "Alle Gruppen" Sicht)
                if (string.IsNullOrWhiteSpace(group) && allowedGroups.Count > 0)
                {
                    var first = allowedGroups.First();
                    return RedirectToAction("Assistenten", new { group = first });
                }

                var selectedNonAdmin = string.IsNullOrWhiteSpace(group) ? null : group?.Trim();
                ViewBag.SelectedGroup = selectedNonAdmin;

                // Get Group IDs for allowed groups
                HashSet<int> allowedGroupIds = new();
                try
                {
                    allowedGroupIds = _db.Groups
                        .AsEnumerable()
                        .Where(g => g?.Name != null && allowedGroups.Contains(g.Name.Trim()))
                        .Select(g => g.Id)
                        .ToHashSet();
                }
                catch { }

                // Get assistant IDs published to allowed groups via AssistantGroups table
                HashSet<int> publishedAssistantIds = new();
                try
                {
                    publishedAssistantIds = _db.AssistantGroups
                        .Where(ag => ag.GroupId != null && allowedGroupIds.Contains(ag.GroupId.Value))
                        .Select(ag => ag.AssistantId)
                        .ToHashSet();
                    // Also check legacy Group string field
                    var legacyIds = _db.AssistantGroups
                        .AsEnumerable()
                        .Where(ag => ag.Group != null && allowedGroups.Contains(ag.Group.Trim()))
                        .Select(ag => ag.AssistantId);
                    foreach (var id in legacyIds) publishedAssistantIds.Add(id);
                }
                catch { }

                // Get all assistants that are published to allowed groups OR created by current user
                var list = _db.Set<Assistant>()
                    .Where(a => a.CreatedByUserId == currentUserId || publishedAssistantIds.Contains(a.Id))
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList();

                // If a specific allowed group is selected, filter assistants by AssistantGroups
                if (!string.IsNullOrWhiteSpace(selectedNonAdmin) && allowedGroups.Contains(selectedNonAdmin))
                {
                    var normGroup = selectedNonAdmin.Trim();
                    // Get GroupId for selected group
                    int? selectedGroupId = null;
                    try
                    {
                        var grp = _db.Groups.AsEnumerable().FirstOrDefault(g => string.Equals(g?.Name?.Trim(), normGroup, StringComparison.OrdinalIgnoreCase));
                        if (grp != null) selectedGroupId = grp.Id;
                    }
                    catch { }

                    // Get assistant IDs published to this specific group
                    HashSet<int> assistantIdsForGroup = new();
                    try
                    {
                        if (selectedGroupId.HasValue)
                        {
                            assistantIdsForGroup = _db.AssistantGroups
                                .Where(ag => ag.GroupId == selectedGroupId.Value)
                                .Select(ag => ag.AssistantId)
                                .ToHashSet();
                        }
                        // Also check legacy Group string field
                        var legacyIds = _db.AssistantGroups
                            .AsEnumerable()
                            .Where(ag => ag.Group != null && string.Equals(ag.Group.Trim(), normGroup, StringComparison.OrdinalIgnoreCase))
                            .Select(ag => ag.AssistantId);
                        foreach (var id in legacyIds) assistantIdsForGroup.Add(id);
                    }
                    catch { }

                    list = list
                        .Where(a => a.CreatedByUserId == currentUserId || assistantIdsForGroup.Contains(a.Id))
                        .OrderByDescending(a => a.CreatedAt)
                        .ToList();
                }

                HashSet<int> sharedAssistantIds = new HashSet<int>();
                if (isDozent)
                {
                    try
                    {
                        var ids = list.Select(a => a.Id).ToList();
                        var now = DateTime.UtcNow;
                        sharedAssistantIds = _db.AssistantShareLinks
                            .Where(l => ids.Contains(l.AssistantId)
                                        && l.IsActive
                                        && (!l.ExpiresAtUtc.HasValue || l.ExpiresAtUtc.Value > now)
                                        && l.CreatedByUserId == currentUserId)
                            .Select(l => l.AssistantId)
                            .Distinct()
                            .ToHashSet();
                    }
                    catch
                    {
                        sharedAssistantIds = new HashSet<int>();
                    }
                }

                ViewBag.SharedAssistantIds = sharedAssistantIds;
                return View(list);
            }
            else
            {
                // Regular users: see assistants published to their groups (+ own)
                HashSet<string> userGroups = new(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var mems = _db.UserGroupMemberships
                                   .Where(m => m.UserId == currentUserId)
                                   .Select(m => m.Group)
                                   .AsEnumerable()
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .Select(s => s!.Trim());
                    foreach (var g in mems) userGroups.Add(g);
                }
                catch { }

                // Build groups dropdown (regular user): own membership groups intersected with existing groups
                List<string> existingGroups2;
                try { existingGroups2 = _db.Groups.AsEnumerable().Select(g => g?.Name?.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!).Distinct(StringComparer.OrdinalIgnoreCase).ToList(); }
                catch { existingGroups2 = new List<string>(); }
                var allowedGroups2 = userGroups
                    .Where(s => existingGroups2.Contains(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                ViewBag.AllGroups = allowedGroups2;
                ViewBag.CanPublishToMultipleGroups = false;
                // Regular users must also always land on a concrete group (if any)
                if (string.IsNullOrWhiteSpace(group) && allowedGroups2.Count > 0)
                {
                    var first = allowedGroups2.First();
                    return RedirectToAction("Assistenten", new { group = first });
                }

                var selectedUser = string.IsNullOrWhiteSpace(group) ? null : group?.Trim();
                ViewBag.SelectedGroup = selectedUser;

                // Get Group IDs for user's groups
                HashSet<int> userGroupIds = new();
                try
                {
                    userGroupIds = _db.Groups
                        .AsEnumerable()
                        .Where(g => g?.Name != null && allowedGroups2.Contains(g.Name.Trim()))
                        .Select(g => g.Id)
                        .ToHashSet();
                }
                catch { }

                // Get assistant IDs published to user's groups via AssistantGroups table
                HashSet<int> publishedAssistantIds = new();
                try
                {
                    publishedAssistantIds = _db.AssistantGroups
                        .Where(ag => ag.GroupId != null && userGroupIds.Contains(ag.GroupId.Value))
                        .Select(ag => ag.AssistantId)
                        .ToHashSet();
                    // Also check legacy Group string field
                    var legacyIds = _db.AssistantGroups
                        .AsEnumerable()
                        .Where(ag => ag.Group != null && allowedGroups2.Contains(ag.Group.Trim()))
                        .Select(ag => ag.AssistantId);
                    foreach (var id in legacyIds) publishedAssistantIds.Add(id);
                }
                catch { }

                // Get all assistants that are published to user's groups OR created by current user
                var list = _db.Set<Assistant>()
                    .Where(a => a.CreatedByUserId == currentUserId || publishedAssistantIds.Contains(a.Id))
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList();

                // If a specific allowed group is selected, filter assistants by AssistantGroups
                if (!string.IsNullOrWhiteSpace(selectedUser) && allowedGroups2.Contains(selectedUser))
                {
                    var normGroup = selectedUser.Trim();
                    // Get GroupId for selected group
                    int? selectedGroupId = null;
                    try
                    {
                        var grp = _db.Groups.AsEnumerable().FirstOrDefault(g => string.Equals(g?.Name?.Trim(), normGroup, StringComparison.OrdinalIgnoreCase));
                        if (grp != null) selectedGroupId = grp.Id;
                    }
                    catch { }

                    // Get assistant IDs published to this specific group
                    HashSet<int> assistantIdsForGroup = new();
                    try
                    {
                        if (selectedGroupId.HasValue)
                        {
                            assistantIdsForGroup = _db.AssistantGroups
                                .Where(ag => ag.GroupId == selectedGroupId.Value)
                                .Select(ag => ag.AssistantId)
                                .ToHashSet();
                        }
                        // Also check legacy Group string field
                        var legacyIds = _db.AssistantGroups
                            .AsEnumerable()
                            .Where(ag => ag.Group != null && string.Equals(ag.Group.Trim(), normGroup, StringComparison.OrdinalIgnoreCase))
                            .Select(ag => ag.AssistantId);
                        foreach (var id in legacyIds) assistantIdsForGroup.Add(id);
                    }
                    catch { }

                    list = list
                        .Where(a => a.CreatedByUserId == currentUserId || assistantIdsForGroup.Contains(a.Id))
                        .OrderByDescending(a => a.CreatedAt)
                        .ToList();
                }

                HashSet<int> sharedAssistantIds = new HashSet<int>();
                ViewBag.SharedAssistantIds = sharedAssistantIds;
                return View(list);
            }
        }

        [Authorize]
        public IActionResult AssistantChat(int id)
        {
            var item = _unitOfWork
                .GetRepository<Assistant>()
                .GetFirstOrDefault(a => a.Id == id);
            if (item == null) return NotFound();

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var isDozent = User.IsInRole("Dozent");
            var isCoach = User.IsInRole("Coach");
            var currentUserId = _userManager.GetUserId(User);

            // Check access: Admin, creator, global assistants, or assistant published to user's groups
            if (!isAdmin && item.CreatedByUserId != currentUserId && !(item?.IsGlobal ?? false))
            {
                // Get groups the assistant is published to
                HashSet<string> assistantGroups = new(StringComparer.OrdinalIgnoreCase);
                HashSet<int> assistantGroupIds = new();
                try
                {
                    var agList = _db.AssistantGroups.Where(ag => ag.AssistantId == id).ToList();
                    foreach (var ag in agList)
                    {
                        if (ag.GroupId.HasValue) assistantGroupIds.Add(ag.GroupId.Value);
                        if (!string.IsNullOrWhiteSpace(ag.Group)) assistantGroups.Add(ag.Group.Trim());
                    }
                }
                catch { }

                // Get user's groups
                HashSet<string> userGroups = new(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var mems = _db.UserGroupMemberships
                                   .Where(m => m.UserId == currentUserId)
                                   .Select(m => m.Group)
                                   .AsEnumerable()
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .Select(s => s!.Trim());
                    foreach (var g in mems) userGroups.Add(g);
                }
                catch { }

                // For Dozent/Coach: also include owned groups
                if (isDozent || isCoach)
                {
                    try
                    {
                        var owned = _db.DozentGroupOwnerships
                                       .Where(o => o.DozentUserId == currentUserId)
                                       .Select(o => o.Group)
                                       .AsEnumerable()
                                       .Where(s => !string.IsNullOrWhiteSpace(s))
                                       .Select(s => s.Trim());
                        foreach (var g in owned) userGroups.Add(g);
                    }
                    catch { }
                }

                // Check if assistant is published to any of user's groups
                bool allowed = false;

                // Check by group name
                foreach (var ug in userGroups)
                {
                    if (assistantGroups.Contains(ug))
                    {
                        allowed = true;
                        break;
                    }
                }

                // Also check by group ID
                if (!allowed && assistantGroupIds.Count > 0)
                {
                    try
                    {
                        var userGroupIds = _db.Groups
                            .AsEnumerable()
                            .Where(g => g?.Name != null && userGroups.Contains(g.Name.Trim()))
                            .Select(g => g.Id)
                            .ToHashSet();
                        allowed = assistantGroupIds.Any(agId => userGroupIds.Contains(agId));
                    }
                    catch { }
                }

                if (!allowed) return Forbid();
            }
            try
            {
                var embCount = _db.Set<AssistantEmbedding>().Count(e => e.AssistantId == id);
                ViewBag.EmbCount = embCount;
                var list = _db.Set<AssistantEmbedding>()
                              .AsQueryable()
                              .Where(e => e.AssistantId == id)
                              .OrderByDescending(e => e.UploadedAtUtc)
                              .ToList();
                ViewBag.Embeddings = list;
            }
            catch { ViewBag.EmbCount = 0; }
            List<string> shareGroups;
            try
            {
                // Start from assistant-specific groups
                shareGroups = _db.AssistantGroups
                    .Where(ag => ag.AssistantId == id)
                    .Select(ag => ag.Group)
                    .AsEnumerable()
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s!.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch
            {
                shareGroups = new List<string>();
            }

            bool canShareAssistant = isAdmin || isDozent;

            if (isAdmin)
            {
                // Admins: allow sharing under any existing group
                try
                {
                    shareGroups = _db.Groups
                        .AsEnumerable()
                        .Select(g => g?.Name?.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
                catch
                {
                    shareGroups = new List<string>();
                }
            }
            else if (isDozent && canShareAssistant)
            {
                // Dozenten: only their owned groups where the assistant is also assigned
                HashSet<string> owned;
                try
                {
                    owned = _db.DozentGroupOwnerships
                        .Where(o => o.DozentUserId == currentUserId)
                        .Select(o => o.Group)
                        .AsEnumerable()
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                }
                catch
                {
                    owned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }

                shareGroups = shareGroups
                    .Where(g => owned.Contains(g))
                    .ToList();
            }

            bool assistantIsShared = false;
            try
            {
                var now = DateTime.UtcNow;
                assistantIsShared = _db.AssistantShareLinks
                    .Any(l => l.AssistantId == id && l.IsActive && (!l.ExpiresAtUtc.HasValue || l.ExpiresAtUtc.Value > now));
            }
            catch { assistantIsShared = false; }

            ViewBag.CanShareAssistant = canShareAssistant && shareGroups.Count > 0;
            ViewBag.AssistantIsShared = assistantIsShared;
            ViewBag.ShareGroups = shareGroups;

            // Determine if user can see meta tabs (Ziele, System-Prompt, Reflektion, Wissensbasis, Besitzer)
            // Only: Admins, Dozenten/Coaches in same group, or assistant owner
            bool canSeeMetaTabs = false;
            if (isAdmin)
            {
                canSeeMetaTabs = true;
            }
            else if (item.CreatedByUserId == currentUserId)
            {
                canSeeMetaTabs = true;
            }
            else if (isDozent || isCoach)
            {
                // Check if Dozent/Coach belongs to any of the assistant's groups
                HashSet<string> dozentGroups = new(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var owned = _db.DozentGroupOwnerships
                                   .Where(o => o.DozentUserId == currentUserId)
                                   .Select(o => o.Group)
                                   .AsEnumerable()
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .Select(s => s!.Trim());
                    foreach (var g in owned) dozentGroups.Add(g);
                }
                catch { }

                // Get assistant's groups
                HashSet<string> asstGroups = new(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var agList = _db.AssistantGroups.Where(ag => ag.AssistantId == id).ToList();
                    foreach (var ag in agList)
                    {
                        if (!string.IsNullOrWhiteSpace(ag.Group)) asstGroups.Add(ag.Group.Trim());
                    }
                }
                catch { }

                // Check if any dozent group matches assistant's groups
                foreach (var dg in dozentGroups)
                {
                    if (asstGroups.Contains(dg))
                    {
                        canSeeMetaTabs = true;
                        break;
                    }
                }
            }
            ViewBag.CanSeeMetaTabs = canSeeMetaTabs;

            return View(item);
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAssistantSystemPrompt(int id, string systemPrompt)
        {
            var repo = _unitOfWork.GetRepository<Assistant>();
            var model = repo.GetFirstOrDefault(a => a.Id == id);
            if (model == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            bool allowed = User.IsInRole(Utility.SD.Role_Admin)
                           || User.IsInRole(Utility.SD.Role_Dozent)
                           || User.IsInRole("Coach")
                           || string.Equals(model.CreatedByUserId, currentUserId, StringComparison.Ordinal);
            if (!allowed) return Forbid();

            var clean = systemPrompt ?? string.Empty;
            clean = Regex.Replace(clean, @"<\s*br\s*/?\s*>", "\n", RegexOptions.IgnoreCase);
            clean = Regex.Replace(clean, "<.*?>", string.Empty, RegexOptions.Singleline);
            clean = WebUtility.HtmlDecode(clean);
            clean = clean.Replace("\r\n", "\n").Replace("\r", "\n");
            model.SystemPrompt = clean.Trim();
            repo.Update(model);
            _unitOfWork.Save();

            return Json(new { ok = true });
        }

        public class ChatMessageDto
        {
            public string Role { get; set; } = "user"; // "user" | "assistant"
            public string Content { get; set; } = string.Empty;
        }

        public class ChatRequestDto
        {
            public List<ChatMessageDto> Messages { get; set; } = new();
        }

        private async Task<string> BuildRagContextAsync(int assistantId, ChatRequestDto body, CancellationToken ct)
        {
            try
            {
                var lastUser = body?.Messages?
                    .LastOrDefault(m => !string.Equals(m?.Role, "assistant", StringComparison.OrdinalIgnoreCase))?
                    .Content ?? string.Empty;
                var query = (lastUser ?? string.Empty).Trim();
                if (string.IsNullOrWhiteSpace(query)) return string.Empty;

                // Load Embeddings API key (always global Admin/ApiKeys OpenAI)
                var apiKey = await _resolver.ResolveEmbeddingsKeyAsync(ct);

                // Embed query
                var embClient = new EmbeddingClient("text-embedding-3-small", apiKey);
                var qResp = await embClient.GenerateEmbeddingAsync(query, cancellationToken: ct);
                float[] qArr = qResp.Value.ToFloats().ToArray();

                // Load assistant embeddings
                var docs = await _db.Set<AssistantEmbedding>()
                                    .AsNoTracking()
                                    .Where(e => e.AssistantId == assistantId)
                                    .Select(e => new { e.SourceFileName, e.Content, e.EmbeddingJson })
                                    .ToListAsync(ct);
                if (docs.Count == 0) return string.Empty;

                static double Cosine(float[] a, float[] b)
                {
                    var len = Math.Min(a.Length, b.Length);
                    double dot = 0, na = 0, nb = 0;
                    for (int i = 0; i < len; i++) { var x = a[i]; var y = b[i]; dot += x * y; na += x * x; nb += y * y; }
                    if (na == 0 || nb == 0) return 0;
                    return dot / (Math.Sqrt(na) * Math.Sqrt(nb));
                }

                var scored = new List<(double score, string name, string content)>(docs.Count);
                foreach (var d in docs)
                {
                    float[]? v = null;
                    try { v = JsonSerializer.Deserialize<float[]>(d.EmbeddingJson); }
                    catch { }
                    if (v == null || v.Length == 0) continue;
                    var s = Cosine(qArr, v);
                    if (!double.IsFinite(s)) s = 0;
                    scored.Add((s, d.SourceFileName ?? "", d.Content ?? ""));
                }

                var top = scored
                    .OrderByDescending(x => x.score)
                    .Take(3)
                    .Where(x => x.score > 0.1)
                    .ToList();
                if (top.Count == 0) return string.Empty;

                var sb = new System.Text.StringBuilder();
                for (int i = 0; i < top.Count; i++)
                {
                    var t = top[i];
                    var snippet = t.content;
                    if (snippet.Length > 2000) snippet = snippet.Substring(0, 2000) + " …";
                    sb.AppendLine($"[Quelle {i + 1} | {t.name}] (score={t.score:0.000})");
                    sb.AppendLine(snippet);
                    if (i < top.Count - 1) sb.AppendLine("---");
                }
                return sb.ToString();
            }
            catch { return string.Empty; }
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AssistantReply(int id, [FromBody] ChatRequestDto body, CancellationToken ct)
        {
            var assistant = _unitOfWork
                .GetRepository<Assistant>()
                .GetFirstOrDefault(a => a.Id == id);
            if (assistant == null) return NotFound();

            // Group-based access control (using AssistantGroups table)
            var isAdmin2 = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            var isDozent2 = User.IsInRole("Dozent");
            var isCoach2 = User.IsInRole("Coach");
            var currentUserId2 = _userManager.GetUserId(User);
            if (!isAdmin2 && assistant.CreatedByUserId != currentUserId2 && !(assistant?.IsGlobal ?? false))
            {
                // Get groups the assistant is published to
                HashSet<string> assistantGroups2 = new(StringComparer.OrdinalIgnoreCase);
                HashSet<int> assistantGroupIds2 = new();
                try
                {
                    var agList2 = _db.AssistantGroups.Where(ag => ag.AssistantId == id).ToList();
                    foreach (var ag in agList2)
                    {
                        if (ag.GroupId.HasValue) assistantGroupIds2.Add(ag.GroupId.Value);
                        if (!string.IsNullOrWhiteSpace(ag.Group)) assistantGroups2.Add(ag.Group.Trim());
                    }
                }
                catch { }

                // Get user's groups
                HashSet<string> userGroups2 = new(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var mems2 = _db.UserGroupMemberships
                                   .Where(m => m.UserId == currentUserId2)
                                   .Select(m => m.Group)
                                   .AsEnumerable()
                                   .Where(s => !string.IsNullOrWhiteSpace(s))
                                   .Select(s => s!.Trim());
                    foreach (var g in mems2) userGroups2.Add(g);
                }
                catch { }

                // For Dozent/Coach: also include owned groups
                if (isDozent2 || isCoach2)
                {
                    try
                    {
                        var owned2 = _db.DozentGroupOwnerships
                                       .Where(o => o.DozentUserId == currentUserId2)
                                       .Select(o => o.Group)
                                       .AsEnumerable()
                                       .Where(s => !string.IsNullOrWhiteSpace(s))
                                       .Select(s => s.Trim());
                        foreach (var g in owned2) userGroups2.Add(g);
                    }
                    catch { }
                }

                // Check if assistant is published to any of user's groups
                bool allowed2 = false;

                // Check by group name
                foreach (var ug in userGroups2)
                {
                    if (assistantGroups2.Contains(ug))
                    {
                        allowed2 = true;
                        break;
                    }
                }

                // Also check by group ID
                if (!allowed2 && assistantGroupIds2.Count > 0)
                {
                    try
                    {
                        var userGroupIds2 = _db.Groups
                            .AsEnumerable()
                            .Where(g => g?.Name != null && userGroups2.Contains(g.Name.Trim()))
                            .Select(g => g.Id)
                            .ToHashSet();
                        allowed2 = assistantGroupIds2.Any(agId => userGroupIds2.Contains(agId));
                    }
                    catch { }
                }

                if (!allowed2) return Forbid();
            }

            // Resolve provider via centralized resolver
            string baseUrl;
            string apiKey;
            string modelId;
            try
            {
                // Admins/SuperAdmins: bypass group-level gating and use global API configuration
                var resolverUserId2 = isAdmin2 ? null : _userManager.GetUserId(User);
                var resolved2 = await _resolver.ResolveChatAsync(resolverUserId2, null, ct);
                baseUrl = resolved2.BaseUrl;
                apiKey = resolved2.ApiKey;
                modelId = resolved2.ModelId;
            }
            catch
            {
                return StatusCode(500, new { error = "API-Konfiguration fehlt (Resolver)." });
            }
            if (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(apiKey))
                return StatusCode(500, new { error = "API-Konfiguration fehlt (BaseUrl, ApiKey)." });
            if (baseUrl.Contains("api.openai.com", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(modelId, "gpt-5-instant", StringComparison.OrdinalIgnoreCase)) modelId = "gpt-5";
                else if (string.Equals(modelId, "4o-mini", StringComparison.OrdinalIgnoreCase)) modelId = "gpt-4o-mini";
                else if (string.Equals(modelId, "4o", StringComparison.OrdinalIgnoreCase)) modelId = "gpt-4o";
                if (modelId.StartsWith("openai-", StringComparison.OrdinalIgnoreCase)) modelId = _cfg["OpenAI:ModelId"] ?? "gpt-4o-mini";
            }
            // Guard: medgemma on Kisski not supported for chat assistant
            if (baseUrl.Contains("chat-ai.academiccloud.de", StringComparison.OrdinalIgnoreCase) && modelId.Contains("medgemma", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(400, new { error = "Derzeit ist der Chat‑Assistent nicht mit Kisski (medgemma) kompatibel. Bitte wählen Sie ein anderes Modell bzw. einen anderen Anbieter." });
            }
            _logger.LogInformation("[AssistantReply] Using endpoint: {Endpoint} with model: {Model}", baseUrl, modelId);

            var ragContext = await BuildRagContextAsync(id, body, ct);
            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(assistant.SystemPrompt)
            };
            if (!string.IsNullOrWhiteSpace(ragContext))
            {
                messages.Add(new SystemChatMessage($"Kontext (aus Wissensbasis):\n{ragContext}"));
            }

            if (body?.Messages != null)
            {
                foreach (var m in body.Messages)
                {
                    var content = m?.Content ?? string.Empty;
                    if (string.Equals(m?.Role, "assistant", StringComparison.OrdinalIgnoreCase))
                        messages.Add(new AssistantChatMessage(content));
                    else
                        messages.Add(new UserChatMessage(content));
                }
            }

            try
            {
                if (baseUrl.StartsWith("gemini://", StringComparison.OrdinalIgnoreCase))
                {
                    var url = $"https://generativelanguage.googleapis.com/v1beta/models/{modelId}:generateContent?key={Uri.EscapeDataString(apiKey)}";
                    using var http = new HttpClient();
                    var sb2 = new StringBuilder();
                    sb2.AppendLine(assistant.SystemPrompt);
                    if (!string.IsNullOrWhiteSpace(ragContext)) sb2.AppendLine($"\nKontext (aus Wissensbasis):\n{ragContext}");
                    var lastUser2 = body?.Messages?.LastOrDefault(m => !string.Equals(m?.Role, "assistant", StringComparison.OrdinalIgnoreCase))?.Content ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(lastUser2)) sb2.AppendLine($"\n{lastUser2}");
                    var payload2 = new
                    {
                        contents = new[]
                        {
                            new { role = "user", parts = new[] { new { text = sb2.ToString() } } }
                        }
                    };
                    var json2 = JsonSerializer.Serialize(payload2);
                    using var req2 = new StringContent(json2, Encoding.UTF8, "application/json");
                    var resp2 = await http.PostAsync(url, req2, ct);
                    var respText2 = await resp2.Content.ReadAsStringAsync(ct);
                    if (!resp2.IsSuccessStatusCode)
                    {
                        _logger.LogError("[AssistantReply] Gemini error: {Status} {Body}", resp2.StatusCode, respText2);
                        return StatusCode(500, new { error = "Gemini request failed" });
                    }
                    using var doc2 = JsonDocument.Parse(respText2);
                    var text2 = doc2.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString() ?? string.Empty;

                    // Analytics: assistant chat (Gemini)
                    try
                    {
                        if (ProjectsWebApp.Utility.FeatureFlags.EnableAnalytics)
                        {
                            string? userGroup = null;
                            try
                            {
                                userGroup = _db.UserGroupMemberships
                                               .Where(m => m.UserId == currentUserId2)
                                               .OrderByDescending(m => m.CreatedAt)
                                               .Select(m => m.Group)
                                               .FirstOrDefault();
                            }
                            catch { }

                            await _activityLogger.LogAsync(
                                currentUserId2 ?? string.Empty,
                                string.IsNullOrWhiteSpace(userGroup) ? null : (userGroup!.Trim()),
                                "assistant_chat",
                                null,
                                new { assistantId = id, provider = "gemini", modelId },
                                ct);
                        }
                    }
                    catch { }

                    return Json(new { reply = text2 });
                }
                else if (baseUrl.Contains("api.openai.com", StringComparison.OrdinalIgnoreCase) && modelId.StartsWith("gpt-5", StringComparison.OrdinalIgnoreCase))
                {
                    var url = baseUrl.TrimEnd('/') + "/responses";
                    using var http = new HttpClient();
                    http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
                    var sb3 = new StringBuilder();
                    sb3.AppendLine(assistant.SystemPrompt);
                    if (!string.IsNullOrWhiteSpace(ragContext)) sb3.AppendLine($"\nKontext (aus Wissensbasis):\n{ragContext}");
                    var lastUser3 = body?.Messages?.LastOrDefault(m => !string.Equals(m?.Role, "assistant", StringComparison.OrdinalIgnoreCase))?.Content ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(lastUser3)) sb3.AppendLine($"\n{lastUser3}");
                    var payload3 = new
                    {
                        model = modelId,
                        input = sb3.ToString(),
                        reasoning = new { effort = "minimal" },
                        stream = true,
                        text = new { verbosity = "low" }
                    };
                    var json3 = JsonSerializer.Serialize(payload3);
                    using var req3 = new StringContent(json3, Encoding.UTF8, "application/json");
                    var resp3 = await http.PostAsync(url, req3, ct);
                    var respText3 = await resp3.Content.ReadAsStringAsync(ct);
                    if (!resp3.IsSuccessStatusCode)
                    {
                        _logger.LogError("[AssistantReply] OpenAI Responses error: {Status} {Body}", resp3.StatusCode, respText3);
                        return StatusCode(500, new { error = "OpenAI Responses request failed" });
                    }
                    string outText = string.Empty;
                    try
                    {
                        using var doc3 = JsonDocument.Parse(respText3);
                        if (doc3.RootElement.TryGetProperty("output_text", out var ot))
                            outText = ot.GetString() ?? string.Empty;
                        else if (doc3.RootElement.TryGetProperty("output", out var outputArr) && outputArr.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var el in outputArr.EnumerateArray())
                            {
                                if (el.ValueKind != JsonValueKind.Undefined && el.TryGetProperty("content", out var cont) && cont.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var part in cont.EnumerateArray())
                                    {
                                        if (part.ValueKind != JsonValueKind.Undefined && part.TryGetProperty("text", out var t)) outText += t.GetString() ?? string.Empty;
                                    }
                                }
                            }
                        }
                        else if (doc3.RootElement.TryGetProperty("content", out var rootContent) && rootContent.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var part in rootContent.EnumerateArray())
                            {
                                if (part.ValueKind != JsonValueKind.Undefined && part.TryGetProperty("text", out var t)) outText += t.GetString() ?? string.Empty;
                            }
                        }
                        else if (doc3.RootElement.TryGetProperty("response", out var respEl))
                        {
                            if (respEl.TryGetProperty("output_text", out var ot2)) outText = ot2.GetString() ?? string.Empty;
                            else if (respEl.TryGetProperty("content", out var cont2) && cont2.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var p in cont2.EnumerateArray())
                                {
                                    if (p.ValueKind != JsonValueKind.Undefined && p.TryGetProperty("text", out var t)) outText += t.GetString() ?? string.Empty;
                                }
                            }
                        }
                        else if (doc3.RootElement.TryGetProperty("choices", out var choices) && choices.ValueKind == JsonValueKind.Array)
                        {
                            var c = choices.EnumerateArray().FirstOrDefault();
                            if (c.ValueKind != JsonValueKind.Undefined && c.TryGetProperty("message", out var msg) && msg.TryGetProperty("content", out var chText))
                                outText = chText.GetString() ?? string.Empty;
                        }
                    }
                    catch { }
                    return Json(new { reply = outText });
                }
                else
                {
                    _logger.LogInformation("[AssistantReply] Creating OpenAIClient with endpoint: {Endpoint} and model: {Model}", baseUrl, modelId);
                    OpenAIClient openai;
                    if (baseUrl.Contains("chat-ai.academiccloud.de", StringComparison.OrdinalIgnoreCase))
                        openai = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions { Endpoint = new Uri(baseUrl) });
                    else
                        openai = new OpenAIClient(new ApiKeyCredential(apiKey));
                    var client = openai.GetChatClient(modelId);
                    ChatCompletionOptions opts = modelId.StartsWith("gpt-5", StringComparison.OrdinalIgnoreCase)
                        ? new ChatCompletionOptions()
                        : new ChatCompletionOptions { Temperature = 0.7f, TopP = 1 };
                    try
                    {
                        try
                        {
                            var result = await client.CompleteChatAsync(messages, opts, cancellationToken: ct);
                            var text = result.Value?.Content?.FirstOrDefault()?.Text ?? string.Empty;
                            return Json(new { reply = text });
                        }
                        catch (System.ClientModel.ClientResultException cre) when (cre.Status == 404 && baseUrl.Contains("chat-ai.academiccloud.de", StringComparison.OrdinalIgnoreCase))
                        {
                            var altBase = baseUrl.EndsWith("/v1", StringComparison.OrdinalIgnoreCase)
                                ? baseUrl.Substring(0, baseUrl.Length - 3)
                                : (baseUrl.TrimEnd('/') + "/v1");
                            OpenAIClient altOpenai = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions { Endpoint = new Uri(altBase) });
                            var altClient = altOpenai.GetChatClient(modelId);
                            var result = await altClient.CompleteChatAsync(messages, opts, cancellationToken: ct);
                            var text = result.Value?.Content?.FirstOrDefault()?.Text ?? string.Empty;
                            return Json(new { reply = text });
                        }
                    }
                    catch (System.ClientModel.ClientResultException cre) when (cre.Status == 404 && baseUrl.Contains("chat-ai.academiccloud.de", StringComparison.OrdinalIgnoreCase))
                    {
                        var altBase = baseUrl.EndsWith("/v1", StringComparison.OrdinalIgnoreCase)
                            ? baseUrl.Substring(0, baseUrl.Length - 3)
                            : (baseUrl.TrimEnd('/') + "/v1");
                        OpenAIClient altOpenai = new OpenAIClient(new ApiKeyCredential(apiKey), new OpenAIClientOptions { Endpoint = new Uri(altBase) });
                        var altClient = altOpenai.GetChatClient(modelId);
                        var result = await altClient.CompleteChatAsync(messages, opts, cancellationToken: ct);
                        var text = result.Value?.Content?.FirstOrDefault()?.Text ?? string.Empty;

                        // Analytics: assistant chat (non-Gemini / non-Responses)
                        try
                        {
                            if (ProjectsWebApp.Utility.FeatureFlags.EnableAnalytics)
                            {
                                string? userGroup = null;
                                try
                                {
                                    userGroup = _db.UserGroupMemberships
                                           .Where(m => m.UserId == currentUserId2)
                                           .OrderByDescending(m => m.CreatedAt)
                                           .Select(m => m.Group)
                                           .FirstOrDefault();
                                }
                                catch { }

                                await _activityLogger.LogAsync(
                                    currentUserId2 ?? string.Empty,
                                    string.IsNullOrWhiteSpace(userGroup) ? null : (userGroup!.Trim()),
                                    "assistant_chat",
                                    null,
                                    new { assistantId = id, provider = baseUrl, modelId },
                                    ct);
                            }
                        }
                        catch { }

                        return Json(new { reply = text });
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Assistant chat failed for AssistantId={Id}", id);
                if (ex is System.ClientModel.ClientResultException cre)
                {
                    _logger.LogError("[AssistantReply] Kisski API error: Status={Status} Message={Message}", cre.Status, cre.Message);
                }
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssistant(string name, string? description, string? avatarUrl, IFormFile? avatarFile, string systemPrompt, string? goals, string? licenses, string? authorName, string? reflektion, string[]? groups)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                TempData["error"] = "Name ist erforderlich.";
                return RedirectToAction(nameof(PromptAssistent));
            }
            if (string.IsNullOrWhiteSpace(systemPrompt))
            {
                TempData["error"] = "System Prompt ist leer.";
                return RedirectToAction(nameof(PromptAssistent));
            }

            string? finalAvatar = null;
            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "assistants");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(avatarFile.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                await using var stream = System.IO.File.Create(filePath);
                await avatarFile.CopyToAsync(stream);
                finalAvatar = $"~/uploads/assistants/{fileName}";
            }
            else if (!string.IsNullOrWhiteSpace(avatarUrl))
            {
                var av = avatarUrl.Trim();
                if (!av.StartsWith("http", StringComparison.OrdinalIgnoreCase) && av.StartsWith("/"))
                    av = "~" + av;
                finalAvatar = av;
            }

            // Ensure default avatar if none provided
            if (string.IsNullOrWhiteSpace(finalAvatar))
            {
                finalAvatar = "~/images/Assistant_default.png";
            }

            var model = new Assistant
            {
                Name = name.Trim(),
                Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
                AvatarUrl = finalAvatar,
                SystemPrompt = systemPrompt,
                Goals = string.IsNullOrWhiteSpace(goals) ? null : goals.Trim(),
                Licenses = string.IsNullOrWhiteSpace(licenses) ? null : licenses.Trim(),
                AuthorName = string.IsNullOrWhiteSpace(authorName) ? null : authorName.Trim(),
                Reflektion = string.IsNullOrWhiteSpace(reflektion) ? null : reflektion.Trim(),
                CreatedByUserId = _userManager.GetUserId(User),
                CreatedAt = DateTime.UtcNow
            };

            _unitOfWork.GetRepository<Assistant>().Add(model);
            _unitOfWork.Save();

            // Save Assistant ↔ Groups associations (optional)
            try
            {
                var selected = (groups ?? Array.Empty<string>())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                // Build allowed groups (reuse logic similar to AllowedGroups API)
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("Coach");
                HashSet<string> existing;
                try { existing = _db.Groups.AsEnumerable().Select(g => g?.Name?.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!).ToHashSet(StringComparer.OrdinalIgnoreCase); }
                catch { existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase); }

                HashSet<string> allowed;
                if (isAdmin)
                {
                    allowed = existing;
                }
                else
                {
                    var uid = _userManager.GetUserId(User);
                    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    try
                    {
                        var myGroup = _db.UserGroupMemberships
                            .Where(m => m.UserId == uid)
                            .OrderByDescending(m => m.CreatedAt)
                            .Select(m => m.Group)
                            .FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(myGroup)) set.Add(myGroup.Trim());
                    }
                    catch { }
                    if (User.IsInRole("Dozent"))
                    {
                        try
                        {
                            foreach (var g in _db.DozentGroupOwnerships.Where(o => o.DozentUserId == uid).Select(o => o.Group).AsEnumerable().Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!.Trim())) set.Add(g);
                        }
                        catch { }
                    }
                    allowed = set.Where(s => existing.Contains(s)).ToHashSet(StringComparer.OrdinalIgnoreCase);
                }

                var final = selected.Where(s => allowed.Contains(s)).ToList();
                if (final.Count > 0)
                {
                    var map = _db.Groups.AsEnumerable().Where(g => g?.Name != null)
                        .ToDictionary(g => g!.Name!.Trim(), g => g, StringComparer.OrdinalIgnoreCase);
                    foreach (var name2 in final)
                    {
                        if (!map.TryGetValue(name2, out var row)) continue; // only existing groups
                        _db.AssistantGroups.Add(new AssistantGroup { AssistantId = model.Id, GroupId = row.Id, Group = name2 });
                    }
                    await _db.SaveChangesAsync();
                }
            }
            catch { }

            TempData["success"] = "Assistent gespeichert.";
            return RedirectToAction(nameof(Assistenten));
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAssistant(int id, string name, string? description, string? avatarUrl, IFormFile? avatarFile, string systemPrompt, string? goals, string? licenses, string? authorName, string? reflektion, string[]? groups)
        {
            var asstRepo = _unitOfWork.GetRepository<Assistant>();
            var model = asstRepo.GetFirstOrDefault(a => a.Id == id);
            if (model == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(name)) model.Name = name.Trim();
            model.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            model.SystemPrompt = systemPrompt;
            model.Goals = string.IsNullOrWhiteSpace(goals) ? null : goals.Trim();
            model.Licenses = string.IsNullOrWhiteSpace(licenses) ? null : licenses.Trim();
            model.AuthorName = string.IsNullOrWhiteSpace(authorName) ? null : authorName.Trim();
            model.Reflektion = string.IsNullOrWhiteSpace(reflektion) ? null : reflektion.Trim();

            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "assistants");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(avatarFile.FileName)}";
                var filePath = Path.Combine(uploads, fileName);
                await using var stream = System.IO.File.Create(filePath);
                await avatarFile.CopyToAsync(stream);
                model.AvatarUrl = $"~/uploads/assistants/{fileName}";
            }
            else if (!string.IsNullOrWhiteSpace(avatarUrl))
            {
                var av = avatarUrl.Trim();
                if (!av.StartsWith("http", StringComparison.OrdinalIgnoreCase) && av.StartsWith("/"))
                    av = "~" + av;
                model.AvatarUrl = av;
            }

            asstRepo.Update(model);
            _unitOfWork.Save();

            // Replace Assistant ↔ Groups associations based on posted groups
            try
            {
                var existingLinks = _db.AssistantGroups.Where(x => x.AssistantId == model.Id);
                _db.AssistantGroups.RemoveRange(existingLinks);
                await _db.SaveChangesAsync();

                var selected = (groups ?? Array.Empty<string>())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("Coach");
                HashSet<string> existing;
                try { existing = _db.Groups.AsEnumerable().Select(g => g?.Name?.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!).ToHashSet(StringComparer.OrdinalIgnoreCase); }
                catch { existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase); }
                HashSet<string> allowed;
                if (isAdmin) allowed = existing;
                else
                {
                    var uid = _userManager.GetUserId(User);
                    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    try
                    {
                        var myGroup = _db.UserGroupMemberships
                            .Where(m => m.UserId == uid)
                            .OrderByDescending(m => m.CreatedAt)
                            .Select(m => m.Group)
                            .FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(myGroup)) set.Add(myGroup.Trim());
                    }
                    catch { }
                    if (User.IsInRole("Dozent"))
                    {
                        try { foreach (var g in _db.DozentGroupOwnerships.Where(o => o.DozentUserId == uid).Select(o => o.Group).AsEnumerable().Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s!.Trim())) set.Add(g); }
                        catch { }
                    }
                    allowed = set.Where(s => existing.Contains(s)).ToHashSet(StringComparer.OrdinalIgnoreCase);
                }

                var final = selected.Where(s => allowed.Contains(s)).ToList();
                if (final.Count > 0)
                {
                    var map = _db.Groups.AsEnumerable().Where(g => g?.Name != null)
                        .ToDictionary(g => g!.Name!.Trim(), g => g, StringComparer.OrdinalIgnoreCase);
                    foreach (var name2 in final)
                    {
                        if (!map.TryGetValue(name2, out var row)) continue;
                        _db.AssistantGroups.Add(new AssistantGroup { AssistantId = model.Id, GroupId = row.Id, Group = name2 });
                    }
                    await _db.SaveChangesAsync();
                }
            }
            catch { }

            TempData["success"] = "Assistent aktualisiert.";
            return RedirectToAction(nameof(Assistenten));
        }

        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAssistantShareLink(int id, string groupName, CancellationToken ct)
        {
            var assistantRepo = _unitOfWork.GetRepository<Assistant>();
            var assistant = assistantRepo.GetFirstOrDefault(a => a.Id == id);
            if (assistant == null) return NotFound();

            if (string.IsNullOrWhiteSpace(groupName))
            {
                Response.StatusCode = 400;
                return Json(new { ok = false, error = "Gruppe ist erforderlich." });
            }

            var normGroup = groupName.Trim();

            var isAdmin = User.IsInRole(SD.Role_Admin) || User.IsInRole("SuperAdmin");
            var isDozent = User.IsInRole(SD.Role_Dozent);
            var currentUserId = _userManager.GetUserId(User);

            if (!isAdmin && !isDozent)
            {
                return Forbid();
            }

            if (isDozent)
            {
                // Dozenten: nur eigene Gruppen und nur, wenn ein eigener Gruppen-API-Key aktiv ist
                HashSet<string> owned;
                try
                {
                    owned = _db.DozentGroupOwnerships
                        .Where(o => o.DozentUserId == currentUserId)
                        .Select(o => o.Group)
                        .AsEnumerable()
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                }
                catch
                {
                    owned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }

                List<string> assistantGroups;
                try
                {
                    assistantGroups = _db.AssistantGroups
                        .Where(x => x.AssistantId == id)
                        .Select(x => x.Group)
                        .AsEnumerable()
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
                catch
                {
                    assistantGroups = new List<string>();
                }

                var allowedGroups = assistantGroups
                    .Where(g => owned.Contains(g))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (!allowedGroups.Contains(normGroup))
                {
                    Response.StatusCode = 400;
                    return Json(new { ok = false, error = "Sie können diesen Assistenten nur für Ihre eigenen Gruppen freigeben." });
                }

                // Prüfen, ob die Gruppe einen eigenen API-Key hat (nicht nur globale Konfiguration)
                bool hasValidGroupKey = false;
                bool hasPrivilegedOwner = false;
                try
                {
                    var gRec = await _db.Set<ProjectsWebApp.Models.GroupApiKeySetting>()
                        .Where(g => g.Group == normGroup)
                        .OrderByDescending(g => g.UpdatedAt)
                        .FirstOrDefaultAsync(ct);

                    string provider = gRec?.ActiveProvider?.Trim().ToLowerInvariant() ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(provider))
                    {
                        if (provider == "openai" && !string.IsNullOrWhiteSpace(gRec!.OpenAIKey))
                            hasValidGroupKey = true;
                        else if (provider == "kisski" && !string.IsNullOrWhiteSpace(gRec!.KisskiApiKey))
                            hasValidGroupKey = true;
                        else if (provider == "gemini" && !string.IsNullOrWhiteSpace(gRec!.GeminiApiKey))
                            hasValidGroupKey = true;
                    }

                    // Prüfen, ob ein ApiManager die Gruppe besitzt (würde globale Keys erlauben)
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
                catch
                {
                    hasValidGroupKey = false;
                    hasPrivilegedOwner = false;
                }

                if (hasPrivilegedOwner)
                {
                    Response.StatusCode = 400;
                    return Json(new { ok = false, error = "Für den Freigabelink müssen Sie Ihren privaten API-Schlüssel aktivieren." });
                }

                if (!hasValidGroupKey)
                {
                    Response.StatusCode = 400;
                    return Json(new { ok = false, error = "Für diese Gruppe ist kein gültiger Gruppen‑API‑Key konfiguriert." });
                }
            }

            AssistantShareLink? link = null;
            try
            {
                link = await _db.AssistantShareLinks
                    .Where(l => l.AssistantId == id && l.Group == normGroup)
                    .OrderByDescending(l => l.CreatedAtUtc)
                    .FirstOrDefaultAsync(ct);
            }
            catch { }

            if (link == null)
            {
                link = new AssistantShareLink
                {
                    AssistantId = id,
                    Group = normGroup,
                    PublicId = Guid.NewGuid().ToString("N"),
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedByUserId = currentUserId
                };
                _db.AssistantShareLinks.Add(link);
            }
            else
            {
                link.IsActive = true;
                if (link.ExpiresAtUtc.HasValue && link.ExpiresAtUtc.Value <= DateTime.UtcNow)
                    link.ExpiresAtUtc = null;
            }

            await _db.SaveChangesAsync(ct);

            string? absoluteUrl = null;
            try
            {
                absoluteUrl = Url.Action(
                    "Assistant",
                    "SharedAssistant",
                    new { area = "User", id = link.PublicId },
                    Request.Scheme,
                    Request.Host.ToString());
            }
            catch { }

            return Json(new
            {
                ok = true,
                shareId = link.Id,
                token = link.PublicId,
                url = absoluteUrl ?? ($"/User/SharedAssistant/Assistant/{link.PublicId}")
            });
        }

        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        [HttpGet]
        public async Task<IActionResult> GetAssistantShareState(int id, string groupName, CancellationToken ct)
        {
            var assistantRepo = _unitOfWork.GetRepository<Assistant>();
            var assistant = assistantRepo.GetFirstOrDefault(a => a.Id == id);
            if (assistant == null) return NotFound();

            if (string.IsNullOrWhiteSpace(groupName))
            {
                Response.StatusCode = 400;
                return Json(new { ok = false, error = "Gruppe ist erforderlich." });
            }

            var normGroup = groupName.Trim();

            var isAdmin = User.IsInRole(SD.Role_Admin) || User.IsInRole("SuperAdmin");
            var isDozent = User.IsInRole(SD.Role_Dozent);
            var currentUserId = _userManager.GetUserId(User);

            if (!isAdmin && !isDozent)
            {
                return Forbid();
            }

            if (isDozent)
            {
                HashSet<string> owned;
                try
                {
                    owned = _db.DozentGroupOwnerships
                        .Where(o => o.DozentUserId == currentUserId)
                        .Select(o => o.Group)
                        .AsEnumerable()
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                }
                catch
                {
                    owned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }

                List<string> assistantGroups;
                try
                {
                    assistantGroups = _db.AssistantGroups
                        .Where(x => x.AssistantId == id)
                        .Select(x => x.Group)
                        .AsEnumerable()
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
                catch
                {
                    assistantGroups = new List<string>();
                }

                var allowedGroups = assistantGroups
                    .Where(g => owned.Contains(g))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (!allowedGroups.Contains(normGroup))
                {
                    Response.StatusCode = 400;
                    return Json(new { ok = false, error = "Sie können diesen Assistenten nur für Ihre eigenen Gruppen freigeben." });
                }
            }

            AssistantShareLink? link = null;
            try
            {
                link = await _db.AssistantShareLinks
                    .Where(l => l.AssistantId == id && l.Group == normGroup)
                    .OrderByDescending(l => l.CreatedAtUtc)
                    .FirstOrDefaultAsync(ct);
            }
            catch { }

            if (link == null)
            {
                return Json(new { ok = true, hasLink = false, isActive = false, url = string.Empty });
            }

            var isActive = link.IsActive && (!link.ExpiresAtUtc.HasValue || link.ExpiresAtUtc.Value > DateTime.UtcNow);

            string? absoluteUrl = null;
            if (isActive)
            {
                try
                {
                    absoluteUrl = Url.Action(
                        "Assistant",
                        "SharedAssistant",
                        new { area = "User", id = link.PublicId },
                        Request.Scheme,
                        Request.Host.ToString());
                }
                catch { }
            }

            return Json(new
            {
                ok = true,
                hasLink = true,
                isActive,
                url = isActive ? (absoluteUrl ?? ($"/User/SharedAssistant/Assistant/{link.PublicId}")) : string.Empty
            });
        }

        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateAssistantShareLink(int id, string groupName, CancellationToken ct)
        {
            var assistantRepo = _unitOfWork.GetRepository<Assistant>();
            var assistant = assistantRepo.GetFirstOrDefault(a => a.Id == id);
            if (assistant == null) return NotFound();

            if (string.IsNullOrWhiteSpace(groupName))
            {
                Response.StatusCode = 400;
                return Json(new { ok = false, error = "Gruppe ist erforderlich." });
            }

            var normGroup = groupName.Trim();

            var isAdmin = User.IsInRole(SD.Role_Admin) || User.IsInRole("SuperAdmin");
            var isDozent = User.IsInRole(SD.Role_Dozent);
            var currentUserId = _userManager.GetUserId(User);

            if (!isAdmin && !isDozent)
            {
                return Forbid();
            }

            if (isDozent)
            {
                HashSet<string> owned;
                try
                {
                    owned = _db.DozentGroupOwnerships
                        .Where(o => o.DozentUserId == currentUserId)
                        .Select(o => o.Group)
                        .AsEnumerable()
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!.Trim())
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                }
                catch
                {
                    owned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                }

                List<string> assistantGroups;
                try
                {
                    assistantGroups = _db.AssistantGroups
                        .Where(x => x.AssistantId == id)
                        .Select(x => x.Group)
                        .AsEnumerable()
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
                catch
                {
                    assistantGroups = new List<string>();
                }

                var allowedGroups = assistantGroups
                    .Where(g => owned.Contains(g))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (!allowedGroups.Contains(normGroup))
                {
                    Response.StatusCode = 400;
                    return Json(new { ok = false, error = "Sie können diesen Assistenten nur für Ihre eigenen Gruppen freigeben." });
                }
            }

            List<AssistantShareLink> links;
            try
            {
                links = await _db.AssistantShareLinks
                    .Where(l => l.AssistantId == id && l.Group == normGroup)
                    .ToListAsync(ct);
            }
            catch
            {
                Response.StatusCode = 500;
                return Json(new { ok = false, error = "Freigabelink konnte nicht deaktiviert werden." });
            }

            if (links == null || links.Count == 0)
            {
                return Json(new { ok = true, changed = false });
            }

            var now = DateTime.UtcNow;
            foreach (var l in links)
            {
                l.IsActive = false;
                if (!l.ExpiresAtUtc.HasValue || l.ExpiresAtUtc.Value > now)
                {
                    l.ExpiresAtUtc = now;
                }
            }

            await _db.SaveChangesAsync(ct);

            return Json(new { ok = true, changed = true });
        }

        [Authorize(Roles = SD.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAssistant(int id)
        {
            var repo = _unitOfWork.GetRepository<Assistant>();
            var item = repo.GetFirstOrDefault(a => a.Id == id);
            if (item == null) return NotFound();
            repo.Remove(item);
            _unitOfWork.Save();
            TempData["success"] = "Assistent gelöscht.";
            return RedirectToAction(nameof(Assistenten));
        }

        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        [HttpGet]
        public IActionResult GetAssistantGroups(int id)
        {
            var repo = _unitOfWork.GetRepository<Assistant>();
            var assistant = repo.GetFirstOrDefault(a => a.Id == id);
            if (assistant == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("Coach");
            var isDozent = User.IsInRole("Dozent");

            if (!isAdmin && !(isDozent && string.Equals(assistant.CreatedByUserId, currentUserId, StringComparison.Ordinal)))
            {
                return Forbid();
            }

            List<string> existing;
            try
            {
                existing = _db.Groups
                    .AsEnumerable()
                    .Select(g => g?.Name?.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s!)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch
            {
                existing = new List<string>();
            }

            HashSet<string> allowed;
            if (isAdmin)
            {
                allowed = existing.ToHashSet(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                var uid = currentUserId;
                var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var mems = _db.UserGroupMemberships
                        .Where(m => m.UserId == uid)
                        .Select(m => m.Group)
                        .AsEnumerable()
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!.Trim());
                    foreach (var g in mems) set.Add(g);
                }
                catch { }

                if (isDozent)
                {
                    try
                    {
                        var owned = _db.DozentGroupOwnerships
                            .Where(o => o.DozentUserId == uid)
                            .Select(o => o.Group)
                            .AsEnumerable()
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(s => s!.Trim());
                        foreach (var g in owned) set.Add(g);
                    }
                    catch { }
                }

                allowed = set
                    .Where(s => existing.Contains(s))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
            }

            List<string> assigned;
            try
            {
                assigned = _db.AssistantGroups
                    .Where(ag => ag.AssistantId == id)
                    .Select(ag => ag.Group)
                    .AsEnumerable()
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s!.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch
            {
                assigned = new List<string>();
            }

            return Json(new
            {
                ok = true,
                allowedGroups = allowed.ToList(),
                assignedGroups = assigned
            });
        }

        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAssistantGroups(int id, string[]? groups)
        {
            var repo = _unitOfWork.GetRepository<Assistant>();
            var assistant = repo.GetFirstOrDefault(a => a.Id == id);
            if (assistant == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("Coach");
            var isDozent = User.IsInRole("Dozent");

            if (!isAdmin && !(isDozent && string.Equals(assistant.CreatedByUserId, currentUserId, StringComparison.Ordinal)))
            {
                return Forbid();
            }

            List<string> existing;
            try
            {
                existing = _db.Groups
                    .AsEnumerable()
                    .Select(g => g?.Name?.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s!)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();
            }
            catch
            {
                existing = new List<string>();
            }

            HashSet<string> allowed;
            if (isAdmin)
            {
                allowed = existing.ToHashSet(StringComparer.OrdinalIgnoreCase);
            }
            else
            {
                var uid = currentUserId;
                var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    var mems = _db.UserGroupMemberships
                        .Where(m => m.UserId == uid)
                        .Select(m => m.Group)
                        .AsEnumerable()
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!.Trim());
                    foreach (var g in mems) set.Add(g);
                }
                catch { }

                if (isDozent)
                {
                    try
                    {
                        var owned = _db.DozentGroupOwnerships
                            .Where(o => o.DozentUserId == uid)
                            .Select(o => o.Group)
                            .AsEnumerable()
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(s => s!.Trim());
                        foreach (var g in owned) set.Add(g);
                    }
                    catch { }
                }

                allowed = set
                    .Where(s => existing.Contains(s))
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
            }

            var selected = (groups ?? Array.Empty<string>())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var final = selected
                .Where(s => allowed.Contains(s))
                .ToList();

            try
            {
                var existingLinks = _db.AssistantGroups.Where(x => x.AssistantId == id);
                _db.AssistantGroups.RemoveRange(existingLinks);
                await _db.SaveChangesAsync();

                if (final.Count > 0)
                {
                    var map = _db.Groups
                        .AsEnumerable()
                        .Where(g => g?.Name != null)
                        .ToDictionary(g => g!.Name!.Trim(), g => g, StringComparer.OrdinalIgnoreCase);

                    foreach (var name2 in final)
                    {
                        if (!map.TryGetValue(name2, out var row)) continue;
                        _db.AssistantGroups.Add(new AssistantGroup { AssistantId = id, GroupId = row.Id, Group = name2 });
                    }

                    await _db.SaveChangesAsync();
                }
            }
            catch
            {
                Response.StatusCode = 500;
                return Json(new { ok = false, error = "Gruppen konnten nicht aktualisiert werden." });
            }

            return Json(new { ok = true, groups = final });
        }

        public IActionResult PromptDetails(int id)
        {
            var tpl = _unitOfWork
                       .GetRepository<PromptTemplate>()
                       .GetFirstOrDefault(
                           filter: t => t.Id == id,
                           includeProperties: "PromptVariations,PromptImages"
                       );

            if (tpl == null) return NotFound();

            var uid = _userManager.GetUserId(User);
            ViewData["CurrentUserId"] = uid;
            ViewData["PromptUserId"] = tpl.UserId;

            // Image management permissions (owner, Admin, Dozent for group prompts)
            bool canManageImages = CanManagePromptImages(tpl, uid);
            ViewBag.CanManageImages = canManageImages;

            // Prompt sharing: same roles as assistant sharing (Admin, SuperAdmin, Dozent)
            var canSharePrompt = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("Dozent");

            // Compute allowed groups for current user (reuse logic from PromptTemplateApiController.AllowedGroups)
            List<string> shareGroups;
            try
            {
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("Coach");
                var isDozent = User.IsInRole("Dozent");

                List<string> existing;
                try
                {
                    existing = _db.Groups
                        .AsEnumerable()
                        .Select(g => g?.Name?.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
                catch
                {
                    existing = new List<string>();
                }

                if (isAdmin)
                {
                    shareGroups = existing
                        .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
                else
                {
                    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    try
                    {
                        var myGroups = _db.UserGroupMemberships
                            .Where(m => m.UserId == uid)
                            .Select(m => m.Group)
                            .AsEnumerable()
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(s => s!.Trim());
                        foreach (var g in myGroups) set.Add(g);
                    }
                    catch { }

                    if (isDozent)
                    {
                        try
                        {
                            var owned = _db.DozentGroupOwnerships
                                .Where(o => o.DozentUserId == uid)
                                .Select(o => o.Group)
                                .AsEnumerable()
                                .Where(s => !string.IsNullOrWhiteSpace(s))
                                .Select(s => s!.Trim());
                            foreach (var g in owned) set.Add(g);
                        }
                        catch { }
                    }

                    shareGroups = set
                        .Where(s => existing.Contains(s))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
            }
            catch
            {
                shareGroups = new List<string>();
            }

            bool promptIsShared = false;
            try
            {
                var now = DateTime.UtcNow;
                promptIsShared = _db.PromptShareLinks
                    .Any(l => l.PromptTemplateId == id && l.IsActive && (!l.ExpiresAtUtc.HasValue || l.ExpiresAtUtc.Value > now));
            }
            catch { promptIsShared = false; }

            ViewBag.CanSharePrompt = canSharePrompt && shareGroups.Count > 0;
            ViewBag.PromptIsShared = promptIsShared;
            ViewBag.ShareGroups = shareGroups;

            return View(tpl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAutorin(int id, string autorin)
        {
            var tpl = _unitOfWork.GetRepository<PromptTemplate>().GetFirstOrDefault(t => t.Id == id);
            if (tpl == null) return NotFound();

            tpl.Autorin = autorin?.Trim();
            _unitOfWork.GetRepository<PromptTemplate>().Update(tpl);
            _unitOfWork.Save();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    value = tpl.Autorin,
                    message = "Autorin gespeichert."
                });
            }

            TempData["success"] = "Autorin gespeichert.";
            return RedirectToAction(nameof(PromptDetails), new { id });
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateVideoUrl(int id, string? videoUrl)
        {
            var tpl = _unitOfWork.GetRepository<PromptTemplate>()
                                 .GetFirstOrDefault(t => t.Id == id);
            if (tpl == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (!User.IsInRole("Admin") && tpl.UserId != currentUserId)
                return Forbid();

            if (!string.IsNullOrWhiteSpace(videoUrl))
            {
                if (!Uri.TryCreate(videoUrl, UriKind.Absolute, out var uri) ||
                    !(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    ModelState.AddModelError("videoUrl", "Ungültige URL.");
                    return View("PromptDetails", tpl);
                }
                tpl.GeneratedVideoUrl = videoUrl.Trim();
            }
            else
            {
                tpl.GeneratedVideoUrl = null; // allow clearing
            }

            _unitOfWork.GetRepository<PromptTemplate>().Update(tpl);
            _unitOfWork.Save();
            TempData["success"] = "Video-URL gespeichert.";
            return RedirectToAction(nameof(PromptDetails), new { id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateAudioUrl(int id, string? audioUrl)
        {
            var tpl = _unitOfWork.GetRepository<PromptTemplate>()
                                 .GetFirstOrDefault(t => t.Id == id);
            if (tpl == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (!User.IsInRole("Admin") && tpl.UserId != currentUserId)
                return Forbid();

            if (!string.IsNullOrWhiteSpace(audioUrl))
            {
                if (!Uri.TryCreate(audioUrl, UriKind.Absolute, out var uri) ||
                    !(uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                {
                    ModelState.AddModelError("audioUrl", "Ungültige URL.");
                    return View("PromptDetails", tpl);
                }
                tpl.GeneratedAudioUrl = audioUrl.Trim();
            }
            else
            {
                tpl.GeneratedAudioUrl = null; // allow clearing
            }

            _unitOfWork.GetRepository<PromptTemplate>().Update(tpl);
            _unitOfWork.Save();
            TempData["success"] = "Audio-URL gespeichert.";
            return RedirectToAction(nameof(PromptDetails), new { id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateLizenz(int id, string lizenz)
        {
            var tpl = _unitOfWork.GetRepository<PromptTemplate>().GetFirstOrDefault(t => t.Id == id);
            if (tpl == null) return NotFound();

            tpl.Lizenz = lizenz?.Trim();
            _unitOfWork.GetRepository<PromptTemplate>().Update(tpl);
            _unitOfWork.Save();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    value = tpl.Lizenz,
                    message = "Lizenz gespeichert."
                });
            }

            TempData["success"] = "Lizenz gespeichert.";
            return RedirectToAction(nameof(PromptDetails), new { id });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateUsedModels(int id, string usedModels)
        {
            var tpl = _unitOfWork.GetRepository<PromptTemplate>()
                                 .GetFirstOrDefault(t => t.Id == id);
            if (tpl == null) return NotFound();

            tpl.UsedModels = usedModels?.Trim();
            _unitOfWork.GetRepository<PromptTemplate>().Update(tpl);
            _unitOfWork.Save();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    value = tpl.UsedModels,
                    message = "Verwendete Modelle gespeichert."
                });
            }

            TempData["success"] = "Verwendete Modelle gespeichert.";
            return RedirectToAction(nameof(PromptDetails), new { id });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdatePromptReflektion(int id, string? reflektion)
        {
            var repo = _unitOfWork.GetRepository<PromptTemplate>();
            var tpl = repo.GetFirstOrDefault(t => t.Id == id);
            if (tpl == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (!CanManagePromptImages(tpl, currentUserId))
                return Forbid();

            string? r = string.IsNullOrWhiteSpace(reflektion) ? null : reflektion.Trim();
            const int MaxRefl = 4000;
            if (r != null && r.Length > MaxRefl)
                r = r.Substring(0, MaxRefl);

            tpl.Reflektion = r;

            try
            {
                repo.Update(tpl);
                _unitOfWork.Save();
                TempData["success"] = "Reflexion gespeichert.";
            }
            catch (DbUpdateException)
            {
                TempData["error"] = "Speichern der Reflexion fehlgeschlagen.";
            }

            return RedirectToAction(nameof(PromptDetails), new { id });
        }

        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UploadImage(int id, IFormFile imageFile)
        //{
        //    if (imageFile == null || imageFile.Length == 0)
        //        ModelState.AddModelError("imageFile", "Bitte wählen Sie eine Bilddatei aus.");

        //    var tpl = _unitOfWork.GetRepository<PromptTemplate>()
        //                         .GetFirstOrDefault(t => t.Id == id);
        //    if (tpl == null) return NotFound();

        //    if (!ModelState.IsValid)
        //        // this will now find your PromptDetails view
        //        return View("PromptDetails", tpl);

        //    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        //    Directory.CreateDirectory(uploads);

        //    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
        //    var filePath = Path.Combine(uploads, fileName);
        //    await using var stream = System.IO.File.Create(filePath);
        //    await imageFile.CopyToAsync(stream);

        //    tpl.GeneratedImagePath = $"/uploads/{fileName}";
        //    _unitOfWork.GetRepository<PromptTemplate>().Update(tpl);
        //    _unitOfWork.Save();

        //    return RedirectToAction(nameof(PromptDetails), new { id });
        //}

        //[Authorize(Roles = "Admin")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult DeleteImage(int id)
        //{
        //    // 1) Datensatz holen
        //    var tpl = _unitOfWork.GetRepository<PromptTemplate>()
        //                         .GetFirstOrDefault(t => t.Id == id);
        //    if (tpl == null) return NotFound();

        //    // 2) Physische Datei ermitteln und löschen
        //    if (!string.IsNullOrWhiteSpace(tpl.GeneratedImagePath))
        //    {
        //        // Pfad ist z. B.  /uploads/abc.jpg  →  wwwroot/uploads/abc.jpg
        //        var absolutePath = Path.Combine(
        //            Directory.GetCurrentDirectory(),
        //            "wwwroot",
        //            tpl.GeneratedImagePath.TrimStart('/')
        //                                  .Replace('/', Path.DirectorySeparatorChar));

        //        if (System.IO.File.Exists(absolutePath))
        //        {
        //            try { System.IO.File.Delete(absolutePath); }
        //            catch (Exception ex)
        //            {
        //                // Loggen, aber trotzdem weiterlaufen lassen
        //                _logger.LogWarning(ex, "Konnte Bild {Path} nicht löschen", absolutePath);
        //            }
        //        }
        //    }

        //    // 3) Datenbank‑Feld leeren
        //    tpl.GeneratedImagePath = null;
        //    _unitOfWork.GetRepository<PromptTemplate>().Update(tpl);
        //    _unitOfWork.Save();

        //    // 4) OK an das Frontend
        //    return RedirectToAction(nameof(PromptDetails), new { id });
        //}
        [Authorize]                      // nur angemeldet
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(int id, List<IFormFile>? imageFiles, string? imageUrl)
        {
            var tpl = _db.PromptTemplate
                         .Include(t => t.PromptImages)
                         .FirstOrDefault(t => t.Id == id);
            if (tpl == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (!CanManagePromptImages(tpl, currentUserId))
                return Forbid();

            var hasFiles = imageFiles != null && imageFiles.Any(f => f != null && f.Length > 0);
            var hasUrl = !string.IsNullOrWhiteSpace(imageUrl);

            if (!hasFiles && !hasUrl)
            {
                ModelState.AddModelError(string.Empty, "Bitte eine Bild-Datei auswählen oder eine Bild-URL eingeben.");
                return View("PromptDetails", tpl);
            }

            if (hasFiles)
            {
                var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "prompts", id.ToString());
                Directory.CreateDirectory(uploadsRoot);

                foreach (var file in imageFiles!.Where(f => f != null && f.Length > 0))
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                    var filePath = Path.Combine(uploadsRoot, fileName);
                    await using var stream = System.IO.File.Create(filePath);
                    await file.CopyToAsync(stream);

                    var relPath = $"/uploads/prompts/{id}/{fileName}";
                    _db.PromptImages.Add(new PromptImage
                    {
                        PromptTemplateId = tpl.Id,
                        ImagePath = relPath,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (hasUrl)
            {
                var urlTrimmed = imageUrl!.Trim();
                _db.PromptImages.Add(new PromptImage
                {
                    PromptTemplateId = tpl.Id,
                    ImagePath = urlTrimmed,
                    CreatedAt = DateTime.UtcNow
                });
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(PromptDetails), new { id });
        }


        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int id, int imageId)
        {
            var tpl = _db.PromptTemplate
                         .Include(t => t.PromptImages)
                         .FirstOrDefault(t => t.Id == id);
            if (tpl == null) return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (!CanManagePromptImages(tpl, currentUserId))
                return Forbid();

            var img = tpl.PromptImages.FirstOrDefault(p => p.Id == imageId);
            if (img == null) return NotFound();

            if (!string.IsNullOrWhiteSpace(img.ImagePath) &&
                img.ImagePath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            {
                var abs = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                            img.ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(abs))
                    System.IO.File.Delete(abs);
            }

            _db.PromptImages.Remove(img);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(PromptDetails), new { id });
        }


        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePromptShareLink(int id, string groupName, CancellationToken ct)
        {
            var tpl = _db.PromptTemplate.FirstOrDefault(t => t.Id == id);
            if (tpl == null) return NotFound();

            if (string.IsNullOrWhiteSpace(groupName))
            {
                Response.StatusCode = 400;
                return Json(new { ok = false, error = "Gruppe ist erforderlich." });
            }

            var normGroup = groupName.Trim();
            var currentUserId = _userManager.GetUserId(User);

            // Allowed groups similar to PromptTemplateApiController.AllowedGroups
            HashSet<string> allowedGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var uid = currentUserId;
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("Coach");
                var isDozent = User.IsInRole("Dozent");

                List<string> existing;
                try
                {
                    existing = _db.Groups
                        .AsEnumerable()
                        .Select(g => g?.Name?.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
                catch
                {
                    existing = new List<string>();
                }

                if (isAdmin)
                {
                    allowedGroups = existing.ToHashSet(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    try
                    {
                        var myGroups = _db.UserGroupMemberships
                            .Where(m => m.UserId == uid)
                            .Select(m => m.Group)
                            .AsEnumerable()
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(s => s!.Trim());
                        foreach (var g in myGroups) set.Add(g);
                    }
                    catch { }

                    if (isDozent)
                    {
                        try
                        {
                            var owned = _db.DozentGroupOwnerships
                                .Where(o => o.DozentUserId == uid)
                                .Select(o => o.Group)
                                .AsEnumerable()
                                .Where(s => !string.IsNullOrWhiteSpace(s))
                                .Select(s => s!.Trim());
                            foreach (var g in owned) set.Add(g);
                        }
                        catch { }
                    }

                    allowedGroups = set
                        .Where(s => existing.Contains(s))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                }
            }
            catch
            {
                allowedGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            if (!allowedGroups.Contains(normGroup))
            {
                Response.StatusCode = 400;
                return Json(new { ok = false, error = "Sie können diesen Prompt nur für Ihre eigenen Gruppen freigeben." });
            }

            PromptShareLink? link = null;
            try
            {
                link = await _db.PromptShareLinks
                    .Where(l => l.PromptTemplateId == id && l.Group == normGroup)
                    .OrderByDescending(l => l.CreatedAtUtc)
                    .FirstOrDefaultAsync(ct);
            }
            catch { }

            if (link == null)
            {
                link = new PromptShareLink
                {
                    PromptTemplateId = id,
                    Group = normGroup,
                    PublicId = Guid.NewGuid().ToString("N"),
                    IsActive = true,
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedByUserId = currentUserId
                };
                _db.PromptShareLinks.Add(link);
            }
            else
            {
                link.IsActive = true;
                if (link.ExpiresAtUtc.HasValue && link.ExpiresAtUtc.Value <= DateTime.UtcNow)
                    link.ExpiresAtUtc = null;
            }

            await _db.SaveChangesAsync(ct);

            string? absoluteUrl = null;
            try
            {
                absoluteUrl = Url.Action(
                    "Prompt",
                    "SharedPrompt",
                    new { area = "User", id = link.PublicId },
                    Request.Scheme,
                    Request.Host.ToString());
            }
            catch { }

            return Json(new
            {
                ok = true,
                shareId = link.Id,
                token = link.PublicId,
                url = absoluteUrl ?? ($"/User/SharedPrompt/Prompt/{link.PublicId}")
            });
        }


        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        [HttpGet]
        public async Task<IActionResult> GetPromptShareState(int id, string groupName, CancellationToken ct)
        {
            var tpl = _db.PromptTemplate.FirstOrDefault(t => t.Id == id);
            if (tpl == null) return NotFound();

            if (string.IsNullOrWhiteSpace(groupName))
            {
                Response.StatusCode = 400;
                return Json(new { ok = false, error = "Gruppe ist erforderlich." });
            }

            var normGroup = groupName.Trim();
            var currentUserId = _userManager.GetUserId(User);

            HashSet<string> allowedGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var uid = currentUserId;
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("Coach");
                var isDozent = User.IsInRole("Dozent");

                List<string> existing;
                try
                {
                    existing = _db.Groups
                        .AsEnumerable()
                        .Select(g => g?.Name?.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
                catch
                {
                    existing = new List<string>();
                }

                if (isAdmin)
                {
                    allowedGroups = existing.ToHashSet(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    try
                    {
                        var myGroups = _db.UserGroupMemberships
                            .Where(m => m.UserId == uid)
                            .Select(m => m.Group)
                            .AsEnumerable()
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(s => s!.Trim());
                        foreach (var g in myGroups) set.Add(g);
                    }
                    catch { }

                    if (isDozent)
                    {
                        try
                        {
                            var owned = _db.DozentGroupOwnerships
                                .Where(o => o.DozentUserId == uid)
                                .Select(o => o.Group)
                                .AsEnumerable()
                                .Where(s => !string.IsNullOrWhiteSpace(s))
                                .Select(s => s!.Trim());
                            foreach (var g in owned) set.Add(g);
                        }
                        catch { }
                    }

                    allowedGroups = set
                        .Where(s => existing.Contains(s))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                }
            }
            catch
            {
                allowedGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            if (!allowedGroups.Contains(normGroup))
            {
                Response.StatusCode = 400;
                return Json(new { ok = false, error = "Sie können diesen Prompt nur für Ihre eigenen Gruppen freigeben." });
            }

            PromptShareLink? link = null;
            try
            {
                link = await _db.PromptShareLinks
                    .Where(l => l.PromptTemplateId == id && l.Group == normGroup)
                    .OrderByDescending(l => l.CreatedAtUtc)
                    .FirstOrDefaultAsync(ct);
            }
            catch { }

            if (link == null)
            {
                return Json(new { ok = true, hasLink = false, isActive = false, url = string.Empty });
            }

            var isActive = link.IsActive && (!link.ExpiresAtUtc.HasValue || link.ExpiresAtUtc.Value > DateTime.UtcNow);

            string? absoluteUrl = null;
            if (isActive)
            {
                try
                {
                    absoluteUrl = Url.Action(
                        "Prompt",
                        "SharedPrompt",
                        new { area = "User", id = link.PublicId },
                        Request.Scheme,
                        Request.Host.ToString());
                }
                catch { }
            }

            return Json(new
            {
                ok = true,
                hasLink = true,
                isActive,
                url = isActive ? (absoluteUrl ?? ($"/User/SharedPrompt/Prompt/{link.PublicId}")) : string.Empty
            });
        }


        [Authorize(Roles = "Admin,SuperAdmin,Dozent")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivatePromptShareLink(int id, string groupName, CancellationToken ct)
        {
            var tpl = _db.PromptTemplate.FirstOrDefault(t => t.Id == id);
            if (tpl == null) return NotFound();

            if (string.IsNullOrWhiteSpace(groupName))
            {
                Response.StatusCode = 400;
                return Json(new { ok = false, error = "Gruppe ist erforderlich." });
            }

            var normGroup = groupName.Trim();
            var currentUserId = _userManager.GetUserId(User);

            HashSet<string> allowedGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var uid = currentUserId;
                var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("Coach");
                var isDozent = User.IsInRole("Dozent");

                List<string> existing;
                try
                {
                    existing = _db.Groups
                        .AsEnumerable()
                        .Select(g => g?.Name?.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .Select(s => s!)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
                catch
                {
                    existing = new List<string>();
                }

                if (isAdmin)
                {
                    allowedGroups = existing.ToHashSet(StringComparer.OrdinalIgnoreCase);
                }
                else
                {
                    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    try
                    {
                        var myGroups = _db.UserGroupMemberships
                            .Where(m => m.UserId == uid)
                            .Select(m => m.Group)
                            .AsEnumerable()
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .Select(s => s!.Trim());
                        foreach (var g in myGroups) set.Add(g);
                    }
                    catch { }

                    if (isDozent)
                    {
                        try
                        {
                            var owned = _db.DozentGroupOwnerships
                                .Where(o => o.DozentUserId == uid)
                                .Select(o => o.Group)
                                .AsEnumerable()
                                .Where(s => !string.IsNullOrWhiteSpace(s))
                                .Select(s => s!.Trim());
                            foreach (var g in owned) set.Add(g);
                        }
                        catch { }
                    }

                    allowedGroups = set
                        .Where(s => existing.Contains(s))
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                }
            }
            catch
            {
                allowedGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            if (!allowedGroups.Contains(normGroup))
            {
                Response.StatusCode = 400;
                return Json(new { ok = false, error = "Sie können diesen Prompt nur für Ihre eigenen Gruppen freigeben." });
            }

            List<PromptShareLink> links;
            try
            {
                links = await _db.PromptShareLinks
                    .Where(l => l.PromptTemplateId == id && l.Group == normGroup)
                    .ToListAsync(ct);
            }
            catch
            {
                Response.StatusCode = 500;
                return Json(new { ok = false, error = "Freigabelink konnte nicht deaktiviert werden." });
            }

            if (links == null || links.Count == 0)
            {
                return Json(new { ok = true, changed = false });
            }

            var now = DateTime.UtcNow;
            foreach (var l in links)
            {
                l.IsActive = false;
                if (!l.ExpiresAtUtc.HasValue || l.ExpiresAtUtc.Value > now)
                {
                    l.ExpiresAtUtc = now;
                }
            }

            await _db.SaveChangesAsync(ct);

            return Json(new { ok = true, changed = true });
        }





        [HttpPost]
        public async Task<IActionResult> DeleteVariation(int id)
        {


            var variation = await _db.SavedPromptVariations
                                    .Include(v => v.SavedPrompt)
                                    .FirstOrDefaultAsync(v
                                        => v.Id == id
                                        );
            if (variation == null)
                return NotFound();

            _db.SavedPromptVariations.Remove(variation);
            await _db.SaveChangesAsync();
            return Ok();
        }

        private bool CanManagePromptImages(PromptTemplate tpl, string? currentUserId)
        {
            if (tpl == null) return false;

            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            if (isAdmin) return true;

            if (string.IsNullOrWhiteSpace(currentUserId)) return false;

            if (!string.IsNullOrWhiteSpace(tpl.UserId) &&
                string.Equals(tpl.UserId, currentUserId, StringComparison.OrdinalIgnoreCase))
            {
                return true; // owner
            }

            if (!User.IsInRole("Dozent"))
                return false;

            try
            {
                var ownedGroups = _db.DozentGroupOwnerships
                    .Where(o => o.DozentUserId == currentUserId && o.Group != null && o.Group != "")
                    .Select(o => o.Group!)
                    .AsEnumerable()
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s!.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (ownedGroups.Count == 0)
                    return false;

                var promptGroups = _db.PromptTemplateGroups
                    .Where(pg => pg.PromptTemplateId == tpl.Id && pg.Group != null && pg.Group != "")
                    .Select(pg => pg.Group!)
                    .AsEnumerable()
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s!.Trim())
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                return ownedGroups.Any(g => promptGroups.Contains(g));
            }
            catch
            {
                return false;
            }
        }
        public IActionResult XRLab()
        {
            var projectList = _unitOfWork.Project.GetAll(includeProperties: "Images")
                                .Where(p => p.IsVirtuellesKlassenzimmer)
                                .Where(p => p.IsEnabled)
                                .ToList();

            var vkSliderItems = _unitOfWork.SliderItem
     .GetAll()
     .Where(s => s.IsForVirtuellesKlassenzimmer)
     .ToList();

            var categories = _unitOfWork.GetRepository<Category>().GetAll().OrderBy(c => c.DisplayOrder).ToDictionary(c => c.Id, c => c.Name);
            var fachgruppen = _unitOfWork.GetRepository<Fachgruppen>().GetAll().OrderBy(c => c.DisplayOrder).ToDictionary(f => f.Id, f => f.Name);
            var techAnforderungen = _unitOfWork.GetRepository<TechAnforderung>().GetAll().OrderBy(c => c.DisplayOrder).ToDictionary(t => t.Id, t => t.Name);
            var fakultaeten = _unitOfWork.GetRepository<Fakultaet>().GetAll().OrderBy(c => c.DisplayOrder).ToDictionary(f => f.Id, f => f.Name);

            ViewBag.Categories = categories.Values.ToList();
            ViewBag.Fachgruppen = fachgruppen.Values.ToList();
            ViewBag.Fakultaet = fakultaeten.Values.ToList();
            ViewBag.TechAnforderungen = techAnforderungen.Values.ToList();
            ViewBag.VKSliderItems = vkSliderItems;

            foreach (var project in projectList)
            {
                project.CategoryIds = GetNamesFromIds(project.CategoryIds, categories);
                project.FachgruppenIds = GetNamesFromIds(project.FachgruppenIds, fachgruppen);
                project.FakultaetIds = GetNamesFromIds(project.FakultaetIds, fakultaeten);
                project.TechAnforderungIds = GetNamesFromIds(project.TechAnforderungIds, techAnforderungen);
            }

            return View("XRLab", projectList);
        }


        [HttpGet]
        public IActionResult PromptAssistent(string? type = null)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Contact", "Home");

            // 1) PromptType ermitteln
            var promptType = Enum.TryParse<PromptType>(type, true, out var parsed)
                             ? parsed
                             : PromptType.Text;

            // 2) UserId holen
            var userId = _userManager.GetUserId(User);
            var isPrivileged = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("Coach");

            // Per-user Sichtbarkeit nur für nicht-privilegierte Nutzer anwenden
            var applyUserVisibility = !string.IsNullOrEmpty(userId);
            HashSet<int> hiddenCategoryIds = new HashSet<int>();
            if (applyUserVisibility)
            {
                try
                {
                    hiddenCategoryIds = _db.UserFilterCategoryVisibilities
                        .Where(p => p.UserId == userId && p.IsHidden)
                        .Select(p => p.FilterCategoryId)
                        .ToHashSet();
                }
                catch
                {
                    hiddenCategoryIds = new HashSet<int>();
                }
            }

            // 3) FilterCategories laden (Admin-Hide + Benutzer-Hide)
            var filters = _unitOfWork.FilterCategory
                .GetAll(includeProperties: "FilterItems")
                .Where(f =>
                    f.Type == promptType && (!f.IsHidden || isPrivileged) &&
                    (!applyUserVisibility || !hiddenCategoryIds.Contains(f.Id))
                )
                .OrderBy(f => f.DisplayOrder)
                .ThenBy(f => f.Name)
                .ToList();

            // Collect suggestions from the "Schlüsselbegriffe" category
            var keywordCat = filters
                .FirstOrDefault(f => string.Equals(f.Name, "Schlüsselbegriffe", StringComparison.OrdinalIgnoreCase));

            var keywordSuggestions = keywordCat?.FilterItems?
                    .Select(i => i.Title)
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(s => s)
                    .ToList() ?? new List<string>();

            // Fallback: take from any type, if current type has none
            if (keywordSuggestions.Count == 0)
            {
                keywordSuggestions = _unitOfWork.FilterCategory
                    .GetAll(includeProperties: "FilterItems")
                    .Where(fc => string.Equals(fc.Name, "Schlüsselbegriffe", StringComparison.OrdinalIgnoreCase))
                    .SelectMany(fc => fc.FilterItems.Select(i => i.Title))
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(s => s)
                    .ToList();
            }

            // Curated keywords managed by Admin
            var curatedKeywords = _unitOfWork
                .GetRepository<PromptKeyword>()
                .GetAll()
                .Select(k => k.Text)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            // Bibliothek keywords + popularity counts
            var allTplKeywords = _unitOfWork
                .GetRepository<PromptTemplate>()
                .GetAll()
                .Select(t => t.Schluesselbegriffe ?? string.Empty)
                .SelectMany(s => s.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(s => s.Trim())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            var tplCounts = allTplKeywords
                .GroupBy(s => s, StringComparer.OrdinalIgnoreCase)
                .Select(g => new { Text = g.Key.Trim(), Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            var tplKeywords = tplCounts
                .Select(x => x.Text)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Only mark truly popular items with fire: require a minimum count and cap to Top N
            const int PopularMinCount = 2;   // adjust if you want stricter filtering (e.g., 3)
            const int PopularTopN = 12;
            var popularTop = tplCounts
                .Where(x => x.Count >= PopularMinCount)
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Text)
                .Take(PopularTopN)
                .Select(x => x.Text)
                .ToList();

            // Always include Bibliothek keywords as well
            keywordSuggestions = curatedKeywords
                .Concat(keywordSuggestions)
                .Concat(tplKeywords)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s)
                .ToList();

            ViewBag.PromptWords = _unitOfWork
        .GetRepository<PromptWord>()
        .GetAll()
        .OrderBy(w => w.Text)
        .ToList();

            ViewBag.PromptModels = _unitOfWork.GetRepository<PromptModel>().GetAll().ToList();
            ViewBag.Type = promptType;
            ViewBag.KeywordSuggestions = keywordSuggestions;
            ViewBag.PopularKeywords = popularTop; // for flame icon
            ViewBag.KeywordCounts = tplCounts.ToDictionary(x => x.Text, x => x.Count, StringComparer.OrdinalIgnoreCase);

            // Compute effective feature flags for current user (per-group -> global -> static); Admins override
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
            bool globalFG = ProjectsWebApp.Utility.FeatureFlags.EnableFilterGeneration;
            bool globalSS = ProjectsWebApp.Utility.FeatureFlags.EnableSmartSelection;
            bool globalPT = ProjectsWebApp.Utility.FeatureFlags.EnablePromptTechnique; // no DB persistence at the moment
            try
            {
                var rec = _db.PromptFeatureSettings
                             .OrderByDescending(x => x.UpdatedAt)
                             .FirstOrDefault();
                if (rec != null)
                {
                    globalFG = rec.EnableFilterGeneration;
                    globalSS = rec.EnableSmartSelection;
                }
            }
            catch { /* tolerate before migration */ }

            string? lastGroup = null;
            try
            {
                lastGroup = _db.UserGroupMemberships
                               .Where(m => m.UserId == userId)
                               .OrderByDescending(m => m.CreatedAt)
                               .Select(m => m.Group)
                               .FirstOrDefault();
            }
            catch { }
            var groupName = string.IsNullOrWhiteSpace(lastGroup) ? "Ohne Gruppe" : lastGroup.Trim();

            // Collect all groups relevant for this user: direct memberships + groups where user is owner/Dozent
            var groupSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                var memberGroups = _db.UserGroupMemberships
                                       .Where(m => m.UserId == userId && m.Group != null && m.Group != "")
                                       .Select(m => m.Group!)
                                       .AsEnumerable()
                                       .Where(s => !string.IsNullOrWhiteSpace(s))
                                       .Select(s => s!.Trim());
                foreach (var g in memberGroups)
                    groupSet.Add(g);
            }
            catch { }

            try
            {
                var ownerGroups = _db.DozentGroupOwnerships
                                     .Where(o => o.DozentUserId == userId && o.Group != null && o.Group != "")
                                     .Select(o => o.Group!)
                                     .AsEnumerable()
                                     .Where(s => !string.IsNullOrWhiteSpace(s))
                                     .Select(s => s!.Trim());
                foreach (var g in ownerGroups)
                    groupSet.Add(g);
            }
            catch { }

            var myGroups = groupSet
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                .ToList();

            // Subset of groups that have group-specific prompt settings (UseGlobal = false)
            var aiGroups = new List<string>();
            try
            {
                // Determine, per group, the latest prompt-AI settings and keep only those
                // whose current state has UseGlobal == false (true "custom" system prompts).
                var localGroups = _db.GroupPromptAiSettings
                                     .Where(g => g.Group != null && g.Group != "")
                                     .AsEnumerable()
                                     .GroupBy(g => g.Group!.Trim(), StringComparer.OrdinalIgnoreCase)
                                     .Select(grp => grp
                                         .OrderByDescending(x => x.UpdatedAt)
                                         .FirstOrDefault())
                                     .Where(gp => gp != null && gp.UseGlobal == false && !string.IsNullOrWhiteSpace(gp.Group))
                                     .Select(gp => gp!.Group!.Trim())
                                     .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (isAdmin)
                {
                    // Admins: see all groups that currently have their own system prompts (independent of membership)
                    aiGroups = localGroups
                        .OrderBy(g => g, StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
                else
                {
                    // Dozenten/Users: only groups they belong to or own that currently have custom prompts
                    aiGroups = myGroups
                        .Where(g => localGroups.Contains(g))
                        .OrderBy(g => g, StringComparer.OrdinalIgnoreCase)
                        .ToList();
                }
            }
            catch { }

            ViewBag.UserGroups = myGroups;
            ViewBag.ActiveGroup = groupName;
            ViewBag.AiGroups = aiGroups;

            bool effFG = globalFG;
            bool effSS = globalSS;
            bool effPT = globalPT;
            try
            {
                var gfs = _db.GroupFeatureSettings
                             .Where(g => g.Group != null && g.Group.Trim() == groupName)
                             .OrderByDescending(g => g.UpdatedAt)
                             .FirstOrDefault();
                if (gfs != null)
                {
                    effFG = gfs.EnableFilterGeneration;
                    effSS = gfs.EnableSmartSelection;
                }
            }
            catch { }

            if (isAdmin) { effFG = true; effSS = true; }

            ViewBag.FeatureGenerationEnabled = effFG;
            ViewBag.FeatureSmartEnabled = effSS;
            ViewBag.FeatureTechniqueEnabled = effPT;
            ViewBag.FeatureGenerationDisabledReason = (!effFG && !isAdmin)
                ? (!globalFG ? "Die KI‑Generierung wurde global vom Admin deaktiviert." : "Die KI‑Generierung ist für deine Gruppe deaktiviert.")
                : null;
            ViewBag.FeatureSmartDisabledReason = (!effSS && !isAdmin)
                ? (!globalSS ? "Smart Selection wurde global vom Admin deaktiviert." : "Smart Selection ist für deine Gruppe deaktiviert.")
                : null;
            ViewBag.FeatureTechniqueDisabledReason = (!effPT && !isAdmin)
                ? (!globalPT ? "Prompt‑Techniken wurden global vom Admin deaktiviert." : null)
                : null;

            // Assistant edit preload (open builder with an existing assistant)
            if (int.TryParse(HttpContext.Request.Query["assistantId"], out var assistantId))
            {
                var asst = _unitOfWork.GetRepository<Assistant>().GetFirstOrDefault(a => a.Id == assistantId);
                if (asst != null)
                {
                    ViewBag.EditAssistant = asst;
                }
            }

            return View(filters);
        }



        [HttpGet]
        public IActionResult Contact()
        {
            // grab your email from the ContactEmail table (you can change to suit your repo pattern)
            var emailRecord = _unitOfWork.GetRepository<ContactEmail>()
                                         .GetAll()
                                         .FirstOrDefault();
            var email = emailRecord?.Email ?? "info@example.com";

            var msg = _unitOfWork.GetRepository<ContactMessageSetting>()
                                         .GetAll()
                                         .FirstOrDefault();
            ViewBag.ContactMessage = msg?.Message;


            ViewBag.ContactEmail = email;
            return View();
        }



        public IActionResult GoBack()
        {
            // Hole die zuletzt besuchte Seite aus der Session
            int lastPage = HttpContext.Session.GetInt32("LastPage") ?? 1;

            // Leite den Benutzer zur letzten Seite um
            return RedirectToAction("Index", new { page = lastPage });
        }
        public IActionResult Impressum()
        {
            // Retrieve all DatenschutzContent entries ordered by DisplayOrder
            var model = _unitOfWork.ImpressumContent.GetAll().OrderBy(dc => dc.DisplayOrder);
            return View(model);
        }

        public IActionResult Datenschutz()
        {
            // Retrieve all DatenschutzContent entries ordered by DisplayOrder
            var model = _unitOfWork.DatenschutzContent.GetAll().OrderBy(dc => dc.DisplayOrder);
            return View(model);
        }



        public IActionResult Details(int id)
        {
            var project = _unitOfWork.Project.Get(u => u.Id == id, includeProperties: "Images,Videos");
            if (project == null)
            {
                return NotFound();
            }

            // Fetch related data for mapping
            var categories = _unitOfWork.GetRepository<Category>().GetAll().ToDictionary(c => c.Id, c => c.Name);
            var fachgruppen = _unitOfWork.GetRepository<Fachgruppen>().GetAll().ToDictionary(f => f.Id, f => f.Name);
            var techAnforderungen = _unitOfWork.GetRepository<TechAnforderung>().GetAll().ToDictionary(t => t.Id, t => t.Name);
            var fakultaeten = _unitOfWork.GetRepository<Fakultaet>().GetAll().ToDictionary(f => f.Id, f => f.Name);

            // Map names for display
            project.CategoryIds = GetNamesFromIds(project.CategoryIds, categories);
            project.FachgruppenIds = GetNamesFromIds(project.FachgruppenIds, fachgruppen);
            project.FakultaetIds = GetNamesFromIds(project.FakultaetIds, fakultaeten);
            project.TechAnforderungIds = GetNamesFromIds(project.TechAnforderungIds, techAnforderungen);

            return View(project);
        }

        private string GetNamesFromIds(string ids, Dictionary<int, string> mapping)
        {
            if (string.IsNullOrWhiteSpace(ids))
                return string.Empty;

            return string.Join(", ", ids.Split(',')
                                        .Select(id => int.TryParse(id, out var parsedId) && mapping.ContainsKey(parsedId) ? mapping[parsedId] : null)
                                        .Where(name => name != null));
        }


        public IActionResult Home()
        {
            var cards = _unitOfWork.PortalCard.GetAll();
            var video = _unitOfWork.PortalVideo.GetAll().FirstOrDefault();
            return View((cards, video));
        }
        [Area("User")]
        public IActionResult Kontakt()
        {
            var kontaktCards = _unitOfWork.GetRepository<KontaktCard>().GetAll().OrderBy(k => k.DisplayOrder);
            return View(kontaktCards);
        }

        [Area("User")]

        // Areas/User/Controllers/HomeController.cs  (Ausschnitt)
        public async Task<IActionResult> PromptEngineering()
        {
            var vm = new LandingPageVM
            {
                Hero = await _db.Heroes.AsNoTracking().SingleAsync(),
                Modes = await _db.Modes.AsNoTracking().OrderBy(m => m.SortOrder).ToListAsync(),
                Features = await _db.Features.AsNoTracking().OrderBy(f => f.SortOrder).ToListAsync()
            };

            // Keine automatische Ergänzung fehlender Modi mehr –
            // Admin‑Konfiguration (LandingItem/Edit) bestimmt, welche Prompt‑Arten sichtbar sind.

            return View(vm);      // Views/User/Home/PromptEngineering.cshtml
        }


        public IActionResult Urheberrecht()
        {
            // Retrieve all DatenschutzContent entries ordered by DisplayOrder
            var model = _unitOfWork.UrheberechtContent.GetAll().OrderBy(dc => dc.DisplayOrder);
            return View(model);
        }

        public IActionResult Leichtesprache()
        {
            var content = _unitOfWork.GetRepository<LeichteSpracheContent>().Get(u => u.Id == 1);
            if (content == null)
            {
                content = new LeichteSpracheContent { ContentHtml = "No content available." };
            }
            return View(content);
        }


        public IActionResult Tipps()
        {
            var contents = _unitOfWork.mitMachenContent.GetAll();
            return View(contents);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


}
