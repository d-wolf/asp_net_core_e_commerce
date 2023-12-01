using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Models.Models;
using ECommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;

namespace ECommerce.WebApp.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController(ILogger<HomeController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;

    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IActionResult Index()
    {
        var claimsEntity = User.Identity as ClaimsIdentity;
        var userId = claimsEntity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        var shoppingCartVM = new ShoppingCartVM()
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

        var shoppingCartVM = new ShoppingCartVM()
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
    public IActionResult SummaryPOST(ShoppingCartVM shoppingCartVM)
    {
        var claimsEntity = User.Identity as ClaimsIdentity;
        var userId = claimsEntity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        shoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, nameof(Product)).Select(x =>
               {
                   x.Price = GetPriceBasedQuantity(x);
                   return x;
               });

        shoppingCartVM.OrderHeader.OrderDate = DateTime.UtcNow;
        shoppingCartVM.OrderHeader.ApplicationUserId = userId;

        var applicationUser = _unitOfWork.ApplicationUser.GetFirstOrDefault(x => x.Id == userId);
        if (applicationUser == null)
        {
            return NotFound();
        }

        shoppingCartVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
        shoppingCartVM.OrderHeader.City = applicationUser.City;
        shoppingCartVM.OrderHeader.Name = applicationUser.Name;
        shoppingCartVM.OrderHeader.StreetAddress = applicationUser.StreetAddress;
        shoppingCartVM.OrderHeader.PostalCode = applicationUser.PostalCode;
        shoppingCartVM.OrderHeader.State = applicationUser.State;

        foreach (var cart in shoppingCartVM.ShoppingCartList)
        {
            shoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
        }

        if (applicationUser.CompanyId.GetValueOrDefault() == 0)
        {
            // regular customer account
            shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
        }
        else
        {
            // it's a company user
            shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
            shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
        }

        _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
        // we need to save here becasue of foreign key violation
        _unitOfWork.Save();
        foreach (var cart in shoppingCartVM.ShoppingCartList)
        {
            var detail = new OrderDetail
            {
                ProductId = cart.ProductId,
                OrderHeaderId = shoppingCartVM.OrderHeader.Id,
                Price = cart.Price,
                Count = cart.Count,
            };
            _unitOfWork.OrderDetail.Add(detail);
        }

        _unitOfWork.Save();

        if(applicationUser.CompanyId.GetValueOrDefault() == 0){
            // regular customer
            // stripe logic here
        }

        return RedirectToAction(nameof(OrderConfirmation), new
        {
            id = shoppingCartVM.OrderHeader.Id,
        });
    }

    public IActionResult OrderConfirmation(int? id)
    {
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

        _unitOfWork.ShoppingCart.Remove(cart);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Index));
    }
}
