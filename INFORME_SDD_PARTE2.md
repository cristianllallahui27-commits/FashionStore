# SOFTWARE DESIGN DESCRIPTION (SDD) - PARTE 2
## FashionStoreSolution - Ciclo de Vida del Software

---

## 8. DISEÑO DE DATOS

### 8.1 Diagrama de Entidades (ERD)
```
┌─────────────────┐
│    Categoria    │
│  ───────────────│
│ - Id (PK)       │
│ - Nombre        │
│ - Descripcion   │
└────────┬────────┘
         │ (1:N)
         │
┌────────▼────────┐
│     Prenda      │
│  ───────────────│
│ - Id (PK)       │
│ - Nombre        │
│ - Precio        │
│ - Stock         │
│ - CategoriaId   │
└────────┬────────┘
         │ (1:N)
         │
┌────────▼────────────────┐
│    DetalleVenta         │
│  ────────────────────── │
│ - Id (PK)               │
│ - VentaId (FK)          │
│ - PrendaId (FK)         │
│ - Cantidad              │
│ - Precio                │
└────────┬────────────────┘
         │
         │ (N:1)
         │
┌────────▼────────┐
│     Venta       │
│  ───────────────│
│ - Id (PK)       │
│ - ClienteId     │
│ - VendedorId    │
│ - Fecha         │
│ - Total         │
│ - Descuento     │
└────────┬────────┘
    ┌────┴───────┬──────────────┐
    │ (N:1)      │ (N:1)        │
┌───▼────┐  ┌────▼─────┐  ┌───▼──────┐
│Cliente │  │Vendedor  │  │Metodo    │
│────────│  │──────────│  │Pago      │
│- Id    │  │- Id      │  │──────────│
│- Nombre│  │- Nombre  │  │- Id      │
│- Email │  │- Correo  │  │- Nombre  │
└────────┘  │- Estado  │  └──────────┘
            └──────────┘
```

### 8.2 Tablas Principales

**Tabla: Prendas**
| Campo | Tipo | Restricciones |
|-------|------|---------------|
| Id | INT | PK, Auto-increment |
| Nombre | VARCHAR(200) | NOT NULL, UNIQUE |
| Precio | DECIMAL(10,2) | NOT NULL, CHECK > 0 |
| Stock | INT | NOT NULL, CHECK >= 0 |
| CategoriaId | INT | FK → Categorias |
| Activo | BOOL | DEFAULT TRUE |

**Tabla: Ventas**
| Campo | Tipo | Restricciones |
|-------|------|---------------|
| Id | INT | PK, Auto-increment |
| ClienteId | INT | FK → Clientes |
| VendedorId | INT | FK → Vendedores |
| MetodoPagoId | INT | FK → MetodosPago |
| Fecha | DATETIME | NOT NULL, DEFAULT NOW() |
| Total | DECIMAL(10,2) | NOT NULL, CHECK >= 0 |
| Descuento | DECIMAL(10,2) | DEFAULT 0 |

### 8.3 Índices de Rendimiento
```sql
CREATE INDEX idx_ventas_fecha ON Ventas(Fecha DESC);
CREATE INDEX idx_ventas_vendedor ON Ventas(VendedorId);
CREATE INDEX idx_detalles_venta ON DetalleVenta(VentaId);
CREATE INDEX idx_prendas_categoria ON Prendas(CategoriaId);
```

---

## 9. CICLO DE VIDA DEL SOFTWARE

### 9.1 Fases del Desarrollo

#### FASE 1: ANÁLISIS Y REQUERIMIENTOS ✅ COMPLETADO
- Identificación de necesidades (Admin web, POS, Inventario)
- Especificación funcional
- Definición de usuarios y roles

#### FASE 2: DISEÑO ✅ COMPLETADO
- Arquitectura N-capas
- Diseño de BD (ERD)
- Patrones (Repository, UoW, DTO)

#### FASE 3: IMPLEMENTACIÓN 🔄 EN PROGRESO
- Codificación de Controllers
- Implementación de Services
- Migraciones EF Core
**Status**: 80% completado

#### FASE 4: PRUEBAS (SIGUIENTE)
- Tests Unitarios (objetivo 91%)
- Tests de Integración (objetivo 91%)
- Tests de UI (Selenium optional)

#### FASE 5: DEPLOYMENT
- Deployment a Supabase
- Configuración de CI/CD
- Monitoreo y logs

---

## 10. PLAN DE PRUEBAS

### 10.1 Pruebas Unitarias (Objetivo: 91%)

**Proyecto**: FashionStore.Tests
**Framework**: xUnit + Moq
**Cobertura Actual**: 285/285 tests pasando (inicial: 100%)

**Áreas de Prueba**:

#### 1. Services (30 tests)
```
ServicioVentas.RegistrarVenta()
├─ Test: Venta válida → registra OK
├─ Test: Stock insuficiente → error
├─ Test: Cliente inexistente → error
└─ Test: Descuento > total → error

CarritoService.AgregarProducto()
├─ Test: Agregar producto válido → OK
├─ Test: Cantidad 0 → error
└─ Test: Stock insuficiente → error

ConfiguracionSistemaService.ObtenerConfiguracion()
├─ Test: Config por defecto
└─ Test: Config personalizada
```

#### 2. Repositories (20 tests)
```
GenericRepository<Prenda>
├─ GetAllAsync() → retorna todas
├─ GetByIdAsync() → retorna una
├─ Add() → crea nueva
├─ Update() → modifica
└─ Delete() → elimina
```

#### 3. Controllers (25 tests)
```
VentasController.Create()
├─ GET Create() → retorna vista
├─ POST Create(válida) → registra
└─ POST Create(inválida) → error

PrendasController.Index()
├─ Retorna lista prendas
└─ Filtra por categoría
```

#### 4. Entidades (15 tests)
```
Prenda.cs
├─ Validación nombre requerido
├─ Validación precio > 0
└─ Validación categoría

Venta.cs
├─ Cálculo total correcto
└─ Validación descuento
```

### 10.2 Pruebas de Integración (Objetivo: 91%)

**Framework**: xUnit + TestDatabase

#### 1. Flujo End-to-End: Crear Venta
```
1. Setup: DB limpia, datos iniciales
2. Dado: Usuario admin, cliente, vendedor, prendas en stock
3. Cuando: Crear venta con 2 detalles
4. Entonces: 
   - Venta guardada en BD
   - Stock disminuye
   - Total calculado correcto
   - Auditoría registrada
```

#### 2. Flujo: Carrito Persistente
```
1. Agregar producto a carrito
2. Navegar a otra página
3. Volver a carrito
4. Verificar: producto aún presente
```

#### 3. Flujo: Seguridad - Vendedor
```
1. Vendedor A crea venta
2. Vendedor B intenta ver venta de A
3. Verificar: Acceso denegado ✅
```

### 10.3 Cobertura de Código

**Objetivo**: 91% líneas cubiertas
**Herramienta**: Coverlet + ReportGenerator

**Por módulo**:
- Controllers: 85% (excluyendo UI rendering)
- Services: 95% (lógica crítica)
- Repositories: 90%
- Entities: 80% (modelos simples)
- **Total esperado**: 91%

---

## 11. SEGURIDAD

### 11.1 Autenticación
- **Mecanismo**: ASP.NET Identity + JWT (futuro)
- **Contraseñas**: Hashing PBKDF2
- **Sesión**: Timeout 30 minutos

### 11.2 Autorización
```csharp
// Por Rol
[Authorize(Roles = "Administrador")]

// Por Política (Granular)
[Authorize(Policy = "PuedeCambiarConfiguracion")]
```

### 11.3 Validación
- **Input**: Data Annotations + Fluent Validation
- **SQL Injection**: Parametrized queries (EF Core)
- **XSS**: HTML encoding en vistas Razor

---

## 12. RENDIMIENTO Y ESCALABILIDAD

### 12.1 Estrategias
- **Lazy Loading**: Relaciones con Include()
- **Paginación**: 20 items por página
- **Caché**: Session para carrito
- **Índices DB**: En fechas y IDs foráneos

### 12.2 Estimaciones
- Usuarios concurrentes: 50
- Transacciones/segundo: 5 TPS
- Tiempo respuesta: < 2 segundos

---

## 13. REFERENCIAS Y APÉNDICES

### 13.1 Estándares Utilizados
- IEEE 1016-2009: SDD
- IEEE 829-2008: Test Documentation
- REST: Para futuras APIs

### 13.2 Herramientas
- IDE: Visual Studio Code / Visual Studio 2022
- BD: PostgreSQL 15.x (Supabase) + SQL Server 2019
- Testing: xUnit, Moq, Selenium
- CI/CD: GitHub Actions (futuro)

### 13.3 Convenciones de Código
```csharp
// Clases
public class NombreClase { }

// Métodos
public async Task<IActionResult> NombreMetodo() { }

// Variables
private readonly IUnitOfWork _unitOfWork;
```

---

**Versión SDD**: 1.0.0  
**Fecha**: Julio 7, 2026  
**Estado**: ✅ APROBADO PARA IMPLEMENTACIÓN
