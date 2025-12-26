using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ProjectsWebApp.DataAccsess.Repository.IRepository;

namespace ProjectsWebApp.ViewComponents
{
    public class BreadcrumbViewComponent : ViewComponent
    {
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        public BreadcrumbViewComponent(
            IActionContextAccessor actionContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _actionContextAccessor = actionContextAccessor;
            _unitOfWork = unitOfWork;
        }

        public IViewComponentResult Invoke()
        {
            var rd = _actionContextAccessor.ActionContext.RouteData;
            var controller = rd.Values["controller"]?.ToString();
            var action = rd.Values["action"]?.ToString();
            var area = rd.Values["area"]?.ToString();

            var crumbs = new List<(string Title, string Url)>();

            // Root link - Startseite
            crumbs.Add((
                "Startseite",
                Url.Action("Home", "Home", new { area = "User" })
            ));

            // ============================================================
            // ADMIN AREA
            // ============================================================
            if (string.Equals(area, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                crumbs.Add((
                    "Administration",
                    Url.Action("Index", "FilterCategory", new { area = "Admin" })
                ));

                // Controller-specific breadcrumbs for Admin
                var adminControllerLabels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "FilterCategory", "Filter-Kategorien" },
                    { "PromptModel", "Modelle" },
                    { "PromptTemplate", "Prompt-Vorlagen" },
                    { "PromptWord", "Prompt Keys" },
                    { "PromptFeature", "Prompt-Funktionen" },
                    { "RegistrationCodes", "Einladungscodes" },
                    { "Groups", "Räume" },
                    { "Assistant", "KI-Assistenten" },
                    { "Event", "Veranstaltungen" },
                    { "Skill", "Kompetenzen" },
                    { "Category", "Kategorien" },
                    { "Fakultaet", "Fakultäten" },
                    { "Fachgruppen", "Fachgruppen" },
                    { "TechAnforderung", "Technische Anforderungen" },
                    { "SliderItem", "Slider-Elemente" },
                    { "PortalCard", "Portal-Karten" },
                    { "LandingItem", "Prompt Engineering" },
                    { "MitmachenContent", "Tipps" },
                    { "Urheberrecht", "Urheberrecht-Inhalte" },
                     { "ImpressumContent", "Impressum-Inhalte" },
                     { "DatenschutzContent", "Datenschutz-Inhalte" },
                    { "MakerSpaceDescription", "KIBar Beschreibung" },
                    { "MakerSpaceProject", "KIBar Inhalte" },
                    { "SiteUsers", "Benutzerverwaltung" },
                    { "ApiKeys", "API-Schlüssel" },
                    { "PromptAiConfig", "System-Prompts" },
                    { "PromptKeyword", "Schlüsselbegriffe" },
                    { "LockedUsers", "Gesperrte Konten" },
                    { "ContactEmail", "Kontakt-E-Mail-Managment" },
                };

                var adminActionLabels = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Index", "Übersicht" },
                    { "Create", "Erstellen" },
                    { "Upsert", "Bearbeiten" },
                    { "Edit", "Bearbeiten" },
                    { "Delete", "Löschen" },
                    { "Details", "Details" },
                    { "UserAnalytics", "Benutzeranalysen" },
                };

                // Controllers that don't have an Index page (skip intermediate link)
                var controllersWithoutIndex = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "ApiKeys",
                    "ContactEmail"
                };

                // Add controller link if not on Index and controller has an Index page
                if (!string.Equals(action, "Index", StringComparison.OrdinalIgnoreCase)
                    && controller != null
                    && !controllersWithoutIndex.Contains(controller))
                {
                    var controllerLabel = adminControllerLabels.TryGetValue(controller, out var cl) ? cl : controller;
                    crumbs.Add((
                        controllerLabel,
                        Url.Action("Index", controller, new { area = "Admin" })
                    ));
                }

                // Add action/page title
                var pageTitle = ViewContext.ViewData["Title"]?.ToString();
                if (string.IsNullOrEmpty(pageTitle) && action != null)
                {
                    pageTitle = adminActionLabels.TryGetValue(action, out var al) ? al : action;
                }

                if (!string.IsNullOrEmpty(pageTitle) && !string.Equals(action, "Index", StringComparison.OrdinalIgnoreCase))
                {
                    crumbs.Add((pageTitle, ""));
                }
                else if (controller != null)
                {
                    var controllerLabel = adminControllerLabels.TryGetValue(controller, out var cl) ? cl : controller;
                    crumbs.Add((controllerLabel, ""));
                }

                return View(crumbs);
            }

            // ============================================================
            // USER AREA - KIBar Details
            // ============================================================
            if (controller == "Home" && action == "KIBarDetails")
            {
                crumbs.Add((
                    "KIBar",
                    Url.Action("KIBar", "Home", new { area = "User" })
                ));

                var title = ViewContext.ViewData["Title"]?.ToString() ?? "Details";
                crumbs.Add((title, ""));
                return View(crumbs);
            }

            // ============================================================
            // USER AREA - KIBar Overview
            // ============================================================
            if (controller == "Home" && action == "KIBar")
            {
                crumbs.Add(("KIBar", ""));
                return View(crumbs);
            }

            // ============================================================
            // USER AREA - Bibliothek
            // ============================================================
            if (controller == "Home" && action == "Bibliothek")
            {
                crumbs.Add(("Bibliothek", ""));
                return View(crumbs);
            }

            // ============================================================
            // USER AREA - PromptDetails
            // ============================================================
            if (controller == "Home" && action == "PromptDetails")
            {
                crumbs.Add((
                    "Bibliothek",
                    Url.Action("Bibliothek", "Home", new { area = "User" })
                ));

                crumbs.Add(("Prompt analysieren", ""));
                return View(crumbs);
            }

            // ============================================================
            // USER AREA - MyPrompts/Index (Prompt-Sammlung)
            // ============================================================
            if (controller == "MyPrompts" && action == "Index")
            {
                crumbs.Add(("Sammlung", ""));
                return View(crumbs);
            }

            // ============================================================
            // USER AREA - MyPrompts/Details
            // ============================================================
            if (controller == "MyPrompts" && action == "Details")
            {
                crumbs.Add((
                    "Sammlung",
                    Url.Action("Index", "MyPrompts", new { area = "User" })
                ));

                crumbs.Add(("Prompt analysieren", ""));
                return View(crumbs);
            }

            // ============================================================
            // USER AREA - Landing / Prompt Engineering Overview
            // ============================================================
            if (controller == "Home" && action == "Landing")
            {
                crumbs.Add(("Prompt-Engineering", ""));
                return View(crumbs);
            }

            if (controller == "Home" && action == "PromptEngineering")
            {
                crumbs.Add(("Prompt-Engineering", ""));
                return View(crumbs);
            }

            // ============================================================
            // USER AREA - PromptAssistent (Prompt-Entwicklung)
            // ============================================================
            if (controller == "Home" && action == "PromptAssistent")
            {
                crumbs.Add((
                    "Prompt-Engineering",
                    Url.Action("PromptEngineering", "Home", new { area = "User" })
                ));

                // Get type from query parameter
                var typeParam = HttpContext?.Request?.Query["type"].ToString() ?? string.Empty;
                var typeNorm = typeParam?.Trim().ToLowerInvariant();

                string typeLabel = typeNorm switch
                {
                    "text" => "Text",
                    "bild" => "Bild",
                    "video" => "Video",
                    "sound" => "Sound",
                    "beruf" => "Agenten",
                    "bildung" => "Domäne",
                    "eigenfilter" => "Szenarien",
                    "framework" => "Framework",
                    "meta" => "Meta",
                    _ => string.Empty
                };

                crumbs.Add((
                    "Promptentwicklung",
                    Url.Action("PromptAssistent", "Home", new { area = "User" })
                ));

                if (!string.IsNullOrEmpty(typeLabel))
                {
                    crumbs.Add((typeLabel, ""));
                }
                return View(crumbs);
            }

            // ============================================================
            // USER AREA - AssistantChat (KI-Assistent)
            // ============================================================
            if (controller == "Home" && action == "AssistantChat")
            {
                crumbs.Add((
                    "KI-Assistenten",
                    Url.Action("Assistenten", "Home", new { area = "User" })
                ));

                var assistantTitle = ViewContext.ViewData["Title"]?.ToString() ?? "Chat";
                crumbs.Add((assistantTitle, ""));
                return View(crumbs);
            }

            // ============================================================
            // USER AREA - Projects
            // ============================================================
            if (controller == "Home" && action == "Index")
            {
                crumbs.Add(("Projekte", ""));
                return View(crumbs);
            }

            if (controller == "Home" && action == "Details")
            {
                crumbs.Add((
                    "Projekte",
                    Url.Action("Index", "Home", new { area = "User" })
                ));

                var projectTitle = ViewContext.ViewData["Title"]?.ToString() ?? "Projekt-Details";
                crumbs.Add((projectTitle, ""));
                return View(crumbs);
            }

            // ============================================================
            // USER AREA - Events
            // ============================================================
            if (controller == "Event" && action == "Index")
            {
                crumbs.Add(("Veranstaltungen", ""));
                return View(crumbs);
            }

            if (controller == "Event" && action == "Details")
            {
                crumbs.Add((
                    "Veranstaltungen",
                    Url.Action("Index", "Event", new { area = "User" })
                ));

                var eventTitle = ViewContext.ViewData["Title"]?.ToString() ?? "Veranstaltungs-Details";
                crumbs.Add((eventTitle, ""));
                return View(crumbs);
            }

            // ============================================================
            // USER AREA - Team/Kontakt
            // ============================================================
            if (controller == "Kontakt" && action == "Index")
            {
                crumbs.Add(("Unser Team", ""));
                return View(crumbs);
            }

            // ============================================================
            // USER AREA - Skills
            // ============================================================
            if (controller == "Skills" && action == "Index")
            {
                crumbs.Add(("Kompetenzen", ""));
                return View(crumbs);
            }

            // ============================================================
            // USER AREA - MakerSpace
            // ============================================================
            if (controller == "Home" && action == "MakerSpace")
            {
                crumbs.Add(("MakerSpace", ""));
                return View(crumbs);
            }

            // ============================================================
            // USER AREA - Home (Startseite)
            // ============================================================
            if (controller == "Home" && action == "Home")
            {
                // Just show "Startseite" as current page
                crumbs.Clear();
                crumbs.Add(("Startseite", ""));
                return View(crumbs);
            }

            // ============================================================
            // IDENTITY AREA - Account Management
            // ============================================================
            if (string.Equals(area, "Identity", StringComparison.OrdinalIgnoreCase))
            {
                crumbs.Add((
                    "Kontoverwaltung",
                    "/Identity/Account/Manage"
                ));

                var pageTitle = ViewContext.ViewData["Title"]?.ToString();
                if (!string.IsNullOrEmpty(pageTitle))
                {
                    crumbs.Add((pageTitle, ""));
                }
                return View(crumbs);
            }

            // ============================================================
            // FALLBACK - Generic handling
            // ============================================================
            if (string.Equals(action, "Index", StringComparison.OrdinalIgnoreCase))
            {
                var indexTitles = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Home", "Projekte" },
                    { "Event", "Veranstaltungen" },
                    { "Skills", "Kompetenzen" },
                    { "Kontakt", "Unser Team" },
                    { "Bibliothek", "Prompt-Bibliothek" },
                    { "FilterCategory", "Filter-Kategorien" },
                };

                var title = indexTitles.TryGetValue(controller ?? "", out var t)
                            ? t
                            : (controller ?? "Übersicht");

                crumbs.Add((title, ""));
                return View(crumbs);
            }

            // Generic Details fallback
            if (string.Equals(action, "Details", StringComparison.OrdinalIgnoreCase))
            {
                var detailsParents = new Dictionary<string, (string Label, string Controller)>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Event", ("Veranstaltungen", "Event") },
                    { "Home", ("Projekte", "Home") },
                };

                if (controller != null && detailsParents.TryGetValue(controller, out var parent))
                {
                    crumbs.Add((
                        parent.Label,
                        Url.Action("Index", parent.Controller, new { area = area ?? "User" })
                    ));
                }

                var detailTitle = ViewContext.ViewData["Title"]?.ToString() ?? "Details";
                crumbs.Add((detailTitle, ""));
                return View(crumbs);
            }

            // Ultimate fallback
            var fallback = ViewContext.ViewData["Title"]?.ToString() ?? action ?? "Seite";
            crumbs.Add((fallback, ""));
            return View(crumbs);
        }
    }
}
