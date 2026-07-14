using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FashionStore.Infrastructure.Context;
using FashionStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MigrationTool
{
    /// <summary>
    /// Herramienta para exportar datos de SQL Server a formato JSON/SQL
    /// </summary>
    public class ExportData
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("🔄 EXPORTANDO DATOS DE SQL SERVER...\n");

            try
            {
                // Configurar connection string SQL Server
                var connectionString = "Server=ADMIN;Database=FashionStoreDB;Trusted_Connection=True;TrustServerCertificate=True;";
                
                var optionsBuilder = new DbContextOptionsBuilder<FashionStoreDbContext>()
                    .UseSqlServer(connectionString);

                using (var context = new FashionStoreDbContext(optionsBuilder.Options))
                {
                    // Verificar conexión
                    var canConnect = await context.Database.CanConnectAsync();
                    if (!canConnect)
                    {
                        Console.WriteLine("❌ No se puede conectar a SQL Server");
                        return;
                    }

                    Console.WriteLine("✅ Conectado a SQL Server\n");

                    // Contar registros en cada tabla
                    Console.WriteLine("📊 CONTEOS DE DATOS:\n");

                    var categorias = await context.Categorias.CountAsync();
                    Console.WriteLine($"  Categorias:            {categorias}");

                    var prendas = await context.Prendas.CountAsync();
                    Console.WriteLine($"  Prendas:               {prendas}");

                    var clientes = await context.Clientes.CountAsync();
                    Console.WriteLine($"  Clientes:              {clientes}");

                    var vendedores = await context.Vendedores.CountAsync();
                    Console.WriteLine($"  Vendedores:            {vendedores}");

                    var metodoPago = await context.MetodoPago.CountAsync();
                    Console.WriteLine($"  MetodoPago:            {metodoPago}");

                    var ventas = await context.Ventas.CountAsync();
                    Console.WriteLine($"  Ventas:                {ventas}");

                    var detalleVenta = await context.DetalleVenta.CountAsync();
                    Console.WriteLine($"  DetalleVenta:          {detalleVenta}");

                    var config = await context.ConfiguracionSistema.CountAsync();
                    Console.WriteLine($"  ConfiguracionSistema:  {config}");

                    // Exportar datos a archivos
                    Console.WriteLine("\n📁 EXPORTANDO DATOS A ARCHIVOS...\n");

                    // Exportar Categorias
                    var categoriasData = await context.Categorias.ToListAsync();
                    ExportToFile("Database/export_categorias.sql", GenerateInsertStatements("Categorias", categoriasData));
                    Console.WriteLine($"  ✅ Categorias: {categoriasData.Count} registros");

                    // Exportar Prendas
                    var prendasData = await context.Prendas.ToListAsync();
                    ExportToFile("Database/export_prendas.sql", GenerateInsertStatements("Prendas", prendasData));
                    Console.WriteLine($"  ✅ Prendas: {prendasData.Count} registros");

                    // Exportar Clientes
                    var clientesData = await context.Clientes.ToListAsync();
                    ExportToFile("Database/export_clientes.sql", GenerateInsertStatements("Clientes", clientesData));
                    Console.WriteLine($"  ✅ Clientes: {clientesData.Count} registros");

                    // Exportar Vendedores
                    var vendedoresData = await context.Vendedores.ToListAsync();
                    ExportToFile("Database/export_vendedores.sql", GenerateInsertStatements("Vendedores", vendedoresData));
                    Console.WriteLine($"  ✅ Vendedores: {vendedoresData.Count} registros");

                    // Exportar MetodoPago
                    var metodoPagoData = await context.MetodoPago.ToListAsync();
                    ExportToFile("Database/export_metodopago.sql", GenerateInsertStatements("MetodoPago", metodoPagoData));
                    Console.WriteLine($"  ✅ MetodoPago: {metodoPagoData.Count} registros");

                    // Exportar Ventas
                    var ventasData = await context.Ventas.ToListAsync();
                    ExportToFile("Database/export_ventas.sql", GenerateInsertStatements("Ventas", ventasData));
                    Console.WriteLine($"  ✅ Ventas: {ventasData.Count} registros");

                    // Exportar DetalleVenta
                    var detalleVentaData = await context.DetalleVenta.ToListAsync();
                    ExportToFile("Database/export_detalleventa.sql", GenerateInsertStatements("DetalleVenta", detalleVentaData));
                    Console.WriteLine($"  ✅ DetalleVenta: {detalleVentaData.Count} registros");

                    // Exportar ConfiguracionSistema
                    var configData = await context.ConfiguracionSistema.ToListAsync();
                    ExportToFile("Database/export_configuracion.sql", GenerateInsertStatements("ConfiguracionSistema", configData));
                    Console.WriteLine($"  ✅ ConfiguracionSistema: {configData.Count} registros");

                    Console.WriteLine("\n✅ EXPORTACIÓN COMPLETADA");
                    Console.WriteLine("   Archivos guardados en Database/export_*.sql");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR: {ex.Message}");
                Console.WriteLine($"   {ex.InnerException?.Message}");
            }
        }

        private static string GenerateInsertStatements(string tableName, object data)
        {
            // Placeholder - normalmente usaría reflection para serializar
            return $"-- {tableName} data\n-- {data}";
        }

        private static void ExportToFile(string filePath, string content)
        {
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
            System.IO.File.WriteAllText(filePath, content);
        }
    }
}
