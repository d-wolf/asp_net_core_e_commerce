using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ECommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.RoleAdmin)]
public class UserController(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : Controller
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult RoleManagement(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == id, includeProperties: nameof(Company));
        var roles = _userManager.GetRolesAsync(applicationUser!).GetAwaiter().GetResult();

        return View(new RoleManagementVM
        {
            ApplicationUser = applicationUser!,
            CurrentRole = roles.First(),
            RoleList = _roleManager.Roles.Select(x => x.Name).Select(x => new SelectListItem
            {
                Text = x,
                Value = x,
            }),
            CompanyList = _unitOfWork.Company.GetAll().Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString(),
            }),
        });
    }

    #region API Calls

    [HttpGet]
    public IActionResult GetAll()
    {
        var userList = new List<UserVM>();

        foreach (var item in _unitOfWork.ApplicationUser.GetAll(includeProperties: nameof(Company)).ToList())
        {
            var user = _userManager.FindByIdAsync(item.Id).GetAwaiter().GetResult();
            var roles = _userManager.GetRolesAsync(user!).GetAwaiter().GetResult();
            userList.Add(new UserVM
            {
                ApplicationUser = item,
                Role = roles.First(),
            });
        }

        return Json(new { data = userList });
    }

    [HttpPost]
    public IActionResult LockUnlock([FromBody] string id)
    {
        var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == id);
        if (user == null)
        {
            return Json(new { success = false, message = "Error while locking/unlocking" });
        }

        if (user.LockoutEnd != null && user.LockoutEnd > DateTime.UtcNow)
        {
            // user currently locked
            user.LockoutEnd = DateTime.UtcNow;
        }
        else
        {
            user.LockoutEnd = DateTime.UtcNow.AddYears(1);
        }

        _unitOfWork.ApplicationUser.Update(user);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Locking/Unlocking successful" });
    }

    #endregion
}