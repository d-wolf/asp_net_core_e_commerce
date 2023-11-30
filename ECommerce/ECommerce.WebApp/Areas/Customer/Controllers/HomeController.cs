using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Models.Models;
using ECommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ECommerce.WebApp.Areas.Customer.Controllers;

[Area("Customer")]
public class HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork) : Controller
{
    private readonly ILogger<HomeController> _logger = logger;
    readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IActionResult Index()
    {
        var productsIncludesCategory = _unitOfWork.Product.GetAll("Category");
        return View(productsIncludesCategory);
    }

    public IActionResult Details(int? id)
    {
        if (id == null || id == 0)
        {
            return BadRequest();
        }

        var productIncludesCategory = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id, "Category");
        if (productIncludesCategory == null)
        {
            return NotFound();
        }

        return View(new ShoppingCart
        {
            Product = productIncludesCategory,
            ProductId = id,
            Count = 1,
        });
    }

    [HttpPost]
    [Authorize]
    public IActionResult Details(ShoppingCart shoppingCart)
    {
        var claimsEntity = User.Identity as ClaimsIdentity;
        var userId = claimsEntity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        shoppingCart.ApplicationUserId = userId;

        var cartForUserAndProduct = _unitOfWork.ShoppingCart.GetFirstOrDefault(x => x.ApplicationUserId == userId && x.ProductId == shoppingCart.ProductId);

        if (cartForUserAndProduct != null)
        {
            cartForUserAndProduct.Count += shoppingCart.Count;
            _unitOfWork.ShoppingCart.Update(cartForUserAndProduct);
            TempData["success"] = "Cart updated successfully";
        }
        else
        {
            _unitOfWork.ShoppingCart.Add(shoppingCart);

        }

        _unitOfWork.Save();
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
