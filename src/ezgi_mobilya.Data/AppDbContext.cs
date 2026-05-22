using System;
using ezgi_mobilya.Core.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ezgi_mobilya.Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<ContactMessage> ContactMessages { get; set; } = null!;
        public DbSet<SocialPost> SocialPosts { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Category Configuration
            builder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Description).HasMaxLength(500);

                entity.HasMany(c => c.Products)
                      .WithOne(p => p.Category)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Product Configuration
            builder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
                entity.Property(p => p.Description).HasMaxLength(1000);
                entity.Property(p => p.Price).HasPrecision(18, 2);
                entity.Property(p => p.ImageUrl).HasMaxLength(500);
                entity.Property(p => p.ThumbnailUrl).HasMaxLength(500);
            });

            // ContactMessage Configuration
            builder.Entity<ContactMessage>(entity =>
            {
                entity.HasKey(cm => cm.Id);
                entity.Property(cm => cm.FullName).IsRequired().HasMaxLength(100);
                entity.Property(cm => cm.Email).IsRequired().HasMaxLength(100);
                entity.Property(cm => cm.Subject).HasMaxLength(150);
                entity.Property(cm => cm.Message).IsRequired().HasMaxLength(2000);
            });

            // SocialPost Configuration
            builder.Entity<SocialPost>(entity =>
            {
                entity.HasKey(sp => sp.Id);
                entity.Property(sp => sp.Caption).IsRequired().HasMaxLength(4000);
                entity.Property(sp => sp.ImageUrl).HasMaxLength(1000);
                entity.Property(sp => sp.Platforms).IsRequired().HasMaxLength(250);
                entity.Property(sp => sp.ErrorMessage).HasMaxLength(1000);
                entity.Property(sp => sp.SocialPostId).HasMaxLength(250);
            });

            // Seed Categories
            var catDecor = new Category { Id = 1, Name = "Ahşap Dekorasyon", Description = "Yaşam alanlarınıza şıklık katacak el emeği ahşap dekoratif ürünler.", CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc) };
            var catKitchen = new Category { Id = 2, Name = "Mutfak & Sunum", Description = "Doğal ahşaptan üretilmiş, sağlıklı mutfak ve sunum gereçleri.", CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc) };
            var catKids = new Category { Id = 3, Name = "Çocuk & Montessori", Description = "Bebek ve çocuklar için organik ahşap oyuncaklar, gelişim setleri ve Montessori mobilyaları.", CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc) };

            builder.Entity<Category>().HasData(catDecor, catKitchen, catKids);

            // Seed Products
            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    CategoryId = 1,
                    Name = "Geometrik Duvar Dekoru",
                    Description = "El emeği geometrik ahşap duvar süsü",
                    Price = 450.00m,
                    ImageUrl = "https://images.unsplash.com/photo-1502005229762-fc1b2b812ca5?auto=format&fit=crop&w=800&q=80",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1502005229762-fc1b2b812ca5?auto=format&fit=crop&w=400&q=80",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 2,
                    CategoryId = 2,
                    Name = "Ahşap Sunum Tabağı",
                    Description = "Zeytin ağacından el yapımı sunum tabağı",
                    Price = 320.00m,
                    ImageUrl = "https://images.unsplash.com/photo-1596797882942-159027409f58?auto=format&fit=crop&w=800&q=80",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1596797882942-159027409f58?auto=format&fit=crop&w=400&q=80",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 3,
                    CategoryId = 3,
                    Name = "Organik Diş Kaşıyıcı",
                    Description = "Akçaağaçtan pürüzsüz bebek diş kaşıyıcı",
                    Price = 125.00m,
                    ImageUrl = "https://images.unsplash.com/photo-1596461404969-9ae70f2830c1?auto=format&fit=crop&w=800&q=80",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1596461404969-9ae70f2830c1?auto=format&fit=crop&w=400&q=80",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 4,
                    CategoryId = 1,
                    Name = "Ahşap Masa Lambası",
                    Description = "Rustik tarzda el yapımı ahşap lamba",
                    Price = 750.00m,
                    ImageUrl = "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?auto=format&fit=crop&w=800&q=80",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?auto=format&fit=crop&w=400&q=80",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 5,
                    CategoryId = 3,
                    Name = "Montessori Aktivite Masası",
                    Description = "Doğal çam ağacından Montessori aktivite masası ve sandalyesi",
                    Price = 1850.00m,
                    ImageUrl = "https://images.unsplash.com/photo-1567538096630-e0c55bd6374c?auto=format&fit=crop&w=800&q=80",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1567538096630-e0c55bd6374c?auto=format&fit=crop&w=400&q=80",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 6,
                    CategoryId = 2,
                    Name = "Ahşap Kahve Tepsisi",
                    Description = "Ceviz ağacından el yapımı şık kahve tepsisi",
                    Price = 380.00m,
                    ImageUrl = "https://images.unsplash.com/photo-1581608370197-0429bfd19641?auto=format&fit=crop&w=800&q=80",
                    ThumbnailUrl = "https://images.unsplash.com/photo-1581608370197-0429bfd19641?auto=format&fit=crop&w=400&q=80",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
