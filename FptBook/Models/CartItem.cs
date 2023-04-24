namespace FptBook.Models;

public class CartItem
{
    public string BookId { get; set; }
    public int Quantity { get; set; }
    public Book Book { get; set; }
}