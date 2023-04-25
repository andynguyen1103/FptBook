using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FptBook.Models;
using Microsoft.AspNetCore.Identity;

namespace FptBook.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<FptBookUser> _userManager;
    private readonly SignInManager<FptBookUser> _signInManager;
    public HomeController(ILogger<HomeController> logger, UserManager<FptBookUser> userManager, SignInManager<FptBookUser> signInManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
    }
    [HttpGet(Name = "default")]
    public async Task<IActionResult> Index()
    {
        if (!_signInManager.IsSignedIn(HttpContext.User))
        {
            return View();
        }
        var user = await _userManager.GetUserAsync(HttpContext.User);
        if (await _userManager.IsInRoleAsync(user,RoleNames.Customer))
        {
            return View();
        }

        if (await _userManager.IsInRoleAsync(user,RoleNames.StoreManager))
        {
            return RedirectToRoute("storeManDefault");
        }
        
        if (await _userManager.IsInRoleAsync(user,RoleNames.Administrator))
        {
            return RedirectToRoute("adminDefault");
        }

        return View();

    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}