# Script para ejecutar Fashion Store con Supabase
# Establece la variable de entorno y ejecuta la aplicación

$env:SUPABASE_PASSWORD = "MiFer2121092001"

Write-Host "✅ Variable SUPABASE_PASSWORD establecida" -ForegroundColor Green
Write-Host "🚀 Iniciando Fashion Store en http://localhost:5100..." -ForegroundColor Cyan

cd "FashionStore.Web"
dotnet run

# Alt para debug en Visual Studio:
# Presiona F5 en Visual Studio 2026 después de establecer la variable de entorno
