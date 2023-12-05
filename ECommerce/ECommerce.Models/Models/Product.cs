using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ECommerce.Models.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string? Title { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public string? ISBN { get; set; }

    [Required]
    public string? Author { get; set; }

    [Required]
    [DisplayName("List Price")]
    [Range(1, 1000)]
    public double ListPrice { get; set; }

    [Required]
    [DisplayName("Price for 1-50")]
    [Range(1, 1000)]
    public double Price { get; set; }

    [Required]
    [DisplayName("Price for 50+")]
    [Range(1, 1000)]
    public double Price50 { get; set; }

    [Required]
    [DisplayName("Price for 100+")]
    [Range(1, 1000)]
    public double Price100 { get; set; }

    [ForeignKey("Category")]
    [DisplayName("Category")]
    public int? CategoryId { get; set; }

    [ValidateNever]
    public Category? Category { get; set; }

    [DisplayName("Images")]
    [ValidateNever]
    public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}