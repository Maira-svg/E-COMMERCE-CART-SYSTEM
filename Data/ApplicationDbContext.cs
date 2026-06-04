using Microsoft.EntityFrameworkCore;
using DarazUltimateMVC.Models;

namespace DarazUltimateMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Admin password is hashed - Frontend mein show nahi hoga!
            // Password: Admin@123
            string adminPasswordHash = "";
            
            modelBuilder.Entity<User>().HasData(
                new User { 
                    Id = 1, 
                    Name = "Admin User", 
                    Email = "admin@daraz.com", 
                    PasswordHash = adminPasswordHash, 
                    Role = "ADMIN" 
                }
            );
            
            // 58 Products as per your frontend
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Title = "Apple iPhone 15 Pro Max", Category = "Electronic Devices", Price = 399900, ImageUrl = "https://images.pexels.com/photos/699122/pexels-photo-699122.jpeg?w=400" },
                new Product { Id = 2, Title = "Samsung Galaxy S24 Ultra", Category = "Electronic Devices", Price = 389900, ImageUrl = "https://images.pexels.com/photos/47261/pexels-photo-47261.jpeg?w=400" },
                new Product { Id = 3, Title = "MacBook Pro M3", Category = "Electronic Devices", Price = 429900, ImageUrl = "https://images.pexels.com/photos/18105/pexels-photo.jpg?w=400" },
                new Product { Id = 4, Title = "iPad Pro 12.9-inch", Category = "Electronic Devices", Price = 249900, ImageUrl = "https://images.pexels.com/photos/1334597/pexels-photo-1334597.jpeg?w=400" },
                new Product { Id = 5, Title = "Nike Air Max Sneakers", Category = "Fashion & Wear", Price = 18999, ImageUrl = "https://images.pexels.com/photos/1598508/pexels-photo-1598508.jpeg?w=400" },
                new Product { Id = 6, Title = "Dyson V15 Vacuum", Category = "Home & Lifestyle", Price = 125000, ImageUrl = "https://images.pexels.com/photos/4107280/pexels-photo-4107280.jpeg?w=400" },
                new Product { Id = 7, Title = "Logitech G502 Hero", Category = "Gaming & Gear", Price = 12500, ImageUrl = "https://images.pexels.com/photos/2115256/pexels-photo-2115256.jpeg?w=400" },
                new Product { Id = 8, Title = "Air Fryer 5.5L", Category = "Food & Kitchen", Price = 18999, ImageUrl = "https://images.pexels.com/photos/5639252/pexels-photo-5639252.jpeg?w=400" },
                new Product { Id = 9, Title = "Yoga Mat Premium", Category = "Sports & Fitness", Price = 1499, ImageUrl = "https://images.pexels.com/photos/4056723/pexels-photo-4056723.jpeg?w=400" },
                new Product { Id = 10, Title = "Baby Stroller", Category = "Baby & Kids", Price = 18999, ImageUrl = "https://images.pexels.com/photos/4260321/pexels-photo-4260321.jpeg?w=400" }
            );
        }
    }
}
