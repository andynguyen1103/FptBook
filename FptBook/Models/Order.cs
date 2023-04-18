using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FptBook.Models;

[Table("Orders")]
public class Order
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int OrderId { get; set; }
    
    [Column(TypeName = "Money")]
    [Required]
    public decimal TotalPrice { get; set; }
    
    [DataType (DataType.Date)]
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public bool IsCompleted { get; set; }
    
    [Required]
    public string UserID { set; get; }
    
    [ForeignKey("UserID")]
    public virtual FptBookUser User { get; set; }
    
    public virtual IEnumerable<OrderDetail> OrderDetails { set; get; }

    public Order()
    {
        CreatedAt = DateTime.Now;
    }
}