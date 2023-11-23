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

    [HttpPost]
    public IActionResult Create(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Add(category);
            _context.SaveChanges();
            TempData["success"] = "Category created successfully";
            return RedirectToAction("Index", "Category");
        }

        return View();
    }

    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var category = _context.Categories.SingleOrDefault(x => x.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost]
    public IActionResult Edit(Category category)
    {
        if (ModelState.IsValid)
        {
            _context.Update(category);
            _context.SaveChanges();
            TempData["success"] = "Category updated successfully";
            return RedirectToAction("Index", "Category");
        }

        return View();
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var category = _context.Categories.SingleOrDefault(x => x.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var category = _context.Categories.SingleOrDefault(x => x.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        _context.Remove(category);
        _context.SaveChanges();
        TempData["success"] = "Category deleted successfully";
        return RedirectToAction("Index", "Category");
    }
}