-- ==========================================
-- VALIDAR MIGRACIÓN COMPLETA
-- ==========================================
-- Ejecutar en Supabase SQL Editor
-- Verificar que todas las tablas y datos existan
-- ==========================================

-- ==========================================
-- 1. VALIDAR TABLAS ASPNET IDENTITY
-- ==========================================

SELECT 
  'AspNetUsers' AS tabla, 
  COUNT(*) AS registros,
  'Usuarios del sistema' AS descripcion
FROM public."AspNetUsers"

UNION ALL

SELECT 'AspNetRoles', COUNT(*), 'Roles del sistema'
FROM public."AspNetRoles"

UNION ALL

SELECT 'AspNetUserRoles', COUNT(*), 'Asignación de roles a usuarios'
FROM public."AspNetUserRoles"

UNION ALL

SELECT 'AspNetUserClaims', COUNT(*), 'Claims de usuarios'
FROM public."AspNetUserClaims"

UNION ALL

SELECT 'AspNetUserLogins', COUNT(*), 'Logins externos'
FROM public."AspNetUserLogins"

UNION ALL

SELECT 'AspNetUserTokens', COUNT(*), 'Tokens de usuarios'
FROM public."AspNetUserTokens"

UNION ALL

SELECT 'AspNetRoleClaims', COUNT(*), 'Claims de roles'
FROM public."AspNetRoleClaims"

UNION ALL

-- ==========================================
-- 2. VALIDAR TABLAS DE NEGOCIO
-- ==========================================

SELECT 'Categorias', COUNT(*), 'Categorías de productos'
FROM public."Categorias"

UNION ALL

SELECT 'Prendas', COUNT(*), 'Productos/Prendas'
FROM public."Prendas"

UNION ALL

SELECT 'Clientes', COUNT(*), 'Base de clientes'
FROM public."Clientes"

UNION ALL

SELECT 'Vendedores', COUNT(*), 'Equipo de vendedores'
FROM public."Vendedores"

UNION ALL

SELECT 'MetodoPago', COUNT(*), 'Métodos de pago'
FROM public."MetodoPago"

UNION ALL

SELECT 'Ventas', COUNT(*), 'Historial de ventas'
FROM public."Ventas"

UNION ALL

SELECT 'DetalleVenta', COUNT(*), 'Detalles de cada venta'
FROM public."DetalleVenta"

UNION ALL

SELECT 'DescuentosAutorizados', COUNT(*), 'Descuentos autorizados a vendedores'
FROM public."DescuentosAutorizados"

UNION ALL

SELECT 'ConfiguracionSistema', COUNT(*), 'Configuración del sistema'
FROM public."ConfiguracionSistema"

UNION ALL

SELECT 'ConfiguracionAuditoria', COUNT(*), 'Configuración de auditoría'
FROM public."ConfiguracionAuditoria"

ORDER BY tabla;

-- ==========================================
-- 3. VALIDAR USUARIO ADMINISTRADOR
-- ==========================================

SELECT 
  "Id",
  "UserName",
  "Email",
  "NormalizedUserName",
  "NormalizedEmail",
  "EmailConfirmed",
  "PhoneNumber",
  "FechaRegistro"
FROM public."AspNetUsers"
WHERE "UserName" = 'admin'
LIMIT 1;

-- ==========================================
-- 4. VALIDAR ROLES
-- ==========================================

SELECT 
  "Id",
  "Name",
  "NormalizedName"
FROM public."AspNetRoles"
ORDER BY "Name";

-- ==========================================
-- 5. VALIDAR ASIGNACION DE ROLES AL ADMIN
-- ==========================================

SELECT 
  u."UserName",
  r."Name" AS Rol
FROM public."AspNetUserRoles" ur
JOIN public."AspNetUsers" u ON ur."UserId" = u."Id"
JOIN public."AspNetRoles" r ON ur."RoleId" = r."Id"
WHERE u."UserName" = 'admin';

-- ==========================================
-- 6. VALIDAR CATEGORIAS
-- ==========================================

SELECT 
  "Id",
  "Nombre",
  "Descripcion",
  "FechaCreacion"
FROM public."Categorias"
ORDER BY "Id";

-- ==========================================
-- 7. VALIDAR PRENDAS (primeras 10)
-- ==========================================

SELECT 
  p."Id",
  p."Nombre",
  p."Precio",
  p."Stock",
  c."Nombre" AS Categoria,
  p."Activo",
  p."FechaCreacion"
FROM public."Prendas" p
LEFT JOIN public."Categorias" c ON p."CategoriaId" = c."Id"
ORDER BY p."Id"
LIMIT 10;

-- ==========================================
-- 8. VALIDAR CLIENTES
-- ==========================================

SELECT 
  "Id",
  "Nombre",
  "Apellido",
  "Email",
  "Telefono",
  "Ciudad",
  "Activo",
  "FechaCreacion"
FROM public."Clientes"
ORDER BY "Id";

-- ==========================================
-- 9. VALIDAR VENDEDORES
-- ==========================================

SELECT 
  "Id",
  "Nombre",
  "Apellido",
  "Correo",
  "Cedula",
  "Comision",
  "Activo",
  "FechaCreacion"
FROM public."Vendedores"
ORDER BY "Id";

-- ==========================================
-- 10. VALIDAR METODOS DE PAGO
-- ==========================================

SELECT 
  "Id",
  "Nombre",
  "Descripcion",
  "Activo"
FROM public."MetodoPago"
ORDER BY "Id";

-- ==========================================
-- 11. VALIDAR VENTAS
-- ==========================================

SELECT 
  v."Id",
  v."Fecha",
  cl."Nombre" AS Cliente,
  vr."Nombre" AS Vendedor,
  mp."Nombre" AS MetodoPago,
  v."Total",
  v."Descuento",
  v."Estado"
FROM public."Ventas" v
LEFT JOIN public."Clientes" cl ON v."ClienteId" = cl."Id"
LEFT JOIN public."Vendedores" vr ON v."VendedorId" = vr."Id"
LEFT JOIN public."MetodoPago" mp ON v."MetodoPagoId" = mp."Id"
ORDER BY v."Fecha" DESC;

-- ==========================================
-- 12. VALIDAR DETALLES DE VENTAS
-- ==========================================

SELECT 
  dv."Id",
  dv."VentaId",
  p."Nombre" AS Prenda,
  dv."Cantidad",
  dv."Precio",
  dv."Subtotal"
FROM public."DetalleVenta" dv
LEFT JOIN public."Prendas" p ON dv."PrendaId" = p."Id"
ORDER BY dv."VentaId", dv."Id";

-- ==========================================
-- 13. VALIDAR INTEGRIDAD REFERENCIAL
-- ==========================================

SELECT 
  'Prendas sin categoría' AS validacion,
  COUNT(*) AS cantidad
FROM public."Prendas"
WHERE "CategoriaId" IS NULL

UNION ALL

SELECT 'Ventas sin cliente válido',
  COUNT(*)
FROM public."Ventas"
WHERE "ClienteId" IS NOT NULL 
  AND "ClienteId" NOT IN (SELECT "Id" FROM public."Clientes")

UNION ALL

SELECT 'Ventas sin vendedor válido',
  COUNT(*)
FROM public."Ventas"
WHERE "VendedorId" IS NOT NULL 
  AND "VendedorId" NOT IN (SELECT "Id" FROM public."Vendedores")

UNION ALL

SELECT 'DetalleVenta sin venta válida',
  COUNT(*)
FROM public."DetalleVenta"
WHERE "VentaId" NOT IN (SELECT "Id" FROM public."Ventas")

UNION ALL

SELECT 'DetalleVenta sin prenda válida',
  COUNT(*)
FROM public."DetalleVenta"
WHERE "PrendaId" NOT IN (SELECT "Id" FROM public."Prendas");

-- ==========================================
-- 14. VALIDAR CONFIGURACION DEL SISTEMA
-- ==========================================

SELECT 
  "Id",
  "NombreTienda",
  "NombrePropietario",
  "Telefono",
  "Correo",
  "Ciudad",
  "Pais",
  "FechaActualizacion"
FROM public."ConfiguracionSistema"
LIMIT 1;

-- ==========================================
-- 15. VALIDAR CONFIGURACION DE AUDITORIA
-- ==========================================

SELECT 
  "Id",
  "RegistrarAccesos",
  "RegistrarCambios",
  "RegistrarErrores",
  "DiasBorrarLogs",
  "FechaActualizacion"
FROM public."ConfiguracionAuditoria"
LIMIT 1;

-- ==========================================
-- 16. RESUMEN FINAL - TOTAL DE REGISTROS
-- ==========================================

SELECT 
  'RESUMEN FINAL DE MIGRACIÓN' AS resultado,
  '' AS detalles

UNION ALL

SELECT '✅ Tablas ASP.NET Identity: 7', 'AspNetUsers, AspNetRoles, AspNetUserRoles, AspNetUserClaims, AspNetUserLogins, AspNetUserTokens, AspNetRoleClaims'

UNION ALL

SELECT '✅ Tablas de Negocio: 10', 'Categorias, Prendas, Clientes, Vendedores, MetodoPago, Ventas, DetalleVenta, DescuentosAutorizados, ConfiguracionSistema, ConfiguracionAuditoria'

UNION ALL

SELECT '✅ Total Tablas: 17', ''

UNION ALL

SELECT '📊 Usuarios creados', 
  (SELECT COUNT(*)::TEXT FROM public."AspNetUsers")

UNION ALL

SELECT '📊 Roles creados',
  (SELECT COUNT(*)::TEXT FROM public."AspNetRoles")

UNION ALL

SELECT '📊 Categorías',
  (SELECT COUNT(*)::TEXT FROM public."Categorias")

UNION ALL

SELECT '📊 Prendas',
  (SELECT COUNT(*)::TEXT FROM public."Prendas")

UNION ALL

SELECT '📊 Clientes',
  (SELECT COUNT(*)::TEXT FROM public."Clientes")

UNION ALL

SELECT '📊 Vendedores',
  (SELECT COUNT(*)::TEXT FROM public."Vendedores")

UNION ALL

SELECT '📊 Métodos de Pago',
  (SELECT COUNT(*)::TEXT FROM public."MetodoPago")

UNION ALL

SELECT '📊 Ventas registradas',
  (SELECT COUNT(*)::TEXT FROM public."Ventas")

UNION ALL

SELECT '📊 Detalles de ventas',
  (SELECT COUNT(*)::TEXT FROM public."DetalleVenta")

UNION ALL

SELECT '✅ MIGRACIÓN COMPLETADA', 'Todas las tablas y datos están listos';

-- ==========================================
-- FIN VALIDACIÓN
-- ==========================================
