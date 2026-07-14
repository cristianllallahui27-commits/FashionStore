#!/bin/bash
# ==========================================
# MIGRACIÓN SQL Server → Supabase con pgloader
# Ejecutar en WSL (Windows Subsystem for Linux)
# ==========================================

echo "🔄 INICIANDO MIGRACIÓN CON PGLOADER..."
echo ""

# Instalar pgloader si no existe
if ! command -v pgloader &> /dev/null; then
    echo "📦 Instalando pgloader..."
    sudo apt-get update -qq
    sudo apt-get install -y pgloader 2>/dev/null
fi

# Crear config.load
cat > /tmp/config.load << 'EOF'
LOAD DATABASE
  FROM mssql://ADMIN/FashionStoreDB
  INTO postgres://postgres:MiFer2121092001@db.bajbvebkmacdnllnxvkv.supabase.co:5432/postgres

WITH include drop, create tables, create indexes, reset sequences,
     foreign keys constraints, disable triggers

BEFORE LOAD DO
  $$ DROP SCHEMA IF EXISTS public CASCADE; $$,
  $$ CREATE SCHEMA public; $$,
  $$ ALTER SCHEMA public OWNER TO postgres; $$;

AFTER LOAD DO
  $$ ALTER SCHEMA public OWNER TO postgres; $$;

EXCLUDING TABLE NAMES MATCHING 'sysdiagrams', '__MigrationHistory', 'AspNet.*';
EOF

echo "✅ config.load creado"
echo ""

# Ejecutar pgloader
echo "🚀 Ejecutando migración..."
echo ""

pgloader /tmp/config.load

echo ""
echo "✅ MIGRACIÓN COMPLETADA"
echo ""

# Validar
echo "🔍 Validando datos en Supabase..."
echo ""

PGPASSWORD="MiFer2121092001" psql -h db.bajbvebkmacdnllnxvkv.supabase.co -U postgres -d postgres -c "
SELECT 
  'Categorias' AS tabla, COUNT(*) AS registros FROM \"Categorias\"
UNION ALL SELECT 'Prendas', COUNT(*) FROM \"Prendas\"
UNION ALL SELECT 'Clientes', COUNT(*) FROM \"Clientes\"
UNION ALL SELECT 'Vendedores', COUNT(*) FROM \"Vendedores\"
UNION ALL SELECT 'MetodoPago', COUNT(*) FROM \"MetodoPago\"
UNION ALL SELECT 'DescuentosAutorizados', COUNT(*) FROM \"DescuentosAutorizados\"
UNION ALL SELECT 'Ventas', COUNT(*) FROM \"Ventas\"
UNION ALL SELECT 'DetalleVentas', COUNT(*) FROM \"DetalleVentas\"
UNION ALL SELECT 'ConfiguracionSistema', COUNT(*) FROM \"ConfiguracionSistema\"
UNION ALL SELECT 'ConfiguracionAuditoria', COUNT(*) FROM \"ConfiguracionAuditoria\"
ORDER BY tabla;
"

echo ""
echo "✅ LISTO"
