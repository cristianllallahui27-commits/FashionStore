# DESIGN_CORRECCION_COMPRAS_POS.md
## Diseno Tecnico - Modulo de Compras/Ventas POS
**Version:** 1.0 | **Fecha:** 2026-07-08

---

## 1. FLUJO POS GENERAL

```
[Pantalla /Ventas/Index]
  |-> [Boton "Nueva Venta"] -> [Vista /Ventas/Create (pagina completa, NO modal)]
        |-> [Selector Cliente] + [Boton "Nuevo Cliente" -> Modal RF-01]
        |-> [Buscador Producto + Boton Agregar]
        |-> [Seccion "Productos seleccionados por pagar"]
        |-> [Selector Metodo Pago]
        |   |-> Si Efectivo: [Campo Monto Recibido] [Calculo Vuelto]
        |-> [Panel Totales: Subtotal / Descuento / Total]
        |-> [Boton "Confirmar Venta"]
              |-> POST /Ventas/api/registrar-venta
              |-> Exito: window.location = /Ventas/Details/{id}
              |-> Error: mostrar mensaje
```

**Cambio clave:** Mover el formulario de venta de un modal a una PAGINA COMPLETA /Ventas/Create.
Esto resuelve los problemas de espacio, usabilidad y flujo de trabajo.

---

## 2. DISENO - ENTIDADES (Cambios requeridos)

### 2.1 Venta.cs - Agregar campos de efectivo

```csharp
// Nuevos campos en Venta
[Column(TypeName = "decimal(10,2)")]
public decimal? MontoRecibido { get; set; }  // Solo si Efectivo

[Column(TypeName = "decimal(10,2)")]
public decimal? Vuelto { get; set; }          // Solo si Efectivo

public decimal Descuento { get; set; } = 0;   // Preparado para futuro
```

Requiere migracion EF Core:
- Add-Migration AgregarCamposEfectivoVenta
- Update-Database

### 2.2 DetalleVenta.cs - Sin cambios requeridos

---

## 3. DISENO - VIEWMODELS

### 3.1 VentaCreateViewModel (NUEVO)
```csharp
public class VentaCreateViewModel
{
    public List<SelectListItem> Clientes { get; set; } = new();
    public List<SelectListItem> Vendedores { get; set; } = new();
    public List<SelectListItem> MetodosPago { get; set; } = new();
    public int? VendedorPreseleccionadoId { get; set; }
}
```

### 3.2 VentaDetalleViewModel - Agregar campos efectivo
```csharp
public decimal? MontoRecibido { get; set; }
public decimal? Vuelto { get; set; }
public decimal Descuento { get; set; }
public string? CodigoBarraPrenda { get; set; }  // en DetalleVentaItem
```

### 3.3 RegistrarVentaRequest - Agregar efectivo
```csharp
public class RegistrarVentaRequest
{
    public int ClienteId { get; set; }
    public int VendedorId { get; set; }
    public int MetodoPagoId { get; set; }
    public decimal? MontoRecibido { get; set; }
    public List<DetalleVentaRequest> Detalles { get; set; } = new();
}
```

---

## 4. DISENO - ENDPOINTS MVC

| Metodo | Ruta | Accion | Rol |
|---|---|---|---|
| GET | /Ventas | Index | Admin, Vendedor |
| GET | /Ventas/Create | Formulario de nueva venta | Admin, Vendedor |
| GET | /Ventas/Details/{id} | Informe comprobante | Admin, Vendedor |
| POST | /Ventas/api/registrar-venta | Registrar venta | Admin, Vendedor |
| POST | /Ventas/api/cliente-rapido | Crear cliente desde POS | Admin, Vendedor |
| GET | /Ventas/api/clientes-disponibles | Lista clientes para selector | Admin, Vendedor |
| GET | /Ventas/api/vendedores-disponibles | Lista vendedores | Admin, Vendedor |
| GET | /Ventas/api/metodos-pago | Lista metodos de pago | Admin, Vendedor |
| GET | /Ventas/api/buscar/{nombre} | Buscar productos por nombre | Admin, Vendedor |
| GET | /Ventas/DescargarPDF/{id} | Descarga PDF | Admin, Vendedor |
| GET | /Ventas/DescargarExcel/{id} | Descarga Excel | Admin, Vendedor |

---

## 5. DISENO - VISTA CREATE (Pagina POS)

Layout de dos columnas:
```
+------------------------------------------+------------------+
|  IZQUIERDA (col-8)                       | DERECHA (col-4)  |
|  [Cliente selector + Boton Nuevo]        | PANEL TOTALES    |
|  [Vendedor selector]                     | Subtotal: S/.    |
|  [Buscador producto + Boton Agregar]     | Descuento: S/.   |
|                                          | TOTAL: S/.       |
|  ## PRODUCTOS SELECCIONADOS POR PAGAR ## |                  |
|  [Tabla: Nombre|CB|PU|Cant|Sub|X]       | MetodoPago:      |
|                                          | [selector]       |
|                                          |                  |
|                                          | [Monto recibido] |
|                                          | [Vuelto]         |
|                                          |                  |
|                                          | [BTN CONFIRMAR]  |
+------------------------------------------+------------------+
```

---

## 6. DISENO - MODAL NUEVO CLIENTE

```html
Modal #modalNuevoCliente (dentro de /Ventas/Create)
- Input: NombreCompleto (required)
- Input: DNI (required, pattern 8 digitos)
- Input: Telefono (optional)
- Input: Direccion (optional)
- Boton: Guardar (POST /Ventas/api/cliente-rapido)
- Al exito: cerrar modal, agregar option al select, seleccionar
- Al error DNI duplicado: mostrar alerta dentro del modal
```

---

## 7. DISENO - CALCULO EFECTIVO (JS)

```javascript
metodoPagoSelect.addEventListener('change', function() {
    const esEfectivo = this.options[this.selectedIndex].text
                           .toLowerCase().includes('efectivo');
    document.getElementById('seccionEfectivo').style.display = 
        esEfectivo ? 'block' : 'none';
});

montoRecibidoInput.addEventListener('input', function() {
    const monto = parseFloat(this.value) || 0;
    const total = parseFloat(document.getElementById('totalFinal').dataset.total) || 0;
    const vuelto = monto - total;
    
    document.getElementById('vueltoDisplay').textContent = 
        'S/. ' + Math.max(0, vuelto).toFixed(2);
    
    const btnConfirmar = document.getElementById('btnConfirmarVenta');
    if (monto >= total && total > 0) {
        btnConfirmar.disabled = false;
        document.getElementById('alertaEfectivo').style.display = 'none';
    } else {
        btnConfirmar.disabled = true;
        document.getElementById('alertaEfectivo').style.display = 'block';
    }
});
```

---

## 8. DISENO - TRANSACCION DE VENTA (Backend)

El endpoint POST /Ventas/api/registrar-venta hace:
1. Recibe RegistrarVentaRequest (clienteId, vendedorId, metodoPagoId, montoRecibido, detalles[])
2. Validacion basica de campos.
3. Llama a ServicioVentas.RegistrarVenta() que:
   a. Abre transaccion SQL.
   b. Consulta precio real de cada PrendaId desde BD.
   c. Calcula total = SUM(precioReal * cantidad).
   d. Calcula vuelto si metodo es Efectivo.
   e. Valida montoRecibido >= total si es Efectivo.
   f. Inserta Venta.
   g. Inserta DetalleVenta por cada producto.
   h. Descuenta stock de cada Prenda.
   i. CommitAsync().
4. Retorna { exito: true, ventaId: X }
5. Frontend hace window.location = '/Ventas/Details/' + ventaId

---

## 9. DISENO - INFORME DE VENTA (Details.cshtml)

Vista /Ventas/Details.cshtml rediseniada:
- Cabecera: Logo tienda, Nombre tienda, Datos contacto.
- Seccion cliente/vendedor: tabla 2 columnas.
- Tabla de productos: Nombre | Cod.Barra | Talla/Color | Precio Unit. | Cant. | Subtotal.
- Panel totales: Subtotal, Descuento, TOTAL en grande.
- Panel efectivo (si aplica): Monto recibido, Vuelto.
- Barra de acciones: Volver | Nueva Venta | Imprimir | PDF | Excel.

---

## 10. DISENO - EXPORTACION PDF

Estrategia: Vista HTML imprimible (/Ventas/Imprimir/{id}) + window.print() con CSS @media print.
Si se instala DinkToPdf: generar PDF desde HTML en backend.

Alternativa principal (sin instalar nada nuevo):
- Endpoint GET /Ventas/DescargarPDF/{id}
- Genera contenido HTML de la vista Details.
- Usa libreria PuppeteerSharp o retorna vista imprimible.
- Por simplicidad inicial: redirigir a vista imprimible que usa window.print().

---

## 11. DISENO - EXPORTACION EXCEL (ClosedXML)

Instalar: dotnet add package ClosedXML

Endpoint GET /Ventas/DescargarExcel/{id}:
```csharp
using ClosedXML.Excel;

var workbook = new XLWorkbook();
var ws = workbook.Worksheets.Add("Venta");

// Fila 1-5: datos generales de venta
// Fila 7+: encabezados tabla + datos productos
// Al final: Total, MontoRecibido, Vuelto

var stream = new MemoryStream();
workbook.SaveAs(stream);
stream.Position = 0;
return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
    $"Venta_{id:D4}.xlsx");
```

---

## 12. RIESGOS Y MITIGACIONES

| Riesgo | Impacto | Mitigacion |
|---|---|---|
| Migracion Venta rompe datos existentes | Alto | Hacer campos nullable (decimal?) |
| Precio enviado desde JS en venta existente | Alto | Backend siempre recalcula desde BD |
| Modal pequeño para carrito | Medio | Mover a pagina completa /Ventas/Create |
| ClosedXML incompatible .NET 9 | Bajo | Usar version 0.100.x+ (compatible) |
| PDF sin libreria instalada | Bajo | Vista imprimible como fallback |
| Vendedor ve ventas de otros | Medio | Filtrar por VendedorId en queries si rol = Vendedor |

---

## 13. CAMBIOS POR ARCHIVO

### FashionStore.Domain
- Venta.cs: agregar MontoRecibido, Vuelto, Descuento

### FashionStore.Infrastructure1
- Migrations: nueva migracion para campos de Venta
- ServicioVentas.cs: actualizar RegistrarVenta para calcular desde BD

### FashionStore.Web
- Controllers/VentasController.cs: agregar Create GET, DescargarPDF, DescargarExcel
- ViewModels/VentaCreateViewModel.cs: NUEVO
- ViewModels/VentaDetalleViewModel.cs: agregar MontoRecibido, Vuelto
- Views/Ventas/Create.cshtml: NUEVA (pagina POS completa)
- Views/Ventas/Details.cshtml: NUEVA (informe rediseniado)
- FashionStore.Web.csproj: agregar ClosedXML
