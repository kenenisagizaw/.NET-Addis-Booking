using AddisBookingAdmin.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AddisBookingAdmin.Services
{
    // Service for generating JWT tokens
    public class JwtService
    {
        private readonly IConfiguration _config; // App configuration

        // Constructor: injects configuration
        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        // Generates a JWT token for the given user
        public string GenerateToken(User user)
        {
            // Define claims for the token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User ID
                new Claim(ClaimTypes.Role, user.Role.ToString()), // User role
                new Claim(ClaimTypes.Email, user.Email) // User email
            };

            // Create security key from config
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            // Create signing credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Build the JWT token
            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1), // Token expires in 1 hour
                signingCredentials: creds
            );

            // Return the token string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
