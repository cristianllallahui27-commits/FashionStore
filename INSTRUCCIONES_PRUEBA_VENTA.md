# 📋 INSTRUCCIONES PASO A PASO: Registrar una Venta

## 🎯 OBJETIVO
Verificar que el dropdown de Métodos de Pago funciona y puede registrarse una venta exitosamente.

---

## ✅ PRE-REQUISITOS

- [ ] Servidor corriendo: `http://localhost:5100`
- [ ] Base de datos Supabase conectada
- [ ] Métodos de pago insertados en BD (5 registros)

---

## 📱 PASOS A SEGUIR

### PASO 1: ABRIR PÁGINA DE LOGIN
```
URL: http://localhost:5100/Identity/Account/Login
```

**Pantalla esperada:** Formulario de login con campos Email y Contraseña

---

### PASO 2: INGRESAR CREDENCIALES
```
Email: Admin@gmail.com
Contraseña: Admin123!
Recordar sesión: ☐ (opcional)
```

**Clic en:** "Iniciar Sesión" (botón verde)

**Resultado esperado:** Redirecciona a Dashboard o Inicio

---

### PASO 3: NAVEGAR A NUEVA VENTA (POS)

**Opción A - Menú:**
1. Clic en dropdown "Admin" (esquina superior)
2. Seleccionar "Ventas"
3. Clic en "Nueva Venta (POS)"

**Opción B - URL directa:**
```
http://localhost:5100/Ventas/Create
```

**Resultado esperado:** Se abre página de Punto de Venta (POS)

---

### PASO 4: LLENAR FORMULARIO DE VENTA

#### 4.1 - SELECCIONAR VENDEDOR
```
Campo: "Vendedor *"
Acción: Click en dropdown y selecciona cualquier vendedor
Ejemplo: "Admin Admin"
```

#### 4.2 - SELECCIONAR CLIENTE
```
Campo: "Cliente *"
Opción 1: Clic en botón "Cliente genérico" (usuario icon)
Opción 2: Seleccionar de la lista
Resultado: Se marca automáticamente en el campo
```

#### 4.3 - AGREGAR PRODUCTOS AL CARRITO
```
Sección: "Catálogo de Productos"
1. En tabla "Productos disponibles", busca un producto
   - Puedes usar: Buscador de nombre
   - O: Scanner de código de barras
   
2. Cuando encuentres un producto con STOCK > 0:
   - Clic en botón "Agregar" (verde)
   
3. Producto aparece en tabla "Productos seleccionados por pagar"
   - Ajusta cantidad si es necesario
   - Puedes agregar más productos
```

**Mínimo requerido:** Al menos 1 producto con cantidad ≥ 1

---

### ⭐ PASO 5: SELECCIONAR MÉTODO DE PAGO (CRÍTICO)

```
Campo: "Método de Pago *"
Ubicación: Panel derecho "Resumen de Cobro"
```

**🔍 VERIFICACIÓN IMPORTANTE:**

Clic en dropdown. Deberías ver exactamente estas 5 opciones:

- [ ] Efectivo
- [ ] Tarjeta de Crédito
- [ ] Tarjeta de Débito
- [ ] Transferencia Bancaria
- [ ] Cheque

**Si el dropdown está vacío:** ❌ PROBLEMA
- Reinicia el servidor
- Contacta al soporte

**Si ves las 5 opciones:** ✅ ÉXITO

**Selecciona:** "Efectivo" (para demostración)

---

### PASO 6: INGRESAR MONTO RECIBIDO (si seleccionó Efectivo)

```
Cuando selecciones "Efectivo":
- Aparece sección verde "Monto Recibido *"
- Ingresa cantidad MAYOR al total
  
Ejemplo:
- Total: S/. 49.99
- Monto Recibido: 100.00

Sistema calcula automáticamente:
- Vuelto: S/. 50.01 ✓
```

**Campo requerido:** Monto Recibido > Total

---

### PASO 7: REGISTRAR VENTA

```
Botón: "Registrar Venta" (verde, esquina inferior derecha)
```

**Verificación:**
- Debe estar HABILITADO (no grisado)
- Si está grisado: Verifica que todos los campos obligatorios estén llenos

**Clic en botón**

---

### PASO 8: VERIFICAR RESULTADO

#### Resultado Esperado A: ✅ ÉXITO

```
1. Popup SweetAlert aparece:
   - Ícono: ✓ (éxito)
   - Título: "Venta Registrada"
   - Mensaje: "La venta se ha procesado correctamente"
   
2. Clic en "Ver Detalle / Comprobante"
   
3. Redirecciona a página:
   URL: http://localhost:5100/Ventas/Details/{ventaId}
   
4. Página muestra:
   - Número de venta
   - Fecha y hora
   - Cliente
   - Vendedor
   - Productos (con cantidades)
   - Método de pago
   - Total
```

#### Resultado Esperado B: ❌ ERROR

```
1. Popup de error aparece:
   - Ícono: ✗ (error)
   - Título: "Error de Venta"
   - Mensaje: Descripción del problema
   
2. Posibles causas:
   - Stock insuficiente
   - Método de pago inválido
   - Error en base de datos
   
3. Verifica:
   - F12 → Console para mensajes de error
   - Logs del servidor
```

---

## 🔍 VERIFICACIONES ADICIONALES

### Verificar en Consola del Navegador (F12):
```
Clic en: F12 → Console tab

No debe haber errores rojo tipo:
❌ "404 not found"
❌ "Method not found"
❌ "undefined"

Si ves errores: Captura pantalla y reporta
```

### Verificar en Base de Datos:

```sql
-- Verificar que venta se registró
SELECT TOP 5 * FROM "Ventas" ORDER BY "Fecha" DESC;

-- Verificar método de pago fue guardado
SELECT "MetodoPagoId" FROM "Ventas" WHERE "Id" = <ventaId>;

-- Verificar detalles de venta
SELECT * FROM "DetalleVentas" WHERE "VentaId" = <ventaId>;
```

---

## ✅ CHECKLIST DE ACEPTACIÓN

Marca todo como completado:

- [ ] Login exitoso
- [ ] Página POS carga
- [ ] Dropdown Métodos muestra 5 opciones
- [ ] Se agregó producto al carrito
- [ ] Se seleccionó método de pago
- [ ] Se ingresó monto recibido
- [ ] Botón "Registrar Venta" está habilitado
- [ ] Se hizo clic en botón
- [ ] Popup de éxito aparece
- [ ] Redirecciona a Details
- [ ] Página Details muestra venta correctamente
- [ ] En BD aparece la venta

**Si todo ✅:** PRUEBA EXITOSA - SISTEMA FUNCIONA CORRECTAMENTE

---

## 🆘 SOPORTE RÁPIDO

| Problema | Solución |
|----------|----------|
| No puedo loguearme | Verifica credenciales: Admin@gmail.com / Admin123! |
| Página POS no carga | Reinicia servidor, limpia caché (Ctrl+Shift+R) |
| Dropdown vacío | Reinicia servidor, verifica BD |
| Botón deshabilitado | Revisa que vendedor, cliente, método y producto estén seleccionados |
| Error al registrar | Revisa F12 console, verifica stock, verifica método pago existe |
| No redirige | Error 500 probable, revisa logs del servidor |

---

## 📸 PANTALLAZOS ESPERADOS

### Pantalla 1: Login
```
[Email field] Admin@gmail.com
[Password field] ••••••••••••
[Checkbox] Recordar sesión
[Button] Iniciar Sesión
```

### Pantalla 2: POS - Dropdown Métodos
```
Método de Pago *
┌─────────────────────────────┐
│ Efectivo                    │ ✓ VISIBLE
│ Tarjeta de Crédito          │ ✓ VISIBLE
│ Tarjeta de Débito           │ ✓ VISIBLE
│ Transferencia Bancaria      │ ✓ VISIBLE
│ Cheque                      │ ✓ VISIBLE
└─────────────────────────────┘
```

### Pantalla 3: Confirmación
```
╔════════════════════════════╗
║ ✓ Venta Registrada        ║
║                            ║
║ La venta se ha procesado   ║
║ correctamente.             ║
║                            ║
║ [Ver Detalle / Comprobante]║
╚════════════════════════════╝
```

---

## 📞 CONTACTO

Si encontras problemas que no puedas resolver:
1. Captura pantallazos del error
2. Anota el URL exacto
3. Revisa F12 Console para mensajes de error
4. Contacta al equipo técnico

---

**Fecha de creación:** 13/07/2026  
**Versión:** 1.0  
**Estado:** Producción
