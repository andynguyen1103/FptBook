using System.ComponentModel.DataAnnotations;

namespace FptBook.Areas.Admin.Models;

public class UserViewModel
{
    [EmailAddress]
    [Required(ErrorMessage = "Missing email address")]
    public string Email { get; set; }
            
    [Required(ErrorMessage = "You must enter your first name!")]
    [Display(Name = "First Name")]
    [StringLength(100,ErrorMessage = "First name too long!")]
    public string FirstName { get; set; }
            
    [Required(ErrorMessage = "You must enter your last name")]
    [Display(Name = "Last Name")]
    [StringLength(100,ErrorMessage = "Last name too long!")]
    public string LastName { get; set; }
            
    [Required(ErrorMessage = "You must enter a phone number to register!")]
    [Display(Name = "Phone Number")]
    [Phone] 
    public string PhoneNumber { get; set; }
}