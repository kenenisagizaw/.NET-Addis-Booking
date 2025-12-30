using System.ComponentModel.DataAnnotations;

namespace AddisBookingAdmin.Models
{
   public class Service
{
    public int Id { get; set; }

    public int ProviderId { get; set; } 
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public decimal Price { get; set; }

    [Required]
    public string Description { get; set; } = null!;

    // Navigation property
    public Provider? Provider { get; set; }  

}
}