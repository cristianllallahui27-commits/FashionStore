// Archivo de prueba E2E - Registrar una venta completa
// Este archivo NO se incluirá en el build - solo para demostrar

/*

PRUEBA MANUAL E2E - CREAR Y REGISTRAR VENTA

1. DATOS NECESARIOS:
   - ClienteId: 1 (or Cliente Genérico)
   - VendedorId: 1
   - MetodoPagoId: 1 (Efectivo)
   - PrendaId: 1, cantidad: 2

2. PAYLOAD JSON para POST /api/registrar-venta:

{
  "clienteId": 1,
  "vendedorId": 1,
  "metodoPagoId": 1,
  "montoRecibido": 100.00,
  "descuentoAutorizadoId": null,
  "detalles": [
    {
      "prendaId": 1,
      "cantidad": 2,
      "precio": 49.99
    }
  ]
}

3. RESPUESTA ESPERADA:
{
  "exito": true,
  "mensaje": "Venta registrada exitosamente",
  "datos": {
    "ventaId": 1
  }
}

4. PASOS MANUALES EN LA UI:
   a) Ir a: Admin → Ventas → Nueva Venta (POS)
   b) Seleccionar Vendedor
   c) Seleccionar Cliente
   d) Buscar producto y agregar al carrito (con cantidad > 0)
   e) Seleccionar Método de Pago (ahora visible con 5 opciones)
      - Efectivo
      - Tarjeta de Crédito
      - Tarjeta de Débito
      - Transferencia Bancaria
      - Cheque
   f) Si es Efectivo: ingresar Monto Recibido
   g) Clic en "Registrar Venta"
   h) Esperado: Redirecciona a page Detalle de Venta con número ventaId

*/
