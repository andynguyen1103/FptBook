using FptBook.Areas.Admin.Models;
using FptBook.Data;
using FptBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FptBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/User")]
    [Authorize]
    // [Route("Admin/User/[action]/{id}")]
    public class UserController : Controller
    {
        private readonly FptBookIdentityDbContext _context;
        private readonly UserManager<FptBookUser> _userManager;

        public UserController(FptBookIdentityDbContext context, UserManager<FptBookUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: User
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'FptBookIdentityDbContext.Users'  is null.");
        }

        [HttpGet("CreateManager")]
        public async Task<IActionResult> CreateManager()
        {
            var userInput = new UserViewModel();
            return View(userInput);
        }
        
        
        [HttpPost("CreateManager")]
        public async Task<IActionResult> CreateManager(UserViewModel userInput)
        {
            var user = new FptBookUser()
            {
                Email = userInput.Email,
                UserName = userInput.Email,
                FirstName = userInput.FirstName,
                LastName = userInput.LastName,
                EmailConfirmed = true
            };
            // Console.WriteLine("user name:"+user.Email);
            //add all the information
            //Im lazy so the password is the phone number
            
            var createResult = await _userManager.CreateAsync(user, "manager12345");
            if (createResult.Succeeded)
            {
                Console.WriteLine("Create User Success");
                await _userManager.SetPhoneNumberAsync(user, userInput.PhoneNumber);
                await _userManager.AddToRoleAsync(user, RoleNames.StoreManager);
            }
            else
            {
                Console.WriteLine(createResult.Errors);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: User/Details/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var fptBookUser = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (fptBookUser == null)
            {
                return NotFound();
            }

            return View(fptBookUser);
        }
        
        [HttpGet("Disable/{id}")]
        public async Task<IActionResult> Disable(string id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'FptBookIdentityDbContext.Users'  is null.");
            }
            Console.WriteLine("id="+id);
            var fptBookUser = await _userManager.FindByIdAsync(id);
            if (fptBookUser == null)
            {
                RedirectToAction(nameof(Index));
            }
            
            if (await _userManager.IsInRoleAsync(fptBookUser,RoleNames.Administrator))
            {
                return NotFound();
            }

            if (!await _userManager.GetLockoutEnabledAsync(fptBookUser))
            {
                RedirectToAction(nameof(Index));
            }

            //Lock user out
            await _userManager.SetLockoutEnabledAsync(fptBookUser, true);
            await _userManager.SetLockoutEndDateAsync(fptBookUser, DateTime.MaxValue);
            
            Console.WriteLine("Disable Successful");
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet("Activate/{id}")]
        public async Task<IActionResult> Activate(string id)
        {
            var fptBookUser = await _userManager.FindByIdAsync(id);
            // Console.WriteLine(fptBookUser.Id);
            if (fptBookUser == null)
            {
                return RedirectToAction(nameof(Index));
            }

            if (await _userManager.IsInRoleAsync(fptBookUser,RoleNames.Administrator))
            {
                return NotFound();
            }

            if (!await _userManager.GetLockoutEnabledAsync(fptBookUser))
            {
                return RedirectToAction(nameof(Index));
            }
            await _userManager.SetLockoutEnabledAsync(fptBookUser, false);
            await _userManager.SetLockoutEndDateAsync(fptBookUser,null);
            
            Console.WriteLine("Activate Successful");

            return RedirectToAction(nameof(Index));
        }
        
        private bool FptBookUserExists(string id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        
        [HttpGet("PasswordReset/{id}")]
        public async Task<IActionResult> PasswordReset(string id)
        {
            return View();
        }
        
        [HttpPost("PasswordReset/{id}")]
        public async Task<IActionResult> PasswordReset(string id,string password)
        {
            
            var user = await _userManager.FindByIdAsync(id);
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("Index");
        
            }
            

            var result = await _userManager.ResetPasswordAsync(user,code,password);
            if (result.Succeeded)
            {
                 return RedirectToAction(nameof(Index));
            }

            return BadRequest();
        }

    }
}
