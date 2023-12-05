using ECommerce.Models.Models;

namespace ECommerce.Models.ViewModels;

public class ShoppingCartsVM
{
    public required OrderHeader OrderHeader { get; set; }
    public required IEnumerable<ShoppingCart> ShoppingCartList { get; set; }
}