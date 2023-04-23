using FptBook.Models;

namespace FptBook.Areas.StoreManager.Models;

public class CustomerDetailsViewModel
{
    public FptBookUser Customer { get; set; }
    public IEnumerable<Order>? CustomerOrders { get; set; }
    
}