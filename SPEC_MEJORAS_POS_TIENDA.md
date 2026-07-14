# SPEC_MEJORAS_POS_TIENDA.md
# FashionStoreSolution — Especificación de Mejoras POS / Tienda Real
**Fecha:** Julio 2026 | **Metodología:** SSD | **Versión:** 1.0

---

## RESUMEN EJECUTIVO

El sistema FashionStore requiere evolucionar desde un CRUD básico a un POS (Point of Sale) funcional que permita operar como una tienda real de ropa. Las mejoras cubren flujo de venta completo, alta rápida de cliente, informe de comprobante, control de imágenes, código de barras futuro, roles diferenciados y branding visible.

---

## REQUISITOS FUNCIONALES

### RF-01 — ROLES Y PERMISOS
| ID | Requisito | Prioridad |
|----|-----------|-----------|
| RF-01.1 | Rol `Administrador`: acceso completo a todos los módulos | Alta |
| RF-01.2 | Rol `Vendedor`: acceso solo a Ventas y consulta de Prendas | Alta |
| RF-01.3 | Vendedor NO puede crear, editar ni eliminar Prendas | Alta |
| RF-01.4 | Vendedor NO puede acceder a Configuración del sistema | Alta |
| RF-01.5 | Vendedor SÍ puede crear cliente rápido desde flujo de venta | Alta |
| RF-01.6 | Vendedor NO tiene acceso al módulo administrativo completo de Clientes | Media |

### RF-02 — CÓDIGO DE BARRAS (preparación futura)
| ID | Requisito | Prioridad |
|----|-----------|-----------|
| RF-02.1 | Campo `CodigoBarra` opcional (string nullable) en entidad `Prenda` | Media |
| RF-02.2 | Campo `CodigoBarra` en `PrendaDTO` | Media |
| RF-02.3 | Visible en formularios Crear, Editar y Detalle de Prenda | Media |
| RF-02.4 | Campo no requerido — queda listo para integración futura con lector | Media |

### RF-03 — CARGA DE IMAGEN DE PRODUCTO
| ID | Requisito | Prioridad |
|----|-----------|-----------|
| RF-03.1 | Campo de upload en Crear Prenda y Editar Prenda | Alta |
| RF-03.2 | Extensiones permitidas: `.jpg`, `.jpeg`, `.png`, `.webp` | Alta |
| RF-03.3 | Tamaño máximo: 5 MB | Alta |
| RF-03.4 | Guardar en `wwwroot/uploads/productos/` con nombre GUID | Alta |
| RF-03.5 | Guardar solo el nombre del archivo en BD (columna `ImagenUrl`) | Alta |
| RF-03.6 | Vista previa de imagen actual en Editar y Detalle | Alta |
| RF-03.7 | Al editar sin subir nueva imagen, conservar imagen existente | Alta |
| RF-03.8 | Crear carpeta automáticamente si no existe | Alta |

### RF-04 — REDISEÑO DE VISTAS DE PRENDAS
| ID | Requisito | Prioridad |
|----|-----------|-----------|
| RF-04.1 | Crear Prenda: tarjetas AdminLTE, iconos Font Awesome, validación visible | Alta |
| RF-04.2 | Editar Prenda: mismo diseño que Crear + vista previa imagen + CodigoBarra | Alta |
| RF-04.3 | Detalle Prenda: diseño moderno con imagen, todos los campos, historial | Alta |
| RF-04.4 | Lista Prendas: tabla con imagen miniatura, badge de stock, acciones | Media |

### RF-05 — ALTA RÁPIDA DE CLIENTE DESDE VENTA
| ID | Requisito | Prioridad |
|----|-----------|-----------|
| RF-05.1 | Botón "Nuevo Cliente" en modal de venta POS | Alta |
| RF-05.2 | Abre modal secundario con formulario rápido | Alta |
| RF-05.3 | Campos: NombreCompleto (req), DNI (req), Teléfono (req), Dirección (opc) | Alta |
| RF-05.4 | Validación de campos obligatorios en frontend y backend | Alta |
| RF-05.5 | Validación anti-duplicados por DNI | Alta |
| RF-05.6 | Endpoint `POST /api/cliente-rapido` (accesible para rol Vendedor) | Alta |
| RF-05.7 | Tras guardar, el nuevo cliente se selecciona automáticamente en la venta | Alta |
| RF-05.8 | El select de clientes se actualiza sin recargar la página | Alta |

### RF-06 — INFORME / DETALLE DE VENTA
| ID | Requisito | Prioridad |
|----|-----------|-----------|
| RF-06.1 | Vista `Ventas/Details/{id}` con informe completo | Alta |
| RF-06.2 | Muestra: cliente, vendedor, fecha, método de pago | Alta |
| RF-06.3 | Tabla de productos: nombre, cantidad, precio unitario, subtotal | Alta |
| RF-06.4 | Totales: subtotal, IGV (18%), total | Alta |
| RF-06.5 | Botón "Volver" y botón "Imprimir" (window.print) | Alta |
| RF-06.6 | Enlace desde tabla de ventas en Index | Alta |
| RF-06.7 | La tabla de ventas en Index debe mostrar ventas reales de BD | Alta |

### RF-07 — BRANDING EN LAYOUT GLOBAL
| ID | Requisito | Prioridad |
|----|-----------|-----------|
| RF-07.1 | Nombre de tienda visible en navbar y sidebar | Alta |
| RF-07.2 | Logo si está configurado (con fallback al icono por defecto) | Media |
| RF-07.3 | Correo y teléfono en footer del layout | Media |
| RF-07.4 | Datos obtenidos de `ConfiguracionSistema` (Id=1) | Alta |
| RF-07.5 | Solo Administrador modifica la configuración | Alta |

---

## REQUISITOS NO FUNCIONALES

| ID | Requisito |
|----|-----------|
| RNF-01 | `dotnet build` sin errores |
| RNF-02 | `dotnet test` 290+ pruebas pasando |
| RNF-03 | No romper funcionalidades existentes |
| RNF-04 | Mantener arquitectura Repository Pattern + Unit of Work |
| RNF-05 | No agregar migraciones EF salvo para CodigoBarra (campo nullable = no breaking) |
| RNF-06 | Interfaces consistentes con AdminLTE 3 + Bootstrap 5 existentes |

---

## CRITERIOS DE ACEPTACIÓN

| CA | Descripción |
|----|-------------|
| CA-01 | Vendedor puede crear cliente desde venta sin salir del flujo |
| CA-02 | El nuevo cliente se selecciona automáticamente tras guardar |
| CA-03 | No permite crear cliente con DNI duplicado |
| CA-04 | Venta permite múltiples productos con cantidades distintas |
| CA-05 | Stock se descuenta correctamente tras venta |
| CA-06 | Se puede subir imagen de producto con validación |
| CA-07 | Vista detalle de venta muestra todos los datos incluyendo tabla de productos |
| CA-08 | Vendedor no accede a Prendas/Create, Prendas/Edit, Prendas/Delete |
| CA-09 | Logo, nombre, correo y teléfono visibles en layout |
| CA-10 | CodigoBarra aparece en formularios de prenda |
