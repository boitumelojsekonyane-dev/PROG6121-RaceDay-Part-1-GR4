
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
        }
    }
}

