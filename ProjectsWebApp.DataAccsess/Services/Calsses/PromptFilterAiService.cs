using Dto;
using Dto.PromptFilters;
using Microsoft.Extensions.Configuration;
using OpenAI.Chat;
using OpenAI;
using System.ClientModel;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.DataAccsess.Services.Interfaces;
using ProjectsWebApp.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ProjectsWebApp.DataAccsess.Services.Calsses
{
    public sealed class PromptFilterAiService : IPromptFilterAiService
    {
        private readonly string _modelId;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _cfg;
        private readonly IHttpContextAccessor _http;
        private readonly IAiProviderResolver _resolver;

        private static readonly JsonSerializerOptions HashOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        // === MASTERLISTE (vollständig, nur für serverseitige Auswahl; NICHT 1:1 in den Prompt senden!) ===
        // Hinweis: Wir bilden daraus eine kurze Top-K-Liste (token-sparend). Das Modell darf explizit
        // weitere relevante Techniken ergänzen (siehe System-Prompt).
        private static readonly string[] MasterTechniques = new[]
        {
            "Active Prompt","Adversarial Prompting","Adaptive Teaching Prompting (ATP)","Agentic Prompting (AP)","Algorithmic Prompting","Anchor Prompting","Analogy Prompting","Audience Prompting","Auto-CoT (Automatic Chain-of-Thought)","Auto-Prompting","Auto-Refine Prompting (ARP)","Automatic Prompt Engineer (APE)","Automatic Prompt Optimization (APO)","AUTO-RAG Prompting","Bias-Contrast Prompting (BCP)","Bias-Detection Prompting","Bias-Mitigation Prompting","Boundary Testing Prompting","Brainstorming Prompting","Calibration Prompting","CARE (Context-Action-Result-Example)","Case Study Prompting","Cain’s Framework","Chain-of-Abstraction (CoA)","Chain-of-Code (CoC)","Chain-of-Density (CoD)","Chain-of-Evaluation (CoE)","Chain-of-Feedback (CoF)","Chain-of-Interpretation (CoI)","Chain-of-Knowledge (CoK)","Chain-of-Planning (CoP)","Chain-of-Query (CoQ)","Chain-of-Rationale (CoR)","Chain-of-Retrieval (CoRt)","Chain-of-Review (CoRev)","Chain-of-Simulation (CoSim)","Chain-of-Symbol (CoS)","Chain-of-Table (CoTBL)","Chain-of-Thought (CoT)","Chain-of-Trust (CoT2)","Chain-of-Verification (CoV)","Citation Prompting","Classification Prompting","CLEAR Prompting","Collaborative Story Prompting","Comparison Prompting","Completion Prompting","Complex CoT","Confidence Scoring Prompting","Constraint-based Prompting","Consensus Prompting","Constructive Alignment Prompting","Context Expansion","Contrastive CoT","Contrastive Prompting","Contrastive Self-Consistency","Corpus-Guided Prompting","Counterfactual Prompting","CRISPE Prompting","Cross-Lingual Prompting","Culturally Aligned Prompting (CAP)","Cultural Perspective Prompting","Curriculum Prompting","Data-Guided Prompting","Data-to-Text Prompting","Debate Prompting","Decision-Tree Prompting","Decomposed Prompting","Defense Prompting (Prompt Injection Detection)","Deliberation Prompting","Dialog In-Context Learning","Directional Stimulus Prompting","Domain-Adapted Prompting","Dynamic Prompt Learning","Dynamic Prompting","Emotion Prompt","Emotion-based Prompting","Embedding-based Prompting","Ensemble Refinement (ER)","Equity-Guided Prompting","Error Analysis Prompting","Error Highlighting Prompting","Ethical Reflection Prompting","Evaluation-Guided Prompting","Experiment with Different Prompt Styles","Expert Panel Prompting","Explainability Prompting","Experimental Design Prompting","Fact-Checking Prompting","Fairness-Constrained Prompting (FCP)","Feedback Prompting","Feedback-Loop Prompting","Few-Shot Prompting","Generated Knowledge Prompting (GKP)","Gender-Inclusive Prompting","Graph-of-Thought (GoT)","Hypothesis Prompting","Hypothesis Testing Prompting","Idea Expansion Prompting","Increasingly Rich Shots Prompting","Instruction Prompting","Interactive Dialogue Prompting","Interactive Learning Prompting","Intersectional Prompting","Iterative Prompt Refinement","Iterative-of-Thought","Iterative Refinement Prompting","Iterative Critique Prompting (ICP)","Jailbreaking Prompting","Knowledge Injection","Least-to-Most Prompting","Length Control Prompting","Literature Comparison Prompting","Literature Review Prompting","Logical Thoughts (LoT)","Maieutic Prompting","MathPrompter","Memory-Augmented Prompting","Meta-Analytical Prompting","Meta-Prompting","Model Comparison Prompting","Multi-Agent Deliberation Prompting (MADP)","Multi-Agent Prompting","Multi-Example Prompting","Multimodal Prompting","One-Shot Prompting","Parameter Sensitivity Prompting","Paper-to-Prompt (P2P)","Peer Review Prompting","Peer Teaching Prompting","Persona Prompting","Plan-and-Solve (PS / PS⁺)","Planning Prompting","Problem-Inversion Prompting","Process-Based Prompting","Program-Aided Prompting","Program-of-Thoughts (PoT)","Progressive Elaboration","Prompt Bench / Prompt Eval Framework","Prompt Chaining","Prompt Compression / Distillation (PCD)","Prompt Decomposition Graph (PDG)","Prompt Explainability","Prompt Instrumentation","Prompt Patterns","Prompt Regularization (PR)","Prompt Scaffolding","Prompt Surgery","Prompt Stacking (PSK)","Prompt Tuning","Prompt Versioning (PV)","Prompt Visualization (PViz)","Prompt-based Learning","Prompt-to-Rubric (PtR)","Random CoT","Reason + Act (ReAct)","Recursive-of-Thought","Reference Prompting","Research Methods Prompting","Research-Hypothesis Prompting (RHP)","Responsible Prompting Framework (RPF)","Retrieval-Augmented Prompting","Reward-Augmented Prompting (RAP)","Rewriting Prompting","Reverse Prompting","Role Prompting","Role-Play Prompting","Rubric-Based Prompting","Rule-based Prompting","Sampling Control Prompting","Scenario Expansion","Schema-Guided Prompting","Scratchpad Prompting","Self-Ask","Self-Consistency","Self-Correct","Self-Evaluation Prompting","Self-Explanation Prompting (SEP)","Self-Generated CoT","Self-Reflection Prompting","Self-Reward Prompting (SRP)","Set Realistic Expectations Prompting","Simulation Prompting","Simulated Thought Chain (SimToM)","Skeleton-of-Thought (SoT)","Socratic Prompting","Source Evaluation Prompting","Statistical Reasoning Prompting","Step-back Prompting","Step-by-Step Reasoning","Structured Chain-of-Thought (SCoT)","Style Transfer Prompting","Summarization Prompting","Synthetic Prompting","System 2 Attention (S2A)","Take-a-step-back Prompting","Task Decomposition Prompting","Teach-Back Prompting","Technique-Based Prompting","Temporal Awareness Prompting","Thread-of-Thought (ThoT)","Toolformer-Style Prompting (TSP)","Tone Control Prompting","Transparency Prompting","Translation Prompting","Tree-of-Thought (ToT)","Trust-Aware Prompting","Uncertainty Estimation","Verify-and-Edit (VE)","What-if Prompting","What-if Scenario Prompting","Zero-Shot Prompting"
        };

        // Public exposure for admin defaults (seed/edit UI)
        public static IReadOnlyList<string> DefaultMasterTechniques => MasterTechniques;

        public PromptFilterAiService(ApplicationDbContext db, IConfiguration cfg, IHttpContextAccessor http, IAiProviderResolver resolver)
        {
            _db = db;
            _cfg = cfg;
            _http = http;
            _resolver = resolver;
            _modelId = cfg["Kisski:ModelId"]
                       ?? cfg["OpenAI:ModelId"]
                       ?? "openai-gpt-oss-120b";
        }

        public async Task<PromptFilterPayloadDto> GenerateAsync(
            PromptFormDto form,
            CancellationToken ct = default)
            => await GenerateCoreAsync(form, PromptType.Text, modelIdOverride: null, ct);

        public async Task<PromptFilterPayloadDto> GenerateAsync(
            PromptFormDto form,
            PromptType targetType,
            CancellationToken ct = default)
            => await GenerateCoreAsync(form, targetType, modelIdOverride: null, ct);

        public async Task<PromptFilterPayloadDto> GenerateAsync(
            PromptFormDto form,
            PromptType targetType,
            string? modelIdOverride,
            CancellationToken ct = default)
            => await GenerateCoreAsync(form, targetType, modelIdOverride, ct);

        private async Task<PromptFilterPayloadDto> GenerateCoreAsync(
            PromptFormDto form,
            PromptType targetType,
            string? modelIdOverride,
            CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            // === Token-sparende Top-K Shortlist bestimmen (ohne externe Abhängigkeiten) ===
            var shortlist = RankTechniquesByForm(form, topK: 8);               // 6–8 ist erfahrungsgemäß sweet spot
            var allowedTechCsv = string.Join(", ", shortlist);

            // 1) Messages
            var sys = await BuildSystemPreambleAsync(allowedTechCsv, allowSupplement: true, ct);
            var user = await BuildUserTaskAsync(form, ct);
            var typeGuidance = await BuildTypeGuidanceAsync(targetType, ct);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(sys),
                new SystemChatMessage(typeGuidance),
                new UserChatMessage(user)
            };

            // 2) Base options (schema will be set after we resolve actual endpoint)
            var baseOptions = new ChatCompletionOptions { Temperature = 0, TopP = 1 };
            ChatResponseFormat? strictFormat = null;


            // 3) Resolve provider (group override → global) via centralized resolver
            string? uid = null;
            try { uid = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier); } catch { }
            var chatRes = await _resolver.ResolveChatAsync(uid, modelIdOverride, ct);
            var baseUrl = chatRes.BaseUrl;
            var chatKey = chatRes.ApiKey;
            var modelResolved = chatRes.ModelId;
            if (string.IsNullOrWhiteSpace(chatKey)) throw new InvalidOperationException("API-ApiKey fehlt.");

            // Decide schema strictly based on actual endpoint
            // Strict JSON schema only for OpenAI Responses/Chats (api.openai.com). Kisski/Gemini use lenient parsing.
            bool useStrictSchema = baseUrl.Contains("api.openai.com", StringComparison.OrdinalIgnoreCase);
            if (useStrictSchema)
            {
                var schemaJson = GetOutputJsonSchema((int)targetType);
                strictFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: "prompt_filter",
                    jsonSchema: BinaryData.FromString(schemaJson),
                    jsonSchemaIsStrict: true);
                baseOptions.ResponseFormat = strictFormat;
            }

            // 4) Call (with timeout to avoid hanging requests) + model fallback on 400/404
            string raw = string.Empty;
            try
            {
                if (baseUrl.StartsWith("gemini://", StringComparison.OrdinalIgnoreCase))
                {
                    using var http = new HttpClient();
                    var model = string.IsNullOrWhiteSpace(modelResolved) ? "gemini-2.5-flash" : modelResolved;
                    if (string.Equals(model, "gemini-pro", StringComparison.OrdinalIgnoreCase) || model.Contains("pro", StringComparison.OrdinalIgnoreCase)) model = "gemini-2.5-pro";
                    else if (string.Equals(model, "gemini-flash", StringComparison.OrdinalIgnoreCase) || model.Contains("flash", StringComparison.OrdinalIgnoreCase)) model = "gemini-2.5-flash";
                    var url = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={Uri.EscapeDataString(chatKey)}";
                    var payload = new
                    {
                        contents = new[]
                        {
                            new { role = "user", parts = new[] { new { text = sys + "\n\n" + typeGuidance + "\n\n" + user } } }
                        },
                        generationConfig = new { temperature = 0, topP = 1, maxOutputTokens = 1024, response_mime_type = "application/json" }
                    };
                    var json = JsonSerializer.Serialize(payload);
                    using var content = new StringContent(json, Encoding.UTF8, "application/json");
                    using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    timeoutCts.CancelAfter(TimeSpan.FromSeconds(75));
                    var resp = await http.PostAsync(url, content, timeoutCts.Token);
                    var respText = await resp.Content.ReadAsStringAsync(timeoutCts.Token);
                    if (!resp.IsSuccessStatusCode)
                        throw new InvalidOperationException($"Gemini API Fehler: {(int)resp.StatusCode} {resp.ReasonPhrase} -> {respText}");
                    try
                    {
                        using var doc = JsonDocument.Parse(respText);
                        string outText = string.Empty;
                        if (doc.RootElement.ValueKind == JsonValueKind.Object && doc.RootElement.TryGetProperty("candidates", out var cands) && cands.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var cand in cands.EnumerateArray())
                            {
                                if (cand.ValueKind != JsonValueKind.Object) continue;
                                if (!cand.TryGetProperty("content", out var contentEl)) continue;
                                // content may be object with parts or array of such objects
                                var partsList = new List<JsonElement>();
                                if (contentEl.ValueKind == JsonValueKind.Object && contentEl.TryGetProperty("parts", out var parts) && parts.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var p in parts.EnumerateArray()) partsList.Add(p);
                                }
                                else if (contentEl.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var ce2 in contentEl.EnumerateArray())
                                    {
                                        if (ce2.ValueKind == JsonValueKind.Object && ce2.TryGetProperty("parts", out var p2) && p2.ValueKind == JsonValueKind.Array)
                                        {
                                            foreach (var p in p2.EnumerateArray()) partsList.Add(p);
                                        }
                                        else
                                        {
                                            partsList.Add(ce2);
                                        }
                                    }
                                }

                                foreach (var p in partsList)
                                {
                                    if (p.ValueKind != JsonValueKind.Object) continue;
                                    if (p.TryGetProperty("text", out var t) && t.ValueKind == JsonValueKind.String)
                                    {
                                        outText += t.GetString();
                                        continue;
                                    }
                                    // functionCall with args (object or string)
                                    if (p.TryGetProperty("functionCall", out var fc) && fc.ValueKind == JsonValueKind.Object)
                                    {
                                        if (fc.TryGetProperty("args", out var argsEl))
                                        {
                                            if (argsEl.ValueKind == JsonValueKind.Object || argsEl.ValueKind == JsonValueKind.Array)
                                                outText += argsEl.GetRawText();
                                            else if (argsEl.ValueKind == JsonValueKind.String)
                                                outText += argsEl.GetString();
                                            continue;
                                        }
                                    }
                                    // generic json field
                                    if (p.TryGetProperty("json", out var jsonEl) && (jsonEl.ValueKind == JsonValueKind.Object || jsonEl.ValueKind == JsonValueKind.Array))
                                    {
                                        outText += jsonEl.GetRawText();
                                        continue;
                                    }
                                }
                            }
                        }
                        raw = outText ?? string.Empty;
                    }
                    catch
                    {
                        raw = string.Empty;
                    }
                    if (string.IsNullOrWhiteSpace(raw))
                    {
                        // Keep the full response for downstream relaxed JSON / plain-text fallbacks
                        raw = respText ?? string.Empty;
                    }
                }
                else
                {
                    var openai = new OpenAIClient(new ApiKeyCredential(chatKey), new OpenAIClientOptions { Endpoint = new Uri(baseUrl) });

                    static string NormalizeAlias(string m)
                    {
                        if (string.IsNullOrWhiteSpace(m)) return m;
                        if (string.Equals(m, "gpt-5-instant", StringComparison.OrdinalIgnoreCase)) return "gpt-5";
                        if (string.Equals(m, "4o-mini", StringComparison.OrdinalIgnoreCase)) return "gpt-4o-mini";
                        if (string.Equals(m, "4o", StringComparison.OrdinalIgnoreCase)) return "gpt-4o";
                        return m;
                    }

                    var resolved = string.IsNullOrWhiteSpace(modelResolved) ? _modelId : modelResolved;
                    resolved = NormalizeAlias(resolved);

                    List<string> attempts;
                    if (baseUrl.Contains("api.openai.com", StringComparison.OrdinalIgnoreCase))
                    {
                        if (string.IsNullOrWhiteSpace(resolved))
                        {
                            resolved = NormalizeAlias(_cfg["OpenAI:ModelId"] ?? "gpt-4o-mini");
                        }
                        attempts = new List<string>
                    {
                        NormalizeAlias(resolved),
                        NormalizeAlias(_cfg["OpenAI:ModelId"] ?? string.Empty),
                        "gpt-4o-mini"
                    };
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(resolved))
                        {
                            resolved = _cfg["Kisski:ModelId"] ?? "meta-llama-3.1-8b-instruct";
                        }
                        attempts = new List<string>
                    {
                        resolved,
                        _cfg["Kisski:ModelId"] ?? string.Empty,
                        "meta-llama-3.1-8b-instruct",
                        "medgemma-27b",
                        "gemma-3-27b-it",
                        "openai-gpt-oss-120b"
                    };
                    }
                    attempts = attempts.Where(s => !string.IsNullOrWhiteSpace(s)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                    var attemptedSummary = string.Join(", ", attempts);
                    var primaryModel = attempts.FirstOrDefault() ?? string.Empty;
                    var isReasoningModel = primaryModel.StartsWith("gpt-5", StringComparison.OrdinalIgnoreCase);
                    var timeoutSeconds = isReasoningModel ? 120 : 45;
                    using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    timeoutCts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

                    ClientResultException? lastCre = null;
                    Exception? lastEx = null;
                    foreach (var model in attempts)
                    {
                        try
                        {
                            if (baseUrl.Contains("api.openai.com", StringComparison.OrdinalIgnoreCase) && model.StartsWith("gpt-5", StringComparison.OrdinalIgnoreCase))
                            {
                                var url = baseUrl.TrimEnd('/') + "/responses";
                                using var http = new HttpClient();
                                http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", chatKey);
                                var inputText = sys + "\n\n" + typeGuidance + "\n\n" + user;
                                object? responseFormatObj = null;
                                if (useStrictSchema)
                                {
                                    var schemaJson2 = GetOutputJsonSchema((int)targetType);
                                    using var schemaDoc = JsonDocument.Parse(schemaJson2);
                                    var schemaEl = schemaDoc.RootElement.Clone();
                                    responseFormatObj = new { type = "json_schema", json_schema = new { name = "prompt_filter", schema = schemaEl, strict = true } };
                                }
                                var payload = new
                                {
                                    model = model,
                                    input = inputText,
                                    reasoning = new { effort = "minimal" },
                                    response_format = responseFormatObj,
                                    text = new { verbosity = "low" }
                                };
                                var json = JsonSerializer.Serialize(payload);
                                using var content = new StringContent(json, Encoding.UTF8, "application/json");
                                var resp = await http.PostAsync(url, content, timeoutCts.Token);
                                var respText = await resp.Content.ReadAsStringAsync(timeoutCts.Token);
                                if (!resp.IsSuccessStatusCode)
                                {
                                    lastEx = new InvalidOperationException($"OpenAI Responses Fehler: {(int)resp.StatusCode} {resp.ReasonPhrase} -> {respText}");
                                    continue;
                                }
                                try
                                {
                                    using var docR = JsonDocument.Parse(respText);
                                    if (docR.RootElement.TryGetProperty("output_text", out var ot))
                                        raw = ot.GetString() ?? string.Empty;
                                    else if (docR.RootElement.TryGetProperty("output", out var outputArr) && outputArr.ValueKind == JsonValueKind.Array)
                                    {
                                        var first = outputArr.EnumerateArray().FirstOrDefault();
                                        if (first.ValueKind != JsonValueKind.Undefined && first.TryGetProperty("content", out var cont) && cont.ValueKind == JsonValueKind.Array)
                                        {
                                            var c0 = cont.EnumerateArray().FirstOrDefault();
                                            if (c0.ValueKind != JsonValueKind.Undefined && c0.TryGetProperty("text", out var t)) raw = t.GetString() ?? string.Empty;
                                        }
                                    }
                                }
                                catch { raw = string.Empty; }
                            }
                            else
                            {
                                var chatClient = openai.GetChatClient(model);
                                ChatCompletionOptions loopOpts = model.StartsWith("gpt-5", StringComparison.OrdinalIgnoreCase)
                                    ? new ChatCompletionOptions()
                                    : new ChatCompletionOptions { Temperature = 0, TopP = 1 };
                                if (strictFormat != null) loopOpts.ResponseFormat = strictFormat;
                                var result = await chatClient.CompleteChatAsync(messages, loopOpts, cancellationToken: timeoutCts.Token);
                                raw = result.Value?.Content?.FirstOrDefault()?.Text ?? string.Empty;
                            }
                            if (string.IsNullOrWhiteSpace(raw))
                            {
                                lastEx = new InvalidOperationException("Leere Antwort vom Modell");
                                continue;
                            }
                            lastCre = null; lastEx = null;
                            break;
                        }
                        catch (ClientResultException cre) when ((cre.Status == 404 || cre.Status == 401) && baseUrl.Contains("chat-ai.academiccloud.de", StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine($"Chat 404 for model '{model}' on Kisski, retrying with toggled '/v1': {cre.Message}");
                            try
                            {
                                var altBase = baseUrl.EndsWith("/v1", StringComparison.OrdinalIgnoreCase)
                                    ? baseUrl.Substring(0, baseUrl.Length - 3)
                                    : (baseUrl.TrimEnd('/') + "/v1");
                                var altOpenai = new OpenAIClient(new ApiKeyCredential(chatKey), new OpenAIClientOptions { Endpoint = new Uri(altBase) });
                                var altClient = altOpenai.GetChatClient(model);
                                ChatCompletionOptions loopOpts = model.StartsWith("gpt-5", StringComparison.OrdinalIgnoreCase)
                                    ? new ChatCompletionOptions()
                                    : new ChatCompletionOptions { Temperature = 0, TopP = 1 };
                                if (strictFormat != null) loopOpts.ResponseFormat = strictFormat;
                                var result = await altClient.CompleteChatAsync(messages, loopOpts, cancellationToken: timeoutCts.Token);
                                raw = result.Value?.Content?.FirstOrDefault()?.Text ?? string.Empty;
                                if (!string.IsNullOrWhiteSpace(raw))
                                {
                                    lastCre = null; lastEx = null;
                                    break;
                                }
                            }
                            catch (ClientResultException altCre)
                            {
                                Console.WriteLine($"Alt endpoint also failed for model '{model}': {altCre.Message}");
                                lastCre = altCre; lastEx = null;
                            }
                            catch (Exception altEx)
                            {
                                lastEx = altEx;
                            }
                            // HTTP fallback to OpenAI-compatible /chat/completions variants
                            if (string.IsNullOrWhiteSpace(raw))
                            {
                                try
                                {
                                    using var http = new HttpClient();
                                    http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", chatKey);
                                    try { http.DefaultRequestHeaders.Add("X-API-KEY", chatKey); } catch { }
                                    try { http.DefaultRequestHeaders.Add("x-api-key", chatKey); } catch { }
                                    var altBase2 = baseUrl.EndsWith("/v1", StringComparison.OrdinalIgnoreCase)
                                        ? baseUrl.Substring(0, baseUrl.Length - 3)
                                        : (baseUrl.TrimEnd('/') + "/v1");
                                    var bases = new[]
                                    {
                                    baseUrl.TrimEnd('/'),
                                    altBase2.TrimEnd('/'),
                                    baseUrl.TrimEnd('/') + "/openai/v1",
                                    altBase2.TrimEnd('/') + "/openai/v1"
                                };
                                    foreach (var b in bases.Distinct(StringComparer.OrdinalIgnoreCase))
                                    {
                                        var url = b + "/chat/completions";
                                        var payload = new
                                        {
                                            model = model,
                                            temperature = 0,
                                            top_p = 1,
                                            messages = new object[]
                                            {
                                            new { role = "system", content = sys + "\n\n" + typeGuidance },
                                            new { role = "user", content = user }
                                            }
                                        };
                                        var json = JsonSerializer.Serialize(payload);
                                        using var content = new StringContent(json, Encoding.UTF8, "application/json");
                                        var resp = await http.PostAsync(url, content, timeoutCts.Token);
                                        var respText = await resp.Content.ReadAsStringAsync(timeoutCts.Token);
                                        if (!resp.IsSuccessStatusCode) continue;
                                        try
                                        {
                                            using var doc = JsonDocument.Parse(respText);
                                            var ch0 = doc.RootElement.GetProperty("choices")[0];
                                            var msg = ch0.GetProperty("message");
                                            if (msg.TryGetProperty("content", out var cont) && cont.ValueKind == JsonValueKind.String)
                                            {
                                                raw = cont.GetString() ?? string.Empty;
                                                if (!string.IsNullOrWhiteSpace(raw))
                                                {
                                                    lastCre = null; lastEx = null;
                                                    break;
                                                }
                                            }
                                        }
                                        catch { /* try next base */ }
                                    }
                                    if (!string.IsNullOrWhiteSpace(raw)) break;
                                }
                                catch { /* swallow and continue outer loop */ }
                                // if still empty, continue to next model attempt
                                continue;
                            }
                        }
                        catch (ClientResultException cre) when (cre.Status == 400)
                        {
                            Console.WriteLine($"Chat 400 for model '{model}': {cre}");
                            lastCre = cre; lastEx = null; continue;
                        }
                    }
                    if (string.IsNullOrWhiteSpace(raw))
                    {
                        if (lastCre != null) throw new InvalidOperationException($"{lastCre.Message} (attempted models: {attemptedSummary}; endpoint: {baseUrl})");
                        throw lastEx ?? new InvalidOperationException($"Unbekannter Fehler beim Abruf der Filter‑Antwort (attempted models: {attemptedSummary}; endpoint: {baseUrl}).");
                    }
                }
            }
            catch (OperationCanceledException oce) when (!ct.IsCancellationRequested)
            {
                Console.WriteLine("‼️  Chat-Request Timeout: " + oce.Message);
                throw new InvalidOperationException("Die Filter-Generierung hat zu lange gedauert. Bitte versuche es erneut oder wähle ein anderes Modell.");
            }
            catch (ClientResultException cre)
            {
                var status = cre.Status;
                var hint = status switch
                {
                    400 => "Die Anfrage wurde vom Modell abgewiesen (400). Das gewählte Modell ist ggf. nicht verfügbar oder unterstützt diesen Aufruf nicht.",
                    401 => "Ungültiger oder fehlender API‑Key (401).",
                    403 => "Zugriff verweigert (403).",
                    404 => "Modell oder Endpoint nicht gefunden (404).",
                    _ => $"Dienstfehler ({status})."
                };
                throw new InvalidOperationException($"{hint} Bitte versuche es mit einem anderen Text‑Modell (z. B. 'openai-gpt-oss-120b' oder 'meta-llama-3.1-8b-instruct'). Details: {cre.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("‼️  Chat-Request fehlgeschlagen: " + ex);
                throw;
            }

            // 5) Raw JSON (strict)
            raw = raw?.Trim() ?? string.Empty;
            if (raw.StartsWith("```", StringComparison.Ordinal))
            {
                raw = Regex.Replace(raw, "^```[a-zA-Z]*\\s*", string.Empty, RegexOptions.Multiline);
                raw = Regex.Replace(raw, "\\s*```\\s*$", string.Empty, RegexOptions.Multiline);
                raw = raw.Trim();
            }
            Console.WriteLine("\n===== ROH-ANTWORT (gekürzt) =====\n" + Truncate(raw, 4000) + "\n");
            if (string.IsNullOrWhiteSpace(raw))
                throw new InvalidOperationException("Leere Antwort vom Modell (prüfe API-Key, ModelId, oder JSON-Schema).");

            // 6) Deserialize
            PromptFilterPayloadDto? parsed = null;
            try
            {
                parsed = JsonSerializer.Deserialize<PromptFilterPayloadDto>(
                    raw,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException jx)
            {
                Console.WriteLine("‼️  JSON-Parse-Fehler: " + jx);
            }

            parsed ??= new PromptFilterPayloadDto
            {
                Data = new List<PromptFilterCategoryDto>(),
                Hash = string.Empty
            };

            // 6b) Fallback: try to extract JSON object if schema not enforced
            if ((parsed.Data == null || parsed.Data.Count == 0) && !useStrictSchema && !string.IsNullOrWhiteSpace(raw))
            {
                try
                {
                    var start = raw.IndexOf('{');
                    var end = raw.LastIndexOf('}');
                    if (start >= 0 && end > start)
                    {
                        var json = raw.Substring(start, end - start + 1);
                        var tmp = JsonSerializer.Deserialize<PromptFilterPayloadDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (tmp != null) parsed = tmp;
                    }
                }
                catch { /* ignore */ }
            }

            // 6c) Alt-shape mapper: support { category: {name,type,displayOrder}, items: [ ... ] }
            if ((parsed.Data == null || parsed.Data.Count == 0) && !string.IsNullOrWhiteSpace(raw))
            {
                try
                {
                    using var doc = JsonDocument.Parse(raw);
                    var root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Object)
                    {
                        string catName = "";
                        int disp = 0;
                        if (root.TryGetProperty("category", out var catEl) && catEl.ValueKind == JsonValueKind.Object)
                        {
                            if (catEl.TryGetProperty("name", out var nameEl) && nameEl.ValueKind == JsonValueKind.String)
                                catName = nameEl.GetString() ?? string.Empty;
                            if (catEl.TryGetProperty("displayOrder", out var dEl) && dEl.ValueKind == JsonValueKind.Number)
                                disp = dEl.GetInt32();
                        }

                        var items = new List<PromptFilterItemDto>();
                        if (root.TryGetProperty("items", out var itemsEl) && itemsEl.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var it in itemsEl.EnumerateArray())
                            {
                                try
                                {
                                    string title = it.TryGetProperty("title", out var tEl) && tEl.ValueKind == JsonValueKind.String ? (tEl.GetString() ?? string.Empty) : string.Empty;
                                    string info = it.TryGetProperty("info", out var iEl) && iEl.ValueKind == JsonValueKind.String ? (iEl.GetString() ?? string.Empty) : string.Empty;
                                    string instr = it.TryGetProperty("instruction", out var insEl) && insEl.ValueKind == JsonValueKind.String ? (insEl.GetString() ?? string.Empty) : string.Empty;
                                    int sort = it.TryGetProperty("sortOrder", out var sEl) && sEl.ValueKind == JsonValueKind.Number ? sEl.GetInt32() : items.Count;
                                    items.Add(new PromptFilterItemDto { Id = 0, Title = title, Info = info, Instruction = instr, SortOrder = sort });
                                }
                                catch { /* skip bad item */ }
                            }
                        }

                        if (items.Count > 0)
                        {
                            var fallbackName = DeriveFallbackCategoryName(form, items, targetType);
                            parsed = new PromptFilterPayloadDto
                            {
                                Data = new List<PromptFilterCategoryDto>
                                {
                                    new PromptFilterCategoryDto
                                    {
                                        Id = 0,
                                        Name = string.IsNullOrWhiteSpace(catName) ? fallbackName : catName,
                                        Type = (int)targetType,
                                        DisplayOrder = disp,
                                        Items = items.OrderBy(x => x.SortOrder).ToList()
                                    }
                                },
                                Hash = string.Empty
                            };
                        }
                    }
                }
                catch { /* ignore */ }
            }

            // 6d) Array-root fallback: [ {title, info, instruction, sortOrder?}, ... ]
            if ((parsed.Data == null || parsed.Data.Count == 0) && !string.IsNullOrWhiteSpace(raw))
            {
                try
                {
                    using var doc = JsonDocument.Parse(raw);
                    var root = doc.RootElement;
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        var items = new List<PromptFilterItemDto>();
                        int idx = 0;
                        foreach (var it in root.EnumerateArray())
                        {
                            try
                            {
                                string title = string.Empty, info = string.Empty, instr = string.Empty;
                                int sort = idx;
                                if (it.ValueKind == JsonValueKind.Object)
                                {
                                    if (it.TryGetProperty("title", out var tEl) && tEl.ValueKind == JsonValueKind.String) title = tEl.GetString() ?? string.Empty;
                                    if (it.TryGetProperty("info", out var iEl) && iEl.ValueKind == JsonValueKind.String) info = iEl.GetString() ?? string.Empty;
                                    if (it.TryGetProperty("instruction", out var insEl) && insEl.ValueKind == JsonValueKind.String) instr = insEl.GetString() ?? string.Empty;
                                    if (it.TryGetProperty("sortOrder", out var sEl) && sEl.ValueKind == JsonValueKind.Number) sort = sEl.GetInt32();
                                }
                                else if (it.ValueKind == JsonValueKind.String)
                                {
                                    title = it.GetString() ?? string.Empty;
                                }
                                items.Add(new PromptFilterItemDto { Id = 0, Title = title, Info = info, Instruction = instr, SortOrder = sort });
                                idx++;
                            }
                            catch { /* skip bad element */ }
                        }
                        if (items.Count > 0)
                        {
                            var fallbackName = DeriveFallbackCategoryName(form, items, targetType);
                            parsed = new PromptFilterPayloadDto
                            {
                                Data = new List<PromptFilterCategoryDto>
                                {
                                    new PromptFilterCategoryDto
                                    {
                                        Id = 0,
                                        Name = fallbackName,
                                        Type = (int)targetType,
                                        DisplayOrder = 0,
                                        Items = items.OrderBy(x => x.SortOrder).ToList()
                                    }
                                },
                                Hash = string.Empty
                            };
                        }
                    }
                }
                catch { /* ignore */ }
            }

            // 6e) Plain-text list fallback: derive items from bullet/numbered lists
            if ((parsed.Data == null || parsed.Data.Count == 0) && !string.IsNullOrWhiteSpace(raw))
            {
                try
                {
                    var lines = raw.Replace("\r", "").Split('\n');
                    var items = new List<PromptFilterItemDto>();
                    var rx = new Regex("^\\s*(?:[-*•·–—]|\\d{1,2}[\\.)])\\s+(?<t>.+)$", RegexOptions.Compiled);
                    int idx = 0;
                    foreach (var ln in lines)
                    {
                        var m = rx.Match(ln);
                        if (!m.Success) continue;
                        var text = m.Groups["t"].Value.Trim();
                        if (string.IsNullOrWhiteSpace(text)) continue;
                        if (text.Length > 300) text = text.Substring(0, 300);
                        items.Add(new PromptFilterItemDto
                        {
                            Id = 0,
                            Title = text,
                            Info = string.Empty,
                            Instruction = string.Empty,
                            SortOrder = idx++
                        });
                        if (items.Count >= 8) break;
                    }
                    items = items.Where(i => !string.IsNullOrWhiteSpace(i.Title)).Take(8).ToList();
                    if (items.Count > 0)
                    {
                        var fallbackName = DeriveFallbackCategoryName(form, items, targetType);
                        parsed = new PromptFilterPayloadDto
                        {
                            Data = new List<PromptFilterCategoryDto>
                            {
                                new PromptFilterCategoryDto
                                {
                                    Id = 0,
                                    Name = fallbackName,
                                    Type = (int)targetType,
                                    DisplayOrder = 0,
                                    Items = items
                                }
                            },
                            Hash = string.Empty
                        };
                    }
                }
                catch { /* ignore */ }
            }

            // 7) Normalize & repair into NEW instances
            if ((parsed.Data == null || parsed.Data.Count == 0))
            {
                var picked = shortlist.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                if (picked.Count < 3)
                {
                    // pad from MasterTechniques deterministically
                    picked.AddRange(DefaultMasterTechniques.Take(8).Where(t => !picked.Contains(t, StringComparer.OrdinalIgnoreCase)));
                }
                var items = picked
                    .Take(8)
                    .Select((t, i) => new PromptFilterItemDto { Id = 0, Title = t, Info = string.Empty, Instruction = string.Empty, SortOrder = i })
                    .ToList();
                if (items.Count > 0)
                {
                    var fallbackName = DeriveFallbackCategoryName(form, items, targetType);
                    parsed = new PromptFilterPayloadDto
                    {
                        Data = new List<PromptFilterCategoryDto>
                        {
                            new PromptFilterCategoryDto
                            {
                                Id = 0,
                                Name = fallbackName,
                                Type = (int)targetType,
                                DisplayOrder = 0,
                                Items = items
                            }
                        },
                        Hash = string.Empty
                    };
                }
            }
            var repaired = RepairIntoNewPayload(form, parsed, targetType);

            // 8) Server-side hash over MINIFIED JSON
            var minified = JsonSerializer.Serialize(repaired, HashOptions);
            var hash = ComputeSha256(minified);

            var finalPayload = new PromptFilterPayloadDto
            {
                Data = repaired.Data,
                Hash = hash
            };

            // Final safety: guarantee at least one category with >= 3 items
            if (finalPayload.Data == null || finalPayload.Data.Count == 0)
            {
                var picked = shortlist.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                if (picked.Count < 3)
                {
                    picked.AddRange(DefaultMasterTechniques.Take(8).Where(t => !picked.Contains(t, StringComparer.OrdinalIgnoreCase)));
                }
                var items = picked
                    .Take(8)
                    .Select((t, i) => new PromptFilterItemDto { Id = 0, Title = t, Info = string.Empty, Instruction = string.Empty, SortOrder = i })
                    .ToList();
                if (items.Count >= 3)
                {
                    var fallbackName = DeriveFallbackCategoryName(form, items, targetType);
                    finalPayload = new PromptFilterPayloadDto
                    {
                        Data = new List<PromptFilterCategoryDto>
                        {
                            new PromptFilterCategoryDto
                            {
                                Id = 0,
                                Name = fallbackName,
                                Type = (int)targetType,
                                DisplayOrder = 0,
                                Items = items
                            }
                        },
                        Hash = hash
                    };
                }
            }

            Console.WriteLine("===== DESERIALISIERT =====\n" +
                JsonSerializer.Serialize(finalPayload, new JsonSerializerOptions { WriteIndented = true }) + "\n");

            return finalPayload;
        }

        private async Task<(string baseUrl, string apiKey, string model)> ResolveChatProviderAsync(string? modelIdOverride, CancellationToken ct)
        {
            string? uid = null;
            try { uid = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier); } catch { }
            // Load global persisted provider (OpenAI/Kisski). Gemini/Claude not supported here.
            ApiKeySetting? globalRec = null;
            try { globalRec = await _db.ApiKeySettings.OrderByDescending(x => x.UpdatedAt).FirstOrDefaultAsync(ct); } catch { globalRec = null; }

            (string baseUrl, string apiKey, string model) FromGlobal()
            {
                var p = (globalRec?.ActiveProvider ?? string.Empty).Trim().ToLowerInvariant();
                if (p == "openai")
                {
                    var baseU = "https://api.openai.com/v1";
                    var key = (globalRec?.OpenAIKey ?? _cfg["OpenAI:ApiKey"]) ?? string.Empty;
                    var m = string.IsNullOrWhiteSpace(modelIdOverride) ? (globalRec?.OpenAIModel ?? _cfg["OpenAI:ModelId"] ?? _modelId) : modelIdOverride!;
                    if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException("API-ApiKey fehlt.");
                    return (baseU, key!, m);
                }
                if (p == "kisski")
                {
                    var baseU = "https://chat-ai.academiccloud.de/v1";
                    var key = (globalRec?.KisskiApiKey ?? _cfg["Kisski:ApiKey"]) ?? string.Empty;
                    var m = string.IsNullOrWhiteSpace(modelIdOverride) ? (globalRec?.KisskiModel ?? _cfg["Kisski:ModelId"] ?? _modelId) : modelIdOverride!;
                    if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException("API-ApiKey fehlt.");
                    return (baseU, key!, m);
                }
                // default preference: OpenAI, then Kisski
                if (!string.IsNullOrWhiteSpace(globalRec?.OpenAIKey ?? _cfg["OpenAI:ApiKey"]))
                {
                    var baseU = "https://api.openai.com/v1";
                    var key = (globalRec?.OpenAIKey ?? _cfg["OpenAI:ApiKey"]) ?? string.Empty;
                    var m = string.IsNullOrWhiteSpace(modelIdOverride) ? (globalRec?.OpenAIModel ?? _cfg["OpenAI:ModelId"] ?? _modelId) : modelIdOverride!;
                    return (baseU, key!, m);
                }
                var baseUF = "https://chat-ai.academiccloud.de/v1";
                var keyF = (globalRec?.KisskiApiKey ?? _cfg["Kisski:ApiKey"]) ?? string.Empty;
                var mF = string.IsNullOrWhiteSpace(modelIdOverride) ? (globalRec?.KisskiModel ?? _cfg["Kisski:ModelId"] ?? _modelId) : modelIdOverride!;
                return (baseUF, keyF!, mF);
            }
            if (!string.IsNullOrWhiteSpace(uid))
            {
                try
                {
                    var grp = await _db.UserGroupMemberships
                                       .Where(m => m.UserId == uid)
                                       .OrderByDescending(m => m.CreatedAt)
                                       .Select(m => m.Group)
                                       .FirstOrDefaultAsync(ct);
                    var groupName = string.IsNullOrWhiteSpace(grp) ? "Ohne Gruppe" : grp!.Trim();
                    var hasOwner = await _db.DozentGroupOwnerships.AnyAsync(o => o.Group == groupName, ct);
                    if (hasOwner)
                    {
                        var rec = await _db.Set<GroupApiKeySetting>()
                                           .Where(g => g.Group == groupName)
                                           .OrderByDescending(g => g.UpdatedAt)
                                           .FirstOrDefaultAsync(ct);
                        if (rec != null && !string.IsNullOrWhiteSpace(rec.ActiveProvider))
                        {
                            var prov = (rec.ActiveProvider == null ? string.Empty : rec.ActiveProvider.ToString()!.Trim().ToLowerInvariant());
                            if (prov == "openai")
                            {
                                var baseUrl = "https://api.openai.com/v1";
                                var key = rec.OpenAIKey ?? _cfg["OpenAI:ApiKey"];
                                var model = string.IsNullOrWhiteSpace(modelIdOverride) ? (_cfg["OpenAI:ModelId"] ?? _modelId) : modelIdOverride!;
                                if (!string.IsNullOrWhiteSpace(key)) return (baseUrl, key!, model);
                            }
                            else if (prov == "kisski")
                            {
                                var baseUrl = "https://chat-ai.academiccloud.de/v1";
                                var key = rec.KisskiApiKey ?? _cfg["Kisski:ApiKey"];
                                var model = string.IsNullOrWhiteSpace(modelIdOverride) ? (_cfg["Kisski:ModelId"] ?? _modelId) : modelIdOverride!;
                                if (!string.IsNullOrWhiteSpace(key)) return (baseUrl, key!, model);
                            }
                            else if (prov == "gemini" || prov == "claude")
                            {
                                // Not supported directly via OpenAI SDK; fall through
                            }
                        }
                    }
                }
                catch { }
            }
            // Global fallback
            return FromGlobal();
        }

        /* ---------------------------- Prompt Pieces ---------------------------- */

        // Kompakter System-Prompt mit Shortlist; dynamischer Kopf aus DB
        private async Task<string> BuildSystemPreambleAsync(string allowedTechCsv, bool allowSupplement, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            string? groupOverride = null;
            try
            {
                string? overrideName = null;
                try { overrideName = _http.HttpContext?.Items["PromptAiGroupOverride"] as string; } catch { }

                string? groupName = null;
                if (!string.IsNullOrWhiteSpace(overrideName))
                {
                    groupName = overrideName.Trim();
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

                // "Ohne Gruppe" is a pseudo-group used as global-selection sentinel;
                // do not apply any group-level override for it.
                bool isNoGroup = string.Equals(groupName, "Ohne Gruppe", StringComparison.OrdinalIgnoreCase);

                if (!string.IsNullOrWhiteSpace(groupName) && !isNoGroup)
                {
                    var gp = await _db.GroupPromptAiSettings
                                      .Where(g => g.Group == groupName)
                                      .OrderByDescending(g => g.UpdatedAt)
                                      .FirstOrDefaultAsync(ct);
                    var useGlobal = gp?.UseGlobal ?? true;
                    if (!useGlobal && gp != null && !string.IsNullOrWhiteSpace(gp.FilterSystemPreamble))
                        groupOverride = gp.FilterSystemPreamble;
                }
            }
            catch { groupOverride = null; }
            string? preamble = null;
            try
            {
                preamble = await Task.Run(() =>
                    _db.PromptAiSettings
                       .OrderByDescending(x => x.UpdatedAt)
                       .Select(x => new { x.FilterSystemPreamble, x.SystemPreamble })
                       .FirstOrDefault(), ct) switch
                {
                    { FilterSystemPreamble: var f } when !string.IsNullOrWhiteSpace(f) => f,
                    { SystemPreamble: var s } when !string.IsNullOrWhiteSpace(s) => s,
                    _ => string.Empty
                };
            }
            catch { preamble = string.Empty; }

            if (!string.IsNullOrWhiteSpace(groupOverride))
            {
                preamble = groupOverride;
            }
            else if (string.IsNullOrWhiteSpace(preamble))
            {
                preamble = "Du bist eine expert*innengeführte Assistenz für Instructional Design, Hochschuldidaktik und wissenschaftliches Arbeiten. Schreibe in präzisem, formalem Deutsch für Forschung & Lehre. Nutze wissenschaftliche Terminologie.";
            }

            var supplementLine = allowSupplement
                ? "Wenn Titel/Thema/Ziele/Beschreibung klar auf zusätzliche Prompt Techniken hindeuten, darfst du bis zu 2 weitere, fachlich passende, wissenschaftlich belegte Prompt Techniken ergänzen (ergänze fehlende bei Bedarf durch Recherche) und im Info-Text begründen."
                : "Nutze ausschließlich die in ALLOWED_TECH genannten Techniken.";

            var sb = new StringBuilder();
            sb.AppendLine(preamble.Trim());
            sb.AppendLine();
            sb.AppendLine("Prompt Technik auswahl:");
            sb.AppendLine("- Die Prompt-Techniken sollen in ihrem offiziellen englischen Fachbegriff stehen, während die zugehörige Beschreibung auf Deutsch formuliert wird.");
            sb.AppendLine("- Verwende primär diese Shortlist (kanonische Namen). " + supplementLine);
            sb.AppendLine();
            sb.AppendLine("Qualitätscheck (intern, nicht ausgeben): Korrektheit, Neutralität, fachübergreifende Anwendbarkeit, Wissenschaftsbezug.");
            return sb.ToString();
        }

        private async Task<string> BuildUserTaskAsync(PromptFormDto form, CancellationToken ct)
        {
            var keys = form.Schlüsselbegriffe ?? Array.Empty<string>();
            var keysJoined = string.Join(",", keys.Select(x => $"\"{x}\""));

            string firstLine = string.Empty;
            try
            {
                firstLine = await Task.Run(() =>
                    _db.PromptTypeGuidances
                       .Where(x => x.Type == PromptType.Eigenfilter)
                       .OrderByDescending(x => x.UpdatedAt)
                       .Select(x => x.GuidanceText)
                       .FirstOrDefault() ?? string.Empty, ct);
            }
            catch { firstLine = string.Empty; }

            try
            {
                string? overrideName = null;
                try { overrideName = _http.HttpContext?.Items["PromptAiGroupOverride"] as string; } catch { }

                string? groupName = null;
                if (!string.IsNullOrWhiteSpace(overrideName))
                {
                    groupName = overrideName.Trim();
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
                    if (!useGlobal && gp != null && !string.IsNullOrWhiteSpace(gp.FilterFirstLine))
                        firstLine = gp.FilterFirstLine.Trim();
                }
            }
            catch { }

            if (string.IsNullOrWhiteSpace(firstLine))
            {
                firstLine = "Wandle die folgende Eingabe in eine strukturierte und wissenschaftlicher Prompt-Techniken um, basierend auf akademischen Frameworks des Prompt Engineerings, drop-in ready für LLMs und liefere GENAU EINE Kategorie mit 1–30 Filter-Items als JSON (keine Kommentare/Markdown)";
            }

            return $$"""
{{firstLine}}:

INPUT
• titel: "{{form.Titel}}"
• thema: "{{form.Thema}}"
• ziele: "{{form.Ziele}}"
• beschreibung: "{{form.Beschreibung}}"
• schluesselbegriffe: [{{keysJoined}}]

KATEGORIE
- name: 1–4 Wörter, treffende Zusammenfassung der Items basierend auf INPUT (z. B. Forschung, Planung, Bewertung, Kontext, Datenanalyse, Schreibstil).
- type: int gemäß Ziel-Modus; displayOrder: 0.

ITEMS – STIL (Masterliste, Shortlist bevorzugen; Kombinationen explizit)
- title: "<p>{Technik} anwenden</p>" (bei Techniken) bzw. kurzes imperatives Label, immer genau ein <p>.
- info: 1–2 Sätze (Nutzen/Zweck/Wirkung, didaktischer Kontext). Danach optional **Beispiele**:
  Erlaube <br>, zusätzliches <p>, <strong>…</strong>, und <ul><li>…</li></ul>.
  Nutze kanonische Techniknamen (z. B. "Chain-of-Thought (CoT)").
- instruction: genau ein <p>, imperative, mit {Platzhaltern}.

PLATZHALTER (sinnvoll einsetzen): {Thema}, {Ziel}, {Forschungsfrage}, {Datensatz}, {Stichprobe}, {Methode}, {Auswertungsverfahren}, {Hypothese}, {Konfidenzniveau}, {Zielgruppe}, {Niveau}, {Zeitbudget}, {Bewertungskriterien}.

WEITERE REGELN
- Begriffe aus {Thema}/{Schlüsselbegriffe} in title und instruction verwenden; Bezug auf titel/thema/ziele herstellen.
- HTML erlaubt: <p>, <br>, <strong>, <em>, <ul>, <li>, &nbsp; (kein Markdown).
- IDs: positive ints (keine Mindesthöhe); sortOrder lückenlos ab 0.
- Sprache: deutsch, präzise, wissenschaftlich-didaktisch.
 - Antworte ausschließlich als JSON-Objekt gemäß obigem Schema (kein Fließtext, keine Erklärungen, kein Surrounding-Text).
""";
        }

        private async Task<string> BuildTypeGuidanceAsync(PromptType t, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            // 1) Try group-specific override if group has custom prompts enabled
            try
            {
                string? overrideName = null;
                try { overrideName = _http.HttpContext?.Items["PromptAiGroupOverride"] as string; } catch { }

                string? groupName = null;
                if (!string.IsNullOrWhiteSpace(overrideName))
                {
                    groupName = overrideName.Trim();
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
                    if (!useGlobal)
                    {
                        var local = await Task.Run(() =>
                            _db.GroupPromptTypeGuidances
                               .Where(x => x.Group == groupName && x.Type == t)
                               .OrderByDescending(x => x.UpdatedAt)
                               .Select(x => x.GuidanceText)
                               .FirstOrDefault() ?? string.Empty, ct);
                        if (!string.IsNullOrWhiteSpace(local)) return local;
                    }
                }
            }
            catch { /* fall through to global */ }

            // 2) Fallback: latest global type-guidance
            try
            {
                var txt = await Task.Run(() =>
                    _db.PromptTypeGuidances
                       .Where(x => x.Type == t)
                       .OrderByDescending(x => x.UpdatedAt)
                       .Select(x => x.GuidanceText)
                       .FirstOrDefault() ?? string.Empty, ct);
                if (!string.IsNullOrWhiteSpace(txt)) return txt;
            }
            catch { /* fallback below */ }

            var sb = new StringBuilder();
            sb.AppendLine("WICHTIG: Ziel-Modus = '" + t + "'.");
            sb.AppendLine("Alle Filter didaktisch-professionell (Ziele, Scaffolding, Differenzierung, Bewertung, klare Handlungsanweisung).");
            switch (t)
            {
                case PromptType.Text:
                    sb.AppendLine("Fokus: Schreiben, Quellenarbeit, Rollen, formative Checks. Platzhalter: {Niveau}, {Zielgruppe}, {Textsorte}.");
                    break;
                case PromptType.Bild:
                    sb.AppendLine("Fokus: Bild-Generierung (Motiv, Stil, Komposition, Licht). Platzhalter: {Motiv}, {Stil}, {Farbpalette}, {Kamera}.");
                    break;
                case PromptType.Video:
                    sb.AppendLine("Fokus: Video (Szenenplan, Shots, Voice-over, Untertitel). Platzhalter: {Dauer}, {Szenen}, {VoiceOver}, {Untertitel}.");
                    break;
                case PromptType.Sound:
                    sb.AppendLine("Fokus: Audio/Musik (Genre, BPM, Tonart, Struktur). Platzhalter: {Genre}, {BPM}, {Instrumente}, {Stimmung}.");
                    break;
                case PromptType.Bildung:
                    sb.AppendLine("Fokus: Bloom, Forschungsfragen, Methodik, Zitieren, Rubrics. Platzhalter: {Fach}, {Niveau}, {Lernziel}, {Bewertungskriterien}.");
                    break;
                case PromptType.Beruf:
                    sb.AppendLine("Fokus: Meta-Prompting, Evaluation, Iteration, Checklisten. Platzhalter: {Kriterien}, {Revision}, {Feedbackschleifen}.");
                    break;
                case PromptType.Eigenfilter:
                    sb.AppendLine("Fokus: Benutzerdefinierte/Eigene Filter. Platziere begründete Freiheitsgrade in den Platzhaltern.");
                    break;
            }
            sb.AppendLine("Jede info muss expliziten Bezug zu titel/thema/ziele herstellen.");
            return sb.ToString();
        }

        /* ---------------------------- JSON Schema ---------------------------- */

        // RAW STRING => valid JSON (no stray backslashes)
        // Relaxed info-pattern to allow extra paragraphs/lists after first <p>…</p>
        private static string GetOutputJsonSchema(int typeConst) => $$"""
{
  "type": "object",
  "properties": {
    "data": {
      "type": "array",
      "minItems": 1,
      "maxItems": 1,
      "items": {
        "type": "object",
        "additionalProperties": false,
        "properties": {
          "id": { "type": "integer" },
          "name": { "type": "string", "pattern": "^(\\S+)(?:\\s+\\S+){0,3}$" },
          "type": { "type": "integer", "const": {{typeConst}} },
          "displayOrder": { "type": "integer", "const": 0 },
          "filterItems": {
            "type": "array",
            "minItems": 3,
            "maxItems": 30,
            "items": {
              "type": "object",
              "additionalProperties": false,
              "properties": {
                "id": { "type": "integer" },
                "title": { "type": "string", "pattern": "^<p>[\\s\\S]*</p>$" },
                "info": { "type": "string", "pattern": "^<p>[\\s\\S]*</p>(?:[\\s\\S]*)$" },
                "instruction": { "type": "string", "pattern": "^<p>[\\s\\S]*</p>$" },
                "sortOrder": { "type": "integer", "minimum": 0 }
              },
              "required": ["id","title","info","instruction","sortOrder"]
            }
          }
        },
        "required": ["id","name","type","displayOrder","filterItems"]
      }
    },
    "hash": { "type": "string" }
  },
  "required": ["data","hash"],
  "additionalProperties": false
}
""";

        /* -------------------- Normalize & IDs -------------------- */

        private static PromptFilterPayloadDto RepairIntoNewPayload(PromptFormDto form, PromptFilterPayloadDto src, PromptType targetType)
        {
            if (src?.Data == null || src.Data.Count == 0)
            {
                return new PromptFilterPayloadDto { Data = new List<PromptFilterCategoryDto>(), Hash = string.Empty };
            }

            var catSrc = src.Data[0];

            // Category name: 1–4 Wörter; if empty or generic, derive from INPUT/items
            var catName = NormalizeCategoryName(catSrc.Name);
            if (string.IsNullOrWhiteSpace(catName) || string.Equals(catName, "Strukturiert", StringComparison.OrdinalIgnoreCase))
            {
                catName = DeriveFallbackCategoryName(form, catSrc.Items, targetType);
            }

            // Deterministic category id
            var categoryId = DeriveIntId($"{form.Titel}|{form.Thema}|{form.Ziele}|{form.Beschreibung}", minInclusive: 1);

            // Items
            var used = new HashSet<int>();
            var fixedItems = new List<PromptFilterItemDto>(catSrc.Items.Count);
            for (int i = 0; i < catSrc.Items.Count; i++)
            {
                var it = catSrc.Items[i];

                var title = EnsureSingleP(it.Title);        // must be exactly one <p>…</p>
                var info = EnsureLeadingP(it.Info);         // may include extra <p>, <ul>, etc.
                var instruction = EnsureSingleP(it.Instruction);

                var seed = $"{title}\n{info}\n{instruction}\n{i}";
                var baseId = DeriveIntId(seed, minInclusive: 1);
                var itemId = DedupId(baseId, used);

                fixedItems.Add(new PromptFilterItemDto
                {
                    Id = itemId,
                    Title = title,
                    Info = info,
                    Instruction = instruction,
                    SortOrder = i
                });
            }

            var fixedCategory = new PromptFilterCategoryDto
            {
                Id = categoryId,
                Name = catName,
                Type = (int)targetType,
                DisplayOrder = 0,
                Items = fixedItems
            };

            return new PromptFilterPayloadDto
            {
                Data = new List<PromptFilterCategoryDto> { fixedCategory },
                Hash = string.Empty
            };
        }

        private static string NormalizeCategoryName(string? suggested)
        {
            var name = (suggested ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length > 4) name = string.Join(' ', parts.Take(4));
            return name;
        }

        private static string DeriveFallbackCategoryName(PromptFormDto form, IEnumerable<PromptFilterItemDto>? items, PromptType targetType)
        {
            // Simple, robust heuristic based on INPUT content and target type
            var sb = new StringBuilder();
            sb.AppendLine(form.Titel);
            sb.AppendLine(form.Thema);
            sb.AppendLine(form.Ziele);
            sb.AppendLine(form.Beschreibung);
            if (form.Schlüsselbegriffe != null && form.Schlüsselbegriffe.Length > 0)
                sb.AppendLine(string.Join(" ", form.Schlüsselbegriffe));

            var text = Normalize(sb.ToString());

            // TargetType hints
            switch (targetType)
            {
                case PromptType.Bild: return "Visuell";
                case PromptType.Video: return "Video";
                case PromptType.Sound: return "Audio";
                case PromptType.Bildung: if (text.Contains("didakt") || text.Contains("lernen")) return "Didaktik"; break;
                case PromptType.Beruf: if (text.Contains("projekt") || text.Contains("arbeit")) return "Beruflich"; break;
                case PromptType.Eigenfilter: if (text.Contains("eigen") || text.Contains("benutzerdefin")) return "Eigene Filter"; break;
            }

            // Keyword-based mapping
            if (text.Contains("forsch") || text.Contains("studie") || text.Contains("hypoth")) return "Forschung";
            if (text.Contains("plan") || text.Contains("planung") || text.Contains("meilenstein")) return "Planung";
            if (text.Contains("bewert") || text.Contains("kriterien") || text.Contains("rubric")) return "Bewertung";
            if (text.Contains("kontext") || text.Contains("hintergrund") || text.Contains("rahmen")) return "Kontext";
            if (text.Contains("daten") || text.Contains("analyse") || text.Contains("statist")) return "Datenanalyse";
            if (text.Contains("schreib") || text.Contains("text") || text.Contains("essay") || text.Contains("bericht")) return "Schreibstil";

            // Fall back to first Schlüsselbegriff if available (max 2 words)
            if (form.Schlüsselbegriffe != null && form.Schlüsselbegriffe.Length > 0)
            {
                var first = (form.Schlüsselbegriffe[0] ?? string.Empty).Trim();
                var parts = first.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (parts.Length > 2) first = string.Join(' ', parts.Take(2));
                if (!string.IsNullOrWhiteSpace(first)) return first;
            }

            // Neutral generic
            return "Kontext";
        }

        private static int DedupId(int candidate, HashSet<int> used)
        {
            var id = candidate;
            while (!used.Add(id)) id++;
            return id;
        }

        // Preserve extra HTML for info: if it doesn't start with <p>, add one; otherwise keep as-is
        private static string EnsureLeadingP(string? s)
        {
            s ??= string.Empty;
            var t = s.Trim();
            if (t.StartsWith("<p>")) return t;
            return $"<p>{t}</p>";
        }

        // Force a single <p>…</p> block (strip other markup); used for title & instruction
        private static string EnsureSingleP(string? s)
        {
            s ??= string.Empty;
            var t = s.Trim();

            // If multiple paragraphs exist, take the first one’s inner text
            int start = t.IndexOf("<p>", StringComparison.OrdinalIgnoreCase);
            int end = t.IndexOf("</p>", StringComparison.OrdinalIgnoreCase);
            if (start >= 0 && end > start)
            {
                var inner = t.Substring(start + 3, end - (start + 3));
                inner = StripTags(inner);
                return $"<p>{inner}</p>";
            }

            // If no paragraph at all, strip tags and wrap once
            t = StripTags(t);
            return $"<p>{t}</p>";
        }

        private static string StripTags(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var sb = new StringBuilder(input.Length);
            bool inside = false;
            foreach (var ch in input)
            {
                if (ch == '<') { inside = true; continue; }
                if (ch == '>') { inside = false; continue; }
                if (!inside) sb.Append(ch);
            }
            return sb.ToString();
        }

        private static int DeriveIntId(string seed, int minInclusive)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(seed)).Take(4).ToArray(); // 32-bit
            var val = Math.Abs(BitConverter.ToInt32(bytes, 0));
            return minInclusive + (val % 1_000_000_000); // large positive band
        }

        private static string ComputeSha256(string text)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(text);
            var hashBytes = sha.ComputeHash(bytes);
            var sb = new StringBuilder(hashBytes.Length * 2);
            foreach (var b in hashBytes) sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private static string Truncate(string? s, int max)
        {
            s ??= string.Empty;
            if (s.Length <= max) return s;
            return s.Substring(0, max) + " …";
        }

        /* -------------------- Token-sparende Shortlist (ohne externe Embeddings) -------------------- */

        // Einfache, robuste Heuristik:
        // - Normalisiert Eingabetext (Kleinbuchstaben, Diakritika entfernt)
        // - Tokenisiert Technik-Namen (inkl. Klammer-Abkürzungen wie "(CoT)")
        // - Score = Überlappung Name/Abkürzungen + leichte Bonus-Heuristiken für Forschung/Planung/Bewertung
        private static IEnumerable<string> RankTechniquesByForm(PromptFormDto form, int topK)
        {
            var sb = new StringBuilder();
            sb.AppendLine(form.Titel);
            sb.AppendLine(form.Thema);
            sb.AppendLine(form.Ziele);
            sb.AppendLine(form.Beschreibung);
            if (form.Schlüsselbegriffe != null && form.Schlüsselbegriffe.Length > 0)
                sb.AppendLine(string.Join(" ", form.Schlüsselbegriffe));

            var text = Normalize(sb.ToString());
            var scores = new List<(string Name, double Score)>(MasterTechniques.Length);

            foreach (var name in MasterTechniques)
            {
                var tokens = TechniqueTokens(name);
                double s = 0;

                // Name-/Abkürzungs-Treffer
                foreach (var tok in tokens)
                {
                    if (tok.Length < 3) continue; // Rauschen reduzieren
                    if (text.Contains(tok)) s += tok.Length >= 5 ? 1.0 : 0.5;
                }

                // Bonus-Heuristiken
                if (text.Contains("forsch") || text.Contains("hypoth") || text.Contains("studie"))
                {
                    if (name.Contains("Verification", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Retrieval", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Rubric", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Chain-of-Thought", StringComparison.OrdinalIgnoreCase))
                        s += 0.6;
                }
                if (text.Contains("plan") || text.Contains("planung") || text.Contains("meilenstein"))
                {
                    if (name.Contains("Plan", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Planning", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Tree-of-Thought", StringComparison.OrdinalIgnoreCase))
                        s += 0.4;
                }
                if (text.Contains("bewert") || text.Contains("rubric") || text.Contains("kriterien"))
                {
                    if (name.Contains("Rubric", StringComparison.OrdinalIgnoreCase) ||
                        name.Contains("Evaluation", StringComparison.OrdinalIgnoreCase))
                        s += 0.5;
                }

                scores.Add((name, s));
            }

            return scores
                .OrderByDescending(x => x.Score)
                .ThenBy(x => x.Name)
                .Take(Math.Clamp(topK, 3, 12))
                .Select(x => x.Name);
        }

        private static string Normalize(string s)
        {
            s ??= string.Empty;
            s = s.ToLowerInvariant().Trim();
            s = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder(s.Length);
            foreach (var ch in s)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                if (uc != UnicodeCategory.NonSpacingMark) sb.Append(ch);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }

        private static IEnumerable<string> TechniqueTokens(string name)
        {
            // Zerlege auf Wörter, Minus/Slash, extrahiere Klammerkürzel (z. B. CoT)
            var n = Normalize(name);
            var list = new List<string>(8);

            // Abkürzung in Klammern extrahieren
            int lp = name.IndexOf('(');
            int rp = name.IndexOf(')');
            if (lp >= 0 && rp > lp && rp - lp <= 10)
            {
                var abbr = name.Substring(lp + 1, rp - lp - 1).Trim();
                if (abbr.Length >= 2) list.Add(Normalize(abbr));
            }

            // Split by whitespace and punctuation
            var tmp = new StringBuilder();
            foreach (var ch in n)
            {
                if (char.IsLetterOrDigit(ch)) tmp.Append(ch);
                else
                {
                    if (tmp.Length > 0) { list.Add(tmp.ToString()); tmp.Clear(); }
                }
            }
            if (tmp.Length > 0) list.Add(tmp.ToString());

            // Kombinierer: chainofthought → chain, thought, chainofthought
            var joined = string.Join("", list);
            if (joined.Length >= 8) list.Add(joined);

            // dedupe
            return list.Distinct();
        }
    }
}
