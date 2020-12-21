using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using WpfApp1.Models;

namespace WpfApp1.Data
{
    /// <summary>
    ///  设计情况下 确保执行migration 命令有效
    /// </summary>
    public class EFDbContextFactory : IDesignTimeDbContextFactory<EFDbContext>
    {
        public EFDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsetting.json", false, true).Build();

            var optionsBuilder = new DbContextOptionsBuilder<EFDbContext>();
            var connectionStr = config.GetConnectionString(nameof(EFDbContext));
            optionsBuilder.UseSqlite(connectionStr);
            return new EFDbContext(optionsBuilder.Options);
        }
    }

    public class EFDbContext : DbContext
    {
        public EFDbContext(DbContextOptions<EFDbContext> opt)
            : base(opt)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(new Category[] {
                  new Category{  CategoryId=1, Name="水果"},
                  new Category{  CategoryId=2, Name="蔬菜"}
            });

            modelBuilder.Entity<Product>().HasData(new Product[] {
                new Product{ ProductId=1, Name="苹果", CategoryId=1 },
                new Product{ ProductId=2, Name="香蕉", CategoryId=1 },
                new Product{ ProductId=3, Name="土豆", CategoryId=2 },
            });
           
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categorys { get; set; }
        public DbSet<Sale> Sales { get; set; }
    }

    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Day { get; set; }
        public double Price { get; set; }
        public string Address { get; set; }
        public int CategoryId { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
        public virtual Category Category { get; set; }
    }
    public class Category
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Product> Products { get; private set; } = new ObservableCollection<Product>();
    }

    public class Sale
    {
        public int SaleId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public int SaleCount { get; set; }
        public DateTime SaleDate { get; set; }
    }

}
