using FptBook.Areas.Customer.Models;
using FptBook.Areas.StoreManager.Models;
using FptBook.Data;
using FptBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FptBook.Areas.Customer.Controllers
{
    [Route("book/[action]")]
    [Area("Customer")]
    public class BookController : Controller
    {
        private readonly FptBookIdentityDbContext _context;
        private readonly UserManager<FptBookUser> _userManager;

        public BookController(FptBookIdentityDbContext context, UserManager<FptBookUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Book
        [HttpGet("/book")]
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.Include(b => b.Author).Include(b => b.Category).ToListAsync();
            var bookViewModel = new BookSearchViewModel()
            {
                Books = books
            };
            ViewData["SearchBy"] = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Selected = true, Text = "By Title", Value = "ByTitle"},
                new SelectListItem { Selected = false, Text = "By Category", Value = "ByCategory"},
            },"Value","Text");
            return View(bookViewModel);
        }
        [HttpPost("/book")]
        public async Task<IActionResult> Index(BookSearchViewModel bookSearchViewModel)
        {
            Console.WriteLine("Search By:" + bookSearchViewModel.SearchBy);
            var books = new List<Book>();
            if (bookSearchViewModel.SearchBy == "ByTitle")
            {
                books = await _context.Books.Include(b => b.Author)
                    .Include(b => b.Category)
                    .Where(b => b.Title.Contains(bookSearchViewModel.SearchString))
                    .ToListAsync();
            }
            else
            {
                books = await _context.Books.Include(b => b.Author)
                    .Include(b => b.Category)
                    .Where(b => b.Category.Name.Contains(bookSearchViewModel.SearchString))
                    .ToListAsync();
            }
            if(bookSearchViewModel.SearchString.IsNullOrEmpty())
            {
                books = await _context.Books.Include(b => b.Author)
                    .Include(b => b.Category)
                    .ToListAsync();
            }

            var bookViewModel = new BookSearchViewModel()
            {
                Books = books
            };
            ViewData["SearchBy"] = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Selected = false, Text = "By Title", Value = "ByTitle"},
                new SelectListItem { Selected = false, Text = "By Category", Value = "ByCategory"},
            },"Value","Text");
            
            return View(bookViewModel);
        }
        
        private bool BookExists(string id)
        {
          return (_context.Books?.Any(e => e.BookId == id)).GetValueOrDefault();
        }
        
        
        
    }
}
