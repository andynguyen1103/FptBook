using Microsoft.AspNetCore.Mvc;

namespace FptBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("/Admin")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}