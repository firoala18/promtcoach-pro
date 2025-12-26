using Microsoft.AspNetCore.Mvc;
using ProjectsWebApp.DataAccsess.Repository.IRepository;
using ProjectsWebApp.Models;
using Microsoft.AspNetCore.Authorization;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class NavbarSettingController : Controller
{
    //    private readonly IUnitOfWork _unitOfWork;

    //    public NavbarSettingController(IUnitOfWork unitOfWork)
    //    {
    //        _unitOfWork = unitOfWork;
    //    }

    //    public IActionResult Edit()
    //    {
    //        var navbarSetting = _unitOfWork.GetRepository<NavbarSetting>().GetAll().FirstOrDefault();
    //        if (navbarSetting == null)
    //        {
    //            navbarSetting = new NavbarSetting();
    //            _unitOfWork.GetRepository<NavbarSetting>().Add(navbarSetting);
    //            _unitOfWork.Save();
    //        }
    //        return View(navbarSetting);
    //    }

    //    [HttpPost]
    //    [ValidateAntiForgeryToken]
    //    public IActionResult Edit(NavbarSetting navbarSetting)
    //    {
    //        if (ModelState.IsValid)
    //        {
    //            var existingSetting = _unitOfWork.GetRepository<NavbarSetting>().GetAll().FirstOrDefault();
    //            if (existingSetting != null)
    //            {
    //                existingSetting.DasPortal = navbarSetting.DasPortal;
    //                existingSetting.Projekte = navbarSetting.Projekte;
    //                existingSetting.Mitmachen = navbarSetting.Mitmachen;
    //                existingSetting.Kontakt = navbarSetting.Kontakt;

    //                _unitOfWork.GetRepository<NavbarSetting>().Update(existingSetting);
    //                _unitOfWork.Save();
    //                TempData["success"] = "Navbar settings updated successfully!";
    //            }
    //            return RedirectToAction("Edit");
    //        }
    //        return View(navbarSetting);
    //    }
}
