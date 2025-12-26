
namespace AddisBookingAdmin.Models.DTOs
{
    public class RegisterDto
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
         public string ConfirmPassword { get; set; } = string.Empty;
    }
}
