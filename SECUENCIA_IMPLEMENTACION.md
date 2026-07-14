# SECUENCIA DE IMPLEMENTACIÓN - Paso a Paso
**Guía para ejecutar el Plan de Corrección Técnica**

---

## FASE 1: Preparación (2 horas)

### Paso 1.1: Renombrar FashionStore.Infrastructure1

**Objetivo**: Alinear nombre de carpeta con nombre de proyecto

**Acciones**:
1. Cerrar Visual Studio / Kiro
2. Navegar a `c:\Users\CRISTIAN\source\repos\FashionStoreSolution\`
3. Renombrar carpeta `FashionStore.Infrastructure1` → `FashionStore.Infrastructure`
4. Abrir nuevamente FashionStoreSolution.sln
5. Visual Studio debería detectar el cambio automáticamente

**Validación**:
```bash
dotnet build FashionStoreSolution.sln
# Esperado: ✓ Build completado (puede haber los 10 warnings de nullability)
```

---

### Paso 1.2: Consolidar ConfiguracionSistemaService

**Objetivo**: Mantener una sola versión de este servicio en Infrastructure

**Archivo A (Mantener)**:
- `FashionStore.Infrastructure\Services\ConfiguracionSistemaService.cs`

**Archivo B (Eliminar)**:
- `FashionStore.Web\Services\ConfiguracionSistemaService.cs`

**Acciones**:
1. Abrir `FashionStore.Web\Services\ConfiguracionSistemaService.cs`
2. Copiar la lógica si es diferente de Infrastructure
3. Comparar con `FashionStore.Infrastructure\Services\ConfiguracionSistemaService.cs`
4. Fusionar si es necesario
5. Eliminar `FashionStore.Web\Services\ConfiguracionSistemaService.cs`
6. Actualizar `FashionStore.Web\Program.cs`:

```csharp
// Cambiar de:
builder.Services.AddScoped(typeof(ConfiguracionSistemaService), ...);

// A:
builder.Services.AddScoped<IConfiguracionSistemaService>(
    sp => sp.GetRequiredService<IConfiguracionSistemaService>());
```

**Validación**:
```bash
dotnet build FashionStoreSolution.sln
# Esperado: ✓ Sin errores de referencia a ConfiguracionSistemaService
```

---

### Paso 1.3: Null-Safety en ClienteDTO

**Objetivo**: Eliminar warning CS8618

**Archivo**: `FashionStore.Domain\DTOs\ClienteDTO.cs`

**Cambio**:
```csharp
// ANTES
public class ClienteDTO
{
    public string NombreCompleto { get; set; }  // ⚠️ CS8618
}

// DESPUÉS
public class ClienteDTO
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
}
```

**Validación**:
```bash
dotnet build FashionStoreSolution.sln
# Esperado: ✓ 0 warnings CS8618 para ClienteDTO
```

---

### Paso 1.4: Null-Safety en Cliente (Entity)

**Objetivo**: Eliminar warning CS8618

**Archivo**: `FashionStore.Domain\Entities\Cliente.cs`

**Mismo cambio que ClienteDTO**: Inicializar propiedades

**Validación**:
```bash
dotnet build FashionStoreSolution.sln
# Esperado: ✓ 0 warnings CS8618 para Cliente
```

---

### Paso 1.5: Null Reference en Register.cshtml.cs

**Objetivo**: Eliminar warning CS8602

**Archivo**: `FashionStore.Web\Areas\Identity\Pages\Account\Register.cshtml.cs`

**Línea ~105**: 
```csharp
// ANTES
foreach (var error in result.Errors)

// DESPUÉS
if (result != null)
{
    foreach (var error in result.Errors)
}
```

**Validación**:
```bash
dotnet build FashionStoreSolution.sln
# Esperado: ✓ 0 warnings CS8602
```

---

### Paso 1.6: Validación Final Fase 1

```bash
cd c:\Users\CRISTIAN\source\repos\FashionStoreSolution

# Build limpio
dotnet build FashionStoreSolution.sln
# Esperado: ✓ Build correctamente

# Tests siguen pasando
dotnet test FashionStore.Tests\FashionStore.Tests.csproj --no-build
# Esperado: ✓ 286/286 tests pasando
```

**✅ Fase 1 Completada** si todo pasa

---

## FASE 2: Refactoring (4 horas)

### Paso 2.1: Refactorizar VentasController

**Objetivo**: Usar IUnitOfWork exclusivamente (no _context directo)

**Archivo**: `FashionStore.Web\Controllers\VentasController.cs`

**Patrón a Buscar y Reemplazar**:
```csharp
// ❌ BUSCAR TODAS ESTAS LÍNEAS:
await _context.Set<Vendedor>()...
await _context.Set<Cliente>()...
_context.DescuentosAutorizados...

// ✓ REEMPLAZAR POR:
await _unitOfWork.Vendedores.FindAsync(...)
await _unitOfWork.Clientes.GetAllAsync()
await _unitOfWork.Repository<DescuentoAutorizado>().GetAllAsync()
```

**Validación**:
```bash
dotnet build FashionStoreSolution.sln
# Esperado: ✓ Sin errores

dotnet test FashionStore.Tests\FashionStore.Tests.csproj --no-build
# Esperado: ✓ 286/286 tests pasando (si VentasController no tiene tests)
```

---

### Paso 2.2: Eliminar Propiedad Imagen Redundante

**Archivo**: `FashionStore.Domain\Entities\Prenda.cs`

**Cambio**:
```csharp
// ANTES
public byte[]? Imagen { get; set; }
public string? ImagenUrl { get; set; }

// DESPUÉS
public string? ImagenUrl { get; set; }
```

**Crear Migración**:
```bash
cd c:\Users\CRISTIAN\source\repos\FashionStoreSolution

dotnet ef migrations add RemoveImagenColumn `
    -p FashionStore.Infrastructure `
    -s FashionStore.Web

# Verificar que la migración se creó en:
# FashionStore.Infrastructure\Migrations\[Fecha]_RemoveImagenColumn.cs
```

**Validación**:
```bash
dotnet build FashionStoreSolution.sln
# Esperado: ✓ Sin errores

dotnet test FashionStore.Tests\FashionStore.Tests.csproj --no-build
# Esperado: ✓ 286/286 tests pasando
```

---

### Paso 2.3: Agregar Validaciones en API Endpoints

**Archivo**: `FashionStore.Web\Controllers\VentasController.cs`

**Endpoints a Validar**:
- POST `/api/registrar-venta`
- POST `/api/cliente-rapido`
- GET `/api/buscar/{nombre}`

**Ejemplo**:
```csharp
[HttpPost("/api/registrar-venta")]
public async Task<IActionResult> RegistrarVenta([FromBody] VentaCreateDto dto)
{
    // ✓ Validar DTO
    if (dto == null)
        return BadRequest("Datos de venta requeridos");
    
    // ✓ Validar modelo
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    // ... resto de lógica
}
```

**Validación**:
```bash
dotnet build FashionStoreSolution.sln && dotnet test
```

---

### Paso 2.4: Agregación de Transacciones (Opcional para Fase 2)

**Archivo**: `FashionStore.Web\Controllers\VentasController.cs` - método `Create (POST)`

**Si tienes tiempo**, agregar:
```csharp
using var transaction = await _unitOfWork.BeginTransactionAsync();
try
{
    // Lógica de venta
    await _unitOfWork.CommitAsync();
    await transaction.CommitAsync();
}
catch
{
    await transaction.RollbackAsync();
    throw;
}
```

---

## FASE 3: Completar Funcionalidad (6 horas)

### Paso 3.1 a 3.4: Controllers Incompletos

Completar según especificación en PLAN_CORRECCION_TECNICA.md

**Tiempo estimado**: 1-2 horas por controller

---

## FASE 4: Hardening (4 horas)

### Paso 4.1: Implementar Serilog

**Archivo**: `FashionStore.Web\Program.cs`

**Agregar**:
```csharp
using Serilog;

// Antes de: var app = builder.Build();
builder.Host.UseSerilog((context, services, config) =>
{
    config
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File(
            "logs/fashionstore-.txt",
            rollingInterval: RollingInterval.Day
        );
});
```

---

## VALIDACIÓN FINAL

```bash
cd c:\Users\CRISTIAN\source\repos\FashionStoreSolution

# 1. Build sin errores
dotnet build FashionStoreSolution.sln
# Esperado: ✓ Build completado correctamente

# 2. Tests pasando
dotnet test FashionStore.Tests\FashionStore.Tests.csproj --no-build
# Esperado: ✓ 286/286 tests pasando

# 3. Verificar estructura
dir FashionStore.Infrastructure  # Debería existir (no Infrastructure1)

# 4. Logs si se implementó Serilog
ls -r logs/  # Debería crear archivos de log
```

---

## Checklist de Entrega

- [ ] Fase 1: Infrastructure renombrado, ConfigService consolidado, nullability fixed
- [ ] Fase 2: VentasController refactorizado, Imagen eliminada, APIs validadas
- [ ] Fase 3: Todos los controllers implementados y funcionales
- [ ] Fase 4: Logging centralizado implementado
- [ ] Fase 5 (Opcional): Record types, Swagger, DNI validation
- [ ] ✓ dotnet build sin errores
- [ ] ✓ 286 tests pasando
- [ ] ✓ Documentación SSD NO modificada

---

**Documento**: SECUENCIA_IMPLEMENTACION.md
**Creado**: Julio 7, 2026
**Estado**: Listo para ejecución
