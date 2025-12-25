using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AddisBookingAdmin.Controllers
{
    [Route("api/test")]
    [ApiController] // Important for API endpoints
    public class TestController : ControllerBase
    {
        // Open route
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("This is a public endpoint.");
        }

        // Protected route, requires JWT auth
        [HttpGet("protected")]
        [Authorize] // JWT authentication required
        public IActionResult Protected()
        {
            return Ok("You accessed a protected route!");
        }
    }
}
