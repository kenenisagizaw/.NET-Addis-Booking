using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AddisBookingAdmin.Models
{
    public class Provider
    {
        public int Id { get; set; }

        // FK to User
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [Required] public string FullName { get; set; } = string.Empty;
        [Required] public string BusinessName { get; set; } = string.Empty;
        [Required] public string Phone { get; set; } = string.Empty;

        // Uploaded docs
        public string NationalIdPath { get; set; } = string.Empty;
        public string BusinessLicensePath { get; set; } = string.Empty;

        public bool IsApproved { get; set; } = false;
        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Service> Services { get; set; } = new List<Service>();
    }
}
