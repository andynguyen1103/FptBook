using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FptBook.Models;
using FptBook.Mail;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FptBook.Controllers
{
    [Route("no-pass-login")]
    public class NoPasswordLoginController : Controller
    {
        public class LoginModel
        {
            [Required]
            [EmailAddress]
            public string? Email { get; set; }
        }
        
        private readonly UserManager<FptBookUser> _userManager;
        private readonly IEmailSender _emailSender;

        public NoPasswordLoginController(UserManager<FptBookUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }
        
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginModel loginModel) //get the email
        {
            var user = await _userManager.FindByEmailAsync(loginModel.Email); //find the user
            // var returnUrl = HttpContext?.Request.Query.FirstOrDefault(r => r.Key == "returnUrl");
            if(user is null)
            {
                return Unauthorized(); //if no user exist return 401
            }
            var token = _userManager.GenerateUserTokenAsync(user, "Default", "passwordless");
            Console.WriteLine(token);
            var loginUrl = Url.Action(action: "Login", controller: "NoPasswordLoginRedirect", values: new 
            { 
                Token = token.Result,
                UserId = user.Id
                // ReturnUrl = returnUrl?.Value
            }, protocol: Request.Scheme);
            Console.WriteLine(loginUrl);
            // send to mail
            await _emailSender.SendEmailAsync(loginModel.Email, "Confirm your email",
            $"Please login with this link <a href='{HtmlEncoder.Default.Encode(loginUrl)}'>clicking here</a>.");
            
            return View("CheckEmail", loginModel.Email); 
        }
    }
}