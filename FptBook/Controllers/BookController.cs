using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FptBook.Data;
using FptBook.Models;

namespace FptBook.Controllers
{
    public class BookController : Controller
    {
        private readonly FptBookIdentityDbContext _context;
        private readonly CartService _cartService;

        public BookController(FptBookIdentityDbContext context, CartService cartService)
        {
            _context = context;
            _cartService = cartService;
        }

        // GET: Book
        public async Task<IActionResult> Index()
        {
            var fptBookIdentityDbContext = _context.Books.Include(b => b.Category);
            return View(await fptBookIdentityDbContext.ToListAsync());
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BookId == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Book/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "Name");
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Tittle,ImageLink,UpdateDate,Amount,Sumary,Price,CategoryID")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "Name", book.CategoryID);
            return View(book);
        }

        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "Name", book.CategoryID);
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id,
            [Bind("BookId,Tittle,ImageLink,UpdateDate,Amount,Sumary,Price,CategoryID")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
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

            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "Name", book.CategoryID);
            return View(book);
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        // GET: Books/AddToCart/5
        [Route ("addcart/{productid:int}", Name = "addcart")]
        public IActionResult AddToCart ([FromRoute] string bookid) {

            var book = _context.Books
                .FirstOrDefault (p => p.BookId == bookid);
            if (book == null)
                return NotFound ("Not found");

            var cart = _cartService.GetCartItems();
            var cartitem = cart.Find(p => p.Book.BookId == bookid);
            if (cartitem != null) {
                cartitem.Quantity++;
            } else {
                cart.Add (new CartItem () { Quantity = 1, Book = book });
            }
            
            return RedirectToAction (nameof (Cart));
        }
        
        [Route ("/cart", Name = "cart")]
        public IActionResult Cart () 
        {
            return View (_cartService.GetCartItems());
        }
        
        [Route ("/removecart/{bookid:int}", Name = "removecart")]
        public IActionResult RemoveCart ([FromRoute] string bookid) {
            var cart = _cartService.GetCartItems ();
            var cartitem = cart.Find (p => p.Book.BookId == bookid);
            if (cartitem != null) {
                cart.Remove(cartitem);
            }

            _cartService.SaveCartSession (cart);
            return RedirectToAction (nameof (Cart));
        }
        
        [Route ("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart ([FromForm] string bookid, [FromForm] int quantity) {
            var cart = _cartService.GetCartItems ();
            var cartitem = cart.Find (p => p.Book.BookId == bookid);
            if (cartitem != null) {
                cartitem.Quantity = quantity;
            }
            _cartService.SaveCartSession (cart);
            return Ok();
        }
        
        [Route ("/checkout")]
        public IActionResult Checkout()
        {
            var cart = _cartService.GetCartItems ();

            _cartService.ClearCart();
            return Content("Succesfull");

        }

        private bool BookExists(string id)
        {
          return (_context.Books?.Any(e => e.BookId == id)).GetValueOrDefault();
        }
    }
}
