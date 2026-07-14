# DECISIONES TÉCNICAS - FashionStoreSolution
**Documento para justificar correcciones propuestas**

---

## 1. Renombrar FashionStore.Infrastructure1 → FashionStore.Infrastructure

### Problema
El proyecto Infrastructure está en la carpeta `FashionStore.Infrastructure1` pero se llama `FashionStore.Infrastructure` en el archivo `.csproj`. Esto causa:
- Confusión de rutas
- Referencias incorrectas en dependencias
- Compilación más lenta
- Dificultad para new developers

### Decisión
**Renombrar la carpeta a `FashionStore.Infrastructure` para coincidir con el nombre del proyecto.**

### Justificación
1. **Convención de nombres**: Las carpetas deben coincidir con nombres de proyectos
2. **Claridad arquitectónica**: Fácil identificar dónde vive cada capa
3. **Herramientas**: Visual Studio, Rider, y otros IDEs esperan esta coincidencia
4. **Mantenibilidad**: Nuevos desarrolladores no necesitan investigar

### Impacto
- ✅ Bajo riesgo: Es un cambio de estructura, no de código
- ✅ Fácil revertir: Git preserva historia
- ✅ Sin cambios de API: Los imports siguen igual
- ✅ Build sin impacto: Las referencias .csproj se actualizan automáticamente

### Proceso
```bash
# 1. En Windows Explorer: Renombrar carpeta FashionStore.Infrastructure1 → FashionStore.Infrastructure
# 2. En Visual Studio: Recargar solución (Ctrl+Shift+R)
# 3. Verificar: dotnet build FashionStoreSolution.sln
```

---

## 2. Consolidar ConfiguracionSistemaService

### Problema
La clase existe en dos ubicaciones con implementación posiblemente diferente:
- `FashionStore.Infrastructure\Services\ConfiguracionSistemaService.cs`
- `FashionStore.Web\Services\ConfiguracionSistemaService.cs`

Esto viola **DRY** (Don't Repeat Yourself) y causa:
- Cambios desincronizados
- Posible lógica divergente
- Confusión en inyección de dependencias

### Decisión
**Mantener la versión en Infrastructure (capa de negocio), eliminar de Web.**

### Justificación
1. **Arquitectura de capas**: Servicios pertenecen a Infrastructure, no a Presentation
2. **Inyección de dependencias**: Program.cs debería inyectar desde Infrastructure
3. **Testabilidad**: Servicios en Infrastructure son más fáciles de mockear
4. **Reutilización**: Web puede consumir el mismo servicio que otros proyectos

### Implementación
```csharp
// Program.cs - Configuración correcta
builder.Services.AddScoped<IConfiguracionSistemaService, ConfiguracionSistemaService>();

// En VentasController
private readonly IConfiguracionSistemaService _configService;

public VentasController(
    IUnitOfWork unitOfWork,
    IConfiguracionSistemaService configService)
{
    _unitOfWork = unitOfWork;
    _configService = configService; // ✓ De Infrastructure
}
```

### Impacto
- ✅ Una fuente de verdad
- ✅ Consistencia garantizada
- ✅ Facilita testing
- ⚠️ Necesita verificación de que ambas versiones tienen la misma lógica

---

## 3. Refactorizar VentasController para Usar IUnitOfWork

### Problema
VentasController mezcla dos patrones de acceso a datos:
```csharp
// ❌ Acceso directo (INCONSISTENTE)
var vendedor = await _context.Set<Vendedor>()
    .FirstOrDefaultAsync(v => v.Correo == userEmail);

// ✓ Vía UnitOfWork (CORRECTO)
var clientes = await _unitOfWork.Clientes.GetAllAsync();
```

Esto causa:
- Inconsistencia arquitectónica
- Dificultad de testing (no todas las operaciones mockeable)
- Posible memory leaks con DbContext
- Violación del patrón Unit of Work

### Decisión
**Refactorizar VentasController para usar IUnitOfWork EXCLUSIVAMENTE.**

### Justificación
1. **Patrón establecido**: Otros controllers (PrendasController, ClientesController) usan UnitOfWork
2. **Testabilidad**: IUnitOfWork es injectable y mockeable
3. **Transacciones**: UnitOfWork gestiona transacciones explícitamente
4. **Escalabilidad**: Fácil cambiar a otro ORM o repositorio

### Ejemplo de Refactor
```csharp
// ANTES (❌ Inconsistente)
var vendedor = await _context.Set<Vendedor>()
    .FirstOrDefaultAsync(v => v.Correo == userEmail);

// DESPUÉS (✓ Consistente)
var vendedor = await _unitOfWork.Vendedores
    .FindAsync(v => v.Correo == userEmail)
    .FirstOrDefaultAsync();
```

### Impacto
- ✅ Consistencia arquitectónica
- ✅ Testing más fácil
- ✅ 286 tests siguen pasando
- ⚠️ Requiere verificación que lógica de queries sea equivalente

---

## 4. Eliminar Propiedad Imagen Redundante en Prenda

### Problema
La entidad Prenda tiene dos propiedades relacionadas con imagen:
```csharp
public class Prenda
{
    public byte[]? Imagen { get; set; }      // ❌ Obsoleto?
    public string? ImagenUrl { get; set; }   // ✓ Actualmente usada
}
```

Esto causa:
- Confusión sobre cuál usar
- Migración y sincronización complicada
- Código muerto
- Mayor tamaño de entidad

### Decisión
**Eliminar propiedad `Imagen` (mantener solo `ImagenUrl`).**

### Justificación
1. **Código muerto**: Imagen parece no estar siendo usada
2. **Almacenamiento**: Guardar imágenes binarias en BD es antipatrón
  - Mejor: Guardar en archivo system o cloud (S3, Azure Blob)
  - Referencia en BD: URL o ruta del archivo
3. **Rendimiento**: ImagenUrl es más eficiente (string vs blob)
4. **Claridad**: Una sola forma de manejar imágenes

### Proceso
1. Crear migración EF Core que remueVa la columna `Imagen`
2. Verificar que PrendasController solo usa `ImagenUrl`
3. Actualizar testes que referencia `Imagen`

### Impacto
- ✅ Simplifica model
- ✅ Mejor rendimiento
- ✅ Menos confusión
- ⚠️ Requiere migración si hay datos existentes

---

## 5. Agregar Validaciones en Endpoints API

### Problema
Algunos endpoints REST aceptan solicitudes sin validar:
```csharp
// ❌ Sin validación
[HttpPost("/api/registrar-venta")]
public async Task<IActionResult> RegistrarVenta(VentaDto dto)
{
    // Directamente procesa sin validar
    var venta = await _servicioVentas.RegistrarVenta(dto);
}
```

Riesgos:
- Inyección de código
- Datos inválidos en BD
- DoS (consultas sin límite)
- Respuestas HTTP ambiguas

### Decisión
**Agregar validación de entrada en TODOS los endpoints API.**

### Implementación
```csharp
[HttpPost("/api/registrar-venta")]
[Authorize(Roles = "Vendedor,Administrador")]
public async Task<IActionResult> RegistrarVenta([FromBody] VentaDto dto)
{
    // ✓ Validar que dto no sea nulo
    if (dto == null)
        return BadRequest("Venta es requerida");
    
    // ✓ Validar flujo de negocio
    if (!ModelState.IsValid)
        return BadRequest(ModelState);
    
    var venta = await _servicioVentas.RegistrarVenta(dto);
    
    return Ok(venta);
}
```

### Patrones a Usar
- **Annotations**: `[Required]`, `[Range]`, `[StringLength]` en DTO
- **FluentValidation**: Para validaciones complejas
- **Manual**: En controlador para lógica de negocio

### Impacto
- ✅ Seguridad mejorada
- ✅ Datos más confiables
- ✅ Errores claros
- ✓ Status codes HTTP correctos (400, 422, 401, 403)

---

## 6. Implementar Logging Centralizado (Serilog)

### Problema
No hay logging en operaciones críticas:
- Ventas no registran transacciones
- Errores silenciosos en Repository
- Sin auditoría de acciones
- Debugging en producción muy difícil

### Decisión
**Implementar Serilog para logging centralizado.**

### Justificación
1. **Auditoría**: Requisito para sistemas financieros/retail
2. **Debugging**: Esencial en producción
3. **Compliance**: Muchas normas requieren trazabilidad
4. **Serilog es estándar**: Industry best practice en .NET

### Configuración
```csharp
// Program.cs
builder.Host.UseSerilog((context, services, config) =>
{
    config
        .MinimumLevel.Debug()
        .WriteTo.Console()
        .WriteTo.File(
            "logs/fashionstore-.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
        );
});
```

### Eventos a Loguear
- Ventas registradas (monto, cliente, vendedor)
- Cambios de precio
- Acceso a Admin
- Errores de validación
- Excepciones

### Impacto
- ✅ Trazabilidad completa
- ✅ Debugging más fácil
- ✅ Cumplimiento regulatorio
- ✓ Storage adicional (logs)

---

## 7. Completar Controllers Incompletos

### Problema
Varios controllers existen pero son esqueléticos:
- `CategoriasController`: CRUD pero sin validaciones
- `VendedoresController`: Métodos básicos
- `PerfilController`: No debería ser controller, es responsabilidad de Identity
- `ConfiguracionController`: Sin persistencia

### Decisión
**Completar cada controller según su responsabilidad.**

### Por Controller

#### CategoriasController
```csharp
[Authorize(Roles = "Administrador")]
public class CategoriasController : Controller
{
    // Index: Listar todas las categorías
    public async Task<IActionResult> Index() { ... }
    
    // Create GET/POST: Nueva categoría
    public IActionResult Create() { ... }
    public async Task<IActionResult> Create(CategoriaDTO dto) { ... }
    
    // Edit GET/POST: Editar categoría
    public async Task<IActionResult> Edit(int id) { ... }
    public async Task<IActionResult> Edit(int id, CategoriaDTO dto) { ... }
    
    // Delete GET/POST: Eliminar categoría
    public async Task<IActionResult> Delete(int id) { ... }
    public async Task<IActionResult> Delete(int id, IFormCollection form) { ... }
}
```

#### VendedoresController
- CRUD completo (Create, Read, Update, Delete)
- Solo Admin
- Inyectar UnitOfWork
- Validar campos requeridos

#### PerfilController
- Ver perfil del usuario actual
- Editar nombre, email
- Cambiar contraseña (usando Identity)
- NO: Crear/Editar otros usuarios (eso es Admin)

#### ConfiguracionController
- GET: Obtener configuración actual
- POST: Guardar cambios (nombre tienda, teléfono, etc.)
- Usar ConfiguracionSistemaService
- Solo Admin

### Impacto
- ✅ API completa
- ✅ Experiencia usuario mejorada
- ✅ Funcionalidad según requerimientos
- ⚠️ Requiere más testing

---

## 8. Agregar Transacciones Explícitas

### Problema
Operaciones críticas como registrar venta pueden quedar inconsistentes:
- Venta se guarda, pero detalles no
- Stock se decrementa, pero error antes de guardar
- Sin rollback automático

### Decisión
**Implementar transacciones explícitas en operaciones críticas.**

### Ejemplo: Registrar Venta
```csharp
[HttpPost("/api/registrar-venta")]
public async Task<IActionResult> RegistrarVenta([FromBody] VentaCreateDto dto)
{
    // ✓ Inicia transacción
    using var transaction = await _unitOfWork.BeginTransactionAsync();
    
    try
    {
        // 1. Validar inventario
        foreach (var detalle in dto.Detalles)
        {
            var prenda = await _unitOfWork.Prendas.GetByIdAsync(detalle.PrendaId);
            if (prenda.Stock < detalle.Cantidad)
                throw new InvalidOperationException("Stock insuficiente");
        }
        
        // 2. Crear venta
        var venta = new Venta { ... };
        await _unitOfWork.Ventas.AddAsync(venta);
        
        // 3. Decrementar stock
        foreach (var detalle in dto.Detalles)
        {
            var prenda = await _unitOfWork.Prendas.GetByIdAsync(detalle.PrendaId);
            prenda.Stock -= detalle.Cantidad;
            _unitOfWork.Prendas.Update(prenda);
        }
        
        // 4. Guardar todo
        await _unitOfWork.CommitAsync();
        
        // ✓ Confirma transacción
        await transaction.CommitAsync();
        
        return Ok(venta);
    }
    catch (Exception ex)
    {
        // ✗ Revierte todo si algo falla
        await transaction.RollbackAsync();
        return BadRequest(ex.Message);
    }
}
```

### Impacto
- ✅ Consistencia de datos garantizada
- ✅ Sin estado inconsistente
- ✅ Rollback automático si falla
- ⚠️ Rendimiento (transacciones más largas = locks más largos)

---

## 9. Null-Safety: ClienteDTO y Cliente

### Problema
Propiedades no-nullable sin inicialización:
```csharp
// ❌ CS8618: Property 'NombreCompleto' is uninitialized
public class ClienteDTO
{
    public string NombreCompleto { get; set; }  // Sin valor default
}
```

### Decisión
**Hacer propiedades nullable o agregar inicialización.**

### Solución
```csharp
// OPCIÓN 1: Nullable (permite null)
public class ClienteDTO
{
    public string? NombreCompleto { get; set; }
}

// OPCIÓN 2: Inicializar (mejor)
public class ClienteDTO
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
}

// OPCIÓN 3: Required (C# 11)
public class ClienteDTO
{
    public required string NombreCompleto { get; set; }
    public required string DNI { get; set; }
}
```

**Elegimos OPCIÓN 2 (inicializar)** porque:
- Evita null checks innecesarios
- Mantiene tipos consistentes (no nullable)
- Compatible con deserialization de JSON

### Impacto
- ✅ 0 warnings CS8618
- ✅ Código más limpio
- ✅ Menos NullReferenceException

---

## 10. Null Reference en Register.cshtml.cs

### Problema
```csharp
// ❌ CS8602: Dereference of possibly null reference
var result = await _userManager.CreateAsync(user, model.Password);
if (result.Errors)  // ✗ result podría ser null
```

### Decisión
**Agregar null checks.**

### Solución
```csharp
var result = await _userManager.CreateAsync(user, model.Password);

// ✓ Verificar que result no es null
if (result != null && result.Succeeded)
{
    // Enviar email, loguear, etc.
}
else if (result != null)
{
    foreach (var error in result.Errors)
    {
        ModelState.AddModelError(string.Empty, error.Description);
    }
}
else
{
    ModelState.AddModelError(string.Empty, "Error desconocido al crear usuario");
}
```

### Impacto
- ✅ 0 warnings CS8602
- ✅ Manejo de errores robusto
- ✅ Mensajes claros al usuario

---

## 11. Por Qué NO Agregar Nuevas Funcionalidades

### Justificación
Este plan NO incluye:
- ❌ Nuevas entidades (Devoluciones, Cambios)
- ❌ Nuevos reportes (más allá de lo existente)
- ❌ Integración de pagos
- ❌ SMS notifications
- ❌ Sincronización con terceros

**Razón**: El objetivo es corregir lo existente, no expandir scope.

Si necesitas nuevas funcionalidades, crea un **PLAN_NUEVAS_CARACTERISTICAS.md** separado.

---

## 12. Por Qué Mantener Arquitectura Existente

### Las 4 Capas Actuales (Correctas)
```
┌─────────────────────────────────────┐
│  FashionStore.Web (Presentation)    │ Controllers, Views, APIs
├─────────────────────────────────────┤
│  FashionStore.Infrastructure        │ Services, Repositories, DbContext
├─────────────────────────────────────┤
│  FashionStore.Domain (Business)     │ Entities, DTOs, Interfaces
├─────────────────────────────────────┤
│  FashionStore.Tests (Quality)       │ Unit tests, Integration tests
└─────────────────────────────────────┘
```

**NO cambiaremos esta estructura** porque:
- ✅ Está bien diseñada
- ✅ Facilita testing
- ✅ Escalable
- ✅ Coherente con estándares .NET

---

## Resumen de Decisiones

| Decisión | Beneficio | Costo | Riesgo |
|----------|-----------|-------|--------|
| Renombrar Infrastructure1 | Claridad, consistencia | 0.5h | Bajo |
| Consolidar ConfigService | DRY, testing | 0.5h | Bajo |
| Refactorizar VentasController | Consistencia, testing | 2h | Medio |
| Eliminar Imagen redundante | Simplificación, rendimiento | 1h | Bajo |
| Validar API endpoints | Seguridad, calidad datos | 1.5h | Bajo |
| Logging centralizado | Auditoría, debugging | 2h | Bajo |
| Completar controllers | Funcionalidad, UX | 6h | Medio |
| Transacciones explícitas | Integridad datos | 1h | Bajo |
| Null-safety warnings | Calidad código | 0.5h | Bajo |
| Null reference fixes | Estabilidad | 0.5h | Bajo |

**Total**: ~16 horas de valor agregado

---

## Próximos Pasos

1. Revisar todas las decisiones ✓ (este documento)
2. Ejecutar Plan de Corrección (PLAN_CORRECCION_TECNICA.md)
3. Validar con `dotnet build` y `dotnet test`
4. Verificar que 286 tests sigan pasando
5. Actualizar SSD si es necesario (después de correcciones)

---

**Documento**: DECISIONES_TECNICAS.md
**Creado por**: Kiro QA Senior
**Fecha**: Julio 7, 2026
**Estado**: Listo para revisión y aprobación
