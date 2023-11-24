using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.Models;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Title { get; set; }

    [Required]
    public required string Description { get; set; }

    [Required]
    public required string ISBN { get; set; }

    [Required]
    public required string Author { get; set; }

    [Required]
    [DisplayName("List Price")]
    [Range(1, 1000)]
    public required double ListPrice { get; set; }

    [Required]
    [DisplayName("Price for 1-50")]
    [Range(1, 1000)]
    public required double Price { get; set; }

    [Required]
    [DisplayName("Price for 50+")]
    [Range(1, 1000)]
    public required double Price50 { get; set; }

    [Required]
    [DisplayName("Price for 100+")]
    [Range(1, 1000)]
    public required double Price100 { get; set; }
}