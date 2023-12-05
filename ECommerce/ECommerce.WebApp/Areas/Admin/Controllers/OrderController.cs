using System.Security.Claims;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

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

    [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]
    [HttpPost]
    public IActionResult UpdateOrderDetail(OrderVM orderVM)
    {
        var headerFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == orderVM.OrderHeader.Id);
        if (headerFromDb == null)
        {
            return NotFound();
        }

        headerFromDb.Name = orderVM.OrderHeader.Name;
        headerFromDb.PhoneNumber = orderVM.OrderHeader.PhoneNumber;
        headerFromDb.StreetAddress = orderVM.OrderHeader.StreetAddress;
        headerFromDb.City = orderVM.OrderHeader.City;
        headerFromDb.State = orderVM.OrderHeader.State;
        headerFromDb.PostalCode = orderVM.OrderHeader.PostalCode;

        if (!string.IsNullOrEmpty(orderVM.OrderHeader.Carrier))
        {
            headerFromDb.Carrier = orderVM.OrderHeader.Carrier;
        }
        if (!string.IsNullOrEmpty(orderVM.OrderHeader.TrackingNumber))
        {
            headerFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
        }

        _unitOfWork.OrderHeader.Update(headerFromDb);
        _unitOfWork.Save();

        TempData["success"] = "Order updated successfully";

        return RedirectToAction(nameof(Details), new { id = headerFromDb.Id });
    }

    [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]
    [HttpPost]
    public IActionResult StartProcessing(OrderVM orderVM)
    {
        _unitOfWork.OrderHeader.UpdateStatus(orderVM.OrderHeader.Id, SD.StatusProcessing);
        _unitOfWork.Save();
        TempData["success"] = "Order updated successfully";
        return RedirectToAction(nameof(Details), new { id = orderVM.OrderHeader.Id });
    }

    [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]
    [HttpPost]
    public IActionResult ShipOrder(OrderVM orderVM)
    {
        var headerFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == orderVM.OrderHeader.Id);
        if (headerFromDb == null)
        {
            return NotFound();
        }

        headerFromDb.TrackingNumber = orderVM.OrderHeader.TrackingNumber;
        headerFromDb.Carrier = orderVM.OrderHeader.Carrier;
        headerFromDb.OrderStatus = SD.StatusShipped;
        headerFromDb.ShippingDate = DateTime.UtcNow;

        if (orderVM.OrderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
        {
            headerFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        }

        _unitOfWork.OrderHeader.Update(headerFromDb);
        _unitOfWork.Save();
        TempData["success"] = "Order shipped successfully";
        return RedirectToAction(nameof(Details), new { id = orderVM.OrderHeader.Id });
    }

    [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]
    [HttpPost]
    public IActionResult CancelOrder(OrderVM orderVM)
    {
        var headerFromDb = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == orderVM.OrderHeader.Id);
        if (headerFromDb == null)
        {
            return NotFound();
        }

        if (headerFromDb.PaymentStatus == SD.PaymentStatusApproved)
        {
            var options = new Stripe.RefundCreateOptions
            {
                Reason = Stripe.RefundReasons.RequestedByCustomer,
                PaymentIntent = headerFromDb.PaymentIntentId,
            };

            var service = new Stripe.RefundService();
            Stripe.Refund refund = service.Create(options);
            _unitOfWork.OrderHeader.UpdateStatus(headerFromDb.Id, SD.StatusCancelled, SD.StatusRefunded);
        }
        else
        {
            _unitOfWork.OrderHeader.UpdateStatus(headerFromDb.Id, SD.StatusCancelled, SD.StatusCancelled);
        }

        _unitOfWork.Save();
        TempData["success"] = "Order cancelled successfully";
        return RedirectToAction(nameof(Details), new { id = orderVM.OrderHeader.Id });
    }

    [HttpPost]
    public IActionResult DetailsPayNow(OrderVM orderVM)
    {
        var header = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == orderVM.OrderHeader.Id, includeProperties: nameof(ApplicationUser));
        var details = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == orderVM.OrderHeader.Id, includeProperties: nameof(Product)) ?? Enumerable.Empty<OrderDetail>();

        if (header == null)
        {
            return NotFound();
        }

        List<SessionLineItemOptions> lineItems = [];

        foreach (var item in orderVM.OrderDetails)
        {
            lineItems.Add(new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmount = (long)(item.Price * 100), // $20.50 -> 2050
                    Currency = "usd",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product!.Title
                    }
                },
                Quantity = item.Count
            });
        }

        var domain = Request.Scheme + "://" + Request.Host.Value + "/";
        var options = new SessionCreateOptions
        {
            SuccessUrl = domain + $"admin/order/PaymentConfirmation?id={orderVM.OrderHeader.Id}",
            CancelUrl = domain + $"admin/order/details?id={orderVM.OrderHeader.Id}",
            LineItems = lineItems,
            Mode = "payment",
        };

        var service = new SessionService();
        var session = service.Create(options);
        _unitOfWork.OrderHeader.UpdatePaymentId(orderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
        _unitOfWork.Save();
        Response.Headers.Append("Location", session.Url);
        return new StatusCodeResult(303);
    }

    public IActionResult PaymentConfirmation(int? id)
    {
        var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id);
        if (orderHeader == null)
        {
            return NotFound();
        }

        if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
        {
            var service = new SessionService();
            var session = service.Get(orderHeader.SessionId);
            if (session.PaymentStatus.Equals("paid", StringComparison.CurrentCultureIgnoreCase))
            {
                _unitOfWork.OrderHeader.UpdatePaymentId(orderHeader.Id, session.Id, session.PaymentIntentId);
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, orderHeader.OrderStatus!, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }
        }

        return View(id);
    }

    #region API Calls

    [HttpGet]
    public IActionResult GetAll(string? status)
    {
        List<OrderHeader> orderList;

        if (User.IsInRole(SD.RoleAdmin) || User.IsInRole(SD.RoleEmployee))
        {
            orderList = _unitOfWork.OrderHeader.GetAll(includeProperties: nameof(ApplicationUser)).ToList();
        }
        else
        {
            var claimsEntity = User.Identity as ClaimsIdentity;
            var userId = claimsEntity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            orderList = _unitOfWork.OrderHeader.GetAll(x => x.ApplicationUserId == userId, includeProperties: nameof(ApplicationUser)).ToList();
        }

        switch (status)
        {
            case "pending":
                orderList = orderList.Where(x => x.PaymentStatus == SD.PaymentStatusDelayedPayment).ToList();
                break;
            case "inprocess":
                orderList = orderList.Where(x => x.OrderStatus == SD.StatusProcessing).ToList();
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