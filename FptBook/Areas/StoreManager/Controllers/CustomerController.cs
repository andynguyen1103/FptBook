using FptBook.Areas.StoreManager.Models;
using FptBook.Data;
using FptBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FptBook.Areas.StoreManager.Controllers
{
    [Area("StoreManager")]
    [Route("StoreManager/Customer")]
    public class CustomerController : Controller
    {
        private readonly FptBookIdentityDbContext _context;
        private readonly UserManager<FptBookUser> _userManager;

        public CustomerController(FptBookIdentityDbContext context, UserManager<FptBookUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var customers = await _userManager.GetUsersInRoleAsync(RoleNames.Customer);
            if (customers.IsNullOrEmpty())
            {
                ViewData["Message"] = "No Customer available";
            }
            var customerViewmodel = new CustomerListViewModel()
            {
                Customers = customers
            };
            return View(customerViewmodel);
        }

        // [HttpPost("")]
        // public async Task<IActionResult> Index(string searchString)
        // {
        //     
        // }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(string id)
        {
            var customer = _userManager.GetUsersInRoleAsync(RoleNames.Customer)
                                                .Result
                                                .FirstOrDefault(u => u.Id == id);
            if (customer==null)
            {
                return NotFound();
            }

            var customerViewModel = new CustomerDetailsViewModel()
            {
                Customer = customer,
                CustomerOrders = await _context.Orders.Where(o => o.UserID == customer.Id).ToListAsync()
            };
            return View(customerViewModel);
        }
    }
}