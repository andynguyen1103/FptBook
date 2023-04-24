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
    
    [StringLength(50)]
    [Required]
    [Display(Name = "Book")]
    public string Tittle { get; set; }
    
    [Required]
    [Display(Name = "Image")]
    public string ImageLink { get; set; }
    
    [DataType (DataType.Date)]
    [Required]
    [Display(Name = "Date")]
    public DateTime UpdateDate { get; set; }
    
    [Range(0, 1000)]
    [Required]
    public int Amount { get; set; }
    
    [StringLength(50)]
    [Required]
    public string Sumary { get; set; }
    
    [Column(TypeName = "Money")]
    [Required]
    public decimal Price { get; set; }
    
    [Required]
    public string CategoryID { get; set; }
    
    [Required]
    public string AuthorID { get; set; }
    
    [ForeignKey("CategoryID")]
    public virtual Category? Category { get; set; }
    
    [ForeignKey("AuthorID")]
    public virtual Author? Author { get; set; }

    public Book()
    {
        UpdateDate = DateTime.Now;
    }
}