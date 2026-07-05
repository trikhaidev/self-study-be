using InMemoryCachingDemo.Database.Entites;
using Microsoft.EntityFrameworkCore;

namespace InMemoryCachingDemo.Database
{
    public class AppDbContext:DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Country>(entity =>
            {
                entity.HasMany(c => c.Cities)
                        .WithOne(c => c.Country)
                        .HasForeignKey(c => c.CountryId)
                        .IsRequired()
                        .OnDelete(DeleteBehavior.NoAction);

                entity.Property(c => c.NameOfVietNamese)
                        .HasDefaultValue("--");
            });
        }
    }
}
