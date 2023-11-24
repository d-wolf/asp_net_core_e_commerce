using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApp.Controllers;

public class CategoryController(ICategoryRepository categoryRepository) : Controller
{
    readonly ICategoryRepository _categoryRepository = categoryRepository;

    public IActionResult Index()
    {
        var categoryList = _categoryRepository.GetAll().ToList();
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
            _categoryRepository.Add(category);
            _categoryRepository.Save();
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

        var category = _categoryRepository.GetFirstOrDefault(x => x.Id == id);

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
            _categoryRepository.Update(category);
            _categoryRepository.Save();
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

        var category = _categoryRepository.GetFirstOrDefault(x => x.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var category = _categoryRepository.GetFirstOrDefault(x => x.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        _categoryRepository.Remove(category);
        _categoryRepository.Save();
        TempData["success"] = "Category deleted successfully";
        return RedirectToAction("Index", "Category");
    }
}