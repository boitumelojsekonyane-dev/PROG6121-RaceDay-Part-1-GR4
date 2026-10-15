
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Models;

namespace RaceDay.API.Data
{
    public class RaceDayDbContext : DbContext
    {
        public RaceDayDbContext(
            DbContextOptions<RaceDayDbContext> options)
            : base(options)
        {
        }

        // All nine database tables
        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Participant> Participants { get; set; } = null!;
        public DbSet<EventType> EventTypes { get; set; } = null!;
        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<EventCategory> EventCategories { get; set; } = null!;
        public DbSet<Enrolment> Enrolments { get; set; } = null!;

        // Full namespace of the routing class prevents "Route" keyword from conflicting with the database and VS.
        public DbSet<RaceDay.API.Models.Route> Routes { get; set; } = null!;

        public DbSet<Weather> Weather { get; set; } = null!;
        public DbSet<Result> Results { get; set; } = null!;

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // EVENTS
            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.DistanceKm)
                    .HasPrecision(6, 2);

                entity.Property(e => e.EntryFee)
                    .HasPrecision(10, 2)
                    .HasDefaultValue(0.00m);

                entity.Property(e => e.StatusOfEvent)
                    .HasDefaultValue("UPCOMING!");
            });

            // EVENT CATEGORIES
            modelBuilder.Entity<EventCategory>(entity =>
            {
                entity.Property(e => e.DistanceKm)
                    .HasPrecision(6, 2);
            });

            // ENROLMENTS
            modelBuilder.Entity<Enrolment>(entity =>
            {
                entity.Property(e => e.AmountPaid)
                    .HasPrecision(10, 2)
                    .HasDefaultValue(0.00m);

                entity.Property(e => e.EventStatus)
                    .HasDefaultValue("Registered");

                entity.Property(e => e.PaymentStatus)
                    .HasDefaultValue("Pending");

                entity.Property(e => e.EnrolmentDate)
                    .HasDefaultValueSql("SYSDATETIME()");
            });

            // ROUTES
            modelBuilder.Entity<RaceDay.API.Models.Route>(entity =>
            {
                entity.Property(e => e.RouteStartKm)
                    .HasPrecision(6, 2);

                entity.Property(e => e.RouteFinishKm)
                    .HasPrecision(6, 2);
            });

            // WEATHER
            modelBuilder.Entity<Weather>(entity =>
            {
                entity.Property(e => e.TemperatureC)
                    .HasPrecision(5, 2);

                entity.Property(e => e.CapturedAt)
                    .HasDefaultValueSql("SYSDATETIME()");
            });

            // RESULTS
            modelBuilder.Entity<Result>(entity =>
            {
                entity.Property(e => e.PaceMinKm)
                    .HasPrecision(6, 2);

                entity.Property(e => e.CapturedAt)
                    .HasDefaultValueSql("SYSDATETIME()");
            });

            // PARTICIPANTS
            modelBuilder.Entity<Participant>(entity =>
            {
                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("SYSDATETIME()");
            });

            // EVENT TYPES
            modelBuilder.Entity<EventType>(entity =>
            {
                entity.Property(e => e.IsActive)
                    .HasDefaultValue(true);
            });
        }
    }
}
