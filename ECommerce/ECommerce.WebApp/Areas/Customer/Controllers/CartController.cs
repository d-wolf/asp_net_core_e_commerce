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

     public IActionResult Summary()
    {
        return View();
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
