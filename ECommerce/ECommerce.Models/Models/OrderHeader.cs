using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ECommerce.Models.Models;

public class OrderHeader
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("ApplicationUser")]
    [DisplayName("Application User")]
    public string? ApplicationUserId { get; set; }

    [ValidateNever]
    public ApplicationUser? ApplicationUser { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime ShippingDate { get; set; }

    public double OrderTotal { get; set; }
    public string? OrderStatus { get; set; }
    public string? PaymentStatus { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }

    public DateTime PaymentDate { get; set; }
    public DateOnly PaymentDueDate { get; set; }

    [Required]
    public string? Name { get; set; }
    [Required]
    public string? StreetAddress { get; set; }
    [Required]
    public string? City { get; set; }
    [Required]
    public string? State { get; set; }
    [Required]
    public string? PostalCode { get; set; }
    [Required]
    public string? PhoneNumber { get; set; }
}