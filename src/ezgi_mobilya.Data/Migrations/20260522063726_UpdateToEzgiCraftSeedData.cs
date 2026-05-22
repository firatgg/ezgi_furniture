using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ezgi_mobilya.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToEzgiCraftSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "El emeği geometrik ahşap duvar süsü", "Geometrik Duvar Dekoru", 450.00m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "Description", "Name", "Price" },
                values: new object[] { 2, "Zeytin ağacından el yapımı sunum tabağı", "Ahşap Sunum Tabağı", 320.00m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "Akçaağaçtan pürüzsüz bebek diş kaşıyıcı", "Organik Diş Kaşıyıcı", 125.00m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "Rustik tarzda el yapımı ahşap lamba", "Ahşap Masa Lambası", 750.00m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "Description", "Name", "Price" },
                values: new object[] { 3, "Doğal çam ağacından Montessori aktivite masası ve sandalyesi", "Montessori Aktivite Masası", 1850.00m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "Description", "Name", "Price" },
                values: new object[] { 2, "Ceviz ağacından el yapımı şık kahve tepsisi", "Ahşap Kahve Tepsisi", 380.00m });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Çocukların zihinsel gelişimini ve motor becerilerini destekleyen ahşap oyuncaklar.", "Eğitici Oyuncaklar" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Montessori pedagojisine uygun, sade ve doğal tasarımlar.", "Montessori Ürünleri" });

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Bebekler için güvenli pürüzsüz ve sağlıklı diş kaşıyıcılar, çıngıraklar.", "Bebek Grubu" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "Ahşap blok oyuncak seti", "Eğitici Bloklar", 250.00m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CategoryId", "Description", "Name", "Price" },
                values: new object[] { 1, "Ahşap şekil yapbozu", "Şekil Yapbozu", 180.00m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "Ahşap bebek diş kaşıyıcı", "Diş Kaşıyıcı", 95.00m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Description", "Name", "Price" },
                values: new object[] { "Ahşap araba oyuncağı", "Ahşap Araba", 120.00m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CategoryId", "Description", "Name", "Price" },
                values: new object[] { 2, "Montessori ahşap sıralama seti", "Montessori Seti", 320.00m });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CategoryId", "Description", "Name", "Price" },
                values: new object[] { 1, "Ahşap zeka oyunu", "Ahşap Zeka Oyunu", 210.00m });
        }
    }
}
