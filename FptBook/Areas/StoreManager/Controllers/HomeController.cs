using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FptBook._Areas_StoreManager_Controllers_Home
{
    [Area("StoreManager")]
    [Route("/StoreManager")]
    [Authorize]
    public class HomeController : Controller
    {
        [HttpGet("",Name = "storeManDefault")]
        public IActionResult Index()
        {
            return View();
        }
    }
}