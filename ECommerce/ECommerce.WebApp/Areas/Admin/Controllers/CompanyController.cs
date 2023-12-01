using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.RoleAdmin)]
public class CompanyController(IUnitOfWork unitOfWork) : Controller
{
    readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IActionResult Index()
    {
        var companyList = _unitOfWork.Company.GetAll().ToList();
        return View(companyList);
    }

    public IActionResult Upsert(int? id)
    {
        if (id != null && id != 0)
        {
            var company = _unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);
            if (company == null) return NotFound();
            return View(company);
        }
        else
        {
            return View(new Company());
        }

    }

    [HttpPost]
    public IActionResult Upsert(Company company, IFormFile? file)
    {
        if (!ModelState.IsValid) return View(company);

        if (company.Id != 0)
        {
            _unitOfWork.Company.Update(company);
            TempData["success"] = "Company updated successfully";
        }
        else
        {
            _unitOfWork.Company.Add(company);
            TempData["success"] = "Company created successfully";
        }

        _unitOfWork.Save();
        return RedirectToAction(nameof(Index), nameof(Company));
    }

    #region API Calls

    [HttpGet]
    public IActionResult GetAll()
    {
        var companyList = _unitOfWork.Company.GetAll().ToList();
        return Json(new { data = companyList });
    }

    public IActionResult Delete(int? id)
    {
        var companyToDelete = _unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);
        if (companyToDelete == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        _unitOfWork.Company.Remove(companyToDelete);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Successfully deleted" });
    }

    #endregion
}