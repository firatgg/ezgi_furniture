using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ezgi_mobilya.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedDataWithRealFurniture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Doğal ahşaptan, el işçiliğiyle üretilmiş benzersiz yemek, toplantı ve bahçe masaları.", "Özel Tasarım Masalar" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Yaşam alanlarınıza estetik ve fonksiyonellik katacak ahşap TV üniteleri ve orta sehpalar.", "TV Üniteleri & Sehpalar" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Modern mutfak tezgahları, resepsiyon bankoları ve özel tasarım ahşap mobilyalar.", "Mutfak & Banko Tasarımları" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "Name", "Price", "ThumbnailUrl" },
                values: new object[] { 3, "Siyah granit ve beyaz dolaplarla uyumlu, dayanıklı ve estetik ahşap mutfak tezgahı tasarımı.", "images/products/mutfak-tezgahi.png", "Modern Ahşap Mutfak Tezgahı", 12500.00m, "images/products/mutfak-tezgahi.png" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "Name", "Price", "ThumbnailUrl" },
                values: new object[] { 1, "Tek parça kütükten üretilmiş, doğal kenar detaylarına sahip şık ahşap yemek masası.", "images/products/dogal-ahsap-masa.png", "Doğal Kenarlı Ahşap Yemek Masası", 8500.00m, "images/products/dogal-ahsap-masa.png" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "Name", "Price", "ThumbnailUrl" },
                values: new object[] { 2, "Tekerlekli tasarımıyla mobil ve fonksiyonel, kalın kütük gövdeli doğal TV sehpası.", "images/products/kutuk-tv-sehpayi.png", "Kütük Ahşap TV Sehpası", 4500.00m, "images/products/kutuk-tv-sehpayi.png" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "Name", "Price", "ThumbnailUrl" },
                values: new object[] { 3, "Ofis ve iş yerleri için minimalist tasarımlı, beyaz lake kaplama modern resepsiyon masası.", "images/products/resepsiyon-bankosu.png", "Modern Resepsiyon Bankosu", 14500.00m, "images/products/resepsiyon-bankosu.png" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "Name", "Price", "ThumbnailUrl" },
                values: new object[] { 2, "Ceviz ağacından üretilmiş, üst yüzeyi temperli cam kaplı modern orta sehpa.", "images/products/cam-orta-sehpa.png", "Cam Kaplama Ahşap Orta Sehpa", 3200.00m, "images/products/cam-orta-sehpa.png" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "Name", "Price", "ThumbnailUrl" },
                values: new object[] { 1, "Geniş ofisler için endüstriyel metal ayaklı, masif ahşap büyük toplantı masası.", "images/products/toplanti-masasi.png", "Metal Ayaklı Ahşap Toplantı Masası", 16500.00m, "images/products/toplanti-masasi.png" });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "CreatedDate", "Description", "ImageUrl", "IsActive", "Name", "Price", "ThumbnailUrl", "UpdatedDate" },
                values: new object[,]
                {
                    { 7, 1, new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Dış mekan koşullarına dayanıklı, X tipi metal ayaklı geniş ahşap bahçe masası.", "images/products/bahce-masasi.png", true, "X Metal Ayaklı Bahçe Masası", 9500.00m, "images/products/bahce-masasi.png", null },
                    { 8, 2, new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Koyu renk ahşap kaplama, bol saklama alanlı ve şık tasarımlı konsol ve TV ünitesi seti.", "images/products/modern-tv-unitesi.png", true, "Modern Duvar Tipi TV Ünitesi", 11500.00m, "images/products/modern-tv-unitesi.png", null },
                    { 9, 1, new DateTime(2026, 5, 22, 0, 0, 0, 0, DateTimeKind.Utc), "Klasik döküm demir ayaklar üzerine oturtulmuş, zengin ahşap dokulu çalışma ve yemek masası.", "images/products/klasik-ayakli-masa.png", true, "Döküm Ayaklı Klasik Ahşap Masa", 7800.00m, "images/products/klasik-ayakli-masa.png", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Yaşam alanlarınıza şıklık katacak el emeği ahşap dekoratif ürünler.", "Ahşap Dekorasyon" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Doğal ahşaptan üretilmiş, sağlıklı mutfak ve sunum gereçleri.", "Mutfak & Sunum" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Bebek ve çocuklar için organik ahşap oyuncaklar, gelişim setleri ve Montessori mobilyaları.", "Çocuk & Montessori" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "Name", "Price", "ThumbnailUrl" },
                values: new object[] { 1, "El emeği geometrik ahşap duvar süsü", "https://images.unsplash.com/photo-1502005229762-fc1b2b812ca5?auto=format&fit=crop&w=800&q=80", "Geometrik Duvar Dekoru", 450.00m, "https://images.unsplash.com/photo-1502005229762-fc1b2b812ca5?auto=format&fit=crop&w=400&q=80" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "Name", "Price", "ThumbnailUrl" },
                values: new object[] { 2, "Zeytin ağacından el yapımı sunum tabağı", "https://images.unsplash.com/photo-1596797882942-159027409f58?auto=format&fit=crop&w=800&q=80", "Ahşap Sunum Tabağı", 320.00m, "https://images.unsplash.com/photo-1596797882942-159027409f58?auto=format&fit=crop&w=400&q=80" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "Name", "Price", "ThumbnailUrl" },
                values: new object[] { 3, "Akçaağaçtan pürüzsüz bebek diş kaşıyıcı", "https://images.unsplash.com/photo-1596461404969-9ae70f2830c1?auto=format&fit=crop&w=800&q=80", "Organik Diş Kaşıyıcı", 125.00m, "https://images.unsplash.com/photo-1596461404969-9ae70f2830c1?auto=format&fit=crop&w=400&q=80" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "Name", "Price", "ThumbnailUrl" },
                values: new object[] { 1, "Rustik tarzda el yapımı ahşap lamba", "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?auto=format&fit=crop&w=800&q=80", "Ahşap Masa Lambası", 750.00m, "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?auto=format&fit=crop&w=400&q=80" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "Name", "Price", "ThumbnailUrl" },
                values: new object[] { 3, "Doğal çam ağacından Montessori aktivite masası ve sandalyesi", "https://images.unsplash.com/photo-1567538096630-e0c55bd6374c?auto=format&fit=crop&w=800&q=80", "Montessori Aktivite Masası", 1850.00m, "https://images.unsplash.com/photo-1567538096630-e0c55bd6374c?auto=format&fit=crop&w=400&q=80" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "Description", "ImageUrl", "Name", "Price", "ThumbnailUrl" },
                values: new object[] { 2, "Ceviz ağacından el yapımı şık kahve tepsisi", "https://images.unsplash.com/photo-1581608370197-0429bfd19641?auto=format&fit=crop&w=800&q=80", "Ahşap Kahve Tepsisi", 380.00m, "https://images.unsplash.com/photo-1581608370197-0429bfd19641?auto=format&fit=crop&w=400&q=80" });
        }
    }
}
