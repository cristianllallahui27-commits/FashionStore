using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDescuentosAutorizados : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DescuentoAutorizadoId",
                table: "Venta",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DescuentosAutorizados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DescuentosAutorizados", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Venta_DescuentoAutorizadoId",
                table: "Venta",
                column: "DescuentoAutorizadoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Venta_DescuentosAutorizados_DescuentoAutorizadoId",
                table: "Venta",
                column: "DescuentoAutorizadoId",
                principalTable: "DescuentosAutorizados",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Venta_DescuentosAutorizados_DescuentoAutorizadoId",
                table: "Venta");

            migrationBuilder.DropTable(
                name: "DescuentosAutorizados");

            migrationBuilder.DropIndex(
                name: "IX_Venta_DescuentoAutorizadoId",
                table: "Venta");

            migrationBuilder.DropColumn(
                name: "DescuentoAutorizadoId",
                table: "Venta");
        }
    }
}
