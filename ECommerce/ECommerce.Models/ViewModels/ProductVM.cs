using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ECommerce.Models.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Models.ViewModels;

public class ProductVM
{
    public required Product Product { get; set; }
    [ValidateNever]
    public required IEnumerable<SelectListItem> CategoryList { get; set; }
}