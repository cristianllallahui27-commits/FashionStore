# ESPECIFICACIÓN DE REQUISITOS (SPEC) - Correcciones de Configuración, Usuarios y POS

Este documento especifica los requisitos funcionales y no funcionales, reglas de negocio y criterios de aceptación para las correcciones de la Configuración del Sistema, creación de vendedores, Buscador del POS y cobro.

---

## 1. Alcance
El alcance abarca:
- El rediseño y modernización de la interfaz de configuración del sistema (`/Configuracion/Index`).
- La creación y administración de usuarios (Administrador y Vendedor) por parte del administrador desde la configuración.
- El rediseño y optimización del buscador visual de productos en el Punto de Venta (POS) y su tabla de productos seleccionados.
- La aplicación correcta de descuentos autorizados (tanto porcentuales como en soles/monto fijo) y cálculos de pago en efectivo y vuelto en el POS.

---

## 2. Actores
1. **Administrador/Propietario**:
   - Administra la configuración del sistema (Branding, Datos de Tienda, Logotipos, etc.).
   - Administra los usuarios y accesos (Crear usuario, asignar rol, activar/desactivar).
   - Administra los descuentos autorizados.
   - Acceso total a reportes globales y POS.
2. **Vendedor**:
   - No tiene acceso a configuración ni administración de usuarios.
   - Solo puede registrar ventas desde el POS y ver e imprimir sus propias ventas.

---

## 3. Requisitos Funcionales

### RF-01: Configuración del Sistema Moderna
- **Interfaz**: Rediseñar la vista `/Configuracion/Index` con estilo idéntico al Home/Dashboard usando tarjetas de Bootstrap/AdminLTE.
- **Navegación**: Agregar un botón visible "Volver a Inicio" en la cabecera del panel.
- **Secciones claras**:
  - Datos de tienda (Nombre, Correo, Teléfono, Dirección, RUC).
  - Logo e imágenes (carga con validación, vista previa en tiempo real).
  - Usuarios y accesos (Listado, creación y cambio de estado).
  - Descuentos autorizados (Listado y creación).
- **Mensajes**: Mostrar notificaciones de éxito/error (Toastr/SweetAlert).

### RF-02: Datos Visibles de Tienda
- **Campos Editables**: Nombre, logo, correo, teléfono, dirección, RUC, y texto de pie de página.
- **Validación de Logotipo**: Solo formatos `.jpg`, `.jpeg`, `.png`, `.webp` y tamaño máximo de 5 MB.
- **LAYOUTS**: Al guardar, los cambios se verán reflejados inmediatamente en la barra de navegación, la cabecera y el pie de página de la aplicación.

### RF-03: Crear Vendedores / Usuarios desde Administrador
- **Formulario de Creación**: Campo de nombres, apellidos (si aplica), correo, nombre de usuario/correo de acceso, contraseña inicial y rol (*Administrador* / *Vendedor*).
- **Identity**: El usuario creado se guardará en ASP.NET Identity con su rol correspondiente.
- **Local DB**: En caso de rol Vendedor, se añadirá un registro a la tabla local `Vendedores` en sincronía con su correo.
- **Registro Público**: El registro público de cuentas externas queda bloqueado y se elimina cualquier enlace de registro en la pantalla de Login.

### RF-04: Perfil de Vendedor
- **Identificación**: Al iniciar sesión, se muestra el nombre de usuario y su rol en la parte superior derecha.
- **Bloqueos**: Si un usuario tiene la bandera `Activo == false`, no podrá iniciar sesión y el sistema mostrará un mensaje descriptivo en Login.

### RF-05: Descuentos Autorizados en Configuración
- **Administración**: El administrador puede crear descuentos indicando Nombre, Tipo (Porcentaje / Monto Fijo en Soles) y Valor.
- **Validaciones**:
  - Porcentaje entre 0% y 100%.
  - Soles mayor o igual a 0.
- **POS**: Solo los descuentos con estado `Activo == true` se listarán en el POS. El vendedor no puede ingresar descuentos manuales libres.

### RF-06: POS - Buscador de Productos Enriquecido
- **Buscador**: Caja de texto que permite buscar por Nombre, Código de Barras o Categoría.
- **Resultados**: Se despliegan debajo del buscador en una lista amplia de cards conteniendo miniatura del producto, nombre, código de barra, categoría, color, talla, precio, stock y botón "+ Agregar".
- **Lector**: Foco inicial por defecto, captura la tecla `Enter` tras un escaneo para añadir directamente al carrito e incrementa cantidad si ya existe.

### RF-07: POS - Productos Seleccionados
- **Tabla**: Muestra las columnas Producto (Nombre), Código de barras, Color, Talla, Precio, Cantidad editable y Subtotal.
- **Validación de Inventario**: Al cambiar la cantidad desde el carrito, se valida contra el stock actual disponible en base de datos.

### RF-08: POS - Cobro y Descuentos
- **Calculo**: Seleccionar descuento autorizado de tipo Porcentaje o Soles. Resta la reducción del subtotal.
- **Seguridad**: El backend recalcula el descuento y total sobre precios de base de datos antes de guardar la transacción, garantizando que el descuento en soles no supere el subtotal.

### RF-09: Pago en Efectivo
- **Vuelto**: Permite ingresar monto recibido y calcular vuelto. Si el monto recibido es menor al total, se bloquea la confirmación.

### RF-10: Informes de Ventas
- **Detalle**: Muestra el tipo y valor del descuento aplicado. Excel y PDF reflejan los datos descontados.
- **Acceso**: Un vendedor solo puede ver e imprimir sus propias ventas. El administrador accede a todo el historial.

---

## 4. Requisitos No Funcionales
- **Seguridad**: Todas las rutas administrativas quedan protegidas con `[Authorize(Roles = "Administrador")]`.
- **Integridad**: Validaciones transaccionales del lado del servidor para compras.
- **Usabilidad**: Diseño responsivo y estético.

---

## 5. Reglas de Negocio
- Los vendedores no pueden aplicar descuentos manuales libres.
- Las cuentas de usuario desactivadas no pueden iniciar sesión.
- Ninguna transacción de venta puede procesarse si solicita más unidades de las disponibles en stock.

---

## 6. Criterios de Aceptación
1. El Administrador puede ver, añadir y activar/desactivar usuarios y descuentos desde el panel de Configuración.
2. Un usuario Vendedor inactivo es rechazado inmediatamente al intentar loguearse.
3. El Buscador del POS muestra tarjetas con imágenes y botones interactivos.
4. El vuelto se calcula automáticamente al actualizar el efectivo ingresado, y bloquea el botón si es menor al total.
