using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics;


namespace ECommerce.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.RoleAdmin)]
public class UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<IdentityUser> _userManager = userManager;

    public IActionResult Index()
    {
        return View();
    }

    #region API Calls

    [HttpGet]
    public IActionResult GetAll()
    {
        var userList = _unitOfWork.ApplicationUser.GetAll(includeProperties: nameof(Company)).ToList();


        foreach (var item in userList)
        {
            var user = _userManager.FindByIdAsync(item.Id).GetAwaiter().GetResult();
            var roles = _userManager.GetRolesAsync(user!).GetAwaiter().GetResult();
            // TODO: create viewmodel with role?
            Debug.WriteLine("");
        }

        return Json(new { data = userList });
    }

    public IActionResult Delete(string? id)
    {
        var userToDelete = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == id);
        if (userToDelete == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        _unitOfWork.ApplicationUser.Remove(userToDelete);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Successfully deleted" });
    }

    #endregion
}