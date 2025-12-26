using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Data;
using ProjectsWebApp.Models;
using System;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public sealed class ApiKeysController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ApiKeysController> _logger;

        public ApiKeysController(ApplicationDbContext db, UserManager<IdentityUser> userManager, ILogger<ApiKeysController> logger)
        {
            _db = db;
            _userManager = userManager;
            _logger = logger;
        }

        // GET: /Admin/ApiKeys/Edit
        [HttpGet]
        public IActionResult Edit()
        {
            try
            {
                // Load the most recently updated key; do not write in GET
                var rec = _db.ApiKeySettings
                              .OrderByDescending(x => x.UpdatedAt)
                              .FirstOrDefault();
                return View(rec ?? new ApiKeySetting());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GET /Admin/ApiKeys/Edit failed");
                TempData["error"] = $"Speichern fehlgeschlagen. (Fehler: {ex.Message})";
                return View(new ApiKeySetting { Id = 1 });
            }
        }

        // POST: /Admin/ApiKeys/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ApiKeySetting form)
        {
            try
            {
                // Work on latest record; create new if none exists
                var rec = _db.ApiKeySettings
                              .OrderByDescending(x => x.UpdatedAt)
                              .FirstOrDefault();
                if (rec == null)
                {
                    rec = new ApiKeySetting();
                    _db.ApiKeySettings.Add(rec);
                }

                string? s(string? v) => string.IsNullOrWhiteSpace(v) ? null : v.Trim();

                var now = DateTime.UtcNow;
                var userId = _userManager.GetUserId(User);

                bool openAiChanged = false;
                bool kisskiChanged = false;
                bool geminiChanged = false;
                bool activeProviderChanged = false;

                // Active provider (global)
                var newActiveProvider = s(form.ActiveProvider);
                if (!string.IsNullOrEmpty(newActiveProvider) && newActiveProvider != rec.ActiveProvider)
                {
                    rec.ActiveProvider = newActiveProvider;
                    activeProviderChanged = true;
                }

                // OpenAI
                var newOpenAIKey = s(form.OpenAIKey);
                var newOpenAIEmbeddingsKey = s(form.OpenAIEmbeddingsKey);
                var newOpenAIModel = s(form.OpenAIModel);

                if (newOpenAIKey != null && newOpenAIKey != rec.OpenAIKey)
                {
                    rec.OpenAIKey = newOpenAIKey;
                    openAiChanged = true;
                }
                if (newOpenAIEmbeddingsKey != null && newOpenAIEmbeddingsKey != rec.OpenAIEmbeddingsKey)
                {
                    rec.OpenAIEmbeddingsKey = newOpenAIEmbeddingsKey;
                    openAiChanged = true;
                }
                if (newOpenAIModel != null && newOpenAIModel != rec.OpenAIModel)
                {
                    rec.OpenAIModel = newOpenAIModel;
                    openAiChanged = true;
                }

                // Kisski
                var newKisskiApiKey = s(form.KisskiApiKey);
                var newKisskiModel = s(form.KisskiModel);

                if (newKisskiApiKey != null && newKisskiApiKey != rec.KisskiApiKey)
                {
                    rec.KisskiApiKey = newKisskiApiKey;
                    kisskiChanged = true;
                }
                if (newKisskiModel != null && newKisskiModel != rec.KisskiModel)
                {
                    rec.KisskiModel = newKisskiModel;
                    kisskiChanged = true;
                }

                // Gemini
                var newGeminiApiKey = s(form.GeminiApiKey);
                var newGeminiModel = s(form.GeminiModel);

                if (newGeminiApiKey != null && newGeminiApiKey != rec.GeminiApiKey)
                {
                    rec.GeminiApiKey = newGeminiApiKey;
                    geminiChanged = true;
                }
                if (newGeminiModel != null && newGeminiModel != rec.GeminiModel)
                {
                    rec.GeminiModel = newGeminiModel;
                    geminiChanged = true;
                }

                if (openAiChanged)
                {
                    rec.OpenAIUpdatedAt = now;
                }
                if (kisskiChanged)
                {
                    rec.KisskiUpdatedAt = now;
                }
                if (geminiChanged)
                {
                    rec.GeminiUpdatedAt = now;
                }

                if (openAiChanged || kisskiChanged || geminiChanged || activeProviderChanged)
                {
                    rec.UpdatedAt = now;
                    rec.UpdatedByUserId = userId;
                }

                _db.SaveChanges();
                TempData["success"] = "Globale Einstellungen gespeichert.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "POST /Admin/ApiKeys/Edit failed");
                TempData["error"] = $"Speichern fehlgeschlagen. (Fehler: {ex.Message})";
            }
            return RedirectToAction(nameof(Edit));
        }
    }
}
