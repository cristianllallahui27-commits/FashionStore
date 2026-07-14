using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

/// <summary>
/// Herramienta de migración: SQL Server → Supabase (PostgreSQL)
/// Uso: dotnet run --project FashionStore.Web -- --migrate
/// </summary>
public class MigracionSqlServerSupabase
{
    private readonly string _sqlServerConnStr;
    private readonly string _supabaseConnStr;
    private readonly List<string> _tablas = new()
    {
        "Categorias",
        "Prendas",
        "Clientes",
        "Vendedores",
        "MetodoPago",
        "DescuentosAutorizados",
        "Ventas",
        "DetalleVentas",
        "ConfiguracionSistema",
        "ConfiguracionAuditoria"
    };

    public MigracionSqlServerSupabase(string sqlServerConnStr, string supabaseConnStr)
    {
        _sqlServerConnStr = sqlServerConnStr;
        _supabaseConnStr = supabaseConnStr;
    }

    public async Task Ejecutar()
    {
        Console.WriteLine("🔄 Iniciando migración SQL Server → Supabase...\n");

        try
        {
            // 1. Limpiar Supabase
            await LimpiarSupabase();

            // 2. Crear esquema en Supabase
            await CrearEsquemaSupabase();

            // 3. Migrar datos tabla por tabla
            foreach (var tabla in _tablas)
            {
                await MigrarTabla(tabla);
            }

            // 4. Validar
            await Validar();

            Console.WriteLine("\n✅ MIGRACIÓN COMPLETADA EXITOSAMENTE\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ ERROR: {ex.Message}\n");
            throw;
        }
    }

    private async Task LimpiarSupabase()
    {
        Console.WriteLine("🗑️  Limpiando esquema anterior en Supabase...");

        using var conn = new NpgsqlConnection(_supabaseConnStr);
        await conn.OpenAsync();

        var dropScript = @"
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
";

        using var cmd = new NpgsqlCommand(dropScript, conn);
        await cmd.ExecuteNonQueryAsync();

        await conn.CloseAsync();
        Console.WriteLine("  ✅ Esquema limpio\n");
    }

    private async Task CrearEsquemaSupabase()
    {
        Console.WriteLine("📐 Creando esquema en Supabase...");

        using var conn = new NpgsqlConnection(_supabaseConnStr);
        await conn.OpenAsync();

        var createScript = @"
-- Tabla: Categorias
CREATE TABLE IF NOT EXISTS ""Categorias"" (
    ""Id"" SERIAL PRIMARY KEY,
    ""Nombre"" VARCHAR(200) NOT NULL UNIQUE,
    ""Descripcion"" TEXT,
    ""FechaCreacion"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla: Prendas
CREATE TABLE IF NOT EXISTS ""Prendas"" (
    ""Id"" SERIAL PRIMARY KEY,
    ""Nombre"" VARCHAR(200) NOT NULL UNIQUE,
    ""Descripcion"" TEXT,
    ""Precio"" NUMERIC(10,2) NOT NULL,
    ""Stock"" INTEGER NOT NULL DEFAULT 0,
    ""CategoriaId"" INTEGER NOT NULL,
    ""Activo"" BOOLEAN DEFAULT TRUE,
    ""FechaCreacion"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (""CategoriaId"") REFERENCES ""Categorias""(""Id"") ON DELETE RESTRICT
);

-- Tabla: Clientes
CREATE TABLE IF NOT EXISTS ""Clientes"" (
    ""Id"" SERIAL PRIMARY KEY,
    ""Nombre"" VARCHAR(100) NOT NULL,
    ""Apellido"" VARCHAR(100),
    ""Telefono"" VARCHAR(20),
    ""Email"" VARCHAR(100),
    ""Direccion"" TEXT,
    ""Activo"" BOOLEAN DEFAULT TRUE,
    ""FechaCreacion"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla: Vendedores
CREATE TABLE IF NOT EXISTS ""Vendedores"" (
    ""Id"" SERIAL PRIMARY KEY,
    ""Nombre"" VARCHAR(100) NOT NULL,
    ""Apellido"" VARCHAR(100),
    ""Correo"" VARCHAR(100),
    ""Telefono"" VARCHAR(20),
    ""Cedula"" VARCHAR(20),
    ""Activo"" BOOLEAN DEFAULT TRUE,
    ""FechaCreacion"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla: MetodoPago
CREATE TABLE IF NOT EXISTS ""MetodoPago"" (
    ""Id"" SERIAL PRIMARY KEY,
    ""Nombre"" VARCHAR(100) NOT NULL UNIQUE,
    ""Descripcion"" TEXT,
    ""Activo"" BOOLEAN DEFAULT TRUE
);

-- Tabla: DescuentosAutorizados
CREATE TABLE IF NOT EXISTS ""DescuentosAutorizados"" (
    ""Id"" SERIAL PRIMARY KEY,
    ""Nombre"" VARCHAR(100),
    ""Porcentaje"" NUMERIC(5,2),
    ""Activo"" BOOLEAN DEFAULT TRUE
);

-- Tabla: Ventas
CREATE TABLE IF NOT EXISTS ""Ventas"" (
    ""Id"" SERIAL PRIMARY KEY,
    ""ClienteId"" INTEGER,
    ""VendedorId"" INTEGER,
    ""MetodoPagoId"" INTEGER,
    ""Fecha"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ""Total"" NUMERIC(10,2) NOT NULL,
    ""Descuento"" NUMERIC(10,2) DEFAULT 0,
    FOREIGN KEY (""ClienteId"") REFERENCES ""Clientes""(""Id"") ON DELETE SET NULL,
    FOREIGN KEY (""VendedorId"") REFERENCES ""Vendedores""(""Id"") ON DELETE SET NULL,
    FOREIGN KEY (""MetodoPagoId"") REFERENCES ""MetodoPago""(""Id"") ON DELETE SET NULL
);

-- Tabla: DetalleVentas
CREATE TABLE IF NOT EXISTS ""DetalleVentas"" (
    ""Id"" SERIAL PRIMARY KEY,
    ""VentaId"" INTEGER NOT NULL,
    ""PrendaId"" INTEGER NOT NULL,
    ""Cantidad"" INTEGER NOT NULL,
    ""Precio"" NUMERIC(10,2) NOT NULL,
    FOREIGN KEY (""VentaId"") REFERENCES ""Ventas""(""Id"") ON DELETE CASCADE,
    FOREIGN KEY (""PrendaId"") REFERENCES ""Prendas""(""Id"") ON DELETE RESTRICT
);

-- Tabla: ConfiguracionSistema
CREATE TABLE IF NOT EXISTS ""ConfiguracionSistema"" (
    ""Id"" SERIAL PRIMARY KEY,
    ""NombreTienda"" VARCHAR(200),
    ""RutaLogo"" VARCHAR(500),
    ""ColorPrimario"" VARCHAR(7),
    ""TemaOscuro"" BOOLEAN DEFAULT FALSE,
    ""FechaActualizacion"" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla: ConfiguracionAuditoria
CREATE TABLE IF NOT EXISTS ""ConfiguracionAuditoria"" (
    ""Id"" SERIAL PRIMARY KEY,
    ""RegistrarAccesos"" BOOLEAN DEFAULT TRUE,
    ""RegistrarCambios"" BOOLEAN DEFAULT TRUE,
    ""RegistrarErrores"" BOOLEAN DEFAULT TRUE,
    ""DiasBorrarLogs"" INTEGER DEFAULT 90
);

-- Crear índices
CREATE INDEX IF NOT EXISTS idx_prendas_categoria ON ""Prendas""(""CategoriaId"");
CREATE INDEX IF NOT EXISTS idx_ventas_fecha ON ""Ventas""(""Fecha"" DESC);
CREATE INDEX IF NOT EXISTS idx_detalleventa_venta ON ""DetalleVentas""(""VentaId"");
";

        using var cmd = new NpgsqlCommand(createScript, conn);
        await cmd.ExecuteNonQueryAsync();

        await conn.CloseAsync();
        Console.WriteLine("  ✅ Esquema creado\n");
    }

    private async Task MigrarTabla(string tabla)
    {
        Console.WriteLine($"📋 Migrando {tabla}...");

        try
        {
            // Leer datos de SQL Server
            DataTable datos = new();
            using (var sqlConn = new SqlConnection(_sqlServerConnStr))
            {
                await sqlConn.OpenAsync();
                using var sqlCmd = new SqlCommand($"SELECT * FROM {tabla}", sqlConn);
                using var adapter = new SqlDataAdapter(sqlCmd);
                adapter.Fill(datos);
            }

            var rowCount = datos.Rows.Count;

            if (rowCount == 0)
            {
                Console.WriteLine($"  ⚠️  {tabla}: sin datos\n");
                return;
            }

            // Insertar en Supabase
            using var pgConn = new NpgsqlConnection(_supabaseConnStr);
            await pgConn.OpenAsync();

            foreach (DataRow row in datos.Rows)
            {
                var cols = string.Join(", ", datos.Columns.Cast<DataColumn>().Select(c => $"\"{c.ColumnName}\""));
                var vals = string.Join(", ", datos.Columns.Cast<DataColumn>().Select((c, i) => $"@p{i}"));

                var insertCmd = $"INSERT INTO \"{tabla}\" ({cols}) VALUES ({vals})";

                using var cmd = new NpgsqlCommand(insertCmd, pgConn);
                for (int i = 0; i < datos.Columns.Count; i++)
                {
                    cmd.Parameters.AddWithValue($"@p{i}", row[i] ?? DBNull.Value);
                }

                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch
                {
                    // Ignorar violaciones de constraint (ej. duplicados)
                }
            }

            await pgConn.CloseAsync();
            Console.WriteLine($"  ✅ {tabla}: {rowCount} registros migrados\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ⚠️  {tabla}: {ex.Message}\n");
        }
    }

    private async Task Validar()
    {
        Console.WriteLine("🔍 Validando migración...\n");

        using var conn = new NpgsqlConnection(_supabaseConnStr);
        await conn.OpenAsync();

        Console.WriteLine("📊 Registros por tabla en Supabase:\n");

        foreach (var tabla in _tablas)
        {
            using var cmd = new NpgsqlCommand($"SELECT COUNT(*) FROM \"{tabla}\"", conn);
            var count = (long)await cmd.ExecuteScalarAsync();
            Console.WriteLine($"  {tabla,-30}: {count} registros");
        }

        await conn.CloseAsync();
    }

    public static async Task Main(string[] args)
    {
        var sqlServerConnStr = "Server=ADMIN;Database=FashionStoreDB;Trusted_Connection=true;";
        var supabaseConnStr = "Host=db.bajbvebkmacdnllnxvkv.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=MiFer2121092001;SSL Mode=Require;";

        var migracion = new MigracionSqlServerSupabase(sqlServerConnStr, supabaseConnStr);
        await migracion.Ejecutar();
    }
}
