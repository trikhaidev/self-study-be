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
                entity.Property(a => a.Id)
                    .ValueGeneratedOnAdd()
                    .UseIdentityColumn(100,5);

                entity.HasIndex(x => x.Title)
                        .IsUnique(); // Việc thiếp lập Unique sẽ bao gồm luôn cả thiết lập Index

                entity.Property(a => a.Description)
                    .HasMaxLength(500)
                    .HasColumnType("NVARCHAR")
                    .ValueGeneratedOnAddOrUpdate();

                entity.HasOne(a => a.Author)
                       .WithMany(au => au.Articles)
                       .HasForeignKey(a => a.AuthorId)
                       .IsRequired(true)
                       .OnDelete(DeleteBehavior.ClientNoAction); // Chỉ rõ cho EF biết phải làm gì khi migrations Db và xóa data cha
            });
        }
    }
}

/*
- UseIdentityColumn(100,5); => bắt đầu từ 100 và mỗi lần tăng lên 5 đơn vị
                            => thiết lập này không liên quan gì đến index hay unique

- ValueGeneratedOnAdd() => EF hiểu rằng giá trị của cột này là do Db tự sinh nên khi INSERT nếu Entity không có giá trị (null) của cột
                        này thì câu lệnh INSERT được sinh ra sẽ không có cột này. Còn nếu Entity có giá trị (khác null) của cột này thì
                        câu lệnh INSERT sẽ kèm thêm cột này và giá trị của nó.
                        => Khi INSERT xong thì EF sẽ map lại giá trị thực tế của cột này dưới db vào property của entity

- ValueGeneratedOnUpdate() => Lệnh Update được sinh ra sẽ không bao gồm cột này. Dù cho đó có là entity đang được tracking hay không.
                        => Khi UPDATE xong thì EF sẽ tự động đọc và map lại giá trị thực tế của cột này dưới db vào property của entity

- ValueGeneratedOnAddOrUpdate() => Cả 2 lệnh INSERT và UPDATE sẽ không bao gồm cột này. Dù cho nó có value hay không, dù cho entity có đang được tracking
                               hay không.
                               => Tất nhiên là sau khi thực thi xong lệnh, EF sẽ tự động đọc và map lại giá trị thực tế của cột này dưới db vào property của
                               entity
*/
