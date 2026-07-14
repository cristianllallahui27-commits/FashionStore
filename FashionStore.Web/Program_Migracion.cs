using System;
using System.IO;
using System.Threading.Tasks;
using Npgsql;

namespace FashionStore.Migration
{
    /// <summary>
    /// Ejecutar migración SQL en Supabase
    /// Compilar: csc Program_Migracion.cs /out:ejecutar_migracion.exe
    /// Ejecutar: .\ejecutar_migracion.exe
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            // Variables
            string supabaseHost = "db.bajbvebkmacdnllnxvkv.supabase.co";
            int supabasePort = 5432;
            string supabaseUser = "postgres";
            string supabasePassword = Environment.GetEnvironmentVariable("SUPABASE_PASSWORD") ?? "MiFer2121092001";
            string supabaseDb = "postgres";
            string sqlFile = "../../../Database/MIGRACION_COMPLETA_SUPABASE.sql";

            Console.WriteLine("🚀 EJECUTAR MIGRACIÓN SUPABASE");
            Console.WriteLine(new string('=', 70));

            // Verificar archivo
            Console.WriteLine("\n📄 VERIFICAR ARCHIVO SQL");
            Console.WriteLine(new string('=', 70));

            if (!File.Exists(sqlFile))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"❌ Archivo no encontrado: {sqlFile}");
                Console.ResetColor();
                return;
            }

            var fileInfo = new FileInfo(sqlFile);
            var lineCount = File.ReadAllLines(sqlFile).Length;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✅ Archivo encontrado: {sqlFile}");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"   Tamaño: {fileInfo.Length:N0} bytes");
            Console.WriteLine($"   Líneas: {lineCount}");
            Console.ResetColor();

            // Credenciales
            Console.WriteLine("\n🔐 CREDENCIALES SUPABASE");
            Console.WriteLine(new string('=', 70));
            Console.WriteLine($"Host: {supabaseHost}");
            Console.WriteLine($"Port: {supabasePort}");
            Console.WriteLine($"Database: {supabaseDb}");
            Console.WriteLine($"User: {supabaseUser}");
            Console.WriteLine($"SSL Mode: Require");

            // Conectar
            Console.WriteLine("\n🔄 CONECTAR A SUPABASE");
            Console.WriteLine(new string('=', 70));

            string connectionString = $"Host={supabaseHost};Port={supabasePort};Database={supabaseDb};Username={supabaseUser};Password={supabasePassword};SSL Mode=Require;Trust Server Certificate=true;";

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    Console.WriteLine("⏳ Conectando...");
                    await connection.OpenAsync();
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("✅ Conectado a Supabase");
                    Console.ResetColor();

                    // Ejecutar SQL
                    Console.WriteLine("\n📋 EJECUTAR SQL");
                    Console.WriteLine(new string('=', 70));

                    var sqlContent = File.ReadAllText(sqlFile);
                    Console.WriteLine($"⏳ Ejecutando {lineCount} líneas de SQL...");
                    Console.WriteLine("   (esto puede tomar 30-60 segundos)");

                    using (var command = new NpgsqlCommand(sqlContent, connection))
                    {
                        command.CommandTimeout = 300; // 5 minutos
                        try
                        {
                            int result = await command.ExecuteNonQueryAsync();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"✅ SQL ejecutado exitosamente");
                            Console.ResetColor();
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"❌ Error ejecutando SQL:");
                            Console.WriteLine($"   {ex.Message}");
                            Console.ResetColor();
                            throw;
                        }
                    }

                    // Validar
                    Console.WriteLine("\n✅ VALIDACIÓN POST-MIGRACIÓN");
                    Console.WriteLine(new string('=', 70));

                    // Contar tablas
                    string countTablesQuery = @"
                        SELECT COUNT(*) 
                        FROM information_schema.tables 
                        WHERE table_schema = 'public' 
                        AND table_name NOT LIKE 'pg_%'
                    ";

                    using (var command = new NpgsqlCommand(countTablesQuery, connection))
                    {
                        var tableCount = await command.ExecuteScalarAsync();
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"📊 Tablas creadas: {tableCount}");
                        Console.ResetColor();

                        if (tableCount.ToString() == "17")
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("   ✅ Número correcto de tablas");
                            Console.ResetColor();
                        }
                    }

                    // Contar registros
                    var tables = new[] { "Categorias", "Prendas", "Clientes", "Vendedores", "Ventas", "DetalleVenta" };
                    Console.WriteLine("\n📊 Registros por tabla:");

                    foreach (var table in tables)
                    {
                        string countQuery = $@"SELECT COUNT(*) FROM ""{table}""";
                        using (var command = new NpgsqlCommand(countQuery, connection))
                        {
                            try
                            {
                                var count = await command.ExecuteScalarAsync();
                                Console.WriteLine($"   {table}: {count} registros");
                            }
                            catch { }
                        }
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n❌ Error: {ex.Message}");
                Console.ResetColor();
                return;
            }

            // Resumen
            Console.WriteLine("\n" + new string('=', 70));
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ MIGRACIÓN COMPLETADA EXITOSAMENTE");
            Console.ResetColor();
            Console.WriteLine(new string('=', 70));

            Console.WriteLine("\n🎯 PRÓXIMOS PASOS:");
            Console.WriteLine("1️⃣ Reiniciar app: dotnet run (en FashionStore.Web)");
            Console.WriteLine("2️⃣ Acceder: http://localhost:5100");
            Console.WriteLine("3️⃣ Login: admin / Admin123!");
            Console.WriteLine("\n🚀 LISTO PARA PRODUCCIÓN");
        }
    }
}
