using ECommerce.Models.Models;

namespace ECommerce.Models.ViewModels;

public class OrderVM
{
    public required OrderHeader OrderHeader { get; set; }

    public required IEnumerable<OrderDetail> OrderDetails { get; set; }
}