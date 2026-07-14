#!/usr/bin/env dotnet-script

#r "nuget: System.Data.SqlClient, 4.8.5"
#r "nuget: Npgsql, 8.0.0"

using System.Data;
using System.Data.SqlClient;
using Npgsql;

Console.WriteLine("🔄 EJECUTANDO MIGRACIÓN SQL Server → Supabase...\n");

var sqlServerConnStr = "Server=ADMIN;Database=FashionStoreDB;Integrated Security=true;Connection Timeout=10;";
var supabaseConnStr = "Host=db.bajbvebkmacdnllnxvkv.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=MiFer2121092001;SSL Mode=Require;";

try
{
    // 1. Crear esquema
    Console.WriteLine("📐 Creando esquema en Supabase...");
    using (var conn = new NpgsqlConnection(supabaseConnStr))
    {
        conn.Open();
        var schema = @"
DROP TABLE IF EXISTS ""DetalleVentas"" CASCADE;
DROP TABLE IF EXISTS ""Ventas"" CASCADE;
DROP TABLE IF EXISTS ""Prendas"" CASCADE;
DROP TABLE IF EXISTS ""Categorias"" CASCADE;
DROP TABLE IF EXISTS ""Clientes"" CASCADE;
DROP TABLE IF EXISTS ""Vendedores"" CASCADE;
DROP TABLE IF EXISTS ""MetodoPago"" CASCADE;
DROP TABLE IF EXISTS ""DescuentosAutorizados"" CASCADE;
DROP TABLE IF EXISTS ""ConfiguracionSistema"" CASCADE;
DROP TABLE IF EXISTS ""ConfiguracionAuditoria"" CASCADE;

CREATE TABLE ""Categorias"" (""Id"" SERIAL PRIMARY KEY, ""Nombre"" VARCHAR(200) NOT NULL UNIQUE, ""Descripcion"" TEXT, ""FechaCreacion"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP);
CREATE TABLE ""Prendas"" (""Id"" SERIAL PRIMARY KEY, ""Nombre"" VARCHAR(200) NOT NULL UNIQUE, ""Descripcion"" TEXT, ""Precio"" NUMERIC(10,2) NOT NULL, ""Stock"" INTEGER NOT NULL DEFAULT 0, ""CategoriaId"" INTEGER NOT NULL, ""Activo"" BOOLEAN DEFAULT TRUE, ""FechaCreacion"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP, FOREIGN KEY (""CategoriaId"") REFERENCES ""Categorias""(""Id"") ON DELETE RESTRICT);
CREATE TABLE ""Clientes"" (""Id"" SERIAL PRIMARY KEY, ""Nombre"" VARCHAR(100) NOT NULL, ""Apellido"" VARCHAR(100), ""Telefono"" VARCHAR(20), ""Email"" VARCHAR(100), ""Direccion"" TEXT, ""Activo"" BOOLEAN DEFAULT TRUE, ""FechaCreacion"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP);
CREATE TABLE ""Vendedores"" (""Id"" SERIAL PRIMARY KEY, ""Nombre"" VARCHAR(100) NOT NULL, ""Apellido"" VARCHAR(100), ""Correo"" VARCHAR(100), ""Telefono"" VARCHAR(20), ""Cedula"" VARCHAR(20), ""Activo"" BOOLEAN DEFAULT TRUE, ""FechaCreacion"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP);
CREATE TABLE ""MetodoPago"" (""Id"" SERIAL PRIMARY KEY, ""Nombre"" VARCHAR(100) NOT NULL UNIQUE, ""Descripcion"" TEXT, ""Activo"" BOOLEAN DEFAULT TRUE);
CREATE TABLE ""DescuentosAutorizados"" (""Id"" SERIAL PRIMARY KEY, ""Nombre"" VARCHAR(100), ""Porcentaje"" NUMERIC(5,2), ""Activo"" BOOLEAN DEFAULT TRUE);
CREATE TABLE ""Ventas"" (""Id"" SERIAL PRIMARY KEY, ""ClienteId"" INTEGER, ""VendedorId"" INTEGER, ""MetodoPagoId"" INTEGER, ""Fecha"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP, ""Total"" NUMERIC(10,2) NOT NULL, ""Descuento"" NUMERIC(10,2) DEFAULT 0, FOREIGN KEY (""ClienteId"") REFERENCES ""Clientes""(""Id"") ON DELETE SET NULL, FOREIGN KEY (""VendedorId"") REFERENCES ""Vendedores""(""Id"") ON DELETE SET NULL, FOREIGN KEY (""MetodoPagoId"") REFERENCES ""MetodoPago""(""Id"") ON DELETE SET NULL);
CREATE TABLE ""DetalleVentas"" (""Id"" SERIAL PRIMARY KEY, ""VentaId"" INTEGER NOT NULL, ""PrendaId"" INTEGER NOT NULL, ""Cantidad"" INTEGER NOT NULL, ""Precio"" NUMERIC(10,2) NOT NULL, FOREIGN KEY (""VentaId"") REFERENCES ""Ventas""(""Id"") ON DELETE CASCADE, FOREIGN KEY (""PrendaId"") REFERENCES ""Prendas""(""Id"") ON DELETE RESTRICT);
CREATE TABLE ""ConfiguracionSistema"" (""Id"" SERIAL PRIMARY KEY, ""NombreTienda"" VARCHAR(200), ""RutaLogo"" VARCHAR(500), ""ColorPrimario"" VARCHAR(7), ""TemaOscuro"" BOOLEAN DEFAULT FALSE, ""FechaActualizacion"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP);
CREATE TABLE ""ConfiguracionAuditoria"" (""Id"" SERIAL PRIMARY KEY, ""RegistrarAccesos"" BOOLEAN DEFAULT TRUE, ""RegistrarCambios"" BOOLEAN DEFAULT TRUE, ""RegistrarErrores"" BOOLEAN DEFAULT TRUE, ""DiasBorrarLogs"" INTEGER DEFAULT 90);
CREATE INDEX idx_prendas_categoria ON ""Prendas""(""CategoriaId"");
CREATE INDEX idx_ventas_fecha ON ""Ventas""(""Fecha"" DESC);
CREATE INDEX idx_detalleventa_venta ON ""DetalleVentas""(""VentaId"");
";
        using var cmd = new NpgsqlCommand(schema, conn);
        cmd.ExecuteNonQuery();
    }
    Console.WriteLine("✅ Esquema creado\n");

    // 2. Migrar datos
    Console.WriteLine("📋 Migrando datos...\n");
    var tablas = new[] { "Categorias", "Prendas", "Clientes", "Vendedores", "MetodoPago", "DescuentosAutorizados", "Ventas", "DetalleVentas", "ConfiguracionSistema", "ConfiguracionAuditoria" };

    foreach (var tabla in tablas)
    {
        DataTable dt = new();
        using (var conn = new SqlConnection(sqlServerConnStr))
        {
            conn.Open();
            using var adapter = new SqlDataAdapter($"SELECT * FROM [{tabla}]", conn);
            adapter.Fill(dt);
        }

        if (dt.Rows.Count == 0)
        {
            Console.WriteLine($"  ⚠️  {tabla}: sin datos");
            continue;
        }

        using (var pgConn = new NpgsqlConnection(supabaseConnStr))
        {
            pgConn.Open();
            int inserted = 0;

            foreach (DataRow row in dt.Rows)
            {
                var cols = string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => $"\"{c.ColumnName}\""));
                var vals = string.Join(", ", dt.Columns.Cast<DataColumn>().Select((c, i) => row[i] is DBNull ? "NULL" : $"'{row[i].ToString().Replace("'", "''")}'"));
                var insertCmd = $"INSERT INTO \"{tabla}\" ({cols}) VALUES ({vals})";

                using var cmd = new NpgsqlCommand(insertCmd, pgConn);
                try { cmd.ExecuteNonQuery(); inserted++; } catch { }
            }

            Console.WriteLine($"  ✅ {tabla}: {inserted}/{dt.Rows.Count}");
        }
    }

    // 3. Validar
    Console.WriteLine("\n🔍 Validando...\n");
    Console.WriteLine("📊 Registros en Supabase:");

    using (var conn = new NpgsqlConnection(supabaseConnStr))
    {
        conn.Open();
        foreach (var tabla in new[] { "Categorias", "Prendas", "Clientes", "Vendedores", "MetodoPago", "Ventas", "DetalleVentas" })
        {
            using var cmd = new NpgsqlCommand($"SELECT COUNT(*) FROM \"{tabla}\"", conn);
            var count = (long)cmd.ExecuteScalar();
            Console.WriteLine($"  {tabla}: {count}");
        }
    }

    Console.WriteLine("\n✅ MIGRACIÓN COMPLETADA");
}
catch (Exception ex)
{
    Console.WriteLine($"\n❌ ERROR: {ex.Message}");
}
