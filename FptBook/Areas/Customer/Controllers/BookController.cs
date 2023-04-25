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
        
        
        [Authorize]
        [HttpGet("add-to-cart/{id}", Name = "addCart")]
        public async Task<IActionResult> AddToCart(string id)
        {
            var book = await _context.Books.Include(b => b.Author).Include(b => b.Category).FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            var cartItem = new CartItem()
            {
                User = await _userManager.GetUserAsync(HttpContext.User),
                Book = book,
                Quantity = 1
            };
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
            
            //to go to previous page that lead to the add action
            return Redirect(HttpContext.Request.Headers["Referer"]);
        }

        [Authorize]
        [HttpGet("/cart", Name = "viewCart")]
        public async Task<IActionResult> Cart(string id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var cardItems = await _context.CartItems.Include(ci => ci.Book)
                                                                .Include(ci => ci.User)
                                                                .Where(ci => ci.User == user).ToListAsync();
            return View(cardItems);
        }

        [Authorize]
        [HttpPost("/cart/update/{id}")]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UpdateCart(string id,[FromForm]int cartInput)
        {
            Console.WriteLine("input:"+cartInput);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var cardItems = await _context.CartItems.Include(ci => ci.Book)
                .Include(ci => ci.User)
                .Where(ci => ci.User == user).ToListAsync();
            var cardItem = cardItems.FirstOrDefault(ci => ci.Id == id);
            cardItem.Quantity = cartInput;
            try
            {
                _context.Update(cardItem);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Cart));
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Cart));
        }
        
        [Authorize]
        [HttpGet("/cart/delete/{id}",Name = "removecart")]
        public async Task<IActionResult> DeleteCart(string id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var cardItems = await _context.CartItems.Include(ci => ci.Book)
                .Include(ci => ci.User)
                .Where(ci => ci.User == user).ToListAsync();
            var cardItem = cardItems.FirstOrDefault(ci => ci.Id == id);

            try
            {
                _context.Remove(cardItem);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return RedirectToAction(nameof(Cart));
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Cart));
        }
        private bool BookExists(string id)
        {
          return (_context.Books?.Any(e => e.BookId == id)).GetValueOrDefault();
        }
        
        
        
    }
}
