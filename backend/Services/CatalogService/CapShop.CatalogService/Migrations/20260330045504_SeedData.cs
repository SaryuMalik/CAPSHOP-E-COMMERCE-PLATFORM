using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CapShop.CatalogService.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Gadgets & devices");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Fashion & apparel");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Books & magazines");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 4, "Home essentials", "Home & Kitchen" },
                    { 5, "Sports & fitness", "Sports" },
                    { 6, "Beauty & personal care", "Beauty" },
                    { 7, "Toys & games", "Toys" },
                    { 8, "Home furniture", "Furniture" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CategoryName", "CreatedAt", "Description", "ImageUrl", "IsActive", "Name", "Price", "Stock" },
                values: new object[,]
                {
                    { 1, 1, "Electronics", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Latest Apple smartphone", "https://images.unsplash.com/photo-1696446702183-a03dff5b2f1f?w=400", true, "iPhone 15 Pro", 999.99m, 50 },
                    { 2, 1, "Electronics", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Apple M3 Pro chip laptop", "https://images.unsplash.com/photo-1517336714731-489689fd1ca8?w=400", true, "MacBook Pro 14", 1999.99m, 30 },
                    { 3, 1, "Electronics", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Premium noise cancelling headphones", "https://images.unsplash.com/photo-1546435770-a3e426bf472b?w=400", true, "Sony WH-1000XM5", 349.99m, 60 },
                    { 4, 1, "Electronics", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Advanced smartwatch", "https://images.unsplash.com/photo-1434493789847-2f02dc6ca35d?w=400", true, "Apple Watch Series 9", 399.99m, 55 },
                    { 5, 1, "Electronics", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Professional Apple tablet", "https://images.unsplash.com/photo-1544244015-0df4b3ffc6b0?w=400", true, "iPad Pro 12.8", 1099.99m, 40 },
                    { 6, 2, "Clothing", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "100% cotton premium quality", "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?w=400", true, "Classic White T-Shirt", 29.99m, 100 },
                    { 7, 2, "Clothing", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Comfortable stretch denim", "https://images.unsplash.com/photo-1542272604-787c3835535d?w=400", true, "Slim Fit Jeans", 59.99m, 80 },
                    { 8, 2, "Clothing", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Genuine leather biker jacket", "https://images.unsplash.com/photo-1551028719-00167b16eac5?w=400", true, "Leather Jacket", 199.99m, 30 },
                    { 9, 2, "Clothing", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lightweight performance shoes", "https://images.unsplash.com/photo-1542291026-7eec264c27ff?w=400", true, "Running Shoes Nike", 89.99m, 60 },
                    { 10, 2, "Clothing", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Comfortable fleece hoodie", "https://images.unsplash.com/photo-1556821840-3a63f15732ce?w=400", true, "Hoodie Pullover", 44.99m, 75 },
                    { 11, 3, "Books", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Agile software craftsmanship by Robert C. Martin", "https://images.unsplash.com/photo-1532012197267-da84d127e765?w=400", true, "Clean Code", 39.99m, 100 },
                    { 12, 3, "Books", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tiny changes remarkable results", "https://images.unsplash.com/photo-1589829085413-56de8ae18c73?w=400", true, "Atomic Habits", 24.99m, 120 },
                    { 13, 3, "Books", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Paulo Coelho's magical novel", "https://images.unsplash.com/photo-1507842217343-583bb7270b66?w=400", true, "The Alchemist", 15.99m, 160 },
                    { 14, 3, "Books", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Hands-on introduction to programming", "https://images.unsplash.com/photo-1550399105-c4db5fb85c18?w=400", true, "Python Crash Course", 39.99m, 110 },
                    { 15, 3, "Books", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "What the rich teach about money", "https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=400", true, "Rich Dad Poor Dad", 19.99m, 140 },
                    { 16, 4, "Home & Kitchen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "7-in-1 electric pressure cooker", "https://images.unsplash.com/photo-1585515320310-259814833e62?w=400", true, "Instant Pot Duo", 89.99m, 45 },
                    { 17, 4, "Home & Kitchen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Automatic espresso coffee maker", "https://images.unsplash.com/photo-1570486829-82b5494d2d4b?w=400", true, "Nespresso Machine", 149.99m, 35 },
                    { 18, 4, "Home & Kitchen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Stand mixer for baking", "https://images.unsplash.com/photo-1556909114-f6e7ad7d3136?w=400", true, "KitchenAid Mixer", 299.99m, 25 },
                    { 19, 4, "Home & Kitchen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "HEPA filter air purifier", "https://images.unsplash.com/photo-1585771724684-38269d6639fd?w=400", true, "Air Purifier", 199.99m, 30 },
                    { 20, 4, "Home & Kitchen", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "1000 thread count Egyptian cotton", "https://images.unsplash.com/photo-1631049307264-da0ec9d70304?w=400", true, "Bed Sheet Set", 79.99m, 60 },
                    { 21, 5, "Sports", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Non-slip exercise yoga mat", "https://images.unsplash.com/photo-1601925260368-ae2f83cf8b7f?w=400", true, "Yoga Mat Premium", 49.99m, 80 },
                    { 22, 5, "Sports", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Adjustable weight dumbbell set", "https://images.unsplash.com/photo-1534438327276-14e5300c3a48?w=400", true, "Dumbbell Set", 129.99m, 40 },
                    { 23, 5, "Sports", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Lightweight road cycling helmet", "https://images.unsplash.com/photo-1558618666-fcd25c85cd64?w=400", true, "Cycling Helmet", 79.99m, 50 },
                    { 24, 5, "Sports", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Professional grade tennis racket", "https://images.unsplash.com/photo-1551698618-1dfe5d97d256?w=400", true, "Tennis Racket", 89.99m, 35 },
                    { 25, 5, "Sports", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Whey protein chocolate flavor 2kg", "https://images.unsplash.com/photo-1593095948071-474c5cc2989d?w=400", true, "Protein Powder", 59.99m, 90 },
                    { 26, 6, "Beauty", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Brightening face serum 30ml", "https://images.unsplash.com/photo-1571781926291-c477ebfd024b?w=400", true, "Vitamin C Serum", 34.99m, 100 },
                    { 27, 6, "Beauty", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Supersonic professional hair dryer", "https://images.unsplash.com/photo-1522338242992-e1a54906a8da?w=400", true, "Hair Dryer Dyson", 429.99m, 25 },
                    { 28, 6, "Beauty", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Daily moisturizer with sun protection", "https://images.unsplash.com/photo-1556228578-8c89e6adf883?w=400", true, "Moisturizer SPF50", 29.99m, 120 },
                    { 29, 6, "Beauty", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Luxury fragrance gift set", "https://images.unsplash.com/photo-1541643600914-78b084683702?w=400", true, "Perfume Set", 89.99m, 45 },
                    { 30, 6, "Beauty", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Oral-B smart electric toothbrush", "https://images.unsplash.com/photo-1559839734-2b71ea197ec2?w=400", true, "Electric Toothbrush", 69.99m, 60 },
                    { 31, 7, "Toys", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Advanced building block set 1500pcs", "https://images.unsplash.com/photo-1587654780291-39c9404d746b?w=400", true, "LEGO Technic Set", 99.99m, 40 },
                    { 32, 7, "Toys", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "High speed RC car 1:10 scale", "https://images.unsplash.com/photo-1594736797933-d0501ba2fe65?w=400", true, "Remote Control Car", 79.99m, 35 },
                    { 33, 7, "Toys", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Classic family board game", "https://images.unsplash.com/photo-1611371805429-8b5c1b2c34ba?w=400", true, "Board Game Monopoly", 34.99m, 70 },
                    { 34, 7, "Toys", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Beginner friendly mini drone", "https://images.unsplash.com/photo-1473968512647-3e447244af8f?w=400", true, "Drone Mini", 49.99m, 45 },
                    { 35, 7, "Toys", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Scenic landscape jigsaw puzzle", "https://images.unsplash.com/photo-1587654780291-39c9404d746b?w=400", true, "Puzzle 1000pcs", 24.99m, 80 },
                    { 36, 8, "Furniture", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ergonomic mesh office chair", "https://images.unsplash.com/photo-1596162954151-cdcb4c0f70a8?w=400", true, "Office Chair", 299.99m, 20 },
                    { 37, 8, "Furniture", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Height adjustable standing desk", "https://images.unsplash.com/photo-1593642632559-0c6d3fc62b89?w=400", true, "Standing Desk", 499.99m, 15 },
                    { 38, 8, "Furniture", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Modern wooden bookshelf", "https://images.unsplash.com/photo-1507003211169-0a1dd7228f2d?w=400", true, "Bookshelf 5 Tier", 149.99m, 25 },
                    { 39, 8, "Furniture", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Modern fabric 3-seater sofa", "https://images.unsplash.com/photo-1555041469-a586c61ea9bc?w=400", true, "Sofa 3-Seater", 799.99m, 10 },
                    { 40, 8, "Furniture", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Solid wood queen size bed frame", "https://images.unsplash.com/photo-1505693314120-0d443867891c?w=400", true, "Bed Frame Queen", 599.99m, 12 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Electronic items");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Clothes and accessories");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Books and magazines");
        }
    }
}
