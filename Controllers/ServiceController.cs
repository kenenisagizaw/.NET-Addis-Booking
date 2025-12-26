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

        // GET: /Service
        public async Task<IActionResult> Index()
        {
            var providerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var services = await _context.Services
                .Where(s => s.ProviderId == providerId)
                .ToListAsync();
            return View(services);
        }

        // GET: /Service/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id);
            if (service == null) return NotFound();

            var providerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (service.ProviderId != providerId) return Forbid();

            return View(service);
        }

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
            var providerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            service.ProviderId = providerId;

            if (ModelState.IsValid)
            {
                _context.Services.Add(service);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Service created successfully.";
                return RedirectToAction(nameof(Index));
            }

            return View(service);
        }

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
