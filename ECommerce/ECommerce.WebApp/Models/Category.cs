using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.WebApp.Models;

public class Category
{
    [Key]
    public int Id { get; set; }

    [Required]
    [DisplayName("Name")]
    [MaxLength(30)]
    public required string Name { get; set; }

    [DisplayName("Display Order")]
    [Range(1, 100, ErrorMessage = "Must be between 1 and 100")]
    public required int DisplayOrder { get; set; }
}