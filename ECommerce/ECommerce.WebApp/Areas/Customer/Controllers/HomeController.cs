using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Models.Models;
using ECommerce.DataAccess.Repository.IRepository;

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
        
        return View(productIncludesCategory);
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
