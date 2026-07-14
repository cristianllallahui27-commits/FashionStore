# Diseño Técnico: POS, Cliente Genérico y Descuentos Autorizados

## 1. Decisión Estratégica: Cliente Genérico vs Nullable ClienteId
- **Decisión:** Opción A (Registro de Cliente Genérico).
- **Justificación:** La entidad `Venta` actual tiene `public int ClienteId { get; set; }` no anulable. Modificar esto a nullable requeriría migraciones, comprobaciones nulas extensas a través de los informes, y rompería asunciones previas del modelo. Al crear un "Cliente Genérico" con DNI "00000000", aseguramos la compatibilidad y facilitamos los filtros en los reportes (simplemente filtrando por el ID de ese cliente o su DNI).
- **Implementación:** Durante la carga del `VentasController` o al iniciar el sistema, se validará si existe el cliente con DNI "00000000". Si no existe, se creará.

## 2. Entidad `DescuentoAutorizado`
```csharp
public class DescuentoAutorizado
{
    public int Id { get; set; }
    public string Nombre { get; set; } // Ej: "Descuento Empleado 10%"
    public TipoDescuento Tipo { get; set; } // Enum: Porcentaje, MontoFijo
    public decimal Valor { get; set; } // 10.00 para 10% o 50.00 para 50 Soles
    public bool Activo { get; set; } = true;
}
public enum TipoDescuento { Porcentaje, MontoFijo }
```
- Se modificará `Venta.cs` agregando `public int? DescuentoAutorizadoId { get; set; }` y su relación de navegación `public DescuentoAutorizado? DescuentoAutorizado { get; set; }`.

## 3. Flujo POS (Nuevo Cliente)
- La vista modal en `Ventas/Create.cshtml` ya fue adaptada visualmente. El código AJAX que hace POST a `/api/cliente-rapido` debe garantizar que retorna un objeto con `id, nombreCompleto y dni`.
- Al recibir `exito: true`, JavaScript agrega una etiqueta `<option>` al select `cliente`, establece su `value` al nuevo `id` y ejecuta `.change()` para actualizar variables dependientes.

## 4. Flujo de Descuentos en POS
- El ViewModel `VentaCreateViewModel` debe incluir `List<SelectListItem> DescuentosActivos`.
- En UI (`Views/Ventas/Create.cshtml`), se añade un select de "Aplicar Descuento" que el vendedor puede usar.
- Cada opción en el select tendrá data-attributes: `data-tipo` y `data-valor`.
- Al seleccionar, JS recalculará el subtotal, le restará el descuento seleccionado, y mostrará el nuevo Total.

## 5. Modificación en `VentasController`
- `RegistrarVenta` API debe recibir el `descuentoAutorizadoId`.
- El backend buscará el descuento en BD. Verificará que esté activo.
- Calculará el total: `Subtotal = sum(detalles * precio_real)`. Si es descuento por porcentaje: `Subtotal * Valor / 100`. Si es fijo: `Valor`.
- Evaluará que no sobrepase el Subtotal.
- Establecerá `venta.Descuento = descuentoCalculado;` y `venta.DescuentoAutorizadoId = id`.
- Operación manejada mediante un bloque de transacción de EF Core: `using var transaction = await _context.Database.BeginTransactionAsync()`.

## 6. Seguridad
- Nuevo controlador `DescuentosController` para ABM (Alta, Baja, Modificación) exclusivo para rol `Administrador`.
- Las acciones del vendedor sólo leen los descuentos `Activo == true`.

## 7. Reportes (PDF y Excel)
- En `VentasController.Details`, la vista mostrará el descuento nominal y su tipo (si existe `DescuentoAutorizado`).
- En `ExportarExcel`, se añadirá una columna "Descuento" y "Total" corregido.
- La funcionalidad usará `ClosedXML` pre-existente.

## 8. Riesgos y Mitigaciones
- **Riesgo:** El cliente manipula JS para aplicar un ID de descuento inactivo.
- **Mitigación:** Validación obligatoria en `VentasController.RegistrarVenta` chequeando que `descuento.Activo == true`.
- **Riesgo:** Concurrencia al crear cliente genérico.
- **Mitigación:** Asegurar su creación con `.Any(c => c.DNI == "00000000")` antes de insertar.
