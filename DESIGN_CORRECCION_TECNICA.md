# DESIGN_CORRECCION_TECNICA.md
# Diseño Técnico de Corrección — FashionStoreSolution

**Metodología:** Specs Driven Development (SSD)  
**Versión:** 1.0  
**Fecha:** 7 de julio de 2026  
**Basado en:** SPEC_REQUISITOS_CORRECCION.md v2.0 + código fuente real

---

## 1. ARQUITECTURA ACTUAL

### 1.1 Diagrama de Capas

```
┌──────────────────────────────────────────────────────────────┐
│  FashionStore.Web  (Presentación)                           │
│  ├─ Controllers/  HomeController, VentasController,          │
│  │                PrendasController, ClientesController,     │
│  │                CategoriasController                       │
│  ├─ Views/        Home, Ventas, Prendas, Clientes, Shared    │
│  ├─ ViewModels/   DashboardViewModel                         │
│  ├─ Mapping/      MappingProfile (AutoMapper)                │
│  ├─ Services/     CarritoService, EmailSender                │
│  └─ Program.cs    Pipeline, DI, Middleware                   │
└───────────────────────┬──────────────────────────────────────┘
                        │ Referencias
┌───────────────────────▼──────────────────────────────────────┐
│  FashionStore.Domain  (Dominio / Core)                      │
│  ├─ Entities/     Prenda, Cliente, Venta, DetalleVenta,      │
│  │                Categoria, Vendedor, MetodoPago, etc.      │
│  ├─ DTOs/         PrendaDTO, ClienteDTO, CategoriaDTO,       │
│  │                VendedorDTO, MetodoPagoDTO                 │
│  └─ Interfaces/   IUnitOfWork, IGenericRepository<T>,        │
│                   IServicioVentas, ICarritoService           │
└───────────────────────┬──────────────────────────────────────┘
                        │ Implementa
┌───────────────────────▼──────────────────────────────────────┐
│  FashionStore.Infrastructure1  (Datos)                      │
│  ├─ Context/      FashionStoreDbContext                       │
│  ├─ Repositories/ GenericRepository<T>                       │
│  ├─ UnitOfWork/   UnitOfWork                                 │
│  ├─ Services/     ServicioVentas, ConfiguracionSistemaService│
│  ├─ Data/         DbInitializer                              │
│  └─ Migrations/   (EF Core migrations)                      │
└──────────────────────────────────────────────────────────────┘
```

### 1.2 Estado Actual del Build

```
dotnet build → Compilación correcta
  FashionStore.Web:    1 advertencia (CS8602 en _Layout.cshtml:135)
  FashionStore.Tests: 15 advertencias (CS8625 + MSTEST0032 — no críticas)
  0 errores de compilación

dotnet test → 290/290 pasan
```

---

## 2. PROBLEMAS DE DISEÑO DETECTADOS

### 2.1 Tabla Consolidada

| ID | Área | Problema | Severidad | Estado |
|----|------|----------|-----------|--------|
| D-01 | Ventas/Index.cshtml | La vista llama a `/api/clientes` y `/api/prendas` que no existen | Crítico | Pendiente |
| D-02 | Ventas/Index.cshtml | El select de `metodoPago` tiene valores hardcoded ("efectivo", "tarjeta") en lugar de IDs numéricos | Crítico | Pendiente |
| D-03 | Ventas/Index.cshtml | El submit del formulario no llama a `POST /api/registrar-venta` — solo llama a `showSuccess` | Crítico | Pendiente |
| D-04 | ServicioVentas.cs | `RegistrarVenta()` no usa transacción explícita — múltiples `SaveChangesAsync()` sin atomicidad | Crítico | Pendiente |
| D-05 | ServicioVentas.cs | `ActualizarInventario()` hace su propio `SaveChangesAsync()` dentro del loop de `RegistrarVenta()` | Crítico | Pendiente |
| D-06 | ServicioVentas.cs | `IServicioVentas` existe y está registrado en DI, pero `VentasController` lo ignora y duplica la lógica | Alto | Pendiente |
| D-07 | Program.cs | `MapRazorPages()` y `MapControllerRoute()` se llaman antes de `DbInitializer.Initialize()` — orden correcto pero mejora posible | Bajo | Ya corregido |
| D-08 | Tests | CS8625: literales null asignados a tipos non-nullable en tests | Bajo | Pendiente |
| D-09 | Tests | MSTEST0032: assertions always true en `VentaEntityTests` y `RolEntityTests` | Bajo | Pendiente |
| D-10 | Ventas/Index.cshtml | No llama a `GET /api/vendedores` — el formulario no incluye campo de vendedor | Medio | Pendiente |

---

## 3. DISEÑO PROPUESTO

### 3.1 Principio de Diseño

> **No duplicar la lógica de negocio.** `IServicioVentas` ya existe, está registrado en DI y tiene `ValidarVenta`, `RegistrarVenta` y `ActualizarInventario`. `VentasController` debe delegarle en lugar de reimplementar la misma lógica.

### 3.2 Diagrama de Flujo — Registro de Venta

```
[Browser: POST /api/registrar-venta]
         │
         ▼
[VentasController.ApiRegistrarVenta()]
         │
         ├─ Valida request != null && Detalles.Any()
         │
         ▼
[IServicioVentas.ValidarVenta()]  ←── usa IUnitOfWork.*.GetByIdAsync()
         │
         ├─ ¿Exito? NO ──────────────────► return { exito: false, mensaje }
         │
         ▼ SÍ
[BeginTransactionAsync()]  ←────── _context.Database (excepción permitida)
         │
[IServicioVentas.RegistrarVenta()]
         │  ├─ Crea Venta → IUnitOfWork.Ventas.AddAsync() + CommitAsync()
         │  ├─ Para cada detalle:
         │  │    ├─ Crea DetalleVenta → IUnitOfWork.DetalleVentas.AddAsync()
         │  │    └─ IServicioVentas.ActualizarInventario(prendaId, cantidad)
         │  └─ CommitAsync() final
         │
[transaction.CommitAsync()]
         │
         ▼
return { exito: true, ventaId: N }

[Si error en cualquier paso]
         │
[transaction.RollbackAsync()]
         │
return { exito: false, mensaje: ex.Message }
```

---

### 3.3 Diagrama de Flujo — Autenticación y Middleware

```
[Request HTTP]
      │
      ▼
app.UseRouting()           ← Resuelve la ruta
      │
      ▼
app.UseAuthentication()    ← Llena context.User con claims del cookie
      │
      ▼
app.UseAuthorization()     ← Evalúa [Authorize], roles, etc.
      │
      ▼
app.Use(middleware)        ← Ahora context.User.Identity.IsAuthenticated es válido
      │
      ├─ Path == "/" → No autenticado → Redirect /Identity/Account/Login
      ├─ Path == "/" → Autenticado    → Redirect /Home/Index
      └─ Cualquier otra ruta          → next()
      │
      ▼
app.MapControllerRoute()   ← Enruta a controladores MVC
app.MapRazorPages()        ← Enruta a Identity UI
```

### 3.4 Diagrama de Flujo — Inicialización de Roles

```
app.MapRazorPages()
      │
      ▼
await DbInitializer.Initialize(app)
      │
      ├─ Obtiene RoleManager<IdentityRole> del DI
      ├─ Para "Administrador": RoleExistsAsync() → NO → CreateAsync()
      ├─ Para "Vendedor":     RoleExistsAsync() → NO → CreateAsync()
      └─ Si error → logger.LogError() — NO lanza excepción
      │
      ▼
app.Run()
```

---

## 4. CAMBIOS POR ARCHIVO

### 4.1 `ServicioVentas.cs` — Agregar Transacción

**Problema actual:** múltiples `SaveChangesAsync()` sin transacción  
**Cambio:** envolver toda la operación en `BeginTransactionAsync`

```
// ANTES (sin transacción):
_context.Ventas.Add(venta);
await _context.SaveChangesAsync();              // Commit 1
// ... loop de detalles
await ActualizarInventario(prendaId, cantidad); // SaveChangesAsync interno
await _context.SaveChangesAsync();              // Commit 2

// DESPUÉS (con transacción):
using var transaction = await _context.Database.BeginTransactionAsync();
try {
    _context.Ventas.Add(venta);
    await _context.SaveChangesAsync();
    foreach (detalle) { ... }
    await _context.SaveChangesAsync();
    await transaction.CommitAsync();
} catch {
    await transaction.RollbackAsync();
    throw;
}
```

---

### 4.2 `VentasController.cs` — Delegar a IServicioVentas

**Problema actual:** `ValidarVentaInterno()` y `RegistrarVentaInterno()` duplican la lógica de `ServicioVentas`  
**Cambio:** inyectar `IServicioVentas` y usarlo

```
// ANTES:
private async Task<int> RegistrarVentaInterno(...) { /* lógica duplicada */ }
private async Task<(bool, string)> ValidarVentaInterno(...) { /* lógica duplicada */ }

// DESPUÉS:
private readonly IServicioVentas _servicioVentas;
// constructor: inyectar IServicioVentas
// ApiRegistrarVenta → var ventaId = await _servicioVentas.RegistrarVenta(...)
// ApiValidarVenta   → var (exito, msg) = await _servicioVentas.ValidarVenta(...)
```

---

### 4.3 `Ventas/Index.cshtml` — Endpoints reales

**Problema actual:** JS llama a `/api/clientes` y `/api/prendas` que no existen; submit no envía datos reales

**Cambio en JavaScript:**
```
// ANTES:
fetch('/api/clientes')       → NO EXISTE
fetch('/api/prendas')        → NO EXISTE
submit → solo showSuccess()  → NO REGISTRA

// DESPUÉS:
fetch('/api/clientes-disponibles')    → VentasController.ApiClientesDisponibles()
fetch('/api/productos-disponibles')   → VentasController.ApiProductosDisponibles() [YA EXISTE]
fetch('/api/vendedores-disponibles')  → VentasController.ApiVendedoresDisponibles()
fetch('/api/metodos-pago')            → VentasController.ApiMetodosPago()
submit → fetch POST /api/registrar-venta con payload real
```

**Cambio en select de métodoPago:**
```
// ANTES (hardcoded, no son IDs):
<option value="efectivo">Efectivo</option>
<option value="tarjeta">Tarjeta de Crédito</option>

// DESPUÉS (cargado dinámicamente desde BD):
<select id="metodoPago">  → popolado por cargarMetodosPago()
// Cada opción: <option value="{id}">{nombre}</option>
```

---

### 4.4 `VentasController.cs` — Agregar endpoints faltantes

**Endpoints nuevos a diseñar:**

| Método | Ruta | Descripción | Autorización |
|--------|------|-------------|-------------|
| GET | `/api/clientes-disponibles` | Lista todos los clientes (Id + NombreCompleto) | `[Authorize]` |
| GET | `/api/vendedores-disponibles` | Lista vendedores activos (Id + Nombre) | `[Authorize]` |
| GET | `/api/metodos-pago` | Lista todos los métodos de pago (Id + Nombre) | `[Authorize]` |

---

## 5. CONTRATOS ESPERADOS — ViewModels y DTOs

### 5.1 `DashboardViewModel` — Contrato completo

```csharp
// Todas las propiedades que Home/Index.cshtml consume:
public class DashboardViewModel
{
    // KPI Cards (usados como @Model.Total*)
    public int     TotalPrendas      { get; set; }
    public int     TotalCategorias   { get; set; }
    public int     TotalClientes     { get; set; }
    public int     TotalVentas       { get; set; }
    public int     TotalUsuarios     { get; set; }
    public int     TotalStock        { get; set; }
    public decimal TotalIngresos     { get; set; }

    // Charts (JSON serializado para JS)
    public string SalesChartLabelsJson       { get; set; } = string.Empty;
    public string SalesChartDataJson         { get; set; } = string.Empty;
    public string WeeklySalesLabelsJson      { get; set; } = string.Empty;
    public string WeeklySalesDataJson        { get; set; } = string.Empty;
    public string PrendasByCategoryLabelsJson { get; set; } = string.Empty;
    public string PrendasByCategoryDataJson  { get; set; } = string.Empty;
    public string TopProductsLabelsJson      { get; set; } = string.Empty;
    public string TopProductsDataJson        { get; set; } = string.Empty;
    public string RevenueByMethodLabelsJson  { get; set; } = string.Empty;
    public string RevenueByMethodDataJson    { get; set; } = string.Empty;

    // Sección "Estado de Stock" — usada como Model?.PrendasAgotandose?.Count > 0
    // y foreach (var prenda in Model.PrendasAgotandose.Take(4))
    // → requiere prenda.Nombre, prenda.Stock, prenda.Id
    public IList<PrendaDTO>? PrendasAgotandose { get; set; }

    // Sección "Prendas Destacadas" — usada como Model?.Prendas?.Count > 0
    // y foreach (var prenda in Model.Prendas.Take(6))
    // → requiere prenda.ImagenUrl, prenda.Nombre, prenda.Descripcion,
    //   prenda.Color, prenda.Talla, prenda.Precio, prenda.Stock, prenda.Id
    public IList<PrendaDTO>? Prendas { get; set; }

    // Listas de objetos anónimos (compatibilidad con otras vistas/partials)
    public IEnumerable<object>? RecentSales      { get; set; }
    public IEnumerable<object>? RecentClients    { get; set; }
    public IEnumerable<object>? TopSellingProducts { get; set; }
    public IEnumerable<object>? LowStockProducts { get; set; }
}
```

---

### 5.2 `PrendaDTO` — Contrato completo

```csharp
public class PrendaDTO
{
    public int     Id           { get; set; }
    public string? Nombre       { get; set; }  // [Required, StringLength(150)]
    public string? Descripcion  { get; set; }  // [StringLength(300)]
    public string? Talla        { get; set; }  // [Required]
    public string? Color        { get; set; }  // [Required]
    public decimal Precio       { get; set; }  // [Range(1, 99999)]
    public int     Stock        { get; set; }  // [Range(0, 10000)]
    public string? Imagen       { get; set; }
    public string? ImagenUrl    { get; set; }
    public int     CategoriaId  { get; set; }
    public string? CategoriaNombre { get; set; }  // mapeado desde Categoria.Nombre

    // Propiedad calculada — compatibilidad con vistas que usan @prenda.Categoria?.Nombre
    public CategoriaInfo? Categoria => CategoriaNombre != null
        ? new CategoriaInfo { Nombre = CategoriaNombre }
        : null;
}

public class CategoriaInfo
{
    public string? Nombre { get; set; }
}
```

---

### 5.3 `ClienteDTO` — Contrato completo

```csharp
public class ClienteDTO
{
    public int    Id            { get; set; }
    public string NombreCompleto { get; set; }  // [Required]
    public string DNI           { get; set; }   // [Required, StringLength(8)]
    public string Telefono      { get; set; }
    public string Direccion     { get; set; }

    // Alias para vistas que usan .Nombre
    public string Nombre => NombreCompleto;
    // Alias para vistas que usan .Email (columna identificación)
    public string Email  => DNI;
}
```

---

### 5.4 Payload `RegistrarVentaRequest` — Contrato de la API

```csharp
// Request (enviado desde el browser via fetch POST /api/registrar-venta)
{
    "clienteId":    1,
    "vendedorId":   2,
    "metodoPagoId": 3,
    "detalles": [
        { "prendaId": 10, "cantidad": 2, "precio": 45.90 },
        { "prendaId": 15, "cantidad": 1, "precio": 120.00 }
    ]
}

// Response exitosa
{ "exito": true, "mensaje": "Venta registrada exitosamente", "ventaId": 42 }

// Response fallida
{ "exito": false, "mensaje": "Stock insuficiente para Blusa Roja. Disponible: 1, Solicitado: 2" }
```

---

## 6. ENDPOINTS REQUERIDOS

### 6.1 Endpoints Existentes (ya implementados)

| Método | Ruta | Controlador | Método | Autorización |
|--------|------|-------------|--------|-------------|
| GET | `/api/productos-disponibles` | VentasController | ApiProductosDisponibles | [AllowAnonymous] |
| GET | `/api/buscar/{nombre}` | VentasController | ApiBuscar | [AllowAnonymous] |
| POST | `/api/registrar-venta` | VentasController | ApiRegistrarVenta | [Authorize] |
| POST | `/api/validar-venta` | VentasController | ApiValidarVenta | [Authorize] |
| GET | `/api/productos-agotandose` | VentasController | ApiProductosAgotandose | [Authorize] |

### 6.2 Endpoints Faltantes (a implementar)

| Método | Ruta | Controlador | Descripción | Respuesta esperada |
|--------|------|-------------|-------------|-------------------|
| GET | `/api/clientes-disponibles` | VentasController | Lista clientes para el selector del modal | `[{ id, nombreCompleto }]` |
| GET | `/api/vendedores-disponibles` | VentasController | Lista vendedores activos para el selector | `[{ id, nombreCompleto }]` |
| GET | `/api/metodos-pago` | VentasController | Lista métodos de pago de BD | `[{ id, nombre }]` |

### 6.3 Diseño de Respuestas JSON

```
GET /api/clientes-disponibles
→ { exito: true, datos: [ { id: 1, nombreCompleto: "Juan Pérez" }, ... ] }

GET /api/vendedores-disponibles
→ { exito: true, datos: [ { id: 1, nombreCompleto: "Ana García", dni: "12345678" }, ... ] }

GET /api/metodos-pago
→ { exito: true, datos: [ { id: 1, nombre: "Efectivo" }, { id: 2, nombre: "Tarjeta" }, ... ] }
```

---

## 7. REGLAS DE SEGURIDAD

### 7.1 Protección de Controladores

| Controlador | `[Authorize]` | Roles adicionales | Estado |
|-------------|--------------|-----------------|--------|
| HomeController | No (solo redirige si no auth en Index) | — | Pendiente agregar `[Authorize]` |
| VentasController | ✅ clase completa | — | ✅ Correcto |
| PrendasController | ✅ clase completa | — | ✅ Corregido |
| ClientesController | ✅ clase completa | — | ✅ Correcto |
| CategoriasController | ⚠️ Verificar | — | Pendiente verificar |
| ConfiguracionController | ⚠️ Debe ser `[Authorize(Roles="Administrador")]` | Solo Administrador | Pendiente |

### 7.2 Orden Obligatorio del Pipeline

```
OBLIGATORIO (no cambiar el orden):
1. app.UseRouting()
2. app.UseAuthentication()   ← ANTES del middleware personalizado
3. app.UseAuthorization()    ← ANTES del middleware personalizado
4. app.Use(middleware)       ← middleware de redirección / con User válido
5. app.MapControllerRoute()
6. app.MapRazorPages()
7. await DbInitializer.Initialize(app)  ← antes de app.Run()
8. app.Run()
```

### 7.3 Reglas Anti-Forgery

Todos los formularios POST deben tener:
- `[ValidateAntiForgeryToken]` en el action del controlador
- `@Html.AntiForgeryToken()` o `asp-antiforgery="true"` en el formulario Razor
- Los endpoints API (`/api/*`) quedan exentos porque reciben JSON, no formularios

### 7.4 Endpoints AllowAnonymous

Los siguientes endpoints son públicos por diseño (catálogo para el módulo de ventas):
- `GET /api/productos-disponibles`
- `GET /api/buscar/{nombre}`

Todos los demás `/api/*` deben requerir autenticación.

---

## 8. ESTRATEGIA DE PRUEBAS

### 8.1 Pruebas Unitarias Existentes (290 tests — todos pasan)

| Archivo de Test | Qué prueba | Estado |
|----------------|------------|--------|
| `PrendasControllerTests.cs` | CRUD de prendas, mock IUnitOfWork + IMapper | ✅ Pasa |
| `ClienteDTOTests.cs` | Propiedades alias Nombre/Email, validaciones | ✅ Pasa (4 warnings CS8625) |
| `MetodoPagoDTOTests.cs` | Validaciones de MetodoPagoDTO | ✅ Pasa (1 warning CS8625) |
| `ClienteEntityTests.cs` | Propiedades de entidad Cliente | ✅ Pasa (4 warnings CS8625) |
| `VentaEntityTests.cs` | Propiedades de entidad Venta | ✅ Pasa (1 warning MSTEST0032) |
| `RolEntityTests.cs` | Propiedades de entidad Rol | ✅ Pasa (1 warning MSTEST0032) |

### 8.2 Pruebas Unitarias a Agregar

#### Prioridad Alta — Lógica de Ventas

```
[TestClass] VentasControllerTests
├─ ApiRegistrarVenta_ConStockInsuficiente_RetornaExitoFalso
├─ ApiRegistrarVenta_ConDetallesVacios_RetornaExitoFalso
├─ ApiRegistrarVenta_Exitosa_RetornaVentaId
├─ ApiRegistrarVenta_ConClienteInexistente_RetornaExitoFalso
└─ ApiValidarVenta_ConDatosValidos_RetornaExitoTrue

[TestClass] ServicioVentasTests
├─ RegistrarVenta_FallaEnStock_HaceRollback
├─ ActualizarInventario_ReduceStock_Correctamente
├─ CalcularTotalVenta_SumaSubtotalesCorrectamente
└─ ValidarVenta_StockInsuficiente_RetornaMensajeDescriptivo
```

#### Prioridad Media — Controladores de soporte

```
[TestClass] HomeControllerTests
├─ Index_UsuarioAutenticado_RetornaDashboardViewModel
└─ Index_PrendasAgotandose_SoloMuestraStock5OMenos

[TestClass] ClientesControllerTests
├─ Index_RetornaListaDeClientesDTO
└─ Create_ModelStateInvalido_RetornaVistaConErrores
```

### 8.3 Correcciones de Tests Existentes

#### CS8625 — Literales null en tipos non-nullable

```csharp
// ANTES (genera warning CS8625):
new ClienteDTO { NombreCompleto = null, DNI = null, ... }

// DESPUÉS (corregido):
new ClienteDTO { NombreCompleto = string.Empty, DNI = string.Empty, ... }
// O usar el patrón:
new ClienteDTO { NombreCompleto = null!, DNI = null!, ... }
```

#### MSTEST0032 — Assertions always true

```csharp
// ANTES (always true — MSTEST0032):
var venta = new Venta();
Assert.IsNotNull(venta);  // Un new siempre es not null

// DESPUÉS (assertion con valor real):
Assert.AreEqual(DateTime.Now.Date, venta.Fecha.Date);
Assert.AreEqual(0, venta.Total);
```

### 8.4 Herramientas de Testing

| Herramienta | Uso |
|------------|-----|
| MSTest | Framework de tests |
| Moq | Mock de IUnitOfWork, IServicioVentas, IMapper |
| EF InMemory | Tests de repositorio sin SQL Server real |
| dotnet test --collect:"XPlat Code Coverage" | Cobertura de código |

---

## 9. RIESGOS Y MITIGACIONES

| ID | Riesgo | Prob. | Impacto | Mitigación |
|----|--------|-------|---------|------------|
| R-01 | Delegar a `IServicioVentas` cambia comportamiento de transacción | Media | Alto | Agregar transacción explícita en `ServicioVentas.RegistrarVenta()` antes de mover la lógica |
| R-02 | Los endpoints nuevos (`/api/clientes-disponibles`, etc.) pueden romper otros tests existentes | Baja | Medio | Agregar tests unitarios antes de implementar |
| R-03 | El formulario de ventas en JS depende de IDs numéricos — cambio de hardcoded a dinámico puede romper flujo si BD no tiene datos | Media | Alto | Documentar que la BD debe tener al menos 1 cliente, 1 vendedor y 1 método de pago para que el módulo funcione |
| R-04 | `ServicioVentas.ActualizarInventario()` llama a `SaveChangesAsync()` propio — dentro de una transacción genera doble commit | Alta | Alto | Refactorizar `ActualizarInventario()` para solo modificar la entidad en memoria (sin `SaveChangesAsync()` interno) — el commit lo hace el `RegistrarVenta()` |
| R-05 | `IServicioVentas` registrado en DI pero `VentasController` no lo usa — riesgo de código muerto acumulado | Media | Bajo | Eliminar métodos privados duplicados en `VentasController` una vez migrado a `IServicioVentas` |
| R-06 | `_Layout.cshtml:135` tiene CS8602 — crash si `User.Identity` es null en contextos anónimos | Baja | Medio | Proteger con: `@(User.Identity?.Name ?? "Usuario")` — cambio aprobado pendiente |
| R-07 | Tests con CS8625 podrían ocultar bugs reales de validación | Baja | Bajo | Corregir usando `string.Empty` en lugar de `null` en propiedades non-nullable |

---

## 10. SECUENCIA DE IMPLEMENTACIÓN PROPUESTA

### Fase A — Preparar la base de datos (pre-requisito)

```
1. dotnet ef database update --project FashionStore.Infrastructure1 --startup-project FashionStore.Web
2. Insertar datos mínimos:
   - Al menos 1 Categoria
   - Al menos 1 MetodoPago
   - Al menos 1 Cliente
   - Al menos 1 Vendedor
```

### Fase B — Corregir ServicioVentas (D-04, D-05, R-04)

```
Archivo: FashionStore.Infrastructure1/Services/ServicioVentas.cs
1. Modificar ActualizarInventario(): eliminar SaveChangesAsync() interno
2. Modificar RegistrarVenta(): agregar BeginTransactionAsync/CommitAsync/RollbackAsync
3. Ejecutar dotnet build → 0 errores
```

### Fase C — Corregir VentasController (D-06)

```
Archivo: FashionStore.Web/Controllers/VentasController.cs
1. Inyectar IServicioVentas en el constructor
2. ApiRegistrarVenta → usar _servicioVentas.RegistrarVenta()
3. ApiValidarVenta   → usar _servicioVentas.ValidarVenta()
4. Eliminar métodos privados RegistrarVentaInterno() y ValidarVentaInterno()
5. Agregar endpoints: ApiClientesDisponibles(), ApiVendedoresDisponibles(), ApiMetodosPago()
6. Ejecutar dotnet build → 0 errores
7. Ejecutar dotnet test  → 290+ pasan
```

### Fase D — Corregir Vista Ventas/Index.cshtml (D-01, D-02, D-03, D-10)

```
Archivo: FashionStore.Web/Views/Ventas/Index.cshtml
1. cargarClientes()   → fetch('/api/clientes-disponibles')
2. cargarProductos()  → fetch('/api/productos-disponibles') [ya existe]
3. cargarVendedores() → fetch('/api/vendedores-disponibles') [nuevo]
4. cargarMetodosPago()→ fetch('/api/metodos-pago') [nuevo]
5. Agregar campo <select id="vendedor"> en el modal
6. Reemplazar select metodoPago hardcoded por poblado dinámicamente
7. submit handler → POST /api/registrar-venta con payload completo
8. Prueba manual: abrir modal, seleccionar datos, completar venta
```

### Fase E — Corregir Tests (D-08, D-09)

```
Archivos: FashionStore.Tests/**/*Tests.cs
1. CS8625: reemplazar null por string.Empty en constructores de DTO con non-nullable
2. MSTEST0032: reemplazar Assert.IsNotNull(new X()) por assertions con valores reales
3. Ejecutar dotnet test → 290+ pasan, 0 warnings CS8625, 0 MSTEST0032
```

### Fase F — Validación Final

```
dotnet clean
dotnet build → Build succeeded. 0 Error(s). ≤1 Warning(s)
dotnet test  → Passed! ≥290 tests. 0 failed.
```

---

## 11. DECISIONES DE DISEÑO DOCUMENTADAS

| Decisión | Alternativa Descartada | Razón |
|----------|----------------------|-------|
| Usar `IServicioVentas` existente en VentasController | Mantener lógica duplicada en el controlador | El servicio ya existe, está en DI, y es la capa correcta para la lógica de negocio |
| Agregar transacción en `ServicioVentas`, no en el controlador | Transacción en el controlador | La transacción debe estar cerca de los datos, en la capa de infraestructura |
| `ActualizarInventario()` sin `SaveChangesAsync()` propio | Commit inmediato dentro del método | Evita doble commit cuando se llama desde dentro de una transacción mayor |
| Endpoints `/api/clientes-disponibles` en VentasController | Crear un nuevo ApiController | Consistencia: todos los endpoints de la UI de ventas están en el mismo controlador |
| `IList<PrendaDTO>` para `PrendasAgotandose` | `IEnumerable<PrendaDTO>` | La vista usa `.Count` que es propiedad en IList, método en IEnumerable (evita CS8978) |
| Mantener `_context` para `Include()` profundo | Extender GenericRepository con Include | Cambio de mayor impacto sin beneficio proporcional — el plan documenta esto como excepción permitida |

---

**Documento generado el:** 7 de julio de 2026  
**Autor:** Kiro — Arquitecto de Software / SSD  
**Versión:** 1.0  
**Estado:** Listo para implementación — No modifica código todavía
