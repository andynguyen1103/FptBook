using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FptBook.Models;

[Table("CategoryRequests")]
public class CategoryRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string? RequestId { get; set; }
    
    [StringLength(50,ErrorMessage = "Category name too long!")]
    [Required]
    [Display(Name = "Category Name ")]
    public string? Name { get; set; }

    [DataType (DataType.Date)]
    [Display(Name = "Day Created")]
    public DateTime CreatedAt { get; set; }
    
    [Display(Name = "Approved Status")]
    public bool? IsApproved { get; set; }
    
    [Display(Name = "Day Approved")]
    public DateTime? ApprovedAt { get; set; }
    
    [Required]
    public string? UserID { set; get; }
    
    [ForeignKey("UserID")]
        
    [Display(Name = "User Email")]
    public FptBookUser? User { get; set; }

    public CategoryRequest()
    {
        CreatedAt = DateTime.Now;
        IsApproved = false;
    }
}