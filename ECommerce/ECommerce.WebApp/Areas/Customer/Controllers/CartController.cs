using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Models.Models;
using ECommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ECommerce.Models.ViewModels;

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
            OrderTotal = 0,
            ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId, nameof(Product)).Select(x =>
                 new ShoppingCart
                 {
                     ApplicationUserId = x.ApplicationUserId,
                     ApplicationUser = x.ApplicationUser,
                     Count = x.Count,
                     Price = GetPriceBasedQuantity(x),
                     Product = x.Product,
                     Id = x.Id,
                     ProductId = x.ProductId,
                 }
            ),
        };


        foreach (var cart in shoppingCartVM.ShoppingCartList)
        {
            shoppingCartVM.OrderTotal += cart.Price * cart.Count;
        }

        return View(shoppingCartVM);
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
}
