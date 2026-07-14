# ============================================================
# FashionStore - Script de Ejecución Sin Debugger
# ============================================================
# Este script inicia la aplicación FashionStore sin necesidad
# del debugger de VS Code, evitando el error de licencia.
# 
# USO:
#   PowerShell -ExecutionPolicy Bypass -File run-fashionstore.ps1
#   PowerShell -ExecutionPolicy Bypass -File run-fashionstore.ps1 -Test
#   PowerShell -ExecutionPolicy Bypass -File run-fashionstore.ps1 -Rebuild
# ============================================================

param(
    [switch]$Clean,
    [switch]$Rebuild,
    [switch]$Test,
    [switch]$Validate
)

$ErrorActionPreference = "Stop"
$projectRoot = "c:\Users\CRISTIAN\source\repos\FashionStoreSolution"
$webProject = "$projectRoot\FashionStore.Web"
$testProject = "$projectRoot\FashionStore.Tests\FashionStore.Tests.csproj"
$uploadsFolder = "$webProject\wwwroot\uploads"

function Write-Header {
    param([string]$Message)
    Write-Host ""
    Write-Host "================================" -ForegroundColor Cyan
    Write-Host $Message -ForegroundColor Green -Bold
    Write-Host "================================" -ForegroundColor Cyan
}

function Write-Success {
    param([string]$Message)
    Write-Host "✅ $Message" -ForegroundColor Green
}

function Write-Error-Custom {
    param([string]$Message)
    Write-Host "❌ ERROR: $Message" -ForegroundColor Red
    exit 1
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ️  $Message" -ForegroundColor Blue
}

# ============================================================
# Validar que estamos en el directorio correcto
# ============================================================

Write-Header "🔍 VALIDANDO ENTORNO"

if (-not (Test-Path $projectRoot)) {
    Write-Error-Custom "No se encontró el proyecto en: $projectRoot"
}

Write-Success "Proyecto encontrado en: $projectRoot"

# ============================================================
# Validar y crear carpeta de uploads
# ============================================================

Write-Header "📁 VALIDANDO CARPETA DE UPLOADS"

if (-not (Test-Path $uploadsFolder)) {
    Write-Info "Creando carpeta: $uploadsFolder"
    New-Item -ItemType Directory -Path $uploadsFolder -Force | Out-Null
    Write-Success "Carpeta de uploads creada"
} else {
    Write-Success "Carpeta de uploads existe"
}

# Validar permisos de escritura
try {
    $testFile = "$uploadsFolder\.write-test-$(Get-Random)"
    "test" | Out-File $testFile -Force
    Remove-Item $testFile -Force
    Write-Success "Permisos de escritura verificados ✅"
} catch {
    Write-Error-Custom "No hay permisos de escritura en $uploadsFolder"
}

# ============================================================
# Limpiar (opcional)
# ============================================================

if ($Clean) {
    Write-Header "🧹 LIMPIANDO SOLUCIÓN"
    Set-Location $projectRoot
    
    Write-Info "Eliminando carpetas bin/ y obj/..."
    Get-ChildItem -Recurse -Directory -Filter "bin" | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    Get-ChildItem -Recurse -Directory -Filter "obj" | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
    
    Write-Success "Limpieza completada"
}

# ============================================================
# Compilar
# ============================================================

Write-Header "🔨 COMPILANDO SOLUCIÓN"

Set-Location $projectRoot

if ($Rebuild) {
    Write-Info "Ejecutando: dotnet clean"
    & dotnet clean
}

Write-Info "Ejecutando: dotnet build FashionStoreSolution.sln"
& dotnet build FashionStoreSolution.sln

if ($LASTEXITCODE -ne 0) {
    Write-Error-Custom "La compilación falló. Verifica los errores arriba."
}

Write-Success "Compilación exitosa"

# ============================================================
# Pruebas (opcional)
# ============================================================

if ($Test) {
    Write-Header "🧪 EJECUTANDO TESTS"
    
    Write-Info "Ejecutando: dotnet test FashionStore.Tests --no-build"
    & dotnet test $testProject --no-build
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error-Custom "Los tests fallaron."
    }
    
    Write-Success "Todos los tests pasaron ✨"
}

# ============================================================
# Ejecutar Aplicación
# ============================================================

Write-Header "🚀 INICIANDO FASHIONSTORE"

Set-Location $webProject

Write-Info "Puerto: http://localhost:5100"
Write-Info "Presiona Ctrl+C para detener la aplicación"
Write-Info ""
Write-Info "Credenciales de prueba:"
Write-Info "  Admin: admin@fashionstore.com / Password123!"
Write-Info "  Vendedor: vendedor@fashionstore.com / Password123!"
Write-Info ""

Write-Success "Iniciando aplicación..."
Write-Host ""

& dotnet run

# ============================================================
# Fin del Script
# ============================================================
