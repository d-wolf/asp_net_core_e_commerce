using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ECommerce.Models.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Models.ViewModels;

public class ShoppingCartsVM
{
    public required OrderHeader OrderHeader { get; set; }
    public required IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
}