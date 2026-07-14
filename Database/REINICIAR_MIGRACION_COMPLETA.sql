-- =============================================================================
-- FASHIONSTORE - Script de Recreación Completa para Supabase (PostgreSQL)
-- ELIMINA TODAS LAS TABLAS EXISTENTES Y LAS VUELVE A CREAR DESDE CERO
-- Optimizado para ASP.NET Core Identity (.NET 9) y la entidad ApplicationUser
-- =============================================================================

-- =============================================================================
-- 🔴 SECCIÓN 1: ELIMINACIÓN DE TABLAS EXISTENTES (CASCADE)
-- =============================================================================
DROP TABLE IF EXISTS public."AspNetUserRoles" CASCADE;
DROP TABLE IF EXISTS public."AspNetUserClaims" CASCADE;
DROP TABLE IF EXISTS public."AspNetUserLogins" CASCADE;
DROP TABLE IF EXISTS public."AspNetUserTokens" CASCADE;
DROP TABLE IF EXISTS public."AspNetRoleClaims" CASCADE;
DROP TABLE IF EXISTS public."AspNetUsers" CASCADE;
DROP TABLE IF EXISTS public."AspNetRoles" CASCADE;
DROP TABLE IF EXISTS public."DetalleVentas" CASCADE;
DROP TABLE IF EXISTS public."Ventas" CASCADE;
DROP TABLE IF EXISTS public."Prendas" CASCADE;
DROP TABLE IF EXISTS public."Categorias" CASCADE;
DROP TABLE IF EXISTS public."Clientes" CASCADE;
DROP TABLE IF EXISTS public."Vendedores" CASCADE;
DROP TABLE IF EXISTS public."MetodosPago" CASCADE;
DROP TABLE IF EXISTS public."DescuentosAutorizados" CASCADE;
DROP TABLE IF EXISTS public."ConfiguracionSistema" CASCADE;
DROP TABLE IF EXISTS public."ConfiguracionAuditoria" CASCADE;

-- =============================================================================
-- 🔵 SECCIÓN 2: CREACIÓN DE TABLAS DEL PROYECTO Y DE ASP.NET CORE IDENTITY
-- =============================================================================

-- ===== 1. TABLA: Categorias =====
CREATE TABLE public."Categorias" (
	"Id" SERIAL PRIMARY KEY,
	"Nombre" VARCHAR(200) NOT NULL UNIQUE,
	"Descripcion" TEXT,
	"FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	"FechaModificacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_categorias_nombre ON public."Categorias"("Nombre");

-- ===== 2. TABLA: Prendas =====
CREATE TABLE public."Prendas" (
	"Id" SERIAL PRIMARY KEY,
	"Nombre" VARCHAR(200) NOT NULL UNIQUE,
	"Descripcion" TEXT,
	"Talla" VARCHAR(50) NOT NULL,
	"Color" VARCHAR(50) NOT NULL,
	"Precio" NUMERIC(10,2) NOT NULL CHECK ("Precio" > 0),
	"Stock" INTEGER NOT NULL DEFAULT 0 CHECK ("Stock" >= 0),
	"ImagenUrl" VARCHAR(500),
	"CodigoBarra" VARCHAR(50),
	"Activo" BOOLEAN DEFAULT TRUE,
	"CategoriaId" INTEGER NOT NULL REFERENCES public."Categorias"("Id") ON DELETE RESTRICT,
	"FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	"FechaModificacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_prendas_nombre ON public."Prendas"("Nombre");
CREATE INDEX idx_prendas_categoria ON public."Prendas"("CategoriaId");
CREATE INDEX idx_prendas_codigo ON public."Prendas"("CodigoBarra");
CREATE INDEX idx_prendas_activo ON public."Prendas"("Activo");

-- ===== 3. TABLA: Clientes =====
CREATE TABLE public."Clientes" (
	"Id" SERIAL PRIMARY KEY,
	"NombreCompleto" VARCHAR(150) NOT NULL,
	"DNI" VARCHAR(20) NOT NULL UNIQUE,
	"Telefono" VARCHAR(20) NOT NULL,
	"Email" VARCHAR(100),
	"Direccion" TEXT NOT NULL,
	"Nombre" VARCHAR(100),
	"Apellido" VARCHAR(100),
	"Activo" BOOLEAN DEFAULT TRUE,
	"FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	"FechaModificacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_clientes_dni ON public."Clientes"("DNI");
CREATE INDEX idx_clientes_email ON public."Clientes"("Email");
CREATE INDEX idx_clientes_activo ON public."Clientes"("Activo");

-- ===== 4. TABLA: Vendedores =====
CREATE TABLE public."Vendedores" (
	"Id" SERIAL PRIMARY KEY,
	"Nombres" VARCHAR(150) NOT NULL,
	"Apellidos" VARCHAR(150) NOT NULL,
	"DNI" VARCHAR(20) NOT NULL UNIQUE,
	"Telefono" VARCHAR(20),
	"Correo" VARCHAR(150) UNIQUE,
	"Estado" BOOLEAN NOT NULL DEFAULT TRUE,
	"UltimaPasswordAdmin" VARCHAR(100),
	"Nombre" VARCHAR(100),
	"Apellido" VARCHAR(100),
	"Activo" BOOLEAN DEFAULT TRUE,
	"FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
	"FechaModificacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_vendedores_dni ON public."Vendedores"("DNI");
CREATE INDEX idx_vendedores_correo ON public."Vendedores"("Correo");
CREATE INDEX idx_vendedores_activo ON public."Vendedores"("Activo");

-- ===== 5. TABLA: MetodosPago =====
CREATE TABLE public."MetodosPago" (
	"Id" SERIAL PRIMARY KEY,
	"Nombre" VARCHAR(100) NOT NULL UNIQUE,
	"Descripcion" TEXT,
	"Activo" BOOLEAN DEFAULT TRUE,
	"FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ===== 6. TABLA: DescuentosAutorizados =====
CREATE TABLE public."DescuentosAutorizados" (
	"Id" SERIAL PRIMARY KEY,
	"Nombre" VARCHAR(100) NOT NULL,
	"Tipo" INTEGER NOT NULL,  -- 0 = Porcentaje, 1 = MontoFijo
	"Valor" NUMERIC(10,2) NOT NULL,
	"Activo" BOOLEAN NOT NULL DEFAULT TRUE,
	"FechaCreacion" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	"Descripcion" VARCHAR(150),
	"Porcentaje" NUMERIC(5,2),
	"FechaVencimiento" TIMESTAMP
);

CREATE INDEX idx_descuentos_fecha ON public."DescuentosAutorizados"("FechaCreacion");

-- ===== 7. TABLA: Ventas =====
CREATE TABLE public."Ventas" (
	"Id" SERIAL PRIMARY KEY,
	"Fecha" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	"ClienteId" INTEGER REFERENCES public."Clientes"("Id") ON DELETE RESTRICT,
	"VendedorId" INTEGER REFERENCES public."Vendedores"("Id") ON DELETE RESTRICT,
	"MetodoPagoId" INTEGER REFERENCES public."MetodosPago"("Id") ON DELETE RESTRICT,
	"Total" NUMERIC(10,2) NOT NULL CHECK ("Total" >= 0),
	"MontoRecibido" NUMERIC(10,2),
	"Vuelto" NUMERIC(10,2),
	"Descuento" NUMERIC(10,2) NOT NULL DEFAULT 0 CHECK ("Descuento" >= 0),
	"DescuentoAutorizadoId" INTEGER REFERENCES public."DescuentosAutorizados"("Id") ON DELETE SET NULL,
	"Estado" VARCHAR(50) DEFAULT 'Completada',
	"FechaModificacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_ventas_fecha ON public."Ventas"("Fecha" DESC);
CREATE INDEX idx_ventas_cliente ON public."Ventas"("ClienteId");
CREATE INDEX idx_ventas_vendedor ON public."Ventas"("VendedorId");

-- ===== 8. TABLA: DetalleVentas =====
CREATE TABLE public."DetalleVentas" (
	"Id" SERIAL PRIMARY KEY,
	"VentaId" INTEGER NOT NULL REFERENCES public."Ventas"("Id") ON DELETE CASCADE,
	"PrendaId" INTEGER NOT NULL REFERENCES public."Prendas"("Id") ON DELETE RESTRICT,
	"Cantidad" INTEGER NOT NULL CHECK ("Cantidad" > 0),
	"Precio" NUMERIC(10,2) NOT NULL CHECK ("Precio" > 0),
	"Subtotal" NUMERIC(10,2) NOT NULL CHECK ("Subtotal" >= 0)
);

CREATE INDEX idx_detalleventa_venta ON public."DetalleVentas"("VentaId");
CREATE INDEX idx_detalleventa_prenda ON public."DetalleVentas"("PrendaId");

-- ===== 9. TABLA: ConfiguracionSistema =====
CREATE TABLE public."ConfiguracionSistema" (
	"Id" SERIAL PRIMARY KEY,
	"NombreTienda" VARCHAR(200) NOT NULL,
	"RutaLogo" VARCHAR(500),
	"RutaFavicon" VARCHAR(500),
	"RutaImagenLogin" VARCHAR(500),
	"RutaImagenInstitucional" VARCHAR(500),
	"RutaBanner" VARCHAR(500),
	"RutaFondoLogin" VARCHAR(500),
	"RutaFondoDashboard" VARCHAR(500),
	"RutaFondoMenu" VARCHAR(500),
	"ColorPrimario" VARCHAR(50) NOT NULL,
	"ColorSecundario" VARCHAR(50) NOT NULL,
	"ColorMenu" VARCHAR(50) NOT NULL,
	"ColorBotones" VARCHAR(50) NOT NULL,
	"ColorDashboard" VARCHAR(50) NOT NULL,
	"ColorFondo" VARCHAR(50) NOT NULL,
	"TemaSeleccionado" VARCHAR(100) NOT NULL,
	"TemaOscuro" BOOLEAN NOT NULL DEFAULT FALSE,
	"NombrePropietario" VARCHAR(150),
	"Telefono" VARCHAR(50),
	"Correo" VARCHAR(150),
	"Direccion" VARCHAR(250),
	"Ciudad" VARCHAR(150),
	"Pais" VARCHAR(150),
	"CodigoPostal" VARCHAR(50),
	"RUC" VARCHAR(50),
	"Descripcion" VARCHAR(500),
	"FacebookUrl" VARCHAR(500),
	"InstagramUrl" VARCHAR(500),
	"TwitterUrl" VARCHAR(500),
	"LinkedInUrl" VARCHAR(500),
	"TikTokUrl" VARCHAR(500),
	"TextoPiePagina" VARCHAR(500),
	"FechaCreacion" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	"FechaActualizacion" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	"TiendaNombre" VARCHAR(100),
	"TiendaDescripcion" VARCHAR(500),
	"TiendaTelefono" VARCHAR(15),
	"TiendaEmail" VARCHAR(100),
	"TiendaDireccion" VARCHAR(250),
	"DescuentoMaximoAutoSinAutorizacion" NUMERIC(5,2)
);

-- ===== 10. TABLA: ConfiguracionAuditoria =====
CREATE TABLE public."ConfiguracionAuditoria" (
	"Id" SERIAL PRIMARY KEY,
	"RegistrarAccesos" BOOLEAN DEFAULT TRUE,
	"RegistrarCambios" BOOLEAN DEFAULT TRUE,
	"RegistrarErrores" BOOLEAN DEFAULT TRUE,
	"DiasBorrarLogs" INTEGER DEFAULT 90
);

-- ===== 11. TABLA: AspNetUsers (Identity) =====
CREATE TABLE public."AspNetUsers" (
	"Id" VARCHAR(450) PRIMARY KEY,
	"UserName" VARCHAR(256),
	"NormalizedUserName" VARCHAR(256) UNIQUE,
	"Email" VARCHAR(256),
	"NormalizedEmail" VARCHAR(256),
	"EmailConfirmed" BOOLEAN NOT NULL DEFAULT FALSE,
	"PasswordHash" TEXT,
	"SecurityStamp" TEXT,
	"ConcurrencyStamp" TEXT,
	"PhoneNumber" VARCHAR(20),
	"PhoneNumberConfirmed" BOOLEAN NOT NULL DEFAULT FALSE,
	"TwoFactorEnabled" BOOLEAN NOT NULL DEFAULT FALSE,
	"LockoutEnd" TIMESTAMP,
	"LockoutEnabled" BOOLEAN NOT NULL DEFAULT TRUE,
	"AccessFailedCount" INTEGER NOT NULL DEFAULT 0,
	"Discriminator" VARCHAR(50),
	"Activo" BOOLEAN NOT NULL DEFAULT TRUE,  -- <-- Columna requerida para compatibilidad con ApplicationUser
	"FechaRegistro" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_aspnetusers_username ON public."AspNetUsers"("UserName");
CREATE INDEX idx_aspnetusers_email ON public."AspNetUsers"("Email");

-- ===== 12. TABLA: AspNetRoles (Identity) =====
CREATE TABLE public."AspNetRoles" (
	"Id" VARCHAR(450) PRIMARY KEY,
	"Name" VARCHAR(256),
	"NormalizedName" VARCHAR(256) UNIQUE,
	"ConcurrencyStamp" TEXT
);

-- ===== 13. TABLA: AspNetUserRoles (Identity) =====
CREATE TABLE public."AspNetUserRoles" (
	"UserId" VARCHAR(450) NOT NULL REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE,
	"RoleId" VARCHAR(450) NOT NULL REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE,
	PRIMARY KEY ("UserId", "RoleId")
);

CREATE INDEX idx_aspnetuserroles_role ON public."AspNetUserRoles"("RoleId");

-- ===== 14. TABLA: AspNetUserClaims (Identity) =====
CREATE TABLE public."AspNetUserClaims" (
	"Id" SERIAL PRIMARY KEY,
	"UserId" VARCHAR(450) NOT NULL REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE,
	"ClaimType" TEXT,
	"ClaimValue" TEXT
);

CREATE INDEX idx_aspnetuserclaims_user ON public."AspNetUserClaims"("UserId");

-- ===== 15. TABLA: AspNetUserLogins (Identity) =====
CREATE TABLE public."AspNetUserLogins" (
	"LoginProvider" VARCHAR(128) NOT NULL,
	"ProviderKey" VARCHAR(128) NOT NULL,
	"ProviderDisplayName" VARCHAR(256),
	"UserId" VARCHAR(450) NOT NULL REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE,
	PRIMARY KEY ("LoginProvider", "ProviderKey")
);

CREATE INDEX idx_aspnetuserlogins_user ON public."AspNetUserLogins"("UserId");

-- ===== 16. TABLA: AspNetRoleClaims (Identity) =====
CREATE TABLE public."AspNetRoleClaims" (
	"Id" SERIAL PRIMARY KEY,
	"RoleId" VARCHAR(450) NOT NULL REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE,
	"ClaimType" TEXT,
	"ClaimValue" TEXT
);

CREATE INDEX idx_aspnetroleclaims_role ON public."AspNetRoleClaims"("RoleId");

-- ===== 17. TABLA: AspNetUserTokens (Identity) =====
CREATE TABLE public."AspNetUserTokens" (
	"UserId" VARCHAR(450) NOT NULL REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE,
	"LoginProvider" VARCHAR(128) NOT NULL,
	"Name" VARCHAR(128) NOT NULL,
	"Value" TEXT,
	PRIMARY KEY ("UserId", "LoginProvider", "Name")
);

-- =============================================================================
-- 🟢 SECCIÓN 3: INSERCIÓN DE DATOS DE SEMILLA (SEED DATA)
-- =============================================================================

-- 1. Categorías
INSERT INTO public."Categorias" ("Nombre", "Descripcion") VALUES 
('Prendas Superiores', 'Camisas, blusas, tops, sudaderas'),
('Prendas Inferiores', 'Pantalones, faldas, shorts, leggins'),
('Lencería', 'Ropa íntima y accesorios'),
('Accesorios', 'Cinturones, bufandas, gorros'),
('Abrigos', 'Chaquetas, abrigos, blazers')
ON CONFLICT ("Nombre") DO NOTHING;

-- 2. Métodos de Pago
INSERT INTO public."MetodosPago" ("Nombre", "Descripcion", "Activo") VALUES 
('Efectivo', 'Pago en efectivo', TRUE),
('Tarjeta Crédito', 'Pago con tarjeta crédito', TRUE),
('Tarjeta Débito', 'Pago con tarjeta débito', TRUE),
('Transferencia Bancaria', 'Transferencia a cuenta bancaria', TRUE),
('Cheque', 'Pago con cheque', TRUE)
ON CONFLICT ("Nombre") DO NOTHING;

-- 3. Prendas
INSERT INTO public."Prendas" ("Nombre", "Descripcion", "Talla", "Color", "Precio", "Stock", "CategoriaId", "Activo") VALUES 
('Camiseta Básica Blanca', 'Camiseta de algodón 100%, talla M-L-XL', 'M', 'Blanco', 29.99, 150, 1, TRUE),
('Camiseta Básica Negra', 'Camiseta de algodón 100%, talla M-L-XL', 'L', 'Negro', 29.99, 120, 1, TRUE),
('Blusa Elegante Rosa', 'Blusa de seda con botones, talla S-M-L', 'S', 'Rosa', 49.99, 80, 1, TRUE),
('Sudadera Gris', 'Sudadera de algodón con capucha, talla M-L-XL', 'XL', 'Gris', 59.99, 100, 1, TRUE),
('Jean Clásico Azul', 'Jean azul oscuro talla 28-32-34-36', '32', 'Azul', 79.99, 200, 2, TRUE),
('Jean Negro Slim', 'Jean negro slim fit talla 28-32-34-36', '30', 'Negro', 89.99, 150, 2, TRUE),
('Falda Plisada Negra', 'Falda plisada midi, talla S-M-L', 'M', 'Negro', 45.99, 90, 2, TRUE),
('Shorts Deportivos', 'Shorts cortos para deporte, talla S-M-L-XL', 'L', 'Azul', 34.99, 120, 2, TRUE),
('Leggins Deportivos', 'Leggins elásticos deportivos, talla S-M-L-XL', 'M', 'Gris', 39.99, 180, 2, TRUE),
('Set Lencería Encaje', 'Set lencería encaje, talla S-M-L', 'S', 'Rojo', 59.99, 60, 3, TRUE),
('Bralette Suave', 'Bralette cómodo sin varillas, talla S-M-L-XL', 'M', 'Blanco', 24.99, 100, 3, TRUE),
('Cinturón Cuero Negro', 'Cinturón de cuero auténtico, tallas 28-32-36-40', '34', 'Negro', 34.99, 80, 4, TRUE),
('Cinturón Tela Dorado', 'Cinturón de tela con hebilla dorada, universal', 'Única', 'Dorado', 19.99, 120, 4, TRUE),
('Bufanda Lana Gris', 'Bufanda de lana pura, larga', 'Única', 'Gris', 24.99, 70, 4, TRUE),
('Gorro Beanie Negro', 'Gorro beanie de punto, talla única', 'Única', 'Negro', 14.99, 150, 4, TRUE),
('Chaqueta Denim', 'Chaqueta denim azul clásica, talla S-M-L-XL', 'L', 'Azul', 99.99, 100, 5, TRUE),
('Blazer Formal Negro', 'Blazer formal para oficina, talla 2-4-6-8', 'M', 'Negro', 129.99, 50, 5, TRUE),
('Abrigo Invierno Beige', 'Abrigo largo invierno, talla S-M-L-XL', 'XL', 'Beige', 149.99, 40, 5, TRUE)
ON CONFLICT ("Nombre") DO NOTHING;

-- 4. Clientes
INSERT INTO public."Clientes" ("NombreCompleto", "DNI", "Telefono", "Email", "Direccion", "Nombre", "Apellido", "Activo") VALUES 
('Juan Pérez', '11111111', '555-1001', 'juan.perez@email.com', 'Calle 123 #456, Apartamento 101', 'Juan', 'Pérez', TRUE),
('María López', '22222222', '555-1002', 'maria.lopez@email.com', 'Avenida 789 #012, Apartamento 202', 'María', 'López', TRUE),
('Carlos Martínez', '33333333', '555-1003', 'carlos.martinez@email.com', 'Carrera 45 #67, Casa 303', 'Carlos', 'Martínez', TRUE),
('Ana García', '44444444', '555-1004', 'ana.garcia@email.com', 'Calle 234 #567, Apartamento 404', 'Ana', 'García', TRUE),
('Pedro Rodríguez', '55555555', '555-1005', 'pedro.rodriguez@email.com', 'Avenida 890 #123, Apartamento 505', 'Pedro', 'Rodríguez', TRUE),
('Sandra Fernández', '66666666', '555-1006', 'sandra.fernandez@email.com', 'Calle 345 #678, Casa 606', 'Sandra', 'Fernández', TRUE),
('Roberto Gómez', '77777777', '555-1007', 'roberto.gomez@email.com', 'Carrera 56 #78, Apartamento 707', 'Roberto', 'Gómez', TRUE),
('Laura Sánchez', '88888888', '555-1008', 'laura.sanchez@email.com', 'Calle 456 #789, Apartamento 808', 'Laura', 'Sánchez', TRUE),
('Miguel Torres', '99999999', '555-1009', 'miguel.torres@email.com', 'Avenida 901 #234, Casa 909', 'Miguel', 'Torres', TRUE),
('Patricia Reyes', '10101010', '555-1010', 'patricia.reyes@email.com', 'Calle 567 #890, Apartamento 1010', 'Patricia', 'Reyes', TRUE)
ON CONFLICT ("DNI") DO NOTHING;

-- 5. Vendedores
INSERT INTO public."Vendedores" ("Nombres", "Apellidos", "DNI", "Correo", "Telefono", "Activo", "Estado", "Nombre", "Apellido") VALUES 
('Carlos', 'Mendoza', '99001122', 'carlos.mendoza@fashionstore.com', '555-2001', TRUE, TRUE, 'Carlos', 'Mendoza'),
('Ana', 'Vallejo', '99003344', 'ana.vallejo@fashionstore.com', '555-2002', TRUE, TRUE, 'Ana', 'Vallejo'),
('Luis', 'Castro', '99005566', 'luis.castro@fashionstore.com', '555-2003', TRUE, TRUE, 'Luis', 'Castro'),
('Sofía', 'Ruiz', '99007788', 'sofia.ruiz@fashionstore.com', '555-2004', TRUE, TRUE, 'Sofía', 'Ruiz'),
('Diego', 'Moreno', '99009900', 'diego.moreno@fashionstore.com', '555-2005', TRUE, TRUE, 'Diego', 'Moreno')
ON CONFLICT ("DNI") DO NOTHING;

-- 6. Configuración del Sistema
INSERT INTO public."ConfiguracionSistema" (
	"NombreTienda", "ColorPrimario", "ColorSecundario", "ColorMenu", "ColorBotones", "ColorDashboard", "ColorFondo", "TemaSeleccionado", "TemaOscuro", "TextoPiePagina"
) VALUES (
	'FashionStore', '#667eea', '#764ba2', '#2c3e50', '#667eea', '#ecf0f1', '#f5f7fa', 'Fashion Store', FALSE, '&copy; 2025 FashionStore. Todos los derechos reservados.'
);

-- 7. Configuración de Auditoría
INSERT INTO public."ConfiguracionAuditoria" ("RegistrarAccesos", "RegistrarCambios", "RegistrarErrores", "DiasBorrarLogs") 
VALUES (TRUE, TRUE, TRUE, 90);

-- 8. Roles de Identity
INSERT INTO public."AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp") VALUES
('00000000-0000-0000-0000-000000000001', 'Administrador', 'ADMINISTRADOR', '00000000-0000-0000-0000-000000000001'),
('00000000-0000-0000-0000-000000000002', 'Vendedor', 'VENDEDOR', '00000000-0000-0000-0000-000000000002'),
('00000000-0000-0000-0000-000000000003', 'Gerente', 'GERENTE', '00000000-0000-0000-0000-000000000003')
ON CONFLICT ("NormalizedName") DO NOTHING;

-- =============================================================================
-- FIN DEL SCRIPT - RECREACIÓN Y SEMILLA COMPLETADAS EXITOSAMENTE
-- =============================================================================
