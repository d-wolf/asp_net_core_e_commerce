using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = SD.RoleAdmin)]
public class ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) : Controller
{
    readonly IUnitOfWork _unitOfWork = unitOfWork;
    readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    public IActionResult Index()
    {
        var productList = _unitOfWork.Product.GetAll(includeProperties: nameof(Category)).ToList();
        return View(productList);
    }

    public IActionResult Upsert(int? id)
    {
        if (id != null && id != 0)
        {
            var product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id, includeProperties: "ProductImages");
            if (product == null) return NotFound();
            return View(new ProductVM()
            {
                Product = product,
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                }),
            });
        }
        else
        {
            return View(new ProductVM()
            {
                Product = new(),
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
                }),
            });
        }
    }

    [HttpPost]
    public IActionResult Upsert(ProductVM productVM, List<IFormFile>? files)
    {
        if (!ModelState.IsValid) return View(productVM);

        if (productVM.Product.Id != 0)
        {
            _unitOfWork.Product.Update(productVM.Product);
            TempData["success"] = "Product updated successfully";
        }
        else
        {
            _unitOfWork.Product.Add(productVM.Product);
            TempData["success"] = "Product created successfully";
        }

        _unitOfWork.Save();

        if (files != null)
        {
            foreach (var f in files)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(f.FileName);
                string path = Path.Combine("images", "product", $"product-{productVM.Product.Id}", fileName);
                string wwwwrootpath = Path.Combine(_webHostEnvironment.WebRootPath, path);
                new FileInfo(wwwwrootpath).Directory?.Create();
                using var fileStream = new FileStream(wwwwrootpath, FileMode.Create);
                f.CopyTo(fileStream);
                var productImage = new ProductImage
                {
                    ProductId = productVM.Product.Id,
                    ImageUrl = Path.DirectorySeparatorChar + path,
                };

                _unitOfWork.ProductImage.Add(productImage);
            }

            _unitOfWork.Save();
        }


        return RedirectToAction(nameof(Index), nameof(Product));
    }

    #region API Calls

    [HttpGet]
    public IActionResult GetAll()
    {
        var productList = _unitOfWork.Product.GetAll(includeProperties: nameof(Category)).ToList();
        return Json(new { data = productList });
    }

    [HttpDelete]
    public IActionResult Delete(int? id)
    {
        var productToDelete = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
        if (productToDelete == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        // if (!string.IsNullOrEmpty(productToDelete.ImageUrl))
        // {
        //     var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, productToDelete.ImageUrl.TrimStart(Path.DirectorySeparatorChar));
        //     if (System.IO.File.Exists(oldPath))
        //     {
        //         System.IO.File.Delete(oldPath);
        //     }
        // }

        _unitOfWork.Product.Remove(productToDelete);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Successfully deleted" });
    }

    #endregion
}