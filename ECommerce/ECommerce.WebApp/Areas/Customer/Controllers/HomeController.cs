using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using ECommerce.Utility;
using ECommerce.Models.Models;
using ECommerce.Models.ViewModels;

namespace ECommerce.WebApp.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IActionResult Index()
    {
        var claimsEntity = User.Identity as ClaimsIdentity;
        var userId = claimsEntity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId != null)
        {
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId).Count());
        }

        var productsIncludesCategory = _unitOfWork.Product.GetAll(includeProperties: nameof(Category));
        return View(productsIncludesCategory);
    }

    public IActionResult Details(int? id)
    {
        if (id == null || id == 0)
        {
            return BadRequest();
        }

        var productIncludesCategory = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id, nameof(Category));
        if (productIncludesCategory == null)
        {
            return NotFound();
        }

        return View(new ShoppingCartVM
        {
            ShoppingCart = new ShoppingCart
            {
                Product = productIncludesCategory,
                ProductId = id,
                Count = 1,
            }
        });
    }

    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCartVM shoppingCartVM)
    {
        var claimsEntity = User.Identity as ClaimsIdentity;
        var userId = claimsEntity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        shoppingCartVM.ShoppingCart.ApplicationUserId = userId;
        var cartForUserAndProduct = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.ApplicationUserId == userId && x.ProductId == shoppingCartVM.ShoppingCart.ProductId);

        if (cartForUserAndProduct != null)
        {
            cartForUserAndProduct.Count += shoppingCartVM.ShoppingCart.Count;
            _unitOfWork.ShoppingCart.Update(cartForUserAndProduct);
            _unitOfWork.Save();
            TempData["success"] = "Cart updated successfully";
        }
        else
        {
            _unitOfWork.ShoppingCart.Add(shoppingCartVM.ShoppingCart);
            _unitOfWork.Save();
            var cart = _unitOfWork.ShoppingCart.GetAll(x => x.ApplicationUserId == userId);
            HttpContext.Session.SetInt32(SD.SessionCart, cart?.Count() ?? 0);
        }

        return RedirectToAction(nameof(Index));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
