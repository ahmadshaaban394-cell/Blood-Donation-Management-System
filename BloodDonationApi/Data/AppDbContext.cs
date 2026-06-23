using BloodDonationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodDonationAPI.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<BloodStock> BloodStocks { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=AHMED-LAPTOP\\SQLEXPRESS;Database=BloodDonationDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}