using Microsoft.EntityFrameworkCore;

using WorldCities.Data.Models;

namespace WorldCities.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            :base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ApplicationDbContext).Assembly);
        }
        public DbSet<City>  Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
    }
}
