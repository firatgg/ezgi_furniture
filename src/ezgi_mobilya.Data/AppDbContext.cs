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
            var catDecor = new Category { Id = 1, Name = "Özel Tasarım Masalar", Description = "Doğal ahşaptan, el işçiliğiyle üretilmiş benzersiz yemek, toplantı ve bahçe masaları.", CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc) };
            var catKitchen = new Category { Id = 2, Name = "TV Üniteleri & Sehpalar", Description = "Yaşam alanlarınıza estetik ve fonksiyonellik katacak ahşap TV üniteleri ve orta sehpalar.", CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc) };
            var catKids = new Category { Id = 3, Name = "Mutfak & Banko Tasarımları", Description = "Modern mutfak tezgahları, resepsiyon bankoları ve özel tasarım ahşap mobilyalar.", CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc) };

            builder.Entity<Category>().HasData(catDecor, catKitchen, catKids);

            // Seed Products
            builder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    CategoryId = 3,
                    Name = "Modern Ahşap Mutfak Tezgahı",
                    Description = "Siyah granit ve beyaz dolaplarla uyumlu, dayanıklı ve estetik ahşap mutfak tezgahı tasarımı.",
                    Price = 12500.00m,
                    ImageUrl = "images/products/mutfak-tezgahi.png",
                    ThumbnailUrl = "images/products/mutfak-tezgahi.png",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 2,
                    CategoryId = 1,
                    Name = "Doğal Kenarlı Ahşap Yemek Masası",
                    Description = "Tek parça kütükten üretilmiş, doğal kenar detaylarına sahip şık ahşap yemek masası.",
                    Price = 8500.00m,
                    ImageUrl = "images/products/dogal-ahsap-masa.png",
                    ThumbnailUrl = "images/products/dogal-ahsap-masa.png",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 3,
                    CategoryId = 2,
                    Name = "Kütük Ahşap TV Sehpası",
                    Description = "Tekerlekli tasarımıyla mobil ve fonksiyonel, kalın kütük gövdeli doğal TV sehpası.",
                    Price = 4500.00m,
                    ImageUrl = "images/products/kutuk-tv-sehpayi.png",
                    ThumbnailUrl = "images/products/kutuk-tv-sehpayi.png",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 4,
                    CategoryId = 3,
                    Name = "Modern Resepsiyon Bankosu",
                    Description = "Ofis ve iş yerleri için minimalist tasarımlı, beyaz lake kaplama modern resepsiyon masası.",
                    Price = 14500.00m,
                    ImageUrl = "images/products/resepsiyon-bankosu.png",
                    ThumbnailUrl = "images/products/resepsiyon-bankosu.png",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 5,
                    CategoryId = 2,
                    Name = "Cam Kaplama Ahşap Orta Sehpa",
                    Description = "Ceviz ağacından üretilmiş, üst yüzeyi temperli cam kaplı modern orta sehpa.",
                    Price = 3200.00m,
                    ImageUrl = "images/products/cam-orta-sehpa.png",
                    ThumbnailUrl = "images/products/cam-orta-sehpa.png",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 6,
                    CategoryId = 1,
                    Name = "Metal Ayaklı Ahşap Toplantı Masası",
                    Description = "Geniş ofisler için endüstriyel metal ayaklı, masif ahşap büyük toplantı masası.",
                    Price = 16500.00m,
                    ImageUrl = "images/products/toplanti-masasi.png",
                    ThumbnailUrl = "images/products/toplanti-masasi.png",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 7,
                    CategoryId = 1,
                    Name = "X Metal Ayaklı Bahçe Masası",
                    Description = "Dış mekan koşullarına dayanıklı, X tipi metal ayaklı geniş ahşap bahçe masası.",
                    Price = 9500.00m,
                    ImageUrl = "images/products/bahce-masasi.png",
                    ThumbnailUrl = "images/products/bahce-masasi.png",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 8,
                    CategoryId = 2,
                    Name = "Modern Duvar Tipi TV Ünitesi",
                    Description = "Koyu renk ahşap kaplama, bol saklama alanlı ve şık tasarımlı konsol ve TV ünitesi seti.",
                    Price = 11500.00m,
                    ImageUrl = "images/products/modern-tv-unitesi.png",
                    ThumbnailUrl = "images/products/modern-tv-unitesi.png",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                },
                new Product
                {
                    Id = 9,
                    CategoryId = 1,
                    Name = "Döküm Ayaklı Klasik Ahşap Masa",
                    Description = "Klasik döküm demir ayaklar üzerine oturtulmuş, zengin ahşap dokulu çalışma ve yemek masası.",
                    Price = 7800.00m,
                    ImageUrl = "images/products/klasik-ayakli-masa.png",
                    ThumbnailUrl = "images/products/klasik-ayakli-masa.png",
                    IsActive = true,
                    CreatedDate = new DateTime(2026, 5, 22, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
