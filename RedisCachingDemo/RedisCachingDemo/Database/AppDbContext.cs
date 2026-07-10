using Microsoft.EntityFrameworkCore;
using RedisCachingDemo.Database.Entities;

namespace RedisCachingDemo.Database
{
    public class AppDbContext:DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public  DbSet<City> Cities { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
