using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FptBook.Models;

public class BookViewModel
{
    public string BookId { get; set; }
    
    [StringLength(50)]
    [Required]
    [Display(Name = "Book")]
    public string Tittle { get; set; }
    
    [Required]
    [Display(Name = "Image")]
    public IFormFile ImageFile { get; set; }

    [Range(0, 1000)]
    [Required]
    public int Amount { get; set; }
    
    [StringLength(50)]
    [Required]
    public string Sumary { get; set; }
    
    [Column(TypeName = "Money")]
    [Required]
    public decimal Price { get; set; }
    
    public SelectList CategoryID { get; set; }
    
    public string AuthorName { get; set; }
}