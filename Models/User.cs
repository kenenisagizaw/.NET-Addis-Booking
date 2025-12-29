using System;
using System.ComponentModel.DataAnnotations;

namespace AddisBookingAdmin.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required] public string FullName { get; set; } = string.Empty;
        [Required] public string Email { get; set; } = string.Empty;
        [Required] public string PasswordHash { get; set; } = string.Empty;

        // Store Role as int in the database
        public UserRole Role { get; set; } = UserRole.Customer;

        public bool IsProvider => Role == UserRole.Provider;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string PasswordSalt { get; set; } = string.Empty;

        // Navigation property for one-to-one relationship with Provider
        public Provider? Provider { get; set; }
    }

    public enum UserRole
    {
        Customer,
        Provider,
        Admin
    }
}
