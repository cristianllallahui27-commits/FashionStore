# ESPECIFICACIÓN DE REQUISITOS (SPEC) - Fix Configuración, Clientes y POS

## 1. Alcance
Esta especificación define los requisitos para rediseñar las interfaces de Configuración del Sistema y Nuevo Cliente, la creación de vendedores seguros, el rediseño del buscador del POS, y la corrección de descuentos autorizados (porcentaje y soles) en el resumen de cobro.

## 2. Actores
1. **Administrador**: Tiene acceso total. Puede modificar la configuración, crear usuarios (vendedores) y administrar descuentos autorizados.
2. **Vendedor**: Acceso restringido. Solo puede operar el Punto de Venta (POS) utilizando descuentos pre-aprobados, sin acceso a configuración ni administración.

## 3. Requisitos Funcionales

### RF-01: Interfaz moderna para Configuración del Sistema
- Rediseñar `/Configuracion/Index`.
- Organizar en pestañas claras: Datos de tienda, Logo, Contacto, Usuarios y Accesos, Descuentos autorizados.
- Botón visible "Volver a Home".
- Solo Administrador puede acceder.

### RF-02: Configuración funcional
- Edición de nombre de tienda, logo, correo, teléfono, dirección, RUC.
- Validación de logo (jpg, png, webp, max 5MB).
- Aplicación automática de los cambios en los layouts principales.

### RF-03: Interfaz moderna para Nuevo Cliente
- Rediseñar `/Clientes/Create` y el modal del POS.
- Formularios limpios y modernos (Bootstrap/AdminLTE).
- Botón visible "Volver a Home" o "Volver".
- Campos: Nombre, DNI, Correo (string validado), Teléfono, Dirección.

### RF-04: Administrador crea vendedores con clave y perfil
- Sección "Usuarios y accesos" en Configuración.
- Formulario de creación: Nombre, correo, contraseña, rol, estado.
- Guardado transaccional en ASP.NET Identity y base de datos local.
- Registro público bloqueado/oculto.

### RF-05: Perfil y restricciones del vendedor
- El vendedor ve su nombre y rol al iniciar sesión.
- Restricción total a configuración, productos y usuarios.

### RF-06: POS - Buscador de Productos amplio y visible
- Lista de resultados amplia debajo del buscador.
- Tarjetas con: imagen, nombre, código de barras, categoría, color, talla, precio, stock y botón "+ Agregar".
- Integración con lector de código de barras (escáner + Enter).

### RF-07: POS - Productos seleccionados por pagar
- Tabla amplia y estructurada (Producto, código, color, talla, precio, cantidad, subtotal).
- Botón de eliminar y control de cantidad con validación de stock.

### RF-08: Configuración de descuentos autorizados
- Administrador puede crear descuentos (Porcentaje o Soles) activos/inactivos.

### RF-09: POS - Resumen de Cobro con descuentos autorizados
- Vendedor solo elige descuentos autorizados ("Sin descuento", Porcentaje activo, Soles activo).
- No se permite descuento libre o manual para vendedores.
- Descuento en soles resta monto exacto sin perder formato.
- Backend recalcula el total, validando que el descuento no supere el subtotal.

### RF-10: Pago efectivo
- Input de monto recibido y cálculo de vuelto.
- Bloqueo si el monto recibido es menor al total.

### RF-11: Informe de venta
- Descuentos detallados (tipo y monto) en la vista web, PDF y Excel.

## 4. Criterios de Aceptación
- La configuración tiene un diseño a nivel del Dashboard.
- Administrador crea un vendedor, y este inicia sesión exitosamente pero sin permisos de admin.
- El POS busca productos visualmente y calcula totales correctos con descuentos autorizados.
- Vuelto se calcula exactamente en efectivo.
