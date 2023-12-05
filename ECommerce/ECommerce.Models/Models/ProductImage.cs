using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ECommerce.Models.Models;

public class ProductImage
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string ImageUrl { get; set; }

    [ForeignKey("Product")]
    [DisplayName("Product")]
    public int? ProductId { get; set; }

    [ValidateNever]
    public Product? Product { get; set; }
}