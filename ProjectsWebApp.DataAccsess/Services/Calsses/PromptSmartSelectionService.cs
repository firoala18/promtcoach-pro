using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using OpenAI.Embeddings;
using OpenAI;
using System.ClientModel;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using ProjectsWebApp.Models;
using Dto.PromptFilters;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Net.Http;

namespace ProjectsWebApp.DataAccsess.Services.Calsses
{
    public sealed class PromptSmartSelectionService : IPromptSmartSelectionService
    {
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _cfg;
        private readonly ISmartSelectionProgressNotifier _progress;
        private readonly ISmartSelectionProgressStore _store;
        private readonly IHttpContextAccessor _http;
        private readonly IAiProviderResolver _resolver;

        public PromptSmartSelectionService(
            ApplicationDbContext db,
            IConfiguration cfg,
            ISmartSelectionProgressNotifier progress,
            ISmartSelectionProgressStore store,
            IHttpContextAccessor http,
            IAiProviderResolver resolver)
        {
            _db = db;
            _cfg = cfg;
            _progress = progress;
            _store = store;
            _http = http;
            _resolver = resolver;
        }

        private static HashSet<string> ExtractKeywordsFromForm(PromptFormDto form)
        {
            var sb = new StringBuilder();
            void add(string? s) { if (!string.IsNullOrWhiteSpace(s)) sb.Append(' ').Append(s); }
            add(form.Titel); add(form.Thema); add(form.Ziele); add(form.Beschreibung);
            var raw = sb.ToString().ToLowerInvariant();
            var stop = new HashSet<string>(new[] { "prompt", "prompts", "text", "texte", "thema", "ziel", "ziele", "kontext", "struktur" });
            var words = Regex.Matches(raw, "[a-zäöüß]{4,}")
                             .Select(m => m.Value)
                             .Where(w => !stop.Contains(w))
                             .Select(Stem)
                             .Distinct()
                             .Take(24) // limit keywords
                             .ToHashSet();
            return words;
        }

        private static string Stem(string w)
        {
            // very crude German-ish stemmer: lower, strip common suffixes
            if (string.IsNullOrWhiteSpace(w)) return string.Empty;
            w = w.ToLowerInvariant();
            var repl = w
                .Replace("ä", "a").Replace("ö", "o").Replace("ü", "u").Replace("ß", "ss");
            string[] suf = { "ungen", "ieren", "ation", "tionen", "ungen", "lich", "keit", "keit", "heit", "ungen", "ische", "isch", "ieren", "ierung", "ungen", "ungen", "ungen", "ender", "ende", "endes", "ungen", "ungen", "ern", "em", "en", "er", "es", "e" };
            foreach (var s in suf) if (repl.EndsWith(s) && repl.Length - s.Length >= 4) { repl = repl.Substring(0, repl.Length - s.Length); break; }
            return repl.Length >= 3 ? repl : w;
        }

        private static HashSet<string> TokenStems(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return new HashSet<string>();
            var raw = text.ToLowerInvariant();
            var toks = Regex.Matches(raw, "[a-zäöüß]{3,}").Select(m => Stem(m.Value));
            return toks.Where(t => t.Length >= 3).ToHashSet();
        }

        public Task<SmartSelectionResultDto> SelectAsync(PromptFormDto form, PromptType targetType, CancellationToken ct = default)
            => SelectCoreAsync(form, targetType, deep: false, opId: null, modelOverride: null, ct);

        public Task<SmartSelectionResultDto> SelectAsync(PromptFormDto form, PromptType targetType, string opId, CancellationToken ct = default)
            => SelectCoreAsync(form, targetType, deep: false, opId: opId, modelOverride: null, ct);

        public Task<SmartSelectionResultDto> SelectAsync(PromptFormDto form, PromptType targetType, bool deep, CancellationToken ct = default)
            => SelectCoreAsync(form, targetType, deep: deep, opId: null, modelOverride: null, ct);

        public Task<SmartSelectionResultDto> SelectAsync(PromptFormDto form, PromptType targetType, bool deep, string opId, CancellationToken ct = default)
            => SelectCoreAsync(form, targetType, deep: deep, opId: opId, modelOverride: null, ct);

        public Task<SmartSelectionResultDto> SelectWithModelAsync(PromptFormDto form, PromptType targetType, bool deep, string? modelIdOverride, CancellationToken ct = default)
            => SelectCoreAsync(form, targetType, deep: deep, opId: null, modelOverride: modelIdOverride, ct);

        public Task<SmartSelectionResultDto> SelectWithModelAsync(PromptFormDto form, PromptType targetType, bool deep, string? modelIdOverride, string opId, CancellationToken ct = default)
            => SelectCoreAsync(form, targetType, deep: deep, opId: opId, modelOverride: modelIdOverride, ct);

        private async Task<SmartSelectionResultDto> SelectCoreAsync(PromptFormDto form, PromptType targetType, bool deep, string? opId, string? modelOverride, CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            // Stage 0: Start
            if (!string.IsNullOrWhiteSpace(opId))
                _progress.Report(opId, SmartSelectionStage.Start, "start", "", 0, 0);

            // 1) Build chat request per spec (DB‑configurable)
            var sys = await GetSystemPromptAsync(ct);
            var userPrompt = await GetUserPromptAsync(form, ct);

            // Embedding API key (always global Admin/ApiKeys OpenAI)
            var apiKey = await LoadEmbeddingKeyAsync(ct);
            // Resolve chat provider (group override → global) via centralized resolver
            string? uid = null;
            try { uid = _http.HttpContext?.User?.FindFirstValue(ClaimsIdentity.DefaultNameClaimType) ?? _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier); } catch { }
            var chatRes = await _resolver.ResolveChatAsync(uid, modelOverride, ct);
            var baseUrl = chatRes.BaseUrl;
            var chatKey = chatRes.ApiKey;
            var chatModel = chatRes.ModelId;
            Console.WriteLine($"[SmartSelection] Provider resolved: {(baseUrl.StartsWith("gemini://", StringComparison.OrdinalIgnoreCase) ? "gemini" : (baseUrl.StartsWith("claude://", StringComparison.OrdinalIgnoreCase) ? "claude" : "openai-compatible"))} model={chatModel} endpoint={baseUrl}");
            OpenAIClient? openai = null;
            ChatClient? chatClient = null;
            if (!baseUrl.StartsWith("gemini://", StringComparison.OrdinalIgnoreCase) && !baseUrl.StartsWith("claude://", StringComparison.OrdinalIgnoreCase))
            {
                if (baseUrl.Contains("chat-ai.academiccloud.de", StringComparison.OrdinalIgnoreCase))
                    openai = new OpenAIClient(new ApiKeyCredential(chatKey), new OpenAIClientOptions { Endpoint = new Uri(baseUrl) });
                else
                    openai = new OpenAIClient(new ApiKeyCredential(chatKey));
                chatClient = openai.GetChatClient(chatModel);
            }

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(sys),
                new UserChatMessage(userPrompt)
            };

            // Stage 3: Auswahl senden
            if (!string.IsNullOrWhiteSpace(opId))
                _progress.Report(opId, SmartSelectionStage.SelectionSending, "selection", "", 0, 0);

            string raw = string.Empty;
            try
            {
                if (baseUrl.StartsWith("gemini://", StringComparison.OrdinalIgnoreCase))
                {
                    // Gemini REST call
                    var http = new HttpClient();
                    var model = string.IsNullOrWhiteSpace(chatModel) ? "gemini-2.5-flash" : chatModel;
                    if (string.Equals(model, "gemini-pro", StringComparison.OrdinalIgnoreCase) || model.Contains("pro", StringComparison.OrdinalIgnoreCase))
                        model = "gemini-2.5-pro";
                    else if (string.Equals(model, "gemini-flash", StringComparison.OrdinalIgnoreCase) || model.Contains("flash", StringComparison.OrdinalIgnoreCase))
                        model = "gemini-2.5-flash";
                    var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={chatKey}";
                    var payload = new
                    {
                        contents = new[]
                        {
                            new { role = "user", parts = new[] { new { text = sys + "\n\n" + userPrompt } } }
                        },
                        generationConfig = new { temperature = 0, topP = 1 }
                    };
                    var json = JsonSerializer.Serialize(payload);
                    using var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var resp = await http.PostAsync(url, content, ct);
                    var respText = await resp.Content.ReadAsStringAsync(ct);
                    if (!resp.IsSuccessStatusCode)
                        throw new InvalidOperationException($"Gemini API Fehler: {(int)resp.StatusCode} {resp.ReasonPhrase} -> {respText}");
                    using var doc = JsonDocument.Parse(respText);
                    raw = doc.RootElement
                             .GetProperty("candidates")[0]
                             .GetProperty("content")
                             .GetProperty("parts")[0]
                             .GetProperty("text")
                             .GetString() ?? string.Empty;
                }
                else if (baseUrl.StartsWith("claude://", StringComparison.OrdinalIgnoreCase))
                {
                    var http = new HttpClient();
                    var model = string.IsNullOrWhiteSpace(chatModel) ? "sonnet-4" : chatModel;
                    // Map common doc aliases to real Anthropic IDs first
                    if (string.Equals(model, "claude-sonnet-4-5", StringComparison.OrdinalIgnoreCase)) model = "claude-3-5-sonnet-20241022";
                    else if (string.Equals(model, "claude-haiku-4-5", StringComparison.OrdinalIgnoreCase)) model = "claude-3-5-haiku-20241022";
                    // If it's already a Claude ID, accept as-is
                    if (model.StartsWith("claude-", StringComparison.OrdinalIgnoreCase))
                    {
                        // already an Anthropic model ID/alias (e.g., claude-sonnet-4-5, claude-3-5-sonnet-20241022)
                    }
                    else if (string.Equals(model, "sonnet-4.5", StringComparison.OrdinalIgnoreCase)) model = "claude-3-5-sonnet-20241022";
                    else if (string.Equals(model, "sonnet-4", StringComparison.OrdinalIgnoreCase)) model = "claude-3-sonnet-20240229";
                    else if (string.Equals(model, "haiku-4.5", StringComparison.OrdinalIgnoreCase)) model = "claude-haiku-4-5";
                    else if (string.Equals(model, "haiku-4", StringComparison.OrdinalIgnoreCase) || string.Equals(model, "haiku", StringComparison.OrdinalIgnoreCase)) model = "claude-3-haiku-20240307";
                    var url = "https://api.anthropic.com/v1/messages";
                    using var reqMsg = new HttpRequestMessage(HttpMethod.Post, url);
                    reqMsg.Headers.Add("x-api-key", chatKey);
                    reqMsg.Headers.Add("anthropic-version", "2023-06-01");
                    Console.WriteLine($"[SmartSelection] Claude request model={model}");
                    var payload = new
                    {
                        model = model,
                        system = sys,
                        max_tokens = 1024,
                        temperature = 0,
                        top_p = 1,
                        messages = new[]
                        {
                            new {
                                role = "user",
                                content = new object[] { new { type = "text", text = userPrompt } }
                            }
                        }
                    };
                    var json = JsonSerializer.Serialize(payload);
                    reqMsg.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    var resp = await http.SendAsync(reqMsg, ct);
                    var respText = await resp.Content.ReadAsStringAsync(ct);
                    if (!resp.IsSuccessStatusCode)
                        throw new InvalidOperationException($"Claude API Fehler: {(int)resp.StatusCode} {resp.ReasonPhrase} -> {respText}");
                    using var doc = JsonDocument.Parse(respText);
                    var arr = doc.RootElement.GetProperty("content");
                    raw = arr.GetArrayLength() > 0 ? (arr[0].TryGetProperty("text", out var t) ? t.GetString() ?? string.Empty : string.Empty) : string.Empty;
                    Console.WriteLine($"[SmartSelection] Claude response len={raw?.Length ?? 0}");
                }
                else
                {
                    ChatCompletionOptions opts;
                    if (!string.IsNullOrWhiteSpace(chatModel) && chatModel.StartsWith("gpt-5", StringComparison.OrdinalIgnoreCase))
                    {
                        opts = new ChatCompletionOptions();
                    }
                    else
                    {
                        opts = new ChatCompletionOptions { Temperature = 0, TopP = 1 };
                    }
                    try
                    {
                        var result = await chatClient!.CompleteChatAsync(messages, opts, cancellationToken: ct);
                        var chat = result.Value;
                        raw = chat?.Content?.FirstOrDefault()?.Text ?? string.Empty;
                    }
                    catch (ClientResultException cre) when ((cre.Status == 404 || cre.Status == 401) && baseUrl.Contains("chat-ai.academiccloud.de", StringComparison.OrdinalIgnoreCase))
                    {
                        var altBase = baseUrl.EndsWith("/v1", StringComparison.OrdinalIgnoreCase)
                            ? baseUrl.Substring(0, baseUrl.Length - 3)
                            : (baseUrl.TrimEnd('/') + "/v1");
                        try
                        {
                            var altOpenai = new OpenAIClient(new ApiKeyCredential(chatKey), new OpenAIClientOptions { Endpoint = new Uri(altBase) });
                            var altClient = altOpenai.GetChatClient(chatModel);
                            var result = await altClient.CompleteChatAsync(messages, opts, cancellationToken: ct);
                            var chat = result.Value;
                            raw = chat?.Content?.FirstOrDefault()?.Text ?? string.Empty;
                        }
                        catch
                        {
                            // ignore and try HTTP fallback below
                        }
                        if (string.IsNullOrWhiteSpace(raw))
                        {
                            // HTTP fallback to OpenAI-compatible /chat/completions
                            try
                            {
                                using var http = new HttpClient();
                                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", chatKey);
                                try { http.DefaultRequestHeaders.Add("X-API-KEY", chatKey); } catch { }
                                try { http.DefaultRequestHeaders.Add("x-api-key", chatKey); } catch { }
                                var bases = new[] {
                                    baseUrl.TrimEnd('/'),
                                    altBase.TrimEnd('/'),
                                    baseUrl.TrimEnd('/') + "/openai/v1",
                                    (baseUrl.EndsWith("/v1", StringComparison.OrdinalIgnoreCase) ? baseUrl.Substring(0, baseUrl.Length - 3) : baseUrl.TrimEnd('/')) + "/openai/v1"
                                };
                                foreach (var b in bases.Distinct(StringComparer.OrdinalIgnoreCase))
                                {
                                    var url = b + "/chat/completions";
                                    var payload = new
                                    {
                                        model = chatModel,
                                        temperature = 0,
                                        top_p = 1,
                                        messages = new object[] {
                                            new { role = "system", content = sys },
                                            new { role = "user", content = userPrompt }
                                        }
                                    };
                                    var json = JsonSerializer.Serialize(payload);
                                    using var content = new StringContent(json, Encoding.UTF8, "application/json");
                                    var resp = await http.PostAsync(url, content, ct);
                                    var respText = await resp.Content.ReadAsStringAsync(ct);
                                    if (!resp.IsSuccessStatusCode) continue;
                                    try
                                    {
                                        using var doc = JsonDocument.Parse(respText);
                                        var ch0 = doc.RootElement.GetProperty("choices")[0];
                                        var msg = ch0.GetProperty("message");
                                        if (msg.TryGetProperty("content", out var cont) && cont.ValueKind == JsonValueKind.String)
                                        {
                                            raw = cont.GetString() ?? string.Empty; break;
                                        }
                                    }
                                    catch { /* try next base */ }
                                }
                            }
                            catch { /* swallow and let outer error surface if empty */ }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Chat-Aufruf fehlgeschlagen: " + ex.Message, ex);
            }

            if (!string.IsNullOrWhiteSpace(opId))
                _progress.Report(opId, SmartSelectionStage.SelectionReceived, "selection", "", 0, 0);
            var techniques = ParseTechniqueLines(raw);

            // If empty, build a fallback technique from the raw user input; if still empty, return no suggestions
            if (techniques.Count == 0)
            {
                var sbf = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(form.Titel)) sbf.AppendLine(form.Titel);
                if (!string.IsNullOrWhiteSpace(form.Thema)) sbf.AppendLine(form.Thema);
                if (!string.IsNullOrWhiteSpace(form.Ziele)) sbf.AppendLine(form.Ziele);
                if (!string.IsNullOrWhiteSpace(form.Beschreibung)) sbf.AppendLine(form.Beschreibung);
                var fallback = sbf.ToString().Trim();
                if (fallback.Length >= 5) techniques.Add(fallback);
                else
                {
                    return new SmartSelectionResultDto
                    {
                        Categories = new List<SelectedCategoryDto>(),
                        CoverageScore = 0
                    };
                }
            }

            // Stage 5: Resolved (IDs/lines parsed)
            if (!string.IsNullOrWhiteSpace(opId))
                _progress.Report(opId, SmartSelectionStage.Resolved, "resolved", "", 0, techniques.Count);

            // Stage 6: Bewertung senden (embedding/search)
            if (!string.IsNullOrWhiteSpace(opId))
                _progress.Report(opId, SmartSelectionStage.EvaluationSending, "scoring", "", 0, techniques.Count);

            // 2) Embed techniques
            var embClient = new EmbeddingClient("text-embedding-3-small", apiKey);
            var techVectors = new List<float[]>();
            foreach (var line in techniques)
            {
                var resp = await embClient.GenerateEmbeddingAsync(line, cancellationToken: ct);
                techVectors.Add(resp.Value.ToFloats().ToArray());
            }

            // 3) Load candidate filter items for target type (include system + user categories)
            var items = await _db.Set<FilterItem>()
                                 .Join(_db.Set<FilterCategory>(), i => i.FilterCategoryId, c => c.Id, (i, c) => new { i, c })
                                 .Where(x => x.c.Type == targetType)
                                 .Select(x => new { Item = x.i, Cat = x.c })
                                 .ToListAsync(ct);

            var idStrings = items.Select(x => x.Item.Id.ToString()).ToHashSet();
            var indexEntries = await _db.SemanticIndexEntries
                                        .AsNoTracking()
                                        .Where(e => e.EntityType == "FilterItem" && idStrings.Contains(e.EntityId))
                                        .ToListAsync(ct);
            var entryById = indexEntries.ToDictionary(e => int.Parse(e.EntityId));

            // Build or compute item vectors for ALL items (compute on-the-fly if index missing)
            var itemVectorById = new Dictionary<int, float[]>();
            var itemNormById = new Dictionary<int, double>();
            var keywords = ExtractKeywordsFromForm(form);
            var joinedText = new StringBuilder(); joinedText.Append(form.Titel).Append(' ').Append(form.Thema).Append(' ').Append(form.Ziele).Append(' ').Append(form.Beschreibung);
            var inputJoined = joinedText.ToString().Trim();
            var inputStems = TokenStems(inputJoined);
            var shortInput = inputJoined.Length <= 100 || keywords.Count <= 5;
            var assessmentStems = new HashSet<string>(new[] { Stem("prüfung"), Stem("klausur"), Stem("exam"), Stem("quiz"), Stem("test") });
            foreach (var row in items)
            {
                var itemId = row.Item.Id;
                // Build lexical basis and gating first
                var basis = new StringBuilder();
                basis.Append(row.Item.Title ?? string.Empty);
                if (!string.IsNullOrWhiteSpace(row.Item.Instruction))
                {
                    basis.Append(' ');
                    basis.Append(row.Item.Instruction);
                }
                var basisStr = basis.ToString();
                var itemStems = TokenStems(basisStr + " " + (row.Cat?.Name ?? string.Empty));
                int overlap = keywords.Count == 0 ? 1 : keywords.Intersect(itemStems).Count();

                // Strict gating on short/simple inputs: skip items with zero lexical overlap
                if (shortInput && overlap <= 0)
                    continue;

                // For very short inputs, demand stronger lexical evidence (>=2 overlaps)
                if (shortInput && overlap < 2)
                    continue;

                // Block assessment-like items unless the input itself mentions assessment terms
                bool itemLooksAssessment = itemStems.Intersect(assessmentStems).Any();
                bool inputMentionsAssessment = inputStems.Intersect(assessmentStems).Any();
                if (shortInput && itemLooksAssessment && !inputMentionsAssessment)
                    continue;

                if (entryById.TryGetValue(itemId, out var entry) && !string.IsNullOrWhiteSpace(entry.EmbeddingJson))
                {
                    try
                    {
                        var vec = JsonSerializer.Deserialize<float[]>(entry.EmbeddingJson) ?? Array.Empty<float>();
                        if (vec.Length > 0) { itemVectorById[itemId] = vec; continue; }
                    }
                    catch { /* fall through to compute */ }
                }

                // Compute on-the-fly from item text if missing in index (after gating)
                try
                {
                    var resp = await embClient.GenerateEmbeddingAsync(basisStr, cancellationToken: ct);
                    var vec = resp.Value.ToFloats().ToArray();
                    if (vec.Length > 0) itemVectorById[itemId] = vec;
                }
                catch { /* ignore items that cannot be embedded right now */ }
            }

            // Precompute norms for cosine
            foreach (var kv in itemVectorById)
            {
                var v = kv.Value;
                double sum = 0; for (int i = 0; i < v.Length; i++) { var x = v[i]; sum += x * x; }
                itemNormById[kv.Key] = Math.Sqrt(sum);
            }

            // 4) Score similarity for each technique vs items, take top suggestions
            var scores = new Dictionary<int, double>(); // itemId -> best score across techniques
            foreach (var vec in techVectors)
            {
                foreach (var kv in itemVectorById)
                {
                    var itemId = kv.Key;
                    var itemVec = kv.Value;
                    if (itemVec.Length == 0) continue;
                    var s = Cosine(vec, itemVec, itemNormById.TryGetValue(itemId, out var n) ? n : (double?)null);
                    if (s > 0)
                    {
                        if (!scores.TryGetValue(itemId, out var prev) || s > prev)
                            scores[itemId] = s;
                    }
                }
            }

            // Dynamic cutoff: keep items close to best score, not a fixed count
            var filtered = new List<KeyValuePair<int, double>>();
            if (scores.Count > 0)
            {
                double best = scores.Max(kv => kv.Value);
                // Stricter thresholds for short inputs
                double baseThreshold = deep ? (shortInput ? 0.45 : 0.28) : (shortInput ? 0.26 : 0.18);
                double alpha = deep ? (shortInput ? 0.92 : 0.72) : (shortInput ? 0.75 : 0.60);
                double cutoff = Math.Max(baseThreshold, best * alpha);

                filtered = scores
                    .Where(kv => kv.Value >= cutoff)
                    .OrderByDescending(kv => kv.Value)
                    .ToList();

                // Adaptive relaxation if empty
                if (filtered.Count == 0)
                {
                    var alphaSteps = deep
                        ? new[] { 0.70, 0.68, 0.66, 0.64, 0.62, 0.60, 0.58, 0.56 }
                        : new[] { 0.58, 0.56, 0.54, 0.52, 0.50 };
                    foreach (var a in alphaSteps)
                    {
                        cutoff = Math.Max(baseThreshold, best * a);
                        var tmp = scores
                            .Where(kv => kv.Value >= cutoff)
                            .OrderByDescending(kv => kv.Value)
                            .ToList();
                        if (tmp.Count > 0) { filtered = tmp; break; }
                    }
                    // Still empty: take top few regardless of cutoff
                    if (filtered.Count == 0)
                    {
                        var maxFallback = deep ? 20 : 25;
                        filtered = scores
                            .OrderByDescending(kv => kv.Value)
                            .Take(Math.Min(maxFallback, Math.Max(1, scores.Count)))
                            .ToList();
                    }
                }

                // Final cap: 0..20 suggestions; stricter cap on short inputs for precision
                filtered = filtered.Take(shortInput ? 5 : 20).ToList();
            }
            var topIds = filtered.Select(kv => kv.Key).ToHashSet();

            var picked = items.Where(x => topIds.Contains(x.Item.Id)).ToList();
            var byCat = picked.GroupBy(x => x.Cat).ToList();

            var resultCats = new List<SelectedCategoryDto>();
            foreach (var g in byCat)
            {
                var cat = g.Key;
                var list = new List<SelectedItemDto>();
                foreach (var row in g)
                {
                    list.Add(new SelectedItemDto
                    {
                        ItemId = row.Item.Id,
                        ItemName = row.Item.Title,
                        Confidence = scores.TryGetValue(row.Item.Id, out var sc) ? sc : 0
                    });
                }
                resultCats.Add(new SelectedCategoryDto
                {
                    CategoryId = cat.Id,
                    CategoryName = cat.Name,
                    Items = list
                });
            }

            if (!string.IsNullOrWhiteSpace(opId))
                _progress.Report(opId, SmartSelectionStage.EvaluationReceived, "scoring", "", techniques.Count, techniques.Count);

            var coverage = resultCats.Sum(c => c.Items.Count) / (double)Math.Max(1, items.Count);

            // Stage 8: Done
            if (!string.IsNullOrWhiteSpace(opId))
                _progress.Report(opId, SmartSelectionStage.Done, "done", "", techniques.Count, techniques.Count);

            return new SmartSelectionResultDto
            {
                Categories = resultCats,
                CoverageScore = coverage
            };
        }

        private async Task<string> GetSystemPromptAsync(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            string? sys = null;
            try
            {
                sys = await _db.PromptAiSettings
                               .Where(x => x.SmartSelectionSystemPreamble != null && x.SmartSelectionSystemPreamble != "")
                               .OrderByDescending(x => x.UpdatedAt)
                               .Select(x => x.SmartSelectionSystemPreamble)
                               .FirstOrDefaultAsync(ct);
            }
            catch { sys = null; }

            // Group-level override if UseGlobal is false
            try
            {
                string? groupOverride = null;
                try { groupOverride = _http.HttpContext?.Items["PromptAiGroupOverride"] as string; } catch { }

                string? groupName = null;
                if (!string.IsNullOrWhiteSpace(groupOverride))
                {
                    groupName = groupOverride.Trim();
                }
                else
                {
                    string? uid = null; try { uid = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier); } catch { }
                    if (!string.IsNullOrWhiteSpace(uid))
                    {
                        var grp = await _db.UserGroupMemberships
                                           .Where(m => m.UserId == uid)
                                           .OrderByDescending(m => m.CreatedAt)
                                           .Select(m => m.Group)
                                           .FirstOrDefaultAsync(ct);
                        groupName = string.IsNullOrWhiteSpace(grp) ? "Ohne Gruppe" : grp!.Trim();
                    }
                }

                // "Ohne Gruppe" acts as a global-selection sentinel; do not
                // attempt any group-level override for this pseudo-group.
                bool isNoGroup = string.Equals(groupName, "Ohne Gruppe", StringComparison.OrdinalIgnoreCase);

                if (!string.IsNullOrWhiteSpace(groupName) && !isNoGroup)
                {
                    var gp = await _db.GroupPromptAiSettings
                                      .Where(g => g.Group == groupName)
                                      .OrderByDescending(g => g.UpdatedAt)
                                      .FirstOrDefaultAsync(ct);
                    var useGlobal = gp?.UseGlobal ?? true;
                    if (!useGlobal && gp != null && !string.IsNullOrWhiteSpace(gp.SmartSelectionSystemPreamble))
                        return gp.SmartSelectionSystemPreamble.Trim();
                }
            }
            catch { }

            if (!string.IsNullOrWhiteSpace(sys)) return sys.Trim();

            return @"Du bist Experte für KI-Prompt-Engineering, kognitive Metaanalyse und didaktische Strukturlogik.
Analysiere jede Eingabe als ganzheitliches Prompt-System: identifiziere die primäre Prompt-Technik (Leitfunktion, Zielabsicht) und die sekundären Prompt-Techniken (Struktur-, Steuer- und Stilmechanismen) in der satzbasierten Eingabe. Wandle jede freie oder unscharfe Eingabe in einen strukturierten, anwendungsreifen Prompt um, der Intention, Kontext und Zielstruktur präzise und kohärent abbildet.

Ziel ist die Erzeugung klarer, nachvollziehbarer und reproduzierbarer Prompts, die semantisch kohärent sind, wissenschaftlichen Standards genügen und sich gezielt im Kontext großer Sprachmodelle einsetzen lassen.

Ausgabeformat:
(Prompt Technique in English): Deutsche Anweisung. <br/>

Richtlinien:

- Verwende keine Aufzählungen oder Nummerierungen.
- Die Prompt-Techniken sollen in ihrem offiziellen englischen Fachbegriff stehen, während die zugehörige Beschreibung auf Deutsch formuliert wird.
- Jede Zeile beschreibt eine einzelne, sofort anwendbare Prompt-Technik.
- Jede Prompt-Technik soll in einer einzigen Zeile stehen, ohne Zeilenumbrüche innerhalb der Beschreibung.

#Verwende bei kausalen oder analytischen Ausführungen ein Role Prompt mit Role-Playing (RPP) und Audience Prompting (AuP), um Rolle, Identität und Kontext des Nutzers zu erfassen.
Wenn die Eingabe eine Rollenformulierung wie „Du bist ein…“ enthält, interpretiere sie als Rollendefinition und erweitere sie nach RPP/AuP.
Frage zu Beginn dialogisch nach Name, Thema und Zielgruppe und warte nach jeder Antwort, bevor du fortfährst.";
        }

        private async Task<string> GetUserPromptAsync(PromptFormDto form, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            string? firstLine = null;
            try
            {
                firstLine = await _db.PromptAiSettings
                                     .OrderByDescending(x => x.UpdatedAt)
                                     .Select(x => x.SmartSelectionUserPrompt)
                                     .FirstOrDefaultAsync(ct);
            }
            catch { firstLine = null; }

            // Group-level override if UseGlobal is false
            try
            {
                string? groupOverride = null;
                try { groupOverride = _http.HttpContext?.Items["PromptAiGroupOverride"] as string; } catch { }

                string? groupName = null;
                if (!string.IsNullOrWhiteSpace(groupOverride))
                {
                    groupName = groupOverride.Trim();
                }
                else
                {
                    string? uid = null; try { uid = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier); } catch { }
                    if (!string.IsNullOrWhiteSpace(uid))
                    {
                        var grp = await _db.UserGroupMemberships
                                           .Where(m => m.UserId == uid)
                                           .OrderByDescending(m => m.CreatedAt)
                                           .Select(m => m.Group)
                                           .FirstOrDefaultAsync(ct);
                        groupName = string.IsNullOrWhiteSpace(grp) ? "Ohne Gruppe" : grp!.Trim();
                    }
                }

                if (!string.IsNullOrWhiteSpace(groupName))
                {
                    var gp = await _db.GroupPromptAiSettings
                                      .Where(g => g.Group == groupName)
                                      .OrderByDescending(g => g.UpdatedAt)
                                      .FirstOrDefaultAsync(ct);
                    var useGlobal = gp?.UseGlobal ?? true;
                    if (!useGlobal && gp != null && !string.IsNullOrWhiteSpace(gp.SmartSelectionUserPrompt))
                        firstLine = gp.SmartSelectionUserPrompt.Trim();
                }
            }
            catch { }

            if (string.IsNullOrWhiteSpace(firstLine))
            {
                firstLine = "Wandle die folgende Eingabe in eine strukturierte und wissenschaftlicher Prompt-Techniken um, basierend auf akademischen Frameworks des Prompt Engineerings, drop-in ready für LLMs.";
            }

            var sb = new StringBuilder();
            sb.AppendLine("user prompt:");
            sb.AppendLine(firstLine);
            sb.AppendLine();
            sb.AppendLine("EINGABE");
            sb.AppendLine($"Titel: {form.Titel}");
            sb.AppendLine($"Thema: {form.Thema}");
            sb.AppendLine($"Ziele: {form.Ziele}");
            sb.AppendLine($"Beschreibung: {form.Beschreibung}");
            return sb.ToString();
        }

        private static List<string> ParseTechniqueLines(string text)
        {
            var list = new List<string>();
            if (string.IsNullOrWhiteSpace(text)) return list;
            foreach (var raw in text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var line = raw.Trim();
                if (line.Length < 5) continue;
                // Expect format: Name: Deutsche Anweisung. <br/>
                // Strip HTML line breaks
                line = Regex.Replace(line, @"<br\s*/?>", string.Empty, RegexOptions.IgnoreCase).Trim();
                // Keep only single-line
                line = Regex.Replace(line, @"\s+", " ").Trim();
                // Ensure contains ':'
                if (!line.Contains(':')) continue;
                list.Add(line);
            }
            return list;
        }

        private static double Cosine(float[] a, float[] b, double? bNorm)
        {
            int n = Math.Min(a.Length, b.Length);
            if (n == 0) return 0;
            double dot = 0, an = 0, bn = 0;
            for (int i = 0; i < n; i++) { var x = a[i]; var y = b[i]; dot += x * y; an += x * x; }
            bn = bNorm ?? Math.Sqrt(b.Take(n).Select(v => (double)v * v).Sum());
            if (an == 0 || bn == 0) return 0;
            return dot / (Math.Sqrt(an) * bn);
        }

        private async Task<string> LoadEmbeddingKeyAsync(CancellationToken ct)
        {
            return await _resolver.ResolveEmbeddingsKeyAsync(ct);
        }

        private async Task<(string baseUrl, string apiKey, string model)> ResolveChatProviderAsync(string? modelOverride, CancellationToken ct)
        {
            string? uid = null;
            try { uid = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier); } catch { }
            var res = await _resolver.ResolveChatAsync(uid, modelOverride, ct);
            return (res.BaseUrl, res.ApiKey, res.ModelId);
        }

    }

    // ... rest of the code remains the same ...
    public sealed class SmartSelectionResultDto
    {
        public List<SelectedCategoryDto> Categories { get; set; } = new();
        public double CoverageScore { get; set; }
    }

    public sealed class SelectedCategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<SelectedItemDto> Items { get; set; } = new();
    }

    public sealed class SelectedItemDto
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public double Confidence { get; set; }
    }

    // Uses SmartSelectionStage defined in Interfaces namespace for progress
}
