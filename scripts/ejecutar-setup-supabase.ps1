# ==========================================
# EJECUTAR SETUP SUPABASE
# ==========================================
# Script para crear tablas + datos en Supabase
# Uso: .\scripts\ejecutar-setup-supabase.ps1

param(
    [string]$Password = "MiFer2121092001",
    [string]$Host = "db.bajbvebkmacdnllnxvkv.supabase.co",
    [int]$Port = 5432,
    [string]$Database = "postgres",
    [string]$User = "postgres"
)

Write-Host "🔄 Conectando a Supabase..." -ForegroundColor Cyan

# Leer script SQL
$scriptPath = "$PSScriptRoot\..\Database\SUPABASE_SETUP_COMPLETO.sql"
if (-not (Test-Path $scriptPath)) {
    Write-Host "❌ Script no encontrado: $scriptPath" -ForegroundColor Red
    exit 1
}

$sqlScript = Get-Content $scriptPath -Raw

# Crear connection string
$connString = "Host=$Host;Port=$Port;Database=$Database;Username=$User;Password=$Password;SSL Mode=Require;Trust Server Certificate=false;"

Write-Host "📝 Ejecutando setup..." -ForegroundColor Yellow

try {
    # Cargar Npgsql
    Add-Type -Path "$PSScriptRoot\..\FashionStore.Web\bin\Release\net9.0\Npgsql.dll"
    
    $conn = New-Object Npgsql.NpgsqlConnection($connString)
    $conn.Open()
    
    Write-Host "✅ Conectado a Supabase" -ForegroundColor Green
    
    # Ejecutar script
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sqlScript
    $cmd.ExecuteNonQuery()
    
    Write-Host "✅ Setup completado" -ForegroundColor Green
    
    # Validar
    $cmd.CommandText = @"
SELECT 
  'Categorias' AS tabla, COUNT(*) AS registros FROM public.Categorias
UNION ALL SELECT 'Prendas', COUNT(*) FROM public.Prendas
UNION ALL SELECT 'Clientes', COUNT(*) FROM public.Clientes
UNION ALL SELECT 'Vendedores', COUNT(*) FROM public.Vendedores
UNION ALL SELECT 'MetodoPago', COUNT(*) FROM public.MetodoPago
UNION ALL SELECT 'Ventas', COUNT(*) FROM public.Ventas
UNION ALL SELECT 'DetalleVenta', COUNT(*) FROM public.DetalleVenta
UNION ALL SELECT 'ConfiguracionSistema', COUNT(*) FROM public.ConfiguracionSistema
UNION ALL SELECT 'ConfiguracionAuditoria', COUNT(*) FROM public.ConfiguracionAuditoria;
"@
    
    $reader = $cmd.ExecuteReader()
    Write-Host "`n📊 Registros por tabla:" -ForegroundColor Cyan
    while ($reader.Read()) {
        $tabla = $reader["tabla"].ToString()
        $registros = $reader["registros"].ToString()
        Write-Host "  $tabla`t: $registros" -ForegroundColor Green
    }
    
    $conn.Close()
    Write-Host "`n✅ LISTO - Sistema Supabase operativo" -ForegroundColor Green
    
} catch {
    Write-Host "❌ Error: $_" -ForegroundColor Red
    exit 1
}
