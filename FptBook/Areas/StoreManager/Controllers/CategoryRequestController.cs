using FptBook.Areas.Identity.Data;
using FptBook.Areas.StoreManager.Models;
using FptBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FptBook.Areas.StoreManager.Controllers
{
    [Area("StoreManager")]
    [Route("/StoreManager/CategoryRequest")]
    public class CategoryRequestController : Controller
    {
        private readonly FptBookIdentityDbContext _context;
        private readonly UserManager<FptBookUser> _userManager;

        public CategoryRequestController(FptBookIdentityDbContext context, UserManager<FptBookUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: CategoryRequest
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var fptBookIdentityDbContext = _context.CategoryRequests.Include(c => c.User);
            return View(await fptBookIdentityDbContext.ToListAsync());
        }

        // GET: CategoryRequest/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null || _context.CategoryRequests == null)
            {
                return NotFound();
            }

            var categoryRequest = await _context.CategoryRequests
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (categoryRequest == null)
            {
                return NotFound();
            }

            return View(categoryRequest);
        }

        // GET: CategoryRequest/Create
        [HttpGet("Create")]
        public IActionResult Create()
        {
            var user = _userManager.GetUserAsync(HttpContext.User).Result;
            ViewData["UserID"] = user.Id;
            return View();
        }

        // POST: CategoryRequest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestId,Name,CreatedAt,IsApproved,ApprovedAt,UserID")] CategoryRequest categoryRequest)
        {
            if (categoryRequest.UserID==null)
            {
                //anonymous
                categoryRequest.UserID = _userManager.GetUsersInRoleAsync(RoleNames.Administrator).Result[0].Id;
            }
            try
            {
                _context.Add(categoryRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                return View(categoryRequest);
            }

        }

        // GET: CategoryRequest/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null || _context.CategoryRequests == null)
            {
                return NotFound();
            }

            var categoryRequest = await _context.CategoryRequests.FindAsync(id);
            if (categoryRequest == null)
            {
                return NotFound();
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", categoryRequest.UserID);
            return View(categoryRequest);
        }

        // POST: CategoryRequest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("RequestId,Name,CreatedAt,IsApproved,ApprovedAt,UserID")] CategoryRequest categoryRequest)
        {
            if (id != categoryRequest.RequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(categoryRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryRequestExists(categoryRequest.RequestId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", categoryRequest.UserID);
            return View(categoryRequest);
        }

        // GET: CategoryRequest/Delete/5
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null || _context.CategoryRequests == null)
            {
                return NotFound();
            }

            var categoryRequest = await _context.CategoryRequests
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (categoryRequest == null)
            {
                return NotFound();
            }

            return View(categoryRequest);
        }

        // POST: CategoryRequest/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Console.WriteLine("id: "+id);
            if (_context.CategoryRequests == null)
            {
                return Problem("Entity set 'FptBookIdentityDbContext.CategoryRequests'  is null.");
            }
            var categoryRequest = await _context.CategoryRequests.FindAsync(id);
            if (categoryRequest != null)
            {
                _context.CategoryRequests.Remove(categoryRequest);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryRequestExists(string id)
        {
          return (_context.CategoryRequests?.Any(e => e.RequestId == id)).GetValueOrDefault();
        }
    }
}
