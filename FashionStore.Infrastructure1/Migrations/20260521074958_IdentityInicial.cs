using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FashionStore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IdentityInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleVentas_Prendas_PrendaId",
                table: "DetalleVentas");

            migrationBuilder.DropForeignKey(
                name: "FK_DetalleVentas_Ventas_VentaId",
                table: "DetalleVentas");

            migrationBuilder.DropForeignKey(
                name: "FK_Prendas_Categorias_CategoriaId",
                table: "Prendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Clientes_ClienteId",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_MetodosPago_MetodoPagoId",
                table: "Ventas");

            migrationBuilder.DropForeignKey(
                name: "FK_Ventas_Vendedores_VendedorId",
                table: "Ventas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ventas",
                table: "Ventas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vendedores",
                table: "Vendedores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetodosPago",
                table: "MetodosPago");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DetalleVentas",
                table: "DetalleVentas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes");

            migrationBuilder.RenameTable(
                name: "Ventas",
                newName: "Venta");

            migrationBuilder.RenameTable(
                name: "Vendedores",
                newName: "Vendedor");

            migrationBuilder.RenameTable(
                name: "MetodosPago",
                newName: "MetodoPago");

            migrationBuilder.RenameTable(
                name: "DetalleVentas",
                newName: "DetalleVenta");

            migrationBuilder.RenameTable(
                name: "Clientes",
                newName: "Cliente");

            migrationBuilder.RenameIndex(
                name: "IX_Ventas_VendedorId",
                table: "Venta",
                newName: "IX_Venta_VendedorId");

            migrationBuilder.RenameIndex(
                name: "IX_Ventas_MetodoPagoId",
                table: "Venta",
                newName: "IX_Venta_MetodoPagoId");

            migrationBuilder.RenameIndex(
                name: "IX_Ventas_ClienteId",
                table: "Venta",
                newName: "IX_Venta_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_DetalleVentas_VentaId",
                table: "DetalleVenta",
                newName: "IX_DetalleVenta_VentaId");

            migrationBuilder.RenameIndex(
                name: "IX_DetalleVentas_PrendaId",
                table: "DetalleVenta",
                newName: "IX_DetalleVenta_PrendaId");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Prendas",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Venta",
                table: "Venta",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vendedor",
                table: "Vendedor",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetodoPago",
                table: "MetodoPago",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DetalleVenta",
                table: "DetalleVenta",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cliente",
                table: "Cliente",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleVenta_Prendas_PrendaId",
                table: "DetalleVenta",
                column: "PrendaId",
                principalTable: "Prendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleVenta_Venta_VentaId",
                table: "DetalleVenta",
                column: "VentaId",
                principalTable: "Venta",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prendas_Categorias_CategoriaId",
                table: "Prendas",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Venta_Cliente_ClienteId",
                table: "Venta",
                column: "ClienteId",
                principalTable: "Cliente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Venta_MetodoPago_MetodoPagoId",
                table: "Venta",
                column: "MetodoPagoId",
                principalTable: "MetodoPago",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Venta_Vendedor_VendedorId",
                table: "Venta",
                column: "VendedorId",
                principalTable: "Vendedor",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetalleVenta_Prendas_PrendaId",
                table: "DetalleVenta");

            migrationBuilder.DropForeignKey(
                name: "FK_DetalleVenta_Venta_VentaId",
                table: "DetalleVenta");

            migrationBuilder.DropForeignKey(
                name: "FK_Prendas_Categorias_CategoriaId",
                table: "Prendas");

            migrationBuilder.DropForeignKey(
                name: "FK_Venta_Cliente_ClienteId",
                table: "Venta");

            migrationBuilder.DropForeignKey(
                name: "FK_Venta_MetodoPago_MetodoPagoId",
                table: "Venta");

            migrationBuilder.DropForeignKey(
                name: "FK_Venta_Vendedor_VendedorId",
                table: "Venta");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Venta",
                table: "Venta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vendedor",
                table: "Vendedor");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MetodoPago",
                table: "MetodoPago");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DetalleVenta",
                table: "DetalleVenta");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cliente",
                table: "Cliente");

            migrationBuilder.RenameTable(
                name: "Venta",
                newName: "Ventas");

            migrationBuilder.RenameTable(
                name: "Vendedor",
                newName: "Vendedores");

            migrationBuilder.RenameTable(
                name: "MetodoPago",
                newName: "MetodosPago");

            migrationBuilder.RenameTable(
                name: "DetalleVenta",
                newName: "DetalleVentas");

            migrationBuilder.RenameTable(
                name: "Cliente",
                newName: "Clientes");

            migrationBuilder.RenameIndex(
                name: "IX_Venta_VendedorId",
                table: "Ventas",
                newName: "IX_Ventas_VendedorId");

            migrationBuilder.RenameIndex(
                name: "IX_Venta_MetodoPagoId",
                table: "Ventas",
                newName: "IX_Ventas_MetodoPagoId");

            migrationBuilder.RenameIndex(
                name: "IX_Venta_ClienteId",
                table: "Ventas",
                newName: "IX_Ventas_ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_DetalleVenta_VentaId",
                table: "DetalleVentas",
                newName: "IX_DetalleVentas_VentaId");

            migrationBuilder.RenameIndex(
                name: "IX_DetalleVenta_PrendaId",
                table: "DetalleVentas",
                newName: "IX_DetalleVentas_PrendaId");

            migrationBuilder.AlterColumn<string>(
                name: "Nombre",
                table: "Prendas",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ventas",
                table: "Ventas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vendedores",
                table: "Vendedores",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MetodosPago",
                table: "MetodosPago",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DetalleVentas",
                table: "DetalleVentas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clientes",
                table: "Clientes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleVentas_Prendas_PrendaId",
                table: "DetalleVentas",
                column: "PrendaId",
                principalTable: "Prendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DetalleVentas_Ventas_VentaId",
                table: "DetalleVentas",
                column: "VentaId",
                principalTable: "Ventas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Prendas_Categorias_CategoriaId",
                table: "Prendas",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Clientes_ClienteId",
                table: "Ventas",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_MetodosPago_MetodoPagoId",
                table: "Ventas",
                column: "MetodoPagoId",
                principalTable: "MetodosPago",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ventas_Vendedores_VendedorId",
                table: "Ventas",
                column: "VendedorId",
                principalTable: "Vendedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
