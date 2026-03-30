using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ReviewEntityFrameworkCore.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Article> Articles { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder => {
                builder
                    .AddFilter(DbLoggerCategory.Database.Command.Name, LogLevel.Information)
                    .AddFilter(DbLoggerCategory.Query.Name, LogLevel.Information)
                    .AddConsole();
            });
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=ReviewEntityFrameworkCore;Trusted_Connection=True;TrustServerCertificate=True");
            optionsBuilder.UseLoggerFactory(loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Article>(entity =>
            {
                entity.HasOne(a => a.Author)
                       .WithMany(au => au.Articles)
                       .HasForeignKey(a => a.AuthorId)
                       .IsRequired(true)
                       .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
