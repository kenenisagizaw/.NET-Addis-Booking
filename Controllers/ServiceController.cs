using AddisBookingAdmin.Data;
using AddisBookingAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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

        // 🔑 Helper: Get ProviderId from logged-in User
        private async Task<int?> GetProviderIdAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim))
                return null;

            int userId = int.Parse(userIdClaim);

            var provider = await _context.Providers
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);

            return provider?.Id;
        }

        // ===============================
        // GET: /Service
        // ===============================
        public async Task<IActionResult> Index()
        {
            var providerId = await GetProviderIdAsync();
            if (providerId == null) return Forbid();

            var services = await _context.Services
                .Where(s => s.ProviderId == providerId)
                .ToListAsync();

            return View(services);
        }

        // ===============================
        // GET: /Service/Details/5
        // ===============================
        public async Task<IActionResult> Details(int id)
        {
            var providerId = await GetProviderIdAsync();
            if (providerId == null) return Forbid();

            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == providerId);

            if (service == null) return NotFound();

            return View(service);
        }

        // ===============================
        // GET: /Service/Create
        // ===============================
        public IActionResult Create()
        {
            return View();
        }

        // ===============================
        // POST: /Service/Create
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Service service)
        {
            var providerId = await GetProviderIdAsync();
            if (providerId == null) return Forbid();

            // ❗IMPORTANT: Provider is NOT posted from the form
            service.ProviderId = providerId.Value;

            // Remove navigation validation error
            ModelState.Remove("Provider");

            if (!ModelState.IsValid)
                return View(service);

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // GET: /Service/Edit/5
        // ===============================
        public async Task<IActionResult> Edit(int id)
        {
            var providerId = await GetProviderIdAsync();
            if (providerId == null) return Forbid();

            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == providerId);

            if (service == null) return NotFound();

            return View(service);
        }

        // ===============================
        // POST: /Service/Edit/5
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Service service)
        {
            var providerId = await GetProviderIdAsync();
            if (providerId == null) return Forbid();

            var existingService = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == providerId);

            if (existingService == null) return NotFound();

            ModelState.Remove("Provider");

            if (!ModelState.IsValid)
                return View(service);

            existingService.Name = service.Name;
            existingService.Price = service.Price;
            existingService.Description = service.Description;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // POST: /Service/Delete/5
        // ===============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var providerId = await GetProviderIdAsync();
            if (providerId == null) return Forbid();

            var service = await _context.Services
                .FirstOrDefaultAsync(s => s.Id == id && s.ProviderId == providerId);

            if (service == null) return NotFound();

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
