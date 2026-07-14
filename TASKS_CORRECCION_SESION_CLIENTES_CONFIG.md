# Plan de Tareas: Corrección Sesión, Clientes y Configuración

## Fase 1: Corregir cierre de sesión (P-01)
- [ ] Modificar `Logout.cshtml.cs` (`OnPost` y añadir `OnGet`) para redirigir explícitamente a `/Identity/Account/Login` si no existe ReturnUrl.

## Fase 2: Revisar y unificar layouts necesarios (P-01)
- [ ] Modificar `Views/Shared/_Layout.cshtml` para reemplazar enlaces de Logout `<a>` por un `<form method="post">` apuntando a `/Account/Logout`.
- [ ] Revisar y replicar el cambio en `Pages/Shared/_Layout.cshtml` si existe un navbar con botón de sesión allí.

## Fase 3: Rediseñar Clientes/Create (P-02)
- [ ] Modificar `Views/Clientes/Create.cshtml`.
- [ ] Aplicar contenedor, estructura de tarjeta (`card`), colores y tipografía usando Bootstrap 5.
- [ ] Incluir íconos y distribución en grid para los inputs.

## Fase 4: Rediseñar modal Nuevo Cliente en Ventas (P-02)
- [ ] Revisar `Views/Ventas/Create.cshtml`.
- [ ] Modificar el HTML del modal para que coincida con el diseño de `Clientes/Create.cshtml`.
- [ ] Asegurar que el modal se cierre correctamente después de un éxito mediante Javascript, que se inyecte en el select y quede seleccionado automáticamente.

## Fase 5: Corregir Configuración del Sistema (P-03)
- [ ] Verificar / Crear `ConfiguracionViewModel` con `IFormFile LogoUpload`.
- [ ] Actualizar el controlador `ConfiguracionController` con `[Authorize(Roles = "Administrador")]`.
- [ ] Modificar `Views/Configuracion/Index.cshtml` para tener un formulario moderno `multipart/form-data`.
- [ ] Implementar la lógica para guardar el logo (validar formato/tamaño) en `wwwroot` y persistir la ruta.
- [ ] Actualizar `IConfiguracionSistemaService` para limpiar cache si existe al guardar.

## Fase 6: Mostrar Configuración en Layout global (P-03)
- [ ] Inyectar `IConfiguracionSistemaService` en `Views/Shared/_Layout.cshtml`.
- [ ] Inyectar el nombre, email, teléfono y logo en el header, sidenav y/o footer, reemplazando estáticos.

## Fase 7: Seguridad por roles (P-03)
- [ ] Revisar controladores clave (`ConfiguracionController`) y asegurar `[Authorize(Roles="Administrador")]`.
- [ ] Ocultar menú de "Configuración" en el sidebar (`_Layout.cshtml`) para el rol Vendedor.

## Fase 8: Pruebas y Validación final
- [ ] Ejecutar `dotnet build` y `dotnet test`.
- [ ] Realizar pruebas manuales de creación de cliente, modal de ventas, subida de imagen, cierre de sesión y redirecciones de seguridad.
- [ ] Crear el archivo final de reporte `REPORTE_CORRECCION_SESION_CLIENTES_CONFIG.md`.
