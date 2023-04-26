using FptBook.Data;
using FptBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FptBook.Areas.StoreManager.Controllers;

[Area("StoreManager")]
[Route("StoreManager/order/[action]")]
[Authorize]

public class OrderController : Controller
{
    private readonly FptBookIdentityDbContext _context;


    public OrderController(FptBookIdentityDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("/StoreManager/order", Name = "viewOrderMan")]
    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
            .Include(c => c.User)
            .Include(o => o.OrderDetails)
            .ThenInclude(od => od.Book)
            .ToListAsync();

        // foreach (var order in orders)
        // {
        //     Console.WriteLine("Orderisnull: " + order.OrderDetails.IsNullOrEmpty());
        // }
        
        return View(orders);
    }
    
    [HttpPost("{id}")]
    public async Task<IActionResult> Complete(string id)
    {
        var order = await _context.Orders
            .Include(c => c.User)
            .Include(o => o.OrderDetails)
            .FirstOrDefaultAsync(o=>o.OrderId==id);

        //if order is completed cannot delete
        if (order ==  null|| order.IsCompleted)
        {
            return NotFound();
        }

        try
        {
            order.IsCompleted = true;
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return RedirectToAction(nameof(Index));
        }
    }
}