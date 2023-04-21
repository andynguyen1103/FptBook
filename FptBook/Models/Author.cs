using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FptBook.Models;

[Table("Authors")]
public class Author
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string AuthorId { get; set; }
    
    [StringLength(50)]
    [Required]
    [Display(Name = "Author")]
    public string Name { get; set; }
}