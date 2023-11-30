using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ECommerce.Models.Models;

public class ShoppingCart
{
    [Key]
    public int Id { get; set; }

    [Range(1, 1000, ErrorMessage = "Please enter a value between 1 and 1000")]
    public int Count { get; set; }

    [ForeignKey("Product")]
    [DisplayName("Product")]
    public int? ProductId { get; set; }

    [ValidateNever]
    public Product? Product { get; set; }

    [ForeignKey("ApplicationUser")]
    [DisplayName("Application User")]
    public string? ApplicationUserId { get; set; }

    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; }

    [NotMapped]
    public double Price { get; set; }
}