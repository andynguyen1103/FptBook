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
            var books = await _context.Books.Include(b => b.Author).Include(b => b.Category).ToListAsync();
            return View(books);
        }

        // GET: Book/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var book = await _context.Books
                .Include(b => b.Category)
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View();
        }

        // GET: Book/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var categories = await _context.Categories.ToListAsync();
            foreach (var item in categories)
            {
                Console.WriteLine("Cat item:"+ item.Name);
            }

            var viewModel = new BookViewModel()
            {
                CategoryID = new SelectList(categories, "CategoryID", "Name")
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel bookViewModel)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra xem tác giả có trong database chưa
                var author = _context.Authors.FirstOrDefault(a => a.Name == bookViewModel.AuthorName);
                if (author == null)
                {
                    author = new Author { Name = bookViewModel.AuthorName };
                    _context.Authors.Add(author);
                    await _context.SaveChangesAsync();
                }

                // Thêm sách vào database
                var book = new Book
                {
                    Tittle = bookViewModel.Tittle,
                    Amount = bookViewModel.Amount,
                    Sumary = bookViewModel.Sumary,
                    Price = bookViewModel.Price,
                    CategoryID = bookViewModel.CategoryID.SelectedValue.ToString(),
                    AuthorID = author.Name
                };

                if (bookViewModel.ImageFile != null)
                {
                    var extension = Path.GetExtension(bookViewModel.ImageFile.FileName);
                    var fileName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", $"{book.Tittle}{extension}");
                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        await bookViewModel.ImageFile.CopyToAsync(fileStream);
                    }
                    book.ImageLink = $"/images/{book.Tittle}{extension}";
                }

                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryName = new SelectList(_context.Categories.ToList(), "Id", "Name", bookViewModel.CategoryID);
            return View();
        }


        // GET: Book/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            var bookViewModel = new BookViewModel
            {
                Tittle = book.Tittle,
                Amount = book.Amount,
                Sumary = book.Sumary,
                Price = book.Price,
                CategoryID = book.Category.Name,
                AuthorName = book.Author.Name
            };
            ViewBag.CategoryID = new SelectList(_context.Categories.ToList(), "Id", "Name", bookViewModel.CategoryID);
            return View();
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, BookViewModel bookViewModel)
        {
            if (id != bookViewModel.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra xem tác giả có trong database chưa
                var author = _context.Authors.FirstOrDefault(a => a.Name == bookViewModel.AuthorName);
                if (author == null)
                {
                    author = new Author { Name = bookViewModel.AuthorName };
                    _context.Authors.Add(author);
                    await _context.SaveChangesAsync();
                }

                // Sửa thông tin sách trong database
                var book = await _context.Books.FindAsync(id);
                book.Tittle = bookViewModel.Tittle;
                book.Amount = bookViewModel.Amount;
                book.Sumary = bookViewModel.Sumary;
                book.Price = bookViewModel.Price;
                book.CategoryID = bookViewModel.CategoryID;
                book.AuthorID = author.Name;

                if (bookViewModel.ImageFile != null)
                {
                    var extension = Path.GetExtension(bookViewModel.ImageFile.FileName);
                    var fileName = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", $"{book.Tittle}{extension}");
                    using (var fileStream = new FileStream(fileName, FileMode.Create))
                    {
                        await bookViewModel.ImageFile.CopyToAsync(fileStream);
                    }
                    book.ImageLink = $"/images/{book.Tittle}{extension}";
                }

                _context.Update(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryID = new SelectList(_context.Categories.ToList(), "Id", "Name", bookViewModel.CategoryID);
            return View();
        }

        // GET: Book/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            var book = await _context.Books.Include(b => b.Author).Include(b => b.Category).FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View();
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
        
        // Lấy danh sách CategoryName để hiển thị lên selectlist
        private async Task<SelectList> GetCategoryListAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            var categoryList = new SelectList(categories, "Id", "Name");
            return categoryList;
        }
        
        // Lấy danh sách AuthorName để kiểm tra
        private async Task<SelectList> GetAuthorListAsync()
        {
            var authors = await _context.Authors.ToListAsync();
            var authorList = new SelectList(authors, "Name");
            return authorList;
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
