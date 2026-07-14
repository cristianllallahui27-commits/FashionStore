# SOFTWARE DESIGN DESCRIPTION (SDD)
## FashionStoreSolution - Tienda de Ropa y Lencería

**Basado en IEEE 1016-2009**

---

## 1. INTRODUCCIÓN

### 1.1 Propósito
Este documento describe el diseño de software del sistema administrativo web FashionStoreSolution. Define la arquitectura, componentes, interfaces y estrategias de implementación para un sistema de gestión de ventas, inventario y administración de tienda de ropa y lencería.

### 1.2 Alcance
- **Nombre del Sistema**: FashionStoreSolution
- **Versión**: 1.0.0
- **Plataforma**: ASP.NET Core 8.0 MVC
- **Base de Datos**: PostgreSQL (Supabase) + SQL Server (Fallback)
- **Usuarios**: Administradores, Vendedores, Clientes
- **Módulos**: Gestión de Prendas, Categorías, Clientes, Vendedores, Ventas, Configuración

### 1.3 Definiciones, Acrónimos y Abreviaturas
- **SDD**: Software Design Description
- **MVC**: Model-View-Controller
- **DTO**: Data Transfer Object
- **BD**: Base de Datos
- **API**: Application Programming Interface
- **ORM**: Object-Relational Mapping

---

## 2. REFERENCIAS
- IEEE 1016-2009: Software Design Descriptions
- ASP.NET Core Documentation: https://docs.microsoft.com/aspnet/core
- Entity Framework Core: https://docs.microsoft.com/ef/core
- Repository Pattern: https://martinfowler.com/eaaCatalog/repository.html

---

## 3. DESCRIPCIÓN GENERAL DEL SISTEMA

### 3.1 Funcionalidad General
El sistema proporciona:
- Gestión de catálogo de prendas (ropa y lencería)
- Gestión de categorías de productos
- Gestión de clientes y vendedores
- Sistema de punto de venta (POS) para registro de ventas
- Reportes y estadísticas de ventas
- Configuración del sistema (logos, colores, temas)

### 3.2 Usuarios del Sistema
1. **Administrador**: Acceso total, configuración, reportes
2. **Vendedor**: Crear ventas, ver historial, gestión limitada
3. **Cliente**: (Futuro) Visualización de catálogo

### 3.3 Restricciones de Diseño
- Seguridad: ASP.NET Identity con roles
- Persistencia: Migración de SQL Server a Supabase PostgreSQL
- Performance: Lazy loading en relaciones N:M
- Escalabilidad: Arquitectura de capas independiente

---

## 4. PERSPECTIVA ARQUITECTÓNICA

### 4.1 Estilos y Patrones
- **Patrón**: Repository + Unit of Work
- **Arquitectura**: N-Capas (Presentation, Application, Domain, Infrastructure)
- **Validación**: Data Annotations + Fluent Validation
- **Mapeo**: AutoMapper para DTOs

### 4.2 Estructura en Capas
```
┌─────────────────────────────────────┐
│  FashionStore.Web (Presentación)    │
│  - Controllers, Views, Pages        │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│  FashionStore.Domain (Lógica)       │
│  - Entities, DTOs, Interfaces       │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│  FashionStore.Infrastructure        │
│  - Context, Repositories, Services  │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│  Base de Datos (PostgreSQL/SQL)     │
└─────────────────────────────────────┘
```

---

## 5. DISEÑO DE SISTEMA DE ALTO NIVEL

### 5.1 Arquitectura de Componentes Principales
1. **Controllers** (VentasController, PrendasController, etc.)
2. **Services** (ServicioVentas, CarritoService, ConfiguracionService)
3. **Repositories** (GenericRepository<T>)
4. **DbContext** (FashionStoreDbContext)
5. **Entities** (Venta, Prenda, Cliente, Vendedor)

### 5.2 Flujos de Datos Principales

#### Flujo: Crear Nueva Venta
```
Usuario (UI) 
  ↓
VentasController.Create(POST)
  ↓
ServicioVentas.RegistrarVenta()
  ↓
UnitOfWork.Ventas.Add()
UnitOfWork.SaveChangesAsync()
  ↓
FashionStoreDbContext
  ↓
PostgreSQL/SQL Server
  ↓
✅ Venta registrada
```

#### Flujo: Carrito Persistente
```
Usuario agrega producto
  ↓
CarritoService.AgregarProducto()
  ↓
HttpContext.Session (almacenamiento)
  ↓
Usuario navega
  ↓
CarritoService.ObtenerItems()
  ↓
Items persisten ✅
```

---

## 6. DECISIONES ARQUITECTÓNICAS

### 6.1 Base de Datos
**Decisión**: Supabase PostgreSQL como primaria, SQL Server como fallback
**Razón**: Escalabilidad, bajo costo, soporte cloud
**Tradeoff**: Compatibilidad SQL Server limitada, requiere migraciones EF Core

### 6.2 Autenticación
**Decisión**: ASP.NET Identity con roles
**Razón**: Integración nativa, seguridad probada
**Tradeoff**: Complejidad en permisos granulares

### 6.3 Validación
**Decisión**: Data Annotations + Server-side
**Razón**: Seguridad primero
**Tradeoff**: Validación client-side opcional

---

## 7. DISEÑO DETALLADO DE COMPONENTES

### 7.1 Componente: VentasController
**Responsabilidad**: Orquestar operaciones de ventas
**Entrada**: Datos de venta (cliente, vendedor, detalles)
**Salida**: IActionResult (View/Redirect)
**Dependencias**: IUnitOfWork, IConfiguracionSistemaService
**Restricciones**: [Authorize] requerido

### 7.2 Componente: ServicioVentas
**Responsabilidad**: Lógica de negocio de ventas
**Métodos**:
- RegistrarVenta(): Crear venta y actualizar stock
- CalcularTotalVenta(): Suma de detalles
- ValidarVenta(): Verificar consistencia datos
**Excepciones**: InvalidOperationException si falla validación
