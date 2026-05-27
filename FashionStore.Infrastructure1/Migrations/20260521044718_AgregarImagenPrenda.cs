using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarImagenPrenda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagenUrl",
                table: "Prendas",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagenUrl",
                table: "Prendas");
        }
    }
}
