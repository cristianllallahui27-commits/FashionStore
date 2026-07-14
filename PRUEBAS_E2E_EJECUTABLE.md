# PRUEBAS E2E - GUÍA EJECUTABLE
## FashionStoreSolution - Flujo Vendedor Completo

**Estado:** LISTO PARA PRUEBA MANUAL  
**Servidor:** http://localhost:5100  
**Última Actualización:** 13 Julio 2026  

---

## PRE-REQUISITOS

✅ Servidor corriendo: `dotnet run --configuration Release` en FashionStore.Web  
✅ Base de datos PostgreSQL conectada (Supabase)  
✅ Navegador moderno (Chrome/Edge/Firefox)  

---

## TEST SUITE 1: AUTENTICACIÓN Y ACCESO

### TEST 1.1 - Admin Login
```
PASO: Abrir navegador → http://localhost:5100
RESULTADO ACTUAL: [  ] Login page
CREDENCIALES:
  Email: admin@fashionstore.com
  Password: Password123!

PASOS:
1. Ingresa email en campo "Email"
2. Ingresa password en campo "Password"  
3. Click "Iniciar sesión" (Entrar)
4. Espera página carga

✓ ÉXITO si: 
  - Dashboard visible con 6 gráficos
  - Navbar muestra usuario "admin@fashionstore.com"
  - Rol "Administrador" visible en badge
  
✗ FALLO si:
  - Redirige a Login again
  - Error 401 / 403
  - Dashboard no carga
```

**Resultado Esperado:** ✓ Dashboard Admin  
**Tiempo Límite:** 5 segundos  
**Estado:** [ ] PASS  [ ] FAIL  

---

### TEST 1.2 - Admin Navbar Visible
```
PASO: Verificar que navbar tiene menú Admin

ELEMENTOS ESPERADOS EN NAVBAR:
✓ Inicio (casa icon)
✓ Catálogo (dropdown)
✓ Admin (dropdown)
✓ Ventas (dropdown)
✓ Usuario (top-right)

CLICK: Admin dropdown
DEBE MOSTRAR:
  - Vendedores
  - Reportes
  - Descuentos
  - Configuración
  - Auditoría (si existe)
```

**Estado:** [ ] PASS  [ ] FAIL  

---

## TEST SUITE 2: GESTIÓN DE VENDEDORES

### TEST 2.1 - Listar Vendedores
```
ACCIÓN: Click Navbar → Admin → Vendedores

RESULTADO ESPERADO:
✓ Página "/Vendedores" carga
✓ Tabla muestra 6 vendedores:
  1. Ana Usuario (DNI: 82030000, Email: ana@fashionstore.com)
  2. Ana Vallejo (DNI: 83051544)
  3. Carlos Mendoza (DNI: 92031122)
  4. Diego Morena (DNI: 93880000)
  5. Luis Castro (DNI: 92908566)
  6. Sofia Ruiz (DNI: 83807768)

✓ Columnas visibles: Nombres, DNI, Teléfono, Correo, Estado, Acciones
✓ Botón "+ Nuevo Vendedor" visible (top-right)
✓ Cada fila tiene icono Editar (lápiz) y toggle Estado

TIEMPO LÍMITE: 3 segundos
```

**Estado:** [ ] PASS  [ ] FAIL  

---

### TEST 2.2 - Editar Vendedor (Ana Usuario)
```
ACCIÓN: Click en icono lápiz (Edit) para "Ana Usuario"

RESULTADO ESPERADO:
✓ Abre página "/Vendedores/Edit/1" (o ID correcto)
✓ Formulario contiene:
  - Nombres: "Ana" (pre-llena)
  - Apellidos: "Usuario" (pre-llena)
  - DNI: "82030000" (pre-llena)
  - Teléfono: (vacío o valor)
  - Correo: "ana@fashionstore.com" (pre-llena)
  - Toggle Estado: ON (verde)

✓ Abajo del formulario está panel amarillo:
  "Cambiar Contraseña de Acceso" (solo visible para Admin)
  
✓ Panel contiene:
  - Texto: "Como administrador, puedes asignar..."
  - Campo "Nueva Contraseña" (type=password)
  - Botón mostrar contraseña (eye icon)
  - Botón "Actualizar Contraseña" (amarillo)

TIEMPO LÍMITE: 2 segundos
```

**Estado:** [ ] PASS  [ ] FAIL  

---

### TEST 2.3 - Asignar Contraseña a Vendedor
```
ACCIÓN: Asignar contraseña a Ana Usuario

PASOS:
1. En el campo "Nueva Contraseña" ingresa: AnaVendor123!
2. Click botón "Actualizar Contraseña" (amarillo)
3. Espera respuesta del servidor

RESULTADO ESPERADO:
✓ Página redirige a "/Vendedores"
✓ Mensaje VERDE aparece (AlertSuccess):
  "✓ Contraseña actualizada correctamente para Ana Usuario."
✓ Mensaje desaparece después de 5 segundos (auto-dismiss)
✓ Tabla de vendedores está intacta

VALIDACIÓN EN BD:
✓ ApplicationUser.PasswordHash de ana@fashionstore.com está actualizado
✓ Vendedor.UltimaPasswordAdmin = "AnaVendor123!" (para auditoría del Admin)

TIEMPO LÍMITE: 3 segundos
```

**Estado:** [ ] PASS  [ ] FAIL  

---

## TEST SUITE 3: AUTENTICACIÓN DE VENDEDOR

### TEST 3.1 - Logout Admin
```
ACCIÓN: Click en usuario (top-right) → Logout

RESULTADO ESPERADO:
✓ Redirige a "/Identity/Account/Login"
✓ Login form visible
✓ Ningún usuario autenticado (sesión cookie eliminada)

TIEMPO LÍMITE: 2 segundos
```

**Estado:** [ ] PASS  [ ] FAIL  

---

### TEST 3.2 - Login Vendedor con Nueva Contraseña
```
ACCIÓN: Iniciar sesión como Ana Usuario

CREDENCIALES:
  Email: ana@fashionstore.com
  Password: AnaVendor123!

PASOS:
1. Ingresa email
2. Ingresa contraseña
3. Click "Iniciar sesión"
4. Espera redirección

RESULTADO ESPERADO:
✓ Redirige a Dashboard (/Home/Index)
✓ Dashboard carga (NO redirige a Login)
✓ Navbar muestra usuario "ana@fashionstore.com"
✓ Rol "Vendedor" visible (si está implementado)
✓ Opciones de Vendedor disponibles:
  - Ventas (crear, listar)
  - Reportes (sus propias ventas)

NEGATIVO (¿QUÉ NO DEBE PASAR?):
✗ No aparecer "Administrador" en menú
✗ No acceso a "/Descuentos" (si hace click, debe error 403)
✗ No acceso a "/Configuracion" (error 403)

TIEMPO LÍMITE: 5 segundos
```

**Estado:** [ ] PASS  [ ] FAIL  

---

## TEST SUITE 4: FLUJO DE VENTAS

### TEST 4.1 - Acceder a Nueva Venta
```
ACCIÓN: Click Navbar → Ventas → Nueva Venta

RESULTADO ESPERADO:
✓ Abre página "/Ventas/Create"
✓ Formulario contiene:

SECCIÓN VENDEDOR:
  - Campo "Vendedor": Pre-llena con "Ana Usuario" (auto-detect de sesión)
  - Indicador: "Detectado de tu sesión"

SECCIÓN CLIENTE:
  - Campo "Cliente" (dropdown o search)
  - Opción "Mostrador" (cliente genérico DNI: 00000000)

SECCIÓN PRODUCTOS:
  - Grid de productos disponibles
  - Cada producto con: Nombre, Categoría, Precio, Stock, Cantidad (input)

SECCIÓN MÉTODO PAGO:
  - Dropdown "Método de Pago"
  - Opciones: Efectivo, Tarjeta Crédito, Débito, Transferencia, Cheque

SECCIÓN CÁLCULO:
  - Subtotal: (calculado automático)
  - Descuento: (si Admin aplica)
  - Total: (calculado automático)
  - Monto Recibido (si Efectivo)
  - Vuelto: (calculado automático si Efectivo)

BOTÓN:
  - "Registrar Venta" (azul, al final)

TIEMPO LÍMITE: 3 segundos
```

**Estado:** [ ] PASS  [ ] FAIL  

---

### TEST 4.2 - Completar Venta (Flujo Completo)
```
ACCIÓN: Crear una venta ejemplo

PASOS:
1. CLIENTE: Click en dropdown "Cliente", selecciona "Mostrador"
   
2. PRODUCTOS: 
   - Busca un producto (ej: búsqueda "Pantalon")
   - Ingresa cantidad: 2
   - Click "Agregar al carrito"
   - Repite si quieres más productos

3. MÉTODO PAGO:
   - Click dropdown "Método de Pago"
   - Selecciona "Efectivo"

4. MONTO RECIBIDO (solo si Efectivo):
   - Campo "Monto Recibido" aparece
   - Ingresa: 500.00 (mayor al total para calcular vuelto)

5. DESCUENTO (solo si Admin):
   - Como vendedor, NO debes ver opción de descuento manual
   - Si aparece, debe estar DESHABILITADA

6. REGISTRAR:
   - Click "Registrar Venta"

RESULTADO ESPERADO:
✓ Página muestra mensaje VERDE:
  "✓ Venta registrada exitosamente"
✓ ID de venta mostrado (ej: "Venta #12345")
✓ Redirige a "/Ventas" después de 2 segundos

VALIDACIONES EN BD:
✓ Tabla Venta: nuevo registro creado
✓ Tabla DetalleVenta: registros de productos agregados
✓ Stock de producto decrementado (Prendas.Stock)
✓ Fecha = NOW (o UtcNow)

TIEMPO LÍMITE: 5 segundos
```

**Estado:** [ ] PASS  [ ] FAIL  

---

## TEST SUITE 5: NAVEGACIÓN Y REDIRECCIONES

### TEST 5.1 - Botón "Inicio" Funciona
```
ACCIÓN: Click en Navbar → Inicio (icono casa)

RESULTADO ESPERADO:
✓ Redirige a "/Home/Index" (o "/" alias)
✓ Dashboard carga
✓ NO redirige a Login
✓ Gráficos son visibles
✓ URL en barra es "http://localhost:5100/" o "http://localhost:5100/Home"

NEGATIVO (¿QUÉ NO DEBE PASAR?):
✗ No redirigir a "/Identity/Account/Login"
✗ No mostrar error 401
✗ Dashboard no debe estar en blanco

TIEMPO LÍMITE: 2 segundos
```

**Estado:** [ ] PASS  [ ] FAIL  

---

### TEST 5.2 - Acceso Denegado a Admin
```
ACCIÓN: Intenta acceder como Vendedor a "/Descuentos"

PASOS:
1. Barra de dirección: http://localhost:5100/Descuentos
2. Press Enter
3. Observa resultado

RESULTADO ESPERADO:
✓ Redirige a "/Identity/Account/AccessDenied"
✓ Mensaje: "Acceso denegado" / "Access Denied"
✓ Botón "Volver a Inicio" visible
✓ NO carga lista de descuentos

TIEMPO LÍMITE: 2 segundos
```

**Estado:** [ ] PASS  [ ] FAIL  

---

## TEST SUITE 6: REPORTES

### TEST 6.1 - Acceder a Reportes (Vendedor)
```
ACCIÓN: Click Navbar → Admin → Reportes

RESULTADO ESPERADO:
✓ Abre "/Reportes"
✓ Tabla visible con SOLO ventas de Ana Usuario
✓ Columnas:
  - ID Venta
  - Fecha
  - Cliente
  - Productos
  - Total
  - Acciones

✓ Botones por venta:
  - Descargar PDF
  - Descargar Excel
  - Ver Detalles

FILTRADO:
✓ NO muestra ventas de otros vendedores
✓ Si Admin, muestra todas (filtro por dropdown)

TIEMPO LÍMITE: 3 segundos
```

**Estado:** [ ] PASS  [ ] FAIL  

---

### TEST 6.2 - Descargar PDF
```
ACCIÓN: Click "Descargar PDF" en una venta

RESULTADO ESPERADO:
✓ Archivo descarga automático (nombre: "Reporte_Venta_12345.pdf")
✓ Archivo contiene:
  - Encabezado con logo/nombre tienda
  - Datos venta: Fecha, Cliente, Vendedor, Método Pago
  - Tabla de productos: Nombre, Cantidad, Precio, Subtotal
  - Total, Descuento, Vuelto
  - Firma/sello

✓ Archivo PDF válido (abre en lector PDF)

TIEMPO LÍMITE: 5 segundos
```

**Estado:** [ ] PASS  [ ] FAIL  

---

### TEST 6.3 - Descargar Excel
```
ACCIÓN: Click "Descargar Excel" en una venta

RESULTADO ESPERADO:
✓ Archivo descarga automático (nombre: "Reporte_Venta_12345.xlsx")
✓ Archivo contiene:
  - Hoja "Venta": datos generales (ID, Fecha, Cliente, Vendedor)
  - Hoja "Productos": tabla con Nombre, Cantidad, Precio, Subtotal
  - Hoja "Resumen": Total, Descuento, Vuelto

✓ Archivo Excel válido (abre en Excel/LibreOffice/Google Sheets)

TIEMPO LÍMITE: 5 segundos
```

**Estado:** [ ] PASS  [ ] FAIL  

---

## TEST SUITE 7: CASOS EDGE & ERRORES

### TEST 7.1 - Intento de Venta sin Stock
```
ACCIÓN: Crear venta con producto agotado

PASOS:
1. En Nueva Venta, busca producto con Stock = 0
2. Intenta agregar cantidad
3. Click "Registrar Venta"

RESULTADO ESPERADO:
✓ Mensaje ROJO:
  "Stock insuficiente para [Producto]. Disponible: 0, solicitado: 1."
✓ NO se registra venta
✓ Página no redirige

TIEMPO LÍMITE: 2 segundos
```

**Estado:** [ ] PASS  [ ] FAIL  

---

### TEST 7.2 - Cambiar Contraseña Débil
```
ACCIÓN: Admin intenta asignar contraseña corta

PASOS:
1. Edit Vendedor → Panel Cambiar Contraseña
2. Ingresa: "abc" (menos de 6 caracteres)
3. Click "Actualizar Contraseña"

RESULTADO ESPERADO:
✓ Mensaje ROJO:
  "La nueva contraseña debe tener al menos 6 caracteres."
✓ NO se actualiza contraseña
✓ Permanece en página Edit

TIEMPO LÍMITE: 2 segundos
```

**Estado:** [ ] PASS  [ ] FAIL  

---

### TEST 7.3 - Logout y Relogin
```
ACCIÓN: Logout → Login con mismas credenciales

PASOS:
1. Click Usuario → Logout
2. Espera Login page
3. Ingresa email: ana@fashionstore.com
4. Ingresa password: AnaVendor123! (contraseña anterior)
5. Click "Iniciar sesión"

RESULTADO ESPERADO:
✓ Login falla con mensaje:
  "Nombre de usuario o contraseña incorrectos."
  
EXPLICACIÓN: Contraseña cambió, datos anteriores inválidos

Ahora intenta con contraseña NUEVA:
6. Ingresa email: ana@fashionstore.com
7. Ingresa password: AnaVendor123!
8. Click "Iniciar sesión"

✓ Login exitoso, Dashboard carga

TIEMPO LÍMITE: 5 segundos (total)
```

**Estado:** [ ] PASS  [ ] FAIL  

---

## CHECKLIST FINAL

### Criterios de Aceptación - TODOS DEBEN PASAR

```
SEGURIDAD
[ ] Admin solo puede cambiar contraseñas
[ ] Vendedor no puede acceder a /Descuentos
[ ] Vendedor no puede acceder a /Configuracion
[ ] Tokens/cookies válidos, no se exponen

AUTENTICACIÓN
[ ] Admin login funciona
[ ] Vendedor login con nueva password funciona
[ ] Logout limpia sesión
[ ] Relogin con nueva password funciona

VENDEDORES
[ ] Listar vendedores carga en < 3s
[ ] Editar formulario pre-llena datos
[ ] Cambiar contraseña actualiza Identity
[ ] Mensaje de éxito aparece

VENTAS
[ ] Nueva venta pre-llena vendedor (auto-detect)
[ ] Formulario valida campos requeridos
[ ] Stock se decrementa correctamente
[ ] Vuelto se calcula si Efectivo
[ ] Mensaje de éxito después de registrar

NAVEGACIÓN
[ ] Botón "Inicio" redirige a Home
[ ] NO redirige a Login
[ ] Navbar siempre visible
[ ] URLs correctas (/Vendedores, /Ventas, etc)

REPORTES
[ ] Reportes muestra solo ventas del vendedor
[ ] PDF descarga correctamente
[ ] Excel descarga correctamente
[ ] Archivos contienen datos válidos

ERRORES
[ ] Stock insuficiente → mensaje error
[ ] Password corta → mensaje error
[ ] Logout → credenciales invalidas
[ ] Credenciales inválidas → No login
```

---

## LOGS / DEBUGGING

Si algo falla, revisar:

### Logs del Servidor
```
Terminal donde corre "dotnet run":
[20:19:03 INF] Now listening on: http://localhost:5100
[20:19:03 ERR] Error... ← AQUÍ BUSCAR ERRORES
```

### Console del Navegador (F12)
```
Errors: Network, JavaScript, CORS, 404, 500
Red (Network tab):
  - POST /Vendedores/CambiarPassword → status 200
  - POST /Ventas/Create → status 200
  - GET /Reportes → status 200
```

### Base de Datos
```
Tabla: ApplicationUser
  - Verificar PasswordHash actualizado para ana@fashionstore.com

Tabla: Vendedor
  - Verificar UltimaPasswordAdmin guarda plaintext

Tabla: Venta
  - Verificar nuevos registros después de "Registrar Venta"

Tabla: DetalleVenta
  - Verificar productos agregados

Tabla: Prenda (Stock)
  - Verificar stock decrementado después de venta
```

---

## RESUMEN DE RESULTADOS

Llenar después de ejecutar todas las pruebas:

```
TOTAL TESTS: 17
PASS: ____ / 17
FAIL: ____ / 17

PROBLEMAS ENCONTRADOS:
[ ] Ninguno
[ ] Test 2.3 - Contraseña no se actualiza
[ ] Test 4.2 - Venta genera error
[ ] Test 5.1 - Botón Inicio redirige a Login
[ ] Otro: ___________________________

RECOMENDACIONES:
_____________________________________________
_____________________________________________
_____________________________________________
```

---

**Fecha Prueba:** ___________  
**Probador:** ___________  
**Estado General:** [ ] PASÓ TODAS  [ ] FALLÓ ALGUNAS  

