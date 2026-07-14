# Especificación Funcional: Cliente Nuevo en POS, Cliente Genérico y Descuentos Autorizados

## 1. Alcance
Este documento detalla los requerimientos para mejorar el flujo de ventas en el Punto de Venta (POS) de FashionStore. El alcance comprende:
- Permitir la creación rápida de clientes desde la interfaz de ventas.
- Permitir registrar ventas al paso (sin nombre o cliente genérico).
- Implementar un sistema de descuentos controlados y autorizados por el Administrador.

## 2. Actores
- **Vendedor:** Registra ventas, crea clientes nuevos, utiliza el cliente genérico y aplica descuentos autorizados activos. No puede crear ni editar descuentos.
- **Administrador / Dueño:** Tiene todas las facultades del vendedor, además de poder crear, editar, activar o desactivar descuentos. Puede aplicar descuentos.

## 3. Requisitos Funcionales

### RF-01: Cliente nuevo desde POS
- El POS debe contar con un botón "Nuevo Cliente" que abre un modal con un formulario moderno.
- Campos: Nombre completo, DNI, Teléfono y Dirección.
- Se debe validar para no duplicar clientes por DNI.
- El cliente creado debe quedar automáticamente seleccionado en el selector de clientes de la venta en curso.

### RF-02: Cliente genérico / sin nombre
- Se opta por la estrategia de **Cliente Genérico** (Opción A): Se creará y utilizará un registro en la tabla `Clientes` con nombre "Cliente Genérico" y DNI "00000000".
- En el POS, esta opción debe estar siempre disponible.
- Las ventas a este cliente deben aparecer en reportes indicando "Cliente Genérico".
- No debe existir más de un "Cliente Genérico" en el sistema.

### RF-03: Descuentos en Ventas
- Permitir aplicar descuento (Porcentaje o Monto Fijo) sobre el subtotal.
- El sistema debe validar en backend que el descuento no supere el subtotal de la venta.
- Se mostrará: Subtotal, Descuento y Total Final.

### RF-04: Descuentos autorizados por administrador
- Creación de entidad `DescuentoAutorizado` con: Id, Nombre, Tipo (Porcentaje/MontoFijo), Valor, Activo.
- En el POS, el vendedor sólo puede elegir de una lista desplegable de descuentos **activos**.
- Interfaz de administración exclusiva para el Administrador que permita gestionar este catálogo.

### RF-05: Venta con descuento (Transaccional)
- Al guardar la venta, el backend recalculará todo basado en los precios de la BD y validará el descuento enviado.
- Se registrará en la venta el ID del `DescuentoAutorizado` aplicado.
- Toda la operación de crear venta, detalles y descontar stock será manejada en una transacción de base de datos.

### RF-06: Informe de venta
- El informe PDF, Excel y vista en pantalla deben reflejar claramente:
  - Subtotal.
  - Descuento aplicado (Nombre del descuento y valor).
  - Total.

### RF-07: Seguridad por roles
- Control estricto a través de anotaciones de ASP.NET Core (`[Authorize(Roles="Administrador")]`) para la mantención de descuentos.

## 4. Requisitos No Funcionales
- Uso de AJAX (Fetch API) para aplicar descuentos sin recargar la página en el POS.
- Validaciones dobles (frontend y backend) de montos, stock y descuentos.

## 5. Reglas de Negocio
- Si un descuento inactivo se intenta aplicar (por manipulación de DOM), el backend debe rechazar la venta.
- Un descuento fijo no puede ser mayor al subtotal. Si es porcentaje, no puede ser > 100.

## 6. Criterios de Aceptación
1. Vendedor puede crear venta al cliente genérico sin errores de validación en BD.
2. Cliente nuevo creado desde el modal se selecciona automáticamente.
3. Solo el administrador puede crear nuevos descuentos autorizados.
4. Vendedor puede aplicar descuento autorizado y el total se recalcula.
5. Exportación PDF/Excel muestra correctamente los valores del descuento.
