using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FptBook.Models;

[Table("Categories")]
public class Category {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public string CategoryId { get; set; }
    
    [StringLength(50)]
    [Required]
    [Display(Name = "Category")]
    public string Name { get; set; }
}