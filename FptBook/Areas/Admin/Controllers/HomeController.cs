using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FptBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/Admin")]
    [Authorize]
    public class HomeController : Controller
    {
        [Route("/Admin",Name = "adminDefault")]
        public IActionResult Index()
        {
            return View();
        }
    }
}