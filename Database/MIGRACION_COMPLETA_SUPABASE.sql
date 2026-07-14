-- ==========================================
-- MIGRACIÓN COMPLETA: SQL Server → Supabase
-- ==========================================
-- Fashion Store - Sistema Administrativo
-- Base de datos: postgres (Supabase)
-- Ejecutar en: Supabase SQL Editor
-- Fecha: 7 de Julio, 2026
-- ==========================================

-- ==========================================
-- 1. CREAR TABLA ASPNETUSERS (ASP.NET Identity)
-- ==========================================

CREATE TABLE IF NOT EXISTS public."AspNetUsers" (
    "Id" VARCHAR(450) PRIMARY KEY,
    "UserName" VARCHAR(256),
    "NormalizedUserName" VARCHAR(256) UNIQUE,
    "Email" VARCHAR(256),
    "NormalizedEmail" VARCHAR(256),
    "EmailConfirmed" BOOLEAN DEFAULT FALSE,
    "PasswordHash" TEXT,
    "SecurityStamp" TEXT,
    "ConcurrencyStamp" TEXT,
    "PhoneNumber" VARCHAR(20),
    "PhoneNumberConfirmed" BOOLEAN DEFAULT FALSE,
    "TwoFactorEnabled" BOOLEAN DEFAULT FALSE,
    "LockoutEnd" TIMESTAMP NULL,
    "LockoutEnabled" BOOLEAN DEFAULT TRUE,
    "AccessFailedCount" INTEGER DEFAULT 0,
    "Discriminator" VARCHAR(MAX),
    "FechaRegistro" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_aspnetusers_normalized_email ON public."AspNetUsers"("NormalizedEmail");
CREATE INDEX IF NOT EXISTS idx_aspnetusers_normalized_username ON public."AspNetUsers"("NormalizedUserName");

-- ==========================================
-- 2. CREAR TABLA ASPNETROLES (ASP.NET Identity)
-- ==========================================

CREATE TABLE IF NOT EXISTS public."AspNetRoles" (
    "Id" VARCHAR(450) PRIMARY KEY,
    "Name" VARCHAR(256),
    "NormalizedName" VARCHAR(256) UNIQUE,
    "ConcurrencyStamp" TEXT
);

-- ==========================================
-- 3. CREAR TABLA ASPNETUSERROLES (ASP.NET Identity)
-- ==========================================

CREATE TABLE IF NOT EXISTS public."AspNetUserRoles" (
    "UserId" VARCHAR(450),
    "RoleId" VARCHAR(450),
    PRIMARY KEY ("UserId", "RoleId"),
    FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_aspnetuserroles_role_id ON public."AspNetUserRoles"("RoleId");

-- ==========================================
-- 4. CREAR TABLA ASPNETUSERCLAIMS (ASP.NET Identity)
-- ==========================================

CREATE TABLE IF NOT EXISTS public."AspNetUserClaims" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" VARCHAR(450) NOT NULL,
    "ClaimType" TEXT,
    "ClaimValue" TEXT,
    FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_aspnetuserclaims_user_id ON public."AspNetUserClaims"("UserId");

-- ==========================================
-- 5. CREAR TABLA ASPNETUSERLOGINS (ASP.NET Identity)
-- ==========================================

CREATE TABLE IF NOT EXISTS public."AspNetUserLogins" (
    "LoginProvider" VARCHAR(128),
    "ProviderKey" VARCHAR(128),
    "ProviderDisplayName" TEXT,
    "UserId" VARCHAR(450),
    PRIMARY KEY ("LoginProvider", "ProviderKey"),
    FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_aspnetuserlogins_user_id ON public."AspNetUserLogins"("UserId");

-- ==========================================
-- 6. CREAR TABLA ASPNETUSERTOKENS (ASP.NET Identity)
-- ==========================================

CREATE TABLE IF NOT EXISTS public."AspNetUserTokens" (
    "UserId" VARCHAR(450),
    "LoginProvider" VARCHAR(128),
    "Name" VARCHAR(128),
    "Value" TEXT,
    PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    FOREIGN KEY ("UserId") REFERENCES public."AspNetUsers"("Id") ON DELETE CASCADE
);

-- ==========================================
-- 7. CREAR TABLA ASPNETROLECLAIMS (ASP.NET Identity)
-- ==========================================

CREATE TABLE IF NOT EXISTS public."AspNetRoleClaims" (
    "Id" SERIAL PRIMARY KEY,
    "RoleId" VARCHAR(450) NOT NULL,
    "ClaimType" TEXT,
    "ClaimValue" TEXT,
    FOREIGN KEY ("RoleId") REFERENCES public."AspNetRoles"("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_aspnetroleclaims_role_id ON public."AspNetRoleClaims"("RoleId");

-- ==========================================
-- 8. INSERTAR ROLES
-- ==========================================

INSERT INTO public."AspNetRoles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp") VALUES
('00000000-0000-0000-0000-000000000001', 'Administrador', 'ADMINISTRADOR', '00000000-0000-0000-0000-000000000001'),
('00000000-0000-0000-0000-000000000002', 'Vendedor', 'VENDEDOR', '00000000-0000-0000-0000-000000000002'),
('00000000-0000-0000-0000-000000000003', 'Gerente', 'GERENTE', '00000000-0000-0000-0000-000000000003')
ON CONFLICT DO NOTHING;

-- ==========================================
-- 9. INSERTAR USUARIO ADMINISTRADOR DE PRUEBA
-- ==========================================
-- Username: admin
-- Email: Admin@gmail.com
-- Password: Admin123! (hasheada con Bcrypt)

INSERT INTO public."AspNetUsers" (
    "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail",
    "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp",
    "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled",
    "AccessFailedCount", "Discriminator"
) VALUES (
    '10000000-0000-0000-0000-000000000001',
    'admin',
    'ADMIN',
    'Admin@gmail.com',
    'ADMIN@GMAIL.COM',
    TRUE,
    '$2a$11$k9HQPKO3gJKx0HQWZQZJC.u6W3GLAhgvL8PjRWLn8iH7FfWpI5Gm2',  -- Admin123!
    '00000000000000000000000000000001',
    '00000000000000000000000000000001',
    '+57-1-2345678',
    TRUE,
    FALSE,
    TRUE,
    0,
    'ApplicationUser'
) ON CONFLICT DO NOTHING;

-- ==========================================
-- 10. ASIGNAR ROLE ADMINISTRADOR AL USUARIO
-- ==========================================

INSERT INTO public."AspNetUserRoles" ("UserId", "RoleId") VALUES
('10000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')
ON CONFLICT DO NOTHING;

-- ==========================================
-- 11. CREAR TABLA CATEGORIAS
-- ==========================================

CREATE TABLE IF NOT EXISTS public."Categorias" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(200) NOT NULL UNIQUE,
    "Descripcion" TEXT,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "FechaModificacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_categorias_nombre ON public."Categorias"("Nombre");

-- ==========================================
-- 12. CREAR TABLA PRENDAS
-- ==========================================

CREATE TABLE IF NOT EXISTS public."Prendas" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(200) NOT NULL UNIQUE,
    "Descripcion" TEXT,
    "Precio" NUMERIC(10,2) NOT NULL CHECK ("Precio" > 0),
    "Stock" INTEGER NOT NULL DEFAULT 0 CHECK ("Stock" >= 0),
    "CategoriaId" INTEGER NOT NULL,
    "ImagenUrl" VARCHAR(500),
    "Activo" BOOLEAN DEFAULT TRUE,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "FechaModificacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY ("CategoriaId") REFERENCES public."Categorias"("Id") ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS idx_prendas_categoria ON public."Prendas"("CategoriaId");
CREATE INDEX IF NOT EXISTS idx_prendas_nombre ON public."Prendas"("Nombre");
CREATE INDEX IF NOT EXISTS idx_prendas_activo ON public."Prendas"("Activo");

-- ==========================================
-- 13. CREAR TABLA CLIENTES
-- ==========================================

CREATE TABLE IF NOT EXISTS public."Clientes" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL,
    "Apellido" VARCHAR(100),
    "Telefono" VARCHAR(20),
    "Email" VARCHAR(100),
    "Direccion" TEXT,
    "Ciudad" VARCHAR(100),
    "Cedula" VARCHAR(20),
    "Activo" BOOLEAN DEFAULT TRUE,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "FechaModificacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_clientes_email ON public."Clientes"("Email");
CREATE INDEX IF NOT EXISTS idx_clientes_cedula ON public."Clientes"("Cedula");
CREATE INDEX IF NOT EXISTS idx_clientes_activo ON public."Clientes"("Activo");

-- ==========================================
-- 14. CREAR TABLA VENDEDORES
-- ==========================================

CREATE TABLE IF NOT EXISTS public."Vendedores" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL,
    "Apellido" VARCHAR(100),
    "Correo" VARCHAR(100) UNIQUE,
    "Telefono" VARCHAR(20),
    "Cedula" VARCHAR(20) UNIQUE,
    "Comision" NUMERIC(5,2) DEFAULT 0,
    "Activo" BOOLEAN DEFAULT TRUE,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "FechaModificacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX IF NOT EXISTS idx_vendedores_correo ON public."Vendedores"("Correo");
CREATE INDEX IF NOT EXISTS idx_vendedores_activo ON public."Vendedores"("Activo");

-- ==========================================
-- 15. CREAR TABLA METODOPAGO
-- ==========================================

CREATE TABLE IF NOT EXISTS public."MetodoPago" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(100) NOT NULL UNIQUE,
    "Descripcion" TEXT,
    "Activo" BOOLEAN DEFAULT TRUE,
    "FechaCreacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ==========================================
-- 16. CREAR TABLA VENTAS
-- ==========================================

CREATE TABLE IF NOT EXISTS public."Ventas" (
    "Id" SERIAL PRIMARY KEY,
    "ClienteId" INTEGER,
    "VendedorId" INTEGER,
    "MetodoPagoId" INTEGER,
    "Fecha" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "Total" NUMERIC(10,2) NOT NULL CHECK ("Total" >= 0),
    "Descuento" NUMERIC(10,2) DEFAULT 0,
    "ImpuID" VARCHAR(50),
    "Estado" VARCHAR(50) DEFAULT 'Completada',
    "FechaModificacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY ("ClienteId") REFERENCES public."Clientes"("Id") ON DELETE SET NULL,
    FOREIGN KEY ("VendedorId") REFERENCES public."Vendedores"("Id") ON DELETE SET NULL,
    FOREIGN KEY ("MetodoPagoId") REFERENCES public."MetodoPago"("Id") ON DELETE SET NULL
);

CREATE INDEX IF NOT EXISTS idx_ventas_fecha ON public."Ventas"("Fecha" DESC);
CREATE INDEX IF NOT EXISTS idx_ventas_vendedor ON public."Ventas"("VendedorId");
CREATE INDEX IF NOT EXISTS idx_ventas_cliente ON public."Ventas"("ClienteId");
CREATE INDEX IF NOT EXISTS idx_ventas_estado ON public."Ventas"("Estado");

-- ==========================================
-- 17. CREAR TABLA DETALLEVENTA
-- ==========================================

CREATE TABLE IF NOT EXISTS public."DetalleVenta" (
    "Id" SERIAL PRIMARY KEY,
    "VentaId" INTEGER NOT NULL,
    "PrendaId" INTEGER NOT NULL,
    "Cantidad" INTEGER NOT NULL CHECK ("Cantidad" > 0),
    "Precio" NUMERIC(10,2) NOT NULL CHECK ("Precio" > 0),
    "Subtotal" NUMERIC(10,2) GENERATED ALWAYS AS ("Cantidad" * "Precio") STORED,
    FOREIGN KEY ("VentaId") REFERENCES public."Ventas"("Id") ON DELETE CASCADE,
    FOREIGN KEY ("PrendaId") REFERENCES public."Prendas"("Id") ON DELETE RESTRICT
);

CREATE INDEX IF NOT EXISTS idx_detalleventa_venta ON public."DetalleVenta"("VentaId");
CREATE INDEX IF NOT EXISTS idx_detalleventa_prenda ON public."DetalleVenta"("PrendaId");

-- ==========================================
-- 18. CREAR TABLA DESCUENTOSAUTORIZADOS
-- ==========================================

CREATE TABLE IF NOT EXISTS public."DescuentosAutorizados" (
    "Id" SERIAL PRIMARY KEY,
    "VendedorId" INTEGER NOT NULL,
    "PorcientoDescuento" NUMERIC(5,2) NOT NULL,
    "Razon" TEXT,
    "FechaAutorizacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "VigenciaHasta" TIMESTAMP,
    "Activo" BOOLEAN DEFAULT TRUE,
    FOREIGN KEY ("VendedorId") REFERENCES public."Vendedores"("Id") ON DELETE CASCADE
);

-- ==========================================
-- 19. CREAR TABLA CONFIGURACIONSISTEMA
-- ==========================================

CREATE TABLE IF NOT EXISTS public."ConfiguracionSistema" (
    "Id" SERIAL PRIMARY KEY,
    "NombreTienda" VARCHAR(200),
    "RutaLogo" VARCHAR(500),
    "RutaFavicon" VARCHAR(500),
    "RutaImagenLogin" VARCHAR(500),
    "RutaImagenInstitucional" VARCHAR(500),
    "RutaBanner" VARCHAR(500),
    "RutaFondoLogin" VARCHAR(500),
    "RutaFondoDashboard" VARCHAR(500),
    "RutaFondoMenu" VARCHAR(500),
    "ColorPrimario" VARCHAR(7),
    "ColorSecundario" VARCHAR(7),
    "ColorMenu" VARCHAR(7),
    "ColorBotones" VARCHAR(7),
    "ColorDashboard" VARCHAR(7),
    "ColorFondo" VARCHAR(7),
    "TemaOscuro" BOOLEAN DEFAULT FALSE,
    "TemaSeleccionado" VARCHAR(50),
    "NombrePropietario" VARCHAR(200),
    "Telefono" VARCHAR(20),
    "Correo" VARCHAR(100),
    "Direccion" TEXT,
    "Ciudad" VARCHAR(100),
    "Pais" VARCHAR(100),
    "CodigoPostal" VARCHAR(10),
    "RUC" VARCHAR(50),
    "Descripcion" TEXT,
    "FacebookUrl" VARCHAR(500),
    "InstagramUrl" VARCHAR(500),
    "TwitterUrl" VARCHAR(500),
    "LinkedInUrl" VARCHAR(500),
    "TikTokUrl" VARCHAR(500),
    "TextoPiePagina" TEXT,
    "FechaActualizacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ==========================================
-- 20. CREAR TABLA CONFIGURACIONAUDITORIA
-- ==========================================

CREATE TABLE IF NOT EXISTS public."ConfiguracionAuditoria" (
    "Id" SERIAL PRIMARY KEY,
    "RegistrarAccesos" BOOLEAN DEFAULT TRUE,
    "RegistrarCambios" BOOLEAN DEFAULT TRUE,
    "RegistrarErrores" BOOLEAN DEFAULT TRUE,
    "DiasBorrarLogs" INTEGER DEFAULT 90,
    "FechaActualizacion" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- ==========================================
-- 21. INSERTAR DATOS INICIALES - CATEGORIAS
-- ==========================================

INSERT INTO public."Categorias" ("Nombre", "Descripcion") VALUES 
('Prendas Superiores', 'Camisas, blusas, tops, sudaderas'),
('Prendas Inferiores', 'Pantalones, faldas, shorts, leggins'),
('Lencería', 'Ropa íntima y accesorios'),
('Accesorios', 'Cinturones, bufandas, gorros'),
('Abrigos', 'Chaquetas, abrigos, blazers')
ON CONFLICT DO NOTHING;

-- ==========================================
-- 22. INSERTAR DATOS INICIALES - METODOS DE PAGO
-- ==========================================

INSERT INTO public."MetodoPago" ("Nombre", "Descripcion", "Activo") VALUES 
('Efectivo', 'Pago en efectivo', TRUE),
('Tarjeta Crédito', 'Pago con tarjeta crédito', TRUE),
('Tarjeta Débito', 'Pago con tarjeta débito', TRUE),
('Transferencia Bancaria', 'Transferencia a cuenta bancaria', TRUE),
('Cheque', 'Pago con cheque', TRUE)
ON CONFLICT DO NOTHING;

-- ==========================================
-- 23. INSERTAR DATOS INICIALES - PRENDAS
-- ==========================================

INSERT INTO public."Prendas" ("Nombre", "Descripcion", "Precio", "Stock", "CategoriaId", "Activo") VALUES 
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

-- ==========================================
-- 24. INSERTAR DATOS INICIALES - CLIENTES
-- ==========================================

INSERT INTO public."Clientes" ("Nombre", "Apellido", "Telefono", "Email", "Direccion", "Ciudad", "Activo") VALUES 
('Juan', 'Pérez', '555-1001', 'juan.perez@email.com', 'Calle 123 #456', 'Bogotá', TRUE),
('María', 'López', '555-1002', 'maria.lopez@email.com', 'Avenida 789 #012', 'Bogotá', TRUE),
('Carlos', 'Martínez', '555-1003', 'carlos.martinez@email.com', 'Carrera 45 #67', 'Medellín', TRUE),
('Ana', 'García', '555-1004', 'ana.garcia@email.com', 'Calle 234 #567', 'Cali', TRUE),
('Pedro', 'Rodríguez', '555-1005', 'pedro.rodriguez@email.com', 'Avenida 890 #123', 'Bogotá', TRUE),
('Sandra', 'Fernández', '555-1006', 'sandra.fernandez@email.com', 'Calle 345 #678', 'Barranquilla', TRUE),
('Roberto', 'Gómez', '555-1007', 'roberto.gomez@email.com', 'Carrera 56 #78', 'Bogotá', TRUE),
('Laura', 'Sánchez', '555-1008', 'laura.sanchez@email.com', 'Calle 456 #789', 'Cartagena', TRUE),
('Miguel', 'Torres', '555-1009', 'miguel.torres@email.com', 'Avenida 901 #234', 'Bogotá', TRUE),
('Patricia', 'Reyes', '555-1010', 'patricia.reyes@email.com', 'Calle 567 #890', 'Bogotá', TRUE)
ON CONFLICT DO NOTHING;

-- ==========================================
-- 25. INSERTAR DATOS INICIALES - VENDEDORES
-- ==========================================

INSERT INTO public."Vendedores" ("Nombre", "Apellido", "Correo", "Telefono", "Cedula", "Comision", "Activo") VALUES 
('Carlos', 'Mendoza', 'carlos.mendoza@fashionstore.com', '555-2001', '1234567890', 5.00, TRUE),
('Ana', 'Vallejo', 'ana.vallejo@fashionstore.com', '555-2002', '0987654321', 5.00, TRUE),
('Luis', 'Castro', 'luis.castro@fashionstore.com', '555-2003', '1122334455', 5.00, TRUE),
('Sofía', 'Ruiz', 'sofia.ruiz@fashionstore.com', '555-2004', '5544332211', 5.00, TRUE),
('Diego', 'Moreno', 'diego.moreno@fashionstore.com', '555-2005', '6677889900', 5.00, TRUE)
ON CONFLICT DO NOTHING;

-- ==========================================
-- 26. INSERTAR CONFIGURACION SISTEMA
-- ==========================================

INSERT INTO public."ConfiguracionSistema" (
    "NombreTienda", "ColorPrimario", "ColorSecundario", "ColorMenu", 
    "ColorBotones", "ColorFondo", "TemaOscuro", "NombrePropietario",
    "Telefono", "Correo", "Direccion", "Ciudad", "Pais", "Descripcion"
) VALUES (
    'FashionStore - Sistema Administrativo',
    '#667eea', '#764ba2', '#1a1a2e', '#667eea', '#f5f7fa', FALSE,
    'Fashion Store S.A.S',
    '+57-1-2345678',
    'info@fashionstore.com',
    'Carrera 7 #123, Edificio Corporate',
    'Bogotá',
    'Colombia',
    'Sistema administrativo integral para gestión de ventas e inventario'
)
ON CONFLICT DO NOTHING;

-- ==========================================
-- 27. INSERTAR CONFIGURACION AUDITORIA
-- ==========================================

INSERT INTO public."ConfiguracionAuditoria" (
    "RegistrarAccesos", "RegistrarCambios", "RegistrarErrores", "DiasBorrarLogs"
) VALUES (TRUE, TRUE, TRUE, 90)
ON CONFLICT DO NOTHING;

-- ==========================================
-- 28. VALIDAR CREACIÓN
-- ==========================================

SELECT 
  'Categorias' AS tabla, COUNT(*) AS registros FROM public."Categorias"
UNION ALL SELECT 'Prendas', COUNT(*) FROM public."Prendas"
UNION ALL SELECT 'Clientes', COUNT(*) FROM public."Clientes"
UNION ALL SELECT 'Vendedores', COUNT(*) FROM public."Vendedores"
UNION ALL SELECT 'MetodoPago', COUNT(*) FROM public."MetodoPago"
UNION ALL SELECT 'ConfiguracionSistema', COUNT(*) FROM public."ConfiguracionSistema"
UNION ALL SELECT 'ConfiguracionAuditoria', COUNT(*) FROM public."ConfiguracionAuditoria"
UNION ALL SELECT 'AspNetUsers', COUNT(*) FROM public."AspNetUsers"
UNION ALL SELECT 'AspNetRoles', COUNT(*) FROM public."AspNetRoles"
ORDER BY tabla;

-- ==========================================
-- FIN - MIGRACION COMPLETADA
-- ==========================================
