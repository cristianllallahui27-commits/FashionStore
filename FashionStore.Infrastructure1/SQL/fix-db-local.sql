IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Configuraciones')
BEGIN
	CREATE TABLE Configuraciones (
		Id INT PRIMARY KEY IDENTITY(1,1),
		NombreTienda NVARCHAR(100),
		RUC NVARCHAR(20),
		Direccion NVARCHAR(255),
		Telefono NVARCHAR(20),
		Correo NVARCHAR(100),
		LogoUrl NVARCHAR(MAX),
		FaviconUrl NVARCHAR(MAX),
		RutaImagenLogin NVARCHAR(MAX),
		RutaBanner NVARCHAR(MAX),
		RutaFondoMenu NVARCHAR(MAX),
		ColorMenu NVARCHAR(20),
		ColorBotones NVARCHAR(20),
		ColorDashboard NVARCHAR(20),
		TemaSeleccionado NVARCHAR(50),
		FacebookUrl NVARCHAR(MAX),
		InstagramUrl NVARCHAR(MAX),
		TwitterUrl NVARCHAR(MAX),
		LinkedInUrl NVARCHAR(MAX),
		TikTokUrl NVARCHAR(MAX),
		TextoPiePagina NVARCHAR(MAX)
	);
	INSERT INTO Configuraciones (NombreTienda, TemaSeleccionado, ColorMenu, ColorBotones, ColorDashboard) 
	VALUES ('Fashion Store', 'light', '#343a40', '#007bff', '#f4f6f9');
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionesAuditoria')
BEGIN
	CREATE TABLE ConfiguracionesAuditoria (
		Id INT PRIMARY KEY IDENTITY(1,1),
		UsuarioId NVARCHAR(450),
		NombreUsuario NVARCHAR(256),
		PropiedadModificada NVARCHAR(100),
		ValorAnterior NVARCHAR(MAX),
		ValorNuevo NVARCHAR(MAX),
		FechaCambio DATETIME2 DEFAULT GETDATE(),
		Descripcion NVARCHAR(MAX)
	);
END
GO
