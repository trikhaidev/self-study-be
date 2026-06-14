using JwtAuth.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtAuth.Database;
public class AppDbContext : DbContext
{
    public virtual DbSet<User> Users {get;set;}
    public virtual DbSet<Role> Roles {get;set;}
    public virtual DbSet<UserRole> UserRoles {get;set;}
    public virtual DbSet<JwtKey> JwtKeys {get;set;}
    public virtual DbSet<RefreshToken> RefreshTokens {get;set;}
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(x => new
            {
                x.UserId,
                x.RoleId,
            });
        });

        modelBuilder.Entity<JwtKey>(entity =>
        {
            entity.Property(x => x.IsActive)
                .HasDefaultValue(false);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.Property(x => x.IsActive)
                    .HasDefaultValue(false);
        });
    }
}