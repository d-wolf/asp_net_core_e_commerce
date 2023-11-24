using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController(IUnitOfWork unitOfWork) : Controller
{
    readonly IUnitOfWork _unitOfWork = unitOfWork;

    public IActionResult Index()
    {
        var productList = _unitOfWork.Product.GetAll().ToList();
        return View(productList);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Create(Product product)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Product.Add(product);
            _unitOfWork.Save();
            TempData["success"] = "Product created successfully";
            return RedirectToAction("Index", "Product");
        }

        return View();
    }

    public IActionResult Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost]
    public IActionResult Edit(Product product)
    {
        if (ModelState.IsValid)
        {
            _unitOfWork.Product.Update(product);
            _unitOfWork.Save();
            TempData["success"] = "Product updated successfully";
            return RedirectToAction("Index", "Product");
        }

        return View();
    }

    public IActionResult Delete(int? id)
    {
        if (id == null || id == 0)
        {
            return NotFound();
        }

        var product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }

    [HttpPost, ActionName("Delete")]
    public IActionResult DeletePost(int? id)
    {
        var product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        _unitOfWork.Product.Remove(product);
        _unitOfWork.Save();
        TempData["success"] = "Product deleted successfully";
        return RedirectToAction("Index", "Product");
    }
}