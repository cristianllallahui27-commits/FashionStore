-- ==========================================
-- SUPABASE - SETUP COMPLETO
-- ==========================================
-- Crear esquema + tablas + datos iniciales
-- Ejecutar TODO en: Supabase SQL Editor (https://supabase.com/)
-- Database: postgres | Schema: public
-- ==========================================

-- ==========================================
-- CREAR TABLAS
-- ==========================================

CREATE TABLE IF NOT EXISTS public.Categorias (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(200) NOT NULL UNIQUE,
    "Descripcion" TEXT,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS public.Prendas (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(200) NOT NULL UNIQUE,
    "Descripcion" TEXT,
    "Precio" NUMERIC(10,2) NOT NULL CHECK ("Precio" > 0),
    "Stock" INTEGER NOT NULL DEFAULT 0 CHECK ("Stock" >= 0),
    "CategoriaId" INTEGER NOT NULL,
    "Activo" BOOLEAN DEFAULT TRUE,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY ("CategoriaId") REFERENCES public.Categorias("Id") ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS public.Clientes (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL,
    "Apellido" VARCHAR(100),
    "Telefono" VARCHAR(20),
    "Email" VARCHAR(100),
    "Direccion" TEXT,
    "Activo" BOOLEAN DEFAULT TRUE,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS public.Vendedores (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL,
    "Apellido" VARCHAR(100),
    "Correo" VARCHAR(100) UNIQUE,
    "Telefono" VARCHAR(20),
    "Cedula" VARCHAR(20) UNIQUE,
    "Activo" BOOLEAN DEFAULT TRUE,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS public.MetodoPago (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL UNIQUE,
    "Descripcion" TEXT,
    "Activo" BOOLEAN DEFAULT TRUE
);

CREATE TABLE IF NOT EXISTS public.Ventas (
    "Id" SERIAL PRIMARY KEY,
    "ClienteId" INTEGER,
    "VendedorId" INTEGER,
    "MetodoPagoId" INTEGER,
    "Fecha" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "Total" NUMERIC(10,2) NOT NULL CHECK ("Total" >= 0),
    "Descuento" NUMERIC(10,2) DEFAULT 0,
    FOREIGN KEY ("ClienteId") REFERENCES public.Clientes("Id") ON DELETE SET NULL,
    FOREIGN KEY ("VendedorId") REFERENCES public.Vendedores("Id") ON DELETE SET NULL,
    FOREIGN KEY ("MetodoPagoId") REFERENCES public.MetodoPago("Id") ON DELETE SET NULL
);

CREATE TABLE IF NOT EXISTS public.DetalleVenta (
    "Id" SERIAL PRIMARY KEY,
    "VentaId" INTEGER NOT NULL,
    "PrendaId" INTEGER NOT NULL,
    "Cantidad" INTEGER NOT NULL CHECK ("Cantidad" > 0),
    "Precio" NUMERIC(10,2) NOT NULL CHECK ("Precio" > 0),
    FOREIGN KEY ("VentaId") REFERENCES public.Ventas("Id") ON DELETE CASCADE,
    FOREIGN KEY ("PrendaId") REFERENCES public.Prendas("Id") ON DELETE RESTRICT
);

CREATE TABLE IF NOT EXISTS public.ConfiguracionSistema (
    "Id" SERIAL PRIMARY KEY,
    "NombreTienda" VARCHAR(200),
    "RutaLogo" VARCHAR(500),
    "RutaFavicon" VARCHAR(500),
    "ColorPrimario" VARCHAR(7),
    "ColorSecundario" VARCHAR(7),
    "ColorMenu" VARCHAR(7),
    "ColorBotones" VARCHAR(7),
    "ColorFondo" VARCHAR(7),
    "TemaOscuro" BOOLEAN DEFAULT FALSE,
    "FechaActualizacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS public.ConfiguracionAuditoria (
    "Id" SERIAL PRIMARY KEY,
    "RegistrarAccesos" BOOLEAN DEFAULT TRUE,
    "RegistrarCambios" BOOLEAN DEFAULT TRUE,
    "RegistrarErrores" BOOLEAN DEFAULT TRUE,
    "DiasBorrarLogs" INTEGER DEFAULT 90
);

-- ==========================================
-- CREAR ÍNDICES
-- ==========================================

CREATE INDEX IF NOT EXISTS idx_prendas_categoria ON public.Prendas("CategoriaId");
CREATE INDEX IF NOT EXISTS idx_prendas_nombre ON public.Prendas("Nombre");
CREATE INDEX IF NOT EXISTS idx_ventas_fecha ON public.Ventas("Fecha" DESC);
CREATE INDEX IF NOT EXISTS idx_ventas_vendedor ON public.Ventas("VendedorId");
CREATE INDEX IF NOT EXISTS idx_ventas_cliente ON public.Ventas("ClienteId");
CREATE INDEX IF NOT EXISTS idx_detalleventa_venta ON public.DetalleVenta("VentaId");
CREATE INDEX IF NOT EXISTS idx_detalleventa_prenda ON public.DetalleVenta("PrendaId");

-- ==========================================
-- INSERTAR DATOS INICIALES
-- ==========================================

-- Categorías
INSERT INTO public.Categorias ("Nombre", "Descripcion") VALUES 
('Prendas Superiores', 'Camisas, blusas, tops, sudaderas'),
('Prendas Inferiores', 'Pantalones, faldas, shorts, leggins'),
('Lencería', 'Ropa íntima y accesorios'),
('Accesorios', 'Cinturones, bufandas, gorros'),
('Abrigos', 'Chaquetas, abrigos, blazers')
ON CONFLICT DO NOTHING;

-- Métodos de Pago
INSERT INTO public.MetodoPago ("Nombre", "Descripcion", "Activo") VALUES 
('Efectivo', 'Pago en efectivo', TRUE),
('Tarjeta Crédito', 'Pago con tarjeta crédito', TRUE),
('Tarjeta Débito', 'Pago con tarjeta débito', TRUE),
('Transferencia Bancaria', 'Transferencia a cuenta bancaria', TRUE),
('Cheque', 'Pago con cheque', TRUE)
ON CONFLICT DO NOTHING;

-- Prendas (ejemplos)
INSERT INTO public.Prendas ("Nombre", "Descripcion", "Precio", "Stock", "CategoriaId", "Activo") VALUES 
('Camiseta Básica Blanca', 'Camiseta de algodón 100%, talla M-L-XL', 29.99, 150, 1, TRUE),
('Camiseta Básica Negra', 'Camiseta de algodón 100%, talla M-L-XL', 29.99, 120, 1, TRUE),
('Blusa Elegante Rosa', 'Blusa de seda con botones, talla S-M-L', 49.99, 80, 1, TRUE),
('Sudadera Gris', 'Sudadera de algodón con capucha, talla M-L-XL', 59.99, 100, 1, TRUE),
('Jean Clásico Azul', 'Jean azul oscuro talla 28-32-34-36', 79.99, 200, 2, TRUE),
('Jean Negro Slim', 'Jean negro slim fit talla 28-32-34-36', 89.99, 150, 2, TRUE),
('Falda Plisada Negra', 'Falda plisada midi, talla S-M-L', 45.99, 90, 2, TRUE),
('Shorts Deportivos', 'Shorts cortos para deporte, talla S-M-L-XL', 34.99, 120, 2, TRUE),
('Leggins Deportivos', 'Leggins elásticos deportivos, talla S-M-L-XL', 39.99, 180, 2, TRUE),
('Set Lencería Encaje', 'Set lencería encaje, talla S-M-L', 59.99, 60, 3, TRUE),
('Bralette Suave', 'Bralette cómodo sin varillas, talla S-M-L-XL', 24.99, 100, 3, TRUE),
('Cinturón Cuero Negro', 'Cinturón de cuero auténtico, tallas 28-32-36-40', 34.99, 80, 4, TRUE),
('Cinturón Tela Dorado', 'Cinturón de tela con hebilla dorada, universal', 19.99, 120, 4, TRUE),
('Bufanda Lana Gris', 'Bufanda de lana pura, larga', 24.99, 70, 4, TRUE),
('Gorro Beanie Negro', 'Gorro beanie de punto, talla única', 14.99, 150, 4, TRUE),
('Chaqueta Denim', 'Chaqueta denim azul clásica, talla S-M-L-XL', 99.99, 100, 5, TRUE),
('Blazer Formal Negro', 'Blazer formal para oficina, talla 2-4-6-8', 129.99, 50, 5, TRUE),
('Abrigo Invierno Beige', 'Abrigo largo invierno, talla S-M-L-XL', 149.99, 40, 5, TRUE)
ON CONFLICT DO NOTHING;

-- Clientes (ejemplos)
INSERT INTO public.Clientes ("Nombre", "Apellido", "Telefono", "Email", "Direccion", "Activo") VALUES 
('Juan', 'Pérez', '555-1001', 'juan.perez@email.com', 'Calle 123 #456, Apartamento 101', TRUE),
('María', 'López', '555-1002', 'maria.lopez@email.com', 'Avenida 789 #012, Apartamento 202', TRUE),
('Carlos', 'Martínez', '555-1003', 'carlos.martinez@email.com', 'Carrera 45 #67, Casa 303', TRUE),
('Ana', 'García', '555-1004', 'ana.garcia@email.com', 'Calle 234 #567, Apartamento 404', TRUE),
('Pedro', 'Rodríguez', '555-1005', 'pedro.rodriguez@email.com', 'Avenida 890 #123, Apartamento 505', TRUE),
('Sandra', 'Fernández', '555-1006', 'sandra.fernandez@email.com', 'Calle 345 #678, Casa 606', TRUE),
('Roberto', 'Gómez', '555-1007', 'roberto.gomez@email.com', 'Carrera 56 #78, Apartamento 707', TRUE),
('Laura', 'Sánchez', '555-1008', 'laura.sanchez@email.com', 'Calle 456 #789, Apartamento 808', TRUE),
('Miguel', 'Torres', '555-1009', 'miguel.torres@email.com', 'Avenida 901 #234, Casa 909', TRUE),
('Patricia', 'Reyes', '555-1010', 'patricia.reyes@email.com', 'Calle 567 #890, Apartamento 1010', TRUE)
ON CONFLICT DO NOTHING;

-- Vendedores (ejemplos)
INSERT INTO public.Vendedores ("Nombre", "Apellido", "Correo", "Telefono", "Cedula", "Activo") VALUES 
('Carlos', 'Mendoza', 'carlos.mendoza@fashionstore.com', '555-2001', '1234567890', TRUE),
('Ana', 'Vallejo', 'ana.vallejo@fashionstore.com', '555-2002', '0987654321', TRUE),
('Luis', 'Castro', 'luis.castro@fashionstore.com', '555-2003', '1122334455', TRUE),
('Sofía', 'Ruiz', 'sofia.ruiz@fashionstore.com', '555-2004', '5544332211', TRUE),
('Diego', 'Moreno', 'diego.moreno@fashionstore.com', '555-2005', '6677889900', TRUE)
ON CONFLICT DO NOTHING;

-- Configuración del Sistema
INSERT INTO public.ConfiguracionSistema ("NombreTienda", "ColorPrimario", "ColorSecundario", "ColorMenu", "ColorBotones", "ColorFondo", "TemaOscuro") 
VALUES ('FashionStore - Sistema Administrativo', '#667eea', '#764ba2', '#1a1a2e', '#667eea', '#f5f7fa', FALSE)
ON CONFLICT DO NOTHING;

-- Configuración de Auditoría
INSERT INTO public.ConfiguracionAuditoria ("RegistrarAccesos", "RegistrarCambios", "RegistrarErrores", "DiasBorrarLogs") 
VALUES (TRUE, TRUE, TRUE, 90)
ON CONFLICT DO NOTHING;

-- ==========================================
-- CREAR ALGUNAS VENTAS DE EJEMPLO
-- ==========================================

-- Venta 1
WITH venta AS (
  INSERT INTO public.Ventas ("ClienteId", "VendedorId", "MetodoPagoId", "Total", "Descuento")
  SELECT 1, 1, 1, 109.98, 0
  RETURNING "Id"
)
INSERT INTO public.DetalleVenta ("VentaId", "PrendaId", "Cantidad", "Precio")
SELECT v."Id", 1, 2, 29.99 FROM venta v
UNION ALL
SELECT v."Id", 11, 1, 59.99 FROM venta v;

-- Venta 2
WITH venta AS (
  INSERT INTO public.Ventas ("ClienteId", "VendedorId", "MetodoPagoId", "Total", "Descuento")
  SELECT 2, 2, 2, 249.97, 10
  RETURNING "Id"
)
INSERT INTO public.DetalleVenta ("VentaId", "PrendaId", "Cantidad", "Precio")
SELECT v."Id", 5, 1, 79.99 FROM venta v
UNION ALL
SELECT v."Id", 6, 2, 89.99 FROM venta v;

-- ==========================================
-- VALIDAR CREACIÓN
-- ==========================================

SELECT 
  'Categorias' AS tabla, COUNT(*) AS registros FROM public.Categorias
UNION ALL SELECT 'Prendas', COUNT(*) FROM public.Prendas
UNION ALL SELECT 'Clientes', COUNT(*) FROM public.Clientes
UNION ALL SELECT 'Vendedores', COUNT(*) FROM public.Vendedores
UNION ALL SELECT 'MetodoPago', COUNT(*) FROM public.MetodoPago
UNION ALL SELECT 'Ventas', COUNT(*) FROM public.Ventas
UNION ALL SELECT 'DetalleVenta', COUNT(*) FROM public.DetalleVenta
UNION ALL SELECT 'ConfiguracionSistema', COUNT(*) FROM public.ConfiguracionSistema
UNION ALL SELECT 'ConfiguracionAuditoria', COUNT(*) FROM public.ConfiguracionAuditoria;

-- ==========================================
-- FIN - SETUP COMPLETADO
-- ==========================================
