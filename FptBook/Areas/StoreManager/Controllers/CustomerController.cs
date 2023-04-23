using System.Drawing.Printing;
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

        [HttpPost("")]
        public async Task<IActionResult> Index(CustomerListViewModel input)
        {
            Console.WriteLine("input search string: " + input.SearchString);
            var customerViewmodel = input;
            if (customerViewmodel.SearchString.IsNullOrEmpty())
            {
                Console.WriteLine("Search String is null");
                var customers = await _userManager.GetUsersInRoleAsync(RoleNames.Customer);
                if (customers.IsNullOrEmpty())
                {
                    ViewData["Message"] = "No Customer available";
                }

                customerViewmodel.Customers = customers;
                return View(customerViewmodel);
            }
            Console.WriteLine("ViewModel Search string: " + customerViewmodel.SearchString);
            Console.WriteLine("contain: "+ "tuhuy1005@gmail".Normalize().Contains(customerViewmodel.SearchString));
            var searchedCustomers = _userManager.GetUsersInRoleAsync(RoleNames.Customer)
                                                                    .GetAwaiter()
                                                                    .GetResult()
                                                                    .Where(u=> u.NormalizedEmail.Contains(customerViewmodel.SearchString.ToUpper()));
            customerViewmodel.Customers = searchedCustomers;
            
            return View(customerViewmodel);
        }

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