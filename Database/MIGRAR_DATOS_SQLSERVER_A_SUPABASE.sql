-- ==========================================
-- MIGRACIÓN DE DATOS: SQL SERVER → SUPABASE
-- ==========================================
-- 
-- INSTRUCCIONES:
-- 1. Ejecutar en Supabase SQL Editor (después de SUPABASE_SCHEMA_POSTGRESQL.sql)
-- 2. Reemplazar valores en COPY statements con datos reales de SQL Server
-- 3. O usar herramientas como dbeaver, pgAdmin, o scripts de exportación

-- ==========================================
-- SCRIPT: EXPORTAR Y IMPORTAR DATOS
-- ==========================================

-- Para exportar de SQL Server (ejecutar en SQL Server Management Studio):
-- Usar BCP (Bulk Copy Program) o consultas SELECT INTO

-- Para importar a Supabase (ejecutar en Supabase SQL Editor):

-- ==========================================
-- OPCIÓN 1: COPIAR DATOS VÍA SCRIPT SQL
-- ==========================================

-- Si tienes los datos como inserts, ejecutar directo en Supabase:

-- Categorías (ejemplo)
INSERT INTO public.Categorias ("Nombre", "Descripcion") VALUES 
('Camisetas', 'Prendas de algodón y mezclas'),
('Pantalones', 'Jeans y pantalones de tela'),
('Accesorios', 'Complementos y accesorios');

-- Prendas (ejemplo)
INSERT INTO public.Prendas ("Nombre", "Descripcion", "Precio", "Stock", "CategoriaId") VALUES
('Camiseta Básica', 'Camiseta de algodón 100%', 29.99, 100, 1),
('Jean Clásico', 'Jean azul oscuro talla única', 79.99, 50, 2),
('Cinturón Negro', 'Cinturón de cuero', 34.99, 30, 3);

-- Clientes (ejemplo)
INSERT INTO public.Clientes ("Nombre", "Apellido", "Telefono", "Email", "Direccion") VALUES
('Juan', 'Pérez', '555-1234', 'juan@example.com', 'Calle 123 #456'),
('María', 'López', '555-5678', 'maria@example.com', 'Avenida 789 #012');

-- Vendedores (ejemplo)
INSERT INTO public.Vendedores ("Nombre", "Apellido", "Correo", "Telefono", "Cedula") VALUES
('Carlos', 'Martínez', 'carlos@fashionstore.com', '555-2345', '12345678'),
('Ana', 'García', 'ana@fashionstore.com', '555-3456', '87654321');

-- Métodos de Pago (ejemplo)
INSERT INTO public.MetodoPago ("Nombre", "Descripcion") VALUES
('Efectivo', 'Pago en efectivo'),
('Tarjeta Crédito', 'Tarjeta de crédito'),
('Transferencia', 'Transferencia bancaria');

-- ==========================================
-- OPCIÓN 2: USAR HERRAMIENTA DE MIGRACIÓN
-- ==========================================

-- Si deseas usar DBeaver o pgAdmin:
-- 1. Conectar a SQL Server
-- 2. Exportar cada tabla como CSV
-- 3. Conectar a Supabase
-- 4. Importar cada CSV a la tabla correspondiente

-- ==========================================
-- VALIDACIÓN POST-MIGRACIÓN
-- ==========================================

-- Verificar que los datos se importaron correctamente:
SELECT COUNT(*) as "TotalCategorias" FROM public.Categorias;
SELECT COUNT(*) as "TotalPrendas" FROM public.Prendas;
SELECT COUNT(*) as "TotalClientes" FROM public.Clientes;
SELECT COUNT(*) as "TotalVendedores" FROM public.Vendedores;
SELECT COUNT(*) as "TotalMetodoPago" FROM public.MetodoPago;
SELECT COUNT(*) as "TotalVentas" FROM public.Ventas;
SELECT COUNT(*) as "TotalDetalleVenta" FROM public.DetalleVenta;
SELECT COUNT(*) as "TotalConfiguracion" FROM public.ConfiguracionSistema;

-- ==========================================
-- CORREGIR SECUENCIAS (SEQUENCES)
-- ==========================================

-- En PostgreSQL, resetear sequences después de importación:
SELECT setval(pg_get_serial_sequence('public.Categorias', 'Id'), 
              (SELECT MAX("Id") FROM public.Categorias) + 1);
SELECT setval(pg_get_serial_sequence('public.Prendas', 'Id'), 
              (SELECT MAX("Id") FROM public.Prendas) + 1);
SELECT setval(pg_get_serial_sequence('public.Clientes', 'Id'), 
              (SELECT MAX("Id") FROM public.Clientes) + 1);
SELECT setval(pg_get_serial_sequence('public.Vendedores', 'Id'), 
              (SELECT MAX("Id") FROM public.Vendedores) + 1);
SELECT setval(pg_get_serial_sequence('public.MetodoPago', 'Id'), 
              (SELECT MAX("Id") FROM public.MetodoPago) + 1);
SELECT setval(pg_get_serial_sequence('public.Ventas', 'Id'), 
              (SELECT MAX("Id") FROM public.Ventas) + 1);
SELECT setval(pg_get_serial_sequence('public.DetalleVenta', 'Id'), 
              (SELECT MAX("Id") FROM public.DetalleVenta) + 1);
SELECT setval(pg_get_serial_sequence('public.ConfiguracionSistema', 'Id'), 
              (SELECT MAX("Id") FROM public.ConfiguracionSistema) + 1);

-- ==========================================
-- FIN DE MIGRACIÓN
-- ==========================================
