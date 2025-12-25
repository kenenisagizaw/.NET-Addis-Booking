using AddisBookingAdmin.Data;
using AddisBookingAdmin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AddisBookingAdmin.Controllers
{
    [ApiController]
    [Route("api/services")]
    [Authorize(Roles = "Provider")]
    public class ServiceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ServiceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Service service)
        {
            var providerId = int.Parse(User.FindFirst("sub")!.Value);
            service.ProviderId = providerId;

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return Ok(service);
        }
    }
}
