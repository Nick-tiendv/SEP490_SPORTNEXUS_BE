using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories.Entities;

namespace SEP490_SPORTNEXUS_BE.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Court> Courts { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Court)
                .WithMany()
                .HasForeignKey(b => b.CourtId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Player)
                .WithMany()
                .HasForeignKey(b => b.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
