using ECommerce.Models.Models;

namespace ECommerce.Models.ViewModels;

public class UserVM
{
    public required ApplicationUser ApplicationUser { get; set; }

    public required string Role { get; set; }
}