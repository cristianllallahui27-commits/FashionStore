@echo off
REM ==============================================
REM FASHIONSTORE - Script de Configuración Rápida
REM ==============================================
REM Este script automatiza toda la configuración

echo.
echo ========================================
echo FashionStore - Configuracion Supabase
echo ========================================
echo.

REM Verificar si estamos en la carpeta correcta
if not exist "supabase_init.sql" (
	echo Error: No estoy en la carpeta correcta
	echo Por favor, ejecuta este script desde la raiz del proyecto
	pause
	exit /b 1
)

echo.
echo [1] Configurar Variable de Entorno...
echo.

REM Ejecutar el script PowerShell
powershell -ExecutionPolicy Bypass -File "setup-supabase-env.ps1"

if %ERRORLEVEL% EQU 0 (
	echo.
	echo ✓ Variable configurada exitosamente
) else (
	echo.
	echo × Error configurando la variable
	pause
	exit /b 1
)

echo.
echo [2] Limpiar compilaciones anteriores...
echo.

cd FashionStore.Web
dotnet clean
dotnet restore

if %ERRORLEVEL% NEQ 0 (
	echo Error en restore
	pause
	exit /b 1
)

echo.
echo [3] Compilar solución...
echo.

cd ..
dotnet build

if %ERRORLEVEL% NEQ 0 (
	echo Error en compilacion
	pause
	exit /b 1
)

echo.
echo ========================================
echo ✓ Configuracion completada!
echo ========================================
echo.
echo Proximos pasos:
echo.
echo 1. Abre https://supabase.com/dashboard
echo 2. Selecciona tu proyecto
echo 3. SQL Editor - New Query
echo 4. Copia TODO el contenido de: supabase_init.sql
echo 5. Pegalo y haz click en Run
echo.
echo Luego ejecuta:
echo    cd FashionStore.Web
echo    dotnet run
echo.
pause
