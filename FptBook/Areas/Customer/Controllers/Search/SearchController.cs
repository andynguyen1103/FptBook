using Microsoft.AspNetCore.Mvc;

namespace FptBook.Areas.Customer.Controllers.Search
{
    public class SearchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}