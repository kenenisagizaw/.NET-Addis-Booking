namespace AddisBookingAdmin.Models
{
    public class Service
    {
        public int Id { get; set; }

        public int ProviderId { get; set; }
        public User Provider { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
