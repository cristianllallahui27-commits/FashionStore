# 🚀 GUÍA DE EJECUCIÓN - FASE A FASE

**Propósito**: Paso a paso para ejecutar cada fase del plan de corrección  
**Audiencia**: Desarrolladores  
**Tiempo Total**: 11-17 días  

---

## 📋 ÍNDICE DE FASES

1. [Fase 1: Preparación](#fase-1-preparación) (1-2 días)
2. [Fase 2: Arquitectura](#fase-2-arquitectura) (3-5 días)
3. [Fase 3: Validación](#fase-3-validación) (3-4 días)
4. [Fase 4: Datos](#fase-4-datos) (2-3 días)
5. [Fase 5: Pulido](#fase-5-pulido) (2-3 días)

---

# FASE 1: PREPARACIÓN

**Duración**: 1-2 días  
**Tareas**: 2  
**Prioridad**: 🔴 CRÍTICO  

## Objetivo
Consolidar estructura de infraestructura y documentar configuración de Supabase.

---

## Tarea 1.1: Consolidar Infrastructure (C1)

### Descripción
Eliminar duplicación: `Infrastructure1/` → `Infrastructure/`

### Pasos

#### Paso 1: Revisar qué está en cada carpeta
```bash
# Terminal
dir FashionStore.Infrastructure
dir FashionStore.Infrastructure1
```

**Esperado**:
- `Infrastructure/`: Vacía o parcial
- `Infrastructure1/`: Contiene archivos reales (Services/, Data/, Context/)

#### Paso 2: Copiar contenido Infrastructure1 → Infrastructure
```bash
# PowerShell
Copy-Item -Recurse "FashionStore.Infrastructure1/*" "FashionStore.Infrastructure/" -Force

# Resultado: Infrastructure/ tiene todo el contenido
```

#### Paso 3: Actualizar referencias en .csproj
Editar `FashionStore.Web/FashionStore.Web.csproj`:

```xml
<!-- ANTES -->
<ProjectReference Include="..\FashionStore.Infrastructure1\FashionStore.Infrastructure1.csproj" />

<!-- DESPUÉS -->
<ProjectReference Include="..\FashionStore.Infrastructure\FashionStore.Infrastructure.csproj" />
```

#### Paso 4: Actualizar namespaces en código
Buscar y reemplazar:
```bash
# Grep para encontrar referencias
grep -r "FashionStore.Infrastructure1" --include="*.cs" .

# Reemplazar todas con FashionStore.Infrastructure
```

O manualmente en archivos críticos:
- `FashionStore.Web/Program.cs`
- Controladores que importan desde Infrastructure1

#### Paso 5: Eliminar carpeta Infrastructure1
```bash
# Eliminar la carpeta duplicada
rmdir FashionStore.Infrastructure1 /s /q
```

#### Paso 6: Compilar y validar
```bash
dotnet clean
dotnet build -c Release
```

**Criterios de Aceptación**:
- [ ] `FashionStore.Infrastructure1/` eliminada
- [ ] Todas las referencias actualizadas
- [ ] Build exitoso (0 errores)
- [ ] Tests aún pasan (285/285)

---

## Tarea 1.2: Documentar Supabase Setup (C2)

### Descripción
Crear archivos `.env.example` y documentación para facilitar onboarding.

### Pasos

#### Paso 1: Crear `.env.example`
En raíz del proyecto (`FashionStoreSolution/`):

```bash
# Crear archivo
New-Item -ItemType File -Path ".env.example"
```

Contenido:
```
# Supabase Configuration
SUPABASE_PASSWORD=your_supabase_password_here

# Database Provider (PostgreSQL or SqlServer)
DATABASE_PROVIDER=PostgreSQL

# Logging
ASPNETCORE_ENVIRONMENT=Development
```

#### Paso 2: Crear `.gitignore` actualizado
Asegurar que `.env` NO se commitea:

```bash
# Verificar .gitignore
cat .gitignore

# Agregar si no existe:
echo ".env" >> .gitignore
echo "*.local" >> .gitignore
```

#### Paso 3: Crear `SETUP_SUPABASE.md`
Ubicación: `FashionStoreSolution/SETUP_SUPABASE.md`

```markdown
# Setup Supabase para FashionStore

## Paso 1: Obtener Contraseña
1. Ve a https://app.supabase.com
2. Selecciona proyecto "fashionstore"
3. Settings → Database → Password
4. Copia la contraseña

## Paso 2: Configurar Variable de Entorno
### Windows PowerShell (Como Admin)
\`\`\`powershell
[Environment]::SetEnvironmentVariable("SUPABASE_PASSWORD", "YOUR_PASSWORD", "User")
\`\`\`

### Linux/Mac
\`\`\`bash
export SUPABASE_PASSWORD="YOUR_PASSWORD"
\`\`\`

Luego **cierra y reabre** VS Code.

## Paso 3: Verificar Conexión
\`\`\`bash
cd FashionStore.Web
dotnet run
\`\`\`

Esperado: "Now listening on: http://localhost:5100"

## Paso 4: Fallback a SQL Server (Opcional)
Si necesitas SQL Server en lugar de Supabase:

Edita `appsettings.json`:
\`\`\`json
{
  "DatabaseProvider": "SqlServer"
}
\`\`\`

Presiona F5.
```

#### Paso 4: Actualizar README.md
Agregar sección en `FashionStoreSolution/README.md`:

```markdown
## Configuración Rápida

### Supabase (Recomendado)
1. Copia `.env.example` → `.env`
2. Edita `.env`: `SUPABASE_PASSWORD=tu_contraseña`
3. Presiona F5 en VS Code

Ver [SETUP_SUPABASE.md](./SETUP_SUPABASE.md) para detalles.

### SQL Server (Local)
Edita `appsettings.json`:
\`\`\`json
{
  "DatabaseProvider": "SqlServer"
}
\`\`\`
```

#### Paso 5: Validar documentación
- [ ] `.env.example` existe y es legible
- [ ] `SETUP_SUPABASE.md` completo
- [ ] `README.md` actualizado
- [ ] `.gitignore` protege `.env`

### Criterios de Aceptación
- [ ] `.env.example` con valores de ejemplo
- [ ] Documentación paso a paso
- [ ] `.gitignore` actualizado
- [ ] README.md actualizado

---

## Checklist Fin de Fase 1

```bash
✅ Tarea 1.1: Infrastructure consolidada
   - Infrastructure1 eliminada
   - Referencias actualizadas
   - Build exitoso

✅ Tarea 1.2: Documentación Supabase
   - .env.example creado
   - SETUP_SUPABASE.md creado
   - README.md actualizado

✅ VALIDACIÓN GLOBAL
   - dotnet build -c Release: 0 errores
   - dotnet test: 285/285 pasando
   - Commit fase 1
```

---

# FASE 2: ARQUITECTURA

**Duración**: 3-5 días  
**Tareas**: 5  
**Prioridad**: 🔴 ALTO  

## Objetivo
Arreglar arquitectura: RemoveDbContext directo, completar DTOs/mapeos, implementar carrito persistente.

---

## Tarea 2.1: Remover DbContext Directo de VentasController (A2)

### Ubicación
`FashionStore.Web/Controllers/VentasController.cs` línea 18-19

### Pasos

#### Paso 1: Revisar uso actual
```csharp
// ACTUAL (usar AMBOS)
private readonly FashionStoreDbContext _context;
private readonly IUnitOfWork _unitOfWork;

// BUSCAR dónde se usa _context
```

#### Paso 2: Reemplazar _context por _unitOfWork
Buscar todas las líneas con `_context` en VentasController:

```bash
grep -n "_context\." FashionStore.Web/Controllers/VentasController.cs
```

Ejemplo de cambios:
```csharp
// ANTES
var ventas = await _context.Ventas.ToListAsync();

// DESPUÉS
var ventas = await _unitOfWork.Ventas.GetAllAsync();

// ANTES
_context.Ventas.Add(nuevaVenta);
await _context.SaveChangesAsync();

// DESPUÉS
_unitOfWork.Ventas.Add(nuevaVenta);
await _unitOfWork.SaveChangesAsync();
```

#### Paso 3: Remover declaración de _context
```csharp
// ELIMINAR esta línea del constructor
private readonly FashionStoreDbContext _context;

// Actualizar constructor:
// ANTES
public VentasController(
    FashionStoreDbContext context,    // ❌ Eliminar
    IUnitOfWork unitOfWork,
    IConfiguracionSistemaService configService,
    ILogger<VentasController> logger)
{
    _context = context;              // ❌ Eliminar
    _unitOfWork = unitOfWork;
    ...
}

// DESPUÉS
public VentasController(
    IUnitOfWork unitOfWork,
    IConfiguracionSistemaService configService,
    ILogger<VentasController> logger)
{
    _unitOfWork = unitOfWork;
    ...
}
```

#### Paso 4: Compilar y testear
```bash
dotnet build
dotnet test --filter VentasController
```

**Criterios de Aceptación**:
- [ ] No hay referencias a `_context` en VentasController
- [ ] Todas las operaciones usan `_unitOfWork`
- [ ] Tests pasan
- [ ] Lógica no cambia (misma funcionalidad)

---

## Tarea 2.2: Mover DetalleVentaDTO a Carpeta Correcta (M1)

### Ubicación
`FashionStore.Domain/Interfaces/IServicioVentas.cs` → `FashionStore.Domain/DTOs/`

### Pasos

#### Paso 1: Crear archivo DTOs
```bash
# Crear DetalleVentaDTO.cs
New-Item -Path "FashionStore.Domain/DTOs/DetalleVentaDTO.cs"
```

#### Paso 2: Mover contenido
De `IServicioVentas.cs` línea 11-15:
```csharp
public class DetalleVentaDTO
{
    public int PrendaId { get; set; }
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }
}
```

Copiar a `FashionStore.Domain/DTOs/DetalleVentaDTO.cs`:
```csharp
namespace FashionStore.Domain.DTOs
{
    public class DetalleVentaDTO
    {
        public int PrendaId { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
    }
}
```

#### Paso 3: Eliminar de IServicioVentas.cs
Mantener solo la interfaz:
```csharp
namespace FashionStore.Domain.Interfaces
{
    public interface IServicioVentas
    {
        Task<int> RegistrarVenta(...);
        Task<bool> ActualizarInventario(...);
        Task<decimal> CalcularTotalVenta(List<DetalleVentaDTO> detalles);
        Task<(bool exito, string mensaje)> ValidarVenta(...);
    }
    
    // ❌ REMOVER DetalleVentaDTO de aquí
}
```

#### Paso 4: Actualizar imports
Todos los archivos que usen DetalleVentaDTO:
```csharp
// ANTES
using FashionStore.Domain.Interfaces;

// DESPUÉS
using FashionStore.Domain.DTOs;
```

Archivos afectados:
- `VentasController.cs`
- `ServicioVentas.cs`
- Cualquier otro que importe

#### Paso 5: Compilar
```bash
dotnet build
```

**Criterios de Aceptación**:
- [ ] `DetalleVentaDTO.cs` en `DTOs/`
- [ ] Eliminado de `IServicioVentas.cs`
- [ ] Imports actualizados
- [ ] Build exitoso

---

## Tarea 2.3: Crear VentaDTO y VentaDetalleDTO (M2)

### Pasos

#### Paso 1: Crear VentaDTO.cs
```bash
New-Item -Path "FashionStore.Domain/DTOs/VentaDTO.cs"
```

Contenido:
```csharp
using System;

namespace FashionStore.Domain.DTOs
{
    public class VentaDTO
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int VendedorId { get; set; }
        public int MetodoPagoId { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public decimal Descuento { get; set; }
        public string Estado { get; set; }
        
        // Relaciones
        public string ClienteNombre { get; set; }
        public string VendedorNombre { get; set; }
        public List<VentaDetalleDTO> Detalles { get; set; }
    }
}
```

#### Paso 2: Crear VentaDetalleDTO.cs
```bash
New-Item -Path "FashionStore.Domain/DTOs/VentaDetalleDTO.cs"
```

Contenido:
```csharp
namespace FashionStore.Domain.DTOs
{
    public class VentaDetalleDTO
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public int PrendaId { get; set; }
        public string PrendaNombre { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal Subtotal => Cantidad * Precio;
    }
}
```

#### Paso 3: Compilar
```bash
dotnet build
```

**Criterios de Aceptación**:
- [ ] `VentaDTO.cs` creado
- [ ] `VentaDetalleDTO.cs` creado
- [ ] Build exitoso

---

## Tarea 2.4: Agregar Mapeos en MappingProfile (M3)

### Ubicación
`FashionStore.Web/Mapping/MappingProfile.cs`

### Pasos

#### Paso 1: Editar MappingProfile.cs
Agregar mapeos:

```csharp
namespace FashionStore.Web.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ... Mapeos existentes ...
            
            // ✅ NUEVO: Venta
            CreateMap<Venta, VentaDTO>()
                .ForMember(dest => dest.ClienteNombre, 
                    opt => opt.MapFrom(src => src.Cliente != null ? src.Cliente.Nombre : null))
                .ForMember(dest => dest.VendedorNombre, 
                    opt => opt.MapFrom(src => src.Vendedor != null ? src.Vendedor.Nombre : null))
                .ReverseMap()
                .ForMember(dest => dest.Cliente, opt => opt.Ignore())
                .ForMember(dest => dest.Vendedor, opt => opt.Ignore());
                
            // ✅ NUEVO: VentaDetalle
            CreateMap<VentaDetalle, VentaDetalleDTO>()
                .ForMember(dest => dest.PrendaNombre, 
                    opt => opt.MapFrom(src => src.Prenda != null ? src.Prenda.Nombre : null))
                .ReverseMap()
                .ForMember(dest => dest.Prenda, opt => opt.Ignore());
                
            // ✅ NUEVO: ConfiguracionSistema
            CreateMap<ConfiguracionSistema, ConfiguracionSistemaDTO>().ReverseMap();
            
            // ✅ NUEVO: ConfiguracionAuditoria
            CreateMap<ConfiguracionAuditoria, ConfiguracionAuditoriaDTO>().ReverseMap();
        }
    }
}
```

#### Paso 2: Compilar y testear mapeos
```bash
dotnet build
dotnet test --filter Mapping
```

**Criterios de Aceptación**:
- [ ] Mapeos agregados sin errores
- [ ] AutoMapper resuelve tipos
- [ ] Build exitoso

---

## Tarea 2.5: Implementar Carrito en Session (C3)

### Ubicación
`FashionStore.Infrastructure/Services/CarritoService.cs`

### Pasos

#### Paso 1: Actualizar CarritoService
**ANTES** (solo en memoria):
```csharp
private List<CarritoItem> _items = new();
```

**DESPUÉS** (en Session):
```csharp
using Microsoft.AspNetCore.Http;

public class CarritoService : ICarritoService
{
    private const string CARRITO_KEY = "carrito_items";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CarritoService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private List<CarritoItem> ObtenerItems()
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session == null)
            return new List<CarritoItem>();

        var itemsJson = session.GetString(CARRITO_KEY);
        if (string.IsNullOrEmpty(itemsJson))
            return new List<CarritoItem>();

        return JsonSerializer.Deserialize<List<CarritoItem>>(itemsJson) ?? new();
    }

    private void GuardarItems(List<CarritoItem> items)
    {
        var session = _httpContextAccessor.HttpContext?.Session;
        if (session != null)
        {
            var itemsJson = JsonSerializer.Serialize(items);
            session.SetString(CARRITO_KEY, itemsJson);
        }
    }

    public void AgregarProducto(Prenda prenda, int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a 0");

        if (prenda.Stock < cantidad)
            throw new InvalidOperationException("No hay suficiente stock disponible");

        var items = ObtenerItems();
        var itemExistente = items.FirstOrDefault(i => i.PrendaId == prenda.Id);

        if (itemExistente != null)
        {
            if (prenda.Stock < itemExistente.Cantidad + cantidad)
                throw new InvalidOperationException("No hay suficiente stock disponible");
            itemExistente.Cantidad += cantidad;
        }
        else
        {
            items.Add(new CarritoItem
            {
                PrendaId = prenda.Id,
                Cantidad = cantidad,
                Precio = prenda.Precio,
                Nombre = prenda.Nombre
            });
        }

        GuardarItems(items);
    }

    // ... Métodos similares: ObtenerItems, LimpiarCarrito, etc.
}
```

#### Paso 2: Registrar Session en Program.cs
```csharp
// En Program.cs, antes de builder.Services.AddControllers():

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
```

#### Paso 3: Habilitar Session en middleware
```csharp
// En Program.cs, en la configuración de middleware:

app.UseSession();  // Después de UseRouting(), antes de MapControllerRoute
```

#### Paso 4: Compilar y testear
```bash
dotnet build
dotnet test --filter Carrito
```

**Criterios de Aceptación**:
- [ ] CarritoService usa Session
- [ ] Items persisten entre peticiones
- [ ] Session se limpia al logout
- [ ] Build exitoso
- [ ] Tests de carrito pasan

---

## Checklist Fin de Fase 2

```bash
✅ Tarea 2.1: DbContext removido
   - VentasController usa solo UnitOfWork
   - Build exitoso

✅ Tarea 2.2: DetalleVentaDTO movido
   - Está en DTOs/
   - Imports actualizados

✅ Tarea 2.3: DTOs Venta y VentaDetalle creados
   - VentaDTO.cs existe
   - VentaDetalleDTO.cs existe

✅ Tarea 2.4: Mapeos agregados
   - MappingProfile completo
   - Todas las entidades mapean

✅ Tarea 2.5: Carrito en Session
   - Items persisten
   - Session configurada

✅ VALIDACIÓN GLOBAL
   - dotnet build -c Release: 0 errores
   - dotnet test: 285/285 pasando
   - Commit fase 2
```

---

# FASE 3: VALIDACIÓN

**Duración**: 3-4 días  
**Tareas**: 4  
**Prioridad**: 🔴 CRÍTICO  

## Objetivo
Seguridad: validar accesos, descuentos, stock, roles.

### Tarea 3.1: Validar Vendedor-Usuario (A1)

[Similar structure con pasos detallados...]

### Tarea 3.2: Validar Descuentos (A3)

[Similar structure...]

### Tarea 3.3: Decidir Rol Admin (A4)

[Similar structure...]

### Tarea 3.4: Stock Transaccional (A5)

[Similar structure...]

---

# FASE 4 y 5

[Estructura similar...]

---

## 📊 TABLA DE PROGRESO

```
FASE    ESTADO    TAREA                          INICIO  FIN
1       [  ]      Preparación
        [ ]       1.1 Infrastructure
        [ ]       1.2 Documentación Supabase
2       [  ]      Arquitectura
        [ ]       2.1 DbContext
        [ ]       2.2 DetalleVentaDTO
        [ ]       2.3 DTOs Venta
        [ ]       2.4 Mapeos
        [ ]       2.5 Carrito Session
3       [  ]      Validación
        [ ]       3.1 Vendedor
        [ ]       3.2 Descuentos
        [ ]       3.3 Rol Admin
        [ ]       3.4 Stock
4       [  ]      Datos
        [ ]       4.1 Entidades
        [ ]       4.2 Migraciones
5       [  ]      Pulido
        [ ]       5.1 Permisos
        [ ]       5.2 UX
        [ ]       5.3 Organización
```

---

**Generado**: 7 Julio 2026  
**Versión**: 1.0.0 (Fase 1-2 Detallada, Fase 3-5 Template)  
**Estado**: 🟢 LISTO PARA COMENZAR CON FASE 1
