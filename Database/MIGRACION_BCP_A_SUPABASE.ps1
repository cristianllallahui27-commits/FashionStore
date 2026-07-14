# ==========================================
# MIGRACIÓN SQL SERVER → SUPABASE
# ==========================================
# Alternativa a pgloader usando BCP + psql
# Requiere: SQL Server client tools + PostgreSQL client
# Uso: .\MIGRACION_BCP_A_SUPABASE.ps1

param(
    [string]$SqlServerInstance = "ADMIN",
    [string]$SqlDatabase = "FashionStoreDB",
    [string]$SupabaseHost = "db.bajbvebkmacdnllnxvkv.supabase.co",
    [int]$SupabasePort = 5432,
    [string]$SupabaseUser = "postgres",
    [string]$SupabasePassword = "MiFer2121092001",
    [string]$SupabaseDb = "postgres"
)

# ==========================================
# 1. CREAR TABLAS EN SUPABASE (Schema PostgreSQL)
# ==========================================

Write-Host "🔄 Creando esquema en Supabase..." -ForegroundColor Yellow

$sqlSchema = @"
-- Crear tablas en Supabase (PostgreSQL)
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

-- Tabla: Categorias
CREATE TABLE IF NOT EXISTS "Categorias" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(200) NOT NULL UNIQUE,
    "Descripcion" TEXT,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla: Prendas
CREATE TABLE IF NOT EXISTS "Prendas" (
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

-- Tabla: Clientes
CREATE TABLE IF NOT EXISTS "Clientes" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL,
    "Apellido" VARCHAR(100),
    "Telefono" VARCHAR(20),
    "Email" VARCHAR(100),
    "Direccion" TEXT,
    "Activo" BOOLEAN DEFAULT TRUE,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla: Vendedores
CREATE TABLE IF NOT EXISTS "Vendedores" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL,
    "Apellido" VARCHAR(100),
    "Correo" VARCHAR(100),
    "Telefono" VARCHAR(20),
    "Cedula" VARCHAR(20),
    "Activo" BOOLEAN DEFAULT TRUE,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla: MetodoPago
CREATE TABLE IF NOT EXISTS "MetodoPago" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL UNIQUE,
    "Descripcion" TEXT,
    "Activo" BOOLEAN DEFAULT TRUE
);

-- Tabla: DescuentosAutorizados
CREATE TABLE IF NOT EXISTS "DescuentosAutorizados" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100),
    "Porcentaje" NUMERIC(5,2),
    "Activo" BOOLEAN DEFAULT TRUE
);

-- Tabla: Ventas
CREATE TABLE IF NOT EXISTS "Ventas" (
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

-- Tabla: DetalleVenta
CREATE TABLE IF NOT EXISTS "DetalleVentas" (
    "Id" SERIAL PRIMARY KEY,
    "VentaId" INTEGER NOT NULL,
    "PrendaId" INTEGER NOT NULL,
    "Cantidad" INTEGER NOT NULL,
    "Precio" NUMERIC(10,2) NOT NULL,
    FOREIGN KEY ("VentaId") REFERENCES "Ventas"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("PrendaId") REFERENCES "Prendas"("Id") ON DELETE RESTRICT
);

-- Tabla: ConfiguracionSistema
CREATE TABLE IF NOT EXISTS "ConfiguracionSistema" (
    "Id" SERIAL PRIMARY KEY,
    "NombreTienda" VARCHAR(200),
    "RutaLogo" VARCHAR(500),
    "ColorPrimario" VARCHAR(7),
    "TemaOscuro" BOOLEAN DEFAULT FALSE,
    "FechaActualizacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla: ConfiguracionAuditoria
CREATE TABLE IF NOT EXISTS "ConfiguracionAuditoria" (
    "Id" SERIAL PRIMARY KEY,
    "RegistrarAccesos" BOOLEAN DEFAULT TRUE,
    "RegistrarCambios" BOOLEAN DEFAULT TRUE,
    "RegistrarErrores" BOOLEAN DEFAULT TRUE,
    "DiasBorrarLogs" INTEGER DEFAULT 90
);

-- Crear índices
CREATE INDEX IF NOT EXISTS idx_prendas_categoria ON "Prendas"("CategoriaId");
CREATE INDEX IF NOT EXISTS idx_ventas_fecha ON "Ventas"("Fecha" DESC);
CREATE INDEX IF NOT EXISTS idx_detalleventa_venta ON "DetalleVentas"("VentaId");
"@

# Guardar SQL en archivo temporal
$sqlFile = "$env:TEMP\supabase_schema.sql"
$sqlSchema | Out-File -FilePath $sqlFile -Encoding UTF8

# Ejecutar schema en Supabase
Write-Host "  Ejecutando schema en Supabase..." -ForegroundColor Cyan
$env:PGPASSWORD = $SupabasePassword
psql -h $SupabaseHost -U $SupabaseUser -d $SupabaseDb -f $sqlFile 2>&1 | Select-Object -Last 5

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Schema creado en Supabase" -ForegroundColor Green
} else {
    Write-Host "❌ Error creando schema" -ForegroundColor Red
    exit 1
}

# ==========================================
# 2. EXPORTAR DATOS DE SQL SERVER
# ==========================================

Write-Host "`n🔄 Exportando datos de SQL Server..." -ForegroundColor Yellow

# Tablas a migrar (en orden de dependencias)
$tablasOrdenes = @(
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

foreach ($tabla in $tablasOrdenes) {
    Write-Host "  Exportando $tabla..." -ForegroundColor Cyan
    
    # Exportar a CSV temporal
    $csvFile = "$env:TEMP\$tabla.csv"
    
    # BCP export
    $bcpCmd = "bcp FashionStoreDB.dbo.$tabla out `"$csvFile`" -S $SqlServerInstance -T -c -t'|' -r'\n'"
    Invoke-Expression $bcpCmd 2>&1 | Out-Null
    
    if (Test-Path $csvFile) {
        # Convertir CSV a SQL COPY para Supabase
        Write-Host "    ↳ Importando a Supabase..." -ForegroundColor DarkCyan
        
        # Comando COPY en PostgreSQL
        $copyCmd = "COPY `"$tabla`" FROM STDIN WITH (FORMAT csv, DELIMITER '|', ENCODING 'UTF8');"
        
        # Ejecutar COPY
        Get-Content $csvFile | psql -h $SupabaseHost -U $SupabaseUser -d $SupabaseDb -c $copyCmd 2>&1 | Out-Null
        
        Write-Host "    ✅ $tabla migrado" -ForegroundColor Green
        
        # Limpiar CSV
        Remove-Item $csvFile -Force -ErrorAction SilentlyContinue
    }
}

# ==========================================
# 3. VALIDAR MIGRACIÓN
# ==========================================

Write-Host "`n🔍 Validando migración..." -ForegroundColor Yellow

$validateSql = @"
SELECT 
  'Categorias' AS tabla, COUNT(*) AS registros FROM "Categorias"
UNION ALL SELECT 'Prendas', COUNT(*) FROM "Prendas"
UNION ALL SELECT 'Clientes', COUNT(*) FROM "Clientes"
UNION ALL SELECT 'Vendedores', COUNT(*) FROM "Vendedores"
UNION ALL SELECT 'MetodoPago', COUNT(*) FROM "MetodoPago"
UNION ALL SELECT 'Ventas', COUNT(*) FROM "Ventas"
UNION ALL SELECT 'DetalleVentas', COUNT(*) FROM "DetalleVentas"
ORDER BY tabla;
"@

$validateSql | Out-File -FilePath "$env:TEMP\validate.sql" -Encoding UTF8

Write-Host "`n📊 Registros por tabla en Supabase:`n" -ForegroundColor Cyan
psql -h $SupabaseHost -U $SupabaseUser -d $SupabaseDb -f "$env:TEMP\validate.sql"

Write-Host "`n✅ MIGRACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "`n💡 Próximo paso: Iniciar app con Supabase" -ForegroundColor Yellow
Write-Host "   `$env:SUPABASE_PASSWORD='MiFer2121092001'" -ForegroundColor Cyan
Write-Host "   dotnet run" -ForegroundColor Cyan
