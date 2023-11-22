using System.ComponentModel.DataAnnotations;

namespace ECommerce.WebApp.Models;

public class Category
{
    [Key]
    public int Id { get; set; } = 3;

    [Required]
    public required string Name { get; set; }

    public required int DisplayOrder { get; set; }
}