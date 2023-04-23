using FptBook.Models;

namespace FptBook.Areas.StoreManager.Models;

public class CustomerListViewModel
{
    public string? SearchString { get; set; } 
    public IEnumerable<FptBookUser> Customers { get; set; }
}