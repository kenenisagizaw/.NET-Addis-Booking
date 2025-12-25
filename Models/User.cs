using System.ComponentModel.DataAnnotations;

namespace AddisBookingAdmin.Models
{
    public enum UserRole
    {
        Customer,
        Provider,
        Admin
    }

    public class User
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
       [EnumDataType(typeof(UserRole))]
        public UserRole Role { get; set; } = UserRole.Customer;

        public bool IsActive { get; set; } = true; // for blocking or soft-delete

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsProvider { get; set; } = false;
        public bool ProviderApproved { get; set; } = false;
        public string? NationalIdDocUrl { get; set; }
        public string? BusinessDocUrl { get; set; }
        public string PasswordSalt { get; set; } = null!;

        // Relationships
        public ICollection<Service>? Services { get; set; }
        public ProviderApplication? ProviderApplication { get; set; }
    
     
}}
