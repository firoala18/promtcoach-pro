using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;
using ProjectsWebApp.Utility;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;
using OpenAI.Embeddings;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Net;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class AssistantController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _cfg;
        private readonly IAiProviderResolver _resolver;
        private readonly IPdfTextExtractor _pdfExtractor;
        private readonly IPdfChunkingService _pdfChunker;

        public AssistantController(
            IUnitOfWork uow,
            ApplicationDbContext db,
            IConfiguration cfg,
            IAiProviderResolver resolver,
            IPdfTextExtractor pdfExtractor,
            IPdfChunkingService pdfChunker)
        {
            _uow = uow;
            _db = db;
            _cfg = cfg;
            _resolver = resolver;
            _pdfExtractor = pdfExtractor;
            _pdfChunker = pdfChunker;
        }

        // GET: /Admin/Assistant
        public IActionResult Index(string? q)
        {
            var repo = _uow.GetRepository<Assistant>();
            var list = repo.GetAll()
                           .OrderByDescending(a => a.CreatedAt)
                           .ToList();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim().ToLowerInvariant();
                list = list.Where(a => (a.Name ?? "").ToLower().Contains(term)
                                     || (a.Description ?? "").ToLower().Contains(term))
                           .ToList();
            }

            ViewBag.Query = q ?? string.Empty;
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleGlobal(int id)
        {
            try
            {
                var repo = _uow.GetRepository<Assistant>();
                var model = repo.GetFirstOrDefault(a => a.Id == id);
                if (model == null) return NotFound();
                model.IsGlobal = !model.IsGlobal;
                repo.Update(model);
                _uow.Save();
                TempData["success"] = model.IsGlobal ? "Assistent global sichtbar." : "Globale Sichtbarkeit deaktiviert.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Umschalten fehlgeschlagen: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Assistant/Edit/5
        public IActionResult Edit(int id)
        {
            var model = _uow.GetRepository<Assistant>().GetFirstOrDefault(a => a.Id == id);
            if (model == null) return NotFound();
            try
            {
                var embRepo = _uow.GetRepository<AssistantEmbedding>();
                var embCount = embRepo.GetAll().Count(e => e.AssistantId == id);
                ViewBag.EmbCount = embCount;
                var list = _db.Set<AssistantEmbedding>()
                              .AsQueryable()
                              .Where(e => e.AssistantId == id)
                              .OrderByDescending(e => e.UploadedAtUtc)
                              .ToList();
                ViewBag.Embeddings = list;
            }
            catch { ViewBag.EmbCount = 0; }

            // Admins: shareable groups = all existing groups, not only current assistant groups
            try
            {
                var shareGroups = _db.Groups
                    .AsEnumerable()
                    .Select(g => g?.Name?.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s!)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                ViewBag.ShareGroups = shareGroups;
            }
            catch
            {
                ViewBag.ShareGroups = new List<string>();
            }

            try
            {
                var now = DateTime.UtcNow;
                bool assistantIsShared = _db.AssistantShareLinks
                    .Any(l => l.AssistantId == id && l.IsActive && (!l.ExpiresAtUtc.HasValue || l.ExpiresAtUtc.Value > now));
                ViewBag.AssistantIsShared = assistantIsShared;
            }
            catch
            {
                ViewBag.AssistantIsShared = false;
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAvatar(int id, IFormFile? avatarFile, string? avatarUrl)
        {
            var repo = _uow.GetRepository<Assistant>();
            var model = repo.GetFirstOrDefault(a => a.Id == id);
            if (model == null) return NotFound();
            string? finalUrl = null;
            if (avatarFile != null && avatarFile.Length > 0)
            {
                if (!avatarFile.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    Response.StatusCode = 400;
                    return Json(new { ok = false, error = "Bitte eine Bilddatei hochladen." });
                }
                var uploads = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "assistants");
                Directory.CreateDirectory(uploads);
                var fileName = $"{Guid.NewGuid()}{System.IO.Path.GetExtension(avatarFile.FileName)}";
                var filePath = System.IO.Path.Combine(uploads, fileName);
                await using (var stream = System.IO.File.Create(filePath))
                {
                    await avatarFile.CopyToAsync(stream);
                }
                finalUrl = $"~/uploads/assistants/{fileName}";
            }
            else if (!string.IsNullOrWhiteSpace(avatarUrl))
            {
                var av = avatarUrl.Trim();
                if (!av.StartsWith("http", StringComparison.OrdinalIgnoreCase) && av.StartsWith("/"))
                    av = "~" + av;
                finalUrl = av;
            }
            if (string.IsNullOrWhiteSpace(finalUrl))
            {
                Response.StatusCode = 400;
                return Json(new { ok = false, error = "Keine Avatar-Quelle angegeben." });
            }
            model.AvatarUrl = finalUrl;
            repo.Update(model);
            _uow.Save();
            string resolved = finalUrl;
            try { resolved = Url.Content(finalUrl); } catch { }
            return Json(new { ok = true, url = resolved });
        }

        // POST: /Admin/Assistant/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(
            int id,
            string systemPrompt,
            string? name,
            string? description,
            string? avatarUrl,
            string? goals,
            string? authorName,
            string? licenses)
        {
            var repo = _uow.GetRepository<Assistant>();
            var model = repo.GetFirstOrDefault(a => a.Id == id);
            if (model == null) return NotFound();

            // System-Prompt immer als gereinigter Plain-Text speichern
            model.SystemPrompt = CleanSystemPrompt(systemPrompt ?? string.Empty);

            // Name (HTML entfernen, dekodieren)
            if (!string.IsNullOrWhiteSpace(name))
            {
                var cleanName = Regex.Replace(name ?? string.Empty, "<.*?>", string.Empty, RegexOptions.Singleline);
                cleanName = WebUtility.HtmlDecode(cleanName);
                model.Name = cleanName.Trim();
            }

            // Beschreibung (Rohtext/HTML wie geliefert, nur Trimmen / Leere = null)
            model.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();

            // Ziele (aus Admin-Form, CKEditor liefert HTML/Text)
            model.Goals = string.IsNullOrWhiteSpace(goals) ? null : goals.Trim();

            // Owner/Autor (HTML entfernen, dekodieren)
            if (!string.IsNullOrWhiteSpace(authorName))
            {
                var cleanAuthor = Regex.Replace(authorName ?? string.Empty, "<.*?>", string.Empty, RegexOptions.Singleline);
                cleanAuthor = WebUtility.HtmlDecode(cleanAuthor);
                model.AuthorName = cleanAuthor.Trim();
            }
            else
            {
                model.AuthorName = null;
            }

            // Lizenz-Auswahl aus Dropdown
            model.Licenses = string.IsNullOrWhiteSpace(licenses) ? null : licenses.Trim();

            // Avatar-URL ggf. normalisieren
            if (!string.IsNullOrWhiteSpace(avatarUrl))
            {
                var av = avatarUrl.Trim();
                if (!av.StartsWith("http", StringComparison.OrdinalIgnoreCase) && av.StartsWith("/"))
                    av = "~" + av;
                model.AvatarUrl = av;
            }

            repo.Update(model);
            _uow.Save();
            TempData["success"] = "Assistent aktualisiert.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadEmbedding(int id, IFormFile? txtFile, CancellationToken ct)
        {
            if (txtFile == null || txtFile.Length == 0)
            {
                TempData["error"] = "Bitte eine Datei auswählen.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            var repo = _uow.GetRepository<Assistant>();
            var model = repo.GetFirstOrDefault(a => a.Id == id);
            if (model == null) return NotFound();

            var extension = System.IO.Path.GetExtension(txtFile.FileName);
            var isPdf = string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(txtFile.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase);
            var isTxt = string.Equals(extension, ".txt", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(txtFile.ContentType, "text/plain", StringComparison.OrdinalIgnoreCase);

            if (!isPdf && !isTxt)
            {
                TempData["error"] = "Nur .txt und .pdf Dateien werden unterstützt.";
                return RedirectToAction(nameof(Edit), new { id });
            }

            var apiKey = await _resolver.ResolveEmbeddingsKeyAsync(ct);
            var baseFileName = System.IO.Path.GetFileNameWithoutExtension(txtFile.FileName);

            try
            {
                var embClient = new EmbeddingClient("text-embedding-3-small", apiKey);
                var embRepo = _uow.GetRepository<AssistantEmbedding>();
                int stored = 0;

                if (isPdf)
                {
                    using var stream = txtFile.OpenReadStream();
                    var pdfResult = await _pdfExtractor.ExtractTextAsync(stream, ct);

                    if (string.IsNullOrWhiteSpace(pdfResult.FullText))
                    {
                        TempData["error"] = "Die PDF-Datei enthält keinen extrahierbaren Text.";
                        return RedirectToAction(nameof(Edit), new { id });
                    }

                    var pdfChunks = _pdfChunker.ChunkDocument(pdfResult, maxTokensPerChunk: 6000);
                    if (pdfChunks.Count == 0)
                    {
                        TempData["error"] = "PDF konnte nicht in gültige Chunks aufgeteilt werden.";
                        return RedirectToAction(nameof(Edit), new { id });
                    }

                    foreach (var chunk in pdfChunks)
                    {
                        var resp = await embClient.GenerateEmbeddingAsync(chunk.Content, cancellationToken: ct);
                        var vector = resp.Value.ToFloats();
                        var json = JsonSerializer.Serialize(vector);

                        embRepo.Add(new AssistantEmbedding
                        {
                            AssistantId = id,
                            SourceFileName = $"{baseFileName}_{chunk.SourceInfo}.pdf",
                            Content = chunk.Content,
                            EmbeddingJson = json,
                            UploadedAtUtc = DateTime.UtcNow
                        });
                        stored++;
                    }
                }
                else
                {
                    string text;
                    using (var sr = new StreamReader(txtFile.OpenReadStream()))
                    {
                        text = await sr.ReadToEndAsync();
                    }
                    text = (text ?? string.Empty).Trim();

                    if (text.Length == 0)
                    {
                        TempData["error"] = "Die Datei ist leer.";
                        return RedirectToAction(nameof(Edit), new { id });
                    }

                    var chunks = SplitIntoChunks(text, maxTokensPerChunk: 6000);
                    if (chunks.Count == 0)
                    {
                        TempData["error"] = "Text konnte nicht in gültige Chunks aufgeteilt werden.";
                        return RedirectToAction(nameof(Edit), new { id });
                    }

                    foreach (var (chunk, index) in chunks.Select((c, i) => (c, i)))
                    {
                        var resp = await embClient.GenerateEmbeddingAsync(chunk, cancellationToken: ct);
                        var vector = resp.Value.ToFloats();
                        var json = JsonSerializer.Serialize(vector);

                        embRepo.Add(new AssistantEmbedding
                        {
                            AssistantId = id,
                            SourceFileName = $"{baseFileName}_teil{index + 1}.txt",
                            Content = chunk,
                            EmbeddingJson = json,
                            UploadedAtUtc = DateTime.UtcNow
                        });
                        stored++;
                    }
                }

                _uow.Save();

                var msg = stored == 1
                    ? "Embedding gespeichert."
                    : $"{stored} Embeddings gespeichert (Dokument wurde aufgeteilt).";
                TempData["success"] = msg;
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Embedding fehlgeschlagen: {ex.Message}";
            }

            return RedirectToAction(nameof(Edit), new { id });
        }

        // Approximate token-based splitter (naïve: split by double newlines; then enforce ~7.5k tokens by char count)
        private static List<string> SplitIntoChunks(string text, int maxTokensPerChunk)
        {
            var chunks = new List<string>();
            if (string.IsNullOrWhiteSpace(text)) return chunks;

            // Rough token estimate: 1 token ≈ 4 characters (German/English)
            int maxCharsPerChunk = maxTokensPerChunk * 4;

            // Preserve paragraphs where possible
            var paragraphs = text.Split(new[] { "\r\n\r\n", "\n\n" }, StringSplitOptions.None);
            var current = new System.Text.StringBuilder();
            int currentLength = 0;

            foreach (var para in paragraphs)
            {
                var trimmed = para.Trim();
                if (trimmed.Length == 0) continue;

                // If adding this paragraph would exceed limit, flush current and start new
                if (currentLength + trimmed.Length + 4 > maxCharsPerChunk) // +4 for two newlines
                {
                    if (current.Length > 0)
                    {
                        chunks.Add(current.ToString().Trim());
                        current.Clear();
                        currentLength = 0;
                    }
                    // If a single paragraph is longer than the limit, split it bluntly
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

        private static string CleanSystemPrompt(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;
            var s = input;
            s = Regex.Replace(s, @"<\s*br\s*/?\s*>", "\n", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "<.*?>", string.Empty, RegexOptions.Singleline);
            s = WebUtility.HtmlDecode(s);
            s = s.Replace("\r\n", "\n").Replace("\r", "\n");
            return s.Trim();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteEmbedding(int id, int embId)
        {
            try
            {
                var repo = _uow.GetRepository<AssistantEmbedding>();
                var item = repo.GetFirstOrDefault(e => e.Id == embId);
                if (item == null || item.AssistantId != id)
                {
                    TempData["error"] = "Embedding nicht gefunden.";
                    return RedirectToAction(nameof(Edit), new { id });
                }
                repo.Remove(item);
                _uow.Save();
                TempData["success"] = "Embedding gelöscht.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Löschen fehlgeschlagen: {ex.Message}";
            }
            return RedirectToAction(nameof(Edit), new { id });
        }

        [HttpGet]
        public IActionResult DownloadEmbedding(int id, int embId)
        {
            var repo = _uow.GetRepository<AssistantEmbedding>();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var aRepo = _uow.GetRepository<Assistant>();
                var assistant = aRepo.GetFirstOrDefault(a => a.Id == id);
                if (assistant == null)
                {
                    TempData["error"] = "Assistent nicht gefunden.";
                    return RedirectToAction(nameof(Index));
                }

                // Remove embeddings linked to this assistant to satisfy FK constraints
                var eRepo = _uow.GetRepository<AssistantEmbedding>();
                var embs = eRepo.GetAll().Where(e => e.AssistantId == id).ToList();
                foreach (var e in embs) eRepo.Remove(e);

                aRepo.Remove(assistant);
                _uow.Save();
                TempData["success"] = "Assistent gelöscht.";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Löschen fehlgeschlagen: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
