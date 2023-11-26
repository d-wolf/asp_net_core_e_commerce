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
        var productsIncludeCategory = _unitOfWork.Product.GetAll("Category");
        return View(productsIncludeCategory);
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
