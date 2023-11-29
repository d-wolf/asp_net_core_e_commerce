using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ECommerce.Models.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    public required string Name { get; set; }
    public string? StreetAddress { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }

    [ForeignKey("Company")]
    [DisplayName("Company")]
    public int? CompanyId { get; set; }

    [ValidateNever]
    public Company? Company { get; set; }
}