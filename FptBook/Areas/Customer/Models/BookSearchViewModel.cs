using System.ComponentModel.DataAnnotations;
using FptBook.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FptBook.Areas.Customer.Models;

//this class is to make searching easier
public class BookSearchViewModel
{
    public string SearchString { get; set; }
    [Display(Name = "Select")]
    public string SearchBy { get; set; }
    public IEnumerable<Book> Books { get; set; }

    public BookSearchViewModel()
    {
        //default is search by name
        SearchBy = "ByTitle";
    }
}