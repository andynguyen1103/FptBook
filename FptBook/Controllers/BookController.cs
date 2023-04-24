using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FptBook.Data;
using FptBook.Models;
using Newtonsoft.Json;

namespace FptBook.Controllers
{
    public class BookController : Controller
    {
        private readonly FptBookIdentityDbContext _context;

        public BookController(FptBookIdentityDbContext context)
        {
            _context = context;
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
            ViewData["AuthorID"] = new SelectList(_context.Authors, "AuthorID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Tittle,ImageLink,UpdateDate,Amount,Sumary,Price,CategoryID,AuthorID")] Book book)
        {
            if (ModelState.IsValid)
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == book.Category.Name);
                if (category == null)
                {
                    category = new Category { Name = book.Category.Name };
                    _context.Categories.Add(category);
                }
                book.Category = category;
                var author = await _context.Authors.FirstOrDefaultAsync(a => a.Name == book.Author.Name);
                if (author == null)
                {
                    author = new Author { Name = book.Author.Name };
                    _context.Authors.Add(author);
                }
                book.Author = author;

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryID"] = new SelectList(_context.Categories, "CategoryID", "Name", book.CategoryID);
            ViewData["AuthorID"] = new SelectList(_context.Authors, "AuthorID", "Name", book.AuthorID);
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
            [Bind("BookId,Tittle,ImageLink,UpdateDate,Amount,Sumary,Price,CategoryID,AuthorID")] Book book)
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

            var cart = GetCartItems();
            var cartitem = cart.Find(p => p.Book.BookId == bookid);
            if (cartitem != null) {
                cartitem.Quantity++;
            } else {
                cart.Add (new CartItem () { Quantity = 1, Book = book });
            }
            
            return RedirectToAction (nameof (Cart));
        }
        
        public const string CARTKEY = "cart";

        // Lấy cart từ Session (danh sách CartItem)
        List<CartItem> GetCartItems () {

            var session = HttpContext.Session;
            string jsoncart = session.GetString (CARTKEY);
            if (jsoncart != null) {
                return JsonConvert.DeserializeObject<List<CartItem>> (jsoncart);
            }
            return new List<CartItem> ();
        }

        // Xóa cart khỏi session
        void ClearCart () {
            var session = HttpContext.Session;
            session.Remove (CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào session
        void SaveCartSession (List<CartItem> ls) {
            var session = HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject (ls);
            session.SetString (CARTKEY, jsoncart);
        }
        
        [Route ("/cart", Name = "cart")]
        public IActionResult Cart () 
        {
            return View(GetCartItems());
        }
        
        [Route ("/removecart/{bookid:int}", Name = "removecart")]
        public IActionResult RemoveCart ([FromRoute] string bookid) {
            var cart = GetCartItems ();
            var cartitem = cart.Find (p => p.Book.BookId == bookid);
            if (cartitem != null) {
                cart.Remove(cartitem);
            }

            SaveCartSession (cart);
            return RedirectToAction (nameof (Cart));
        }
        
        [Route ("/updatecart", Name = "updatecart")]
        [HttpPost]
        public IActionResult UpdateCart ([FromForm] string bookid, [FromForm] int quantity) {
            var cart = GetCartItems ();
            var cartitem = cart.Find (p => p.Book.BookId == bookid);
            if (cartitem != null) {
                cartitem.Quantity = quantity;
            }
            SaveCartSession (cart);
            return Ok();
        }
        
        [Route ("/checkout")]
        public IActionResult Checkout()
        {
            var cart = GetCartItems ();

            ClearCart();
            return Content("Succesfull");

        }

        private bool BookExists(string id)
        {
          return (_context.Books?.Any(e => e.BookId == id)).GetValueOrDefault();
        }
    }
}
