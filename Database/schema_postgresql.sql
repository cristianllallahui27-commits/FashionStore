-- Esquema básico generado manualmente para PostgreSQL (supabase)
-- Este archivo es un punto de partida; ejecutar pruebas y ajustar tipos según sea necesario.

CREATE SCHEMA IF NOT EXISTS public;

-- AspNetRoles
CREATE TABLE IF NOT EXISTS public."AspNetRoles" (
	"Id" varchar(450) NOT NULL PRIMARY KEY,
	"Name" varchar(256),
	"NormalizedName" varchar(256),
	"ConcurrencyStamp" varchar(256)
);

-- AspNetUsers
CREATE TABLE IF NOT EXISTS public."AspNetUsers" (
	"Id" varchar(450) NOT NULL PRIMARY KEY,
	"UserName" varchar(256),
	"NormalizedUserName" varchar(256),
	"Email" varchar(256),
	"NormalizedEmail" varchar(256),
	"EmailConfirmed" boolean NOT NULL DEFAULT false,
	"PasswordHash" text,
	"SecurityStamp" text,
	"ConcurrencyStamp" text,
	"PhoneNumber" text,
	"PhoneNumberConfirmed" boolean NOT NULL DEFAULT false,
	"TwoFactorEnabled" boolean NOT NULL DEFAULT false,
	"LockoutEnd" timestamp with time zone,
	"LockoutEnabled" boolean NOT NULL DEFAULT false,
	"AccessFailedCount" integer NOT NULL DEFAULT 0,
	"Activo" boolean NOT NULL DEFAULT true
);

-- AspNetUserRoles
CREATE TABLE IF NOT EXISTS public."AspNetUserRoles" (
	"UserId" varchar(450) NOT NULL,
	"RoleId" varchar(450) NOT NULL,
	PRIMARY KEY ("UserId", "RoleId")
);

-- DescuentosAutorizados
CREATE TABLE IF NOT EXISTS public."DescuentosAutorizados" (
	"Id" serial PRIMARY KEY,
	"Nombre" varchar(100) NOT NULL,
	"Tipo" integer NOT NULL,
	"Valor" numeric(10,2) NOT NULL,
	"Activo" boolean NOT NULL DEFAULT true,
	"FechaCreacion" timestamp NOT NULL DEFAULT now()
);

-- Configuraciones
CREATE TABLE IF NOT EXISTS public."Configuraciones" (
	"Id" integer PRIMARY KEY,
	"NombreTienda" varchar(200),
	"RutaLogo" text,
	"RutaFavicon" text,
	"RutaImagenLogin" text,
	"RutaImagenInstitucional" text,
	"RutaBanner" text,
	"RutaFondoLogin" text,
	"RutaFondoDashboard" text,
	"RutaFondoMenu" text,
	"ColorPrimario" varchar(20),
	"ColorSecundario" varchar(20),
	"ColorMenu" varchar(20),
	"ColorBotones" varchar(20),
	"ColorDashboard" varchar(20),
	"ColorFondo" varchar(20),
	"TemaSeleccionado" varchar(100),
	"TemaOscuro" boolean NOT NULL DEFAULT false,
	"NombrePropietario" varchar(200),
	"Telefono" varchar(50),
	"Correo" varchar(200),
	"Direccion" varchar(400),
	"Ciudad" varchar(100),
	"Pais" varchar(100),
	"CodigoPostal" varchar(50),
	"RUC" varchar(50),
	"Descripcion" text,
	"FacebookUrl" text,
	"InstagramUrl" text,
	"TwitterUrl" text,
	"LinkedInUrl" text,
	"TikTokUrl" text,
	"TextoPiePagina" text,
	"FechaCreacion" timestamp NOT NULL DEFAULT now(),
	"FechaActualizacion" timestamp NOT NULL DEFAULT now()
);

-- ConfiguracionesAuditoria
CREATE TABLE IF NOT EXISTS public."ConfiguracionesAuditoria" (
	"Id" serial PRIMARY KEY,
	"UsuarioId" varchar(450),
	"NombreUsuario" varchar(200),
	"PropiedadModificada" varchar(200),
	"ValorAnterior" varchar(1000),
	"ValorNuevo" varchar(1000),
	"FechaCambio" timestamp NOT NULL DEFAULT now(),
	"Descripcion" text
);

-- Otras tablas de negocio (simplificadas si no existen)
CREATE TABLE IF NOT EXISTS public."Categorias" (
	"Id" serial PRIMARY KEY,
	"Nombre" varchar(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS public."Prendas" (
	"Id" serial PRIMARY KEY,
	"Nombre" varchar(200) NOT NULL,
	"CategoriaId" integer,
	"Precio" numeric(10,2) NOT NULL,
	CONSTRAINT fk_prenda_categoria FOREIGN KEY("CategoriaId") REFERENCES public."Categorias"("Id")
);

CREATE TABLE IF NOT EXISTS public."Clientes" (
	"Id" serial PRIMARY KEY,
	"NombreCompleto" varchar(150)
);

CREATE TABLE IF NOT EXISTS public."Vendedores" (
	"Id" serial PRIMARY KEY,
	"Nombres" varchar(150),
	"Apellidos" varchar(150),
	"Correo" varchar(200),
	"Estado" boolean NOT NULL DEFAULT true
);

CREATE TABLE IF NOT EXISTS public."MetodosPago" (
	"Id" serial PRIMARY KEY,
	"Nombre" varchar(50)
);

CREATE TABLE IF NOT EXISTS public."Ventas" (
	"Id" serial PRIMARY KEY,
	"ClienteId" integer,
	"VendedorId" integer,
	"MetodoPagoId" integer,
	"Total" numeric(10,2) NOT NULL,
	CONSTRAINT fk_venta_cliente FOREIGN KEY("ClienteId") REFERENCES public."Clientes"("Id"),
	CONSTRAINT fk_venta_vendedor FOREIGN KEY("VendedorId") REFERENCES public."Vendedores"("Id"),
	CONSTRAINT fk_venta_metodo FOREIGN KEY("MetodoPagoId") REFERENCES public."MetodosPago"("Id")
);

CREATE TABLE IF NOT EXISTS public."DetalleVentas" (
	"Id" serial PRIMARY KEY,
	"VentaId" integer NOT NULL,
	"PrendaId" integer NOT NULL,
	"Precio" numeric(10,2) NOT NULL,
	"Subtotal" numeric(10,2) NOT NULL,
	CONSTRAINT fk_detalle_venta FOREIGN KEY("VentaId") REFERENCES public."Ventas"("Id"),
	CONSTRAINT fk_detalle_prenda FOREIGN KEY("PrendaId") REFERENCES public."Prendas"("Id")
);

-- Indices simples
CREATE INDEX IF NOT EXISTS idx_prenda_categoria ON public."Prendas"("CategoriaId");
CREATE INDEX IF NOT EXISTS idx_venta_cliente ON public."Ventas"("ClienteId");

-- Insert default configuración
INSERT INTO public."Configuraciones" ("Id", "NombreTienda", "ColorPrimario", "ColorSecundario", "ColorMenu", "ColorBotones", "ColorDashboard", "ColorFondo", "TemaSeleccionado", "TemaOscuro", "FechaCreacion", "FechaActualizacion")
VALUES (1, 'FashionStore', '#667eea', '#764ba2', '#2c3e50', '#667eea', '#ecf0f1', '#f5f7fa', 'Fashion Store', false, now(), now())
ON CONFLICT ("Id") DO NOTHING;
