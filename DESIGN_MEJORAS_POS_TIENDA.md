# DESIGN_MEJORAS_POS_TIENDA.md
# FashionStoreSolution — Diseño Técnico de Mejoras POS
**Fecha:** Julio 2026 | **Metodología:** SSD | **Versión:** 1.0

---

## DISEÑO DE ROLES

### Cambios en controladores
```
[Authorize(Roles = "Administrador")]       → Configuracion, Prendas/Create, Edit, Delete
[Authorize(Roles = "Administrador,Vendedor")] → Ventas/Index, Clientes/*
[Authorize]                                → Home, Prendas/Index, Details
```

---

## DISEÑO: CÓDIGO DE BARRAS

### Entidad Prenda (campo nuevo)
```csharp
[StringLength(50)]
public string? CodigoBarra { get; set; }
```

### PrendaDTO (campo nuevo)
```csharp
[StringLength(50)]
public string? CodigoBarra { get; set; }
```

Sin migración EF nueva — se agrega como campo [NotMapped] si no hay migración lista, o se crea migración simple con `ALTER TABLE Prendas ADD CodigoBarra nvarchar(50) NULL`.

---

## DISEÑO: CARGA DE IMAGEN

### Flujo
```
Browser → form enctype="multipart/form-data" → PrendasController.Create/Edit(IFormFile)
→ Validar extensión y tamaño → Guid.NewGuid() + extension → wwwroot/uploads/productos/
→ prenda.ImagenUrl = nombreArchivo → _unitOfWork.CommitAsync()
```

### Validaciones backend
```csharp
string[] extensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };
long maxSize = 5 * 1024 * 1024; // 5 MB
```

### Carpeta destino
```
wwwroot/uploads/productos/{guid}.{ext}
```

---

## DISEÑO: ALTA RÁPIDA DE CLIENTE DESDE VENTA

### Endpoint
```
POST /api/cliente-rapido
Authorization: Authorize (Administrador o Vendedor)
Content-Type: application/json

Request:
{
  "nombreCompleto": "string (required)",
  "dni": "string (required, 8 chars)",
  "telefono": "string (required)",
  "direccion": "string (optional)"
}

Response éxito:
{ "exito": true, "id": 5, "nombreCompleto": "Juan Pérez" }

Response duplicado:
{ "exito": false, "mensaje": "Ya existe un cliente con ese DNI." }

Response validación:
{ "exito": false, "mensaje": "El nombre completo es obligatorio." }
```

### Validación anti-duplicados
```csharp
var existente = await _unitOfWork.Clientes.FindAsync(c => c.DNI == request.Dni);
if (existente.Any())
    return Json(new { exito = false, mensaje = "Ya existe un cliente con ese DNI." });
```

### Modal UI
```
Modal "modalNuevoCliente" lanzado desde botón "Nuevo Cliente" dentro de modalNuevaVenta.
Campos: NombreCompleto*, DNI*, Teléfono*, Dirección.
Al guardar → fetch POST /api/cliente-rapido → si exito:
  1. Agrega <option> al select #cliente
  2. Selecciona el nuevo cliente automáticamente
  3. Cierra modal secundario
  4. Muestra toast de éxito
```

---

## DISEÑO: INFORME DE VENTA

### Endpoint
```
GET /Ventas/Details/{id}
Carga Venta + Cliente + Vendedor + MetodoPago + DetalleVenta + Prenda (Include)
Retorna vista con ViewModel VentaDetalleViewModel
```

### ViewModel VentaDetalleViewModel
```csharp
public class VentaDetalleViewModel
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; }
    public string ClienteNombre { get; set; }
    public string ClienteDNI { get; set; }
    public string VendedorNombre { get; set; }
    public string MetodoPagoNombre { get; set; }
    public List<DetalleVentaItem> Detalles { get; set; }
    public decimal Subtotal { get; set; }
    public decimal IGV { get; set; }
    public decimal Total { get; set; }
    public string NombreTienda { get; set; }  // de ConfiguracionSistema
}

public class DetalleVentaItem
{
    public string NombrePrenda { get; set; }
    public string Talla { get; set; }
    public string Color { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
}
```

### Index: tabla dinámica de ventas
```javascript
// Cargar ventas desde GET /api/ventas-recientes
async function cargarVentas() { ... }
// Cada fila tiene link a /Ventas/Details/{id}
```

---

## DISEÑO: BRANDING EN LAYOUT

### ViewComponent TiendaInfo (o via ViewBag en _Layout)
Opción elegida: **filtro de acción global** inyectado en `_Layout.cshtml` via `@inject IConfiguracionSistemaService ConfigService`

```cshtml
@inject FashionStore.Infrastructure.Services.IConfiguracionSistemaService ConfigService
@{
    var config = await ConfigService.ObtenerConfiguracionAsync();
}
<!-- Navbar: config.NombreTienda -->
<!-- Footer: config.Correo | config.Telefono -->
```

---

## ARQUITECTURA GENERAL DE CAMBIOS

```
Domain/
  Entities/Prenda.cs          + CodigoBarra
  DTOs/PrendaDTO.cs            + CodigoBarra
  ViewModels/
    VentaDetalleViewModel.cs   NEW

Infrastructure/
  (sin cambios de repositorios)

Web/
  Controllers/
    PrendasController.cs       + roles + imagen validada + CodigoBarra
    VentasController.cs        + ApiClienteRapido + ApiVentasRecientes + Details
    ClientesController.cs      + roles (Index/Edit/Delete solo Admin)
  Views/
    Prendas/Create.cshtml      REDISEÑADO
    Prendas/Edit.cshtml        REDISEÑADO
    Prendas/Details.cshtml     REDISEÑADO
    Ventas/Index.cshtml        + tabla real + modal cliente rápido
    Ventas/Details.cshtml      NEW (informe)
    Shared/_Layout.cshtml      + branding config
```
