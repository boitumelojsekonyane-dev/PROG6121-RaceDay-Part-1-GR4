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

        public DbSet<Admin> Admins { get; set; }
    }
}
