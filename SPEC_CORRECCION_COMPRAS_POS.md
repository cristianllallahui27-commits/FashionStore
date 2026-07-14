# SPEC_CORRECCION_COMPRAS_POS.md
## Especificacion de Correccion - Modulo de Compras/Ventas POS
**Version:** 1.0 | **Fecha:** 2026-07-08 | **Estado:** APROBADO PARA IMPLEMENTACION

---

## 1. ALCANCE

Corregir y completar el modulo de ventas (POS) de FashionStoreSolution para funcionar como una tienda real de ropa. Abarca desde la creacion rapida del cliente hasta la exportacion del comprobante en PDF y Excel.

**Fuera de alcance:** cambio de BD/ORM, devoluciones, facturacion electronica, integracion lector fisico de codigos de barra.

---

## 2. ACTORES

| Actor | Rol | Permisos |
|---|---|---|
| Vendedor | Operador de caja | Registrar ventas, crear cliente rapido, ver sus informes |
| Administrador | Dueno / gerente | Todos los permisos + configuracion + todos los informes |
| Sistema | ASP.NET Core MVC | Valida stock, calcula total, genera transaccion |

---

## 3. REQUISITOS FUNCIONALES

### RF-01 - Alta rapida de cliente desde compra
- Boton "Nuevo Cliente" en pantalla de compra.
- Modal con: Nombre completo, DNI, Telefono, Direccion.
- Validar campos obligatorios: NombreCompleto, DNI.
- Validar DNI no duplicado (endpoint POST /Ventas/api/cliente-rapido).
- Tras guardar: cliente seleccionado automaticamente en selector de venta.
- Cliente queda en modulo /Clientes.

### RF-02 - Seleccion de productos tipo lista/carrito
- Buscador de productos por nombre.
- Al agregar aparece en seccion "Productos seleccionados por pagar".
- Cada fila: Nombre, Codigo barra, Precio unitario, Cantidad, Subtotal, Boton quitar.
- Validar cantidad <= stock disponible.
- Sin productos: no se puede confirmar venta.

### RF-03 - Total de compra
- Mostrar Subtotal, Descuento (0 por ahora), Total final.
- Total calculado en BACKEND desde BD.

### RF-04 - Pago en efectivo
- Si metodoPago = Efectivo: mostrar "Monto recibido".
- Calcular Vuelto = MontoRecibido - Total (JS + verificacion backend).
- Si MontoRecibido < Total: bloquear confirmacion.
- Guardar MontoRecibido y Vuelto en entidad Venta (requiere migracion).

### RF-05 - Otros metodos de pago
- Ocultar campos de efectivo si no es efectivo.
- Registrar venta normalmente.

### RF-06 - Registro transaccional de venta
- Validar: cliente, vendedor, metodo pago, productos, cantidades > 0, stock suficiente.
- Total calculado en BD (precio real de cada Prenda).
- Unica transaccion SQL: Venta + DetalleVentas + descuento stock.
- Si falla: rollback completo.

### RF-07 - Informe/detalle de venta
- Redirigir a GET /Ventas/Details/{id} tras registro exitoso.
- Mostrar: ID, Fecha/hora, Cliente, Vendedor, MetodoPago, productos, Total, MontoRecibido, Vuelto, Datos tienda.
- Botones: Volver, Nueva Venta, Imprimir, PDF, Excel.

### RF-08 - Descarga PDF
- Endpoint GET /Ventas/DescargarPDF/{id}
- Usar vista HTML imprimible o libreria PDF.
- Nombre: Venta_0001.pdf

### RF-09 - Descarga Excel
- Endpoint GET /Ventas/DescargarExcel/{id}
- Usar ClosedXML.
- Nombre: Venta_0001.xlsx

### RF-10 - Seguridad por roles
- Vendedor: crear cliente rapido, registrar ventas, ver/descargar sus informes.
- Administrador: todo lo anterior + ver todos los informes + configuracion.
- Vendedor NO puede acceder a configuracion ni editar productos.

---

## 4. REQUISITOS NO FUNCIONALES

| ID | Requisito | Detalle |
|---|---|---|
| RNF-01 | Consistencia | Transaccion SQL para registro venta |
| RNF-02 | Seguridad | Precios siempre desde BD, nunca desde JS |
| RNF-03 | Usabilidad | UI tipo POS real: carrito visible, totales prominentes |
| RNF-04 | Compatibilidad | .NET 9, EF Core, SQL Server, ASP.NET Identity |
| RNF-05 | Rendimiento | Buscador de productos, no carga masiva |
| RNF-06 | Mantenibilidad | No romper modulos existentes |

---

## 5. REGLAS DE NEGOCIO

- RN-01: Precio desde BD al momento del registro.
- RN-02: No vender mas del stock disponible.
- RN-03: DNI de cliente es unico.
- RN-04: Pago efectivo requiere monto recibido >= total.
- RN-05: Vuelto = MontoRecibido - Total (solo efectivo).
- RN-06: Venta sin productos no se registra.
- RN-07: Vendedor asociado a la venta.
- RN-08: Descuento = 0 por defecto.

---

## 6. CRITERIOS DE ACEPTACION

| ID | Criterio | Resultado esperado |
|---|---|---|
| CA-01 | Crear cliente desde modal | Cliente guardado, en selector y en /Clientes |
| CA-02 | DNI duplicado | Error claro, no se guarda |
| CA-03 | Agregar producto | Aparece en "Productos seleccionados por pagar" |
| CA-04 | Cambiar cantidad | Subtotal y total actualizados |
| CA-05 | Cantidad > stock | Error, no se agrega |
| CA-06 | Seleccionar Efectivo | Campo monto recibido aparece |
| CA-07 | Monto < total | Boton deshabilitado, alerta |
| CA-08 | Monto >= total | Vuelto calculado y visible |
| CA-09 | Confirmar venta | Redirige a informe completo |
| CA-10 | Descargar Excel | Archivo Venta_XXXX.xlsx |
| CA-11 | Descargar PDF | Archivo Venta_XXXX.pdf |
| CA-12 | Vendedor accede a config | Acceso denegado |

---

## 7. CASOS DE USO

### CU-01: Crear cliente desde compra
Actor: Vendedor
1. Clic en "Nuevo Cliente" en pantalla de venta.
2. Modal muestra formulario.
3. Vendedor llena datos y guarda.
4. Sistema valida y verifica DNI no duplicado.
5. Guarda via POST /Ventas/api/cliente-rapido.
6. Modal cierra. Cliente seleccionado automaticamente.

Alt: DNI duplicado -> mensaje de error, modal no cierra.

### CU-02: Agregar productos al carrito
Actor: Vendedor
1. Escribe nombre en buscador.
2. Selecciona producto.
3. Clic en "Agregar".
4. Producto aparece en lista con cantidad 1.
5. Vendedor ajusta cantidad o quita producto.
6. Total se actualiza.

### CU-03: Pagar con efectivo y calcular vuelto
Actor: Vendedor
1. Selecciona "Efectivo".
2. Aparece campo "Monto recibido".
3. JS calcula vuelto en tiempo real.
4. Monto >= total: vuelto verde, confirmar habilitado.
5. Monto < total: alerta roja, confirmar deshabilitado.

### CU-04: Confirmar venta
Actor: Vendedor
1. Pulsa "Confirmar venta".
2. Backend valida todo.
3. Recalcula total desde BD.
4. Transaccion: Venta + DetalleVentas + stock.
5. Commit -> ventaId.
6. Redirige a /Ventas/Details/{ventaId}.

### CU-05: Ver informe de venta
Actor: Vendedor / Administrador
1. Acceso a /Ventas/Details/{id}.
2. Vista muestra comprobante completo.
3. Botones: Volver, Nueva Venta, Imprimir, PDF, Excel.

### CU-06: Descargar PDF
Actor: Vendedor / Administrador
Clic en "PDF" -> GET /Ventas/DescargarPDF/{id} -> descarga Venta_XXXX.pdf

### CU-07: Descargar Excel
Actor: Vendedor / Administrador
Clic en "Excel" -> GET /Ventas/DescargarExcel/{id} -> descarga Venta_XXXX.xlsx
