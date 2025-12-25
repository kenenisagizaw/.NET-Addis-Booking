namespace AddisBookingAdmin.Models
{
    public class Availability
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public bool IsBooked { get; set; } = false;
    }
}
