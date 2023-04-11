using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FptBook.Models;

public class ApplicationUser: IdentityUser
{
    [Display(Name = "First Name")]
    public string FirstName { get; set; }
    
    [Display(Name = "Last Name")]
    public string LastName { get; set; }
    
    [Required(ErrorMessage = "Please enter your home address")]
    [Display(Name = "Address")]
    public string HomeAddress { get; set; }
}