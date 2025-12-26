using AddisBookingAdmin.Data;
using AddisBookingAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AddisBookingAdmin.Controllers
{
    [Authorize]
    public class ProviderController : Controller
    {
        private readonly AppDbContext _context;

        public ProviderController(AppDbContext context)
        {
            _context = context;
        }


        // GET: /Provider/Apply
        [Authorize(Roles = "Customer")]
        public IActionResult Apply()
        {
            return View();
        }

        // POST: /Provider/Apply
        [Authorize(Roles = "Customer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Apply(ProviderOnboardDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return Unauthorized();
            if (user.IsProvider)
            {
                TempData["Error"] = "You have already applied.";
                return RedirectToAction(nameof(Apply));
            }

            user.IsProvider = true;
            user.ProviderApproved = false;
            user.NationalIdDocUrl = model.NationalIdDocUrl;
            user.BusinessDocUrl = model.BusinessDocUrl;

            await _context.SaveChangesAsync();

            TempData["Message"] = "Provider application submitted. Waiting for admin approval.";
            return RedirectToAction("Index", "Home");
        }

        // Admin: List all providers
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AllProviders()
        {
            var providers = await _context.Users
                .Where(u => u.IsProvider)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.ProviderApproved,
                    u.NationalIdDocUrl,
                    u.BusinessDocUrl
                })
                .ToListAsync();

            return View(providers);
        }

        // Admin: Approve provider
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveProvider(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || !user.IsProvider) return NotFound();

            user.ProviderApproved = true;
            user.Role = UserRole.Provider;

            await _context.SaveChangesAsync();

            TempData["Message"] = "Provider approved successfully.";
            return RedirectToAction(nameof(AllProviders));
        }

        // Admin: Deny provider
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DenyProvider(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || !user.IsProvider) return NotFound();

            user.IsProvider = false;
            user.ProviderApproved = false;
            user.Role = UserRole.Customer;

            await _context.SaveChangesAsync();

            TempData["Message"] = "Provider denied.";
            return RedirectToAction(nameof(AllProviders));
        }
    }
}
