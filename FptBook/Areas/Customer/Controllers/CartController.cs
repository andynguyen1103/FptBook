using FptBook.Data;
using FptBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FptBook.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize]
public class CartController : Controller
{
    private readonly FptBookIdentityDbContext _context;
    private readonly UserManager<FptBookUser> _userManager;

    public CartController(UserManager<FptBookUser> userManager, FptBookIdentityDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    
    [HttpGet("add-to-cart/{id}", Name = "addCart")]
    public async Task<IActionResult> AddToCart(string id)
    {
        var book = await _context.Books.Include(b => b.Author).Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.BookId == id);
        if (book == null) return NotFound();

        var cartItem = new CartItem
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
    
    [HttpGet("/cart", Name = "viewCart")]
    public async Task<IActionResult> Cart()
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
    public async Task<IActionResult> UpdateCart(string id, [FromForm] int cartInput)
    {
        Console.WriteLine("input:" + cartInput);
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
    
    [HttpGet("/cart/delete/{id}", Name = "removecart")]
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

    [HttpGet("/checkout")]
    public async Task<IActionResult> Checkout()
    {
        var checkoutList =
            await _context.CartItems.Include(ci => ci.Book)
                .Include(ci => ci.User)
                .Where(ci => ci.User == _userManager.GetUserAsync(HttpContext.User).Result)
                .ToListAsync();
        return View(checkoutList);
    }

    [HttpPost("/checkout")]
    [AutoValidateAntiforgeryToken]
    public async Task<IActionResult> Checkout(decimal total)
    {
        if (total == 0) return BadRequest();
        var checkoutList =
            await _context.CartItems.Include(ci => ci.Book)
                .Include(ci => ci.User)
                .Where(ci => ci.User == _userManager.GetUserAsync(HttpContext.User).Result)
                .ToListAsync();

        var order = new Order
        {
            CreatedAt = DateTime.Now,
            TotalPrice = total,
            User = await _userManager.GetUserAsync(HttpContext.User),
            IsCompleted = false
        };

        var orderDetails = new List<OrderDetail>();
        foreach (var item in checkoutList.Select(cl => new { cl.Book, cl.Quantity }))
        {
            var temp = new OrderDetail
            {
                Order = order,
                Book = item.Book,
                Quantity = item.Quantity,
                Total = (float?)(item.Book.Price * item.Quantity)
            };

            orderDetails.Add(temp);
        }

        order.OrderDetails = orderDetails;
        await _context.Orders.AddAsync(order);
        await _context.OrderDetails.AddRangeAsync(orderDetails);
        _context.CartItems.RemoveRange(checkoutList);
        await _context.SaveChangesAsync();
        return RedirectToRoute("viewOrder");
    }
}