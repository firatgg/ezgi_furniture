using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ezgi_mobilya.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductSeedImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageUrl", "ThumbnailUrl" },
                values: new object[] { "https://images.unsplash.com/photo-1502005229762-fc1b2b812ca5?auto=format&fit=crop&w=800&q=80", "https://images.unsplash.com/photo-1502005229762-fc1b2b812ca5?auto=format&fit=crop&w=400&q=80" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImageUrl", "ThumbnailUrl" },
                values: new object[] { "https://images.unsplash.com/photo-1596797882942-159027409f58?auto=format&fit=crop&w=800&q=80", "https://images.unsplash.com/photo-1596797882942-159027409f58?auto=format&fit=crop&w=400&q=80" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ImageUrl", "ThumbnailUrl" },
                values: new object[] { "https://images.unsplash.com/photo-1596461404969-9ae70f2830c1?auto=format&fit=crop&w=800&q=80", "https://images.unsplash.com/photo-1596461404969-9ae70f2830c1?auto=format&fit=crop&w=400&q=80" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ImageUrl", "ThumbnailUrl" },
                values: new object[] { "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?auto=format&fit=crop&w=800&q=80", "https://images.unsplash.com/photo-1507473885765-e6ed057f782c?auto=format&fit=crop&w=400&q=80" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ImageUrl", "ThumbnailUrl" },
                values: new object[] { "https://images.unsplash.com/photo-1567538096630-e0c55bd6374c?auto=format&fit=crop&w=800&q=80", "https://images.unsplash.com/photo-1567538096630-e0c55bd6374c?auto=format&fit=crop&w=400&q=80" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ImageUrl", "ThumbnailUrl" },
                values: new object[] { "https://images.unsplash.com/photo-1581608370197-0429bfd19641?auto=format&fit=crop&w=800&q=80", "https://images.unsplash.com/photo-1581608370197-0429bfd19641?auto=format&fit=crop&w=400&q=80" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ImageUrl", "ThumbnailUrl" },
                values: new object[] { "themes/infinite-loop/img/toy-gallery-01.jpg", "themes/infinite-loop/img/toy-gallery-thumb-01.jpg" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ImageUrl", "ThumbnailUrl" },
                values: new object[] { "themes/infinite-loop/img/toy-gallery-02.jpg", "themes/infinite-loop/img/toy-gallery-thumb-02.jpg" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ImageUrl", "ThumbnailUrl" },
                values: new object[] { "themes/infinite-loop/img/toy-gallery-03.jpg", "themes/infinite-loop/img/toy-gallery-thumb-03.jpg" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ImageUrl", "ThumbnailUrl" },
                values: new object[] { "themes/infinite-loop/img/toy-gallery-04.jpg", "themes/infinite-loop/img/toy-gallery-thumb-04.jpg" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ImageUrl", "ThumbnailUrl" },
                values: new object[] { "themes/infinite-loop/img/toy-gallery-05.jpg", "themes/infinite-loop/img/toy-gallery-thumb-05.jpg" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "ImageUrl", "ThumbnailUrl" },
                values: new object[] { "themes/infinite-loop/img/toy-gallery-06.jpg", "themes/infinite-loop/img/toy-gallery-thumb-06.jpg" });
        }
    }
}
