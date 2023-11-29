using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models.Models;

public class Company
{
    [Key]
    public int Id { get; set; }

    [Required]
    [DisplayName("Name")]
    public string? Name { get; set; }

    [DisplayName("Street")]
    public string? StreetAddress { get; set; } 

    public string? City { get; set; }

    public string? State { get; set; }

    [DisplayName("Postal Code")]
    public string? PostalCode { get; set; } 

    [DisplayName("Phone Number")]
    public string? PhoneNumber { get; set; }

}