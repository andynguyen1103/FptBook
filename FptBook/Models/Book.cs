using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FptBook.Models;

[Table("Books")]
public class Book
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string BookId { get; set; }
    
    [Required]
    [Display(Name = "Book")]
    public string Title { get; set; }
    
    [Required]
    [Display(Name = "Image")]
    public string? ImageLink { get; set; }
    
    [DataType (DataType.Date)]
    [Required]
    [Display(Name = "Date")]
    public DateTime UpdateDate { get; set; }
    
    [Range(0, 10000)]
    [Required]
    public int Amount { get; set; }
    
    [Required]
    public string Summary { get; set; }
    
    [Column(TypeName = "Money")]
    [Required]
    public decimal Price { get; set; }
    
    [Required]
    public string CategoryId { get; set; }
    
    [Required]
    public string AuthorId { get; set; }
    
    [ForeignKey("CategoryId")]
    public virtual Category? Category { get; set; }
    
    [ForeignKey("AuthorId")]
    public virtual Author? Author { get; set; }

    public Book()
    {
        UpdateDate = DateTime.Now;
    }
}