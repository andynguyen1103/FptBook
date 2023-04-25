using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FptBook.Models;

[Table("OrderDetails")]
public class OrderDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string ID { set; get; }
    
    [Required]
    public string OrderId { get; set; }
    
    [Required]
    public string BookId { get; set; }

    [Required]
    public int Quantity { get; set; }
    
    public float? Total { get; set; }

    [ForeignKey("OrderId")]
    public virtual Order Order { get; set; }

    [ForeignKey("BookId")]
    public virtual Book Book { get; set; }
}