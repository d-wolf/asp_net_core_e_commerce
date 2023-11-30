using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ECommerce.Models.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Models.ViewModels;

public class ShoppingCartVM
{
    public required double OrderTotal { get; set; }
    public required IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
}