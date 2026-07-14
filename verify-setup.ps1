#!/usr/bin/env pwsh
<#
.SYNOPSIS
	Verificador de Configuración Supabase para FashionStore
.DESCRIPTION
	Verifica que todo esté configurado correctamente antes de ejecutar
#>

Write-Host "`n" -ForegroundColor Cyan
Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║     ✓ VERIFICADOR DE CONFIGURACIÓN - FASHIONSTORE         ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Array para guardar resultados
$checks = @()

# CHECK 1: Variable de Entorno
Write-Host "🔍 Verificando..." -ForegroundColor Yellow
Write-Host ""

$password = [System.Environment]::GetEnvironmentVariable("SUPABASE_PASSWORD", [System.EnvironmentVariableTarget]::User)
if ($password) {
	Write-Host "✅ SUPABASE_PASSWORD configurado" -ForegroundColor Green
	$checks += $true
} else {
	Write-Host "❌ SUPABASE_PASSWORD no configurado" -ForegroundColor Red
	$checks += $false
}

# CHECK 2: appsettings.Development.json
if (Test-Path "FashionStore.Web\appsettings.Development.json") {
	$content = Get-Content "FashionStore.Web\appsettings.Development.json" -Raw
	if ($content -match "ConnectionStrings") {
		Write-Host "✅ appsettings.Development.json configurado" -ForegroundColor Green
		$checks += $true
	} else {
		Write-Host "❌ appsettings.Development.json sin ConnectionStrings" -ForegroundColor Red
		$checks += $false
	}
} else {
	Write-Host "❌ appsettings.Development.json no encontrado" -ForegroundColor Red
	$checks += $false
}

# CHECK 3: supabase_init.sql
if (Test-Path "supabase_init.sql") {
	$lines = (Get-Content "supabase_init.sql" | Measure-Object -Line).Lines
	Write-Host "✅ supabase_init.sql existe ($lines líneas)" -ForegroundColor Green
	$checks += $true
} else {
	Write-Host "❌ supabase_init.sql no encontrado" -ForegroundColor Red
	$checks += $false
}

# CHECK 4: Script PowerShell
if (Test-Path "setup-supabase-env.ps1") {
	Write-Host "✅ setup-supabase-env.ps1 existe" -ForegroundColor Green
	$checks += $true
} else {
	Write-Host "❌ setup-supabase-env.ps1 no encontrado" -ForegroundColor Red
	$checks += $false
}

# CHECK 5: Solución .NET
if (Test-Path "FashionStoreSolution.sln") {
	Write-Host "✅ Solución FashionStoreSolution.sln encontrada" -ForegroundColor Green
	$checks += $true
} else {
	Write-Host "❌ Solución no encontrada" -ForegroundColor Red
	$checks += $false
}

# CHECK 6: Documentación
$docs = @("README_SUPABASE.md", "PASOS_VISUALES.md", "FAQ.md", "DATABASE_SCHEMA.md")
$docsFound = 0
foreach ($doc in $docs) {
	if (Test-Path $doc) {
		$docsFound++
	}
}

if ($docsFound -eq $docs.Count) {
	Write-Host "✅ Toda la documentación está disponible ($docsFound/$($docs.Count))" -ForegroundColor Green
	$checks += $true
} else {
	Write-Host "⚠️  Documentación incompleta ($docsFound/$($docs.Count))" -ForegroundColor Yellow
	$checks += $false
}

# CHECK 7: Compilación
Write-Host ""
Write-Host "🔨 Verificando compilación..." -ForegroundColor Yellow
$buildResult = dotnet build -q 2>&1
if ($LASTEXITCODE -eq 0) {
	Write-Host "✅ Proyecto compila sin errores" -ForegroundColor Green
	$checks += $true
} else {
	Write-Host "❌ Error en compilación" -ForegroundColor Red
	$checks += $false
}

# RESULTADO FINAL
Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan

$passed = ($checks | Where-Object { $_ -eq $true } | Measure-Object).Count
$total = $checks.Count

if ($passed -eq $total) {
	Write-Host ""
	Write-Host "✅ TODOS LOS CHECKS PASARON ($passed/$total)" -ForegroundColor Green
	Write-Host ""
	Write-Host "✨ ¡Tu proyecto está listo!" -ForegroundColor Green
	Write-Host ""
	Write-Host "Próximos pasos:" -ForegroundColor Cyan
	Write-Host "1. Ejecuta el script SQL en Supabase" -ForegroundColor White
	Write-Host "2. Presiona F5 para ejecutar la aplicación" -ForegroundColor White
	Write-Host ""
} else {
	Write-Host ""
	Write-Host "⚠️  FALLOS DETECTADOS: $($total - $passed)/$total" -ForegroundColor Yellow
	Write-Host ""
	if (-not $password) {
		Write-Host "1️⃣  Configura SUPABASE_PASSWORD:" -ForegroundColor Yellow
		Write-Host "   .\setup-supabase-env.ps1" -ForegroundColor White
		Write-Host ""
	}
	if ($docsFound -lt $docs.Count) {
		Write-Host "2️⃣  Verifica que todos los archivos estén presentes" -ForegroundColor Yellow
		Write-Host ""
	}
}

Write-Host "📚 Lee: README_SUPABASE.md para instrucciones detalladas" -ForegroundColor Cyan
Write-Host ""
