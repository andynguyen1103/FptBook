using FptBook.Areas.Identity.Data;
using FptBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FptBook.Areas.Admin.Controllers.User
{
    [Area("Admin")]
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
        [HttpGet("Admin/User")]
        public async Task<IActionResult> Index()
        {
              return _context.Users != null ? 
                          View(await _context.Users.ToListAsync()) :
                          Problem("Entity set 'FptBookIdentityDbContext.Users'  is null.");
        }

        // GET: User/Details/5
        [HttpGet("Admin/User/{id}")]
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

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string id)
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

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'FptBookIdentityDbContext.Users'  is null.");
            }
            var fptBookUser = await _context.Users.FindAsync(id);
            if (fptBookUser != null)
            {
                _context.Users.Remove(fptBookUser);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        private bool FptBookUserExists(string id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        
        [HttpGet("Admin/User/PasswordReset/{id}")]
        public async Task<IActionResult> PasswordReset(string id)
        {
            return View();
        }
        
        [HttpPost("Admin/User/PasswordReset/{id}")]
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
                 return RedirectToAction("Index");
            }

            return BadRequest();
        }
    }
}
