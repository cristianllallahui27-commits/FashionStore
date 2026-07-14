# TASKS_CORRECCION.md
# Lista de Tareas Implementables — FashionStoreSolution

**Metodología:** Specs Driven Development  
**Basado en:** SPEC_REQUISITOS_CORRECCION.md v2.0 + DESIGN_CORRECCION_TECNICA.md v1.0  
**Fecha:** 7 de julio de 2026  
**Estado del build actual:** ✅ 0 errores | 1 advertencia (CS8602 en _Layout.cshtml)  
**Tests actuales:** ✅ 290/290 pasan

> **Leyenda de estado:**
> - ✅ YA CORREGIDO — verificado en código actual
> - 🔧 PENDIENTE — requiere implementación
> - ⚠️ PARCIAL — implementado pero con problemas detectados

---

## FASE 1 — COMPILACIÓN Y DTOs

---

### TASK-01
**Descripción:** Verificar que Home/Index.cshtml y DashboardViewModel están sincronizados  
**Estado:** ✅ YA CORREGIDO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-01 |
| **Archivos afectados** | `Views/Home/Index.cshtml`, `ViewModels/DashboardViewModel.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | La vista usa `Model.PrendasAgotandose.Count`, `Model.Prendas.Count`, `Model.TotalPrendas`, `Model.TotalCategorias`, `Model.TotalClientes`, `Model.TotalVentas`. El ViewModel ya tiene todas estas propiedades. `PrendasAgotandose` y `Prendas` son `IList<PrendaDTO>` — correcto para `.Count`. |
| **Criterio de aceptación** | `dotnet build` → 0 errores en FashionStore.Web |
| **Comando de verificación** | `dotnet build "FashionStoreSolution.sln"` |

---

### TASK-02
**Descripción:** Verificar que Clientes/Index.cshtml y ClienteDTO están sincronizados  
**Estado:** ✅ YA CORREGIDO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-02 |
| **Archivos afectados** | `Views/Clientes/Index.cshtml`, `Domain/DTOs/ClienteDTO.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | La vista usa `@cliente.NombreCompleto` y `@cliente.DNI` — ambas propiedades existen en `ClienteDTO`. También usa `@cliente.Nombre` en el JS de `confirmarEliminar` — la propiedad alias `Nombre => NombreCompleto` existe en el DTO. |
| **Criterio de aceptación** | Vista compila, tabla muestra NombreCompleto y DNI correctamente |
| **Comando de verificación** | `dotnet build "FashionStoreSolution.sln"` |

---

### TASK-03
**Descripción:** Verificar que Prendas/Index.cshtml y PrendaDTO están sincronizados  
**Estado:** ✅ YA CORREGIDO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-03 |
| **Archivos afectados** | `Views/Prendas/Index.cshtml`, `Domain/DTOs/PrendaDTO.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | La vista usa `@prenda.Categoria?.Nombre` — el DTO expone la propiedad calculada `Categoria` de tipo `CategoriaInfo` con campo `Nombre`. También usa `prenda.Nombre`, `prenda.Color`, `prenda.Talla`, `prenda.Precio`, `prenda.Stock`, `prenda.Id` — todos presentes en `PrendaDTO`. |
| **Criterio de aceptación** | Vista compila y muestra el nombre de categoría correctamente |
| **Comando de verificación** | `dotnet build "FashionStoreSolution.sln"` |

---

### TASK-04
**Descripción:** Eliminar usings duplicados — ClientesController.cs  
**Estado:** ✅ YA CORREGIDO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-04 |
| **Archivos afectados** | `Controllers/ClientesController.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | Se eliminó la segunda directiva `using AutoMapper;` que generaba CS0105. El archivo ahora tiene `using AutoMapper;` solo una vez. |
| **Criterio de aceptación** | `dotnet build` sin advertencias CS0105 |
| **Comando de verificación** | `dotnet build "FashionStoreSolution.sln" 2>&1 \| findstr CS0105` → sin resultados |

---

### TASK-05
**Descripción:** Corregir desreferencia null CS8602 en _Layout.cshtml línea 135  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-05 |
| **Archivos afectados** | `Views/Shared/_Layout.cshtml` |
| **Dependencias** | Ninguna |
| **Detalle** | La línea 135 usa `@User.Identity.Name` directamente. El compilador advierte CS8602 porque `User.Identity` puede ser null. Corrección: cambiar a `@(User.Identity?.Name ?? "Usuario")`. Nota: este cambio fue rechazado anteriormente — requiere aprobación del usuario. |
| **Criterio de aceptación** | `dotnet build` → 0 advertencias CS8602 en FashionStore.Web |
| **Comando de verificación** | `dotnet build "FashionStoreSolution.sln" 2>&1 \| findstr CS8602` → sin resultados |

---

### TASK-06
**Descripción:** Validar build final de Fase 1  
**Estado:** ✅ YA CUMPLIDO (pendiente solo TASK-05)

| Campo | Valor |
|-------|-------|
| **ID** | TASK-06 |
| **Archivos afectados** | Solución completa |
| **Dependencias** | TASK-01 a TASK-05 |
| **Criterio de aceptación** | Build succeeded. 0 Error(s). 0 Warning(s) en FashionStore.Web |
| **Comando de verificación** | `dotnet build "C:\Users\CRISTIAN\source\repos\FashionStoreSolution\FashionStoreSolution.sln"` |

---

## FASE 2 — VENTAS E INVENTARIO

---

### TASK-07
**Descripción:** Agregar endpoint `GET /api/clientes-disponibles` en VentasController  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-07 |
| **Archivos afectados** | `Controllers/VentasController.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | La vista `Ventas/Index.cshtml` llama a `fetch('/api/clientes')` que no existe. Debe crearse `GET /api/clientes-disponibles` que retorne `{ exito: true, datos: [{ id, nombreCompleto }] }` usando `_unitOfWork.Clientes.GetAllAsync()`. Requiere `[Authorize]`. |
| **Criterio de aceptación** | `GET /api/clientes-disponibles` retorna JSON con lista de clientes; `dotnet build` sin errores |
| **Comando de verificación** | `dotnet build "FashionStoreSolution.sln"` + prueba manual con navegador autenticado |

---

### TASK-08
**Descripción:** Agregar endpoint `GET /api/vendedores-disponibles` en VentasController  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-08 |
| **Archivos afectados** | `Controllers/VentasController.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | El modal de nueva venta no incluye selector de vendedor. Debe crearse `GET /api/vendedores-disponibles` que retorne vendedores con `Estado = true` usando `_unitOfWork.Vendedores.FindAsync(v => v.Estado)`. Requiere `[Authorize]`. Respuesta: `{ exito: true, datos: [{ id, nombreCompleto, dni }] }`. |
| **Criterio de aceptación** | `GET /api/vendedores-disponibles` retorna solo vendedores activos |
| **Comando de verificación** | `dotnet build "FashionStoreSolution.sln"` + prueba manual |

---

### TASK-09
**Descripción:** Agregar endpoint `GET /api/metodos-pago` en VentasController  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-09 |
| **Archivos afectados** | `Controllers/VentasController.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | El `<select id="metodoPago">` en `Ventas/Index.cshtml` tiene opciones hardcoded con valores de texto ("efectivo", "tarjeta") — no son IDs numéricos válidos para la BD. Debe crearse `GET /api/metodos-pago` que retorne `{ exito: true, datos: [{ id, nombre }] }` desde `_unitOfWork.MetodosPago.GetAllAsync()`. Requiere `[Authorize]`. |
| **Criterio de aceptación** | Select de método de pago se puebla con IDs numéricos reales de la BD |
| **Comando de verificación** | `dotnet build "FashionStoreSolution.sln"` + verificar que BD tiene al menos 1 MetodoPago |

---

### TASK-10
**Descripción:** Corregir Vista Ventas/Index.cshtml — actualizar JS para usar endpoints reales  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-10 |
| **Archivos afectados** | `Views/Ventas/Index.cshtml` |
| **Dependencias** | TASK-07, TASK-08, TASK-09 |
| **Detalle** | Tres problemas en el JS actual: (1) `fetch('/api/clientes')` → cambiar a `fetch('/api/clientes-disponibles')`. (2) `fetch('/api/prendas')` → cambiar a `fetch('/api/productos-disponibles')` (ya existe). (3) El submit solo llama a `showSuccess` sin enviar datos. Debe construir el payload `RegistrarVentaRequest` y hacer `POST /api/registrar-venta`. |
| **Criterio de aceptación** | Al abrir el modal: selects de cliente, producto y método de pago se cargan desde la BD. Al dar "Completar Venta": se envía el POST y la venta se registra. |
| **Comando de verificación** | Prueba manual: abrir modal → seleccionar datos → completar venta → verificar en BD |

---

### TASK-11
**Descripción:** Agregar campo vendedor al modal de nueva venta en Ventas/Index.cshtml  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-11 |
| **Archivos afectados** | `Views/Ventas/Index.cshtml` |
| **Dependencias** | TASK-08 |
| **Detalle** | El modal actualmente no tiene campo para seleccionar vendedor. La entidad `Venta` requiere `VendedorId` obligatorio. Debe agregarse `<select id="vendedor">` en el modal, poblado por `cargarVendedores()` desde `/api/vendedores-disponibles`. |
| **Criterio de aceptación** | El modal incluye selector de vendedor con opciones de la BD; el payload incluye `vendedorId` |
| **Comando de verificación** | Prueba manual: modal muestra select de vendedor con datos |

---

### TASK-12
**Descripción:** Verificar que IServicioVentas.RegistrarVenta usa transacción atómica  
**Estado:** ✅ YA CORREGIDO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-12 |
| **Archivos afectados** | `Infrastructure1/Services/ServicioVentas.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | `ServicioVentas.RegistrarVenta()` ya usa `BeginTransactionAsync/CommitAsync/RollbackAsync`. `ActualizarInventario()` ya no llama `SaveChangesAsync()` propio — solo modifica la entidad en memoria. `ValidarVenta()` ya usa `_unitOfWork` en lugar de `_context` directo. El D-04+R-04 fue corregido. |
| **Criterio de aceptación** | Si falla actualización de stock → venta no queda en BD (rollback) |
| **Comando de verificación** | Test unitario: `ServicioVentasTests.RegistrarVenta_FallaEnStock_HaceRollback` |

---

### TASK-13
**Descripción:** Migrar VentasController para usar IServicioVentas en lugar de lógica duplicada  
**Estado:** ⚠️ PARCIAL

| Campo | Valor |
|-------|-------|
| **ID** | TASK-13 |
| **Archivos afectados** | `Controllers/VentasController.cs` |
| **Dependencias** | TASK-12 |
| **Detalle** | `VentasController` actualmente tiene sus propios métodos `RegistrarVentaInterno()` y `ValidarVentaInterno()` que duplican la lógica de `ServicioVentas`. Según el diseño, el controlador debe inyectar `IServicioVentas` y delegar. Los métodos privados duplicados deben eliminarse. Actualmente `IServicioVentas` está registrado en DI pero `VentasController` no lo inyecta ni usa. |
| **Criterio de aceptación** | `VentasController` no tiene `RegistrarVentaInterno()` ni `ValidarVentaInterno()`. Usa `_servicioVentas.RegistrarVenta()` y `_servicioVentas.ValidarVenta()`. `dotnet build` + `dotnet test` pasan. |
| **Comando de verificación** | `dotnet build "FashionStoreSolution.sln"` + `dotnet test` |

---

### TASK-14
**Descripción:** Validar stock insuficiente antes de registrar venta  
**Estado:** ✅ YA CORREGIDO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-14 |
| **Archivos afectados** | `Infrastructure1/Services/ServicioVentas.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | `ValidarVenta()` verifica que `prenda.Stock >= detalle.Cantidad` antes de continuar. Si no, retorna `(false, "Stock insuficiente para {nombre}. Disponible: {stock}, Solicitado: {cantidad}")`. Esto se ejecuta antes de abrir la transacción. |
| **Criterio de aceptación** | `POST /api/registrar-venta` con cantidad > stock retorna `{ exito: false, mensaje: "Stock insuficiente..." }` sin crear venta |
| **Comando de verificación** | Test: `ValidarVenta_StockInsuficiente_RetornaMensajeDescriptivo` |

---

### TASK-15
**Descripción:** Verificar actualización automática de stock post-venta  
**Estado:** ✅ YA CORREGIDO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-15 |
| **Archivos afectados** | `Infrastructure1/Services/ServicioVentas.cs` |
| **Dependencias** | TASK-12 |
| **Detalle** | `ActualizarInventario()` reduce `prenda.Stock -= cantidad` en memoria y llama `_unitOfWork.Prendas.Update()`. El commit lo hace `RegistrarVenta()` en un único punto. Stock nunca queda negativo (validado previamente). |
| **Criterio de aceptación** | Después de venta exitosa: `SELECT Stock FROM Prendas WHERE Id=X` muestra stock reducido |
| **Comando de verificación** | Prueba manual: consulta BD antes y después de venta |

---

## FASE 3 — SEGURIDAD

---

### TASK-16
**Descripción:** Verificar orden correcto del middleware de autenticación en Program.cs  
**Estado:** ✅ YA CORREGIDO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-16 |
| **Archivos afectados** | `FashionStore.Web/Program.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | El orden actual en Program.cs es: `UseRouting()` → `UseAuthentication()` → `UseAuthorization()` → middleware personalizado de redirección. Este es el orden correcto. El bug P-02 fue corregido. |
| **Criterio de aceptación** | Usuario autenticado que accede a `/` es redirigido a `/Home/Index`, no a login |
| **Comando de verificación** | Prueba manual: login → navegar a `https://localhost:5001/` → debe ir a `/Home/Index` |

---

### TASK-17
**Descripción:** Verificar inicialización automática de roles en primera ejecución  
**Estado:** ✅ YA CORREGIDO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-17 |
| **Archivos afectados** | `Program.cs`, `Infrastructure1/Data/DbInitializer.cs` |
| **Dependencias** | Base de datos aplicada (migraciones) |
| **Detalle** | `await FashionStore.Infrastructure.Data.DbInitializer.Initialize(app)` es invocado en `Program.cs` antes de `app.Run()`. `DbInitializer.SeedRoles()` verifica `RoleExistsAsync` antes de crear, garantizando idempotencia. |
| **Criterio de aceptación** | `SELECT COUNT(*) FROM AspNetRoles` retorna 2 tras primera ejecución |
| **Comando de verificación** | `dotnet run --project FashionStore.Web` + consulta SQL a `AspNetRoles` |

---

### TASK-18
**Descripción:** Verificar que PrendasController tiene [Authorize]  
**Estado:** ✅ YA CORREGIDO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-18 |
| **Archivos afectados** | `Controllers/PrendasController.cs` |
| **Dependencias** | TASK-17 |
| **Detalle** | `[Authorize]` agregado a nivel de clase en `PrendasController`. Todos los endpoints `/Prendas/*` requieren autenticación. |
| **Criterio de aceptación** | Navegar a `/Prendas` sin login redirige a `/Identity/Account/Login` |
| **Comando de verificación** | Prueba manual: abrir ventana privada → navegar a `/Prendas/Index` → debe redirigir a login |

---

### TASK-19
**Descripción:** Verificar que CategoriasController tiene [Authorize]  
**Estado:** ✅ VERIFICADO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-19 |
| **Archivos afectados** | `Controllers/CategoriasController.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | `[Authorize]` ya está presente a nivel de clase en `CategoriasController` (línea 8). Correcto. |
| **Criterio de aceptación** | Navegar a `/Categorias` sin login redirige a login |
| **Comando de verificación** | Prueba manual: ventana privada → `/Categorias/Index` → redirige a login |

---

### TASK-20
**Descripción:** Agregar [Authorize] a HomeController  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-20 |
| **Archivos afectados** | `Controllers/HomeController.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | `HomeController` no tiene `[Authorize]` a nivel de clase. El `Index()` tiene una verificación manual `if (!User.Identity?.IsAuthenticated)` pero no es suficiente — no protege los otros métodos (`Privacy()`, `Error()`). Debe agregarse `[Authorize]` a nivel de clase y eliminar la verificación manual redundante del `Index()`. |
| **Criterio de aceptación** | Navegar a `/Home/Index` sin login redirige a `/Identity/Account/Login` automáticamente |
| **Comando de verificación** | Prueba manual: ventana privada → `/Home/Index` → redirige a login |

---

### TASK-21
**Descripción:** Revisar y restringir acceso a ConfiguracionController solo a Administrador  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-21 |
| **Archivos afectados** | `Controllers/ConfiguracionController.cs` (si existe) |
| **Dependencias** | TASK-17 (roles inicializados) |
| **Detalle** | El módulo de configuración del sistema debe ser accesible solo por usuarios con rol `Administrador`. Si `ConfiguracionController` no tiene `[Authorize(Roles = "Administrador")]`, debe agregarse. Un vendedor que acceda a `/Configuracion` debe recibir 403 Forbidden. |
| **Criterio de aceptación** | Usuario con rol `Vendedor` que accede a `/Configuracion` recibe redirección a `/Identity/Account/AccessDenied` |
| **Comando de verificación** | Prueba manual: login como Vendedor → navegar a `/Configuracion` → debe dar acceso denegado |

---

### TASK-22
**Descripción:** Verificar que los endpoints /api/* protegidos requieren autenticación  
**Estado:** ⚠️ PARCIAL

| Campo | Valor |
|-------|-------|
| **ID** | TASK-22 |
| **Archivos afectados** | `Controllers/VentasController.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | Situación actual: `ApiProductosDisponibles` y `ApiBuscar` tienen `[AllowAnonymous]` (intencional). `ApiRegistrarVenta`, `ApiValidarVenta` y `ApiProductosAgotandose` heredan `[Authorize]` de la clase. Los nuevos endpoints `ApiClientesDisponibles`, `ApiVendedoresDisponibles`, `ApiMetodosPago` (TASK-07/08/09) deben heredar `[Authorize]` también. |
| **Criterio de aceptación** | Llamada sin autenticación a `/api/registrar-venta` retorna 401; `/api/productos-disponibles` retorna 200 |
| **Comando de verificación** | `curl -X POST https://localhost:5001/api/registrar-venta` sin cookie → debe retornar 302 a login |

---

## FASE 4 — DASHBOARDS Y REPORTES

---

### TASK-23
**Descripción:** Verificar que Home/Index.cshtml usa datos reales del ViewModel  
**Estado:** ✅ YA CORREGIDO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-23 |
| **Archivos afectados** | `Controllers/HomeController.cs`, `ViewModels/DashboardViewModel.cs` |
| **Dependencias** | Base de datos con datos |
| **Detalle** | `HomeController.Index()` usa `_unitOfWork` para: totales de categorías, prendas, clientes, ventas, stock e ingresos. Solo usa `_context.Users.CountAsync()` para TotalUsuarios (excepción de Identity). Los gráficos de ventas mensuales/semanales usan datos reales de `allVentas`. No hay contadores estáticos (0, null). |
| **Criterio de aceptación** | `Model.TotalPrendas` coincide con `SELECT COUNT(*) FROM Prendas` |
| **Comando de verificación** | Prueba manual: comparar valores en dashboard con consultas SQL directas |

---

### TASK-24
**Descripción:** Corregir contadores estáticos en Ventas/Index.cshtml  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-24 |
| **Archivos afectados** | `Views/Ventas/Index.cshtml`, `Controllers/VentasController.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | La vista `Ventas/Index.cshtml` muestra "Ventas Hoy: 0", "Ingresos Hoy: S/. 0.00", "Total Ventas: 0", "Ingresos Totales: S/. 0.00" como valores hardcodeados en HTML. Deben ser datos reales. Opciones: (A) Convertir la vista a typed con un ViewModel que incluya estos totales, o (B) Cargarlos con JavaScript desde un endpoint. Recomendado: pasar un ViewModel con los datos al View desde `VentasController.Index()`. |
| **Criterio de aceptación** | Las tarjetas de resumen muestran datos reales de la BD, no ceros estáticos |
| **Comando de verificación** | Insertar una venta → navegar a `/Ventas/Index` → "Total Ventas" muestra ≥ 1 |

---

### TASK-25
**Descripción:** Verificar que Ventas/Dashboard.cshtml usa datos reales del DashboardViewModel  
**Estado:** ✅ YA CORRECTO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-25 |
| **Archivos afectados** | `Views/Ventas/Dashboard.cshtml`, `Controllers/VentasController.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | La vista ya usa `@Model.TotalVentas`, `@Model.TotalClientes`, `@Model.TotalIngresos`, `@Html.Raw(Model.SalesChartLabelsJson)`, `@Html.Raw(Model.PrendasByCategoryLabelsJson)`. El controlador `VentasController.Dashboard()` los puebla desde `_context` (con Include necesario). Los datos son reales — no estáticos. |
| **Criterio de aceptación** | Dashboard de ventas muestra gráficos con datos reales al existir ventas registradas |
| **Comando de verificación** | Prueba manual: navegar a `/Ventas/Dashboard` con BD con datos |

---

### TASK-26
**Descripción:** Verificar que Prendas/Dashboard usa datos reales por categoría  
**Estado:** ✅ YA CORRECTO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-26 |
| **Archivos afectados** | `Controllers/PrendasController.cs` → `Dashboard()` |
| **Dependencias** | Ninguna |
| **Detalle** | `PrendasController.Dashboard()` agrupa prendas por `CategoriaId` y obtiene el nombre de cada categoría via `_unitOfWork.Categorias.GetByIdAsync()`. Genera `Labels`, `Counts`, `Stocks` para ViewData. Datos reales de BD. |
| **Criterio de aceptación** | Dashboard de prendas muestra la cantidad correcta de prendas por categoría |
| **Comando de verificación** | Prueba manual: navegar a `/Prendas/Dashboard` con prendas en BD |

---

### TASK-27
**Descripción:** Verificar que Home/Index dashboard muestra prendas agotándose correctamente  
**Estado:** ✅ YA CORRECTO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-27 |
| **Archivos afectados** | `Controllers/HomeController.cs`, `Views/Home/Index.cshtml` |
| **Dependencias** | Ninguna |
| **Detalle** | `HomeController.Index()` usa `_unitOfWork.Prendas.FindAsync(p => p.Stock > 0 && p.Stock <= 5)`. La vista verifica `Model?.PrendasAgotandose?.Count > 0` y muestra hasta 4 ítems. Si no hay prendas agotándose, muestra mensaje "Todos los productos tienen un stock óptimo." |
| **Criterio de aceptación** | Insertar prenda con stock=3 → aparece en sección "Estado de Stock" del dashboard |
| **Comando de verificación** | Prueba manual con BD |

---

### TASK-28
**Descripción:** Verificar que el dashboard de Clientes muestra datos reales  
**Estado:** ✅ YA CORRECTO

| Campo | Valor |
|-------|-------|
| **ID** | TASK-28 |
| **Archivos afectados** | `Controllers/ClientesController.cs` → `Dashboard()` |
| **Dependencias** | Ninguna |
| **Detalle** | `ClientesController.Dashboard()` usa `_unitOfWork.Clientes.GetAllAsync()`, calcula `total` y selecciona los 10 más recientes. Los datos son reales — no estáticos. |
| **Criterio de aceptación** | `ViewData["TotalClientes"]` coincide con `SELECT COUNT(*) FROM Clientes` |
| **Comando de verificación** | Prueba manual: navegar a `/Clientes/Dashboard` |

---

## FASE 5 — PRUEBAS

---

### TASK-29
**Descripción:** Corregir advertencias CS8625 en tests (literales null en tipos non-nullable)  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-29 |
| **Archivos afectados** | `Tests/DTOs/ClienteDTOTests.cs`, `Tests/DTOs/MetodoPagoDTOTests.cs`, `Tests/Entities/ClienteEntityTests.cs`, `Tests/Controllers/PrendasControllerTests.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | 13 advertencias CS8625 en el proyecto de tests: asignación de `null` a propiedades con tipo `string` (non-nullable). Corrección: reemplazar `null` por `string.Empty` o `null!` (null-forgiving) en los constructores de datos de prueba. |
| **Criterio de aceptación** | `dotnet build` del proyecto Tests → 0 advertencias CS8625 |
| **Comando de verificación** | `dotnet build "FashionStore.Tests\FashionStore.Tests.csproj" 2>&1 \| findstr CS8625` → sin resultados |

---

### TASK-30
**Descripción:** Corregir advertencias MSTEST0032 — assertions always true  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-30 |
| **Archivos afectados** | `Tests/Entities/VentaEntityTests.cs`, `Tests/Entities/RolEntityTests.cs` |
| **Dependencias** | Ninguna |
| **Detalle** | 2 advertencias MSTEST0032: `Assert.IsNotNull(new Venta())` y `Assert.IsNotNull(new Rol())` — un `new` siempre es not null. Corrección: reemplazar con assertions que prueben valores reales: `Assert.AreEqual(DateTime.Now.Date, venta.Fecha.Date)`, `Assert.AreEqual(0, venta.Total)`, etc. |
| **Criterio de aceptación** | `dotnet build` del proyecto Tests → 0 advertencias MSTEST0032 |
| **Comando de verificación** | `dotnet build "FashionStore.Tests\FashionStore.Tests.csproj" 2>&1 \| findstr MSTEST0032` → sin resultados |

---

### TASK-31
**Descripción:** Agregar tests unitarios para ServicioVentas  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-31 |
| **Archivos afectados** | `Tests/Services/ServicioVentasTests.cs` (nuevo archivo) |
| **Dependencias** | TASK-12, TASK-13 |
| **Detalle** | Crear clase de tests con MSTest + Moq para cubrir los casos críticos del servicio. Tests mínimos requeridos: `RegistrarVenta_ConStockInsuficiente_LanzaInvalidOperationException`, `ValidarVenta_ClienteInexistente_RetornaExitoFalso`, `ValidarVenta_VendedorInexistente_RetornaExitoFalso`, `ValidarVenta_DetallesVacios_RetornaExitoFalso`, `CalcularTotalVenta_SumaCorrectamenteSubtotales`, `ActualizarInventario_ReduceStock_SinSaveChanges`. |
| **Criterio de aceptación** | `dotnet test` → todos los nuevos tests pasan |
| **Comando de verificación** | `dotnet test "FashionStore.Tests\FashionStore.Tests.csproj" --filter "ServicioVentasTests"` |

---

### TASK-32
**Descripción:** Agregar tests unitarios para VentasController  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-32 |
| **Archivos afectados** | `Tests/Controllers/VentasControllerTests.cs` (nuevo archivo) |
| **Dependencias** | TASK-13 |
| **Detalle** | Tests con Moq de `IServicioVentas` (después de migrar el controlador en TASK-13). Tests requeridos: `ApiRegistrarVenta_RequestNull_RetornaExitoFalso`, `ApiRegistrarVenta_DetallesVacios_RetornaExitoFalso`, `ApiRegistrarVenta_ServicioFalla_RetornaExitoFalso`, `ApiRegistrarVenta_Exitosa_RetornaVentaIdEnJson`, `ApiValidarVenta_DatosValidos_LlamaAlServicio`. |
| **Criterio de aceptación** | `dotnet test` → todos los nuevos tests pasan; `IServicioVentas` se puede mockear |
| **Comando de verificación** | `dotnet test "FashionStore.Tests\FashionStore.Tests.csproj" --filter "VentasControllerTests"` |

---

### TASK-33
**Descripción:** Agregar test de autorización — PrendasController requiere autenticación  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-33 |
| **Archivos afectados** | `Tests/Controllers/PrendasControllerTests.cs` (existente — agregar test) |
| **Dependencias** | TASK-18 |
| **Detalle** | Agregar test que verifique que `PrendasController` tiene el atributo `[Authorize]` a nivel de clase usando reflexión: `typeof(PrendasController).GetCustomAttributes(typeof(AuthorizeAttribute), true)` debe retornar al menos 1 elemento. |
| **Criterio de aceptación** | Test `PrendasController_TieneAtributoAuthorize` pasa |
| **Comando de verificación** | `dotnet test "FashionStore.Tests\FashionStore.Tests.csproj" --filter "TieneAtributoAuthorize"` |

---

### TASK-34
**Descripción:** Agregar test de autorización — HomeController requiere autenticación  
**Estado:** 🔧 PENDIENTE

| Campo | Valor |
|-------|-------|
| **ID** | TASK-34 |
| **Archivos afectados** | `Tests/Controllers/HomeControllerTests.cs` (nuevo archivo) |
| **Dependencias** | TASK-20 |
| **Detalle** | Agregar test que verifique que `HomeController` tiene `[Authorize]` a nivel de clase usando reflexión, similar a TASK-33. |
| **Criterio de aceptación** | Test `HomeController_TieneAtributoAuthorize` pasa |
| **Comando de verificación** | `dotnet test "FashionStore.Tests\FashionStore.Tests.csproj" --filter "HomeControllerTests"` |

---

### TASK-35
**Descripción:** Validar suite completa de tests — estado final  
**Estado:** ✅ BASE APROBADA (290 tests pasan) | 🔧 PENDIENTE ampliar

| Campo | Valor |
|-------|-------|
| **ID** | TASK-35 |
| **Archivos afectados** | `FashionStore.Tests` (proyecto completo) |
| **Dependencias** | TASK-29, TASK-30, TASK-31, TASK-32, TASK-33, TASK-34 |
| **Detalle** | Al completar todas las tareas de tests, ejecutar la suite completa. Objetivo: ≥ 305 tests (290 existentes + ≥ 15 nuevos). 0 errores. 0 advertencias CS8625. 0 advertencias MSTEST0032. |
| **Criterio de aceptación** | `dotnet test` → Passed! ≥ 305 tests - 0 failed - 0 skipped. Build succeeded. 0 Warning(s). |
| **Comando de verificación** | `dotnet test "C:\Users\CRISTIAN\source\repos\FashionStoreSolution\FashionStore.Tests\FashionStore.Tests.csproj" --verbosity normal` |

---

## RESUMEN EJECUTIVO DE TAREAS

### Conteo por estado

| Estado | Fase 1 | Fase 2 | Fase 3 | Fase 4 | Fase 5 | Total |
|--------|--------|--------|--------|--------|--------|-------|
| ✅ Ya corregido | 4 | 4 | 3 | 5 | 1 | **17** |
| ⚠️ Parcial | 0 | 1 | 1 | 0 | 0 | **2** |
| 🔧 Pendiente | 2 | 4 | 2 | 1 | 6 | **15** |
| **Total** | **6** | **9** | **6** | **6** | **7** | **34** |

---

### Tareas críticas (ordenadas por prioridad de implementación)

| Prioridad | ID | Tarea | Por qué es urgente |
|-----------|----|---------|--------------------|
| 🔴 1 | TASK-05 | CS8602 en _Layout.cshtml | Advertencia de compilación visible en cada build |
| 🔴 2 | TASK-13 | VentasController → IServicioVentas | Elimina duplicación de lógica de negocio |
| 🔴 3 | TASK-10 | JS de Ventas/Index con endpoints reales | Sin esto el modal de venta no funciona |
| 🔴 4 | TASK-07 | Endpoint /api/clientes-disponibles | Requerido por TASK-10 |
| 🔴 5 | TASK-08 | Endpoint /api/vendedores-disponibles | Requerido por TASK-11 |
| 🔴 6 | TASK-09 | Endpoint /api/metodos-pago | Select hardcodeado en vista |
| 🔴 7 | TASK-11 | Campo vendedor en modal | Venta sin vendedor falla en BD |
| 🟠 8 | TASK-20 | [Authorize] en HomeController | Sin esto el dashboard es accesible sin login |
| 🟠 9 | TASK-21 | Configuracion solo Admin | Riesgo de seguridad medio |
| 🟠 10 | TASK-24 | Contadores reales en Ventas/Index | Datos estáticos confunden al usuario |
| 🟡 11 | TASK-29 | CS8625 en tests | Deuda técnica en suite de tests |
| 🟡 12 | TASK-30 | MSTEST0032 | Tests con assertions vacías |
| 🟡 13 | TASK-31 | Tests de ServicioVentas | Cobertura de lógica crítica |
| 🟡 14 | TASK-32 | Tests de VentasController | Cobertura de controlador principal |
| 🟢 15 | TASK-33/34 | Tests de autorización | Verificación automática de seguridad |

---

### Comandos de validación final

```bash
# Compilación limpia
dotnet build "C:\Users\CRISTIAN\source\repos\FashionStoreSolution\FashionStoreSolution.sln"
# Esperado: Build succeeded. 0 Error(s). 0 Warning(s).

# Suite completa de tests
dotnet test "C:\Users\CRISTIAN\source\repos\FashionStoreSolution\FashionStore.Tests\FashionStore.Tests.csproj" --verbosity normal
# Esperado: Passed! ≥305 tests. 0 failed.

# Aplicar migraciones (si BD nueva)
dotnet ef database update --project FashionStore.Infrastructure1 --startup-project FashionStore.Web

# Verificar roles en BD
# SELECT * FROM AspNetRoles  →  debe tener: Administrador, Vendedor
```

---

**Documento generado el:** 7 de julio de 2026  
**Autor:** Kiro — Líder Técnico / SSD  
**Versión:** 1.0  
**Estado:** Listo para ejecución — No modifica código
