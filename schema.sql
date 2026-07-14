IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Categorias] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(100) NOT NULL,
    [Descripcion] nvarchar(250) NULL,
    CONSTRAINT [PK_Categorias] PRIMARY KEY ([Id])
);

CREATE TABLE [Clientes] (
    [Id] int NOT NULL IDENTITY,
    [Nombres] nvarchar(150) NOT NULL,
    [DNI] nvarchar(8) NOT NULL,
    [Telefono] nvarchar(15) NULL,
    [Direccion] nvarchar(200) NULL,
    CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
);

CREATE TABLE [MetodosPago] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_MetodosPago] PRIMARY KEY ([Id])
);

CREATE TABLE [Vendedores] (
    [Id] int NOT NULL IDENTITY,
    [Nombres] nvarchar(150) NOT NULL,
    [Apellidos] nvarchar(150) NOT NULL,
    [DNI] nvarchar(8) NOT NULL,
    [Telefono] nvarchar(15) NULL,
    [Correo] nvarchar(150) NULL,
    [Estado] bit NOT NULL,
    CONSTRAINT [PK_Vendedores] PRIMARY KEY ([Id])
);

CREATE TABLE [Prendas] (
    [Id] int NOT NULL IDENTITY,
    [Nombre] nvarchar(150) NOT NULL,
    [Descripcion] nvarchar(300) NULL,
    [Talla] nvarchar(50) NOT NULL,
    [Color] nvarchar(50) NOT NULL,
    [Precio] decimal(10,2) NOT NULL,
    [Stock] int NOT NULL,
    [Imagen] nvarchar(max) NULL,
    [CategoriaId] int NOT NULL,
    CONSTRAINT [PK_Prendas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Prendas_Categorias_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [Categorias] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Ventas] (
    [Id] int NOT NULL IDENTITY,
    [Fecha] datetime2 NOT NULL,
    [ClienteId] int NOT NULL,
    [VendedorId] int NOT NULL,
    [MetodoPagoId] int NOT NULL,
    [Total] decimal(10,2) NOT NULL,
    CONSTRAINT [PK_Ventas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Ventas_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Ventas_MetodosPago_MetodoPagoId] FOREIGN KEY ([MetodoPagoId]) REFERENCES [MetodosPago] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Ventas_Vendedores_VendedorId] FOREIGN KEY ([VendedorId]) REFERENCES [Vendedores] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [DetalleVentas] (
    [Id] int NOT NULL IDENTITY,
    [VentaId] int NOT NULL,
    [PrendaId] int NOT NULL,
    [Cantidad] int NOT NULL,
    [Precio] decimal(10,2) NOT NULL,
    [Subtotal] decimal(10,2) NOT NULL,
    CONSTRAINT [PK_DetalleVentas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DetalleVentas_Prendas_PrendaId] FOREIGN KEY ([PrendaId]) REFERENCES [Prendas] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_DetalleVentas_Ventas_VentaId] FOREIGN KEY ([VentaId]) REFERENCES [Ventas] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_DetalleVentas_PrendaId] ON [DetalleVentas] ([PrendaId]);

CREATE INDEX [IX_DetalleVentas_VentaId] ON [DetalleVentas] ([VentaId]);

CREATE INDEX [IX_Prendas_CategoriaId] ON [Prendas] ([CategoriaId]);

CREATE INDEX [IX_Ventas_ClienteId] ON [Ventas] ([ClienteId]);

CREATE INDEX [IX_Ventas_MetodoPagoId] ON [Ventas] ([MetodoPagoId]);

CREATE INDEX [IX_Ventas_VendedorId] ON [Ventas] ([VendedorId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260521023404_InitialCreate', N'9.0.16');

ALTER TABLE [Prendas] ADD [ImagenUrl] nvarchar(max) NULL;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260521044718_AgregarImagenPrenda', N'9.0.16');

ALTER TABLE [DetalleVentas] DROP CONSTRAINT [FK_DetalleVentas_Prendas_PrendaId];

ALTER TABLE [DetalleVentas] DROP CONSTRAINT [FK_DetalleVentas_Ventas_VentaId];

ALTER TABLE [Prendas] DROP CONSTRAINT [FK_Prendas_Categorias_CategoriaId];

ALTER TABLE [Ventas] DROP CONSTRAINT [FK_Ventas_Clientes_ClienteId];

ALTER TABLE [Ventas] DROP CONSTRAINT [FK_Ventas_MetodosPago_MetodoPagoId];

ALTER TABLE [Ventas] DROP CONSTRAINT [FK_Ventas_Vendedores_VendedorId];

ALTER TABLE [Ventas] DROP CONSTRAINT [PK_Ventas];

ALTER TABLE [Vendedores] DROP CONSTRAINT [PK_Vendedores];

ALTER TABLE [MetodosPago] DROP CONSTRAINT [PK_MetodosPago];

ALTER TABLE [DetalleVentas] DROP CONSTRAINT [PK_DetalleVentas];

ALTER TABLE [Clientes] DROP CONSTRAINT [PK_Clientes];

EXEC sp_rename N'[Ventas]', N'Venta', 'OBJECT';

EXEC sp_rename N'[Vendedores]', N'Vendedor', 'OBJECT';

EXEC sp_rename N'[MetodosPago]', N'MetodoPago', 'OBJECT';

EXEC sp_rename N'[DetalleVentas]', N'DetalleVenta', 'OBJECT';

EXEC sp_rename N'[Clientes]', N'Cliente', 'OBJECT';

EXEC sp_rename N'[Venta].[IX_Ventas_VendedorId]', N'IX_Venta_VendedorId', 'INDEX';

EXEC sp_rename N'[Venta].[IX_Ventas_MetodoPagoId]', N'IX_Venta_MetodoPagoId', 'INDEX';

EXEC sp_rename N'[Venta].[IX_Ventas_ClienteId]', N'IX_Venta_ClienteId', 'INDEX';

EXEC sp_rename N'[DetalleVenta].[IX_DetalleVentas_VentaId]', N'IX_DetalleVenta_VentaId', 'INDEX';

EXEC sp_rename N'[DetalleVenta].[IX_DetalleVentas_PrendaId]', N'IX_DetalleVenta_PrendaId', 'INDEX';

DECLARE @var sysname;
SELECT @var = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Prendas]') AND [c].[name] = N'Nombre');
IF @var IS NOT NULL EXEC(N'ALTER TABLE [Prendas] DROP CONSTRAINT [' + @var + '];');
ALTER TABLE [Prendas] ALTER COLUMN [Nombre] nvarchar(100) NOT NULL;

ALTER TABLE [Venta] ADD CONSTRAINT [PK_Venta] PRIMARY KEY ([Id]);

ALTER TABLE [Vendedor] ADD CONSTRAINT [PK_Vendedor] PRIMARY KEY ([Id]);

ALTER TABLE [MetodoPago] ADD CONSTRAINT [PK_MetodoPago] PRIMARY KEY ([Id]);

ALTER TABLE [DetalleVenta] ADD CONSTRAINT [PK_DetalleVenta] PRIMARY KEY ([Id]);

ALTER TABLE [Cliente] ADD CONSTRAINT [PK_Cliente] PRIMARY KEY ([Id]);

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

ALTER TABLE [DetalleVenta] ADD CONSTRAINT [FK_DetalleVenta_Prendas_PrendaId] FOREIGN KEY ([PrendaId]) REFERENCES [Prendas] ([Id]) ON DELETE CASCADE;

ALTER TABLE [DetalleVenta] ADD CONSTRAINT [FK_DetalleVenta_Venta_VentaId] FOREIGN KEY ([VentaId]) REFERENCES [Venta] ([Id]) ON DELETE CASCADE;

ALTER TABLE [Prendas] ADD CONSTRAINT [FK_Prendas_Categorias_CategoriaId] FOREIGN KEY ([CategoriaId]) REFERENCES [Categorias] ([Id]) ON DELETE CASCADE;

ALTER TABLE [Venta] ADD CONSTRAINT [FK_Venta_Cliente_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Cliente] ([Id]) ON DELETE CASCADE;

ALTER TABLE [Venta] ADD CONSTRAINT [FK_Venta_MetodoPago_MetodoPagoId] FOREIGN KEY ([MetodoPagoId]) REFERENCES [MetodoPago] ([Id]) ON DELETE CASCADE;

ALTER TABLE [Venta] ADD CONSTRAINT [FK_Venta_Vendedor_VendedorId] FOREIGN KEY ([VendedorId]) REFERENCES [Vendedor] ([Id]) ON DELETE CASCADE;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260521074958_IdentityInicial', N'9.0.16');

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cliente]') AND [c].[name] = N'Nombres');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Cliente] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Cliente] DROP COLUMN [Nombres];

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cliente]') AND [c].[name] = N'Telefono');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Cliente] DROP CONSTRAINT [' + @var2 + '];');
UPDATE [Cliente] SET [Telefono] = N'' WHERE [Telefono] IS NULL;
ALTER TABLE [Cliente] ALTER COLUMN [Telefono] nvarchar(max) NOT NULL;
ALTER TABLE [Cliente] ADD DEFAULT N'' FOR [Telefono];

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cliente]') AND [c].[name] = N'Direccion');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Cliente] DROP CONSTRAINT [' + @var3 + '];');
UPDATE [Cliente] SET [Direccion] = N'' WHERE [Direccion] IS NULL;
ALTER TABLE [Cliente] ALTER COLUMN [Direccion] nvarchar(max) NOT NULL;
ALTER TABLE [Cliente] ADD DEFAULT N'' FOR [Direccion];

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Cliente]') AND [c].[name] = N'DNI');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Cliente] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [Cliente] ALTER COLUMN [DNI] nvarchar(max) NOT NULL;

ALTER TABLE [Cliente] ADD [NombreCompleto] nvarchar(max) NOT NULL DEFAULT N'';

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260526060916_FixFinal', N'9.0.16');

CREATE TABLE [Configuraciones] (
    [Id] int NOT NULL,
    [NombreTienda] nvarchar(100) NOT NULL,
    [RutaLogo] nvarchar(max) NULL,
    [RutaFavicon] nvarchar(max) NULL,
    [RutaImagenInstitucional] nvarchar(max) NULL,
    [ColorPrimario] nvarchar(7) NOT NULL,
    [ColorSecundario] nvarchar(7) NOT NULL,
    [ColorFondo] nvarchar(7) NOT NULL,
    [TemaOscuro] bit NOT NULL,
    [RutaFondoLogin] nvarchar(max) NULL,
    [RutaFondoDashboard] nvarchar(max) NULL,
    [NombrePropietario] nvarchar(max) NULL,
    [Telefono] nvarchar(20) NULL,
    [Correo] nvarchar(100) NULL,
    [Direccion] nvarchar(max) NULL,
    [Ciudad] nvarchar(max) NULL,
    [Pais] nvarchar(max) NULL,
    [CodigoPostal] nvarchar(max) NULL,
    [RUC] nvarchar(20) NULL,
    [Descripcion] nvarchar(max) NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NOT NULL,
    CONSTRAINT [PK_Configuraciones] PRIMARY KEY ([Id])
);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260704120000_AgregarConfiguracionSistema', N'9.0.16');

COMMIT;
GO

