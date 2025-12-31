using AddisBookingAdmin.Models;
using Microsoft.EntityFrameworkCore;

namespace AddisBookingAdmin.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Provider> Providers => Set<Provider>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<ProviderApplication> ProviderApplications => Set<ProviderApplication>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Enum as int
            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<int>();

            // User ↔ Provider (1–1)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Provider)
                .WithOne(p => p.User)
                .HasForeignKey<Provider>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Provider ↔ Services (1–many)
            modelBuilder.Entity<Provider>()
                .HasMany(p => p.Services)
                .WithOne(s => s.Provider)
                .HasForeignKey(s => s.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
