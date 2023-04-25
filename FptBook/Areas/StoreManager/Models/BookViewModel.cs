using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FptBook.Areas.StoreManager.Models;

public class BookViewModel
{
    
    [Required]
    [Display(Name = "Book")]
    public string Title { get; set; }
    
    [Display(Name = "Image")]
    public IFormFile? ImageFile { get; set; }

    [Range(0, 10000)]
    [Required]
    public int Amount { get; set; }
    
    [Required]
    public string Summary { get; set; }
    
    [Column(TypeName = "Money")]
    [Required]
    public decimal Price { get; set; }
    
    public string CategoryID { get; set; }
    
    public string AuthorName { get; set; }
}