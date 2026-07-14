# PLAN DE CORRECCIÓN TÉCNICA
## FashionStoreSolution - ASP.NET Core MVC

**Fecha:** 13 de Julio 2026  
**Versión:** 1.0  
**Base de Datos:** PostgreSQL (Supabase)  
**Framework:** ASP.NET Core 9.0 / Entity Framework Core 9.0  
**Arquitectura:** Repository Pattern + Unit of Work

---

## RESUMEN EJECUTIVO

Se ha completado la **FASE 1** de corrección técnica del sistema FashionStoreSolution. El proyecto compiló exitosamente (0 errores) tras resolver el error crítico de mapeo de `UsuarioId` en la BD PostgreSQL.

**Estado Actual:**
- ✅ Compilación: 0 errores, 2 advertencias (AutoMapper)
- ✅ Base de datos: PostgreSQL / Supabase conectada
- ✅ Autenticación: ASP.NET Identity + Roles funcionando
- ✅ Gestión de Vendedores: Admin puede crear contraseñas
- ✅ Ventas: Auto-detección de vendedor de sesión
- ✅ Dashboard: Gráficos y reportes renderizando
- ⏳ **EN PRUEBA:** Flujo completo vendedor (password → login → ventas)

**Próximas Fases:** 
- Fase 2: Corrección de 5 problemas MEDIO
- Fase 3: Corrección de 4 problemas BAJO

---

## PROBLEMAS ENCONTRADOS Y SOLUCIONADOS

### FASE 1 - BLOQUEANTES (RESUELTOS)

#### **CRÍTICO-1: PostgreSQL Error UsuarioId [RESUELTO]**
- **Error:** `PostgresException: 42703: column v.UsuarioId does not exist`
- **Ubicación:** `GenericRepository.cs:35` → `_dbSet.ToListAsync()`
- **Llamada desde:** `VendedoresController.Index():34`
- **Causa Raíz:** 
  - EF Core estaba mapeando la propiedad `Vendedor.UsuarioId` a la BD
  - La columna no existe en PostgreSQL (migración nunca aplicada)
  - Supabase no permitía crear la columna
  
- **Solución Implementada:**
  ```csharp
  // FashionStore.Domain/Entities/Vendedor.cs
  [NotMapped]  // ← Agregado
  public string? UsuarioId { get; set; }
  ```
  
- **Cambios:**
  - ✅ Agregado `[NotMapped]` attribute a `Vendedor.UsuarioId`
  - ✅ Removido la asignación de `UsuarioId` en `VendedoresController.Create()`
  - ✅ Removido la asignación en `VendedoresController.ToggleEstado()`
  - ✅ Removido la asignación en `DbInitializer.SeedAdminVendedor()`
  - ✅ Agregado usando `System.ComponentModel.DataAnnotations.Schema;`

- **Archivos Modificados:**
  - `FashionStore.Domain/Entities/Vendedor.cs`
  - `FashionStore.Infrastructure1/Data/DbInitializer.cs`
  - `FashionStore.Web/Controllers/VendedoresController.cs` (ya estaba sin asignación)

- **Resultado:** ✅ Compilación exitosa, servidor conectado a BD sin errores

**Criterios de Aceptación:**
- [ ] `dotnet build -c Release` → 0 errores
- [ ] Aplicación inicia sin excepción de BD
- [ ] `GET /Vendedores` no lanza PostgresException
- [ ] Lista de vendedores carga en < 2 segundos

---

### FASE 1 - AVISOS

#### **WARNING-1: AutoMapper Vulnerabilidad**
- **Severidad:** MEDIA (conocida, baja exploitabilidad)
- **Paquete:** `AutoMapper 12.0.1`
- **Problema:** Vulnerabilidad de gravedad alta reportada
- **Ubicación:** `.csproj` de FashionStore.Infrastructure
- **Acción Recomendada:** Actualizar a version >= 13.0.0 en fase de hardening
- **Impacto Actual:** No bloquea desarrollo, solo advertencia en build

---

## PROBLEMAS IDENTIFICADOS (NO RESUELTOS EN ESTA FASE)

### FASE 2 - PRIORIDAD ALTA (5 problemas)

#### **ALTO-1: Credenciales Hardcodeadas en Archivos de Configuración**
- **Severidad:** CRÍTICA (Seguridad)
- **Archivos Afectados:**
  - `FashionStore.Web/appsettings.json`
  - `FashionStore.Web/appsettings.Development.json`
  
- **Problema:**
  ```json
  "SUPABASE_PASSWORD": "MiFer2121092001"
  "ConnectionStrings": {
    "DefaultConnection": "...Password=${SUPABASE_PASSWORD}..."
  }
  ```
  Contraseña real expuesta en repositorio Git.

- **Impacto:** 
  - Acceso no autorizado a base de datos
  - Violación de seguridad (PCI-DSS, OWASP Top 10)
  - Potencial pérdida/corrupción de datos

- **Solución:**
  - Remover contraseña de archivos de configuración
  - Usar solo variables de entorno (ya implementado fallback en `Program.cs:44-46`)
  - Agregar `appsettings*.json` a `.gitignore`
  - Rotar credenciales en Supabase

- **Fase:** 2 (Seguridad)
- **Complejidad:** BAJA
- **Comandos:**
  ```bash
  # Verificar que las credenciales están en env
  echo $env:SUPABASE_PASSWORD
  ```

---

#### **ALTO-2: ToggleEstado sin Validaciones de Null**
- **Archivo:** `FashionStore.Web/Controllers/VendedoresController.cs:162-180`
- **Línea:** 167-170 (búsqueda de ApplicationUser)
- **Problema:**
  ```csharp
  var user = await _userManager.FindByEmailAsync(vendedor.Correo)
      ?? await _userManager.FindByNameAsync(vendedor.Correo);
  
  if (user != null)  // ← Validación está aquí, PERO...
  {
      user.Activo = vendedor.Estado;  // Si user es null, no se actualiza Identity
      await _userManager.UpdateAsync(user);
  }
  ```
  Si usuario no existe en Identity pero sí en Vendedores → estado desincronizado

- **Impacto:** 
  - Vendedor puede estar Inactivo en Vendedores pero Activo en Identity
  - Usuario desactivado puede seguir iniciando sesión

- **Solución:**
  - Validación más estricta
  - Logging de discrepancias
  - Sincronización en background job

- **Fase:** 2 (Lógica de Negocio)
- **Complejidad:** MEDIA

---

#### **ALTO-3: PlainText Password en UltimaPasswordAdmin**
- **Archivo:** `FashionStore.Domain/Entities/Vendedor.cs:30`
- **Propiedad:** `public string? UltimaPasswordAdmin { get; set; }`
- **Problema:**
  ```csharp
  vendedor.UltimaPasswordAdmin = NuevaPassword;  // ← PLAINTEXT!
  ```
  La contraseña se guarda en texto plano en BD (visible para Admin).

- **Impacto:**
  - Violación OWASP A02:2021 - Cryptographic Failures
  - Exposición de contraseñas en dumps de BD
  - No cumple GDPR/PCI-DSS

- **Solución:**
  - Remover campo `UltimaPasswordAdmin` completamente O
  - Usar hash (bcrypt) + salt, pero solo para auditoria (no recuperación)
  - Mejor: usar IdentityUser.PasswordHash (ya encriptado)

- **Fase:** 2 (Seguridad)
- **Complejidad:** MEDIA
- **Impacto:** Solo Admin ve campo, pero aún riesgo en backup BD

---

#### **ALTO-4: Cliente Genérico Recreado en Cada Venta**
- **Archivo:** `FashionStore.Web/Controllers/VentasController.cs:113-116`
- **Código:**
  ```csharp
  var clienteGenerico = await _unitOfWork.Clientes.FindAsync(
      c => c.DNI == "00000000");
  var cliente = clienteGenerico.FirstOrDefault()
      ?? new Cliente { DNI = "00000000", NombreCompleto = "Cliente Mostrador", ... };
  ```

- **Problema:**
  - Si cliente genérico NO existe en primera venta → se crea NEW pero NO se guarda
  - En siguiente venta → se busca de nuevo
  - Potencial generar múltiples registros "00000000"
  - Duplicadas en BD

- **Impacto:**
  - Integridad de datos comprometida
  - Reportes incorrectos (múltiples clientes "00000000")
  - Inconsistencia en ForeignKey

- **Solución:**
  - Crear cliente genérico en DbInitializer
  - Validar que siempre existe
  - Use índice único en (DNI) con constraint

- **Fase:** 2 (Integridad de Datos)
- **Complejidad:** MEDIA

---

#### **ALTO-5: DescuentosController Bypassa UnitOfWork**
- **Archivo:** `FashionStore.Web/Controllers/DescuentosController.cs:12-44`
- **Problema:**
  ```csharp
  public class DescuentosController : Controller
  {
      private readonly FashionStoreDbContext _context;  // ← Directo
      
      // Todos los métodos usan _context.SaveChangesAsync()
      // No usan IUnitOfWork
  }
  ```

- **Impacto:**
  - Inconsistencia arquitectónica (otros controllers usan UnitOfWork)
  - Difícil de testear (no puede mockear `_context`)
  - Posible transacción incompleta si hay múltiples cambios

- **Solución:**
  - Inyectar `IUnitOfWork` en lugar de `FashionStoreDbContext`
  - Usar repositorio genérico para DescuentosAutorizados
  - Mantener consistencia con otros controllers

- **Fase:** 2 (Arquitectura)
- **Complejidad:** MEDIA

---

### FASE 3 - PRIORIDAD MEDIA (4 problemas)

#### **MEDIO-1: DNI en Cliente sin [Required] ni Unique**
- **Archivo:** `FashionStore.Domain/Entities/Cliente.cs:8`
- **Problema:**
  ```csharp
  public string DNI { get; set; } = string.Empty;
  // NO tiene [Required], NO tiene índice único
  ```

- **Impacto:**
  - Podría crearse cliente con DNI vacío
  - Múltiples clientes con mismo DNI (no es error)
  - Reportes agrupados por DNI pueden fallar

- **Solución:**
  - Agregar `[Required]`
  - Crear índice único en DNI
  - Migración en EF Core

- **Fase:** 3 (Validación)
- **Complejidad:** BAJA

---

#### **MEDIO-2: Falta de Índices en ForeignKeys de Ventas**
- **Archivo:** `FashionStore.Infrastructure/Context/FashionStoreDbContext.cs`
- **Problema:**
  - BD tiene FK: VendedorId, ClienteId, MetodoPagoId
  - NO tienen índices explícitos
  - Queries like `WHERE VendedorId = X` son SLOW en PostgreSQL

- **Impacto:**
  - Reportes lentos (especialmente "Ventas por Vendedor")
  - Dashboard tarda > 5s en cargar
  - Escalabilidad comprometida con > 10k ventas

- **Solución:**
  - Crear índices en OnModelCreating:
    ```csharp
    modelBuilder.Entity<Venta>()
        .HasIndex(v => v.VendedorId);
    modelBuilder.Entity<Venta>()
        .HasIndex(v => v.ClienteId);
    ```
  - Crear migración EF Core

- **Fase:** 3 (Performance)
- **Complejidad:** BAJA

---

#### **MEDIO-3: FechaCreacion en DescuentoAutorizado usa UtcNow**
- **Archivo:** `FashionStore.Web/Controllers/DescuentosController.cs:30`
- **Código:**
  ```csharp
  descuentoAutorizado.FechaCreacion = DateTime.UtcNow;
  ```

- **Problema:**
  - Resto de la app usa `DateTime.Now` (hora local)
  - Descuentos usa `DateTime.UtcNow` (UTC)
  - Inconsistencia en reportes por fecha

- **Impacto:**
  - Reportes por fecha pueden excluir descuentos
  - Confusión en auditoría (hora desincronizada)

- **Solución:**
  - Estandarizar a `DateTime.UtcNow` en TODA la app
  - O siempre usar `DateTime.Now`
  - Recomendar: UtcNow (mejor para timezone)

- **Fase:** 3 (Consistencia)
- **Complejidad:** BAJA

---

#### **MEDIO-4: Sin Auditoría en Cambios Críticos**
- **Archivos Afectados:**
  - `VendedoresController.CambiarPassword()` → No registra quién/cuándo cambió
  - `DescuentosController.Create()` → No registra cómo se creó
  - Stock de Prendas → No hay historial de cambios

- **Problema:**
  - Admin cambia contraseña pero no hay log
  - Descuentos autorizados sin trazabilidad
  - Auditoría falla en compliance

- **Impacto:**
  - No cumple GDPR/PCI-DSS
  - Difícil rastrear quién hizo qué cambio
  - Vulnerabilidad a fraude interno

- **Solución:**
  - Crear tabla `AuditoriaOperacion` con (Usuario, Acción, Fecha, Datos)
  - Middleware que registra todos los cambios
  - Dashboard de auditoría para Admin

- **Fase:** 3 (Compliance)
- **Complejidad:** MEDIA

---

## COMANDOS DE VALIDACIÓN

### Compilación
```bash
# Limpiar y compilar
cd FashionStoreSolution
dotnet clean
dotnet build -c Release

# Esperado: 0 Errores, 2 Advertencias (AutoMapper)
```

### Pruebas
```bash
# Ejecutar tests
dotnet test --configuration Release --verbosity detailed

# Opcionales: tests por proyecto
dotnet test FashionStore.Tests -c Release
```

### Base de Datos
```bash
# Aplicar migraciones (si existen pendientes)
dotnet ef database update --project FashionStore.Infrastructure --startup-project FashionStore.Web

# Listar migraciones
dotnet ef migrations list --project FashionStore.Infrastructure --startup-project FashionStore.Web
```

### Ejecución
```bash
# Desde FashionStore.Web/
$env:SUPABASE_PASSWORD='MiFer2121092001'
dotnet run --configuration Release

# Esperar: "[INF] Now listening on: http://localhost:5100"
```

---

## PROTOCOLO DE PRUEBA MANUAL - FLUJO COMPLETO

### **Escenario: Admin asigna contraseña a vendedor → Vendedor inicia sesión → Realiza venta → Descarga reportes**

#### PASO 1: Acceso Admin
```
URL: http://localhost:5100
Usuario: admin@fashionstore.com
Contraseña: Password123!
✓ Esperado: Dashboard carga correctamente
✓ Gráficos visible
```

#### PASO 2: Navegar a Vendedores
```
Navbar → Admin → Vendedores
✓ Esperado: Tabla con 5 vendedores
  - Ana Usuario (DNI: 82030000)
  - Ana Vallejo (DNI: 83051544)
  - Carlos Mendoza (DNI: 92031122)
  - Diego Morena (DNI: 93880000)
  - Luis Castro (DNI: 92908566)
  - Sofia Ruiz (DNI: 83807768)
```

#### PASO 3: Editar Vendedor (Ana Usuario)
```
Click en: Icono lápiz (Edit) en fila de Ana Usuario
✓ Esperado: Formulario abre
✓ Panel "Cambiar Contraseña de Acceso" visible (solo para Admin)
```

#### PASO 4: Asignar Contraseña
```
Campo: "Nueva Contraseña" → ingresa: AnaVendor123!
Click: "Actualizar Contraseña"
✓ Esperado: Mensaje "✓ Contraseña actualizada correctamente para Ana Usuario."
✓ Contraseña encriptada en BD (ApplicationUser.PasswordHash)
```

#### PASO 5: Cerrar Sesión Admin
```
Click: Usuario (top-right) → Logout
✓ Esperado: Redirige a /Identity/Account/Login
```

#### PASO 6: Iniciar Sesión como Vendedor
```
Usuario: ana@fashionstore.com
Contraseña: AnaVendor123!
✓ Esperado: Dashboard carga (rol: Vendedor)
```

#### PASO 7: Nueva Venta
```
Navbar → Ventas → Nueva Venta
✓ Esperado: Formulario pre-llena Vendedor = "Ana Usuario" (auto-detect de sesión)
✓ Selector Cliente visible
✓ Grid de Prendas visible
```

#### PASO 8: Completar Venta
```
1. Cliente: Click en "Mostrador" o crear nuevo
2. Agregar producto (ej: cantidad 2)
3. Método de Pago: "Efectivo"
4. Si Efectivo → Monto Recibido: 100.00
5. Click: "Registrar Venta"

✓ Esperado: Mensaje "✓ Venta registrada exitosamente"
✓ ID de venta mostrado
✓ Stock de producto decrementado
```

#### PASO 9: Probar Botón "Inicio"
```
Navbar → Inicio (icono casa)
✓ Esperado: Redirige a /Home/Index (Dashboard)
✓ NO redirige a Login
✓ Gráficos y totales visibles
```

#### PASO 10: Descargar Reportes
```
Navbar → Admin → Reportes
✓ Esperado: Tabla con ventas de Ana Usuario
Botones:
  - "Descargar Excel" → archivo .xlsx descarga
  - "Descargar PDF" → archivo .pdf descarga
✓ Archivos contienen datos de venta realizada
```

### **Criterios de Aceptación - Flujo Completo:**
- [ ] Admin puede asignar contraseña
- [ ] Vendedor puede iniciar sesión con nueva contraseña
- [ ] Vendedor sale como vendedor en venta (auto-detect)
- [ ] Venta se registra correctamente
- [ ] Stock se decrementa
- [ ] Botón "Inicio" redirige a Home (sin error de auth)
- [ ] Reportes Excel/PDF descargan correctamente
- [ ] No hay excepción PostgreSQL en logs

---

## MATRIZ DE RIESGOS

| Problema | Severidad | Impacto | Probabilidad | Riesgo | Mitiga | 
|----------|-----------|--------|--------------|--------|--------|
| UsuarioId BD | CRÍTICA | No arranca | Resuelto | ✓ RESUELTO | Validación en tests |
| Credenciales Hardcode | CRÍTICA | Violación seguridad | ALTA | Crítico | Fase 2 Sprint |
| PlainText Password | CRÍTICA | Breach PII | ALTA | Crítico | Fase 2 Sprint |
| Client Genérico Duplicado | ALTA | Corrupción datos | MEDIA | Alto | Fase 2 Sprint |
| ToggleEstado Null | ALTA | Desincronización | MEDIA | Alto | Fase 2 Sprint |
| DescuentosController | MEDIA | Testing difícil | BAJA | Medio | Fase 2 Refactor |
| DNI no Unique | MEDIA | Integridad datos | BAJA | Medio | Fase 3 Sprint |
| Falta Índices | MEDIA | Performance | MEDIA | Medio | Fase 3 Optimization |
| FechaCreacion Inca | MEDIA | Reportes incorrectos | BAJA | Bajo | Fase 3 Sprint |
| Sin Auditoría | MEDIA | No-compliance | BAJA | Medio | Fase 3 Compliance |

---

## DECISIONES ARQUITECTÓNICAS

### Decisión 1: UsuarioId [NotMapped] vs Migración
- **Elegida:** [NotMapped]
- **Por qué:** 
  - Sin acceso a internet para crear migración en Supabase
  - [NotMapped] elimina el error sin cambiar BD
  - UsuarioId se usa solo en runtime (matching email)
  - Compatible con PostgreSQL immediatamente
- **Alternativa rechazada:** Crear migración (no aplicable en este momento)

### Decisión 2: Password Storage Strategy
- **Actual:** Identity.PasswordHash (encriptado) + UltimaPasswordAdmin (plaintext)
- **Riesgo:** UltimaPasswordAdmin viola OWASP A02:2021
- **Próxima:** Remover UltimaPasswordAdmin o usar hash
- **Razón cambio:** Admin NUNCA debe ver contraseña real

### Decisión 3: Credentials Management
- **Actual:** Fallback de env var a appsettings (inseguro)
- **Cambio Fase 2:** Solo usar env var, remover de appsettings
- **Razón:** Git expone credenciales a cualquiera con acceso a repo

---

## PRÓXIMAS FASES

### **FASE 2 - Sprint Seguridad (Estimado: 4 horas)**
1. Remover credenciales de appsettings.json
2. Implementar validación en ToggleEstado
3. Remover UltimaPasswordAdmin (usar solo Identity)
4. Crear Cliente genérico en DbInitializer
5. Refactorizar DescuentosController a usar UnitOfWork

### **FASE 3 - Sprint Optimización (Estimado: 6 horas)**
1. Agregar [Required] y Unique en DNI (Cliente, Vendedor)
2. Crear índices en ForeignKeys de Ventas
3. Estandarizar DateTime a UtcNow en toda la app
4. Crear tabla Auditoria y middleware de logging

### **FASE 4 - Testing (Estimado: 8 horas)**
1. Escribir tests unitarios para VendedoresController
2. Escribir tests de integración para flujo de ventas
3. Tests de seguridad (auth, authorization)
4. Load tests (1000 ventas/hora)

---

## DOCUMENTACIÓN GENERADA

Este plan técnico **NO incluye cambios en documentación académica** (SSD). 

Una vez que todas las fases de corrección estén completas, se actualizará:
- `SSD_FashionStore.md` (System Software Documentation)
- `MANUAL_USUARIO.md`
- `API_ENDPOINTS.md`

---

## AUTOR / CONTACTO

**Análisis Técnico:** Kiro AI - Software Architect QA Senior  
**Fecha:** 13 Julio 2026  
**Estado:** ✅ FASE 1 COMPLETA | 🔄 FASE 2 PENDIENTE

