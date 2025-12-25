using AddisBookingAdmin.Data;
using AddisBookingAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AddisBookingAdmin.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        // View all provider applications
        [HttpGet("provider-applications")]
        public async Task<IActionResult> GetApplications()
        {
            var apps = await _context.ProviderApplications
                .Include(p => p.User)
                .ToListAsync();

            return Ok(apps);
        }

        [HttpGet("provider-applications/{id}")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> GetOne(int id)
{
    var app = await _context.ProviderApplications
        .Include(p => p.User)
        .FirstOrDefaultAsync(p => p.Id == id);

    if (app == null)
        return NotFound();

    return Ok(app);
}


        // Approve / Reject provider
        [HttpPost("provider-applications/{id}/decision")]
        public async Task<IActionResult> Decide(int id, bool approve)
        {
            var app = await _context.ProviderApplications
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (app == null)
                return NotFound();

            app.Status = approve ? ApplicationStatus.Approved : ApplicationStatus.Rejected;

            if (approve)
            {
                app.User.IsProvider = true;
                app.User.ProviderApproved = true;
                app.User.Role = UserRole.Provider;
            }

            await _context.SaveChangesAsync();
            return Ok("Decision saved");
        }
    }
}
