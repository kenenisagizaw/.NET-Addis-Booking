// Path: Controllers/ProviderController.cs

using AddisBookingAdmin.Data;
using AddisBookingAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AddisBookingAdmin.Controllers
{
    [ApiController]
    [Route("api/provider")]
    public class ProviderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProviderController(AppDbContext context)
        {
            _context = context;
        }

        // User applies to become provider
        [HttpPost("apply")]
        [Authorize]
        public async Task<IActionResult> Apply([FromBody] ProviderOnboardDto model)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return Unauthorized();
            if (user.IsProvider) return BadRequest("You have already applied.");

            user.IsProvider = true;
            user.ProviderApproved = false;
            user.NationalIdDocUrl = model.NationalIdDocUrl;
            user.BusinessDocUrl = model.BusinessDocUrl;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Provider application submitted. Waiting for admin approval." });
        }

        // Admin: list providers
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllProviders()
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

            return Ok(providers);
        }

        // Admin: approve provider
        [HttpPost("approve/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ApproveProvider(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || !user.IsProvider) return NotFound();

            user.ProviderApproved = true;
            user.Role = UserRole.Provider;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Provider approved successfully." });
        }

        // Admin: deny provider
        [HttpPost("deny/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DenyProvider(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || !user.IsProvider) return NotFound();

            user.IsProvider = false;
            user.ProviderApproved = false;
            user.Role = UserRole.Customer;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Provider denied." });
        }
    }
}
