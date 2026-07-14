# PLAN DE TRABAJO (TASKS) - Correcciones y Mejoras Finales POS, Ventas y Seguridad

Este plan de trabajo detalla el estado y pasos de implementación de las correcciones del sistema de acuerdo con la metodología SSD.

---

## 📅 Estado de las Fases

### Fase 1: Cierre de sesión y layouts
- [x] Corregir la acción del formulario de logout en `Views/Shared/_Layout.cshtml` y `Pages/Shared/_Layout.cshtml` usando POST directo.
- [x] Sincronizar enlaces y corregir redirección post-logout al formulario de login de Identity.

### Fase 2: Configuración moderna y funcional
- [x] Mapear las propiedades `RutaLogo` y `RutaFavicon` a las columnas `LogoUrl` y `FaviconUrl` en la base de datos mediante Fluent API.
- [x] Mejorar visualmente la pantalla de configuración en `/Configuracion` aplicando el tema de la tienda.
- [x] Habilitar la subida y visualización en tiempo real del logotipo (máximo 5MB, JPG/PNG/WEBP).
- [x] Inyectar y mostrar teléfono, correo y logotipo de la tienda en el pie de página de los layouts principales de la web.

### Fase 3: Gestión de prendas, color legible y código de barras
- [x] Implementar representación de colores de prendas mediante un dot circular de color + texto oscuro sobre badge claro.
- [x] Añadir validación de código de barras único (`CodigoBarra`) en los POST de creación y edición en `PrendasController`.
- [x] Configurar e indexar el campo `CodigoBarra` con restricción filtered unique en el DbContext.

### Fase 4: Buscador visual de productos
- [x] Rediseñar los resultados de autocompletado y búsqueda del POS para mostrar imagen, nombre, talla, color, categoría, precio y stock.
- [x] Incluir el botón "+ Agregar" interactivo en cada resultado de la búsqueda del POS.

### Fase 5: POS con productos seleccionados por pagar
- [x] Diseñar y ampliar la tabla de productos seleccionados por pagar en el POS, separando columnas de imagen, nombre, código de barra, color, talla, cantidad y subtotal.
- [x] Implementar controles de cantidad e incremento/decremento dinámicos en JavaScript.

### Fase 6: Descuentos sin descuento / porcentaje / soles autorizados
- [x] Incorporar el selector de descuento estructurado en el POS (Sin descuento, Porcentaje, Soles).
- [x] Restringir a los vendedores para que solo puedan seleccionar descuentos autorizados y pre-cargados por el administrador.
- [x] Permitir a los administradores ingresar valores personalizados para porcentaje o soles en campos dedicados.
- [x] Ampliar el objeto de petición en backend `RegistrarVentaRequest` y validar transaccionalmente el tipo y monto de descuento aplicado.

### Fase 7: Pago efectivo con vuelto
- [x] Mantener e integrar el cálculo dinámico de vuelto en el POS al pagar en efectivo.
- [x] Garantizar validación de que el monto recibido en efectivo sea mayor o igual al total a pagar antes de procesar el registro de venta.

### Fase 8: Informe de venta, PDF and Excel
- [x] Actualizar la vista de detalles y ticket impreso (`Details.cshtml` e `Imprimir.cshtml`) para mostrar el desglose de subtotal, descuento por nombre, tipo de descuento y total general.
- [x] Incluir la información de subtotal y descuento detallado en la exportación Excel de una venta individual.

### Fase 9: Perfil de usuario por rol
- [x] Reemplazar los accesos del menú perfil para redirigir directamente al controlador personalizado `/Perfil`.
- [x] Asegurar que el layout principal muestre dinámicamente el nombre de usuario y su rol.

### Fase 10: Creación/autorización de vendedor
- [x] Mantener el flujo en `VendedoresController` para dar de alta cuentas de vendedor con roles de Identity.

### Fase 11: Seguridad y restricciones
- [x] Aplicar restricciones en backend para que los vendedores solo puedan visualizar y descargar Excel de sus propias ventas en `Details`, `Imprimir`, `DescargarExcel` y `ExportarExcel`.
- [x] Mantener la seguridad de roles `[Authorize(Roles = "Administrador")]` en las secciones de inventario, usuarios, configuración y descuentos.

### Fase 12: Home Estado de Stock
- [x] Actualizar la consulta del Dashboard en `HomeController` con includes EF Core para cargar correctamente los nombres de categorías de los productos en alerta.
- [x] Asegurar la correcta actualización de los stocks tras cada venta realizada.

### Fase 13: Pruebas automáticas
- [x] Ejecutar compilación de solución (`dotnet build`).
- [x] Correr pruebas unitarias (`dotnet test`) y asegurar que todas pasen.

### Fase 14: Pruebas en navegador
- [x] Probar en vivo las restricciones de vendedor, escaneo de barras, aplicación de descuentos de porcentaje/soles y descarga de archivos.

### Fase 15: Reporte final
- [x] Crear el archivo `REPORTE_CORRECCION_FINAL_POS_ADMIN.md` resumiendo las correcciones implementadas.
