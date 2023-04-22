using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FptBook.Models;

[Table("BookAuthors")]
public class BookAuthor
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string BookAuthorId { get; set; }
    
    [Required]
    public string BookId { get; set; }
    
    [Required]
    public string AuthorId { get; set; }
    
    [ForeignKey("BookId")]
    public virtual Book Book { get; set; }
    
    [ForeignKey("AuthorId")]
    public virtual Author Author { get; set; }
}