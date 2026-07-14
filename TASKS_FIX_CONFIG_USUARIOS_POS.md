# LISTADO DE TAREAS (TASKS) - Correcciones de Configuración, Usuarios y POS

Este documento detalla el plan de actividades y el estado de avance para implementar y corregir las fallas del sistema.

---

## 📋 Lista de Tareas

### Fase 1: Rediseñar Configuración
- [x] Agregar la cabecera premium con título y subtítulo en [Index.cshtml](file:///c:/Users/CRISTIAN/source/repos/FashionStoreSolution/FashionStore.Web/Views/Configuracion/Index.cshtml).
- [x] Añadir el botón "Volver a Inicio" enlazado a `/Home/Index`.

### Fase 2: Botón volver a Home y navegación
- [x] Redireccionar enlaces de navegación de Configuración en el menú lateral y layouts.

### Fase 3: Crear usuarios/vendedores desde administrador
- [x] Diseñar el formulario modal para la creación de usuarios indicando rol y contraseña.
- [x] Proteger el controlador de configuración con autorización exclusiva a Administradores.

### Fase 4: Guardar usuario en Identity y base de datos
- [x] Agregar campo `Activo` en `ApplicationUser` y aplicar migración en base de datos.
- [x] Sincronizar la creación de usuarios con la tabla local `Vendedores` en caso de rol Vendedor.
- [x] Validar el estado activo en la pantalla de inicio de sesión.

### Fase 5: Restringir registro público
- [x] Modificar la página de registro de Identity para redirigir/forzar autorización de administrador.
- [x] Quitar enlaces de registro en la vista de login.

### Fase 6: Crear descuentos autorizados en Configuración
- [x] Añadir pestaña y tabla de descuentos autorizados (Porcentaje / Soles) en la vista de configuración.

### Fase 7: Rediseñar Buscador de Productos POS
- [x] Ampliar visualmente el buscador de productos del POS.
- [x] Implementar la lista en formato de tarjetas detalladas con miniaturas, stocks y botón "+ Agregar".
- [x] Configurar el buscador por código de barras escuchando la tecla Enter.

### Fase 8: Corregir Productos seleccionados por pagar
- [x] Diseñar la tabla amplia de ítems seleccionados para el pago en el POS.
- [x] Agregar botones de incremento y decremento con validación de stock.

### Fase 9: Corregir Resumen de Cobro con descuentos autorizados
- [x] Cargar descuentos activos autorizados en el POS.
- [x] Bloquear inputs manuales de descuento para vendedores.
- [x] Actualizar totales en tiempo real al seleccionar un descuento.

### Fase 10: Corregir descuento en soles
- [x] Validar que el descuento en soles reste correctamente y no sea mayor que el subtotal.
- [x] Integrar descuento en el backend recalculando precios y montos.

### Fase 11: Pago efectivo y vuelto
- [x] Agregar inputs para el cálculo y validación de dinero recibido y vuelto.

### Fase 12: Informe/PDF/Excel con descuentos
- [x] Mostrar descuentos desglosados en el ticket, PDF y Excel.
- [x] Restringir las consultas a vendedores para que solo vean sus propias ventas.

### Fase 13: Seguridad por roles
- [x] Añadir políticas de autorización por rol en controladores.

### Fase 14: Pruebas automáticas
- [x] Compilar el proyecto con `dotnet build` con 0 errores.
- [x] Correr pruebas unitarias con `dotnet test` asegurando 100% de éxito.

### Fase 15: Pruebas en navegador
- [x] Validar el funcionamiento completo de los flujos modificados en el navegador.

### Fase 16: Reporte final
- [x] Crear el archivo `REPORTE_FIX_CONFIG_USUARIOS_POS.md` con los resultados obtenidos.
