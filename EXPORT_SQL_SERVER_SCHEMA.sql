-- ============================================================
-- EXPORT SCHEMA DE SQL SERVER
-- ============================================================
-- Ejecuta esto en SQL Server Management Studio
-- Base de datos: FashionStoreDB
-- ============================================================
-- PASOS:
-- 1. Abre SQL Server Management Studio
-- 2. Conecta a: Server=ADMIN; Database=FashionStoreDB
-- 3. Abre una New Query
-- 4. Copia TODO este archivo
-- 5. Ejecuta (F5)
-- 6. Guarda el resultado a un archivo .sql
-- 7. Ese archivo lo pasas a PostgreSQL
-- ============================================================

-- Ver todas las tablas
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo'
ORDER BY TABLE_NAME;

-- Ver estructura de tabla ejemplo (Prendas)
EXEC sp_help 'Prendas';

-- Contar registros
SELECT 'Categorias' as Tabla, COUNT(*) as Cantidad FROM Categorias
UNION ALL
SELECT 'Prendas', COUNT(*) FROM Prendas
UNION ALL
SELECT 'Clientes', COUNT(*) FROM Clientes
UNION ALL
SELECT 'Vendedores', COUNT(*) FROM Vendedores
UNION ALL
SELECT 'Ventas', COUNT(*) FROM Ventas
UNION ALL
SELECT 'DetalleVenta', COUNT(*) FROM DetalleVenta
UNION ALL
SELECT 'MetodoPago', COUNT(*) FROM MetodoPago
UNION ALL
SELECT 'DescuentoAutorizado', COUNT(*) FROM DescuentosAutorizados
UNION ALL
SELECT 'ConfiguracionSistema', COUNT(*) FROM ConfiguracionSistema
UNION ALL
SELECT 'AspNetUsers', COUNT(*) FROM AspNetUsers
UNION ALL
SELECT 'AspNetRoles', COUNT(*) FROM AspNetRoles
UNION ALL
SELECT 'AspNetUserRoles', COUNT(*) FROM AspNetUserRoles;

-- ============================================================
-- ALTERNATIVA: GENERAR SCRIPT DE CREACIÓN
-- ============================================================
-- En SQL Server Management Studio:
-- 1. Right-click en Base de datos "FashionStoreDB"
-- 2. Tasks > Generate Scripts
-- 3. Next > Select All Tables
-- 4. Next > Guardar a archivo .sql
-- 5. Ese archivo contiene el schema completo
-- ============================================================
