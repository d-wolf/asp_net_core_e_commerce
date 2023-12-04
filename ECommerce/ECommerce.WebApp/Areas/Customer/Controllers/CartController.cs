using Microsoft.AspNetCore.Mvc;
using ECommerce.Models.Models;
using ECommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;
using Stripe.Checkout;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ECommerce.WebApp.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IEmailSender emailSender) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEmailSender _emailSender = emailSender;

    public IActionResult Index()
    {
        var claimsEntity = User.Identity as ClaimsIdentity;
        var userId = claimsEntity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        var shoppingCartVM = new ShoppingCartsVM()
        {
            OrderHeader = new(),
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, nameof(Product)).Select(x =>
                {
                    x.Price = GetPriceBasedQuantity(x);
                    return x;
                }
            ),
        };

        foreach (var cart in shoppingCartVM.ShoppingCartList)
        {
            shoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }

        return View(shoppingCartVM);
    }

    public IActionResult Summary()
    {
        var claimsEntity = User.Identity as ClaimsIdentity;
        var userId = claimsEntity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        var shoppingCartVM = new ShoppingCartsVM()
        {
            OrderHeader = new(),
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, nameof(Product)).Select(x =>
                {
                    x.Price = GetPriceBasedQuantity(x);
                    return x;
                }
            ),
        };

        var user = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == userId);
        if (user == null)
        {
            return NotFound();
        }

        shoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
        shoppingCartVM.OrderHeader.City = user.City;
        shoppingCartVM.OrderHeader.Name = user.Name;
        shoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
        shoppingCartVM.OrderHeader.PostalCode = user.PostalCode;
        shoppingCartVM.OrderHeader.State = user.State;

        foreach (var cart in shoppingCartVM.ShoppingCartList)
        {
            shoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }

        return View(shoppingCartVM);
    }

    [HttpPost]
    [ActionName("Summary")]
    public IActionResult SummaryPOST(ShoppingCartsVM shoppingCartsVM)
    {
        var claimsEntity = User.Identity as ClaimsIdentity;
        var userId = claimsEntity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        shoppingCartsVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, nameof(Product)).Select(x =>
               {
                   x.Price = GetPriceBasedQuantity(x);
                   return x;
               });

        shoppingCartsVM.OrderHeader.OrderDate = DateTime.UtcNow;
        shoppingCartsVM.OrderHeader.ApplicationUserId = userId;

        var applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == userId);
        if (applicationUser == null)
        {
            return NotFound();
        }

        shoppingCartsVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
        shoppingCartsVM.OrderHeader.City = applicationUser.City;
        shoppingCartsVM.OrderHeader.Name = applicationUser.Name;
        shoppingCartsVM.OrderHeader.StreetAddress = applicationUser.StreetAddress;
        shoppingCartsVM.OrderHeader.PostalCode = applicationUser.PostalCode;
        shoppingCartsVM.OrderHeader.State = applicationUser.State;

        foreach (var cart in shoppingCartsVM.ShoppingCartList)
        {
            shoppingCartsVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }

        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            // regular customer account
            shoppingCartsVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            shoppingCartsVM.OrderHeader.OrderStatus = SD.StatusPending;
        }
        else
        {
            // it's a company user
            shoppingCartsVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            shoppingCartsVM.OrderHeader.OrderStatus = SD.StatusApproved;
        }

        _unitOfWork.OrderHeader.Add(shoppingCartsVM.OrderHeader);
        // we need to save here becasue of foreign key violation
        _unitOfWork.Save();
        foreach (var cart in shoppingCartsVM.ShoppingCartList)
        {
            var detail = new OrderDetail
            {
                ProductId = cart.ProductId,
                OrderHeaderId = shoppingCartsVM.OrderHeader.Id,
                Price = cart.Price,
                Count = cart.Count,
            };
            _unitOfWork.OrderDetail.Add(detail);
        }

        _unitOfWork.Save();

        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            List<SessionLineItemOptions> lineItems = [];

            foreach (var item in shoppingCartsVM.ShoppingCartList)
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
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={shoppingCartsVM.OrderHeader.Id}",
                CancelUrl = domain + "customer/cart/index",
                LineItems = lineItems,
                Mode = "payment",
            };

            var service = new SessionService();
            var session = service.Create(options);
            _unitOfWork.OrderHeader.UpdatePaymentId(shoppingCartsVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _unitOfWork.Save();
            Response.Headers.Append("Location", session.Url);
            return new StatusCodeResult(303);
        }

        return RedirectToAction(nameof(OrderConfirmation), new
        {
            id = shoppingCartsVM.OrderHeader.Id,
        });
    }

    public IActionResult OrderConfirmation(int? id)
    {
        var orderHeader = _unitOfWork.OrderHeader.GetFirstOrDefault(x => x.Id == id, includeProperties: nameof(ApplicationUser));
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
                _unitOfWork.OrderHeader.UpdateStatus(orderHeader.Id, SD.StatusApproved, SD.PaymentStatusApproved);
                _unitOfWork.Save();
            }

            HttpContext.Session.Clear();
        }

        _emailSender.SendEmailAsync(orderHeader.ApplicationUser!.Email!, "New Order E-Commerce",
         $"<p>New Order Created - {orderHeader.Id}</p>");

        var shoppingCarts = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId);
        _unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);
        _unitOfWork.Save();

        return View(id);
    }

    private static double GetPriceBasedQuantity(ShoppingCart shoppingCart)
    {
        if (shoppingCart.Product == null) return 0.0;

        if (shoppingCart.Count <= 50)
        {
            return shoppingCart.Product.Price;
        }
        else if (shoppingCart.Count <= 100)
        {
            return shoppingCart.Product.Price50;
        }

        return shoppingCart.Product.Price100;
    }

    public IActionResult Plus(int? id)
    {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == id, nameof(Product));
        if (cart == null)
        {
            return NotFound();
        }

        cart!.Count += 1;
        _unitOfWork.ShoppingCart.Update(cart);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Minus(int? id)
    {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == id, nameof(Product));
        if (cart == null)
        {
            return NotFound();
        }

        if (cart.Count > 1)
        {
            cart!.Count -= 1;
            _unitOfWork.ShoppingCart.Update(cart);
            _unitOfWork.Save();
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Remove(int? id)
    {
        var cart = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.Id == id, nameof(Product));
        if (cart == null)
        {
            return NotFound();
        }

        HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).Count() - 1);
        _unitOfWork.ShoppingCart.Remove(cart);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }
}
