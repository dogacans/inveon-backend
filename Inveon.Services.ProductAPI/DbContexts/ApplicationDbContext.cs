using Inveon.Models;
using Microsoft.EntityFrameworkCore;

namespace Inveon.Services.ProductAPI.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Category>().HasData(
                new Category { CategoryID = 1, CategoryName = "Pants" },
                new Category { CategoryID = 2, CategoryName = "Shirts" },
                new Category { CategoryID = 3, CategoryName = "T-shirts" },
                new Category { CategoryID = 4, CategoryName = "Skirts" },
                new Category { CategoryID = 5, CategoryName = "Dresses" },
                new Category { CategoryID = 6, CategoryName = "Hoodies" },
                new Category { CategoryID = 7, CategoryName = "Jackets" }
            );
            modelBuilder.Entity<Brand>().HasData(
                new Brand { BrandId = 1, Name = "Zara" },
                new Brand { BrandId = 2, Name = "Mavi" },
                new Brand { BrandId = 3, Name = "Colins" },
                new Brand { BrandId = 4, Name = "Bershka" },
                new Brand { BrandId = 5, Name = "Oysho" },
                new Brand { BrandId = 6, Name = "H&M" },
                new Brand { BrandId = 7, Name = "Defacto" },
                new Brand { BrandId = 8, Name = "Decathlon" }
            );
            
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductId = 1, Color = "Green", Cut = "Skinny", Name = "Blue Jeans", Price = 39.99, CurrencyType = "TRY", Description = "Classic blue jeans", CategoryID = 1, BrandID = 1, Gender = "Unisex", Material = "Denim", Style = "Casual", Season = "All-season", ImageUrls = "jeans.jpg", DateAdded = DateTime.Now.ToString() },
                new Product { ProductId = 2, Color = "Yellow", Cut = "Slim-fit", Name = "White T-shirt", Price = 24.99, CurrencyType = "TRY", Description = "Plain white cotton T-shirt", CategoryID = 3, BrandID = 2, Gender = "Unisex", Material = "Cotton", Style = "Basic", Season = "Summer", ImageUrls = "tshirt.jpg", DateAdded = DateTime.Now.ToString() },
                new Product { ProductId = 3, Color = "Red", Cut = "Mini", Name = "Black Skirt", Price = 29.99, CurrencyType = "TRY", Description = "Knee-length black skirt", CategoryID = 4, BrandID = 3, Gender = "Female", Material = "Polyester", Style = "Formal", Season = "All-season", ImageUrls = "skirt.jpg", DateAdded = DateTime.Now.ToString() },
                new Product { ProductId = 4, Color = "Black", Cut = "Oversize", Name = "Hooded Sweatshirt", Price = 45.99, CurrencyType = "TRY", Description = "Warm hooded sweatshirt", CategoryID = 6, BrandID = 1, Gender = "Unisex", Material = "Fleece", Style = "Casual", Season = "Winter", ImageUrls = "hoodie.jpg", DateAdded = DateTime.Now.ToString() }
            );

            modelBuilder.Entity<ProductStock>().HasData(
                new ProductStock { ProductStockId = 1, ProductId = 1, Size = "S", StockQuantity = 5 },
                new ProductStock { ProductStockId = 2, ProductId = 1, Size = "M", StockQuantity = 5 },
                new ProductStock { ProductStockId = 3, ProductId = 1, Size = "L", StockQuantity = 5 },
                new ProductStock { ProductStockId = 4, ProductId = 2, Size = "S", StockQuantity = 5 },
                new ProductStock { ProductStockId = 5, ProductId = 2, Size = "M", StockQuantity = 5 },
                new ProductStock { ProductStockId = 6, ProductId = 2, Size = "L", StockQuantity = 5 },
                new ProductStock { ProductStockId = 7, ProductId = 3, Size = "S", StockQuantity = 5 },
                new ProductStock { ProductStockId = 8, ProductId = 3, Size = "M", StockQuantity = 5 },
                new ProductStock { ProductStockId = 9, ProductId = 3, Size = "L", StockQuantity = 5 },
                new ProductStock { ProductStockId = 10, ProductId = 4, Size = "S", StockQuantity = 5 },
                new ProductStock { ProductStockId = 11, ProductId = 4, Size = "M", StockQuantity = 5 },
                new ProductStock { ProductStockId = 12, ProductId = 4, Size = "L", StockQuantity = 5 }
                );


        }
    }
}
