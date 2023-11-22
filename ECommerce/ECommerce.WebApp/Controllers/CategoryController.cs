using ECommerce.WebApp.Data;
using ECommerce.WebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApp.Controllers;

public class CategoryController : Controller
{

    readonly ApplicationDbContext _context;

    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }


    public IActionResult Index()
    {
        var categoryList = _context.Categories.ToList();
        return View(categoryList);
    }

    public IActionResult Create()
    {
        return View();
    }
}