# TASKS_MEJORAS_POS_TIENDA.md
# FashionStoreSolution — Plan de Tareas de Implementación
**Fecha:** Julio 2026 | **Metodología:** SSD | **Versión:** 1.0

---

## FASE 1 — SEGURIDAD Y ROLES
- [ ] T-01: PrendasController: Create/Edit/Delete → `[Authorize(Roles="Administrador")]`
- [ ] T-02: PrendasController: Index/Details → `[Authorize]` (todos)
- [ ] T-03: ClientesController: Index/Edit/Delete → `[Authorize(Roles="Administrador")]`
- [ ] T-04: ClientesController: Create (solo desde módulo) → `[Authorize(Roles="Administrador")]`
- [ ] T-05: VentasController: toda la clase → `[Authorize(Roles="Administrador,Vendedor")]`
- [ ] T-06: ConfiguracionController (si existe) → `[Authorize(Roles="Administrador")]`
- [ ] T-07: Menú _Layout.cshtml: ocultar opciones admin a rol Vendedor

## FASE 2 — CÓDIGO DE BARRAS
- [ ] T-08: Agregar `CodigoBarra` a entidad `Prenda`
- [ ] T-09: Agregar `CodigoBarra` a `PrendaDTO`
- [ ] T-10: Agregar campo CodigoBarra en vista Create Prenda
- [ ] T-11: Agregar campo CodigoBarra en vista Edit Prenda
- [ ] T-12: Agregar campo CodigoBarra en vista Details Prenda
- [ ] T-13: Ejecutar migración SQL directa para agregar columna nullable

## FASE 3 — CARGA DE IMÁGENES
- [ ] T-14: PrendasController.Create POST: validar ext/tamaño, guardar en uploads/productos/
- [ ] T-15: PrendasController.Edit POST: validar imagen, mantener existente si no se sube nueva
- [ ] T-16: Rediseñar Create Prenda con AdminLTE (tarjetas, upload, preview)
- [ ] T-17: Rediseñar Edit Prenda con AdminLTE + preview imagen actual
- [ ] T-18: Rediseñar Details Prenda con imagen grande y todos los campos

## FASE 4 — REDISEÑO INTERFACES PRENDAS
- [ ] T-19: Views/Prendas/Create.cshtml → diseño moderno con cards AdminLTE
- [ ] T-20: Views/Prendas/Edit.cshtml → diseño moderno con preview imagen
- [ ] T-21: Views/Prendas/Details.cshtml → ficha completa de producto

## FASE 5 — ALTA RÁPIDA CLIENTE DESDE VENTA
- [ ] T-22: VentasController: agregar endpoint `POST /api/cliente-rapido`
- [ ] T-23: Validación anti-duplicados por DNI en endpoint
- [ ] T-24: Validación campos obligatorios (NombreCompleto, DNI, Telefono)
- [ ] T-25: Views/Ventas/Index.cshtml: agregar botón "Nuevo Cliente" en modal
- [ ] T-26: Modal `modalNuevoCliente` con formulario y validación JS
- [ ] T-27: JS: tras crear cliente → agregar al select + seleccionar automáticamente

## FASE 6 — TABLA REAL DE VENTAS EN INDEX
- [ ] T-28: VentasController: agregar `GET /api/ventas-recientes`
- [ ] T-29: Views/Ventas/Index.cshtml: tabla dinámica cargada desde API
- [ ] T-30: Cada fila tiene enlace "Ver detalle" → Ventas/Details/{id}

## FASE 7 — INFORME / DETALLE DE VENTA
- [ ] T-31: Crear `VentaDetalleViewModel.cs` con `DetalleVentaItem`
- [ ] T-32: VentasController: agregar acción `Details(int id)` con Include completo
- [ ] T-33: Crear Views/Ventas/Details.cshtml con informe profesional
- [ ] T-34: Botón Imprimir con `window.print()` y estilos de impresión

## FASE 8 — BRANDING EN LAYOUT
- [ ] T-35: _Layout.cshtml: inyectar `IConfiguracionSistemaService`
- [ ] T-36: Mostrar `NombreTienda` en navbar y sidebar
- [ ] T-37: Mostrar logo si existe (fallback a icono)
- [ ] T-38: Mostrar correo y teléfono en footer

## FASE 9 — PRUEBAS
- [ ] T-39: `dotnet build` → 0 errores
- [ ] T-40: `dotnet test` → 290+ pruebas pasando

## FASE 10 — VALIDACIÓN FINAL
- [ ] T-41: Verificar flujo completo de venta con cliente nuevo
- [ ] T-42: Verificar stock se descuenta
- [ ] T-43: Verificar informe de venta
- [ ] T-44: Verificar roles (vendedor no accede a admin)
- [ ] T-45: Crear REPORTE_MEJORAS_POS_TIENDA.md
