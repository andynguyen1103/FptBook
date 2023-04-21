using FptBook.Areas.Identity.Data;
using FptBook.Models;
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

        public CategoryRequestController(FptBookIdentityDbContext context)
        {
            _context = context;
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
        public async Task<IActionResult> Details(int? id)
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
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: CategoryRequest/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestId,Name,CreatedAt,IsApproved,ApprovedAt,UserID")] CategoryRequest categoryRequest)
        {
            if (ModelState.IsValid)
            {
                _context.Add(categoryRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", categoryRequest.UserID);
            return View(categoryRequest);
        }

        // GET: CategoryRequest/Edit/5
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
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
        public async Task<IActionResult> Edit(int id, [Bind("RequestId,Name,CreatedAt,IsApproved,ApprovedAt,UserID")] CategoryRequest categoryRequest)
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
        public async Task<IActionResult> Delete(int? id)
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
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
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

        private bool CategoryRequestExists(int id)
        {
          return (_context.CategoryRequests?.Any(e => e.RequestId == id)).GetValueOrDefault();
        }
    }
}
