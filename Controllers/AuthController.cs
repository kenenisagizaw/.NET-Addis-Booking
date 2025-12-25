using AddisBookingAdmin.Data;
using AddisBookingAdmin.Models;
using AddisBookingAdmin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AddisBookingAdmin.Models.DTOs;


namespace AddisBookingAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordService _passwordService;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, PasswordService passwordService, JwtService jwtService)
        {
            _context = context;
            _passwordService = passwordService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest("Email already exists");

            var user = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Role = UserRole.Customer, // default role
                CreatedAt = DateTime.UtcNow
            };

            // Hash the password
            _passwordService.HashPassword(user, dto.Password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return Unauthorized("Invalid credentials");

            if (!_passwordService.VerifyPassword(user, dto.Password))
                return Unauthorized("Invalid credentials");

            var token = _jwtService.Generate(user); // ✅ Works with updated JwtService

            return Ok(new { Token = token });
        }

        [HttpGet("protected")]
        public IActionResult ProtectedRoute()
        {
            return Ok("You are authorized to access this route!");
        }
    }
}
