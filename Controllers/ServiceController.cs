using AddisBookingAdmin.Data;
using AddisBookingAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;

namespace AddisBookingAdmin.Controllers
{
    [Authorize(Roles = "Provider")]
    public class ServiceController : Controller
    {
        private readonly AppDbContext _context;

        public ServiceController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // LIST ALL SERVICES (INDEX)
        // =========================
        // GET: /Service
public async Task<IActionResult> Index()
{
    // Get the logged-in user's ID
    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Find the Provider entity for this user
    var provider = await _context.Providers
        .Include(p => p.Services) // Include related services
        .FirstOrDefaultAsync(p => p.UserId == userId);

    if (provider == null)
    {
        // If no provider found for this user, return empty list or forbid
        return Forbid();
    }

    // Pass the services to the view
    return View(provider.Services);
}


        // =========================
        // DETAILS OF A SERVICE
        // =========================
        // GET: /Service/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            var providerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (service.ProviderId != providerId) return Forbid();

            return View(service);
        }

        // =========================
        // CREATE NEW SERVICE
        // =========================
        // GET: /Service/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Service/Create
 [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(Service service)
{
    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Get the provider for the current user
    var provider = await _context.Providers.FirstOrDefaultAsync(p => p.UserId == userId);
    if (provider == null) 
        return Forbid("You are not registered as a provider yet.");

    service.ProviderId = provider.Id;

    if (!ModelState.IsValid)
    {
        var errors = ModelState.Values.SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();
        return Content("Validation errors: " + string.Join(", ", errors));
    }

    _context.Services.Add(service);
    await _context.SaveChangesAsync();
    TempData["Message"] = "Service created successfully.";

    return RedirectToAction(nameof(Index));
}



        // =========================
        // EDIT EXISTING SERVICE
        // =========================
        // GET: /Service/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            var providerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (service.ProviderId != providerId) return Forbid();

            return View(service);
        }

        // POST: /Service/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            if (id != service.Id) return BadRequest();

            var existingService = await _context.Services.FindAsync(id);
            if (existingService == null) return NotFound();

            var providerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (existingService.ProviderId != providerId) return Forbid();

            if (ModelState.IsValid)
            {
                existingService.Name = service.Name;
                existingService.Price = service.Price;
                existingService.Description = service.Description;

                await _context.SaveChangesAsync();
                TempData["Message"] = "Service updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(service);
        }

        // =========================
        // DELETE SERVICE
        // =========================
        // POST: /Service/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null) return NotFound();

            var providerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (service.ProviderId != providerId) return Forbid();

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();
            TempData["Message"] = "Service deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}
