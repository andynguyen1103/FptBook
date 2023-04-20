using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace FptBook.Models;

public class FptBookUser: IdentityUser
{
    [PersonalData]
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }
    
    [PersonalData]
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }
    
    [PersonalData]
    [Display(Name = "Address")]
    public string? HomeAddress { get; set; }
    
    
}