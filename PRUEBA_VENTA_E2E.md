# 🧪 PRUEBA E2E: Registro de Venta Completo

## ✅ ESTADO ACTUAL

### Verificaciones Completadas:
- ✓ **Métodos de Pago insertados en BD**: 5 registros (Efectivo, Tarjeta de Crédito, Tarjeta de Débito, Transferencia Bancaria, Cheque)
- ✓ **Seed data ejecutado**: Log de inicio muestra `✓ Métodos de pago ya existen (5)`
- ✓ **Build sin errores**: `dotnet build -c Release` → 0 errores
- ✓ **Servidor corriendo**: `http://localhost:5100`
- ✓ **API productos-disponibles**: Retorna lista de productos con stock > 0
- ✓ **API metodos-pago**: Retorna lista de 5 métodos

---

## 📋 PASOS PARA PRUEBA MANUAL

### 1. Login
```
URL: http://localhost:5100/Identity/Account/Login
Email: Admin@gmail.com
Contraseña: Admin123!
```

### 2. Ir a Nueva Venta (POS)
```
Admin → Ventas → Nueva Venta (POS)
O directamente: http://localhost:5100/Ventas/Create
```

### 3. Llenar Formulario
**Paso A: Seleccionar Vendedor**
- Campo: "Vendedor *"
- Selecciona cualquier vendedor disponible

**Paso B: Seleccionar Cliente**
- Campo: "Cliente *"
- Opción rápida: Clic en botón "Cliente genérico" (usuario icono)
- O selecciona cliente de la lista

**Paso C: Agregar Productos al Carrito**
- En "Catálogo de Productos", selecciona un producto
- Clic en botón verde "Agregar"
- Verifica que aparezca en tabla "Productos seleccionados por pagar"
- Cantidad mínima: 1 (verifica stock disponible)

**Paso D: Seleccionar Método de Pago** ⭐ **CRÍTICO**
- Campo: "Método de Pago *"
- **DEBE MOSTRAR 5 OPCIONES:**
  - Efectivo
  - Tarjeta de Crédito
  - Tarjeta de Débito
  - Transferencia Bancaria
  - Cheque
- Selecciona: **"Efectivo"** (para que aparezca campo de "Monto Recibido")

**Paso E: Ingresar Monto Recibido** (solo si seleccionó Efectivo)
- Campo: "Monto Recibido *"
- Ingresa cantidad mayor al total
- Ejemplo: Si total es S/. 50.00, ingresa S/. 100.00
- Sistema calcula automáticamente el vuelto

**Paso F: Registrar Venta**
- Verifica que botón "Registrar Venta" esté **habilitado** (no grisado)
- Clic en botón verde "Registrar Venta"

### 4. Verificar Resultado
**Esperado:**
- Popup SweetAlert con mensaje: "✓ Venta Registrada"
- Redirecciona a: `/Ventas/Details/{ventaId}`
- Página muestra: Detalle de la venta con número, fecha, cliente, vendedor, productos, total

---

## 🔍 QUÉ VERIFICAR

### En el Dropdown de Métodos de Pago:
- [ ] Dropdown NO está vacío
- [ ] Muestra opciones: Efectivo, Tarjeta de Crédito, Tarjeta de Débito, Transferencia Bancaria, Cheque
- [ ] Se puede seleccionar una opción

### En el Registro de Venta:
- [ ] Sistema acepta la venta sin errores
- [ ] No aparecen excepciones en consola (F12 → Console)
- [ ] Redirecciona correctamente a Details

### En la Base de Datos (SQL):
```sql
-- Verificar que la venta se registró
SELECT TOP 5 * FROM "Ventas" ORDER BY "Fecha" DESC;

-- Verificar detalles de la venta
SELECT * FROM "DetalleVentas" WHERE "VentaId" = <ventaId>;

-- Verificar métodos de pago disponibles
SELECT * FROM "MetodosPago" ORDER BY "Id";
```

---

## 🚨 POSIBLES PROBLEMAS Y SOLUCIONES

| Problema | Solución |
|----------|----------|
| Dropdown vacío | Reiniciar servidor (`dotnet run`). Verificar que MetodosPago tiene registros en BD |
| Error "Metodo de pago no encontrado" | Verificar que MetodoPagoId existe en BD |
| Botón "Registrar Venta" deshabilitado | Verificar: vendedor ≠ vacío, cliente ≠ vacío, método pago ≠ vacío, carrito ≠ vacío, monto recibido ≥ total |
| Error 404 en redirect | Verificar que VentaId devuelto es válido |
| Stock insuficiente | Seleccionar otra prenda con stock > cantidad solicitada |

---

## 📊 DATOS DE PRUEBA

### Métodos de Pago (ya en BD):
| ID | Nombre |
|----|--------|
| 1 | Efectivo |
| 2 | Tarjeta de Crédito |
| 3 | Tarjeta de Débito |
| 4 | Transferencia Bancaria |
| 5 | Cheque |

### Cuenta de Admin:
```
Email: Admin@gmail.com
Password: Admin123!
Rol: Administrador, Vendedor
```

---

## 🎯 CRITERIOS DE ACEPTACIÓN

✅ **ÉXITO si:**
1. Dropdown "Método de Pago" muestra 5 opciones (no vacío)
2. Se puede seleccionar una opción del dropdown
3. Se registra una venta sin errores
4. Sistema redirige a página de Detalle de Venta
5. Venta aparece en BD con método de pago correcto

❌ **FALLO si:**
- Dropdown está vacío
- Error 500 en endpoint `/api/registrar-venta`
- No redirige a Detalle
- Venta no aparece en BD

---

## 📝 NOTAS

- La prueba no requiere modificar código, solo usar la interfaz
- El seed de Métodos de Pago se ejecuta automáticamente al iniciar
- Si reinician el servidor, se ejecutará el seed nuevamente (pero solo inserta si no existen)
- El endpoint `/api/registrar-venta` requiere autenticación (session cookies del login)

