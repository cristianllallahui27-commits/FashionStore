#!/usr/bin/env dotnet-script
/**
 * Ejecutar migración SQL en Supabase usando Npgsql
 * 
 * Uso: dotnet script ExecuteMigration.cs
 * O: dotnet run
 */

#r "nuget: Npgsql, 9.0.0"

using System;
using System.IO;
using System.Threading.Tasks;
using Npgsql;

// ==========================================
// VARIABLES
// ==========================================
string supabaseHost = "db.bajbvebkmacdnllnxvkv.supabase.co";
int supabasePort = 5432;
string supabaseUser = "postgres";
string supabasePassword = Environment.GetEnvironmentVariable("SUPABASE_PASSWORD") ?? "MiFer2121092001";
string supabaseDb = "postgres";
string sqlFile = "MIGRACION_COMPLETA_SUPABASE.sql";

Console.WriteLine("🚀 EJECUTAR MIGRACIÓN SUPABASE");
Console.WriteLine(new string('=', 70));

// ==========================================
// VERIFICAR REQUISITOS
// ==========================================
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

// ==========================================
// CREDENCIALES
// ==========================================
Console.WriteLine("\n🔐 CREDENCIALES SUPABASE");
Console.WriteLine(new string('=', 70));
Console.WriteLine($"Host: {supabaseHost}");
Console.WriteLine($"Port: {supabasePort}");
Console.WriteLine($"Database: {supabaseDb}");
Console.WriteLine($"User: {supabaseUser}");
Console.WriteLine($"SSL Mode: Require");

// ==========================================
// CONECTAR
// ==========================================
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

        // ==========================================
        // EJECUTAR SQL
        // ==========================================
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

        // ==========================================
        // VALIDAR
        // ==========================================
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
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   ⚠️  Se esperaban 17 tablas, se encontraron {tableCount}");
                Console.ResetColor();
            }
        }

        // Contar registros principales
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
                catch
                {
                    Console.WriteLine($"   {table}: (no encontrada)");
                }
            }
        }

        // Verificar usuario admin
        Console.WriteLine("\n🔐 Verificar usuario Administrador:");
        string adminQuery = @"SELECT ""UserName"", ""Email"" FROM public.""AspNetUsers"" WHERE ""UserName"" = 'admin' LIMIT 1";
        using (var command = new NpgsqlCommand(adminQuery, connection))
        {
            try
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        string username = reader["UserName"].ToString();
                        string email = reader["Email"].ToString();
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"   ✅ Usuario encontrado: {username} ({email})");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("   ⚠️  Usuario admin no encontrado");
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"   ⚠️  No se pudo verificar usuario: {ex.Message}");
                Console.ResetColor();
            }
        }

        connection.Close();
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n❌ Error de conexión:");
    Console.WriteLine($"   {ex.Message}");
    Console.ResetColor();
    return;
}

// ==========================================
// RESUMEN
// ==========================================
Console.WriteLine("\n" + new string('=', 70));
Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✅ MIGRACIÓN COMPLETADA EXITOSAMENTE");
Console.ResetColor();
Console.WriteLine(new string('=', 70));

Console.WriteLine("\n🎯 PRÓXIMOS PASOS:");
Console.WriteLine("\n1️⃣ Reiniciar aplicación:");
Console.WriteLine("   $env:SUPABASE_PASSWORD='MiFer2121092001'");
Console.WriteLine("   cd FashionStore.Web");
Console.WriteLine("   dotnet run");

Console.WriteLine("\n2️⃣ Acceder a la app:");
Console.WriteLine("   http://localhost:5100");

Console.WriteLine("\n3️⃣ Login:");
Console.WriteLine("   Username: admin");
Console.WriteLine("   Password: Admin123!");

Console.WriteLine("\n4️⃣ Verificar en Supabase:");
Console.WriteLine("   https://supabase.com/dashboard/project/bajbvebkmacdnllnxvkv/editor");

Console.WriteLine("\n" + new string('=', 70));
Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("🚀 LISTO PARA PRODUCCIÓN");
Console.ResetColor();
