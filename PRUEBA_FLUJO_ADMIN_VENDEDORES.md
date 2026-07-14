# PRUEBA FUNCIONAL: Admin Asigna Acceso a Vendedores
## FashionStoreSolution - Flujo Completo

**Aplicación:** Iniciada en `http://localhost:5100`  
**Fecha:** 13 Julio 2026  
**Objetivo:** Verificar que Admin puede crear contraseñas para vendedores y que vendedores pueden iniciar sesión

---

## PASO 1: Iniciar Sesión como Admin

**URL:** `http://localhost:5100/Identity/Account/Login`

```
Usuario: admin@fashionstore.com
Contraseña: Password123!
```

**Esperado:**
- ✅ Login exitoso
- ✅ Redirige a Dashboard
- ✅ Navbar muestra "Admin" (usuario autenticado)
- ✅ Botón "Vendedores" visible en Admin menu

---

## PASO 2: Navegar a Gestión de Vendedores

**Ruta:** Admin → Vendedores  
**URL:** `http://localhost:5100/Vendedores`

**Esperado:**
- ✅ Tabla de vendedores carga
- ✅ Se ven 6 vendedores listados:
  1. Ana Usuario (DNI: 82030000)
  2. Ana Vallejo (DNI: 83051544)
  3. Carlos Mendoza (DNI: 92031122)
  4. Diego Morena (DNI: 93880000)
  5. Luis Castro (DNI: 92908566)
  6. Sofia Ruiz (DNI: 83807768)

---

## PASO 3: Editar Primer Vendedor (Ana Usuario)

**Acción:** Click en icono lápiz (Edit) en fila de Ana Usuario

**URL esperada:** `http://localhost:5100/Vendedores/Edit/1`

**Esperado:**
- ✅ Formulario carga
- ✅ Campos visibles:
  - Nombres: "Ana"
  - Apellidos: "Usuario"
  - DNI: "82030000"
  - Teléfono: (campo)
  - Correo: "ana@fashionstore.com"
- ✅ Panel "Cambiar Contraseña de Acceso" visible
  - Botón "Cambiar Contraseña"
  - Campo de entrada para nueva contraseña

---

## PASO 4: Asignar Contraseña a Ana Usuario

**Campo:** "Nueva Contraseña"  
**Ingresa:** `AnaVendedor123!`

**Acción:** Click botón "Cambiar Contraseña"

**Esperado:**
- ✅ Mensaje: "✓ Contraseña actualizada correctamente para Ana Usuario"
- ✅ No hay error
- ✅ Página redirecciona a Vendedores (Index)

**Verificación BD (SQL):**
```sql
SELECT "Id", "Nombres", "Apellidos", "Correo", "Estado"
FROM "Vendedor"
WHERE "DNI" = '82030000';

-- Esperado: 1 registro con Estado = true
```

---

## PASO 5: Editar Segundo Vendedor (Sofia Ruiz)

**Acción:** Click en icono lápiz (Edit) en fila de Sofia Ruiz

**URL esperada:** `http://localhost:5100/Vendedores/Edit/6`

**Esperado:**
- ✅ Formulario carga con datos de Sofia
  - Nombres: "Sofia"
  - Apellidos: "Ruiz"
  - Correo: "sofia@fashionstore.com"

---

## PASO 6: Asignar Contraseña a Sofia Ruiz

**Campo:** "Nueva Contraseña"  
**Ingresa:** `SofiaVendedor456!`

**Acción:** Click botón "Cambiar Contraseña"

**Esperado:**
- ✅ Mensaje: "✓ Contraseña actualizada correctamente para Sofia Ruiz"
- ✅ Redirecciona a Vendedores

---

## PASO 7: Logout (Admin cierra sesión)

**Acción:** Click en username (top-right) → Logout

**URL:** `http://localhost:5100/Identity/Account/Logout`

**Esperado:**
- ✅ Sesión se cierra
- ✅ Redirige a `/Identity/Account/Login`

---

## PASO 8: Login como ANA USUARIO

**URL:** `http://localhost:5100/Identity/Account/Login`

```
Usuario: ana@fashionstore.com
Contraseña: AnaVendedor123!
```

**Esperado:**
- ✅ Login exitoso
- ✅ Redirige a Dashboard
- ✅ Rol mostrado: "Vendedor" (no Admin)
- ✅ Navbar muestra menú limitado (NO ve "Admin" menu)

**Verificación:**
- ❌ Menu "Admin" NO debe aparecer
- ❌ Link "Vendedores" NO debe aparecer
- ✅ Link "Ventas" SÍ debe aparecer
- ✅ Link "Inicio" SÍ debe aparecer

---

## PASO 9: Vendedor Ana - Acceder a Ventas

**Acción:** Click en "Ventas" → "Ver todas"

**URL:** `http://localhost:5100/Ventas`

**Esperado:**
- ✅ Carga tabla de ventas
- ✅ **SOLO** muestra ventas de Ana Usuario (VendedorId = 1)
- ✅ No ve ventas de otros vendedores
- ✅ Totales mostrados son solo de Ana:
  - Total Ventas: X
  - Ingresos Hoy: $Y
  - Total Ingresos: $Z

**Verificación SQL (ejecutar como admin):**
```sql
-- Ventas que Ana debería ver
SELECT COUNT(*) FROM "Venta" WHERE "VendedorId" = 1;

-- Verificar que Ana NO ve ventas de otros
SELECT DISTINCT "VendedorId" FROM "Venta" 
WHERE "VendedorId" NOT IN (1);
-- No debe aparecer en la tabla del vendedor
```

---

## PASO 10: Verificar que Vendedor NO puede cambiar Contraseña

**Acción:** Intentar acceder directamente a:  
`http://localhost:5100/Vendedores/Edit/1`

**Esperado:**
- 🔴 Acceso denegado (AccessDenied)
- 🔴 Error 403 Forbidden
- 🔴 Mensaje: "No tienes permiso para acceder a este recurso"
- ✅ Redirecciona a `/Identity/Account/AccessDenied`

---

## PASO 11: Logout Ana y Login como SOFIA RUIZ

**Acción:** Logout

**URL:** `http://localhost:5100/Identity/Account/Login`

```
Usuario: sofia@fashionstore.com
Contraseña: SofiaVendedor456!
```

**Esperado:**
- ✅ Login exitoso
- ✅ Dashboard carga
- ✅ Rol: "Vendedor"
- ✅ Solo ve sus propias ventas

---

## PASO 12: Sofia - Verificar que NO puede acceder a Vendedores

**Acción:** Intentar acceder a:  
`http://localhost:5100/Vendedores`

**Esperado:**
- 🔴 Acceso denegado (AccessDenied)
- 🔴 Mensaje de error de autorización
- 🔴 Redirecciona a `/Identity/Account/AccessDenied`

---

## PASO 13: Login como Admin nuevamente para cambiar contraseña de Ana

**Logout Sofia**

**Login:**
```
Usuario: admin@fashionstore.com
Contraseña: Password123!
```

**URL:** `http://localhost:5100/Vendedores/Edit/1`

---

## PASO 14: Admin Cambia Contraseña de Ana a Nueva Clave

**Campo:** "Nueva Contraseña"  
**Ingresa:** `AnaVendedor789!` (NUEVA contraseña)

**Acción:** Click "Cambiar Contraseña"

**Esperado:**
- ✅ Mensaje: "✓ Contraseña actualizada correctamente para Ana Usuario"
- ✅ Ana debe usar la NUEVA contraseña para iniciar sesión siguiente

---

## PASO 15: Verificar que Ana Usa Nueva Contraseña

**Logout Admin**

**Intentar login con CONTRASEÑA ANTIGUA:**
```
Usuario: ana@fashionstore.com
Contraseña: AnaVendedor123!  ← ANTIGUA
```

**Esperado:**
- 🔴 Login falla
- 🔴 Mensaje: "Email o contraseña inválidos"

---

## PASO 16: Login Ana con Contraseña Nueva

```
Usuario: ana@fashionstore.com
Contraseña: AnaVendedor789!  ← NUEVA
```

**Esperado:**
- ✅ Login exitoso
- ✅ Dashboard carga correctamente

---

## PASO 17: Cambiar Contraseña de Sofia Nuevamente

**Admin Login:** `admin@fashionstore.com` / `Password123!`

**Navegar a:** `http://localhost:5100/Vendedores/Edit/6`

**Campo:** "Nueva Contraseña"  
**Ingresa:** `SofiaVendedor999!`

**Acción:** Click "Cambiar Contraseña"

**Esperado:**
- ✅ Mensaje: "✓ Contraseña actualizada correctamente para Sofia Ruiz"

---

## CHECKLIST DE VALIDACIÓN FINAL

```
PASO 1-2: Acceso Admin
[ ] Admin login exitoso
[ ] Página Vendedores carga
[ ] 6 vendedores listados

PASO 3-6: Asignar Contraseñas
[ ] Editar Ana Usuario funciona
[ ] Contraseña AnaVendedor123! asignada
[ ] Editar Sofia Ruiz funciona
[ ] Contraseña SofiaVendedor456! asignada

PASO 7-9: Ana Usuario Acceso
[ ] Ana login exitoso con AnaVendedor123!
[ ] Dashboard carga
[ ] NO ve menu "Admin"
[ ] NO puede acceder a /Vendedores
[ ] Solo ve sus propias ventas

PASO 10-12: Sofia Ruiz Acceso
[ ] Sofia login exitoso con SofiaVendedor456!
[ ] Dashboard carga
[ ] NO puede acceder a /Vendedores/Edit/6
[ ] Acceso denegado

PASO 13-16: Cambio de Contraseña
[ ] Admin cambia contraseña de Ana a AnaVendedor789!
[ ] Ana login falla con contraseña ANTIGUA
[ ] Ana login exitoso con contraseña NUEVA
[ ] Sofia contraseña cambiada a SofiaVendedor999!

SEGURIDAD VERIFICADA
[ ] Solo Admin puede cambiar contraseñas
[ ] Vendedores no ven página de gestión
[ ] Vendedores solo ven sus propias ventas
[ ] Cambio de contraseña invalida antigua clave
[ ] Sin exposición de contraseña plaintext
```

---

## NOTAS IMPORTANTES

### Permisos y Roles

| Acción | Admin | Vendedor |
|--------|-------|----------|
| Ver Vendedores | ✅ Sí | ❌ No |
| Editar Vendedor | ✅ Sí | ❌ No |
| Cambiar Contraseña | ✅ Sí | ❌ No |
| Ver Ventas Propias | ✅ Sí | ✅ Sí |
| Ver Ventas de Otros | ✅ Sí | ❌ No |
| Crear Venta | ✅ Sí | ✅ Sí |
| Ver Dashboard | ✅ Sí | ✅ Sí |

### Contraseñas Utilizadas

| Usuario | Email | Contraseña Original | Contraseña 1 | Contraseña 2 |
|---------|-------|---------------------|--------------|--------------|
| Admin | admin@fashionstore.com | Password123! | - | - |
| Ana Usuario | ana@fashionstore.com | (sin acceso) | AnaVendedor123! | AnaVendedor789! |
| Sofia Ruiz | sofia@fashionstore.com | (sin acceso) | SofiaVendedor456! | SofiaVendedor999! |

### Base de Datos

**Tabla Vendedor:**
- Id = 1: Ana Usuario (DNI: 82030000)
- Id = 6: Sofia Ruiz (DNI: 83807768)

**Tabla AspNetUsers:**
- Email = ana@fashionstore.com → Rol: Vendedor
- Email = sofia@fashionstore.com → Rol: Vendedor
- Email = admin@fashionstore.com → Rol: Administrador

**Tabla Venta:**
- Se filtra por VendedorId según usuario autenticado
- Vendedor solo ve donde VendedorId = su Id

---

## PRUEBA DE SEGURIDAD

### Test 1: SQL Injection en Login
```
Email: ' OR '1'='1
Password: cualquiera
```
**Esperado:** 🔴 Falsa (Identity maneja esto de forma segura)

### Test 2: Acceso directo sin autenticar
```
URL: http://localhost:5100/Vendedores
```
**Esperado:** 🔴 Redirecciona a /Identity/Account/Login

### Test 3: Vendedor intenta cambiar a Admin
```
1. Vendedor login
2. URL directo: /Vendedores/Edit/1
```
**Esperado:** 🔴 AccessDenied (403)

### Test 4: Cambio de contraseña invalida la antigua
```
1. Ana: Password AnaVendedor123!
2. Admin cambia a AnaVendedor789!
3. Intenta login con AnaVendedor123!
```
**Esperado:** 🔴 Falla (Identity hash cambió)

---

## CONCLUSIÓN

Si todos los checkpoints pasan:

✅ **Sistema de autenticación funciona correctamente**  
✅ **Autorización por roles funciona correctamente**  
✅ **Admin puede gestionar contraseñas de vendedores**  
✅ **Vendedores solo ven su información**  
✅ **Seguridad de acceso está implementada**  

**Aplicación LISTA para validación de negocio** 🚀

---

**Documento de Prueba:** Kiro AI - QA Tester  
**Generado:** 13 Julio 2026  
**Estado:** Listo para ejecución manual

