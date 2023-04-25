using FptBook.Areas.StoreManager.Models;
using FptBook.Data;
using FptBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FptBook.Areas.StoreManager.Controllers
{
    [Route("StoreManager/book/[action]")]
    [Area("StoreManager")]
    [Authorize]
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
        [HttpGet("/StoreManager/book")]
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.Include(b => b.Author).Include(b => b.Category).ToListAsync();
            return View(books);
        }

        // GET: Book/Details/5
        [HttpGet("{id}")]
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

            return View(book);
        }

        // GET: Book/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // foreach (var item in categories)
            // {
            //     Console.WriteLine("Cat item:"+ item.Name);
            // }

            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name");
            // Console.WriteLine(ViewData["Categories"] !=null);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel bookViewModel)
        {
            try
            {
                string normalizedName = bookViewModel.AuthorName.Normalize().Trim();
                // Kiểm tra xem tác giả có trong database chưa
                var author = _context.Authors.FirstOrDefault(a =>
                    a.Name.ToUpper() == normalizedName);
                if (author == null)
                {
                    author = new Author { Name = bookViewModel.AuthorName.Trim() };
                    _context.Authors.Add(author);
                    await _context.SaveChangesAsync();
                }

                var category =
                    await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == bookViewModel.CategoryID);
                
                // Thêm sách vào database
                var book = new Book
                {
                    Title = bookViewModel.Title,
                    Amount = bookViewModel.Amount,
                    Summary = bookViewModel.Summary,
                    Price = bookViewModel.Price,
                    Author = author,
                    Category =  category
                };

                if (bookViewModel.ImageFile != null)
                {
                    var file = Path.GetFileNameWithoutExtension(Path.GetRandomFileName())+ 
                                    Path.GetExtension(bookViewModel.ImageFile.FileName);
                    var fileDirect = Path.Combine("wwwroot", "Upload", file);
                    using (var fileStream = new FileStream(fileDirect, FileMode.Create))
                    {
                        await bookViewModel.ImageFile.CopyToAsync(fileStream);
                    }

                    book.ImageLink = $"/Upload/"+file;
                }

                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name");
                return View();
            }
        }


        // GET: Book/Edit/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var book = await _context.Books.Include(b => b.Author).Include(b => b.Category).FirstOrDefaultAsync(b=>b.BookId == id);
            if (book == null)
            {
                return NotFound();
            }
            Console.WriteLine("book category id:" + _context.Authors.FirstOrDefault(a=>a.AuthorId==book.AuthorId).Name);
            var bookViewModel = new BookViewModel()
            {
                Title = book.Title,
                Amount = book.Amount,
                Summary = book.Summary,
                Price = book.Price,
                CategoryID = book.Category.Name,
                AuthorName = book.Author.Name,
                ImageFile = null
            };
            Console.WriteLine("yes");
            ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name",bookViewModel.CategoryID);
            return View(bookViewModel);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, BookViewModel bookViewModel)
        {
            if (!BookExists(id))
            {
                return NotFound();
            }

            try
            {
                // Kiểm tra xem tác giả có trong database chưa
                var author = _context.Authors.FirstOrDefault(a => a.Name == bookViewModel.AuthorName);
                if (author == null)
                {
                    author = new Author { Name = bookViewModel.AuthorName };
                    _context.Authors.Add(author);
                    await _context.SaveChangesAsync();
                }
                var category =
                    await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == bookViewModel.CategoryID);
                
                // Thêm sách vào database
                var book = await _context.Books.Include(b => b.Author).Include(b => b.Category)
                    .FirstOrDefaultAsync(b => b.BookId == id);
                book.Title = bookViewModel.Title;
                book.Amount = bookViewModel.Amount;
                book.Summary = bookViewModel.Summary;
                book.Price = bookViewModel.Price;
                book.Author = author;
                book.Category = category;
                
                if (bookViewModel.ImageFile != null)
                {
                    var file = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) +
                               Path.GetExtension(bookViewModel.ImageFile.FileName);
                    var fileDirect = Path.Combine("wwwroot", "Upload", file);
                    using (var fileStream = new FileStream(fileDirect, FileMode.Create))
                    {
                        await bookViewModel.ImageFile.CopyToAsync(fileStream);
                    }

                    book.ImageLink = $"/Upload/" + file;
                }

                _context.Update(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ViewData["Categories"] = new SelectList(_context.Categories, "CategoryId", "Name");
                return View();
            }
        }

        // GET: Book/Delete/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var book = await _context.Books.Include(b => b.Author).Include(b => b.Category).FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }
        

        // POST: Book/Delete/5
        [HttpPost("{id}"), ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var book = await _context.Books.FindAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        private bool BookExists(string id)
        {
          return (_context.Books?.Any(e => e.BookId == id)).GetValueOrDefault();
        }
        
    }
}
