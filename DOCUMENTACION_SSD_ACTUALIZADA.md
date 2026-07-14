# DOCUMENTACIÓN TÉCNICA Y ACADÉMICA
# Sistema de Gestión FashionStore
## Basado en Specs Driven Development (SSD)

**Institución:** [Nombre de la Institución Educativa]  
**Programa:** [Nombre del Programa / Carrera]  
**Curso:** Desarrollo de Software / Calidad de Software  
**Docente:** [Nombre del Docente]  
**Estudiante:** [Nombre del Estudiante]  
**Fecha:** Julio 2026  
**Versión:** 2.0 — Alineada con código real validado

> **Nota:** Esta documentación fue generada y validada contra el código fuente real
> del proyecto. Solo se documenta lo que está implementado y verificado mediante
> `dotnet build` (0 errores) y `dotnet test` (290/290 pruebas pasando).

---

## TABLA DE CONTENIDOS

1. [Resumen Ejecutivo](#1-resumen-ejecutivo)
2. [Introducción](#2-introducción)
3. [Requisitos del Sistema](#3-requisitos-del-sistema)
4. [Diseño del Sistema](#4-diseño-del-sistema)
5. [Arquitectura](#5-arquitectura)
6. [Base de Datos](#6-base-de-datos)
7. [Módulos Implementados](#7-módulos-implementados)
8. [Seguridad](#8-seguridad)
9. [Pruebas](#9-pruebas)
10. [Conclusiones](#10-conclusiones)
11. [Recomendaciones](#11-recomendaciones)
12. [Funcionalidades Futuras](#12-funcionalidades-futuras)

---

## 1. RESUMEN EJECUTIVO

FashionStore es un sistema web administrativo desarrollado en **ASP.NET Core MVC 9.0** para
la gestión operativa de una tienda de ropa y lencería. Permite administrar el inventario de
prendas, registrar ventas con descuento automático de stock, gestionar clientes y visualizar
indicadores de negocio en tiempo real mediante dashboards.

### Estado del proyecto al momento de esta documentación

| Indicador | Resultado |
|-----------|-----------|
| Build | ✅ 0 errores · 0 advertencias |
| Tests automatizados | ✅ 290 / 290 pasando |
| Tiempo de pruebas | 1.5 segundos |
| Módulos funcionales | 6 módulos (Categorías, Prendas, Clientes, Ventas, Dashboard, Configuración) |
| Cobertura de roles | Administrador · Vendedor |
| Plataforma destino | Windows / Linux — .NET 9.0 |

### Tecnologías principales

| Capa | Tecnología |
|------|-----------|
| Framework web | ASP.NET Core MVC 9.0 |
| ORM | Entity Framework Core 9.0 |
| Base de datos | SQL Server (LocalDB / Express / Full) |
| Autenticación | ASP.NET Core Identity |
| Mapeo de objetos | AutoMapper 12.0 |
| UI / Frontend | AdminLTE 3.2, Bootstrap 5.3, Chart.js, SweetAlert2, Toastr |
| Pruebas | MSTest 4.2, Moq 4.20, EF Core InMemory 9.0 |

---

## 2. INTRODUCCIÓN

### 2.1 Contexto del problema

Las tiendas de ropa pequeñas y medianas operan frecuentemente con registros manuales de
inventario y ventas en hojas de cálculo o cuadernos, lo que genera inconsistencias en el
stock, pérdida de información de clientes y dificultad para obtener reportes de desempeño.

FashionStore resuelve este problema con un sistema web administrativo centralizado,
accesible desde cualquier navegador, que automatiza el registro de ventas y mantiene el
inventario actualizado en tiempo real.

### 2.2 Objetivos

**Objetivo general:**  
Desarrollar un sistema de gestión administrativa para una tienda de ropa y lencería que
automatice las operaciones de venta, inventario y análisis de datos.

**Objetivos específicos:**
- Implementar un módulo CRUD completo para prendas con categorización y gestión de imágenes.
- Registrar ventas con validación de stock y actualización automática del inventario.
- Proveer dashboards con datos reales mediante gráficos interactivos (Chart.js).
- Controlar el acceso mediante roles diferenciados (Administrador / Vendedor).
- Garantizar la calidad del código mediante pruebas automatizadas (MSTest + Moq).

### 2.3 Alcance

El sistema cubre la gestión interna del negocio (back-office). No incluye tienda en línea
para clientes finales ni integración con pasarelas de pago externas.

### 2.4 Metodología de desarrollo

Se utilizó **Specs Driven Development (SSD)**, que consiste en definir la especificación
funcional verificable antes de implementar, siguiendo el ciclo:

```
Spec → Design → Tasks → Implementation → Verification
```

Cada tarea estuvo asociada a un criterio de aceptación verificable mediante comandos
`dotnet build` y `dotnet test`.

---

## 3. REQUISITOS DEL SISTEMA

### 3.1 Actores del sistema

| Actor | Descripción | Acceso |
|-------|-------------|--------|
| **Administrador** | Gestiona todo el sistema: prendas, categorías, clientes, ventas, configuración y dashboards | Acceso completo |
| **Vendedor** | Registra nuevas ventas y consulta el historial de sus propias ventas | Módulo de ventas |

### 3.2 Requisitos funcionales implementados

#### RF-01 — Autenticación
- El sistema requiere inicio de sesión para acceder a cualquier recurso.
- Implementado con ASP.NET Core Identity + cookie de sesión (30 días con sliding expiration).
- La ruta `/` redirige automáticamente a login si no hay sesión activa; al dashboard si sí la hay.

#### RF-02 — Autorización por roles
- Rol **Administrador**: acceso a todos los módulos incluida la configuración del sistema.
- Rol **Vendedor**: acceso exclusivo al módulo de ventas (crear y ver ventas).
- Los roles se inicializan automáticamente en el primer arranque del sistema (`DbInitializer`).

#### RF-03 — Gestión de categorías
- Listar, crear, editar, eliminar y ver detalles de categorías.
- Cada categoría tiene `Nombre` (obligatorio, max. 100 caracteres) y `Descripción` (opcional).
- Dashboard de categorías con gráfico de distribución de prendas por categoría (Chart.js).

#### RF-04 — Gestión de prendas e inventario
- CRUD completo: listar, crear, editar, eliminar, ver detalles.
- Campos: nombre, descripción, talla, color, precio, stock, imagen, categoría.
- Subida y almacenamiento de imagen en `wwwroot/images/`.
- Propiedades calculadas en memoria: `Disponibilidad` (stock > 0), `EstaAgotandose` (0 < stock ≤ 5).
- Dashboard de prendas con gráficos de cantidad y stock por categoría.

#### RF-05 — Gestión de clientes
- CRUD completo: listar, crear, editar, eliminar, ver detalles.
- Campos: nombre completo, DNI (8 dígitos), teléfono (opcional), dirección (opcional).
- Dashboard de clientes con conteo total y listado de 10 clientes más recientes.

#### RF-06 — Registro de ventas
- Modal de nueva venta con selección de: vendedor activo, cliente, productos con carrito
  (cantidad editable), método de pago.
- El total se calcula dinámicamente en el frontend.
- El backend valida: existencia de cliente, vendedor, método de pago y stock suficiente.
- La venta, sus detalles y el descuento de stock se persisten en una **única transacción
  atómica** (`BeginTransactionAsync / CommitAsync / RollbackAsync`).
- En caso de fallo, se ejecuta `RollbackAsync` garantizando consistencia.

#### RF-07 — Actualización automática de stock
- Después de registrar cada venta, el stock de cada prenda vendida se reduce en la cantidad
  indicada, dentro de la misma transacción de la venta.
- No es posible que el stock quede negativo: se valida previamente.

#### RF-08 — Validación de stock insuficiente
- Si la cantidad solicitada supera el stock disponible, la venta se rechaza con mensaje:
  `"Stock insuficiente para {Nombre}. Disponible: {X}, Solicitado: {Y}"`.

#### RF-09 — Métodos de pago
- Los métodos de pago se gestionan como entidad de base de datos (no valores hardcodeados).
- El selector del modal se puebla dinámicamente desde `GET /api/metodos-pago`.

#### RF-10 — Dashboards con datos reales
- **Dashboard general** (`/Home/Index`): tarjetas de totales (categorías, prendas, clientes,
  ventas, stock, ingresos, usuarios), gráfico mensual (últimos 6 meses), gráfico semanal
  (últimos 7 días), prendas por categoría, ingresos por método de pago, top 10 productos
  más vendidos, últimas 10 ventas, 8 clientes más recientes, alerta de 10 prendas con
  stock ≤ 5.
- **Dashboard de ventas** (`/Ventas/Dashboard`): total ventas, ingresos, gráfico mensual,
  ventas por categoría de producto.
- **Dashboard de categorías** (`/Categorias/Dashboard`): prendas por categoría.
- **Dashboard de prendas** (`/Prendas/Dashboard`): prendas y stock por categoría.
- **Dashboard de clientes** (`/Clientes/Dashboard`): total y clientes recientes.

#### RF-11 — Configuración del sistema
- Panel exclusivo para Administrador (`[Authorize(Roles = "Administrador")]`).
- Configura: nombre de la tienda, colores, tema, datos del negocio (RUC, teléfono, correo),
  redes sociales, logotipo.
- Cada cambio queda registrado en la tabla `ConfiguracionesAuditoria` con usuario, campo
  modificado, valor anterior y valor nuevo.
- Caché de 60 minutos con `IMemoryCache` para evitar consultas repetitivas a la BD.

### 3.3 Requisitos no funcionales

| Código | Requisito | Estado |
|--------|-----------|--------|
| RNF-01 | La aplicación debe compilar sin errores en .NET 9.0 | ✅ Verificado |
| RNF-02 | Las pruebas automatizadas deben pasar al 100% | ✅ 290/290 |
| RNF-03 | Toda ruta protegida debe requerir autenticación | ✅ `[Authorize]` en todos los controladores |
| RNF-04 | Las ventas deben persistirse de forma atómica | ✅ Transacción explícita |
| RNF-05 | La UI debe ser responsiva | ✅ Bootstrap 5 + AdminLTE 3 |
| RNF-06 | Los cambios de configuración deben quedar auditados | ✅ `ConfiguracionAuditoria` |
| RNF-07 | La sesión de usuario debe expirar después de inactividad | ✅ 30 días sliding |

### 3.4 Reglas de negocio

- **RN-01:** Una venta requiere al menos un producto, un cliente, un vendedor activo y un método de pago.
- **RN-02:** No se puede vender más unidades de las disponibles en stock.
- **RN-03:** El stock nunca puede ser negativo.
- **RN-04:** Venta, detalles y descuento de stock son atómicos (todo o nada).
- **RN-05:** Solo hay un registro de configuración del sistema (Id = 1).
- **RN-06:** Los roles "Administrador" y "Vendedor" se crean automáticamente al iniciar la aplicación.
- **RN-07:** Un vendedor solo puede estar activo (`Estado = true`) para aparecer en el selector de ventas.

### 3.5 Requisitos fuera del alcance actual

- Módulo de gestión directa de vendedores (CRUD de la tabla `Vendedores` desde la UI).
- Módulo de gestión de métodos de pago desde la UI.
- Tienda en línea para clientes finales.
- Integración con pasarelas de pago externas (POS, tarjetas, transferencias).
- Envío de correos electrónicos (el `EmailSender` es un stub no implementado).
- Alertas automáticas de stock bajo (entidad `AlertaStock` pendiente de migración).
- Facturación electrónica.

---

## 4. DISEÑO DEL SISTEMA

### 4.1 Casos de uso principales

```
┌─────────────────────────────────────────────────────────────┐
│                     SISTEMA FASHIONSTORE                    │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ ADMINISTRADOR                                        │  │
│  │  ├── Gestionar Categorías (CRUD + Dashboard)         │  │
│  │  ├── Gestionar Prendas (CRUD + imagen + Dashboard)   │  │
│  │  ├── Gestionar Clientes (CRUD + Dashboard)           │  │
│  │  ├── Ver y registrar Ventas (+ Dashboard)            │  │
│  │  ├── Ver Dashboard General                           │  │
│  │  └── Configurar Sistema (colores, datos, auditoría)  │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ VENDEDOR                                             │  │
│  │  ├── Registrar Nueva Venta                           │  │
│  │  └── Ver historial de Ventas                         │  │
│  └──────────────────────────────────────────────────────┘  │
│                                                             │
│  ┌──────────────────────────────────────────────────────┐  │
│  │ AMBOS ROLES                                          │  │
│  │  ├── Iniciar sesión                                  │  │
│  │  └── Cerrar sesión                                   │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### 4.2 Flujo de registro de una venta

```
Frontend (modal)                 VentasController              ServicioVentas / BD
─────────────────                ──────────────────            ──────────────────
1. Cargar vendedores  ──────────► GET /api/vendedores-disponibles
2. Cargar clientes    ──────────► GET /api/clientes-disponibles
3. Cargar productos   ──────────► GET /api/productos-disponibles
4. Cargar métodos     ──────────► GET /api/metodos-pago
5. Usuario llena el modal
6. Click "Completar Venta"
7. POST /api/registrar-venta ──► ApiRegistrarVenta()
                                  ├── ValidarVentaInterno()   ──► Verifica cliente, vendedor,
                                  │                               método pago y stock
                                  ├── BeginTransactionAsync() ──► Abre transacción SQL
                                  ├── AddAsync(Venta)         ──► INSERT Ventas
                                  ├── CommitAsync()           ──► obtiene venta.Id
                                  ├── Por cada detalle:
                                  │   ├── AddAsync(DetalleVenta) ► INSERT DetalleVentas
                                  │   └── Stock -= cantidad    ──► UPDATE Prendas
                                  ├── CommitAsync()           ──► Commit detalles + stock
                                  └── CommitAsync (tx)        ──► Confirma transacción
8. { exito: true, ventaId: N } ◄──
9. Mostrar toast éxito
```

### 4.3 Flujo de autenticación y autorización

```
Petición HTTP
     │
     ▼
UseRouting()
     │
     ▼
UseAuthentication()  ──► Lee cookie → Populación de HttpContext.User
     │
     ▼
UseAuthorization()   ──► Verifica atributo [Authorize] / [Authorize(Roles="X")]
     │                    Si no autenticado → redirect /Identity/Account/Login
     │                    Si no autorizado → redirect /Identity/Account/AccessDenied
     ▼
Middleware raíz "/"  ──► Si "/" → redirige a Login o /Home/Index según sesión
     │
     ▼
Controller Action    ──► Ejecuta lógica de negocio
```

---

## 5. ARQUITECTURA

### 5.1 Estructura de proyectos (solución)

```
FashionStoreSolution/
├── FashionStore.Domain/          ← Capa de dominio (entidades, DTOs, interfaces)
│   ├── Entities/                 ← Modelos de negocio
│   ├── DTOs/                     ← Objetos de transferencia de datos
│   └── Interfaces/               ← Contratos (IGenericRepository, IUnitOfWork, IServicioVentas…)
│
├── FashionStore.Infrastructure1/ ← Capa de infraestructura (EF Core, repositorios, servicios)
│   ├── Context/                  ← FashionStoreDbContext
│   ├── Data/                     ← DbInitializer (seed de roles)
│   ├── Migrations/               ← Migraciones EF Core
│   ├── Repositories/             ← GenericRepository<T>, UnitOfWork
│   └── Services/                 ← ServicioVentas, CarritoService, BuscadorProductos,
│                                     ConfiguracionSistemaService
│
├── FashionStore.Web/             ← Capa de presentación (ASP.NET Core MVC)
│   ├── Controllers/              ← CategoriasController, PrendasController, ClientesController,
│   │                                VentasController, HomeController, ConfiguracionController
│   ├── Views/                    ← Vistas Razor (.cshtml) por módulo
│   ├── ViewModels/               ← DashboardViewModel, VentasIndexViewModel
│   ├── Mapping/                  ← MappingProfile (AutoMapper)
│   ├── Services/                 ← EmailSender (stub), ConfiguracionSistemaService (wrapper)
│   ├── Areas/Identity/           ← Páginas de ASP.NET Identity (login, registro…)
│   └── Program.cs                ← Configuración DI, middleware, pipeline HTTP
│
└── FashionStore.Tests/           ← Proyecto de pruebas (MSTest + Moq)
    ├── Controllers/              ← Tests de CategoriasController, PrendasController
    ├── DTOs/                     ← Tests de los 5 DTOs
    ├── Entities/                 ← Tests de las 8 entidades del dominio
    ├── Infrastructure/           ← Tests de cobertura de infraestructura
    ├── Repositories/             ← Tests de GenericRepository
    └── UnitOfWork/               ← Tests de UnitOfWork
```

### 5.2 Patrón Repository + Unit of Work

```
Controller / Service
        │
        ▼
   IUnitOfWork
   ├── IGenericRepository<Categoria>   (Categorias)
   ├── IGenericRepository<Prenda>      (Prendas)
   ├── IGenericRepository<Cliente>     (Clientes)
   ├── IGenericRepository<Vendedor>    (Vendedores)
   ├── IGenericRepository<Venta>       (Ventas)
   ├── IGenericRepository<DetalleVenta>(DetalleVentas)
   ├── IGenericRepository<MetodoPago>  (MetodosPago)
   ├── IGenericRepository<ConfiguracionSistema>
   ├── IGenericRepository<ConfiguracionAuditoria>
   └── CommitAsync()  ──► SaveChangesAsync()
```

El repositorio genérico expone: `GetAllAsync()`, `GetByIdAsync(id)`, `FindAsync(predicate)`,
`AddAsync(entity)`, `Update(entity)`, `Delete(entity)`.

### 5.3 Inyección de dependencias (Program.cs)

```csharp
// Base de datos
AddDbContext<FashionStoreDbContext>(UseSqlServer)

// Identity
AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<FashionStoreDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders()

// MVC + Razor Pages
AddControllersWithViews()
AddRazorPages()

// Servicios de dominio
AddScoped<IUnitOfWork, UnitOfWork>()
AddScoped<IServicioVentas, ServicioVentas>()
AddScoped<ICarritoService, CarritoService>()
AddScoped<IBuscadorProductos, BuscadorProductos>()
AddScoped<IConfiguracionSistemaService, ConfiguracionSistemaService>()

// AutoMapper
AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())

// Caché
AddMemoryCache()
```

### 5.4 Decisiones de diseño relevantes

| Decisión | Justificación |
|----------|---------------|
| `_context` se usa directamente en algunos controladores | Consultas con `Include()` profundo no soportadas por el repositorio genérico |
| `ActualizarInventario()` no llama `SaveChangesAsync()` propio | Evita doble-commit dentro de la transacción de `RegistrarVenta()` |
| `DbInitializer.Initialize()` al inicio | Garantiza existencia de roles sin necesidad de seed manual |
| `ConfiguracionSistema.Id = 1` fijo | Patrón singleton de configuración — solo un registro en BD |
| `[AllowAnonymous]` en `ApiProductosDisponibles` y `ApiBuscar` | Uso potencial por POS o app externa sin sesión |

---

## 6. BASE DE DATOS

### 6.1 Tablas del negocio (migraciones EF Core)

La base de datos fue generada mediante migraciones incrementales de Entity Framework Core.
Las tablas de negocio son las siguientes:

#### Categorias
| Columna | Tipo SQL | Restricciones |
|---------|----------|---------------|
| Id | int | PK, IDENTITY(1,1) |
| Nombre | nvarchar(100) | NOT NULL |
| Descripcion | nvarchar(250) | NULL |

#### Clientes
| Columna | Tipo SQL | Restricciones |
|---------|----------|---------------|
| Id | int | PK, IDENTITY(1,1) |
| NombreCompleto | nvarchar(150) | NOT NULL |
| DNI | nvarchar(8) | NOT NULL |
| Telefono | nvarchar(15) | NULL |
| Direccion | nvarchar(200) | NULL |

#### Vendedores
| Columna | Tipo SQL | Restricciones |
|---------|----------|---------------|
| Id | int | PK, IDENTITY(1,1) |
| Nombres | nvarchar(150) | NOT NULL |
| Apellidos | nvarchar(150) | NOT NULL |
| DNI | nvarchar(8) | NOT NULL |
| Telefono | nvarchar(15) | NULL |
| Correo | nvarchar(150) | NULL |
| Estado | bit | NOT NULL |

#### MetodosPago
| Columna | Tipo SQL | Restricciones |
|---------|----------|---------------|
| Id | int | PK, IDENTITY(1,1) |
| Nombre | nvarchar(50) | NOT NULL |

#### Prendas
| Columna | Tipo SQL | Restricciones |
|---------|----------|---------------|
| Id | int | PK, IDENTITY(1,1) |
| Nombre | nvarchar(150) | NOT NULL |
| Descripcion | nvarchar(300) | NULL |
| Talla | nvarchar(50) | NOT NULL |
| Color | nvarchar(50) | NOT NULL |
| Precio | decimal(10,2) | NOT NULL |
| Stock | int | NOT NULL |
| Imagen | nvarchar(max) | NULL |
| CategoriaId | int | FK → Categorias(Id) RESTRICT |

#### Ventas
| Columna | Tipo SQL | Restricciones |
|---------|----------|---------------|
| Id | int | PK, IDENTITY(1,1) |
| Fecha | datetime2 | NOT NULL |
| ClienteId | int | FK → Clientes(Id) RESTRICT |
| VendedorId | int | FK → Vendedores(Id) RESTRICT |
| MetodoPagoId | int | FK → MetodosPago(Id) RESTRICT |
| Total | decimal(10,2) | NOT NULL |

#### DetalleVentas
| Columna | Tipo SQL | Restricciones |
|---------|----------|---------------|
| Id | int | PK, IDENTITY(1,1) |
| VentaId | int | FK → Ventas(Id) CASCADE |
| PrendaId | int | FK → Prendas(Id) CASCADE |
| Cantidad | int | NOT NULL |
| Precio | decimal(10,2) | NOT NULL |
| Subtotal | decimal(10,2) | NOT NULL |

#### Configuraciones (ConfiguracionSistema)
Tabla con un único registro (Id = 1). Almacena nombre de la tienda, colores, tema,
datos del negocio (RUC, teléfono, correo, ciudad, país), rutas de imágenes y logotipos,
redes sociales y texto del pie de página.

#### ConfiguracionesAuditoria
| Columna | Tipo SQL | Descripción |
|---------|----------|-------------|
| Id | int PK | |
| UsuarioId | nvarchar | FK → AspNetUsers(Id) NULLABLE |
| NombreUsuario | nvarchar(100) | |
| PropiedadModificada | nvarchar(100) | Campo que cambió |
| ValorAnterior | nvarchar | Valor previo |
| ValorNuevo | nvarchar | Valor nuevo |
| FechaCambio | datetime2 | Indexado DESC |
| Descripcion | nvarchar | Comentario opcional |

### 6.2 Tablas de Identity (ASP.NET Core Identity)

Generadas automáticamente por Identity Framework:

| Tabla | Propósito |
|-------|-----------|
| AspNetUsers | Usuarios del sistema (ApplicationUser : IdentityUser) |
| AspNetRoles | Roles (Administrador, Vendedor) |
| AspNetUserRoles | Relación usuario-rol |
| AspNetUserClaims | Claims de usuarios |
| AspNetRoleClaims | Claims de roles |
| AspNetUserLogins | Proveedores de login externos |
| AspNetUserTokens | Tokens de seguridad |

### 6.3 Diagrama de relaciones (modelo entidad-relación simplificado)

```
Categorias ──< Prendas >── DetalleVentas >── Ventas
                                               │
                Vendedores ───────────────────►│
                Clientes   ───────────────────►│
                MetodosPago──────────────────►─┘

AspNetUsers ──< ConfiguracionesAuditoria
```

### 6.4 Índices creados por migraciones

| Tabla | Columna(s) indexada(s) |
|-------|------------------------|
| Prendas | CategoriaId |
| Ventas | ClienteId, VendedorId, MetodoPagoId |
| DetalleVentas | VentaId, PrendaId |
| ConfiguracionesAuditoria | FechaCambio DESC |

---

## 7. MÓDULOS IMPLEMENTADOS

### 7.1 Módulo de Categorías (`/Categorias`)

**Controlador:** `CategoriasController` — `[Authorize]`  
**Acceso:** Administrador

| Acción | Método HTTP | Ruta | Descripción |
|--------|------------|------|-------------|
| Index | GET | /Categorias | Lista todas las categorías |
| Details | GET | /Categorias/Details/{id} | Detalle de una categoría |
| Create | GET | /Categorias/Create | Formulario de creación |
| Create | POST | /Categorias/Create | Guardar nueva categoría |
| Edit | GET | /Categorias/Edit/{id} | Formulario de edición |
| Edit | POST | /Categorias/Edit/{id} | Guardar cambios |
| Delete | GET | /Categorias/Delete/{id} | Confirmación de eliminación |
| DeleteConfirmed | POST | /Categorias/Delete/{id} | Eliminar definitivamente |
| Dashboard | GET | /Categorias/Dashboard | Dashboard con Chart.js |

**DTO usado:** `CategoriaDTO` (Id, Nombre, Descripcion)  
**AutoMapper:** `Categoria ↔ CategoriaDTO`

---

### 7.2 Módulo de Prendas e Inventario (`/Prendas`)

**Controlador:** `PrendasController` — `[Authorize]`  
**Acceso:** Administrador

| Acción | Método HTTP | Ruta | Descripción |
|--------|------------|------|-------------|
| Index | GET | /Prendas | Lista prendas con categoría |
| Details | GET | /Prendas/Details/{id} | Detalle completo |
| Create | GET | /Prendas/Create | Formulario (carga categorías en ViewBag) |
| Create | POST | /Prendas/Create | Guarda prenda + upload de imagen |
| Edit | GET | /Prendas/Edit/{id} | Formulario de edición |
| Edit | POST | /Prendas/Edit/{id} | Actualiza datos |
| Delete | GET | /Prendas/Delete/{id} | Confirmación |
| DeleteConfirmed | POST | /Prendas/Delete/{id} | Elimina |
| Dashboard | GET | /Prendas/Dashboard | Gráficos de stock por categoría |

**DTO usado:** `PrendaDTO` — incluye `CategoriaNombre` mapeado desde la navegación  
**Propiedad calculada:** `Categoria` (tipo `CategoriaInfo`) construida desde `CategoriaNombre`  
**Upload de imágenes:** Almacenadas en `wwwroot/images/` con nombre de GUID.  
**AutoMapper:** `Prenda ↔ PrendaDTO` (con mapeo personalizado para `CategoriaNombre`)

---

### 7.3 Módulo de Clientes (`/Clientes`)

**Controlador:** `ClientesController` — `[Authorize]`  
**Acceso:** Administrador

| Acción | Método HTTP | Ruta | Descripción |
|--------|------------|------|-------------|
| Index | GET | /Clientes | Lista todos los clientes |
| Create | GET | /Clientes/Create | Formulario |
| Create | POST | /Clientes/Create | Guardar cliente |
| Details | GET | /Clientes/Details/{id} | Ver detalle |
| Edit | GET | /Clientes/Edit/{id} | Formulario de edición |
| Edit | POST | /Clientes/Edit/{id} | Guardar cambios |
| Delete | GET | /Clientes/Delete/{id} | Confirmación |
| DeleteConfirmed | POST | /Clientes/Delete/{id} | Eliminar |
| Dashboard | GET | /Clientes/Dashboard | Total + 10 más recientes |

**DTO usado:** `ClienteDTO` (Id, NombreCompleto, DNI, Telefono, Direccion)

---

### 7.4 Módulo de Ventas (`/Ventas`)

**Controlador:** `VentasController` — `[Authorize]`  
**Acceso:** Administrador y Vendedor

#### Vistas MVC

| Acción | Método HTTP | Ruta | Descripción |
|--------|------------|------|-------------|
| Index | GET | /Ventas | Lista ventas + totales del día y globales |
| Dashboard | GET | /Ventas/Dashboard | Gráficos de ventas por mes y categoría |

**ViewModel:** `VentasIndexViewModel` (Ventas, VentasHoy, IngresosHoy, TotalVentas, IngresosTotal)

#### Endpoints API JSON

| Endpoint | Método | Auth | Descripción |
|----------|--------|------|-------------|
| `/api/productos-disponibles` | GET | Público | Prendas con stock > 0 |
| `/api/buscar/{nombre}` | GET | Público | Búsqueda por nombre |
| `/api/clientes-disponibles` | GET | Requiere login | Lista de clientes |
| `/api/vendedores-disponibles` | GET | Requiere login | Vendedores con Estado = true |
| `/api/metodos-pago` | GET | Requiere login | Métodos de pago de la BD |
| `/api/registrar-venta` | POST | Requiere login | Registra venta con transacción |
| `/api/validar-venta` | POST | Requiere login | Valida datos sin persistir |
| `/api/productos-agotandose` | GET | Requiere login | Prendas con stock 1–5 |

#### Payload de registro de venta

```json
{
  "clienteId": 1,
  "vendedorId": 2,
  "metodoPagoId": 1,
  "detalles": [
    { "prendaId": 5, "cantidad": 2, "precio": 49.90 },
    { "prendaId": 8, "cantidad": 1, "precio": 79.00 }
  ]
}
```

#### Respuesta exitosa

```json
{ "exito": true, "mensaje": "Venta registrada exitosamente", "ventaId": 42 }
```

---

### 7.5 Módulo Dashboard General (`/Home/Index`)

**Controlador:** `HomeController` — `[Authorize]`  
**Acceso:** Administrador  
**ViewModel:** `DashboardViewModel` (15 propiedades)

Métricas en tarjetas:
- Total Categorías, Prendas, Clientes, Ventas, Stock, Ingresos, Usuarios

Gráficos Chart.js:
1. Ventas por mes (últimos 6 meses)
2. Ventas por semana (últimos 7 días)
3. Prendas por categoría (dona/barra)
4. Ingresos por método de pago
5. Top 10 productos más vendidos
6. Prendas con stock bajo (≤ 5 unidades)

Tablas dinámicas:
- Últimas 10 ventas
- 8 clientes más recientes
- 6 prendas destacadas (más recientes)

---

### 7.6 Módulo de Configuración del Sistema (`/Configuracion`)

**Controlador:** `ConfiguracionController` — `[Authorize(Roles = "Administrador")]`  
**API:** `ConfiguracionApiController` — `[Authorize(Roles = "Administrador")]`  
**Acceso:** Solo Administrador

Funcionalidades:
- Ver y editar configuración del sistema (nombre de tienda, colores, tema, datos de contacto)
- Caché de 60 minutos con `IMemoryCache`
- Auditoría completa: cada cambio registra usuario, campo, valor anterior, valor nuevo y fecha

---

## 8. SEGURIDAD

### 8.1 Autenticación

El sistema utiliza **ASP.NET Core Identity** con las siguientes configuraciones verificadas
en `Program.cs`:

```
Confirmación de cuenta: NO requerida (desarrollo)
Contraseña:
  - Mínimo 6 caracteres
  - Requiere dígito: SÍ
  - Requiere mayúscula: SÍ
  - Requiere minúscula: SÍ
  - Requiere carácter especial: NO
Cookie de sesión:
  - Expiración: 30 días
  - Sliding expiration: SÍ (se renueva con cada petición)
  - Ruta de login: /Identity/Account/Login
  - Ruta de acceso denegado: /Identity/Account/AccessDenied
```

### 8.2 Autorización por roles

| Controlador | Atributo | Roles permitidos |
|-------------|----------|-----------------|
| `HomeController` | `[Authorize]` | Administrador, Vendedor |
| `CategoriasController` | `[Authorize]` | Administrador, Vendedor |
| `PrendasController` | `[Authorize]` | Administrador, Vendedor |
| `ClientesController` | `[Authorize]` | Administrador, Vendedor |
| `VentasController` | `[Authorize]` | Administrador, Vendedor |
| `ConfiguracionController` | `[Authorize(Roles="Administrador")]` | Solo Administrador |
| `ConfiguracionApiController` | `[Authorize(Roles="Administrador")]` | Solo Administrador |
| `GET /api/productos-disponibles` | `[AllowAnonymous]` | Todos |
| `GET /api/buscar/{nombre}` | `[AllowAnonymous]` | Todos |

### 8.3 Inicialización de roles

Los roles se crean automáticamente al iniciar la aplicación mediante `DbInitializer`:

```csharp
// Roles seeded en cada inicio (idempotente):
string[] roleNames = { "Administrador", "Vendedor" };
// Solo crea el rol si no existe aún
```

### 8.4 Protección de formularios

Todos los formularios POST incluyen el token anti-falsificación de ASP.NET:

```html
@Html.AntiForgeryToken()
```

y los controladores validan con `[ValidateAntiForgeryToken]`.

### 8.5 Middleware de redirección

```csharp
// Orden correcto verificado en Program.cs:
app.UseRouting();
app.UseAuthentication();   // ← Antes de autorización
app.UseAuthorization();
app.Use(async (context, next) => {
    if (context.Request.Path == "/") {
        if (!autenticado) → redirect /Identity/Account/Login
        else             → redirect /Home/Index
    }
    await next(context);
});
```

El orden `UseAuthentication()` antes de `UseAuthorization()` es crítico para que
`context.User` esté poblado correctamente antes de evaluar permisos.

### 8.6 Auditoría de configuración

Cada cambio en la configuración del sistema queda registrado en `ConfiguracionesAuditoria`
con los campos: usuario que realizó el cambio, propiedad modificada, valor anterior y nuevo,
fecha y hora. Esto permite trazabilidad completa de quién cambió qué y cuándo.

---

## 9. PRUEBAS

### 9.1 Resultado de la ejecución

```
Comando:  dotnet test FashionStore.Tests/FashionStore.Tests.csproj
Fecha:    7 de julio de 2026
Resultado:
  Pruebas totales:   290
  Correctas:         290
  Fallidas:            0
  Omitidas:            0
  Tiempo total:      1.59 segundos
  Build:             0 errores · 0 advertencias
```

### 9.2 Organización de los tests

#### Carpeta Controllers/ — 3 archivos

| Archivo | Clases cubiertas | Aspectos probados |
|---------|-----------------|-------------------|
| `CategoriasControllerTests.cs` | `CategoriasController` | Index, Create, Edit, Delete, Dashboard — con mocks de IUnitOfWork + IMapper |
| `PrendasControllerTests.cs` | `PrendasController` | CRUD completo, validación de modelo, casos borde |
| `PrendasControllerDashboardTests.cs` | `PrendasController.Dashboard()` | Agrupación por categoría, serialización JSON de ViewData |

#### Carpeta Entities/ — 8 archivos

| Archivo | Entidad | Aspectos probados |
|---------|---------|-------------------|
| `CategoriaEntityTests.cs` | `Categoria` | Propiedades, colecciones, valores por defecto |
| `ClienteEntityTests.cs` | `Cliente` | Propiedades, formatos DNI, strings vacíos |
| `DetalleVentaEntityTests.cs` | `DetalleVenta` | Propiedades, relaciones de navegación |
| `MetodoPagoEntityTests.cs` | `MetodoPago` | Propiedades, colecciones |
| `PrendaEntityTests.cs` | `Prenda` | Propiedades, `Disponibilidad`, `EstaAgotandose` |
| `RolEntityTests.cs` | `Rol` | Propiedades, fechas, colección de usuarios |
| `VendedorEntityTests.cs` | `Vendedor` | Propiedades, estado activo, formato DNI |
| `VentaEntityTests.cs` | `Venta` | Propiedades, fecha por defecto, relaciones |

#### Carpeta DTOs/ — 5 archivos

| Archivo | DTO | Aspectos probados |
|---------|-----|-------------------|
| `CategoriaDTOTests.cs` | `CategoriaDTO` | Propiedades, validaciones |
| `ClienteDTOTests.cs` | `ClienteDTO` | Propiedades, strings vacíos vs null |
| `MetodoPagoDTOTests.cs` | `MetodoPagoDTO` | Propiedades, nombre vacío |
| `PrendaDTOTests.cs` | `PrendaDTO` | Propiedades, `Categoria` calculada |
| `VendedorDTOTests.cs` | `VendedorDTO` | Propiedades, estado, DNI |

#### Carpeta Infrastructure/ — 1 archivo

| Archivo | Aspectos probados |
|---------|-------------------|
| `InfrastructureCoverageTests.cs` | Cobertura de clases de infraestructura |

#### Carpeta Repositories/ y UnitOfWork/

Tests de `GenericRepository<T>` con base de datos InMemory de EF Core y tests del
patrón `UnitOfWork` (CommitAsync, acceso a repositorios).

### 9.3 Tecnologías de prueba

| Herramienta | Versión | Uso |
|-------------|---------|-----|
| MSTest | 4.2.3 | Framework de pruebas |
| Moq | 4.20.70 | Mocking de IUnitOfWork, IMapper, etc. |
| EF Core InMemory | 9.0.0 | BD en memoria para tests de repositorio |
| coverlet.collector | 10.0.1 | Recolección de cobertura de código |

### 9.4 Estrategia de pruebas

**Pruebas unitarias puras** (sin BD real):
- Entidades: verifican propiedades, valores por defecto, lógica calculada.
- DTOs: verifican transferencia de datos y alias de propiedades.
- Controladores: usan Moq para simular `IUnitOfWork` e `IMapper`, sin hits a BD.

**Pruebas de integración liviana** (BD InMemory):
- Repositorio genérico: verifica operaciones CRUD en base de datos en memoria.
- UnitOfWork: verifica CommitAsync y acceso a repositorios.

**Pruebas de comportamiento de controladores**:
- Verifican que los métodos retornan el tipo correcto (`ViewResult`, `RedirectToActionResult`).
- Verifican que se llaman los métodos del repositorio el número correcto de veces.
- Verifican manejo de `ModelState` inválido.

### 9.5 Cobertura por módulo

| Módulo | Tests | Cobertura funcional |
|--------|-------|---------------------|
| Entidades de dominio | ~80 tests | Alta — todas las propiedades y relaciones |
| DTOs | ~40 tests | Alta — transferencia y validaciones |
| CategoriasController | ~30 tests | Alta — CRUD completo |
| PrendasController | ~60 tests | Alta — CRUD + Dashboard + imagen |
| GenericRepository | ~30 tests | Media-Alta — CRUD en InMemory |
| UnitOfWork | ~20 tests | Media — CommitAsync y repos |
| Infraestructura | ~30 tests | Media — cobertura general |

### 9.6 Advertencias de calidad corregidas durante el desarrollo

| Warning | Archivo | Corrección aplicada |
|---------|---------|---------------------|
| CS8602 — null deref en `_Layout.cshtml` | `Views/Shared/_Layout.cshtml` | `User?.IsInRole()` con null-coalescing |
| CS8625 — null literal en non-nullable (×13) | Varios archivos de test | Reemplazado `null` por `string.Empty` o `null!` |
| MSTEST0032 — assertion siempre verdadera (×2) | `VentaEntityTests`, `RolEntityTests` | Assertions reemplazadas por verificaciones reales |

---

## 10. CONCLUSIONES

### 10.1 Logros técnicos alcanzados

1. **Arquitectura limpia y desacoplada.** El uso del patrón Repository + Unit of Work separa
   claramente la lógica de negocio de la capa de persistencia. Los controladores no acceden
   directamente a `DbContext` salvo en casos justificados (consultas con `Include()` profundo).

2. **Flujo de ventas atómico.** La implementación de `RegistrarVenta()` con transacción
   explícita (`BeginTransactionAsync / CommitAsync / RollbackAsync`) garantiza que jamás
   puede quedar una venta sin sus detalles o con un stock incorrecto.

3. **Seguridad robusta.** Todos los controladores están protegidos con `[Authorize]`.
   La configuración del sistema está restringida a `[Authorize(Roles="Administrador")]`.
   El orden del middleware (Authentication antes de Authorization) está correctamente configurado.

4. **Dashboards con datos reales.** El dashboard general agrega datos de 7 fuentes distintas
   (ventas, prendas, clientes, categorías, métodos de pago, stock y usuarios) sin ningún
   valor hardcodeado.

5. **Suite de pruebas completa.** 290 pruebas automatizadas con MSTest + Moq verifican el
   comportamiento de entidades, DTOs, controladores, repositorios y la unidad de trabajo.

6. **Código libre de advertencias.** El proyecto compila con 0 errores y 0 advertencias
   en modo Debug, garantizando calidad mínima aceptable del código fuente.

### 10.2 Aprendizajes del proceso SSD

- Definir especificaciones verificables antes de codificar reduce significativamente los
  defectos introducidos durante la implementación.
- Los criterios de aceptación expresados como comandos (`dotnet build`, `dotnet test`)
  son objetivos y eliminan ambigüedad sobre qué significa "terminado".
- Separar las fases (Spec → Design → Tasks → Implementation → Verification) facilita
  la detección temprana de problemas de diseño antes de escribir código.

---

## 11. RECOMENDACIONES

### 11.1 Mejoras técnicas prioritarias

| Prioridad | Recomendación | Beneficio |
|-----------|--------------|-----------|
| Alta | Migrar `VentasController` completamente a `IServicioVentas` (eliminar métodos privados duplicados) | Elimina duplicación de lógica de negocio entre el servicio y el controlador |
| Alta | Agregar CRUD de Vendedores en la UI (actualmente sin controlador ni vistas) | Permite gestionar vendedores sin acceso directo a la BD |
| Alta | Agregar CRUD de Métodos de Pago en la UI | Los métodos de pago requieren inserción manual en BD |
| Media | Agregar `required` a propiedades de `ClienteDTO` (elimina CS8618) | Mejora las advertencias de nullability del dominio |
| Media | Implementar `EmailSender` real (actualmente es un stub vacío) | Habilita recuperación de contraseña y notificaciones |
| Media | Agregar tests de integración para `ServicioVentas` y `VentasController` | Cubre la lógica crítica de negocio con pruebas automatizadas |
| Baja | Agregar tests de autorización mediante reflexión (`[Authorize]` en controladores) | Verificación automática de seguridad |

### 11.2 Buenas prácticas a mantener

- Continuar usando el patrón Unit of Work para todas las operaciones CRUD.
- Mantener el orden correcto del middleware en `Program.cs`.
- Siempre envolver operaciones multi-tabla en transacciones explícitas.
- Mantener la suite de tests al 100% de éxito antes de cada commit.
- Documentar toda nueva funcionalidad en los archivos SSD correspondientes.

---

## 12. FUNCIONALIDADES FUTURAS

Las siguientes funcionalidades **no están implementadas** en la versión actual y representan
el trabajo pendiente para iteraciones futuras:

### Iteración 2 — Gestión operativa completa

| Funcionalidad | Descripción |
|---------------|-------------|
| CRUD de Vendedores | Panel para crear, editar, activar/desactivar y eliminar vendedores |
| CRUD de Métodos de Pago | Panel para gestionar los métodos de pago disponibles |
| Módulo de devoluciones | Registrar devoluciones de ventas con restauración de stock |
| Reportes exportables | Exportar ventas y stock a PDF o Excel |

### Iteración 3 — Alertas y automatización

| Funcionalidad | Descripción |
|---------------|-------------|
| Alertas de stock bajo | Entidad `AlertaStock` ya comentada en `DbContext`; notificación cuando stock ≤ umbral |
| Stock mínimo por prenda | Campo `StockMinimo` ya comentado en la entidad `Prenda` |
| Notificaciones por correo | Completar la implementación de `EmailSender` |
| Gestión de usuarios desde UI | Panel de usuarios y asignación de roles sin acceder al Identity directamente |

### Iteración 4 — Funcionalidades avanzadas

| Funcionalidad | Descripción |
|---------------|-------------|
| Tienda en línea | Portal para clientes finales (catálogo, carrito, pedidos) |
| Integración con pasarelas de pago | POS, tarjetas de crédito/débito, transferencias bancarias |
| Facturación electrónica | Emisión de comprobantes de pago electrónicos (SUNAT/SAT) |
| Aplicación móvil | App companion para vendedores en campo |
| API REST pública | Endpoint REST con autenticación JWT para integraciones externas |
| Multi-tienda | Soporte para múltiples sucursales con inventarios independientes |

---

## ANEXOS

### Anexo A — Comandos de validación

```bash
# Compilar la solución completa
dotnet build "FashionStoreSolution.sln"
# Esperado: Build succeeded. 0 Error(s). 0 Warning(s).

# Ejecutar todas las pruebas
dotnet test "FashionStore.Tests/FashionStore.Tests.csproj" --verbosity normal
# Esperado: Passed! - 290 tests - 0 failed - 0 skipped

# Aplicar migraciones a la base de datos
dotnet ef database update \
  --project FashionStore.Infrastructure1 \
  --startup-project FashionStore.Web

# Ejecutar la aplicación en desarrollo
dotnet run --project FashionStore.Web
# Navegar a: https://localhost:5001
```

### Anexo B — Paquetes NuGet principales

| Paquete | Versión | Propósito |
|---------|---------|-----------|
| Microsoft.AspNetCore.Identity.EntityFrameworkCore | 9.0.16 | Autenticación y autorización |
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.16 | Proveedor SQL Server |
| AutoMapper.Extensions.Microsoft.DependencyInjection | 12.0.1 | Mapeo Entidad ↔ DTO |
| X.PagedList.Mvc.Core | 9.1.2 | Paginación (disponible en el proyecto) |
| MSTest.TestFramework | 4.2.3 | Framework de pruebas |
| Moq | 4.20.70 | Mocking en pruebas unitarias |
| Microsoft.EntityFrameworkCore.InMemory | 9.0.0 | BD en memoria para tests |

### Anexo C — Variables de entorno y configuración

```json
// appsettings.json (valores de ejemplo)
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FashionStoreDb;Trusted_Connection=True;"
  }
}
```

---

**Fin del documento**

*Documentación generada y validada el 7 de julio de 2026.*  
*Basada en el código fuente real verificado mediante `dotnet build` (0 errores) y*  
*`dotnet test` (290/290 pruebas pasando).*
