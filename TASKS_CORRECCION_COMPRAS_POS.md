# TASKS_CORRECCION_COMPRAS_POS.md
## Plan de Tareas - Correccion Modulo POS
**Version:** 1.0 | **Fecha:** 2026-07-08

---

## FASE 1 - Alta rapida de cliente desde compra

- [x] T1.1 Verificar endpoint POST /Ventas/api/cliente-rapido existe y valida DNI duplicado
- [ ] T1.2 Actualizar respuesta del endpoint para devolver { id, nombreCompleto }
- [ ] T1.3 Agregar modal #modalNuevoCliente en vista /Ventas/Create
- [ ] T1.4 JS: al guardar cliente, agregar option al select y seleccionarlo
- [ ] T1.5 Probar: cliente nuevo aparece en /Clientes

## FASE 2 - Rediseno de seleccion de productos tipo carrito

- [ ] T2.1 Crear vista /Ventas/Create.cshtml (pagina completa, layout 2 columnas)
- [ ] T2.2 Agregar accion GET Create en VentasController
- [ ] T2.3 Crear VentaCreateViewModel con listas de selectores
- [ ] T2.4 Seccion "Productos seleccionados por pagar": tabla visible, amplia
- [ ] T2.5 Buscador de productos con fetch a /api/buscar/{nombre}
- [ ] T2.6 Input cantidad con validacion de stock en tiempo real
- [ ] T2.7 Boton quitar producto de la lista
- [ ] T2.8 Cambiar boton "Nueva Venta" en Index para ir a /Ventas/Create
- [ ] T2.9 Probar: agregar, modificar y quitar productos correctamente

## FASE 3 - Pago en efectivo, monto recibido y vuelto

- [ ] T3.1 Agregar campos MontoRecibido y Vuelto a RegistrarVentaRequest
- [ ] T3.2 JS: detectar cuando metodo seleccionado es "Efectivo"
- [ ] T3.3 JS: mostrar/ocultar seccion de efectivo segun metodo
- [ ] T3.4 JS: calcular vuelto en tiempo real
- [ ] T3.5 JS: deshabilitar boton Confirmar si monto < total
- [ ] T3.6 Backend: validar montoRecibido >= total si metodo es efectivo
- [ ] T3.7 Probar: flujo completo de pago en efectivo con vuelto

## FASE 4 - Registro transaccional de venta

- [ ] T4.1 Agregar campos MontoRecibido, Vuelto, Descuento a entidad Venta.cs
- [ ] T4.2 Crear migracion EF Core: AgregarCamposEfectivoVenta
- [ ] T4.3 Ejecutar migracion en BD
- [ ] T4.4 Actualizar ServicioVentas.RegistrarVenta():
         - Obtener precio real de Prenda desde BD
         - Calcular total en backend
         - Guardar MontoRecibido y Vuelto
- [ ] T4.5 Actualizar endpoint POST /api/registrar-venta para usar nuevos campos
- [ ] T4.6 Verificar rollback en caso de fallo
- [ ] T4.7 Probar: registro completo, stock descontado, venta en BD

## FASE 5 - Informe/detalle de venta

- [ ] T5.1 Actualizar VentaDetalleViewModel: agregar MontoRecibido, Vuelto, CodigoBarra en detalles
- [ ] T5.2 Redisenar vista Views/Ventas/Details.cshtml
         - Header con datos de tienda (logo, nombre, contacto)
         - Tabla de productos con CodigoBarra
         - Panel totales grande y visible
         - Panel efectivo (condicional si MetodoPago = Efectivo)
         - Barra de acciones: Volver | Nueva Venta | Imprimir | PDF | Excel
- [ ] T5.3 Actualizar accion Details en VentasController para incluir nuevos campos
- [ ] T5.4 Probar: informe muestra todos los datos correctamente

## FASE 6 - Exportacion PDF

- [ ] T6.1 Crear vista /Ventas/Imprimir/{id} (HTML optimizada para print)
- [ ] T6.2 CSS @media print para ocultar navegacion y mostrar solo comprobante
- [ ] T6.3 Agregar accion GET DescargarPDF en VentasController
         - Opcion A: redirigir a vista imprimible
         - Opcion B (preferida): generar PDF con PuppeteerSharp o similar
- [ ] T6.4 Boton "Descargar PDF" en Details apunta a /Ventas/DescargarPDF/{id}
- [ ] T6.5 Probar: PDF descargado contiene datos completos

## FASE 7 - Exportacion Excel

- [ ] T7.1 Instalar paquete ClosedXML en FashionStore.Web.csproj
- [ ] T7.2 Agregar accion GET DescargarExcel en VentasController
         - Hoja "Venta": cabecera con datos generales, tabla de productos
         - Formateo: negrita en encabezados, alineacion numerica
         - Nombre archivo: Venta_{id:D4}.xlsx
- [ ] T7.3 Boton "Descargar Excel" en Details apunta a /Ventas/DescargarExcel/{id}
- [ ] T7.4 Probar: Excel descargado con datos correctos

## FASE 8 - Seguridad por roles

- [ ] T8.1 Verificar [Authorize(Roles = "Administrador,Vendedor")] en VentasController
- [ ] T8.2 Verificar que configuracion/prendas tiene [Authorize(Roles = "Administrador")]
- [ ] T8.3 Agregar filtro en Index: si rol = Vendedor, mostrar solo sus ventas
- [ ] T8.4 Agregar filtro en DescargarPDF/Excel: verificar acceso a esa venta
- [ ] T8.5 Probar: vendedor no puede acceder a configuracion

## FASE 9 - Pruebas

- [ ] T9.1 dotnet build (sin errores)
- [ ] T9.2 dotnet test (todos los tests pasan)
- [ ] T9.3 Prueba manual: crear cliente desde compra
- [ ] T9.4 Prueba manual: cliente nuevo en lista /Clientes
- [ ] T9.5 Prueba manual: agregar varios productos al carrito
- [ ] T9.6 Prueba manual: modificar cantidad, validar stock
- [ ] T9.7 Prueba manual: pago efectivo con vuelto
- [ ] T9.8 Prueba manual: pago no efectivo (sin campos efectivo)
- [ ] T9.9 Prueba manual: informe de venta despues de registrar
- [ ] T9.10 Prueba manual: descarga Excel
- [ ] T9.11 Prueba manual: descarga PDF / impresion
- [ ] T9.12 Prueba manual: vendedor no puede acceder a Configuracion

## FASE 10 - Validacion final

- [ ] T10.1 Ejecutar dotnet build final
- [ ] T10.2 Ejecutar dotnet test final
- [ ] T10.3 Verificar todos los criterios de aceptacion CA-01 a CA-12
- [ ] T10.4 Crear REPORTE_CORRECCION_COMPRAS_POS.md con resultados
