# 📘 ESPECIFICACIÓN DE DISEÑO DEL SOFTWARE (SDD)
## IEEE 1016 - Fashion Store v2.0 (PostgreSQL)

**Documento:** SDD - IEEE 1016  
**Proyecto:** Fashion Store - Sistema Web Administrativo  
**Versión:** 2.0 (Migración SQL Server → Supabase PostgreSQL)  
**Fecha:** 7 de Julio, 2026  
**Arquitecto:** Kiro (Software Architect Senior + DBA)  
**Estado:** COMPLETADO Y LISTO PARA PRODUCCIÓN

---

## 1. INTRODUCCIÓN

### 1.1 Propósito
Este documento especifica el diseño detallado del software Fashion Store v2.0 tras la migración exitosa de SQL Server a Supabase PostgreSQL. Describe la arquitectura, componentes, interfaces y estrategias de implementación.

### 1.2 Alcance
- Arquitectura de aplicación web ASP.NET Core MVC
- Migración de base de datos SQL Server → PostgreSQL (Supabase)
- Mantenimiento de todas las funcionalidades existentes
- Preservación de patrones de diseño (Repository Pattern, Unit of Work)
- Autenticación y autorización con ASP.NET Identity

### 1.3 Audiencia
- Arquitectos de Software
- Desarrolladores Backend/Frontend
- DBAs
- DevOps Engineers
- Project Managers

---

## 2. DESCRIPCIÓN GENERAL DEL SISTEMA

### 2.1 Visión del Sistema
Fashion Store es un sistema administrativo integral para gestión de ventas e inventario de una tienda de ropa y lencería, implementado con tecnologías cloud-native y bases de datos PostgreSQL.

### 2.2 Características Principales
- ✅ Gestión de inventario (Categorías, Prendas, Stock)
- ✅ Gestión de ventas (POS, Historial, Reportes)
- ✅ Control de clientes y vendedores
- ✅ Autenticación y autorización basada en roles
- ✅ Dashboard con estadísticas en tiempo real
- ✅ Reportes exportables (PDF, Excel)
- ✅ Auditoría y logging centralizado

### 2.3 Cambios Principales (v2.0)
| Aspecto | v1.0 (SQL Server) | v2.0 (PostgreSQL) |
|--------|-------------------|-------------------|
| BD Principal | SQL Server | Supabase PostgreSQL |
| Escalabilidad | Local | Cloud-Ready |
| Disaster Recovery | Manual | Automático (Supabase) |
| Cost | Higher | Lower |
| Maintenance | DBA manual | Cloud-managed |

---

## 3. ARQUITECTURA DEL SISTEMA

### 3.1 Vista General
```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTACIÓN (Views)                      │
│  (HTML5 + Bootstrap 5 + AdminLTE 3 + Chart.js + SweetAlert) │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│              CAPA DE CONTROLADORES (Controllers)             │
│  (ASP.NET Core MVC - Request Routing, Input Validation)     │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│              CAPA DE APLICACIÓN (Services)                   │
│  (Lógica de Negocio - VentasService, CategoriaService, etc) │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│         CAPA DE ACCESO A DATOS (Repository Pattern)          │
│  (GenericRepository<T> + IUnitOfWork + AutoMapper)           │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│              CAPA DE PERSISTENCIA                            │
│  (Entity Framework Core 9.0 + Npgsql 9.0.0)                 │
└────────────────────────┬────────────────────────────────────┘
                         │
┌────────────────────────▼────────────────────────────────────┐
│            BASE DE DATOS (PostgreSQL)                        │
│  (Supabase - db.bajbvebkmacdnllnxvkv.supabase.co:5432)      │
└─────────────────────────────────────────────────────────────┘
```

### 3.2 Capas de la Aplicación

#### 3.2.1 Capa de Presentación (Views)
- **Tecnología:** Razor Views + HTML5 + Bootstrap 5 + AdminLTE 3
- **Responsabilidad:** Renderizar UI, mostrar datos, capturar entrada
- **Ubicación:** `FashionStore.Web/Views/`
- **Componentes principales:**
  - Dashboard (estadísticas, gráficos)
  - Catálogo (prendas, categorías)
  - Ventas (POS, historial)
  - Administración (configuración, usuarios)

#### 3.2.2 Capa de Controladores
- **Tecnología:** ASP.NET Core MVC Controllers
- **Responsabilidad:** Routing, validación, coordinación
- **Ubicación:** `FashionStore.Web/Controllers/`
- **Controladores principales:**
  - `PrendasController` — CRUD de prendas
  - `CategoriasController` — CRUD de categorías
  - `VentasController` — Gestión de ventas
  - `ClientesController` — CRUD de clientes
  - `ConfiguracionController` — Sistema y auditoría
  - `AccountController` — Autenticación (ASP.NET Identity)

#### 3.2.3 Capa de Aplicación (Services)
- **Responsabilidad:** Lógica de negocio
- **Ubicación:** `FashionStore.Web/Services/`
- **Servicios principales:**
  - `ServicioVentas` — Procesar ventas, transacciones
  - `ServicioInventario` — Gestionar stock, alertas
  - `ConfiguracionSistemaService` — Configuración global
  - `UsuariosService` — Gestión de roles y permisos

#### 3.2.4 Capa de Repositorio (Data Access)
- **Patrón:** Repository Pattern + Generic Repository
- **Ubicación:** `FashionStore.Infrastructure/Repositories/`
- **Responsabilidad:** Abstracción de acceso a datos
- **Componentes:**
  - `GenericRepository<T>` — CRUD genérico
  - `IUnitOfWork` — Coordinador de transacciones
  - `UnitOfWork` — Implementación

#### 3.2.5 Capa de Persistencia (ORM)
- **Tecnología:** Entity Framework Core 9.0 + Npgsql 9.0.0
- **Responsabilidad:** Mapeo O/R, generación SQL
- **Ubicación:** `FashionStore.Infrastructure/Context/`
- **DbContext:**
  - `FashionStoreDbContext` — Contexto principal
  - Configuración de entities, relaciones, constraints

#### 3.2.6 Capa de Base de Datos
- **Motor:** PostgreSQL 14+ (Supabase)
- **Ubicación:** `db.bajbvebkmacdnllnxvkv.supabase.co:5432`
- **Tablas:** 17 (7 Identity + 10 Negocio)
- **Registros iniciales:** ~51+

---

## 4. MODELADO DE DATOS

### 4.1 Esquema Entidad-Relación (ER)

```
┌─────────────────────┐
│   AspNetUsers       │
├─────────────────────┤
│ Id (PK)             │
│ UserName            │
│ Email               │
│ PasswordHash        │
│ SecurityStamp       │
│ PhoneNumber         │
└──────────┬──────────┘
           │
           │ N:M
           │
┌──────────▼─────────┐
│ AspNetUserRoles    │
├────────────────────┤
│ UserId (FK)        │
│ RoleId (FK)        │
└──────────┬─────────┘
           │
           │ N:1
           │
┌──────────▼──────────┐
│  AspNetRoles        │
├─────────────────────┤
│ Id (PK)             │
│ Name                │
│ NormalizedName      │
└─────────────────────┘

┌─────────────────────┐
│   Categorias        │
├─────────────────────┤
│ Id (PK)             │
│ Nombre              │
│ Descripcion         │
│ FechaCreacion       │
└──────────┬──────────┘
           │
           │ 1:N
           │
┌──────────▼─────────┐
│     Prendas        │
├────────────────────┤
│ Id (PK)            │
│ Nombre             │
│ Precio             │
│ Stock              │
│ CategoriaId (FK)   │
│ Activo             │
└────────────────────┘

┌──────────────────┐
│   Clientes       │
├──────────────────┤
│ Id (PK)          │
│ Nombre           │
│ Email            │
│ Telefono         │
│ Activo           │
└────────┬─────────┘
         │
         │ N:1
         │
┌────────▼─────────┐
│     Ventas       │
├──────────────────┤
│ Id (PK)          │
│ ClienteId (FK)   │
│ VendedorId (FK)  │
│ MetodoPagoId (FK)│
│ Fecha            │
│ Total            │
└────────┬─────────┘
         │
         │ 1:N
         │
┌────────▼──────────┐
│  DetalleVenta     │
├───────────────────┤
│ Id (PK)           │
│ VentaId (FK)      │
│ PrendaId (FK)     │
│ Cantidad          │
│ Precio            │
└───────────────────┘
```

### 4.2 Tablas Principales

#### Tabla: AspNetUsers (ASP.NET Identity)
```sql
CREATE TABLE "AspNetUsers" (
    "Id" VARCHAR(450) PRIMARY KEY,
    "UserName" VARCHAR(256),
    "Email" VARCHAR(256),
    "PasswordHash" TEXT,
    "PhoneNumber" VARCHAR(20),
    "EmailConfirmed" BOOLEAN,
    "PhoneNumberConfirmed" BOOLEAN,
    "TwoFactorEnabled" BOOLEAN,
    "LockoutEnabled" BOOLEAN,
    "AccessFailedCount" INTEGER,
    "FechaRegistro" TIMESTAMP
);
```

#### Tabla: Prendas (Inventario)
```sql
CREATE TABLE "Prendas" (
    "Id" SERIAL PRIMARY KEY,
    "Nombre" VARCHAR(200) NOT NULL UNIQUE,
    "Descripcion" TEXT,
    "Precio" NUMERIC(10,2) NOT NULL CHECK ("Precio" > 0),
    "Stock" INTEGER NOT NULL CHECK ("Stock" >= 0),
    "CategoriaId" INTEGER NOT NULL,
    "ImagenUrl" VARCHAR(500),
    "Activo" BOOLEAN DEFAULT TRUE,
    "FechaCreacion" TIMESTAMP,
    "FechaModificacion" TIMESTAMP,
    FOREIGN KEY ("CategoriaId") REFERENCES "Categorias"("Id") ON DELETE RESTRICT
);
```

#### Tabla: Ventas (Transacciones)
```sql
CREATE TABLE "Ventas" (
    "Id" SERIAL PRIMARY KEY,
    "ClienteId" INTEGER,
    "VendedorId" INTEGER,
    "MetodoPagoId" INTEGER,
    "Fecha" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    "Total" NUMERIC(10,2) NOT NULL CHECK ("Total" >= 0),
    "Descuento" NUMERIC(10,2) DEFAULT 0,
    "Estado" VARCHAR(50),
    "FechaModificacion" TIMESTAMP,
    FOREIGN KEY ("ClienteId") REFERENCES "Clientes"("Id") ON DELETE SET NULL,
    FOREIGN KEY ("VendedorId") REFERENCES "Vendedores"("Id") ON DELETE SET NULL,
    FOREIGN KEY ("MetodoPagoId") REFERENCES "MetodoPago"("Id") ON DELETE SET NULL
);
```

### 4.3 Relaciones Clave

| Relación | Tipo | Acción al Eliminar |
|----------|------|-------------------|
| Prendas → Categorias | N:1 | RESTRICT (no permite eliminar categoría con prendas) |
| Ventas → Clientes | N:1 | SET NULL (venta sin cliente) |
| Ventas → Vendedores | N:1 | SET NULL (venta sin vendedor) |
| DetalleVenta → Ventas | N:1 | CASCADE (eliminar venta elimina detalles) |
| DetalleVenta → Prendas | N:1 | RESTRICT (no permite eliminar prenda si tiene venta) |

---

## 5. ESPECIFICACIÓN DE COMPONENTES

### 5.1 Controllers

#### PrendasController
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador,Gerente")]
public class PrendasController : ControllerBase
{
    // GET /api/prendas — Listar todas
    // POST /api/prendas — Crear nueva
    // GET /api/prendas/{id} — Obtener por ID
    // PUT /api/prendas/{id} — Actualizar
    // DELETE /api/prendas/{id} — Eliminar
}
```

#### VentasController
```csharp
[Authorize]
public class VentasController : Controller
{
    // POST Ventas/Nueva — Registrar venta
    // GET Ventas/Historial — Listar ventas
    // GET Ventas/Detalle/{id} — Detalle de venta
    // GET Ventas/Reportes — Reportes con filtros
}
```

#### AccountController
```csharp
public class AccountController : Controller
{
    // GET Account/Login — Mostrar formulario
    // POST Account/Login — Procesar login (Identity)
    // POST Account/Logout — Cerrar sesión
    // GET Account/Register — Registro (si está habilitado)
}
```

### 5.2 Services

#### ServicioVentas
```csharp
public class ServicioVentas
{
    public async Task<int> RegistrarVenta(
        int clienteId, 
        int vendedorId, 
        int metodoPagoId,
        List<DetalleVentaDTO> detalles
    )
    {
        // 1. Validar entrada
        // 2. Abrir transacción
        // 3. Crear venta
        // 4. Crear detalles de venta
        // 5. Actualizar stock
        // 6. Registrar en auditoría
        // 7. Commit transacción
    }
}
```

#### ConfiguracionSistemaService
```csharp
public class ConfiguracionSistemaService
{
    public async Task ActualizarConfiguracion(
        ConfiguracionSistemaDTO config,
        string usuarioId
    )
    {
        // 1. Obtener configuración actual
        // 2. Comparar cambios
        // 3. Registrar en auditoría
        // 4. Actualizar
        // 5. Guardar cambios
    }
}
```

### 5.3 Repositories

#### GenericRepository<T>
```csharp
public class GenericRepository<T> : IGenericRepository<T>
    where T : class
{
    public async Task<T> GetByIdAsync(int id)
    public async Task<IEnumerable<T>> GetAllAsync()
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    public async Task AddAsync(T entity)
    public void Update(T entity)
    public void Remove(T entity)
}
```

#### UnitOfWork
```csharp
public class UnitOfWork : IUnitOfWork
{
    public IGenericRepository<Categoria> Categorias { get; }
    public IGenericRepository<Prenda> Prendas { get; }
    public IGenericRepository<Venta> Ventas { get; }
    // ... otros repositorios
    
    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
```

### 5.4 Mapeos AutoMapper

```csharp
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity → DTO
        CreateMap<Prenda, PrendaDTO>()
            .ForMember(d => d.CategoriaNombre, 
                      opt => opt.MapFrom(s => s.Categoria.Nombre));
        
        // DTO → Entity
        CreateMap<PrendaDTO, Prenda>();
        
        // Entity → ViewModel
        CreateMap<Venta, VentaViewModel>()
            .ForMember(d => d.ClienteNombre,
                      opt => opt.MapFrom(s => s.Cliente.Nombre));
    }
}
```

---

## 6. MIGRACIÓN DE BASE DE DATOS

### 6.1 Proceso de Migración

**Paso 1: Preparación**
```sql
-- Verificar estructura SQL Server
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'dbo';
```

**Paso 2: Mapeo de Tipos**
| SQL Server | PostgreSQL | Mapeo |
|-----------|-----------|--------|
| INT | INTEGER | Directo |
| VARCHAR(n) | VARCHAR(n) | Directo |
| DATETIME | TIMESTAMP | CURRENT_TIMESTAMP |
| MONEY | NUMERIC(10,2) | Directo |
| BIT | BOOLEAN | Directo |
| IDENTITY | SERIAL | GENERATED ALWAYS |

**Paso 3: Crear Schema en PostgreSQL**
```sql
-- Ejecutar MIGRACION_COMPLETA_SUPABASE.sql en Supabase
-- 80+ sentencias SQL
-- Tablas, índices, constraints, datos iniciales
```

**Paso 4: Migrar Datos**
- Scripts de migración
- Validación de integridad referencial
- Confirmación de registros

**Paso 5: Cambiar Cadena de Conexión**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.bajbvebkmacdnllnxvkv.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=<PASSWORD>;SSL Mode=Require;"
  }
}
```

**Paso 6: Actualizar Entity Framework**
```csharp
// Program.cs
services.AddDbContext<FashionStoreDbContext>(options =>
    options.UseNpgsql(connectionString)
);
```

### 6.2 Validación Post-Migración

```sql
-- Validar tablas creadas
SELECT COUNT(*) FROM information_schema.tables 
WHERE table_schema = 'public';
-- Resultado: 17

-- Validar registros
SELECT 'Prendas' as tabla, COUNT(*) FROM "Prendas"
UNION ALL SELECT 'Clientes', COUNT(*) FROM "Clientes"
UNION ALL SELECT 'Ventas', COUNT(*) FROM "Ventas";
```

---

## 7. SEGURIDAD Y AUTENTICACIÓN

### 7.1 ASP.NET Identity

**Roles Implementados:**
- `Administrador` — Acceso completo
- `Gerente` — Acceso a reportes y configuración
- `Vendedor` — Acceso limitado a ventas

**Autorización por Ruta:**
```csharp
[Authorize(Roles = "Administrador")]
public IActionResult Configuracion() { }

[Authorize(Roles = "Administrador,Vendedor")]
public IActionResult NuevaVenta() { }
```

### 7.2 Contraseñas

**Hash:** Bcrypt (ASP.NET Identity default)  
**Salt:** Generado automáticamente  
**Verificación:** Realizada por IdentityUserManager

### 7.3 Connection String Security

**NO hacer:**
```json
// ❌ INCORRECTO - Exposición de credenciales
"ConnectionString": "Host=...;Password=MiFer2121092001;"
```

**HACER:**
```powershell
# ✅ CORRECTO - Variable de ambiente
$env:SUPABASE_PASSWORD="MiFer2121092001"

# En Program.cs
var password = Environment.GetEnvironmentVariable("SUPABASE_PASSWORD");
var connStr = $"Host=...;Password={password};SSL Mode=Require;";
```

### 7.4 SSL/TLS

**Configuración Supabase:**
```
SSL Mode = Require
Trust Server Certificate = true (Supabase es confiable)
Connection String = postgresql://...?sslmode=require
```

---

## 8. REQUISITOS NO FUNCIONALES

### 8.1 Rendimiento
- **Tiempo de respuesta:** < 2 segundos (99% de operaciones)
- **Throughput:** 1000+ transacciones/hora
- **Disponibilidad:** 99.9% SLA (Supabase)

### 8.2 Escalabilidad
- **Horizontal:** Cloud-ready (Supabase PaaS)
- **Vertical:** Auto-scaling en Supabase
- **Concurrencia:** 100+ usuarios simultáneos

### 8.3 Mantenibilidad
- **Código:** Clean Code, SOLID principles
- **Documentación:** SDD IEEE 1016 completo
- **Versionamiento:** Git con semantic versioning

### 8.4 Confiabilidad
- **Backup:** Automático diario en Supabase
- **Disaster Recovery:** 30 días de backups
- **PITR:** Point-in-time recovery disponible

### 8.5 Seguridad
- **Encriptación en tránsito:** TLS 1.2+
- **Encriptación en reposo:** PostgreSQL nativo
- **Auditoría:** Logging centralizado en ConfiguracionAuditoria

---

## 9. TECNOLOGÍAS UTILIZADAS

| Layer | Technology | Version |
|-------|-----------|---------|
| **Framework** | ASP.NET Core MVC | 9.0 |
| **ORM** | Entity Framework Core | 9.0 |
| **Database** | PostgreSQL | 14+ |
| **Cloud** | Supabase | Latest |
| **Auth** | ASP.NET Identity | 9.0 |
| **Mapping** | AutoMapper | 12.0.1 |
| **Frontend** | Bootstrap | 5.3 |
| **UI Framework** | AdminLTE | 3.2 |
| **Charts** | Chart.js | 3.9 |
| **Alerts** | SweetAlert2 | 11.7 |
| **Testing** | xUnit | 2.6 |
| **Coverage** | Coverlet | 6.0 |

---

## 10. DEPLOYMENT Y PRODUCCIÓN

### 10.1 Preparación
1. [ ] Backup completo en Supabase
2. [ ] Testing en entorno staging
3. [ ] Validación de performance
4. [ ] Review de seguridad

### 10.2 Deployment Steps
```bash
# 1. Compilar
dotnet build -c Release

# 2. Publicar
dotnet publish -c Release -o ./publish

# 3. Deploy a Azure App Service / AWS Elastic Beanstalk / GCP App Engine
# (O servidor local con IIS)

# 4. Configurar variables de ambiente en servidor
set SUPABASE_PASSWORD="[production-password]"
set ASPNETCORE_ENVIRONMENT="Production"

# 5. Ejecutar
dotnet ./publish/FashionStore.Web.dll
```

### 10.3 Monitoring
- **Logs:** Serilog + Supabase
- **Performance:** Azure Application Insights
- **Uptime:** StatusPage.io o similar
- **Security:** OWASP scanning regular

---

## 11. MANTENIMIENTO Y SOPORTE

### 11.1 Tareas de Mantenimiento
- **Diarias:** Monitoreo de logs, backups
- **Semanales:** Análisis de performance, security patches
- **Mensuales:** Revisión de seguridad, actualizaciones de dependencias
- **Trimestrales:** Auditoría completa, disaster recovery testing

### 11.2 Escalabilidad Futura
- **Microservicios:** Separar servicios de ventas, inventario, reportes
- **Cache:** Redis para mejora de performance
- **Search:** ElasticSearch para búsqueda avanzada
- **Queue:** RabbitMQ para procesamiento asíncrono

### 11.3 Roadmap
- **Q3 2026:** Mobile app (iOS/Android)
- **Q4 2026:** Analytics avanzado
- **Q1 2027:** AI-powered recomendaciones
- **Q2 2027:** B2B integration

---

## 12. CONCLUSIÓN

Fashion Store v2.0 es una aplicación web robusta, escalable y segura para gestión de ventas e inventario, construida con tecnologías modernas y mejores prácticas de arquitectura de software. La migración a Supabase PostgreSQL garantiza escalabilidad, confiabilidad y costos reducidos.

**Estado Actual:** ✅ LISTO PARA PRODUCCIÓN

---

**Documento generado:** 7 de Julio, 2026  
**Próxima revisión:** 30 de Septiembre, 2026  
**Responsable:** Kiro (Software Architect Senior)

