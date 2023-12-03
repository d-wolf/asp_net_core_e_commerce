using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using ECommerce.Models.ViewModels;
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

    public IActionResult Details(int? id)
    {
        var header = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id, includeProperties: nameof(ApplicationUser));
        var details = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == id, includeProperties: nameof(Product)) ?? Enumerable.Empty<OrderDetail>();

        if (header == null)
        {
            return NotFound();
        }

        var orderVM = new OrderVM
        {
            OrderHeader = header,
            OrderDetails = details ?? Enumerable.Empty<OrderDetail>(),
        };

        return View(orderVM);
    }

    #region API Calls

    [HttpGet]
    public IActionResult GetAll(string? status)
    {
        var orderList = _unitOfWork.OrderHeader.GetAll(includeProperties: nameof(ApplicationUser));

        switch (status)
        {
            case "pending":
                orderList = orderList.Where(x => x.PaymentStatus == SD.PaymentStatusDelayedPayment);
                break;
            case "inprocess":
                orderList = orderList.Where(x => x.OrderStatus == SD.StatusProcessing);
                break;
            case "completed":
                orderList = orderList.Where(x => x.OrderStatus == SD.StatusShipped).ToList();
                break;
            case "approved":
                orderList = orderList.Where(x => x.OrderStatus == SD.StatusApproved).ToList();
                break;
            default:
                break;
        }

        return Json(new { data = orderList });
    }
    #endregion
}