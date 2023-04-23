using FptBook.Data;
using FptBook.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FptBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/CategoryRequest")]
    public class CategoryRequestController : Controller
    {
        private readonly FptBookIdentityDbContext _context;

        public CategoryRequestController(FptBookIdentityDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var categoryRequests = _context.CategoryRequests
                                                                        .OrderBy(cr=>cr.IsApproved)
                                                                        .Include(c => c.User);
            return View(await categoryRequests.ToListAsync());
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
        
        [HttpGet("Approve/{id}")]
        public async Task<IActionResult> Approve(string? id)
        {
            if (id == null && !await _context.CategoryRequests.AnyAsync())
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
            
            if (categoryRequest.IsApproved.GetValueOrDefault())
            {
                return NotFound();
            }
            // ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", categoryRequest.UserID);
            return View(categoryRequest);
        }

        // POST: CategoryRequest/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("Approve/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(string id, [Bind("RequestId,Name,CreatedAt,IsApproved,ApprovedAt,UserID")] CategoryRequest categoryRequest)
        {
            // Console.WriteLine("queryID:"+id);
            // Console.WriteLine("requestID"+categoryRequest.RequestId);
            if (id != categoryRequest.RequestId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (categoryRequest.IsApproved.GetValueOrDefault())
                {
                    // Console.WriteLine(!categoryRequest.IsApproved.GetValueOrDefault());
                    return NotFound();
                }
                try
                {
                    categoryRequest.IsApproved = true;
                    categoryRequest.ApprovedAt = DateTime.Now;
                    _context.Update(categoryRequest);
                    
                    //add category to database

                    var category = new Category()
                    {
                        Name = categoryRequest.Name
                    };
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.CategoryRequests.AnyAsync().Result)
                    {
                        return NotFound();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // ViewData["UserID"] = new SelectList(_context.Users, "Id", "Id", categoryRequest.UserID);
            return View(categoryRequest);
        }
    }
}