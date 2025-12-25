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

        public string NationalIdDocUrl { get; set; } = string.Empty;
        public string BusinessDocUrl { get; set; } = string.Empty;

        public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }
}
