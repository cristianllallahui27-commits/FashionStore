# Tareas de Implementación: POS, Cliente Genérico y Descuentos

## Fase 1: Cliente nuevo desde POS (RF-01)
- [x] Verificar el endpoint `/api/cliente-rapido` en `VentasController` para asegurar que procesa correctamente la validación por DNI y retorna el id del cliente.
- [x] Asegurarse de que el JS del modal en `Views/Ventas/Create.cshtml` inyecte la opción de forma predeterminada al select tras el alta.

## Fase 2: Cliente genérico o venta sin nombre (RF-02)
- [x] Modificar `VentasController.Create` (GET) para inyectar a la base de datos el "Cliente Genérico" con DNI "00000000" si no existe.
- [x] Añadir este cliente por defecto como una de las opciones principales del selector de clientes en el POS.

## Fase 3: Modelo de descuentos autorizados (RF-03, RF-04)
- [x] Crear Entidad `DescuentoAutorizado` (`Id`, `Nombre`, `Tipo`, `Valor`, `Activo`).
- [x] Actualizar entidad `Venta` añadiendo `DescuentoAutorizadoId` nullable y la propiedad de navegación.
- [x] Crear la configuración en el `DbContext` (FashionStoreDbContext).
- [x] Crear y aplicar migración en EF Core para la BD SQL Server.

## Fase 4: Administración de descuentos por administrador (RF-04, RF-07)
- [x] Crear `DescuentosController` (`[Authorize(Roles = "Administrador")]`) con vistas Index, Create, Edit.
- [x] Añadir entrada en el menú `_Layout.cshtml` bajo la sección Administración.

## Fase 5: Aplicación de descuentos en POS (RF-05)
- [x] Pasar lista de `DescuentosActivos` en `VentaCreateViewModel`.
- [x] Actualizar UI del POS (`Create.cshtml`) con el selector de descuentos.
- [x] Actualizar lógica en Javascript para recalcular `Total` descontando el porcentaje o monto fijo.

## Fase 6: Registro transaccional de venta con descuento (RF-05)
- [x] Modificar Endpoint `/api/registrar-venta` para aceptar `DescuentoAutorizadoId`.
- [x] Implementar transacción EF Core.
- [x] Recalcular y validar subtotal vs descuento en backend, evitando descuentos que superen el subtotal o envíos maliciosos.

## Fase 7: Informe/PDF/Excel con descuento (RF-06)
- [x] Actualizar vista `Ventas/Details.cshtml` para mostrar Descuento y Total.
- [x] Actualizar `ExportarExcel` en `VentasController` para incluir la columna del nombre y monto del descuento.
- [x] Actualizar visor o generación PDF para que refleje el descuento si está en uso.

## Fase 8: Seguridad por roles (RF-07)
- [x] Asegurarse que ningún otro usuario tenga acceso a las tablas de configuración.
- [x] Confirmar que el vendedor interactúe sin privilegios en los descuentos.

## Fase 9: Pruebas
- [x] Compilar el proyecto `dotnet build`.
- [x] Pasar todas las pruebas automatizadas `dotnet test`.

## Fase 10: Validación final
- [x] Validar manualmente los requisitos y flujos detallados en SPEC.
- [x] Escribir y exportar REPORTE final.
