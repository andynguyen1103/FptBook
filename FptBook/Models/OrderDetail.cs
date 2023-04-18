using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FptBook.Models;

[Table("OrderDetails")]
public class OrderDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID { set; get; }
    
    [Required]
    public int OrderID { get; set; }
    
    [Required]
    public int BookID { get; set; }

    [Required]
    public int Quantity { get; set; }

    [ForeignKey("OrderID")]
    public virtual Order Order { get; set; }

    [ForeignKey("ProductID")]
    public virtual Book Book { get; set; }
}