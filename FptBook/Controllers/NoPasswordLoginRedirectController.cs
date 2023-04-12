using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FptBook.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FptBook.Controllers
{
    public class NoPasswordLoginRedirectController : Controller
    {
        private readonly UserManager<FptBookUser> _userManager;
        private readonly SignInManager<FptBookUser> _signInManager;

        public NoPasswordLoginRedirectController(UserManager<FptBookUser> userManager, SignInManager<FptBookUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        [HttpGet]
        public async Task<IActionResult> Login([FromQuery] string token, [FromQuery] string userId)
        {
            // Fetch your user from the database
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var isValid = await _userManager.VerifyUserTokenAsync(user, "Default", "passwordless", token);
            if (isValid)
            {
                await _signInManager.SignInAsync(user, false);
                    
                return new RedirectResult("/");
            }
            return Unauthorized();


        }
    }
}