using FptBook.Data;
using FptBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FptBook.Areas.Customer.Controllers;

[Route("/order/[action]")]
[Area("Customer")]
public class OrderController : Controller
{
    private readonly FptBookIdentityDbContext _context;
    private readonly UserManager<FptBookUser> _userManager;

    public OrderController(FptBookIdentityDbContext context, UserManager<FptBookUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    [HttpGet("/order", Name = "viewOrder")]
    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(HttpContext.User);
        var orders = await _context.Orders
            .Include(c => c.User)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Book)
            .Where(o => o.User == user).ToListAsync();

        foreach (var order in orders)
        {
            Console.WriteLine("Orderisnull: " + order.OrderDetails.IsNullOrEmpty());
        }
        
        return View(orders);
    }
}