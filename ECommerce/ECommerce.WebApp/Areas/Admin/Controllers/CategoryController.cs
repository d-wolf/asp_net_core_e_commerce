using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
public class CategoryController(IUnitOfWork unitOfWork) : Controller
{
    readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IActionResult Index()
    {
        var categoryList = _unitOfWork.Category.GetAll().ToList();
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
            _unitOfWork.Category.Add(category);
            _unitOfWork.Save();
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

        var category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);

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
            _unitOfWork.Category.Update(category);
            _unitOfWork.Save();
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

        var category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);

        if (category == null)
        {
            return NotFound();
        }

        return View(category);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var category = _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
        if (category == null)
        {
            return NotFound();
        }

        _unitOfWork.Category.Remove(category);
        _unitOfWork.Save();
        TempData["success"] = "Category deleted successfully";
        return RedirectToAction("Index", "Category");
    }
}