using JwtAuthentication.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthentication.Database;

public class AppDbContext : DbContext
{
    public DbSet<AppUser> Users { get; set; }
    public DbSet<AppRole> Roles { get; set; }
    public DbSet<AppUserRole> UserRoles { get; set; }
    public DbSet<AppJwtKey> JwtKeys {get;set;}
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppUserRole>(entity =>
        {
            entity.ToTable("UserRole");
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            entity.HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ur => ur.Role)
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<AppUser>(entity =>
        {
            entity.HasIndex(u => u.UserName)
                .IsUnique();
        });

        modelBuilder.Entity<AppJwtKey>(entity =>
        {
            entity.ToTable("JwtKey");
            entity.HasKey(k => k.Id);

            entity.Property(k => k.KeyId)
                .IsRequired()
                .HasColumnType("VARCHAR")
                .HasMaxLength(250);

            entity.Property(k => k.PublicKey)
                .IsRequired()
                .HasColumnType("NVARCHAR(MAX)");

            entity.Property(k => k.IsActive)
                .IsRequired()
                .HasColumnType("NVARCHAR(MAX)");

            entity.Property(k => k.IsActive)
                .IsRequired();
            
            entity.Property(k => k.Exp)
                .IsRequired();
        });

        FakeDataAppUser(modelBuilder);
        FakeDataAppRole(modelBuilder);
        FakeDataAppUserRole(modelBuilder);
    }

    private void FakeDataAppUser(ModelBuilder modelBuilder){
        modelBuilder.Entity<AppUser>().HasData(
            new AppUser
            {
                Id = 1,
                FullName = "Nguyễn Văn An",
                BirthDay = new DateOnly(1995, 5, 12),
                Address = "Hà Nội",
                UserName = "nguyenvanan",
                Password = "123456"
            },
            new AppUser
            {
                Id = 2,
                FullName = "Trần Thị Bình",
                BirthDay = new DateOnly(1997, 8, 20),
                Address = "TP Hồ Chí Minh",
                UserName = "tranthibinh",
                Password = "123456"
            },
            new AppUser
            {
                Id = 3,
                FullName = "Lê Văn Cường",
                BirthDay = new DateOnly(1993, 3, 15),
                Address = "Đà Nẵng",
                UserName = "levancuong",
                Password = "123456"
            },
            new AppUser
            {
                Id = 4,
                FullName = "Phạm Thị Dung",
                BirthDay = new DateOnly(1999, 11, 2),
                Address = "Cần Thơ",
                UserName = "phamthidung",
                Password = "123456"
            },
            new AppUser
            {
                Id = 5,
                FullName = "Hoàng Văn Em",
                BirthDay = new DateOnly(1990, 1, 25),
                Address = "Hải Phòng",
                UserName = "hoangvanem",
                Password = "123456"
            },
            new AppUser
            {
                Id = 6,
                FullName = "Vũ Thị Hoa",
                BirthDay = new DateOnly(1996, 6, 30),
                Address = "Nam Định",
                UserName = "vuthihoa",
                Password = "123456"
            },
            new AppUser
            {
                Id = 7,
                FullName = "Đặng Văn Khoa",
                BirthDay = new DateOnly(1994, 9, 18),
                Address = "Bình Dương",
                UserName = "dangvankhoa",
                Password = "123456"
            },
            new AppUser
            {
                Id = 8,
                FullName = "Bùi Thị Lan",
                BirthDay = new DateOnly(2000, 12, 5),
                Address = "Nghệ An",
                UserName = "buithilan",
                Password = "123456"
            },
            new AppUser
            {
                Id = 9,
                FullName = "Phan Văn Minh",
                BirthDay = new DateOnly(1992, 4, 10),
                Address = "Khánh Hòa",
                UserName = "phanvanminh",
                Password = "123456"
            },
            new AppUser
            {
                Id = 10,
                FullName = "Đỗ Thị Ngọc",
                BirthDay = new DateOnly(1998, 7, 22),
                Address = "Quảng Ninh",
                UserName = "dothingoc",
                Password = "123456"
            }
        );
    }

    private void FakeDataAppRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppRole>().HasData(
            new AppRole
            {
                Id = 1,
                Name = "Admin"
            },
            new AppRole
            {
                Id = 2,
                Name = "Manager"
            },
            new AppRole
            {
                Id = 3,
                Name = "Staff"
            },
            new AppRole
            {
                Id = 4,
                Name = "User"
            },
            new AppRole
            {
                Id = 5,
                Name = "Guest"
            }
        );
    }

    private void FakeDataAppUserRole(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AppUserRole>().HasData(
            new AppUserRole { UserId = 1, RoleId = 1 },
            new AppUserRole { UserId = 1, RoleId = 2 },
            new AppUserRole { UserId = 1, RoleId = 3 },

            new AppUserRole { UserId = 2, RoleId = 2 },
            new AppUserRole { UserId = 2, RoleId = 3 },

            new AppUserRole { UserId = 3, RoleId = 3 },

            new AppUserRole { UserId = 4, RoleId = 4 },

            new AppUserRole { UserId = 5, RoleId = 5 },

            new AppUserRole { UserId = 6, RoleId = 1 },
            new AppUserRole { UserId = 6, RoleId = 2 },

            new AppUserRole { UserId = 7, RoleId = 2 },
            new AppUserRole { UserId = 7, RoleId = 3 },
            new AppUserRole { UserId = 7, RoleId = 4 },

            new AppUserRole { UserId = 8, RoleId = 4 }
        );
    }
}