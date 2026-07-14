# Especificación Funcional: Corrección Sesión, Clientes y Configuración

## 1. Alcance
Este documento detalla la corrección de tres problemas críticos en la plataforma FashionStore:
1. Fallos en el cierre de sesión y redirecciones correctas de usuarios.
2. Interfaz visual deficiente al crear nuevos clientes, especialmente desde el modal POS.
3. El módulo de Configuración del Sistema no cumple con los requisitos del administrador para personalizar la tienda (nombre, logo, datos de contacto).

## 2. Actores
- **Administrador/Dueño:** Puede cambiar configuración general (logo, teléfono, correo) y realizar todas las funciones del sistema.
- **Vendedor:** Puede registrar clientes y cerrar sesión, pero NO tiene acceso al módulo de configuración.
- **Usuario No Autenticado:** Debe ser redirigido a la página de login si intenta acceder a páginas protegidas.

## 3. Requisitos Funcionales

### RF-01: Cerrar Sesión
- El botón "Cerrar sesión" debe usar un método `POST` seguro (con token antiforgery si es posible) o configurarse de modo que elimine exitosamente la cookie de autenticación de Identity.
- El usuario debe ser redirigido al `/Identity/Account/Login` inmediatamente después de desloguearse.
- Un usuario deslogueado que use el botón "atrás" del navegador no debe poder ver contenido protegido sin reautenticarse.

### RF-02: Consistencia de Layouts
- Asegurar que la barra de navegación que muestra el usuario, su rol y el botón de "Cerrar Sesión" sea la misma y funcione tanto en `Views/Shared/_Layout.cshtml` como en `Pages/Shared/_Layout.cshtml`.

### RF-03: Nuevo Cliente (Visual Moderno)
- La vista de `Clientes/Create.cshtml` debe ser rediseñada usando clases de Bootstrap (o AdminLTE) consistentes con el diseño de las otras vistas (tarjetas, sombras, botones estilizados).
- Debe incluir los campos: Nombre completo, DNI, Teléfono y Dirección.
- Debe incluir validaciones visuales claras.

### RF-04: Alta Rápida de Cliente (Modal POS)
- Al estar en la creación de una venta, el botón "Nuevo Cliente" debe abrir un modal moderno (consistente con RF-03).
- Debe validar duplicados por DNI.
- Al guardarse correctamente (vía AJAX), el cliente debe seleccionarse automáticamente en la lista desplegable de clientes del POS y mostrar un mensaje de éxito sin recargar la página completa.

### RF-05: Configuración del Sistema (Admin)
- La vista `Configuracion/Index.cshtml` permitirá modificar: NombreTienda, Correo, Telefono, Direccion, RUC, TextoPiePagina y RutaLogo.
- Permitirá la subida de un archivo de imagen (jpg, png, webp) hasta 5MB para el Logo, y mostrará una vista previa del logo actual.
- Al guardar, la imagen debe subirse al servidor (`wwwroot/uploads` o similar) y los datos actualizarse en la tabla `Configuraciones`.
- Restringido a usuarios con rol `Administrador`.
- Se debe refrescar o invalidar cualquier caché que afecte la configuración global.

### RF-06: Mostrar Configuración Global
- `_Layout.cshtml` (el principal) debe leer la configuración activa (o usar los valores por defecto) para inyectar dinámicamente el Logo, Nombre de la Tienda, Correo y Teléfono en el encabezado o pie de página.
- Eliminar datos quemados en las plantillas.

### RF-07: Seguridad
- Validar mediante el atributo `[Authorize(Roles = "Administrador")]` el acceso al Controlador de Configuración.
- Los Vendedores sólo tienen acceso a Ventas, Clientes y reportes básicos de su rol.

## 4. Requisitos No Funcionales
- **Compatibilidad:** Seguir el estándar de ASP.NET Core Identity.
- **Rendimiento:** Evitar múltiples consultas a base de datos por petición leyendo la configuración; usar caching u obtenerlo eficientemente.
- **Diseño Responsivo:** Usar clases de Bootstrap 5 o el framework existente para asegurar visualización móvil.

## 5. Casos de Uso
1. **Cerrar Sesión:** Usuario hace clic en Logout -> El sistema borra cookies y redirige a Login.
2. **Entrar con otro usuario:** Tras el caso de uso 1, la página muestra formulario de login donde otro usuario ingresa credenciales.
3. **Crear cliente (Módulo Clientes):** Vendedor va a Clientes -> Nuevo, completa el formulario con nuevo diseño y guarda.
4. **Crear cliente (Ventas/POS):** Vendedor hace clic en "Nuevo Cliente" en el POS -> Aparece modal moderno -> Ingresa DNI y nombre -> Guarda -> Modal se cierra y el cliente está seleccionado.
5. **Editar Configuración:** Admin ingresa a Ajustes -> Sube nuevo logo y cambia teléfono -> Guarda -> El navbar se actualiza instantáneamente.
6. **Mostrar ConfigGlobal:** Cualquier usuario navega por la app -> El menú principal lee el logo subido en Configuración en lugar de un texto quemado.
