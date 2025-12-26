using AddisBookingAdmin.Data;
using AddisBookingAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AddisBookingAdmin.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalProviders = await _context.Users.CountAsync(u => u.IsProvider);
            var totalApprovedProviders = await _context.Users.CountAsync(u => u.ProviderApproved);
            var totalServices = await _context.Services.CountAsync();

            var model = new
            {
                TotalUsers = totalUsers,
                TotalProviders = totalProviders,
                TotalApprovedProviders = totalApprovedProviders,
                TotalServices = totalServices
            };

            return View(model);
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.Role,
                    u.IsProvider,
                    u.ProviderApproved,
                    u.CreatedAt
                })
                .ToListAsync();

            return View(users);
        }

        // POST: /Admin/BlockUser/5
        [HttpPost]
        public async Task<IActionResult> BlockUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            await _context.SaveChangesAsync();

            TempData["Message"] = user.IsActive ? "User unblocked" : "User blocked";
            return RedirectToAction(nameof(Users));
        }

        // GET: /Admin/PendingProviders
        public async Task<IActionResult> PendingProviders()
        {
            var providers = await _context.Users
                .Where(u => u.IsProvider && !u.ProviderApproved)
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.NationalIdDocUrl,
                    u.BusinessDocUrl,
                    u.CreatedAt
                })
                .ToListAsync();

            return View(providers);
        }
    }
}
