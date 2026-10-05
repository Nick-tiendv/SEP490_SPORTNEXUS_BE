using Microsoft.EntityFrameworkCore;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Bookings;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Finances;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Facilities;
using SEP490_SPORTNEXUS_BE.Repositories.Enums;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Socials;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Tournaments;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.Identities;
using SEP490_SPORTNEXUS_BE.Repositories.Entities.MasterData;

namespace SEP490_SPORTNEXUS_BE.Repositories
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Court> Courts { get; set; }
        public DbSet<CourtSlot> CourtSlots { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingParticipant> BookingParticipants { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<LfgCard> LfgCards { get; set; }
        public DbSet<LfgParticipant> LfgParticipants { get; set; }
        public DbSet<CommunityPost> CommunityPosts { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentPrize> TournamentPrizes { get; set; }
        public DbSet<TournamentParticipant> TournamentParticipants { get; set; }
        public DbSet<BracketMatch> BracketMatches { get; set; }
        public DbSet<MatchScoreDetail> MatchScoreDetails { get; set; }
        public DbSet<CheckInLog> CheckInLogs { get; set; }
        public DbSet<FairPlayRating> FairPlayRatings { get; set; }




        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<SportCategory> SportCategories { get; set; }
        public DbSet<Amenity> Amenities { get; set; }
        public DbSet<Facility> Facilities { get; set; }
        public DbSet<FacilityImage> FacilityImages { get; set; }
        public DbSet<FacilityAmenity> FacilityAmenities { get; set; }
        public DbSet<FacilityReview> FacilityReviews { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();
            modelBuilder.Entity<Account>()
                .HasIndex(a => a.Username)
                .IsUnique();
            modelBuilder.Entity<Account>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.Restrict);
            
            

            modelBuilder.Entity<Team>()
                .HasOne(t => t.CreatedBy)
                .WithMany()
                .HasForeignKey(t => t.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Team)
                .WithMany(t => t.Members)
                .HasForeignKey(tm => tm.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.Account)
                .WithMany()
                .HasForeignKey(tm => tm.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<FacilityAmenity>()
                .HasKey(fa => new { fa.FacilityId, fa.AmenityId });

            modelBuilder.Entity<Facility>()
                .HasOne(f => f.Owner)
                .WithMany()
                .HasForeignKey(f => f.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FacilityReview>()
                .HasOne(fr => fr.User)
                .WithMany()
                .HasForeignKey(fr => fr.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Court>()
                .HasOne(c => c.Facility)
                .WithMany(f => f.Courts)
                .HasForeignKey(c => c.FacilityId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Court>()
                .HasOne(c => c.Category)
                .WithMany()
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CourtSlot>()
                .Property(c => c.Status)
                .HasConversion<string>();

            modelBuilder.Entity<CourtSlot>()
                .HasOne(c => c.Court)
                .WithMany()
                .HasForeignKey(c => c.CourtId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Booking>()
                .Property(b => b.Status)
                .HasConversion<string>();

            modelBuilder.Entity<BookingParticipant>()
                .Property(bp => bp.PaymentStatus)
                .HasConversion<string>();

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Type)
                .HasConversion<string>();

            modelBuilder.Entity<Wallet>()
                .HasIndex(w => w.UserId)
                .IsUnique();

            modelBuilder.Entity<LfgCard>()
                .Property(c => c.Status)
                .HasConversion<string>();

            modelBuilder.Entity<LfgParticipant>()
                .Property(p => p.Status)
                .HasConversion<string>();

            modelBuilder.Entity<LfgParticipant>()
                .HasOne(p => p.LfgCard)
                .WithMany(c => c.Participants)
                .HasForeignKey(p => p.LfgCardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostComment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tournament>()
                .Property(t => t.CategoryType)
                .HasConversion<string>();

            modelBuilder.Entity<Tournament>()
                .Property(t => t.Status)
                .HasConversion<string>();

            modelBuilder.Entity<TournamentParticipant>()
                .Property(tp => tp.PaymentStatus)
                .HasConversion<string>();
                
            modelBuilder.Entity<BracketMatch>()
                .HasOne(bm => bm.Participant1)
                .WithMany()
                .HasForeignKey(bm => bm.Participant1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BracketMatch>()
                .HasOne(bm => bm.Participant2)
                .WithMany()
                .HasForeignKey(bm => bm.Participant2Id)
                .OnDelete(DeleteBehavior.Restrict);
                
            modelBuilder.Entity<BracketMatch>()
                .HasOne(bm => bm.Winner)
                .WithMany()
                .HasForeignKey(bm => bm.WinnerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = RoleIds.Player, Name = "Player" },
                new Role { Id = RoleIds.Owner, Name = "Owner" },
                new Role { Id = RoleIds.Admin, Name = "Admin" }
            );
        }
    }
}
