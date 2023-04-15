using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FptBook.Models;

[Table("CategoryRequests")]
public class CategoryRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int RequestId { get; set; }
    
    [StringLength(50)]
    [Required]
    [Display(Name = "Category")]
    public string Name { get; set; }

    [DataType (DataType.Date)]
    [Required]
    public DateTime CreatedAt { get; set; }
    
    [Required]
    public string UserID { set; get; }
    
    [ForeignKey("UserID")]
    public virtual FptBookUser User { get; set; }

    public CategoryRequest()
    {
        CreatedAt = DateTime.Now;
    }
}