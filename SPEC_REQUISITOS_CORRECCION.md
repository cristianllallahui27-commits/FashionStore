# SPEC_REQUISITOS_CORRECCION.md
# Especificación Funcional — FashionStore (SSD / Specs Driven Development)

**Proyecto:** Sistema Administrativo para Tienda de Ropa y Lencería  
**Versión:** 2.0 — Basada en código real y fallas detectadas  
**Fecha:** 7 de julio de 2026  
**Metodología:** SSD — Specs Driven Development  
**Estado:** Listo para implementación de correcciones

---

## 1. ALCANCE DE CORRECCIÓN

### 1.1 Descripción del Sistema

FashionStore es un sistema web administrativo interno para gestión de una tienda de ropa y lencería. Permite administrar el catálogo de prendas, clientes, ventas, inventario y configuración del sistema. Está dirigido exclusivamente a usuarios internos con roles definidos.

### 1.2 Tecnologías Fijas (No Modificables)

| Capa | Tecnología | Versión |
|------|-----------|---------|
| Presentación | ASP.NET Core MVC + Razor | 9.0 |
| Dominio | C# / .NET | 9.0 |
| Datos | Entity Framework Core + SQL Server | 9.0.16 |
| Identidad | ASP.NET Core Identity | 9.0.16 |
| Mapeo | AutoMapper | 12.0.1 |
| Tests | MSTest + Moq + EF InMemory | 4.2.3+ |

### 1.3 Lo que se Corrige (en alcance)

- Orden del middleware de autenticación
- Inicialización automática de roles base
- Transacciones en registro de ventas
- Autorización faltante en PrendasController
- Refactorización hacia IUnitOfWork
- Errores de compilación y advertencias activas

### 1.4 Lo que NO se Corrige (fuera de alcance)

Ver sección 9 del presente documento.

---

## 2. ACTORES DEL SISTEMA

| Actor | Descripción | Acceso |
|-------|-------------|--------|
| **Administrador** | Usuario con acceso total al sistema | Todos los módulos + Configuración |
| **Vendedor** | Usuario con acceso operacional | Ventas, Catálogo, Clientes, Dashboard |
| **Sistema** | Proceso automático | Seeding de roles, actualización de stock |
| **Anónimo** | Usuario sin autenticar | Solo página de Login |

---

## 3. REQUISITOS FUNCIONALES

### RF-01 — Autenticación con ASP.NET Identity

**Prioridad:** Alta | **Estado:** Implementado con falla de middleware (P-02 corregido)

| ID | Requisito | Fuente en código |
|----|-----------|-----------------|
| RF-01.1 | El sistema debe autenticar usuarios mediante email y contraseña usando ASP.NET Core Identity | `Program.cs` — `AddIdentity<ApplicationUser, IdentityRole>()` |
| RF-01.2 | La contraseña debe requerir: mínimo 6 caracteres, al menos 1 dígito, 1 mayúscula, 1 minúscula | `Program.cs` — `options.Password.*` |
| RF-01.3 | No se requiere confirmación de email para login (`RequireConfirmedAccount = false`) | `Program.cs` |
| RF-01.4 | La sesión debe persistir 30 días con deslizamiento (`SlidingExpiration = true`) | `Program.cs` — `ConfigureApplicationCookie` |
| RF-01.5 | Un usuario no autenticado que accede a cualquier ruta protegida debe ser redirigido a `/Identity/Account/Login` | `Program.cs` — `LoginPath` |
| RF-01.6 | El middleware de autenticación (`UseAuthentication`) debe ejecutarse **antes** del middleware personalizado de redirección | `Program.cs` — orden de pipeline **[P-02 corregido]** |
| RF-01.7 | Un usuario autenticado que accede a `/` debe ser redirigido a `/Home/Index` | `Program.cs` — middleware personalizado |
| RF-01.8 | Un usuario no autenticado que accede a `/` debe ser redirigido a `/Identity/Account/Login` | `Program.cs` — middleware personalizado |

---

### RF-02 — Autorización por Roles

**Prioridad:** Alta | **Estado:** Roles existen pero no se inicializaban (P-01 corregido). PrendasController sin [Authorize] (P-05 corregido)

| ID | Requisito | Fuente en código |
|----|-----------|-----------------|
| RF-02.1 | El sistema debe tener exactamente dos roles: `Administrador` y `Vendedor` | `DbInitializer.cs` — `SeedRoles()` |
| RF-02.2 | Los roles deben crearse automáticamente al iniciar la aplicación si no existen (`RoleExistsAsync` + `CreateAsync`) | `DbInitializer.cs` **[P-01 corregido]** |
| RF-02.3 | El proceso de seeding de roles debe ser idempotente (sin errores si los roles ya existen) | `DbInitializer.cs` |
| RF-02.4 | Si el seeding falla, debe registrarse el error en el log sin impedir el inicio de la aplicación | `DbInitializer.cs` — bloque `catch` con `logger.LogError` |
| RF-02.5 | Todos los controladores MVC deben requerir autenticación (`[Authorize]`) | `VentasController`, `ClientesController`, `PrendasController` **[P-05 corregido]** |
| RF-02.6 | Solo usuarios con rol `Administrador` deben acceder a la configuración del sistema | Pendiente en `ConfiguracionController` |
| RF-02.7 | El acceso denegado debe redirigir a `/Identity/Account/AccessDenied` | `Program.cs` — `AccessDeniedPath` |

---

### RF-03 — Gestión de Categorías

**Prioridad:** Media | **Estado:** Implementado y funcional

| ID | Requisito | Fuente en código |
|----|-----------|-----------------|
| RF-03.1 | El sistema debe permitir crear, editar, eliminar y listar categorías (CRUD completo) | `CategoriasController` + `IUnitOfWork.Categorias` |
| RF-03.2 | Una categoría tiene: Id (PK), Nombre (requerido) | `Categoria.cs` |
| RF-03.3 | No se puede eliminar una categoría si tiene prendas asociadas | Regla de negocio — FK constraint en BD |
| RF-03.4 | Las categorías deben cargarse en selectores de formularios de prendas | `PrendasController.CargarCategorias()` — `ViewBag.Categorias` |
| RF-03.5 | El mapeo `Categoria → CategoriaDTO` debe ser bidireccional | `MappingProfile.cs` — `CreateMap<Categoria, CategoriaDTO>().ReverseMap()` |

---

### RF-04 — Gestión de Prendas e Inventario

**Prioridad:** Alta | **Estado:** CRUD funcional, autorización corregida (P-05)

| ID | Requisito | Fuente en código |
|----|-----------|-----------------|
| RF-04.1 | El sistema debe permitir crear, editar, eliminar y listar prendas (CRUD completo) | `PrendasController.cs` |
| RF-04.2 | Una prenda tiene: Id, Nombre (req, max 150), Descripcion (max 300), Talla (req, max 50), Color (req, max 50), Precio (1-99999), Stock (0-10000), ImagenUrl, CategoriaId | `Prenda.cs` |
| RF-04.3 | El campo `Disponibilidad` es calculado: `true` si `Stock > 0` (no mapeado a BD) | `Prenda.cs` — `[NotMapped] Disponibilidad` |
| RF-04.4 | El campo `EstaAgotandose` es calculado: `true` si `Stock > 0 && Stock <= 5` (no mapeado a BD) | `Prenda.cs` — `[NotMapped] EstaAgotandose` |
| RF-04.5 | Al crear una prenda, se puede subir una imagen (jpg, jpeg, png, gif, webp) que se guarda en `wwwroot/images/` | `PrendasController.Create()` |
| RF-04.6 | El nombre del archivo de imagen debe generarse con `Guid.NewGuid()` para evitar colisiones | `PrendasController.Create()` |
| RF-04.7 | El DTO `PrendaDTO` debe incluir `CategoriaNombre` mapeado desde `Categoria.Nombre` | `PrendaDTO.cs` + `MappingProfile.cs` |
| RF-04.8 | El DTO `PrendaDTO` debe exponer `Categoria` como propiedad calculada `CategoriaInfo` para compatibilidad con vistas | `PrendaDTO.cs` — `CategoriaInfo` |
| RF-04.9 | Todos los endpoints de `/Prendas/*` deben requerir autenticación | `PrendasController.cs` — `[Authorize]` **[P-05 corregido]** |

---

### RF-05 — Gestión de Clientes

**Prioridad:** Media | **Estado:** Implementado y funcional

| ID | Requisito | Fuente en código |
|----|-----------|-----------------|
| RF-05.1 | El sistema debe permitir crear, editar, eliminar y listar clientes (CRUD completo) | `ClientesController.cs` |
| RF-05.2 | Un cliente tiene: Id, NombreCompleto (req), DNI (req, max 8 chars), Telefono, Direccion | `Cliente.cs` |
| RF-05.3 | `ClienteDTO` debe exponer alias `Nombre` que retorna `NombreCompleto` (compatibilidad con vistas) | `ClienteDTO.cs` — `public string Nombre => NombreCompleto` |
| RF-05.4 | `ClienteDTO` debe exponer alias `Email` que retorna `DNI` (compatibilidad con vistas de identificación) | `ClienteDTO.cs` — `public string Email => DNI` |
| RF-05.5 | El mapeo `Cliente → ClienteDTO` debe ser bidireccional | `MappingProfile.cs` — `CreateMap<Cliente, ClienteDTO>().ReverseMap()` |
| RF-05.6 | No se puede eliminar un cliente si tiene ventas registradas | Regla de negocio — FK constraint en BD |

---

### RF-06 — Registro de Ventas

**Prioridad:** Alta | **Estado:** Implementado — transacción corregida (P-04), UnitOfWork aplicado (P-03)

| ID | Requisito | Fuente en código |
|----|-----------|-----------------|
| RF-06.1 | Una venta tiene: Id, Fecha (auto), ClienteId, VendedorId, MetodoPagoId, Total, DetalleVentas | `Venta.cs` |
| RF-06.2 | El registro de venta se realiza mediante `POST /api/registrar-venta` con payload `RegistrarVentaRequest` | `VentasController.ApiRegistrarVenta()` |
| RF-06.3 | El payload debe incluir: ClienteId, VendedorId, MetodoPagoId y lista de `Detalles` (PrendaId, Cantidad, Precio) | `RegistrarVentaRequest.cs` |
| RF-06.4 | El total de la venta se calcula automáticamente como `SUM(Precio × Cantidad)` de los detalles | `VentasController.RegistrarVentaInterno()` |
| RF-06.5 | Toda la operación de registro (venta + detalles + actualización de stock) debe ejecutarse en **una sola transacción** | `VentasController.cs` — `BeginTransactionAsync/CommitAsync/RollbackAsync` **[P-04 corregido]** |
| RF-06.6 | Si cualquier operación dentro de la transacción falla, debe revertirse completamente (rollback) | `VentasController.RegistrarVentaInterno()` — bloque catch |
| RF-06.7 | La validación de la venta debe ejecutarse **antes** de iniciar el commit de datos | `ValidarVentaInterno()` llamado al inicio |
| RF-06.8 | El Id del usuario autenticado debe registrarse en el contexto de la operación | `ApiRegistrarVenta()` — `User.FindFirst(ClaimTypes.NameIdentifier)` |
| RF-06.9 | El CRUD de ventas usa `IUnitOfWork` para operaciones simples (AddAsync, CommitAsync) | `VentasController.cs` **[P-03 corregido]** |

---

### RF-07 — Actualización Automática de Stock

**Prioridad:** Alta | **Estado:** Implementado dentro de transacción (P-04 corregido)

| ID | Requisito | Fuente en código |
|----|-----------|-----------------|
| RF-07.1 | Tras registrar una venta exitosa, el stock de cada prenda vendida debe reducirse por la cantidad vendida | `RegistrarVentaInterno()` — `prenda.Stock -= detalle.Cantidad` |
| RF-07.2 | La actualización de stock debe ocurrir en la misma transacción que el registro de la venta | `RegistrarVentaInterno()` — dentro del bloque `using var transaction` |
| RF-07.3 | El stock nunca puede quedar en valor negativo | `ValidarVentaInterno()` — validación previa de stock |
| RF-07.4 | Si la prenda no se encuentra al actualizar stock, la operación continúa sin error (prenda ya eliminada) | `RegistrarVentaInterno()` — `if (prenda != null)` |
| RF-07.5 | La actualización de stock usa `IUnitOfWork.Prendas.GetByIdAsync()` y `Update()` | `VentasController.cs` **[P-03 corregido]** |

---

### RF-08 — Validación de Stock Insuficiente

**Prioridad:** Alta | **Estado:** Implementado en `ValidarVentaInterno()`

| ID | Requisito | Fuente en código |
|----|-----------|-----------------|
| RF-08.1 | Antes de registrar una venta, el sistema debe validar que el stock de cada prenda sea suficiente | `ValidarVentaInterno()` |
| RF-08.2 | Si el stock es insuficiente, debe retornar mensaje descriptivo: `"Stock insuficiente para {nombre}. Disponible: {stock}, Solicitado: {cantidad}"` | `ValidarVentaInterno()` |
| RF-08.3 | Si el cliente no existe, debe retornar: `"Cliente no encontrado"` | `ValidarVentaInterno()` |
| RF-08.4 | Si el vendedor no existe, debe retornar: `"Vendedor no encontrado"` | `ValidarVentaInterno()` |
| RF-08.5 | Si el método de pago no existe, debe retornar: `"Método de pago no encontrado"` | `ValidarVentaInterno()` |
| RF-08.6 | Si la lista de detalles está vacía, debe retornar: `"La venta debe contener al menos un producto"` | `ValidarVentaInterno()` y `ApiRegistrarVenta()` |
| RF-08.7 | La validación también está disponible como endpoint separado: `POST /api/validar-venta` | `VentasController.ApiValidarVenta()` |
| RF-08.8 | Las consultas de validación usan `IUnitOfWork.*.GetByIdAsync()` | `ValidarVentaInterno()` **[P-03 corregido]** |

---

### RF-09 — Métodos de Pago

**Prioridad:** Media | **Estado:** Implementado como catálogo fijo

| ID | Requisito | Fuente en código |
|----|-----------|-----------------|
| RF-09.1 | Los métodos de pago son un catálogo administrado (no hardcoded) en la BD | `IUnitOfWork.MetodosPago` — `IGenericRepository<MetodoPago>` |
| RF-09.2 | Cada venta debe asociarse a un método de pago válido (validado en `ValidarVentaInterno`) | `ValidarVentaInterno()` — `GetByIdAsync(metodoPagoId)` |
| RF-09.3 | El mapeo `MetodoPago → MetodoPagoDTO` debe ser bidireccional | `MappingProfile.cs` — `CreateMap<MetodoPago, MetodoPagoDTO>().ReverseMap()` |
| RF-09.4 | El dashboard de ventas muestra ingresos agrupados por método de pago | `HomeController.Index()` — `RevenueByMethodLabelsJson` / `RevenueByMethodDataJson` |

---

### RF-10 — Dashboards con Datos Reales

**Prioridad:** Alta | **Estado:** Implementado — HomeController refactorizado (P-06 corregido)

| ID | Requisito | Fuente en código |
|----|-----------|-----------------|
| RF-10.1 | El Dashboard principal (`/Home/Index`) muestra: TotalPrendas, TotalCategorias, TotalClientes, TotalVentas, TotalUsuarios, TotalStock, TotalIngresos | `DashboardViewModel.cs` + `HomeController.Index()` |
| RF-10.2 | TotalUsuarios se obtiene de `_context.Users.CountAsync()` (ASP.NET Identity, sin repositorio) | `HomeController.Index()` — excepción documentada |
| RF-10.3 | El dashboard muestra gráfico de ventas de los últimos 6 meses con labels `"MMM yyyy"` | `HomeController.Index()` — `SalesChartLabelsJson/DataJson` |
| RF-10.4 | El dashboard muestra gráfico de ventas de los últimos 7 días con labels `"ddd dd/MMM"` | `HomeController.Index()` — `WeeklySalesLabelsJson/DataJson` |
| RF-10.5 | El dashboard muestra prendas por categoría (gráfico de barras/dona) | `HomeController.Index()` — `PrendasByCategoryLabelsJson/DataJson` |
| RF-10.6 | El dashboard muestra Top 10 productos más vendidos por cantidad | `HomeController.Index()` — `TopProductsLabelsJson/DataJson` + `TopSellingProducts` |
| RF-10.7 | El dashboard muestra ingresos por método de pago | `HomeController.Index()` — `RevenueByMethodLabelsJson/DataJson` |
| RF-10.8 | `PrendasAgotandose` muestra hasta 10 prendas con stock entre 1 y 5, usando `IUnitOfWork.Prendas.FindAsync()` | `HomeController.Index()` — `FindAsync(p => p.Stock > 0 && p.Stock <= 5)` |
| RF-10.9 | `Prendas` muestra las últimas 6 prendas registradas con sus categorías (requiere Include) | `HomeController.Index()` — `_context.Set<Prenda>().Include(p => p.Categoria)` |
| RF-10.10 | `RecentSales` muestra las últimas 10 ventas ordenadas por fecha descendente | `HomeController.Index()` — `allVentas.OrderByDescending(v => v.Fecha).Take(10)` |
| RF-10.11 | `RecentClients` muestra los últimos 8 clientes registrados | `HomeController.Index()` — `allClientes.OrderByDescending(c => c.Id).Take(8)` |
| RF-10.12 | Los totales simples (categorías, prendas, clientes, ventas, stock, ingresos) usan `IUnitOfWork` | `HomeController.Index()` **[P-06 corregido]** |

---

### RF-11 — Reportes Administrativos Básicos

**Prioridad:** Media | **Estado:** Datos disponibles en dashboards de controladores

| ID | Requisito | Fuente en código |
|----|-----------|-----------------|
| RF-11.1 | El dashboard de ventas (`/Ventas/Dashboard`) muestra: TotalVentas, TotalIngresos, TotalClientes únicos, ventas por mes, ventas por categoría | `VentasController.Dashboard()` |
| RF-11.2 | El dashboard de prendas (`/Prendas/Dashboard`) muestra: cantidad de prendas por categoría y stock total por categoría | `PrendasController.Dashboard()` |
| RF-11.3 | El endpoint `GET /api/productos-agotandose` retorna prendas con stock entre 1 y 5, marcando "Crítico" si stock ≤ 2 | `VentasController.ApiProductosAgotandose()` |
| RF-11.4 | El endpoint `GET /api/productos-disponibles` retorna todas las prendas con stock > 0 | `VentasController.ApiProductosDisponibles()` — `[AllowAnonymous]` |
| RF-11.5 | El endpoint `GET /api/buscar/{nombre}` busca prendas disponibles por nombre | `VentasController.ApiBuscar()` — `[AllowAnonymous]` |

---

### RF-12 — Pruebas Unitarias y de Integración

**Prioridad:** Media | **Estado:** 290 tests existentes pasan — advertencias CS8625 en tests

| ID | Requisito | Fuente en código |
|----|-----------|-----------------|
| RF-12.1 | El proyecto `FashionStore.Tests` debe compilar sin errores | Verificado: `dotnet build` → 0 errores |
| RF-12.2 | Los tests usan MSTest como framework | `FashionStore.Tests.csproj` |
| RF-12.3 | Los tests de controladores usan Moq para mockear dependencias | `PrendasControllerTests.cs` |
| RF-12.4 | Los tests de entidades y DTOs validan propiedades y validaciones de datos | `ClienteEntityTests.cs`, `ClienteDTOTests.cs` |
| RF-12.5 | El comando `dotnet test` debe ejecutarse sin fallos — 290 tests deben pasar | Verificado: `Passed! 290/290` |
| RF-12.6 | Las advertencias CS8625 en tests (null literals) no bloquean ejecución pero deben corregirse | `ClienteDTOTests.cs`, `PrendasControllerTests.cs` |
| RF-12.7 | Las advertencias MSTEST0032 en tests (assertions always true) deben revisarse | `VentaEntityTests.cs`, `RolEntityTests.cs` |

---

### RF-13 — Repository Pattern y Unit of Work

**Prioridad:** Alta | **Estado:** Implementado — controladores refactorizados (P-03, P-06 corregidos)

| ID | Requisito | Fuente en código |
|----|-----------|-----------------|
| RF-13.1 | `IGenericRepository<T>` expone: `GetAllAsync()`, `GetByIdAsync(int)`, `AddAsync(T)`, `Update(T)`, `Delete(T)`, `FindAsync(Expression)` | `IGenericRepository.cs` |
| RF-13.2 | `IUnitOfWork` expone repositorios para: Categorias, Prendas, Clientes, Vendedores, Ventas, DetalleVentas, MetodosPago, Configuraciones, ConfiguracionesAuditoria | `IUnitOfWork.cs` |
| RF-13.3 | `IUnitOfWork.CommitAsync()` persiste todos los cambios pendientes en la BD | `UnitOfWork.cs` |
| RF-13.4 | Los controladores deben inyectar y usar `IUnitOfWork` para todas las operaciones CRUD | `VentasController`, `HomeController`, `PrendasController`, `ClientesController` **[P-03/P-06 corregidos]** |
| RF-13.5 | Excepción permitida: `_context.Users` (Identity) y consultas que requieren `Include()` profundo pueden usar `DbContext` directamente | `HomeController.cs` — documentado con comentarios |
| RF-13.6 | Las transacciones explícitas (`BeginTransactionAsync`) requieren acceso directo al `DbContext` — esta es la segunda excepción permitida | `VentasController.RegistrarVentaInterno()` |
| RF-13.7 | `GenericRepository<T>` implementa todas las operaciones de `IGenericRepository<T>` usando `DbSet<T>` | `GenericRepository.cs` |

---

### RF-14 — Inicialización de Roles Base

**Prioridad:** Alta | **Estado:** Implementado e invocado (P-01 corregido)

| ID | Requisito | Fuente en código |
|----|-----------|-----------------|
| RF-14.1 | Al iniciar la aplicación, se deben crear los roles `Administrador` y `Vendedor` si no existen | `DbInitializer.SeedRoles()` |
| RF-14.2 | La invocación de `DbInitializer.Initialize(app)` se realiza en `Program.cs` antes de `app.Run()` | `Program.cs` — `await FashionStore.Infrastructure.Data.DbInitializer.Initialize(app)` **[P-01 corregido]** |
| RF-14.3 | El seeding es idempotente: usa `RoleExistsAsync()` antes de `CreateAsync()` | `DbInitializer.SeedRoles()` |
| RF-14.4 | Errores en el seeding se registran en el log sin interrumpir el inicio de la aplicación | `DbInitializer.Initialize()` — bloque `catch` |

---

## 4. REQUISITOS NO FUNCIONALES

### RNF-01 — Seguridad

| ID | Requisito | Estado |
|----|-----------|--------|
| RNF-01.1 | Contraseñas almacenadas con hash (mecanismo por defecto de ASP.NET Identity) | ✅ Implementado |
| RNF-01.2 | Formularios con `[ValidateAntiForgeryToken]` en operaciones de escritura | ✅ Presente en Create/Edit/Delete |
| RNF-01.3 | Todos los controladores MVC deben requerir autenticación | ✅ Corregido (P-05) |
| RNF-01.4 | Roles inicializados automáticamente para que la autorización funcione desde la primera ejecución | ✅ Corregido (P-01) |

### RNF-02 — Integridad de Datos

| ID | Requisito | Estado |
|----|-----------|--------|
| RNF-02.1 | El registro de venta con actualización de stock debe ser atómico (transacción) | ✅ Corregido (P-04) |
| RNF-02.2 | El stock nunca puede quedar negativo | ✅ Validado en `ValidarVentaInterno()` |
| RNF-02.3 | Las relaciones FK (Cliente, Vendedor, MetodoPago) deben validarse antes de persistir | ✅ Validado en `ValidarVentaInterno()` |

### RNF-03 — Mantenibilidad

| ID | Requisito | Estado |
|----|-----------|--------|
| RNF-03.1 | Arquitectura por capas: Domain → Infrastructure → Web | ✅ Implementado |
| RNF-03.2 | El acceso a datos debe pasar por `IUnitOfWork` (excepto `_context.Users` e `Include()` profundo) | ✅ Corregido (P-03, P-06) |
| RNF-03.3 | Mapeo entre entidades y DTOs mediante AutoMapper | ✅ `MappingProfile.cs` |
| RNF-03.4 | No debe haber `using` duplicados en el código | ✅ Corregido (P-07) |
| RNF-03.5 | No usar operador null-forgiving (`!`) sin validación previa en lambdas EF | ✅ Corregido (P-08) |

### RNF-04 — Calidad de Build

| ID | Requisito | Estado |
|----|-----------|--------|
| RNF-04.1 | `dotnet build` debe completarse con 0 errores | ✅ Verificado |
| RNF-04.2 | `dotnet test` debe completarse con 0 fallos (290 tests) | ✅ Verificado |
| RNF-04.3 | Advertencias en `FashionStore.Web` reducidas al mínimo | ✅ Reducidas de 3 a 1 |

---

## 5. REGLAS DE NEGOCIO

| ID | Regla | Implementación |
|----|-------|---------------|
| RN-01 | No se puede registrar una venta si el stock de cualquier prenda es insuficiente | `ValidarVentaInterno()` |
| RN-02 | El stock de una prenda nunca puede ser negativo | Validación previa en `ValidarVentaInterno()` |
| RN-03 | No se puede eliminar una categoría si tiene prendas asociadas | FK constraint en BD |
| RN-04 | No se puede eliminar un cliente si tiene ventas registradas | FK constraint en BD |
| RN-05 | El total de una venta = `SUM(Precio × Cantidad)` de sus detalles | `RegistrarVentaInterno()` |
| RN-06 | Cada venta debe tener al menos un detalle (mínimo un producto) | `ApiRegistrarVenta()` + `ValidarVentaInterno()` |
| RN-07 | Una prenda con stock entre 1 y 5 se considera "agotándose" | `Prenda.EstaAgotandose` + `FindAsync(p => p.Stock <= 5)` |
| RN-08 | Una prenda con stock = 0 está "agotada" y no aparece en productos disponibles para venta | `ApiProductosDisponibles()` — `Where(p => p.Stock > 0)` |
| RN-09 | Una prenda con stock ≤ 2 se marca como "Crítico" en el API de productos agotándose | `ApiProductosAgotandose()` — `Estado = p.Stock <= 2 ? "Crítico" : "Bajo"` |
| RN-10 | Los roles son fijos: `Administrador` y `Vendedor` | `DbInitializer.SeedRoles()` |
| RN-11 | La sesión de usuario persiste 30 días con deslizamiento | `Program.cs` — `ExpireTimeSpan = TimeSpan.FromDays(30)` |
| RN-12 | El seeding de roles es idempotente (no falla si ya existen) | `DbInitializer.SeedRoles()` — `RoleExistsAsync` |
| RN-13 | El DNI del cliente tiene máximo 8 caracteres | `ClienteDTO.cs` — `[StringLength(8)]` |
| RN-14 | Venta + detalles + actualización de stock son atómicos (todo o nada) | `RegistrarVentaInterno()` — transacción explícita |

---

## 6. CASOS DE USO PRINCIPALES

### CU-01 — Autenticar Usuario

**Actor:** Administrador, Vendedor  
**Precondición:** El usuario tiene credenciales válidas en la BD  
**Trigger:** Usuario accede a cualquier ruta protegida o a `/`

**Flujo Principal:**
1. Usuario no autenticado accede a `/`
2. Middleware de redirección detecta `IsAuthenticated = false` (middleware ejecutado DESPUÉS de `UseAuthentication`)
3. Sistema redirige a `/Identity/Account/Login`
4. Usuario ingresa email y contraseña
5. ASP.NET Identity valida las credenciales
6. Sistema crea cookie de sesión con `ExpireTimeSpan = 30 días`
7. Sistema redirige a `/Home/Index`

**Flujo Alternativo 5a:** Credenciales incorrectas → muestra error en formulario, permanece en Login

**Postcondición:** Usuario autenticado con rol asignado, sesión activa 30 días

---

### CU-02 — Registrar Venta

**Actor:** Vendedor, Administrador  
**Precondición:** Usuario autenticado, existen prendas con stock > 0, al menos un cliente y un vendedor  
**Trigger:** `POST /api/registrar-venta` con `RegistrarVentaRequest`

**Flujo Principal:**
1. Cliente envía payload con ClienteId, VendedorId, MetodoPagoId y lista de detalles
2. Sistema valida que la lista no esté vacía
3. `ValidarVentaInterno()` verifica: cliente existe, vendedor existe, método de pago existe
4. Para cada detalle: verifica que la prenda exista y tenga stock suficiente
5. Si validación exitosa → inicia transacción (`BeginTransactionAsync`)
6. Crea entidad `Venta` con total calculado y persiste con `CommitAsync()`
7. Para cada detalle: crea `DetalleVenta` y reduce stock de la prenda
8. Persiste detalles y actualizaciones de stock con `CommitAsync()`
9. Confirma transacción (`CommitAsync` de la transaction)
10. Retorna `{ exito: true, ventaId: N }`

**Flujo Alternativo 4a:** Stock insuficiente → retorna `{ exito: false, mensaje: "Stock insuficiente para X..." }`, sin iniciar transacción

**Flujo Alternativo 8a:** Error en cualquier operación → `RollbackAsync()`, retorna `{ exito: false, mensaje: "..." }`

**Postcondición:** Venta registrada, detalles guardados, stock actualizado, todo en la misma transacción

---

### CU-03 — Consultar Dashboard Principal

**Actor:** Administrador, Vendedor  
**Precondición:** Usuario autenticado  
**Trigger:** GET `/Home/Index`

**Flujo Principal:**
1. `HomeController.Index()` verifica autenticación
2. Consulta totales via `IUnitOfWork`: categorías, prendas, clientes, ventas, stock, ingresos
3. Consulta `TotalUsuarios` via `_context.Users.CountAsync()` (Identity)
4. Genera datos JSON para gráficos: ventas 6 meses, ventas 7 días, prendas por categoría, top 10 productos, ingresos por método de pago
5. Carga `PrendasAgotandose` via `FindAsync(p => p.Stock > 0 && p.Stock <= 5)`
6. Carga `Prendas` destacadas (últimas 6) con Include de Categoria
7. Carga últimas 10 ventas y últimos 8 clientes
8. Retorna View con `DashboardViewModel` completo

**Postcondición:** Dashboard visible con todos los datos en tiempo real

---

### CU-04 — Gestionar Prendas (CRUD)

**Actor:** Administrador, Vendedor  
**Precondición:** Usuario autenticado (`[Authorize]` en `PrendasController`)  
**Trigger:** Navegación a `/Prendas/*`

**Flujo Crear:**
1. Usuario navega a `/Prendas/Create`
2. Sistema carga categorías disponibles en `ViewBag.Categorias`
3. Usuario completa formulario y opcionalmente sube imagen
4. Si imagen válida: genera nombre con `Guid.NewGuid()`, guarda en `wwwroot/images/`
5. Mapea `PrendaDTO → Prenda` via AutoMapper
6. Persiste con `IUnitOfWork.Prendas.AddAsync()` + `CommitAsync()`
7. Redirige a `/Prendas/Index`

**Flujo Alternativo 4a:** ModelState inválido → muestra errores, recarga categorías, permanece en formulario

---

## 7. CRITERIOS DE ACEPTACIÓN VERIFICABLES

### Autenticación y Autorización

| ID | Criterio | Método | Estado |
|----|----------|--------|--------|
| CA-01 | Usuario no autenticado que accede a `/` es redirigido a login | Navegar a `/` sin cookie | ✅ Corregido (P-02) |
| CA-02 | Usuario autenticado que accede a `/` es redirigido a `/Home/Index` | Login → navegar a `/` | ✅ Corregido (P-02) |
| CA-03 | Navegar a `/Prendas` sin login redirige a login | Navegación manual | ✅ Corregido (P-05) |
| CA-04 | Query `SELECT COUNT(*) FROM AspNetRoles` retorna 2 tras primera ejecución | SQL directo | ✅ Corregido (P-01) |
| CA-05 | Build: `dotnet build` → 0 errores, 0 advertencias CS0105 | Terminal | ✅ Verificado |

### Ventas e Inventario

| ID | Criterio | Método | Estado |
|----|----------|--------|--------|
| CA-06 | Venta con stock insuficiente retorna `{ exito: false }` con mensaje descriptivo | POST `/api/validar-venta` | ✅ Implementado |
| CA-07 | Tras venta exitosa, stock de cada prenda se reduce en la cantidad vendida | Query BD antes/después | ✅ Implementado |
| CA-08 | Si falla actualización de stock, la venta NO queda guardada (rollback) | Test unitario / forzar error | ✅ Corregido (P-04) |
| CA-09 | Venta sin detalles retorna `{ exito: false, mensaje: "...al menos un producto" }` | POST con lista vacía | ✅ Implementado |
| CA-10 | Total de venta = `SUM(Precio × Cantidad)` de todos los detalles | Comparar respuesta con cálculo manual | ✅ Implementado |

### Dashboard

| ID | Criterio | Método | Estado |
|----|----------|--------|--------|
| CA-11 | Dashboard muestra conteos correctos de prendas, clientes, ventas | Comparar con `SELECT COUNT(*)` | ✅ Usa IUnitOfWork (P-06) |
| CA-12 | Dashboard muestra prendas con stock entre 1 y 5 en sección "agotándose" | Insertar prenda stock=3, verificar aparece | ✅ Implementado |
| CA-13 | `DashboardViewModel.PrendasAgotandose` es de tipo `IList<PrendaDTO>` (permite `.Count`) | Compilación + inspección | ✅ Implementado |
| CA-14 | `DashboardViewModel.Prendas` es de tipo `IList<PrendaDTO>` | Compilación + inspección | ✅ Implementado |

### Arquitectura

| ID | Criterio | Método | Estado |
|----|----------|--------|--------|
| CA-15 | `VentasController.ValidarVentaInterno()` usa `_unitOfWork.*.GetByIdAsync()` | Code review | ✅ Corregido (P-03) |
| CA-16 | `HomeController.Index()` usa `_unitOfWork` para totales simples | Code review | ✅ Corregido (P-06) |
| CA-17 | `dotnet test` → 290/290 tests pasan | Terminal | ✅ Verificado |
| CA-18 | No hay `using AutoMapper` duplicado en `ClientesController.cs` | Compilación sin CS0105 | ✅ Corregido (P-07) |

---

## 8. ESTADO DE CORRECCIONES (POST-IMPLEMENTACIÓN)

| ID Plan | Descripción | Estado | Archivo |
|---------|-------------|--------|---------|
| P-01 | DbInitializer no invocado | ✅ CORREGIDO | `Program.cs` |
| P-02 | Middleware auth en orden incorrecto | ✅ CORREGIDO | `Program.cs` |
| P-03 | DbContext inyectado directamente | ✅ CORREGIDO | `VentasController.cs`, `HomeController.cs` |
| P-04 | Registro de venta sin transacción | ✅ CORREGIDO | `VentasController.cs` |
| P-05 | PrendasController sin [Authorize] | ✅ CORREGIDO | `PrendasController.cs` |
| P-06 | HomeController no usa UnitOfWork | ✅ CORREGIDO | `HomeController.cs` |
| P-07 | Using duplicado CS0105 | ✅ CORREGIDO | `ClientesController.cs` |
| P-08 | Null-forgiving operator mal usado | ✅ CORREGIDO | `VentasController.cs` |
| P-09 | Desreferencia null en Layout | ⚠️ PENDIENTE (rechazado por usuario) | `_Layout.cshtml` |

---

## 9. REQUISITOS FUERA DE ALCANCE

| Requisito Excluido | Justificación |
|--------------------|---------------|
| Módulo de administración de usuarios (Identity UI) | Scaffolded Identity UI es suficiente |
| Recuperación de contraseña por email | Requiere SMTP externo |
| Reportes en PDF o Excel | No implementados en base de código |
| Historial de cambios en ventas (ventas no se editan) | Diseño intencional |
| Frontend público de e-commerce | Sistema es administrativo interno |
| API REST con JWT | Endpoints JSON son para uso interno de la UI |
| Notificaciones push o emails automáticos | No implementados |
| Módulo de devoluciones o cancelaciones de venta | No implementado |
| Integración con pasarelas de pago externas | Métodos de pago son catálogo fijo |
| Gestión de proveedores | No existe entidad `Proveedor` |
| Módulo de descuentos o promociones | No implementado |
| Multi-tenant / Multi-tienda | Sistema mono-tienda |
| Modificar `_Layout.cshtml:135` (CS8602) | Rechazado por el usuario — advertencia no crítica |

---

## 10. COMANDOS DE VALIDACIÓN FINAL

```bash
# 1. Compilar solución completa
dotnet build "C:\Users\CRISTIAN\source\repos\FashionStoreSolution\FashionStoreSolution.sln"
# Esperado: Build succeeded. 0 Error(s). 1 Warning(s) [solo _Layout.cshtml]

# 2. Ejecutar todos los tests
dotnet test "C:\Users\CRISTIAN\source\repos\FashionStoreSolution\FashionStore.Tests\FashionStore.Tests.csproj"
# Esperado: Passed! 290 tests - 0 failed

# 3. Verificar roles (tras primera ejecución con BD)
# SELECT * FROM AspNetRoles
# Esperado: 2 registros — Administrador, Vendedor

# 4. Aplicar migraciones (si BD nueva)
dotnet ef database update --project FashionStore.Infrastructure1 --startup-project FashionStore.Web
```

---

**Documento generado el:** 7 de julio de 2026  
**Autor:** Kiro — Analista de Requisitos / SSD  
**Versión:** 2.0 — Basada en código real post-correcciones  
**Estado:** ✅ Sincronizado con implementación actual
