using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FptBook.Models;

[Table("CartItem")]
public class CartItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string Id { get; set; }
    
    public string BookId { get; set; }
    [ForeignKey("BookId")]
    public Book Book { get; set; }

    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public FptBookUser User { get; set; }
    
    public int Quantity { get; set; }
}
