# Diseño Técnico: Corrección Sesión, Clientes y Configuración

## 1. Diagnóstico de Logout
- Actualmente, `Logout.cshtml.cs` redirige a la misma página cuando `returnUrl` es nulo, lo que no actualiza correctamente el estado en el cliente si no hay recarga total, o puede causar confusión. 
- En el Layout principal (`Views/Shared/_Layout.cshtml` y posiblemente en `Pages/Shared/_Layout.cshtml`), el enlace o botón de "Cerrar sesión" podría estar configurado como un `<a>` (método `GET`) cuando Identity Core espera un `POST` con un token anti-falsificación para evitar vulnerabilidades de CSRF.

### Solución propuesta para Logout:
- Modificar ambos `_Layout.cshtml` para garantizar que el "Logout" use una etiqueta `<form method="post" asp-area="Identity" asp-page="/Account/Logout">` en lugar de un simple hipervínculo `<a href="...">`.
- Modificar `Logout.cshtml.cs` en sus métodos `OnPost` y (añadiendo) `OnGet` para forzar explícitamente `return RedirectToPage("/Account/Login", new { area = "Identity" });` si no existe `returnUrl`.

## 2. Rediseño Visual de Clientes/Create
- Se actualizará el archivo `Views/Clientes/Create.cshtml`.
- Se reemplazará el layout básico por un contenedor estructurado (`container-fluid`).
- Se añadirán tarjetas con `card shadow-sm border-0 rounded-4`.
- Botones `btn btn-primary` (Guardar) y `btn btn-secondary` (Volver).
- Uso de íconos de FontAwesome (`fas fa-user-plus`).
- Validaciones en rojo con clases predeterminadas de `asp-validation-for`.

## 3. Alta Rápida en Ventas/POS
- Modificar `Views/Ventas/Create.cshtml` en la sección del selector de clientes.
- Reemplazar el viejo modal (o añadirlo si no existe) con una estructura de Bootstrap: `<div class="modal fade">` y diseño acorde a la aplicación principal.
- Modificar o revisar la lógica de Javascript (`fetch` a `/api/cliente-rapido`) para mostrar un SweetAlert / Toast de éxito tras la creación.
- Automáticamente inyectar el nuevo cliente en el select de Select2/HTML y seleccionarlo.

## 4. Configuración del Sistema
- Archivos a intervenir: `ConfiguracionController.cs`, `ConfiguracionSistemaService.cs` y las vistas `Views/Configuracion/Index.cshtml`.
- **Vista de Edición:** Un formulario estructurado con `enctype="multipart/form-data"`.
- Campos: Nombre de Tienda, Teléfono, Correo, Dirección, RUC, Footer.
- Campo tipo `<input type="file" name="logoFile" accept="image/jpeg, image/png, image/webp" />`.
- **Lógica Controller:**
  - `[Authorize(Roles = "Administrador")]` aplicado a `ConfiguracionController`.
  - Acción `Update` que recibe el ViewModel y una imagen `IFormFile logoFile`.
  - Guardado del archivo en `wwwroot/img/uploads/` usando `IWebHostEnvironment`.
  - Persistencia usando `_context.Configuraciones`.
  - Creación de un registro en `ConfiguracionesAuditoria` si corresponde.
  - Refresco de la memoria caché llamando a un método para invalidarla o recargarla en `IConfiguracionSistemaService`.

## 5. Inyección Global de Configuración (Layout)
- Modificar `Views/Shared/_Layout.cshtml` y `Pages/Shared/_Layout.cshtml`.
- Utilizar la inyección en vistas: `@inject FashionStore.Web.Services.IConfiguracionSistemaService ConfigService`.
- Llamar a `var config = await ConfigService.ObtenerConfiguracionAsync();`.
- Reemplazar textos quemados (e.g. `<span class="brand-text">FashionStore</span>`) por `config.NombreTienda`.
- Reemplazar el `src` del Logo quemado por `@(!string.IsNullOrEmpty(config.RutaLogo) ? config.RutaLogo : "/img/default-logo.png")`.
- Reemplazar teléfonos e emails en la barra superior si la plantilla lo tiene.

## 6. Riesgos y Mitigaciones
- **Fallo al subir imagen:** Se debe validar en el backend tamaño máximo (5MB) y extensión permitida. Si falla, devolver error visible en la UI.
- **Acceso concurrente:** Si dos admins editan al mismo tiempo, el último gana. Aceptable dado que es raro.
- **Cacheo de logo en navegadores:** Añadir query string versionada para el logo: `src=".../logo.png?v=@DateTime.Now.Ticks"` para evitar que la UI quede cacheada si suben un logo con el mismo nombre.
