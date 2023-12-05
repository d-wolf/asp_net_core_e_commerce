using ECommerce.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ECommerce.Models.ViewModels;

public class RoleManagementVM
{
    public required ApplicationUser ApplicationUser { get; set; }
    public required IEnumerable<SelectListItem> RoleList { get; set; }
    public required IEnumerable<SelectListItem> CompanyList { get; set; }
    public required string CurrentRole { get; set; }
}