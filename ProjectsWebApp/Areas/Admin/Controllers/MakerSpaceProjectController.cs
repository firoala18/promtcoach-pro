using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using ProjectsWebApp.DataAccsess.Repository.IRepository.Intefaces;
using ProjectsWebApp.Models.ViewModels;
using System.Text.Json;
using System.IO.Compression;
using System.Text;
using System.IO;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class MakerSpaceProjectController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public MakerSpaceProjectController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        return View();
    }



    [HttpPost]
    public IActionResult ToggleTop(int id, bool isTop)
    {
        var project = _unitOfWork.MakerSpaceProject.Get(p => p.Id == id);
        if (project == null)
            return Json(new { success = false, message = "Projekt nicht gefunden." });

        project.Top = isTop;
        _unitOfWork.MakerSpaceProject.Update(project);
        _unitOfWork.Save();

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult ToggleForschung(int id, bool isForschung)
    {
        var project = _unitOfWork.MakerSpaceProject.Get(p => p.Id == id);
        if (project == null)
            return Json(new { success = false, message = "Projekt nicht gefunden." });

        project.Forschung = isForschung;
        _unitOfWork.MakerSpaceProject.Update(project);
        _unitOfWork.Save();

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult ToggleDownload(int id, bool isDownload)
    {
        var project = _unitOfWork.MakerSpaceProject.Get(p => p.Id == id);
        if (project == null)
            return Json(new { success = false, message = "Projekt nicht gefunden." });

        project.download = isDownload;
        _unitOfWork.MakerSpaceProject.Update(project);
        _unitOfWork.Save();

        return Json(new { success = true });
    }


    [HttpPost]
    public IActionResult ToggleTutorial(int id, bool isTutorial)
    {
        var project = _unitOfWork.MakerSpaceProject.Get(p => p.Id == id);
        if (project == null)
            return Json(new { success = false, message = "Projekt nicht gefunden." });

        project.tutorial = isTutorial;
        _unitOfWork.MakerSpaceProject.Update(project);
        _unitOfWork.Save();

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult ToggleEvent(int id, bool isEvent)
    {
        var project = _unitOfWork.MakerSpaceProject.Get(p => p.Id == id);
        if (project == null)
            return Json(new { success = false, message = "Projekt nicht gefunden." });

        project.events = isEvent;
        _unitOfWork.MakerSpaceProject.Update(project);
        _unitOfWork.Save();

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult ToggleNetzwerk(int id, bool isNetzwerk)
    {
        var project = _unitOfWork.MakerSpaceProject.Get(p => p.Id == id);
        if (project == null)
            return Json(new { success = false, message = "Projekt nicht gefunden." });

        project.netzwerk = isNetzwerk;
        _unitOfWork.MakerSpaceProject.Update(project);
        _unitOfWork.Save();

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult ToggleLesezeichen(int id, bool isLesezeichen)
    {
        var project = _unitOfWork.MakerSpaceProject.Get(p => p.Id == id);
        if (project == null)
            return Json(new { success = false, message = "Projekt nicht gefunden." });

        project.lesezeichen = isLesezeichen;
        _unitOfWork.MakerSpaceProject.Update(project);
        _unitOfWork.Save();

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult ToggleITRecht(int id, bool isITRecht)
    {
        var project = _unitOfWork.MakerSpaceProject.Get(p => p.Id == id);
        if (project == null)
            return Json(new { success = false, message = "Projekt nicht gefunden." });

        project.ITRecht = isITRecht;
        _unitOfWork.MakerSpaceProject.Update(project);
        _unitOfWork.Save();

        return Json(new { success = true });
    }

    [HttpPost]
    public IActionResult ToggleBeitraege(int id, bool isBeitraege)
    {
        var project = _unitOfWork.MakerSpaceProject.Get(p => p.Id == id);
        if (project == null)
            return Json(new { success = false, message = "Projekt nicht gefunden." });

        project.Beitraege = isBeitraege;
        _unitOfWork.MakerSpaceProject.Update(project);
        _unitOfWork.Save();

        return Json(new { success = true });
    }


    public IActionResult Upsert(int? id)
    {
        MakerSpaceVM makerSpaceVM = new()
        {
            MakerSpaceProject = id == null
                ? new MakerSpaceProject()
                : _unitOfWork.MakerSpaceProject.Get(u => u.Id == id)
        };

        if (makerSpaceVM.MakerSpaceProject == null)
            return NotFound();

        // Dropdowns immer befüllen
        PopulateDropdownLists(makerSpaceVM);

        return View(makerSpaceVM); // Immer das VM zurückgeben
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(100_000_000)]
    public IActionResult Upsert(MakerSpaceVM makerSpaceVM, IFormFile? file)
    {
        if (!ModelState.IsValid)
        {
            PopulateDropdownLists(makerSpaceVM);
            return View(makerSpaceVM);
        }
        // Mindest­anforderung: Entweder Tags oder mindestens ein Toggle
        bool anyToggle = makerSpaceVM.MakerSpaceProject.Top
                         || makerSpaceVM.MakerSpaceProject.Forschung
                         || makerSpaceVM.MakerSpaceProject.lesezeichen
                         || makerSpaceVM.MakerSpaceProject.tutorial
                         || makerSpaceVM.MakerSpaceProject.ITRecht
                         || makerSpaceVM.MakerSpaceProject.Beitraege;

        if (string.IsNullOrWhiteSpace(makerSpaceVM.MakerSpaceProject.Tags) && !anyToggle)
        {
            ModelState.AddModelError(string.Empty,
                "Bitte mindestens einen Tag oder eine Kategorie auswählen.");
            PopulateDropdownLists(makerSpaceVM);
            return View(makerSpaceVM);
        }


        string wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var project = makerSpaceVM.MakerSpaceProject;

        if (file != null && file.Length > 0)
        {
            // 1. Upload the new file

            string uploadsFolder = Path.Combine(wwwRootPath, "images", "makerspace");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(uploadsFolder, fileName);

            // Delete old uploaded image if exists
            if (project.Id != 0)
            {
                var existingProject = _unitOfWork.MakerSpaceProject.Get(p => p.Id == project.Id);
                if (existingProject != null && !string.IsNullOrEmpty(existingProject.ImageUrl) && existingProject.ImageUrl.StartsWith("/images"))
                {
                    var oldImagePath = Path.Combine(wwwRootPath, existingProject.ImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
            }

            // Save the new image
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }

            project.ImageUrl = "/images/makerspace/" + fileName;
        }
        else
        {
            if (project.Id != 0)
            {
                var existingProject = _unitOfWork.MakerSpaceProject.Get(p => p.Id == project.Id);
                if (existingProject != null)
                {
                    // ⚡ Keep whatever was set (URL or existing upload)
                    if (string.IsNullOrWhiteSpace(project.ImageUrl))
                        project.ImageUrl = existingProject.ImageUrl;
                }
            }
        }


        if (project.Id == 0)
            _unitOfWork.MakerSpaceProject.Add(project);
        else
            _unitOfWork.MakerSpaceProject.Update(project);

        _unitOfWork.Save();
        return RedirectToAction(nameof(Index));
    }


    private void PopulateDropdownLists(MakerSpaceVM makerSpaceVM)
    {
        var projects = _unitOfWork.Project.GetAll();

        makerSpaceVM.ExistingTags = projects
            .Where(p => !string.IsNullOrEmpty(p.Tags))
            .SelectMany(p => p.Tags.Split(','))
            .Select(tag => tag.Trim())
            .Distinct()
            .ToList();
    }


    // Export all MakerSpace projects (including local makerspace images) as a ZIP
    [HttpGet]
    public IActionResult Export()
    {
        try
        {
            var projects = _unitOfWork.MakerSpaceProject.GetAll().ToList();
            var exportItems = new List<MakerSpaceProjectExportDto>();

            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var imageRoot = Path.Combine(wwwRootPath, "images", "makerspace");

            using var ms = new MemoryStream();
            using (var archive = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
            {
                foreach (var p in projects)
                {
                    string? imageFileName = null;
                    if (!string.IsNullOrEmpty(p.ImageUrl) &&
                        p.ImageUrl.StartsWith("/images/makerspace", StringComparison.OrdinalIgnoreCase))
                    {
                        imageFileName = Path.GetFileName(p.ImageUrl);
                        var imagePath = Path.Combine(imageRoot, imageFileName);
                        if (System.IO.File.Exists(imagePath))
                        {
                            var entry = archive.CreateEntry($"images/{imageFileName}", CompressionLevel.Optimal);
                            using var entryStream = entry.Open();
                            using var fileStream = System.IO.File.OpenRead(imagePath);
                            fileStream.CopyTo(entryStream);
                        }
                    }

                    exportItems.Add(new MakerSpaceProjectExportDto
                    {
                        Id = p.Id,
                        DisplayOrder = p.DisplayOrder,
                        Title = p.Title,
                        Tags = p.Tags,
                        Top = p.Top,
                        Description = p.Description,
                        Forschung = p.Forschung,
                        ProjectUrl = p.ProjectUrl,
                        download = p.download,
                        tutorial = p.tutorial,
                        events = p.events,
                        netzwerk = p.netzwerk,
                        lesezeichen = p.lesezeichen,
                        ITRecht = p.ITRecht,
                        Beitraege = p.Beitraege,
                        ImageUrl = p.ImageUrl,
                        ImageFileName = imageFileName
                    });
                }

                var jsonEntry = archive.CreateEntry("makerspace-projects.json", CompressionLevel.Optimal);
                using (var jsonStream = jsonEntry.Open())
                {
                    var options = new JsonSerializerOptions { WriteIndented = true };
                    JsonSerializer.Serialize(jsonStream, new MakerSpaceProjectExportRoot { data = exportItems }, options);
                }
            }

            ms.Position = 0;
            var fileName = $"makerspace-projects-{DateTime.UtcNow:yyyyMMddHHmmss}.zip";
            return File(ms.ToArray(), "application/zip", fileName);
        }
        catch
        {
            TempData["error"] = "Export der KIBar-Projekte ist fehlgeschlagen.";
            return RedirectToAction(nameof(Index));
        }
    }

    // Import MakerSpace projects (and images) from an export ZIP
    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(200_000_000)]
    public IActionResult Import(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            TempData["error"] = "Bitte wählen Sie eine Exportdatei (ZIP) aus.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var imageRoot = Path.Combine(wwwRootPath, "images", "makerspace");
            Directory.CreateDirectory(imageRoot);

            MakerSpaceProjectExportRoot? root;
            using (var archive = new ZipArchive(file.OpenReadStream(), ZipArchiveMode.Read))
            {
                var jsonEntry = archive.Entries
                    .FirstOrDefault(e => e.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase));

                if (jsonEntry == null)
                {
                    TempData["error"] = "In der ZIP-Datei wurde keine Projekt-JSON gefunden.";
                    return RedirectToAction(nameof(Index));
                }

                using (var reader = new StreamReader(jsonEntry.Open(), Encoding.UTF8))
                {
                    var json = reader.ReadToEnd();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    root = JsonSerializer.Deserialize<MakerSpaceProjectExportRoot>(json, options);
                }

                if (root == null || root.data == null || root.data.Count == 0)
                {
                    TempData["error"] = "Die Projektliste in der Exportdatei ist leer.";
                    return RedirectToAction(nameof(Index));
                }

                // Entferne bestehende Projekte, bevor importiert wird
                var existing = _unitOfWork.MakerSpaceProject.GetAll().ToList();
                foreach (var p in existing)
                {
                    _unitOfWork.MakerSpaceProject.Remove(p);
                }
                _unitOfWork.Save();

                foreach (var dto in root.data)
                {
                    string? imageUrl = dto.ImageUrl;

                    if (!string.IsNullOrWhiteSpace(dto.ImageFileName))
                    {
                        var entry = archive.Entries
                            .FirstOrDefault(e => string.Equals(e.FullName, $"images/{dto.ImageFileName}", StringComparison.OrdinalIgnoreCase));

                        if (entry != null)
                        {
                            var targetPath = Path.Combine(imageRoot, dto.ImageFileName);
                            using var entryStream = entry.Open();
                            using var outStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write);
                            entryStream.CopyTo(outStream);

                            imageUrl = "/images/makerspace/" + dto.ImageFileName;
                        }
                        else if (string.IsNullOrEmpty(imageUrl))
                        {
                            // Fallback: wenn eine ImageFileName angegeben ist, aber kein Eintrag vorhanden ist
                            imageUrl = "/images/makerspace/" + dto.ImageFileName;
                        }
                    }

                    var project = new MakerSpaceProject
                    {
                        Title = dto.Title ?? string.Empty,
                        Tags = dto.Tags,
                        Description = dto.Description ?? string.Empty,
                        DisplayOrder = dto.DisplayOrder,
                        ProjectUrl = dto.ProjectUrl ?? string.Empty,
                        Top = dto.Top,
                        Forschung = dto.Forschung,
                        download = dto.download,
                        tutorial = dto.tutorial,
                        events = dto.events,
                        netzwerk = dto.netzwerk,
                        lesezeichen = dto.lesezeichen,
                        ITRecht = dto.ITRecht,
                        Beitraege = dto.Beitraege,
                        ImageUrl = imageUrl
                    };

                    _unitOfWork.MakerSpaceProject.Add(project);
                }
            }

            _unitOfWork.Save();
            TempData["success"] = "KIBar-Projekte wurden erfolgreich importiert.";
        }
        catch
        {
            TempData["error"] = "Import der KIBar-Projekte ist fehlgeschlagen.";
        }

        return RedirectToAction(nameof(Index));
    }

    private class MakerSpaceProjectExportDto
    {
        public int Id { get; set; }
        public int DisplayOrder { get; set; }
        public string? Title { get; set; }
        public string? Tags { get; set; }
        public bool Top { get; set; }
        public string? Description { get; set; }
        public bool Forschung { get; set; }
        public string? ProjectUrl { get; set; }
        public bool download { get; set; }
        public bool tutorial { get; set; }
        public bool events { get; set; }
        public bool netzwerk { get; set; }
        public bool lesezeichen { get; set; }
        public bool ITRecht { get; set; }
        public bool Beitraege { get; set; }
        public string? ImageUrl { get; set; }
        public string? ImageFileName { get; set; }
    }

    private class MakerSpaceProjectExportRoot
    {
        public List<MakerSpaceProjectExportDto> data { get; set; } = new List<MakerSpaceProjectExportDto>();
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteAll()
    {
        try
        {
            var all = _unitOfWork.MakerSpaceProject.GetAll().ToList();

            var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            foreach (var project in all)
            {
                if (!string.IsNullOrEmpty(project.ImageUrl) &&
                    project.ImageUrl.StartsWith("/images/makerspace", StringComparison.OrdinalIgnoreCase))
                {
                    var imgPath = Path.Combine(wwwroot,
                        project.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(imgPath))
                    {
                        System.IO.File.Delete(imgPath);
                    }
                }

                _unitOfWork.MakerSpaceProject.Remove(project);
            }

            _unitOfWork.Save();
            TempData["success"] = "Alle KIBar-Projekte wurden gelöscht.";
        }
        catch
        {
            TempData["error"] = "Löschen aller KIBar-Projekte ist fehlgeschlagen.";
        }

        return RedirectToAction(nameof(Index));
    }


    // POST: Admin/MakerSpaceProject/Delete
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var project = _unitOfWork.MakerSpaceProject.Get(p => p.Id == id);
        if (project == null)
            return Json(new { success = false, message = "Projekt nicht gefunden." });

        /* optional: hochgeladenes Bild mit‑löschen */
        if (!string.IsNullOrEmpty(project.ImageUrl) &&
            project.ImageUrl.StartsWith("/images", StringComparison.OrdinalIgnoreCase))
        {
            var wwwroot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var imgPath = Path.Combine(wwwroot,
                              project.ImageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(imgPath))
                System.IO.File.Delete(imgPath);
        }

        _unitOfWork.MakerSpaceProject.Remove(project);
        _unitOfWork.Save();

        return Json(new { success = true, message = "Projekt wurde gelöscht." });
    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll()
    {
        var list = _unitOfWork.MakerSpaceProject.GetAll()
            .Select(p => new
            {
                p.Id,
                verlauf = p.DisplayOrder,
                p.Title,
                p.Tags,
                p.Top,
                p.Description,
                forschung = p.Forschung,
                projectUrl = p.ProjectUrl,
                download = p.download,
                lesezeichen = p.lesezeichen,
                p.tutorial,   
                p.events,         
                p.netzwerk,
                itRecht = p.ITRecht,
                beitraege = p.Beitraege,
                imageUrl = p.ImageUrl
            }).ToList();



        return Json(new { data = list });
    }
    #endregion
}
