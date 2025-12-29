using System.ComponentModel.DataAnnotations;

namespace AddisBookingAdmin.Models
{
    public class Service
    {
        public int Id { get; set; }

        [Required] public int ProviderId { get; set; }
        public Provider Provider { get; set; } = null!;

        [Required] public string Name { get; set; } = string.Empty;
        [Required] public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
