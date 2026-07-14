#!/usr/bin/env pwsh
<#
.SYNOPSIS
	Configura la variable de entorno SUPABASE_PASSWORD para FashionStore
.DESCRIPTION
	Este script establece la variable de entorno SUPABASE_PASSWORD de manera persistente
	en el sistema Windows, para que Entity Framework pueda conectarse a Supabase.
.PARAMETER Password
	La contraseña de la base de datos Supabase (usuario: postgres)
.EXAMPLE
	.\setup-supabase-env.ps1 -Password "tuContraseñaAqui"
#>

param(
	[Parameter(Mandatory = $false, HelpMessage = "Contraseña de Supabase (usuario: postgres)")]
	[securestring]$Password
)

# Solicitar contraseña si no fue proporcionada
if (-not $Password) {
	Write-Host "========================================" -ForegroundColor Cyan
	Write-Host "FashionStore - Configuración de Supabase" -ForegroundColor Cyan
	Write-Host "========================================" -ForegroundColor Cyan
	Write-Host ""
	Write-Host "Necesitamos tu contraseña de Supabase para configurar la conexión." -ForegroundColor Yellow
	Write-Host "📌 La encontrarás en: https://supabase.com/dashboard" -ForegroundColor White
	Write-Host "   Usuario: postgres" -ForegroundColor White
	Write-Host ""
	$Password = Read-Host -Prompt "Ingresa tu contraseña" -AsSecureString
}

# Convertir securestring a string
$PlainPassword = [System.Runtime.InteropServices.Marshal]::PtrToStringAuto(
	[System.Runtime.InteropServices.Marshal]::SecureStringToCoTaskMemUnicode($Password)
)

# Establecer variable de entorno de manera permanente (usuario)
try {
	Write-Host ""
	Write-Host "⚙️  Configurando variable de entorno..." -ForegroundColor Cyan

	[System.Environment]::SetEnvironmentVariable(
		"SUPABASE_PASSWORD", 
		$PlainPassword, 
		[System.EnvironmentVariableTarget]::User
	)

	# Validar que se configuró
	$Verificar = [System.Environment]::GetEnvironmentVariable("SUPABASE_PASSWORD", [System.EnvironmentVariableTarget]::User)

	if ($Verificar) {
		Write-Host "✅ SUPABASE_PASSWORD configurado exitosamente" -ForegroundColor Green
		Write-Host ""
		Write-Host "📝 Próximos pasos:" -ForegroundColor Cyan
		Write-Host "   1. ⚠️  Reinicia Visual Studio" -ForegroundColor Yellow
		Write-Host "   2. 🚀 Abre la solución FashionStoreSolution" -ForegroundColor Yellow
		Write-Host "   3. 📊 Compila el proyecto (Ctrl+Shift+B)" -ForegroundColor Yellow
		Write-Host "   4. ▶️  Ejecuta la aplicación (F5)" -ForegroundColor Yellow
		Write-Host ""
		Write-Host "🔗 También ejecuta el script SQL en Supabase:" -ForegroundColor Cyan
		Write-Host "   archivo: supabase_init.sql" -ForegroundColor White
		Write-Host ""
	} else {
		Write-Host "❌ Error: No se pudo configurar la variable de entorno" -ForegroundColor Red
		exit 1
	}
} catch {
	Write-Host "❌ Error: $_" -ForegroundColor Red
	exit 1
}

# Intentar también configurar a nivel de sistema (requiere admin, pero es opcional)
Write-Host ""
$AdminPrompt = Read-Host "¿Deseas también configurarla a nivel de SISTEMA? (requiere admin) [s/N]"

if ($AdminPrompt -eq "s" -or $AdminPrompt -eq "S") {
	if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] 'Administrator')) {
		Write-Host "⚠️  Se requieren permisos de administrador. Relanzando..." -ForegroundColor Yellow
		Start-Process pwsh -ArgumentList "-Command", ("`$ScriptPath = '$PSCommandPath'; & `$ScriptPath -Password (Read-Host 'Contraseña' -AsSecureString)") -Verb RunAs
	} else {
		[System.Environment]::SetEnvironmentVariable(
			"SUPABASE_PASSWORD", 
			$PlainPassword, 
			[System.EnvironmentVariableTarget]::Machine
		)
		Write-Host "✅ Configurado también a nivel de SISTEMA" -ForegroundColor Green
	}
}

Write-Host ""
Write-Host "✨ ¡Configuración completada!" -ForegroundColor Green
Write-Host ""
