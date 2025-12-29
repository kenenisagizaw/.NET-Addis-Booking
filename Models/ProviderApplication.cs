using System;

namespace AddisBookingAdmin.Models
{
    public enum ApplicationStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class ProviderApplication
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string NationalIdPath { get; set; } = string.Empty;
        public string BusinessLicensePath { get; set; } = string.Empty;
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }
}
