# CASOS DE PRUEBA Y VALIDACIÓN
## FashionStoreSolution - Escenarios Críticos

**Objetivo:** Validar que las correcciones se implementen correctamente y que no haya regresiones.

---

## CASO 1: Validación de Credenciales Removidas

### Pre-requisito
- Archivo: `FashionStore.Web/appsettings.json`
- Archivo: `FashionStore.Web/appsettings.Development.json`
- Archivo: `.gitignore`

### Pasos de Prueba

```bash
# TEST 1.1: Credenciales NO deben estar en archivos
grep "MiFer2121092001" FashionStore.Web/appsettings.json
# ESPERADO: Sin resultados (grep retorna 0)

grep "MiFer2121092001" FashionStore.Web/appsettings.Development.json
# ESPERADO: Sin resultados

# TEST 1.2: .gitignore debe contener appsettings*.json
grep "appsettings" .gitignore
# ESPERADO: Match encontrado con "appsettings*.json"

# TEST 1.3: Git history no debe contener credenciales
git log -p --all -S "MiFer2121092001" -- "*.json"
# ESPERADO: Sin resultados (o remoto indicará rebase realizado)

# TEST 1.4: Aplicación requiere env var
$env:SUPABASE_PASSWORD = ""
dotnet run --project FashionStore.Web
# ESPERADO: Excepción: "SUPABASE_PASSWORD environment variable is REQUIRED"

# TEST 1.5: Con env var configurado, inicia OK
$env:SUPABASE_PASSWORD = "MiFer2121092001"  # Variable de entorno temporal
dotnet run --project FashionStore.Web
# ESPERADO: Aplicación inicia sin error de BD
```

### Criterios de Aceptación
- [ ] Credenciales removidas de JSON
- [ ] .gitignore actualizado
- [ ] Aplicación rechaza inicio si SUPABASE_PASSWORD vacía
- [ ] Con env var, aplicación inicia correctamente

---

## CASO 2: UltimaPasswordAdmin Removido

### Pre-requisito
- Campo removido de `Vendedor.cs`
- Migraciones aplicadas
- BD actualizada

### Pasos de Prueba

```bash
# TEST 2.1: Campo NO existe en entidad
grep -n "UltimaPasswordAdmin" FashionStore.Domain/Entities/Vendedor.cs
# ESPERADO: Sin resultados

# TEST 2.2: Campo NO existe en tabla BD
# Ejecutar en Supabase SQL Editor:
SELECT column_name FROM information_schema.columns 
WHERE table_name = 'Vendedor' AND column_name = 'UltimaPasswordAdmin';
# ESPERADO: Sin resultados

# TEST 2.3: Cambiar contraseña vendedor NO guarda plaintext
# 1. Admin -> Vendedores -> Edit (Ana Usuario)
# 2. Click "Cambiar Contraseña"
# 3. Ingresa: "AnaVendor123!"
# 4. Click "Actualizar Contraseña"
# ESPERADO: Mensaje "✓ Contraseña actualizada correctamente"

# TEST 2.4: Verificar que solo PasswordHash se guarda
# Ejecutar en SQL:
SELECT "Id", "Nombres", "UltimaPasswordAdmin" FROM "Vendedor" WHERE "DNI" = '82030000';
# ESPERADO: UltimaPasswordAdmin es NULL (o columna no existe)
```

### Criterios de Aceptación
- [ ] Campo removido de entidad
- [ ] Migración aplicada en BD
- [ ] Cambio de password funciona
- [ ] No hay almacenamiento de plaintext

---

## CASO 3: Admin Passwords No Hardcodeados

### Pre-requisito
- DbInitializer refactorizado
- Environment variables configuradas (ADMIN1_PASSWORD, ADMIN2_PASSWORD)

### Pasos de Prueba

```bash
# TEST 3.1: Constantes removidas de código
grep -n "private const string Admin1Password" FashionStore.Infrastructure/Data/DbInitializer.cs
# ESPERADO: Sin resultados

grep -n "private const string Admin2Password" FashionStore.Infrastructure/Data/DbInitializer.cs
# ESPERADO: Sin resultados

# TEST 3.2: Código usa Environment.GetEnvironmentVariable
grep "Environment.GetEnvironmentVariable" FashionStore.Infrastructure/Data/DbInitializer.cs
# ESPERADO: Match encontrado

# TEST 3.3: Con env vars, admins pueden iniciar sesión
$env:ADMIN1_PASSWORD = "Password123!"
$env:ADMIN2_PASSWORD = "Admin123!"
dotnet run --project FashionStore.Web

# Navegar a: http://localhost:5100/Identity/Account/Login
# Usuario: admin@fashionstore.com
# Contraseña: Password123!
# ESPERADO: Login exitoso, acceso al dashboard

# TEST 3.4: Sin env vars, admins no se crean
$env:ADMIN1_PASSWORD = ""
$env:ADMIN2_PASSWORD = ""
# Limpiar BD y ejecutar (DbInitializer intenta crear)
# ESPERADO: Log indica que admin users FUERON creados con contraseñas random
# (y debería estar logueado)
```

### Criterios de Aceptación
- [ ] Constantes removidas
- [ ] Código usa variables de entorno
- [ ] Admin puede iniciar sesión
- [ ] Sin hardcoding en código

---

## CASO 4: Autorización en DescuentosController.Edit()

### Pre-requisito
- `[Authorize(Roles = "Administrador")]` agregado en Edit()
- Validación de rango implementada

### Pasos de Prueba

```bash
# TEST 4.1: Solo Admin puede acceder Edit GET
# Como Vendedor:
# 1. Iniciar sesión como ana@fashionstore.com
# 2. Navegar a: http://localhost:5100/Descuentos/Edit/1
# ESPERADO: AccessDenied o Redirect a Home

# TEST 4.2: Admin puede acceder Edit GET
# Como Admin:
# 1. Iniciar sesión como admin@fashionstore.com
# 2. Navegar a: http://localhost:5100/Descuentos/Edit/1
# ESPERADO: Formulario carga

# TEST 4.3: Validación rango en POST
# Como Admin, intentar POST con valor inválido
# Form: Tipo=Porcentaje, Valor=150
# ESPERADO: ModelState error o validation message

# TEST 4.4: Validación negativo rechazado
# Form: Tipo=Porcentaje, Valor=-10
# ESPERADO: Validation error

# TEST 4.5: Valor válido se acepta
# Form: Tipo=Porcentaje, Valor=15
# Click Save
# ESPERADO: Redirect a Index, descuento actualizado
```

### Criterios de Aceptación
- [ ] Vendedor NO puede acceder Edit
- [ ] Admin puede acceder Edit
- [ ] Valores > 100% rechazados
- [ ] Valores negativos rechazados
- [ ] Valores válidos se guardan

---

## CASO 5: Cliente Genérico "00000000" Creado

### Pre-requisito
- DbInitializer contiene SeedClienteGenerico()
- Migración aplicada
- BD limpiada e inicializada

### Pasos de Prueba

```bash
# TEST 5.1: Cliente existe en BD desde inicio
# Ejecutar en SQL:
SELECT * FROM "Cliente" WHERE "DNI" = '00000000';
# ESPERADO: 1 registro con NombreCompleto='Cliente Mostrador'

# TEST 5.2: Crear venta sin seleccionar cliente
# 1. Admin -> Ventas -> Nueva Venta
# 2. Dejar cliente vacío (o seleccionar genérico)
# 3. Agregar producto (cantidad 1)
# 4. Seleccionar método de pago
# 5. Click "Registrar Venta"
# ESPERADO: Venta se registra con ClienteId del genérico

# TEST 5.3: No hay duplicados de "00000000"
# Ejecutar en SQL:
SELECT COUNT(*) FROM "Cliente" WHERE "DNI" = '00000000';
# ESPERADO: Exactamente 1 registro

# TEST 5.4: Múltiples ventas usan el mismo cliente genérico
# Crear 3 ventas sin cliente específico
# Ejecutar en SQL:
SELECT DISTINCT "ClienteId" FROM "Venta" WHERE "ClienteId" = 
  (SELECT "Id" FROM "Cliente" WHERE "DNI" = '00000000');
# ESPERADO: 1 ClienteId (todas las ventas usan el mismo)
```

### Criterios de Aceptación
- [ ] Cliente "00000000" existe en BD
- [ ] Venta puede usar cliente genérico
- [ ] No hay duplicados
- [ ] Múltiples ventas usan el mismo cliente

---

## CASO 6: Índices en Tabla Venta

### Pre-requisito
- Migración aplicada
- Índices creados en BD

### Pasos de Prueba

```bash
# TEST 6.1: Índices existen en BD
# Ejecutar en SQL:
SELECT indexname FROM pg_indexes 
WHERE tablename = 'Venta' AND indexname LIKE 'IX_Venta%';
# ESPERADO: 
# - IX_Venta_VendedorId
# - IX_Venta_ClienteId
# - IX_Venta_MetodoPagoId
# - IX_Venta_VendedorId_Fecha (si se crea)

# TEST 6.2: Índices mejoran performance
# Crear 1000 ventas (script de bulk insert)
# Antes: SELECT * FROM "Venta" WHERE "VendedorId" = 1 LIMIT 1;
# Verificar EXPLAIN en Supabase
# ESPERADO: Index Scan (no Seq Scan)

# TEST 6.3: Dashboard carga rápido
# 1. Admin -> Inicio
# 2. Medir tiempo de carga en Network tab (DevTools)
# ESPERADO: Carga en < 2 segundos (con 10k+ ventas)
```

### Criterios de Aceptación
- [ ] Todos los índices creados
- [ ] Índices usados en queries (EXPLAIN)
- [ ] Performance mejorada

---

## CASO 7: Validación Email Duplicado en Vendedor

### Pre-requisito
- Validación implementada en VendedoresController.Create()

### Pasos de Prueba

```bash
# TEST 7.1: Email duplicado rechazado
# 1. Admin -> Vendedores -> Crear Nuevo
# 2. Nombres: "Test"
# 3. Apellidos: "Duplicate"
# 4. DNI: "99999999"
# 5. Email: "admin@fashionstore.com" (ya existe)
# 6. Contraseña: "Test123!"
# 7. Click "Crear"
# ESPERADO: Error "Email ya registrado en sistema"

# TEST 7.2: Email único se acepta
# 1. Admin -> Vendedores -> Crear Nuevo
# 2. Nombres: "Test2"
# 3. Apellidos: "New"
# 4. DNI: "98888888"
# 5. Email: "test2@example.com"
# 6. Contraseña: "Test123!"
# 7. Click "Crear"
# ESPERADO: ✓ Vendedor creado exitosamente

# TEST 7.3: Vendedor nuevo puede iniciar sesión
# 1. Logout (si Admin)
# 2. Login con test2@example.com / Test123!
# ESPERADO: Login exitoso
```

### Criterios de Aceptación
- [ ] Email duplicado rechazado con mensaje específico
- [ ] Email único se crea
- [ ] Vendedor nuevo puede iniciar sesión

---

## CASO 8: Coherencia DateTime (UtcNow)

### Pre-requisito
- DateTime.UtcNow usado en Venta y DescuentoAutorizado
- Verificación en todas las entidades críticas

### Pasos de Prueba

```bash
# TEST 8.1: Venta usa UtcNow
# 1. Admin -> Ventas -> Nueva Venta
# 2. Crear venta con fecha actual
# 3. Ejecutar en SQL:
SELECT "Fecha" FROM "Venta" ORDER BY "Id" DESC LIMIT 1;
# ESPERADO: Fecha en UTC (sin timezone info)

# TEST 8.2: DescuentoAutorizado usa UtcNow
# 1. Admin -> Descuentos -> Crear
# 2. Nombre: "Test Descuento"
# 3. Tipo: "Porcentaje", Valor: 10
# 4. Click "Crear"
# 5. SQL:
SELECT "FechaCreacion" FROM "DescuentoAutorizado" WHERE "Nombre" = 'Test Descuento';
# ESPERADO: Fecha en UTC

# TEST 8.3: Reportes por fecha funcionan
# 1. Admin -> Reportes
# 2. Filtrar por rango de fechas (hoy)
# ESPERADO: Todas las ventas del día aparecen
# (incluso si creadas en diferentes horas UTC)
```

### Criterios de Aceptación
- [ ] Venta usa UtcNow
- [ ] DescuentoAutorizado usa UtcNow
- [ ] Reportes por fecha funcionan correctamente

---

## CASO 9: ToggleEstado - Sincronización Usuario/Vendedor

### Pre-requisito
- ToggleEstado refactorizado con validación
- Lanza excepción si usuario no encontrado

### Pasos de Prueba

```bash
# TEST 9.1: Desactivar vendedor desactiva usuario
# 1. Admin -> Vendedores -> Ana Usuario
# 2. Click botón "Desactivar" (toggle estado)
# ESPERADO: ✓ Vendedor desactivado
# SQL: SELECT "Id", "Estado", "Activo" FROM "Vendedor" v 
#      INNER JOIN "AspNetUsers" u ON v."Correo" = u."Email"
#      WHERE v."DNI" = '82030000';
# ESPERADO: Estado=false, Activo=false (ambos sincronizados)

# TEST 9.2: Usuario desactivado NO puede iniciar sesión
# 1. Logout
# 2. Intentar login como ana@fashionstore.com / AnaVendor123!
# ESPERADO: Login rechazado (usuario inactivo)

# TEST 9.3: Reactivar vendedor reactiva usuario
# 1. Admin -> Vendedores -> Ana Usuario
# 2. Click botón "Activar" (toggle estado)
# ESPERADO: ✓ Vendedor reactivado
# 3. Logout, Intentar login
# ESPERADO: Login exitoso

# TEST 9.4: Excepción si correo vacío
# Editar vendedor, dejar Correo en blanco
# Click "Desactivar"
# ESPERADO: Error "Vendedor sin correo no puede ser desactivado"
```

### Criterios de Aceptación
- [ ] Desactivar vendedor desactiva usuario Identity
- [ ] Usuario desactivado NO puede iniciar sesión
- [ ] Reactivar funcionausario puede iniciar sesión
- [ ] Excepción si correo vacío

---

## CASO 10: Auditoría de Cambios Críticos

### Pre-requisito
- Tabla AuditoriaOperacion creada
- Middleware de auditoría implementado

### Pasos de Prueba

```bash
# TEST 10.1: Cambio de contraseña se registra
# 1. Admin -> Vendedores -> Edit (Ana)
# 2. Cambiar contraseña a "NewPass123!"
# 3. SQL:
SELECT * FROM "AuditoriaOperacion" 
WHERE "Accion" = 'CambiarPassword' 
ORDER BY "Fecha" DESC LIMIT 1;
# ESPERADO: 1 registro con UsuarioId (Admin), Entidad="Vendedor", EntidadId=1

# TEST 10.2: Creación de descuento se registra
# 1. Admin -> Descuentos -> Crear
# 2. "Test Audit", 5%
# 3. SQL:
SELECT * FROM "AuditoriaOperacion" 
WHERE "Accion" = 'CrearDescuento' 
ORDER BY "Fecha" DESC LIMIT 1;
# ESPERADO: 1 registro con UsuarioId (Admin)

# TEST 10.3: JSON de cambios capturados
# Ejecutar SQL anterior
# Verificar DatosAntes y DatosDespues contengan JSON válido
# ESPERADO: 
# DatosAntes: {"Nombres":"Ana","Apellidos":"Usuario",...}
# DatosDespues: {"Nombres":"Ana",...}

# TEST 10.4: Vendedor NO ve auditoría de otros
# 1. Login como vendedor (ana@fashionstore.com)
# 2. Intentar acceder /Admin/Auditoria
# ESPERADO: AccessDenied
```

### Criterios de Aceptación
- [ ] Cambios de password registrados
- [ ] Cambios de descuentos registrados
- [ ] JSON de datos antes/después capturado
- [ ] Vendedor NO puede ver auditoría de otros

---

## CHECKLIST DE VALIDACIÓN FINAL

```
SEGURIDAD CRÍTICA (Fase 1)
[ ] Credenciales removidas de JSON
[ ] .gitignore actualizado
[ ] Environment variables requeridas
[ ] UltimaPasswordAdmin removido
[ ] Admin passwords no hardcodeados
[ ] DescuentosController autorizado

ARQUITECTURA Y LÓGICA (Fase 2)
[ ] Cliente genérico creado
[ ] DescuentosController usa UnitOfWork
[ ] Índices en Venta creados
[ ] Email duplicado validado
[ ] Auditoría implementada

OPTIMIZACIÓN (Fase 3)
[ ] Cliente.DNI validado y Unique
[ ] DateTime coherente
[ ] ToggleEstado sincronizado
[ ] Performance mejorada
[ ] Tests 100% pasados

COMPILACIÓN Y TESTS
[ ] dotnet build: 0 errores
[ ] dotnet test: 285/285 pasados
[ ] No hay warnings relacionados a cambios
[ ] Performance tests pasan
```

---

**Documento de Validación:** Kiro AI - QA Senior  
**Última Actualización:** 7 Julio 2026

