-- SUPABASE SCHEMA - PostgreSQL
-- Crear estructura completa en Supabase
-- Ejecutar en: Supabase SQL Editor

-- ==========================================
-- 1. CREAR TABLAS (PostgreSQL)
-- ==========================================

-- Tabla: Categorias
CREATE TABLE IF NOT EXISTS public.Categorias (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(200) NOT NULL UNIQUE,
    "Descripcion" TEXT,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabla: Prendas
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

-- Tabla: Clientes
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

-- Tabla: Vendedores
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

-- Tabla: MetodoPago
CREATE TABLE IF NOT EXISTS public.MetodoPago (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL UNIQUE,
    "Descripcion" TEXT,
    "Activo" BOOLEAN DEFAULT TRUE
);

-- Tabla: Ventas
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

-- Tabla: DetalleVenta
CREATE TABLE IF NOT EXISTS public.DetalleVenta (
    "Id" SERIAL PRIMARY KEY,
    "VentaId" INTEGER NOT NULL,
    "PrendaId" INTEGER NOT NULL,
    "Cantidad" INTEGER NOT NULL CHECK ("Cantidad" > 0),
    "Precio" NUMERIC(10,2) NOT NULL CHECK ("Precio" > 0),
    FOREIGN KEY ("VentaId") REFERENCES public.Ventas("Id") ON DELETE CASCADE,
    FOREIGN KEY ("PrendaId") REFERENCES public.Prendas("Id") ON DELETE RESTRICT
);

-- Tabla: ConfiguracionSistema
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

-- Tabla: ConfiguracionAuditoria
CREATE TABLE IF NOT EXISTS public.ConfiguracionAuditoria (
    "Id" SERIAL PRIMARY KEY,
    "RegistrarAccesos" BOOLEAN DEFAULT TRUE,
    "RegistrarCambios" BOOLEAN DEFAULT TRUE,
    "RegistrarErrores" BOOLEAN DEFAULT TRUE,
    "DiasBorrarLogs" INTEGER DEFAULT 90
);

-- ==========================================
-- 2. CREAR ÍNDICES
-- ==========================================

CREATE INDEX IF NOT EXISTS idx_prendas_categoria ON public.Prendas("CategoriaId");
CREATE INDEX IF NOT EXISTS idx_prendas_nombre ON public.Prendas("Nombre");
CREATE INDEX IF NOT EXISTS idx_ventas_fecha ON public.Ventas("Fecha" DESC);
CREATE INDEX IF NOT EXISTS idx_ventas_vendedor ON public.Ventas("VendedorId");
CREATE INDEX IF NOT EXISTS idx_ventas_cliente ON public.Ventas("ClienteId");
CREATE INDEX IF NOT EXISTS idx_detalleventa_venta ON public.DetalleVenta("VentaId");
CREATE INDEX IF NOT EXISTS idx_detalleventa_prenda ON public.DetalleVenta("PrendaId");

-- ==========================================
-- 3. INSERTAR DATOS INICIALES (PRUEBA)
-- ==========================================

-- Datos de prueba (comentados)
-- INSERT INTO public.Categorias ("Nombre", "Descripcion") VALUES 
-- ('Prendas Superiores', 'Camisas, blusas, etc'),
-- ('Prendas Inferiores', 'Pantalones, faldas, etc'),
-- ('Lencería', 'Prendas íntimas');

-- INSERT INTO public.MetodoPago ("Nombre", "Descripcion") VALUES 
-- ('Efectivo', 'Pago en efectivo'),
-- ('Tarjeta', 'Pago con tarjeta crédito/débito'),
-- ('Transferencia', 'Transferencia bancaria');

-- INSERT INTO public.ConfiguracionSistema 
-- ("NombreTienda", "ColorPrimario", "ColorSecundario", "ColorMenu", "ColorBotones", "ColorFondo", "TemaOscuro")
-- VALUES ('FashionStore', '#667eea', '#764ba2', '#1a1a2e', '#667eea', '#f5f7fa', FALSE);

-- ==========================================
-- 4. VERIFICAR CREACIÓN
-- ==========================================

-- Listar todas las tablas
SELECT * FROM information_schema.tables WHERE table_schema = 'public';

-- Ver estructura de tabla
-- \d public.Prendas

-- Contar registros
SELECT 'Categorias' AS tabla, COUNT(*) AS registros FROM public.Categorias
UNION ALL
SELECT 'Prendas', COUNT(*) FROM public.Prendas
UNION ALL
SELECT 'Clientes', COUNT(*) FROM public.Clientes
UNION ALL
SELECT 'Vendedores', COUNT(*) FROM public.Vendedores
UNION ALL
SELECT 'MetodoPago', COUNT(*) FROM public.MetodoPago
UNION ALL
SELECT 'Ventas', COUNT(*) FROM public.Ventas
UNION ALL
SELECT 'DetalleVenta', COUNT(*) FROM public.DetalleVenta
UNION ALL
SELECT 'ConfiguracionSistema', COUNT(*) FROM public.ConfiguracionSistema;
