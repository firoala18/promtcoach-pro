using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models.ViewModels;
using ProjectsWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace ProjectsWebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Restrict access to admin users
    public class ProjectController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const string DefaultImagePath = @"/images/Projects/default.jpg";

        public ProjectController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        #region Index

        public IActionResult Index(bool isVK = false)
        {
            var projects = _unitOfWork.Project.GetAll()
                            .Where(p => p.IsVirtuellesKlassenzimmer == isVK)
                            .OrderBy(p => p.DisplayOrder)
                            .ToList();

            ViewBag.IsVK = isVK; // optional: pass to view if needed

            return View(projects);
        }

        #endregion

        #region Upsert

        // GET: Add or Edit Project
        // In Upsert (GET method)
        public IActionResult Upsert(int? id, bool isVK = false)

        {
            var projectVM = new ProjectVM
            {
                Project = id == null
                    ? new Project()
                    : _unitOfWork.GetRepository<Project>()
                        .Get(u => u.Id == id, includeProperties: "Images,Videos")
            };

            if (id != null)
            {
                var existingProject = projectVM.Project;

                projectVM.SelectedCategories = existingProject.CategoryIds?.Split(',').Select(int.Parse).ToList() ?? new List<int>();
                projectVM.SelectedFakultaets = existingProject.FakultaetIds?.Split(',').Select(int.Parse).ToList() ?? new List<int>();
                projectVM.SelectedFachgruppen = existingProject.FachgruppenIds?.Split(',').Select(int.Parse).ToList() ?? new List<int>();
                projectVM.SelectedTechAnforderungen = existingProject.TechAnforderungIds?.Split(',').Select(int.Parse).ToList() ?? new List<int>();
            }


            ViewBag.IsVK = isVK; 

            // Populate dropdown lists for multi-select options
            PopulateDropdownLists(projectVM);

            return View(projectVM);
        }

        [HttpPost]
        [RequestSizeLimit(500_000_000)]
        public IActionResult Upsert(ProjectVM projectVM,

List<IFormFile> files,
List<IFormFile> videoFiles,
int? mainImageId,
List<int> selectedCategories,
List<int> selectedFakultaets,
List<int> selectedFachgruppen,
List<int> selectedTechAnforderungen,
string? externalImageUrl,
bool? isVK = false)
        {



            // Validation to ensure at least one checkbox is selected for each property
            if (selectedCategories == null || !selectedCategories.Any())
            {
                ModelState.AddModelError("SelectedCategories", "At least one category must be selected.");
            }
            if (selectedFakultaets == null || !selectedFakultaets.Any())
            {
                ModelState.AddModelError("SelectedFakultaets", "At least one faculty must be selected.");
            }
            if (selectedFachgruppen == null || !selectedFachgruppen.Any())
            {
                ModelState.AddModelError("SelectedFachgruppen", "At least one Fachgruppe must be selected.");
            }
            if (selectedTechAnforderungen == null || !selectedTechAnforderungen.Any())
            {
                ModelState.AddModelError("SelectedTechAnforderungen", "At least one technical requirement must be selected.");
            }


            if (ModelState.IsValid)
            {
                Project existingProject;

                // Check if it's a new project or updating an existing one
                if (projectVM.Project.Id == 0)
                {
                    projectVM.Project.IsEnabled = false;
                    // Assign selected IDs as comma-separated strings
                    projectVM.Project.CategoryIds = string.Join(",", selectedCategories);
                    projectVM.Project.FakultaetIds = string.Join(",", selectedFakultaets);
                    projectVM.Project.FachgruppenIds = string.Join(",", selectedFachgruppen);
                    projectVM.Project.TechAnforderungIds = string.Join(",", selectedTechAnforderungen);
                    projectVM.Project.IsVirtuellesKlassenzimmer = isVK ?? false; //  preset toggle
                    _unitOfWork.Project.Add(projectVM.Project);
                    _unitOfWork.Save(); // Save first to get the Id
                    existingProject = projectVM.Project;
                }
                else
                {
                    existingProject = _unitOfWork.Project.Get(u => u.Id == projectVM.Project.Id, includeProperties: "Images");

                    if (existingProject == null)
                    {
                        TempData["error"] = "Project not found.";
                        return RedirectToAction("Index", new { isVK = existingProject.IsVirtuellesKlassenzimmer });
                    }


                    // Map fields from projectVM.Project to existingProject
                    MapProjectFields(existingProject, projectVM.Project);


                    existingProject.IsVirtuellesKlassenzimmer = isVK ?? false;

                }
                // Save selected multiple-choice options
                existingProject.CategoryIds = string.Join(",", projectVM.SelectedCategories);
                existingProject.FakultaetIds = string.Join(",", projectVM.SelectedFakultaets);
                existingProject.FachgruppenIds = string.Join(",", projectVM.SelectedFachgruppen);
                existingProject.TechAnforderungIds = string.Join(",", projectVM.SelectedTechAnforderungen);

                _unitOfWork.Project.Update(existingProject); // Mark the project as updated

                string wwwRootPath = _webHostEnvironment.WebRootPath;

                // Handle image upload
                if (files != null && files.Count > 0)
                {
                    var existingImagesCount = existingProject.Images?.Count ?? 0;
                    if (existingImagesCount + files.Count > 10)
                    {
                        TempData["error"] = "You can upload a maximum of 10 images for a Project.";
                        PopulateDropdownLists(projectVM);
                        projectVM.Project.Images = existingProject.Images;
                        return View(projectVM);
                    }

                    foreach (var file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string projectPath = Path.Combine(wwwRootPath, $"images\\Projects\\project-{existingProject.Id}");

                        if (!Directory.Exists(projectPath))
                            Directory.CreateDirectory(projectPath);

                        using (var fileStream = new FileStream(Path.Combine(projectPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProjectImage projectImage = new()
                        {
                            Url = $"\\images\\Projects\\project-{existingProject.Id}\\{fileName}",
                            ProjectId = existingProject.Id,
                        };

                        existingProject.Images ??= new List<ProjectImage>();
                        existingProject.Images.Add(projectImage);
                    }
                }

                // handling url images after processing uploaded files...

                if (!string.IsNullOrEmpty(externalImageUrl))
                {

                    ProjectImage externalImage = new()
                    {
                        Url = externalImageUrl,
                        ProjectId = existingProject.Id
                    };

                    existingProject.Images ??= new List<ProjectImage>();
                    existingProject.Images.Add(externalImage);

                }

                // Handle video uploads
                if (videoFiles != null && videoFiles.Any())
                {
                    // Initialize the Videos list if null
                    existingProject.Videos ??= new List<ProjectVideo>();

                    foreach (var videoFile in videoFiles)
                    {
                        string videoFileName = Guid.NewGuid().ToString() + Path.GetExtension(videoFile.FileName);
                        string videoPath = Path.Combine(wwwRootPath, "videos\\Projects");

                        if (!Directory.Exists(videoPath))
                            Directory.CreateDirectory(videoPath);

                        using (var videoStream = new FileStream(Path.Combine(videoPath, videoFileName), FileMode.Create))
                        {
                            videoFile.CopyTo(videoStream);
                        }

                        // Create and add a new ProjectVideo record
                        var projectVideo = new ProjectVideo
                        {
                            Url = $"\\videos\\Projects\\{videoFileName}",
                            ProjectId = existingProject.Id
                        };
                        existingProject.Videos.Add(projectVideo);
                    }
                }


                // Save the project and images
                _unitOfWork.Project.Update(existingProject);
                _unitOfWork.Save();

                // Set main image only after the images are saved
                if (mainImageId.HasValue)
                {
                    var allImages = _unitOfWork.GetRepository<ProjectImage>()
                                 .GetAll()
                                 .Where(i => i.ProjectId == existingProject.Id)
                                 .ToList();

                    foreach (var image in allImages)
                    {
                        image.IsMainImage = image.Id == mainImageId.Value;
                        _unitOfWork.GetRepository<ProjectImage>().Update(image);
                    }
                    _unitOfWork.Save();
                }

                TempData["success"] = "Project saved successfully";
                return RedirectToAction("Index", new { isVK = existingProject.IsVirtuellesKlassenzimmer });
            }
            else
            {
                PopulateDropdownLists(projectVM);
                projectVM.Project.Images = _unitOfWork.GetRepository<ProjectImage>()
                                                  .GetAll()
                                                  .Where(i => i.ProjectId == projectVM.Project.Id)
                                                  .ToList();
                return View(projectVM);
            }
        }




        private void MapProjectFields(Project existingProject, Project updatedProject)
        {
            // Map all fields from updatedProject to existingProject
            existingProject.DisplayOrder = updatedProject.DisplayOrder;
            existingProject.Title = updatedProject.Title;
            existingProject.ProjectAcronym = updatedProject.ProjectAcronym;
            existingProject.KurzeBeschreibung = updatedProject.KurzeBeschreibung;
            existingProject.Status = updatedProject.Status;
            existingProject.ReleaseYear = updatedProject.ReleaseYear;
            existingProject.Version = updatedProject.Version;
            existingProject.IsVirtuellesKlassenzimmer = updatedProject.IsVirtuellesKlassenzimmer;
            existingProject.Lizenz = updatedProject.Lizenz;
            existingProject.OpenSource = updatedProject.OpenSource;
            existingProject.CreativeCommons = updatedProject.CreativeCommons;

            existingProject.Url = updatedProject.Url;
            existingProject.DokuLink = updatedProject.DokuLink;
            existingProject.DownloadURL = updatedProject.DownloadURL;

            // Removed the single video mapping as we now manage multiple videos
            // if (!string.IsNullOrEmpty(updatedProject.VideoUrl))
            // {
            //     existingProject.VideoUrl = updatedProject.VideoUrl;
            // }

            existingProject.ProjectResponsibility = updatedProject.ProjectResponsibility;
            existingProject.ProjectDevelopment = updatedProject.ProjectDevelopment;
            existingProject.ProjectLeadership = updatedProject.ProjectLeadership;
            existingProject.ProjectManagement = updatedProject.ProjectManagement;
            existingProject.ProjectPartners = updatedProject.ProjectPartners;
            existingProject.Netzwerk = updatedProject.Netzwerk;
            existingProject.DidaktischerAnsatz = updatedProject.DidaktischerAnsatz;
            existingProject.Expertise = updatedProject.Expertise;
            existingProject.ProjectCoordination = updatedProject.ProjectCoordination;
            existingProject.ProjectConception = updatedProject.ProjectConception;
            existingProject.ProjectSupport = updatedProject.ProjectSupport;
            existingProject.zusaetzlicheInformationen = updatedProject.zusaetzlicheInformationen;
            existingProject.zusaetzlicheInformationen1 = updatedProject.zusaetzlicheInformationen1;
            existingProject.Foerderung = updatedProject.Foerderung;
            existingProject.ThreeDArtist = updatedProject.ThreeDArtist;
            existingProject.DidacticDesign = updatedProject.DidacticDesign;
            existingProject.longDescription = updatedProject.longDescription;
            existingProject.Conception = updatedProject.Conception;
            existingProject.Tags = updatedProject.Tags;

            existingProject.Development = updatedProject.Development;
            existingProject.SoftwareDevelopers = updatedProject.SoftwareDevelopers;
            existingProject.Programming = updatedProject.Programming;
            existingProject.DidacticDesignTeam = updatedProject.DidacticDesignTeam;
            existingProject.Design = updatedProject.Design;
            existingProject.MediaDesign = updatedProject.MediaDesign;
            existingProject.UXDesign = updatedProject.UXDesign;
            existingProject.InteractionDesign = updatedProject.InteractionDesign;
            existingProject.SoundDesign = updatedProject.SoundDesign;

            existingProject.Didactics = updatedProject.Didactics;
            existingProject.ContentDevelopment = updatedProject.ContentDevelopment;
            existingProject.StoryDesign = updatedProject.StoryDesign;

            existingProject.Research = updatedProject.Research;
            existingProject.Evaluation = updatedProject.Evaluation;
            existingProject.EvaluationTeam = updatedProject.EvaluationTeam;
            existingProject.ResearchTeam = updatedProject.ResearchTeam;

            existingProject.PrimaryTargetGroup = updatedProject.PrimaryTargetGroup;
            existingProject.ProjectGoals = updatedProject.ProjectGoals;
            existingProject.TaxonomyLevel = updatedProject.TaxonomyLevel;
            existingProject.Methods = updatedProject.Methods;
            existingProject.Applications = updatedProject.Applications;
            existingProject.Recommendations = updatedProject.Recommendations;

            existingProject.Documents = updatedProject.Documents;
            existingProject.References = updatedProject.References;
            existingProject.Media = updatedProject.Media;
        }



        [HttpPost]
        public IActionResult ToggleStatus(int id, bool isEnabled)
        {
            var project = _unitOfWork.Project.GetFirstOrDefault(c => c.Id == id);

            if (project == null)
            {
                return Json(new { success = false, message = "Project not found." });
            }

            project.IsEnabled = isEnabled;
            _unitOfWork.Project.Update(project);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Project status updated successfully." });
        }

        private void PopulateDropdownLists(ProjectVM projectVM)
        {



            projectVM.CategoryList = _unitOfWork.GetRepository<Category>()
                                                .GetAll()
                                                .Select(c => new SelectListItem
                                                {
                                                    Text = c.Name,
                                                    Value = c.Id.ToString(),
                                                    Selected = projectVM.SelectedCategories.Contains(c.Id)
                                                });

            projectVM.FakulteatList = _unitOfWork.GetRepository<Fakultaet>()
                                                 .GetAll()
                                                 .Select(f => new SelectListItem
                                                 {
                                                     Text = f.Name,
                                                     Value = f.Id.ToString(),
                                                     Selected = projectVM.SelectedFakultaets.Contains(f.Id)
                                                 });

            projectVM.FachgruppenList = _unitOfWork.GetRepository<Fachgruppen>()
                                                   .GetAll()
                                                   .Select(fg => new SelectListItem
                                                   {
                                                       Text = fg.Name,
                                                       Value = fg.Id.ToString(),
                                                       Selected = projectVM.SelectedFachgruppen.Contains(fg.Id)
                                                   });

            projectVM.TechAnforderungList = _unitOfWork.GetRepository<TechAnforderung>()
                                                       .GetAll()
                                                       .Select(t => new SelectListItem
                                                       {
                                                           Text = t.Name,
                                                           Value = t.Id.ToString(),
                                                           Selected = projectVM.SelectedTechAnforderungen.Contains(t.Id)
                                                       });
            projectVM.ReleaseYearList = Enumerable.Range(2020, 101) // 2020 to 2120 (101 years inclusive)
                                             .OrderBy(year => year) // Sort ascending (2020, 2021, ..., 2120)
                                             .Select(year => new SelectListItem
                                             {
                                                 Text = year.ToString(),
                                                 Value = year.ToString()
                                             })
                                             .ToList();


            var projects = _unitOfWork.Project.GetAll();

            projectVM.ExistingConception = projects.Select(p => p.Conception).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingVersion = projects.Select(p => p.Version).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingLizenz = projects.Select(p => p.Lizenz).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingOpenSource = projects.Select(p => p.OpenSource).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingCreativeCommons = projects.Select(p => p.CreativeCommons).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingProjectResponsibility = projects.Select(p => p.ProjectResponsibility).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingFoerderung = projects.Select(p => p.Foerderung).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingProjectDevelopment = projects.Select(p => p.ProjectDevelopment).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingProjectLeadership = projects.Select(p => p.ProjectLeadership).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingProjectManagement = projects.Select(p => p.ProjectManagement).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingProjectPartners = projects.Select(p => p.ProjectPartners).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingNetzwerk = projects.Select(p => p.Netzwerk).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingExpertise = projects.Select(p => p.Expertise).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingProjectCoordination = projects.Select(p => p.ProjectCoordination).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingProjectConception = projects.Select(p => p.ProjectConception).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            projectVM.ExistingProjectSupport = projects.Select(p => p.ProjectSupport).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
          
            projectVM.ExistingTaxonomyLevel = projects.Select(p => p.TaxonomyLevel).Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
           
        }


        //private void ManageProjectImage(ProjectVM projectVM, IFormFile? file)
        //{
        //    string wwwRootPath = _webHostEnvironment.WebRootPath;

        //    // Handle file upload
        //    if (file != null)
        //    {
        //        string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        //        string projectPath = Path.Combine(wwwRootPath, @"images\Projects");

        //        // Delete old image if updating
        //        if (!string.IsNullOrEmpty(projectVM.Project.ImageUrl) && projectVM.Project.ImageUrl != DefaultImagePath)
        //        {
        //            string oldImagePath = Path.Combine(wwwRootPath, projectVM.Project.ImageUrl.TrimStart('/'));
        //            if (System.IO.File.Exists(oldImagePath))
        //            {
        //                System.IO.File.Delete(oldImagePath);
        //            }
        //        }

        //        // Save the new image
        //        using (var fileStream = new FileStream(Path.Combine(projectPath, fileName), FileMode.Create))
        //        {
        //            file.CopyTo(fileStream);
        //        }
        //        projectVM.Project.ImageUrl = @"\images\Projects" + fileName;
        //    }
        //    else if (projectVM.Project.Id == 0)
        //    {
        //        projectVM.Project.ImageUrl = DefaultImagePath; // Set default image if new project and no file uploaded
        //    }
        //    else
        //    {
        //        // Keep existing image for update if no new file is provided
        //        var existingProject = _unitOfWork.GetRepository<Project>().Get(u => u.Id == projectVM.Project.Id);
        //        projectVM.Project.ImageUrl = existingProject?.ImageUrl ?? DefaultImagePath;
        //    }
        //}


        #endregion

        #region Delete

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var project = _unitOfWork.GetRepository<Project>().Get(u => u.Id == id);
            if (project == null) return NotFound();

            return View(project);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = _unitOfWork.GetRepository<Project>().Get(u => u.Id == id);
            if (project == null) return NotFound();

            string wwwRootPath = _webHostEnvironment.WebRootPath;

            // Delete each individual image file
            if (project.Images != null && project.Images.Any())
            {
                foreach (var image in project.Images)
                {
                    string imagePath = Path.Combine(wwwRootPath, image.Url.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
            }

            // Delete the entire project folder (e.g., wwwroot\images\Projects\project-<id>)
            string projectFolder = Path.Combine(wwwRootPath, "images", "Projects", $"project-{project.Id}");
            if (Directory.Exists(projectFolder))
            {
                Directory.Delete(projectFolder, recursive: true);
            }

            // Remove the associated images from the database
            if (project.Images != null && project.Images.Any())
            {
                var imageRepository = _unitOfWork.GetRepository<ProjectImage>();
                foreach (var image in project.Images)
                {
                    imageRepository.Remove(image);
                }
            }

            // Remove the project record
            _unitOfWork.GetRepository<Project>().Remove(project);
            _unitOfWork.Save();

            TempData["success"] = "Project deleted successfully";
            return RedirectToAction(nameof(Index));
        }



        [HttpDelete]
        public IActionResult DeleteImage(int imageId)
        {
            var image = _unitOfWork.GetRepository<ProjectImage>().Get(i => i.Id == imageId);

            if (image == null)
            {
                return NotFound("Image not found.");
            }

            // Delete the image file from the server
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fullPath = Path.Combine(wwwRootPath, image.Url.TrimStart('/'));

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            // Remove the image from the database
            _unitOfWork.GetRepository<ProjectImage>().Remove(image);
            _unitOfWork.Save();

            return Ok("Image deleted successfully.");
        }


        [HttpDelete]
        public IActionResult DeleteVideo(int videoId)
        {
            var video = _unitOfWork.GetRepository<ProjectVideo>().Get(v => v.Id == videoId);
            if (video == null)
            {
                return NotFound("Video not found.");
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fullPath = Path.Combine(wwwRootPath, video.Url.TrimStart('/'));

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            _unitOfWork.GetRepository<ProjectVideo>().Remove(video);
            _unitOfWork.Save();

            return Ok("Video deleted successfully.");
        }




        #endregion

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll(string type)
        {
            var allProjects = _unitOfWork.GetRepository<Project>()
                .GetAll(includeProperties: "Images")
                .ToList();

            var filteredProjects = type == "vr"
                ? allProjects.Where(p => p.IsVirtuellesKlassenzimmer)
                : allProjects.Where(p => !p.IsVirtuellesKlassenzimmer);

            var result = filteredProjects
                .Select(p => new
                {
                    p.Id,
                    p.DisplayOrder,
                    p.Title,
                    p.IsEnabled,
                    MainImageUrl = p.Images != null && p.Images.Any()
                        ? (p.Images.Any(img => img.IsMainImage)
                            ? p.Images.First(img => img.IsMainImage).Url
                            : p.Images.First().Url)
                        : "/images/Projects/default.jpg"
                }).ToList();

            return Json(new { data = result });
        }






        #endregion
    }
}
