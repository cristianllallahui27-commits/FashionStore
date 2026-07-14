# DISEÑO DE SOLUCIÓN (DESIGN) - Fix Configuración, Clientes y POS

## 1. Diseño de UI y Navegación
- **Configuración (`/Configuracion/Index`)**: Se reemplazará el layout básico por un encabezado premium (Dashboard-like) que incluirá un botón grande "Volver a Inicio". Las secciones se agruparán en pestañas Bootstrap claras.
- **Nuevo Cliente (`/Clientes/Create`)**: Se añadirá un botón explícito de "Volver a Inicio" junto con tarjetas y alertas para las validaciones del correo electrónico.

## 2. Creación de Usuarios y Autenticación
- Se añadirá la propiedad `Activo` (booleana) a `ApplicationUser` para llevar el control de acceso en Identity.
- El controlador `ConfiguracionController` contendrá las acciones POST para `CrearUsuario` y `ToggleUsuario`, las cuales sincronizarán los estados en ASP.NET Identity y en la tabla local `Vendedores`.
- La vista de Login interceptará a usuarios inactivos o bloqueados y denegará el acceso antes de intentar el login. El registro público (`Register.cshtml`) quedará protegido por la política de roles de Administrador.

## 3. Buscador de POS Enriquecido
- El script de Javascript para el autocompletado reconstruirá los resultados en un div absoluto (dropdown) con un diseño de *cards* anchas que muestran miniaturas, texto descriptivo y badges de estado y categoría.
- Se capturará el evento `keyup` en la caja de búsqueda; si la tecla es `Enter` (escáner), se disparará un request para añadir directamente el producto.

## 4. Descuentos y Cobro
- El modelo `VentaCreateViewModel` incluirá un `string DescuentosJson` que contendrá la lista serializada de descuentos activos (Porcentaje y Soles) creados desde configuración.
- El formulario del POS cargará un `<select>` nativo con estas opciones, bloqueando campos de texto manual.
- El objeto de respuesta y petición de cobro validará los montos (que un descuento en soles no reduzca el total por debajo de 0).
- Se añadirán campos visuales dinámicos que muestran el monto exacto restado y el cálculo del vuelto en tiempo real para métodos en Efectivo.
