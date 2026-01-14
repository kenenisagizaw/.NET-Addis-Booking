using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AddisBookingAdmin.Models;


// =========================
// HOME CONTROLLER
// Handles main site navigation and error pages
// =========================
namespace AddisBookingAdmin.Controllers;

// Controller for home and informational pages.
public class HomeController : Controller
{
    // Displays the home page.
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    // Displays the error page with request details.
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
