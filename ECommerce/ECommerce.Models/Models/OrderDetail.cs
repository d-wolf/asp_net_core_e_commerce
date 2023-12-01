using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace ECommerce.Models.Models;

public class OrderDetail
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("OrderHeader")]
    public int? OrderHeaderId { get; set; }

    [ValidateNever]
    public OrderHeader? OrderHeader { get; set; }

    [Required]
    [ForeignKey("Product")]
    public int? ProductId { get; set; }

    [ValidateNever]
    public Product? Product { get; set; }

    public int Count { get; set; }

    public double Price { get; set; }
}