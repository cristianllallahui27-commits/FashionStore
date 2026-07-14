-- ==========================================
-- VALIDAR MIGRACIÓN SQL Server → Supabase
-- ==========================================
-- Ejecutar en: Supabase SQL Editor

-- Contar registros por tabla
SELECT 
  'Categorias' AS tabla, COUNT(*) AS registros FROM "Categorias"
UNION ALL SELECT 'Prendas', COUNT(*) FROM "Prendas"
UNION ALL SELECT 'Clientes', COUNT(*) FROM "Clientes"
UNION ALL SELECT 'Vendedores', COUNT(*) FROM "Vendedores"
UNION ALL SELECT 'MetodoPago', COUNT(*) FROM "MetodoPago"
UNION ALL SELECT 'DescuentosAutorizados', COUNT(*) FROM "DescuentosAutorizados"
UNION ALL SELECT 'Ventas', COUNT(*) FROM "Ventas"
UNION ALL SELECT 'DetalleVentas', COUNT(*) FROM "DetalleVentas"
UNION ALL SELECT 'ConfiguracionSistema', COUNT(*) FROM "ConfiguracionSistema"
UNION ALL SELECT 'ConfiguracionAuditoria', COUNT(*) FROM "ConfiguracionAuditoria"
ORDER BY tabla;

-- Validar integridad de datos
SELECT 'Categorias' as check_name, COUNT(*) as count FROM "Categorias" WHERE "Nombre" IS NULL
UNION ALL
SELECT 'Prendas sin categoria', COUNT(*) FROM "Prendas" WHERE "CategoriaId" IS NULL
UNION ALL
SELECT 'Clientes sin nombre', COUNT(*) FROM "Clientes" WHERE "Nombre" IS NULL
UNION ALL
SELECT 'Ventas sin fecha', COUNT(*) FROM "Ventas" WHERE "Fecha" IS NULL
UNION ALL
SELECT 'DetalleVentas sin venta', COUNT(*) FROM "DetalleVentas" WHERE "VentaId" IS NULL;

-- Ver ejemplos de datos
SELECT * FROM "Categorias" LIMIT 5;
SELECT * FROM "Prendas" LIMIT 5;
SELECT * FROM "Clientes" LIMIT 5;
