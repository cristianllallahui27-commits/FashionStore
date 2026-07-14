# ==========================================
# MIGRACIÓN DIRECTA: SQL Server → Supabase
# ==========================================
# Sin dependencias: usa .NET + Npgsql + SqlClient

param(
    [string]$SqlServer = "ADMIN",
    [string]$SqlDb = "FashionStoreDB",
    [string]$SupabaseHost = "db.bajbvebkmacdnllnxvkv.supabase.co",
    [string]$SupabaseUser = "postgres",
    [string]$SupabasePass = "MiFer2121092001"
)

Write-Host "🔄 Iniciando migración SQL Server → Supabase..`n" -ForegroundColor Cyan

# ==========================================
# 1. VERIFICAR CONECTIVIDAD
# ==========================================

Write-Host "🔍 Verificando conectividad..." -ForegroundColor Yellow

# SQL Server
try {
    $sqlConn = New-Object System.Data.SqlClient.SqlConnection
    $sqlConn.ConnectionString = "Server=$SqlServer;Database=$SqlDb;Integrated Security=true;Connection Timeout=5;"
    $sqlConn.Open()
    $sqlCmd = $sqlConn.CreateCommand()
    $sqlCmd.CommandText = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE'"
    $tableCount = $sqlCmd.ExecuteScalar()
    $sqlConn.Close()
    Write-Host "  ✅ SQL Server ($SqlServer): $tableCount tablas" -ForegroundColor Green
} catch {
    Write-Host "  ❌ SQL Server no disponible: $_" -ForegroundColor Red
    exit 1
}

# Supabase
try {
    $pgConn = New-Object Npgsql.NpgsqlConnection
    $pgConn.ConnectionString = "Host=$SupabaseHost;Port=5432;Database=postgres;Username=$SupabaseUser;Password=$SupabasePass;SSL Mode=Require;"
    $pgConn.Open()
    $pgCmd = $pgConn.CreateCommand()
    $pgCmd.CommandText = "SELECT 1"
    $pgCmd.ExecuteScalar() | Out-Null
    $pgConn.Close()
    Write-Host "  ✅ Supabase (PostgreSQL): Conectado`n" -ForegroundColor Green
} catch {
    Write-Host "  ❌ Supabase no disponible: $_" -ForegroundColor Red
    exit 1
}

# ==========================================
# 2. CREAR ESQUEMA EN SUPABASE
# ==========================================

Write-Host "📐 Creando esquema en Supabase..." -ForegroundColor Yellow

$pgConn = New-Object Npgsql.NpgsqlConnection
$pgConn.ConnectionString = "Host=$SupabaseHost;Port=5432;Database=postgres;Username=$SupabaseUser;Password=$SupabasePass;SSL Mode=Require;"
$pgConn.Open()

$schema = @"
DROP TABLE IF EXISTS "DetalleVentas" CASCADE;
DROP TABLE IF EXISTS "Ventas" CASCADE;
DROP TABLE IF EXISTS "Prendas" CASCADE;
DROP TABLE IF EXISTS "Categorias" CASCADE;
DROP TABLE IF EXISTS "Clientes" CASCADE;
DROP TABLE IF EXISTS "Vendedores" CASCADE;
DROP TABLE IF EXISTS "MetodoPago" CASCADE;
DROP TABLE IF EXISTS "DescuentosAutorizados" CASCADE;
DROP TABLE IF EXISTS "ConfiguracionSistema" CASCADE;
DROP TABLE IF EXISTS "ConfiguracionAuditoria" CASCADE;

CREATE TABLE "Categorias" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(200) NOT NULL UNIQUE,
    "Descripcion" TEXT,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "Prendas" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(200) NOT NULL UNIQUE,
    "Descripcion" TEXT,
    "Precio" NUMERIC(10,2) NOT NULL,
    "Stock" INTEGER NOT NULL DEFAULT 0,
    "CategoriaId" INTEGER NOT NULL,
    "Activo" BOOLEAN DEFAULT TRUE,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY ("CategoriaId") REFERENCES "Categorias"("Id") ON DELETE RESTRICT
);

CREATE TABLE "Clientes" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL,
    "Apellido" VARCHAR(100),
    "Telefono" VARCHAR(20),
    "Email" VARCHAR(100),
    "Direccion" TEXT,
    "Activo" BOOLEAN DEFAULT TRUE,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "Vendedores" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL,
    "Apellido" VARCHAR(100),
    "Correo" VARCHAR(100),
    "Telefono" VARCHAR(20),
    "Cedula" VARCHAR(20),
    "Activo" BOOLEAN DEFAULT TRUE,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "MetodoPago" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL UNIQUE,
    "Descripcion" TEXT,
    "Activo" BOOLEAN DEFAULT TRUE
);

CREATE TABLE "DescuentosAutorizados" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100),
    "Porcentaje" NUMERIC(5,2),
    "Activo" BOOLEAN DEFAULT TRUE
);

CREATE TABLE "Ventas" (
    "Id" SERIAL PRIMARY KEY,
    "ClienteId" INTEGER,
    "VendedorId" INTEGER,
    "MetodoPagoId" INTEGER,
    "Fecha" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "Total" NUMERIC(10,2) NOT NULL,
    "Descuento" NUMERIC(10,2) DEFAULT 0,
    FOREIGN KEY ("ClienteId") REFERENCES "Clientes"("Id") ON DELETE SET NULL,
    FOREIGN KEY ("VendedorId") REFERENCES "Vendedores"("Id") ON DELETE SET NULL,
    FOREIGN KEY ("MetodoPagoId") REFERENCES "MetodoPago"("Id") ON DELETE SET NULL
);

CREATE TABLE "DetalleVentas" (
    "Id" SERIAL PRIMARY KEY,
    "VentaId" INTEGER NOT NULL,
    "PrendaId" INTEGER NOT NULL,
    "Cantidad" INTEGER NOT NULL,
    "Precio" NUMERIC(10,2) NOT NULL,
    FOREIGN KEY ("VentaId") REFERENCES "Ventas"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("PrendaId") REFERENCES "Prendas"("Id") ON DELETE RESTRICT
);

CREATE TABLE "ConfiguracionSistema" (
    "Id" SERIAL PRIMARY KEY,
    "NombreTienda" VARCHAR(200),
    "RutaLogo" VARCHAR(500),
    "ColorPrimario" VARCHAR(7),
    "TemaOscuro" BOOLEAN DEFAULT FALSE,
    "FechaActualizacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "ConfiguracionAuditoria" (
    "Id" SERIAL PRIMARY KEY,
    "RegistrarAccesos" BOOLEAN DEFAULT TRUE,
    "RegistrarCambios" BOOLEAN DEFAULT TRUE,
    "RegistrarErrores" BOOLEAN DEFAULT TRUE,
    "DiasBorrarLogs" INTEGER DEFAULT 90
);

CREATE INDEX idx_prendas_categoria ON "Prendas"("CategoriaId");
CREATE INDEX idx_ventas_fecha ON "Ventas"("Fecha" DESC);
CREATE INDEX idx_detalleventa_venta ON "DetalleVentas"("VentaId");
"@

$pgCmd = $pgConn.CreateCommand()
$pgCmd.CommandText = $schema
$pgCmd.ExecuteNonQuery() | Out-Null

$pgConn.Close()
Write-Host "  ✅ Esquema creado`n" -ForegroundColor Green

# ==========================================
# 3. MIGRAR DATOS
# ==========================================

Write-Host "📋 Migrando datos...`n" -ForegroundColor Yellow

$tablas = @(
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
)

foreach ($tabla in $tablas) {
    # Leer de SQL Server
    $sqlConn = New-Object System.Data.SqlClient.SqlConnection
    $sqlConn.ConnectionString = "Server=$SqlServer;Database=$SqlDb;Integrated Security=true;"
    $sqlConn.Open()
    
    $sqlCmd = $sqlConn.CreateCommand()
    $sqlCmd.CommandText = "SELECT * FROM [$tabla]"
    
    $adapter = New-Object System.Data.SqlClient.SqlDataAdapter
    $adapter.SelectCommand = $sqlCmd
    
    $dt = New-Object System.Data.DataTable
    $adapter.Fill($dt) | Out-Null
    
    $sqlConn.Close()
    
    $rowCount = $dt.Rows.Count
    
    if ($rowCount -eq 0) {
        Write-Host "  ⚠️  $tabla : sin datos" -ForegroundColor DarkYellow
        continue
    }
    
    # Insertar en Supabase
    $pgConn = New-Object Npgsql.NpgsqlConnection
    $pgConn.ConnectionString = "Host=$SupabaseHost;Port=5432;Database=postgres;Username=$SupabaseUser;Password=$SupabasePass;SSL Mode=Require;"
    $pgConn.Open()
    
    $inserted = 0
    foreach ($row in $dt.Rows) {
        $cols = @()
        $vals = @()
        
        for ($i = 0; $i -lt $dt.Columns.Count; $i++) {
            $colName = $dt.Columns[$i].ColumnName
            $colValue = $row[$i]
            
            $cols += "`"$colName`""
            
            if ($colValue -is [System.DBNull]) {
                $vals += "NULL"
            } else {
                $vals += "'$($colValue.ToString().Replace("'", "''"))'".Replace("'NULL'", "NULL")
            }
        }
        
        $insertCmd = "INSERT INTO `"$tabla`" ($([string]::Join(", ", $cols))) VALUES ($([string]::Join(", ", $vals)))"
        
        $pgCmd = $pgConn.CreateCommand()
        $pgCmd.CommandText = $insertCmd
        
        try {
            $pgCmd.ExecuteNonQuery() | Out-Null
            $inserted++
        } catch {
            # Ignorar duplicados y errores de constraint
        }
    }
    
    $pgConn.Close()
    Write-Host "  ✅ $tabla : $inserted/$rowCount registros" -ForegroundColor Green
}

# ==========================================
# 4. VALIDAR
# ==========================================

Write-Host "`n🔍 Validando migración...`n" -ForegroundColor Yellow

$pgConn = New-Object Npgsql.NpgsqlConnection
$pgConn.ConnectionString = "Host=$SupabaseHost;Port=5432;Database=postgres;Username=$SupabaseUser;Password=$SupabasePass;SSL Mode=Require;"
$pgConn.Open()

Write-Host "📊 Registros por tabla en Supabase:`n" -ForegroundColor Cyan

foreach ($tabla in $tablas) {
    $pgCmd = $pgConn.CreateCommand()
    $pgCmd.CommandText = "SELECT COUNT(*) FROM `"$tabla`""
    $count = $pgCmd.ExecuteScalar()
    Write-Host "  $tabla : $count registros" -ForegroundColor Green
}

$pgConn.Close()

Write-Host "`n✅ MIGRACIÓN COMPLETADA`n" -ForegroundColor Green
Write-Host "💡 Próximo paso: Ejecutar app" -ForegroundColor Yellow
Write-Host "   `$env:SUPABASE_PASSWORD='MiFer2121092001'" -ForegroundColor Cyan
Write-Host "   dotnet run" -ForegroundColor Cyan
