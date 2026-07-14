@echo off
REM Script para ejecutar FashionStore.Web en Release Mode
REM Usado por VS Code F5 (launch.json)

setlocal enabledelayedexpansion

echo ========================================
echo FashionStore.Web - Release Mode
echo ========================================
echo.

REM Establecer variable de entorno para Supabase
set SUPABASE_PASSWORD=MiFer2121092001

REM Ir al directorio del proyecto
cd /d "%~dp0"

echo [1/2] Compilando proyecto...
dotnet build -c Release FashionStoreSolution.sln
if errorlevel 1 (
    echo Error en compilacion
    pause
    exit /b 1
)

echo.
echo [2/2] Ejecutando aplicacion...
echo.
echo Accede a: http://localhost:5100
echo.
echo Presiona Ctrl+C para detener
echo.

cd FashionStore.Web
dotnet run --configuration Release --no-build

pause
