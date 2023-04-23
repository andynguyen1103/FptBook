

using System.ComponentModel.DataAnnotations;
using FptBook.Models;

namespace FptBook.Areas.Admin.Models;

public class CategoryViewModel
{
    [Required(ErrorMessage = "Category name is needed")]
    public string Name { get; set; }
}