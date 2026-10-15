
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Identity;

namespace RaceDay.API.Data
{
    public class RaceDayIdentityDbContext
        : IdentityDbContext<RaceDayUser, IdentityRole, string>
    {
        public RaceDayIdentityDbContext(
            DbContextOptions<RaceDayIdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(
            ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // A participant profile can belong to
            // only one Identity account.
            builder.Entity<RaceDayUser>()
                .HasIndex(u => u.ParticipantID)
                .IsUnique()
                .HasFilter("[ParticipantID] IS NOT NULL");

            // An organiser profile can belong to
            // only one Identity account.
            builder.Entity<RaceDayUser>()
                .HasIndex(u => u.AdminID)
                .IsUnique()
                .HasFilter("[AdminID] IS NOT NULL");

            // Every account must link to either
            // a participant or an organiser,
            // but never both.
            builder.Entity<RaceDayUser>()
                .ToTable("AspNetUsers", table =>
                {
                    table.HasCheckConstraint(
                        "CK_AspNetUsers_ProfileLink",
                        "([ParticipantID] IS NOT NULL AND [AdminID] IS NULL) OR " +
                        "([ParticipantID] IS NULL AND [AdminID] IS NOT NULL)");
                });
        }
    }
}
