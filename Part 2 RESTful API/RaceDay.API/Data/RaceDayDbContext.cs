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

        public DbSet<Admin> Admins { get; set; } = null!;

        public DbSet<Participant> Participants { get; set; } = null!;

        public DbSet<EventType> EventTypes { get; set; } = null!;

        public DbSet<Event> Events { get; set; } = null!;

        public DbSet<EventCategory> EventCategories { get; set; } = null!;

        public DbSet<Enrolment> Enrolments { get; set; } = null!;
    }
}
