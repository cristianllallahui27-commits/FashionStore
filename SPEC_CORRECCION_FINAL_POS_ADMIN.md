# ESPECIFICACIÓN DE REQUISITOS (SPEC) - Correcciones y Mejoras Finales POS, Ventas y Seguridad

Este documento describe detalladamente el alcance, actores, requisitos funcionales y no funcionales, reglas de negocio y criterios de aceptación para las correcciones del Punto de Venta (POS), descuentos, configuración, gestión de prendas por color, lectura de código de barras, perfiles por rol e informes de venta.

---

## 1. Alcance del Proyecto
El proyecto abarca la optimización final, seguridad y adecuación funcional del sistema FashionStore. Se centra en proveer una experiencia de tienda real para el Punto de Venta (POS), incluyendo la aplicación de descuentos autorizados, el soporte directo para lectores de códigos de barra, la personalización visual de prendas con contraste óptimo, la administración centralizada de la información de branding mediante una interfaz moderna, y la segregación de perfiles (Administrador vs. Vendedor).

---

## 2. Actores del Sistema
1. **Administrador/Dueño**:
   - Acceso total a todas las vistas y funciones.
   - Puede crear, modificar y eliminar prendas, categorías y clientes.
   - Puede definir, autorizar y dar de baja descuentos.
   - Administra los usuarios vendedores (creación y asignación de rol).
   - Accede a la configuración general del sistema (nombre, logo, colores, datos fiscales).
   - Descarga informes de ventas generales de toda la tienda.
2. **Vendedor**:
   - Acceso restringido.
   - Solo puede vender a través del POS.
   - Puede consultar únicamente sus propias ventas e imprimir sus comprobantes correspondientes.
   - No tiene acceso a configuración, gestión de prendas, administración de usuarios ni creación de descuentos.

---

## 3. Requisitos Funcionales por Módulo

### RF-01: Descuentos en POS
- **Selección de Tipo de Descuento**: El POS permitirá elegir entre:
  - *Sin descuento*: Valor por defecto.
  - *Porcentaje (%)*: Aplica una reducción porcentual.
  - *Monto Fijo (S/.)*: Aplica una reducción en soles.
- **Entrada de Descuento**:
  - Para vendedores: Solo pueden seleccionar de una lista de descuentos autorizados por el Administrador. No pueden escribir montos libres.
  - Para administradores: Pueden seleccionar descuentos autorizados o escribir montos/porcentajes de descuento libres en un campo de texto de control.
- **Validaciones en Backend**:
  - Validar que el valor de porcentaje esté entre 0% y 100%.
  - Validar que el descuento en soles no supere el subtotal de la compra.
  - Recalcular en el servidor todos los montos (Subtotal, Descuento y Total).

### RF-02: Configuración del Sistema
- **Interfaz Moderna**: El módulo de configuración debe rediseñarse con estilo consistente al Dashboard.
- **Campos Editables**: Nombre de tienda, logotipo de tienda, correo electrónico, teléfono, dirección, RUC y texto del pie de página.
- **Carga y Vista Previa del Logo**:
  - Cargar imagen con validación (.jpg, .jpeg, .png, .webp; máx. 5 MB).
  - Mostrar vista previa interactiva antes de guardar.
- **Branding Global**: El logotipo, teléfono, correo y nombre de la tienda deben cargarse dinámicamente y mostrarse en el pie de página de los layouts principales de la aplicación.

### RF-03: Gestión de Colores en Prendas
- **Legibilidad de Color**: La visualización de colores en el listado de prendas y detalles debe utilizar un diseño que asegure el contraste.
- **Estructura del Campo**: Debe permitir nombres comerciales (ej. "Estampado militar", "Verde oliva", "Beige").
- **Muestra Visual**: Se representará el color comercial con un círculo coloreado (si se reconoce el color en un diccionario CSS estándar) junto al texto legible en un badge con fondo claro y letra oscura (`bg-light border text-dark`).

### RF-04: Código de Barras y Lector
- **Atributo en Prendas**: Campo `CodigoBarra` opcional y único.
- **Integración con Lector**:
  - Un campo en la cabecera del POS "Escanear o ingresar código de barras" con foco por defecto.
  - Procesar la entrada de teclado enviada por un lector de código de barras físico al presionar la tecla `Enter`.
  - Buscar la prenda por coincidencia exacta. Si se encuentra y hay stock, añadir automáticamente al carrito de ventas e incrementar la cantidad.
  - Si no existe o no tiene stock, alertar con mensajes descriptivos.

### RF-05: Buscador Visual de Productos
- **Resultados de Búsqueda Mejorados**: El autocompletado y búsqueda del POS debe mostrar detalles enriquecidos: imagen miniatura, nombre, código de barras, categoría, talla, color, precio y stock.
- **Botón Agregar**: Cada fila de búsqueda debe contener un botón "+ Agregar" para pasar el artículo al carrito.

### RF-06: Carrito del POS (Productos por Pagar)
- **Grid de Productos**: Tabla amplia con columnas independientes de: imagen, prenda, código de barras, color/talla, precio, cantidad editable y subtotal por línea.
- **Totales y Pago**: Mostrar el desglose claro de Subtotal, Descuento y Total.
- **Vuelto**: Si el método de pago es Efectivo, pedir el monto recibido y validar que cubra el Total, calculando y mostrando el vuelto exacto.

### RF-07: Perfil de Usuario
- **Menú y Rol**: Los layouts de cabecera y barra lateral deben mostrar el nombre del usuario y una etiqueta de su rol actual.
- **Vista de Perfil**: Redireccionar `/Identity/Account/Manage` a la vista personalizada `/Perfil` que permite actualizar datos básicos y cambiar la contraseña de acceso.

### RF-08: Gestión de Vendedores
- **Creación por Admin**: El Administrador puede crear nuevos vendedores asignando nombre, DNI, correo, teléfono y contraseña. El sistema los da de alta en ASP.NET Identity con el rol `Vendedor`.
- **Restricción de Rutas**: Los controladores de configuración, prendas, categorías y descuentos deben bloquearse con `[Authorize(Roles = "Administrador")]` y retornar 403/Forbidden o redirigir a páginas autorizadas al vendedor.

### RF-09: Informes de Ventas
- **Detalle de Venta**: Mostrar toda la información de la tienda, cliente, vendedor, método de pago, desglose de prendas, descuentos aplicados y total general.
- **Descargas**: Habilitar botones de impresión física, exportación a Excel y descarga en PDF.
- **Control de Permisos**:
  - El Vendedor solo puede exportar/descargar los reportes de sus propias ventas.
  - El Administrador puede exportar y ver todos los reportes de la tienda.

### RF-10: Dashboard - Estado de Stock
- **Stock en Tiempo Real**: El KPI de Estado de Stock en el Home debe recuperar datos reales de la base de datos (Agotados = 0, Bajo Stock <= 5, Normal > 5).
- **Lista de Alertas**: Se listarán los nombres de prendas con stock bajo o agotado mostrando su categoría y un botón de edición directa (solo visible para Administradores).

---

## 4. Requisitos No Funcionales
- **Seguridad**: Prevención de manipulación de precios o montos del carrito en JS mediante recalculación y validación transaccional del lado del servidor.
- **Usabilidad**: Diseño responsivo y contrastes de color óptimos (W3C WCAG) para el módulo de color de productos.
- **Rendimiento**: Búsquedas eficientes mediante autocompletado optimizado.

---

## 5. Reglas de Negocio
- Un vendedor no puede aplicar descuentos libres creados al momento en el POS; solo puede seleccionar descuentos de la lista activa autorizada.
- Ninguna venta puede registrarse si el stock solicitado es mayor que el stock actual en el inventario.
- El valor del vuelto en efectivo no puede ser negativo.
- Un usuario con rol `Vendedor` tiene prohibido acceder a las páginas de configuración, administración de prendas y gestión de usuarios.

---

## 6. Criterios de Aceptación
1. Al escanear un código de barras en el POS y presionar `Enter`, el producto se carga al carrito directamente sin perder el foco del input para permitir un escaneo continuo.
2. Al intentar descargar el Excel general de ventas desde un usuario Vendedor, el archivo generado solo contiene sus transacciones propias.
3. El logotipo cargado en la configuración de la tienda se actualiza en tiempo real en la barra lateral del sistema y en el pie de página de la web principal.
4. El stock de la prenda disminuye en la base de datos de manera atómica después de una compra y se ve reflejado en el gráfico de stock del Dashboard al volver a Inicio.
