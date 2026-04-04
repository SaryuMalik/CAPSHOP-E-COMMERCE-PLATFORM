using CapShop.CatalogService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CapShop.CatalogService.Infrastructure.Persistence;

public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<CartItem>()
            .Property(p => p.ProductPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Category>().HasData(
    new Category { Id = 1, Name = "Electronics", Description = "Gadgets & devices" },
    new Category { Id = 2, Name = "Clothing", Description = "Fashion & apparel" },
    new Category { Id = 3, Name = "Books", Description = "Books & magazines" },
    new Category { Id = 4, Name = "Home & Kitchen", Description = "Home essentials" },
    new Category { Id = 5, Name = "Sports", Description = "Sports & fitness" },
    new Category { Id = 6, Name = "Beauty", Description = "Beauty & personal care" },
    new Category { Id = 7, Name = "Toys", Description = "Toys & games" },
    new Category { Id = 8, Name = "Furniture", Description = "Home furniture" }
);


modelBuilder.Entity<Product>().HasData(

    // Electronics (Cat 1)
    new Product { Id=1, Name="iPhone 15 Pro", Description="Latest Apple smartphone", Price=999.99m, Stock=50, ImageUrl="https://images.unsplash.com/photo-1696446702183-a03dff5b2f1f?w=400", CategoryId=1, CategoryName="Electronics", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=2, Name="MacBook Pro 14", Description="Apple M3 Pro chip laptop", Price=1999.99m, Stock=30, ImageUrl="https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400", CategoryId=1, CategoryName="Electronics", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=3, Name="Sony WH-1000XM5", Description="Premium noise cancelling headphones", Price=349.99m, Stock=60, ImageUrl="https://images.unsplash.com/photo-1546435770-a3e426bf472b?w=400", CategoryId=1, CategoryName="Electronics", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=4, Name="Apple Watch Series 9", Description="Advanced smartwatch", Price=399.99m, Stock=55, ImageUrl="https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?w=400", CategoryId=1, CategoryName="Electronics", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=5, Name="iPad Pro 12.8", Description="Professional Apple tablet", Price=1099.99m, Stock=40, ImageUrl="https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400", CategoryId=1, CategoryName="Electronics", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

    // Clothing (Cat 2)
    new Product { Id=6, Name="Classic White T-Shirt", Description="100% cotton premium quality", Price=29.99m, Stock=100, ImageUrl="https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400", CategoryId=2, CategoryName="Clothing", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=7, Name="Slim Fit Jeans", Description="Comfortable stretch denim", Price=59.99m, Stock=80, ImageUrl="https://images.unsplash.com/photo-1542272604-787c3835535d?w=400", CategoryId=2, CategoryName="Clothing", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=8, Name="Leather Jacket", Description="Genuine leather biker jacket", Price=199.99m, Stock=30, ImageUrl="https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400", CategoryId=2, CategoryName="Clothing", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=9, Name="Running Shoes Nike", Description="Lightweight performance shoes", Price=89.99m, Stock=60, ImageUrl="https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400", CategoryId=2, CategoryName="Clothing", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=10, Name="Hoodie Pullover", Description="Comfortable fleece hoodie", Price=44.99m, Stock=75, ImageUrl="https://images.unsplash.com/photo-1556821840-3a63f15732ce?w=400", CategoryId=2, CategoryName="Clothing", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

    // Books (Cat 3)
    new Product { Id=11, Name="Clean Code", Description="Agile software craftsmanship by Robert C. Martin", Price=39.99m, Stock=100, ImageUrl="https://images.unsplash.com/photo-1532012197267-da84d127e765?w=400", CategoryId=3, CategoryName="Books", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=12, Name="Atomic Habits", Description="Tiny changes remarkable results", Price=24.99m, Stock=120, ImageUrl="https://images.unsplash.com/photo-1589829085413-56de8ae18c73?w=400", CategoryId=3, CategoryName="Books", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=13, Name="The Alchemist", Description="Paulo Coelho's magical novel", Price=15.99m, Stock=160, ImageUrl="https://images.unsplash.com/photo-1507842217343-583bb7270b66?w=400", CategoryId=3, CategoryName="Books", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=14, Name="Python Crash Course", Description="Hands-on introduction to programming", Price=39.99m, Stock=110, ImageUrl="https://images.unsplash.com/photo-1550399105-c4db5fb85c18?w=400", CategoryId=3, CategoryName="Books", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=15, Name="Rich Dad Poor Dad", Description="What the rich teach about money", Price=19.99m, Stock=140, ImageUrl="https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=400", CategoryId=3, CategoryName="Books", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

    // Home & Kitchen (Cat 4)
    new Product { Id=16, Name="Instant Pot Duo", Description="7-in-1 electric pressure cooker", Price=89.99m, Stock=45, ImageUrl="https://images.unsplash.com/photo-1585515320310-259814833e62?w=400", CategoryId=4, CategoryName="Home & Kitchen", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=17, Name="Nespresso Machine", Description="Automatic espresso coffee maker", Price=149.99m, Stock=35, ImageUrl="https://images.unsplash.com/photo-1570486829-82b5494d2d4b?w=400", CategoryId=4, CategoryName="Home & Kitchen", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=18, Name="KitchenAid Mixer", Description="Stand mixer for baking", Price=299.99m, Stock=25, ImageUrl="https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=400", CategoryId=4, CategoryName="Home & Kitchen", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=19, Name="Air Purifier", Description="HEPA filter air purifier", Price=199.99m, Stock=30, ImageUrl="https://images.unsplash.com/photo-1585771724684-38269d6639fd?w=400", CategoryId=4, CategoryName="Home & Kitchen", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=20, Name="Bed Sheet Set", Description="1000 thread count Egyptian cotton", Price=79.99m, Stock=60, ImageUrl="https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=400", CategoryId=4, CategoryName="Home & Kitchen", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

    // Sports (Cat 5)
    new Product { Id=21, Name="Yoga Mat Premium", Description="Non-slip exercise yoga mat", Price=49.99m, Stock=80, ImageUrl="https://images.unsplash.com/photo-1601925260368-ae2f83cf8b7f?w=400", CategoryId=5, CategoryName="Sports", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=22, Name="Dumbbell Set", Description="Adjustable weight dumbbell set", Price=129.99m, Stock=40, ImageUrl="https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=400", CategoryId=5, CategoryName="Sports", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=23, Name="Cycling Helmet", Description="Lightweight road cycling helmet", Price=79.99m, Stock=50, ImageUrl="https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400", CategoryId=5, CategoryName="Sports", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=24, Name="Tennis Racket", Description="Professional grade tennis racket", Price=89.99m, Stock=35, ImageUrl="https://images.unsplash.com/photo-1551698618-1dfe5d97d256?w=400", CategoryId=5, CategoryName="Sports", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=25, Name="Protein Powder", Description="Whey protein chocolate flavor 2kg", Price=59.99m, Stock=90, ImageUrl="https://images.unsplash.com/photo-1593095948071-474c5cc2989d?w=400", CategoryId=5, CategoryName="Sports", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

    // Beauty (Cat 6)
    new Product { Id=26, Name="Vitamin C Serum", Description="Brightening face serum 30ml", Price=34.99m, Stock=100, ImageUrl="https://images.unsplash.com/photo-1571781926291-c477ebfd024b?w=400", CategoryId=6, CategoryName="Beauty", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=27, Name="Hair Dryer Dyson", Description="Supersonic professional hair dryer", Price=429.99m, Stock=25, ImageUrl="https://images.unsplash.com/photo-1522338242992-e1a54906a8da?w=400", CategoryId=6, CategoryName="Beauty", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=28, Name="Moisturizer SPF50", Description="Daily moisturizer with sun protection", Price=29.99m, Stock=120, ImageUrl="https://images.unsplash.com/photo-1556228578-8c89e6adf883?w=400", CategoryId=6, CategoryName="Beauty", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=29, Name="Perfume Set", Description="Luxury fragrance gift set", Price=89.99m, Stock=45, ImageUrl="https://images.unsplash.com/photo-1541643600914-78b084683702?w=400", CategoryId=6, CategoryName="Beauty", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=30, Name="Electric Toothbrush", Description="Oral-B smart electric toothbrush", Price=69.99m, Stock=60, ImageUrl="https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=400", CategoryId=6, CategoryName="Beauty", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

    // Toys (Cat 7)
    new Product { Id=31, Name="LEGO Technic Set", Description="Advanced building block set 1500pcs", Price=99.99m, Stock=40, ImageUrl="https://images.unsplash.com/photo-1587654780291-39c9404d746b?w=400", CategoryId=7, CategoryName="Toys", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=32, Name="Remote Control Car", Description="High speed RC car 1:10 scale", Price=79.99m, Stock=35, ImageUrl="https://images.unsplash.com/photo-1594736797933-d0501ba2fe65?w=400", CategoryId=7, CategoryName="Toys", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=33, Name="Board Game Monopoly", Description="Classic family board game", Price=34.99m, Stock=70, ImageUrl="https://images.unsplash.com/photo-1611371805429-8b5c1b2c34ba?w=400", CategoryId=7, CategoryName="Toys", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=34, Name="Drone Mini", Description="Beginner friendly mini drone", Price=49.99m, Stock=45, ImageUrl="https://images.unsplash.com/photo-1473968512647-3e447244af8f?w=400", CategoryId=7, CategoryName="Toys", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=35, Name="Puzzle 1000pcs", Description="Scenic landscape jigsaw puzzle", Price=24.99m, Stock=80, ImageUrl="https://images.unsplash.com/photo-1587654780291-39c9404d746b?w=400", CategoryId=7, CategoryName="Toys", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },

    // Furniture (Cat 8)
    new Product { Id=36, Name="Office Chair", Description="Ergonomic mesh office chair", Price=299.99m, Stock=20, ImageUrl="https://images.unsplash.com/photo-1596162954151-cdcb4c0f70a8?w=400", CategoryId=8, CategoryName="Furniture", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=37, Name="Standing Desk", Description="Height adjustable standing desk", Price=499.99m, Stock=15, ImageUrl="https://images.unsplash.com/photo-1593642632559-0c6d3fc62b89?w=400", CategoryId=8, CategoryName="Furniture", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=38, Name="Bookshelf 5 Tier", Description="Modern wooden bookshelf", Price=149.99m, Stock=25, ImageUrl="https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400", CategoryId=8, CategoryName="Furniture", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=39, Name="Sofa 3-Seater", Description="Modern fabric 3-seater sofa", Price=799.99m, Stock=10, ImageUrl="https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=400", CategoryId=8, CategoryName="Furniture", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Product { Id=40, Name="Bed Frame Queen", Description="Solid wood queen size bed frame", Price=599.99m, Stock=12, ImageUrl="https://images.unsplash.com/photo-1505693314120-0d443867891c?w=400", CategoryId=8, CategoryName="Furniture", IsActive=true, CreatedAt=new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
);
    }


}