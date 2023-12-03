using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.RoleAdmin)]
public class OrderController(IUnitOfWork unitOfWork) : Controller
{
    readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IActionResult Index()
    {
        return View();
    }

    #region API Calls

    [HttpGet]
    public IActionResult GetAll()
    {
        var orderList = _unitOfWork.OrderHeader.GetAll(includeProperties: nameof(ApplicationUser)).ToList();
        return Json(new { data = orderList });
    }
    #endregion
}