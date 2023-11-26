using ECommerce.DataAccess.Repository.IRepository;
using ECommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.WebApp.Areas.Admin.Controllers;

[Area("Admin")]
public class ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) : Controller
{
    readonly IUnitOfWork _unitOfWork = unitOfWork;
    readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;

    public IActionResult Index()
    {
        var productList = _unitOfWork.Product.GetAll("Category").ToList();
        return View(productList);
    }

    public IActionResult Upsert(int? id)
    {
        if (id != null && id != 0)
        {
            var product = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
            if (product == null) return NotFound();
            return View(new ProductVM()
            {
                Product = product,
                CategoryList = _unitOfWork.Category.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString(),
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
    public IActionResult Upsert(ProductVM productVM, IFormFile? file)
    {
        if (!ModelState.IsValid) return View();


        if (file != null)
        {
            if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
            {
                var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, productVM.Product.ImageUrl.TrimStart(Path.DirectorySeparatorChar));
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string path = Path.Combine("images", "product", fileName);
            using var fileStream = new FileStream(Path.Combine(_webHostEnvironment.WebRootPath, path), FileMode.Create);
            file.CopyTo(fileStream);
            productVM.Product.ImageUrl = Path.DirectorySeparatorChar + path;
        }

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
        return RedirectToAction("Index", "Product");
    }

    #region API Calls

    [HttpGet]
    public IActionResult GetAll()
    {
        var productList = _unitOfWork.Product.GetAll("Category").ToList();
        return Json(new { data = productList });
    }

    public IActionResult Delete(int? id)
    {
        var productToDelete = _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
        if (productToDelete == null)
        {
            return Json(new { success = false, message = "Error while deleting" });
        }

        if (!string.IsNullOrEmpty(productToDelete.ImageUrl))
        {
            var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, productToDelete.ImageUrl.TrimStart(Path.DirectorySeparatorChar));
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
        }

        _unitOfWork.Product.Remove(productToDelete);
        _unitOfWork.Save();
        return Json(new { success = true, message = "Successfully deleted" });
    }

    #endregion
}