# TAREAS (TASKS) - Fix Configuración, Clientes y POS

### Fase 1: Rediseñar Configuración
- [ ] Aplicar diseño moderno en `/Configuracion/Index`.

### Fase 2: Agregar botón Volver a Home
- [ ] Insertar enlace/botón visual "Volver a Inicio" en la vista de Configuración.

### Fase 3: Rediseñar Nuevo Cliente
- [ ] Modernizar `/Clientes/Create` y añadir "Volver a Inicio".

### Fase 4: Crear usuarios/vendedores desde administrador
- [ ] Construir la tabla de usuarios y el modal de creación en la pestaña Usuarios de Configuración.

### Fase 5: Guardar usuarios en Identity y perfil
- [ ] Agregar propiedad `Activo` en el ApplicationUser y actualizar base de datos local de Vendedores.

### Fase 6: Bloquear registro público externo
- [ ] Restringir `/Identity/Account/Register` y limpiar enlaces públicos de Login.

### Fase 7: Restringir vendedor por rol
- [ ] Implementar la validación en Login para rechazar a los usuarios con estado inactivo.

### Fase 8: Crear descuentos autorizados en Configuración
- [ ] Habilitar pestaña de creación y listado de descuentos porcentuales y en soles.

### Fase 9: Rediseñar Buscador de Productos POS
- [ ] Mejorar la vista visual del menú desplegable del autocompletado en el POS.

### Fase 10: Rediseñar Productos seleccionados por pagar
- [ ] Transformar la lista del carrito en una tabla amplia que resalte color, stock y subtotales.

### Fase 11: Corregir Resumen de Cobro
- [ ] Sustituir input libre de descuentos por un select de Descuentos Autorizados activos.

### Fase 12: Corregir descuento en soles
- [ ] Validar Javascript y transacciones del backend para la aplicación exacta de descuentos fijos sin alterar montos.

### Fase 13: Corregir pago efectivo y vuelto
- [ ] Incorporar input de Efectivo Recibido y validación de pago exacto o superior en el POS.

### Fase 14: Informe/PDF/Excel con descuentos
- [ ] Mostrar el descuento desglosado en las exportaciones de la venta final.

### Fase 15: Pruebas automáticas
- [ ] Compilar solución con 0 errores y pasar todos los Test Unitarios.

### Fase 16: Pruebas en navegador
- [ ] Comprobar funcionamiento visual integral.

### Fase 17: Reporte final
- [ ] Redactar `REPORTE_FIX_CONFIG_CLIENTE_POS.md`.
