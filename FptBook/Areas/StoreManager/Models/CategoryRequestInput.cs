using System.ComponentModel.DataAnnotations;

namespace FptBook.Areas.StoreManager.Models;

public class CategoryRequestInput
{
    [StringLength(50,ErrorMessage = "Category name too long!")]
    [Required]
    [Display(Name = "Category")]
    public string Name { get; set; }
}