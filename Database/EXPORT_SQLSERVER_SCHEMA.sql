-- EXPORT SCHEMA - SQL Server
-- Exportar estructura completa de FashionStoreDB
-- Ejecutar en: SQL Server Management Studio

USE FashionStoreDB;

-- 1. TABLAS BASE (Sin Foreign Keys primero)
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE' 
ORDER BY TABLE_NAME;

-- 2. EXPORTAR SCHEMA DDL (copiar resultados)
SELECT OBJECT_DEFINITION(OBJECT_ID(TABLE_NAME)) AS TableDefinition
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_TYPE = 'BASE TABLE';

-- 3. TABLAS (Para verificar)
EXEC sp_tables;

-- 4. VERIFICAR DATOS (Conteos)
PRINT '=== CONTEOS DE DATOS ===';
SELECT 'Categorias' AS [Tabla], COUNT(*) AS [Registros] FROM Categorias
UNION ALL
SELECT 'Prendas', COUNT(*) FROM Prendas
UNION ALL
SELECT 'Clientes', COUNT(*) FROM Clientes
UNION ALL
SELECT 'Vendedores', COUNT(*) FROM Vendedores
UNION ALL
SELECT 'MetodoPago', COUNT(*) FROM MetodoPago
UNION ALL
SELECT 'Ventas', COUNT(*) FROM Ventas
UNION ALL
SELECT 'DetalleVenta', COUNT(*) FROM DetalleVenta
UNION ALL
SELECT 'ConfiguracionSistema', COUNT(*) FROM ConfiguracionSistema
UNION ALL
SELECT 'AspNetUsers', COUNT(*) FROM AspNetUsers
UNION ALL
SELECT 'AspNetRoles', COUNT(*) FROM AspNetRoles;

-- 5. SCRIPT PARA EXPORTAR DATOS (Generar INSERT statements)
-- Ejecutar en SQL Server y copiar output

-- Categorias
SELECT 'INSERT INTO Categorias (Id, Nombre, Descripcion, FechaCreacion) VALUES (' + 
       CAST(Id AS VARCHAR) + ', ''' + ISNULL(Nombre, '') + ''', ''' + ISNULL(Descripcion, '') + ''', ''' + CONVERT(VARCHAR, ISNULL(FechaCreacion, GETDATE()), 121) + ''');'
FROM Categorias;

-- Prendas (SIN FOREIGN KEYS TODAVÍA)
SELECT 'INSERT INTO Prendas (Id, Nombre, Descripcion, Precio, Stock, CategoriaId, Activo, FechaCreacion) VALUES (' + 
       CAST(Id AS VARCHAR) + ', ''' + ISNULL(Nombre, '') + ''', ''' + ISNULL(Descripcion, '') + ''', ' +
       CAST(Precio AS VARCHAR) + ', ' + CAST(Stock AS VARCHAR) + ', ' + CAST(ISNULL(CategoriaId, 0) AS VARCHAR) + ', ' +
       CAST(CAST(Activo AS INT) AS VARCHAR) + ', ''' + CONVERT(VARCHAR, ISNULL(FechaCreacion, GETDATE()), 121) + ''');'
FROM Prendas;

-- Clientes
SELECT 'INSERT INTO Clientes (Id, Nombre, Apellido, Telefono, Email, Direccion, Activo, FechaCreacion) VALUES (' + 
       CAST(Id AS VARCHAR) + ', ''' + ISNULL(Nombre, '') + ''', ''' + ISNULL(Apellido, '') + ''', ''' + ISNULL(Telefono, '') + ''', ''' + ISNULL(Email, '') + ''', ''' + ISNULL(Direccion, '') + ''', ' +
       CAST(CAST(Activo AS INT) AS VARCHAR) + ', ''' + CONVERT(VARCHAR, ISNULL(FechaCreacion, GETDATE()), 121) + ''');'
FROM Clientes;

-- Vendedores
SELECT 'INSERT INTO Vendedores (Id, Nombre, Apellido, Correo, Telefono, Cedula, Activo, FechaCreacion) VALUES (' + 
       CAST(Id AS VARCHAR) + ', ''' + ISNULL(Nombre, '') + ''', ''' + ISNULL(Apellido, '') + ''', ''' + ISNULL(Correo, '') + ''', ''' + ISNULL(Telefono, '') + ''', ''' + ISNULL(Cedula, '') + ''', ' +
       CAST(CAST(Activo AS INT) AS VARCHAR) + ', ''' + CONVERT(VARCHAR, ISNULL(FechaCreacion, GETDATE()), 121) + ''');'
FROM Vendedores;

-- MetodoPago
SELECT 'INSERT INTO MetodoPago (Id, Nombre, Descripcion, Activo) VALUES (' + 
       CAST(Id AS VARCHAR) + ', ''' + ISNULL(Nombre, '') + ''', ''' + ISNULL(Descripcion, '') + ''', ' +
       CAST(CAST(Activo AS INT) AS VARCHAR) + ');'
FROM MetodoPago;

-- Ventas
SELECT 'INSERT INTO Ventas (Id, ClienteId, VendedorId, MetodoPagoId, Fecha, Total, Descuento) VALUES (' + 
       CAST(Id AS VARCHAR) + ', ' + CAST(ISNULL(ClienteId, 0) AS VARCHAR) + ', ' + CAST(ISNULL(VendedorId, 0) AS VARCHAR) + ', ' +
       CAST(ISNULL(MetodoPagoId, 0) AS VARCHAR) + ', ''' + CONVERT(VARCHAR, ISNULL(Fecha, GETDATE()), 121) + ''', ' +
       CAST(Total AS VARCHAR) + ', ' + CAST(ISNULL(Descuento, 0) AS VARCHAR) + ');'
FROM Ventas;

-- DetalleVenta
SELECT 'INSERT INTO DetalleVenta (Id, VentaId, PrendaId, Cantidad, Precio) VALUES (' + 
       CAST(Id AS VARCHAR) + ', ' + CAST(ISNULL(VentaId, 0) AS VARCHAR) + ', ' + CAST(ISNULL(PrendaId, 0) AS VARCHAR) + ', ' +
       CAST(Cantidad AS VARCHAR) + ', ' + CAST(Precio AS VARCHAR) + ');'
FROM DetalleVenta;

-- ConfiguracionSistema
SELECT 'INSERT INTO ConfiguracionSistema (Id, NombreTienda, RutaLogo, RutaFavicon, ColorPrimario, ColorSecundario, ColorMenu, ColorBotones, ColorFondo, TemaOscuro) VALUES (' + 
       CAST(Id AS VARCHAR) + ', ''' + ISNULL(NombreTienda, '') + ''', ''' + ISNULL(RutaLogo, '') + ''', ''' + ISNULL(RutaFavicon, '') + ''', ''' + ISNULL(ColorPrimario, '') + ''', ''' + ISNULL(ColorSecundario, '') + ''', ''' + ISNULL(ColorMenu, '') + ''', ''' + ISNULL(ColorBotones, '') + ''', ''' + ISNULL(ColorFondo, '') + ''', ' +
       CAST(CAST(TemaOscuro AS INT) AS VARCHAR) + ');'
FROM ConfiguracionSistema;
