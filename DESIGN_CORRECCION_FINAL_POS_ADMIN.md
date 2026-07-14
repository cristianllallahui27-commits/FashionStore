# DOCUMENTO DE DISEÑO (DESIGN) - Correcciones y Mejoras Finales POS, Ventas y Seguridad

Este documento describe la arquitectura, diagramas, cambios en la base de datos, flujos de datos y mitigación de riesgos para las mejoras funcionales y de seguridad de la tienda FashionStore.

---

## 1. Arquitectura de Descuentos en el POS

### Flujo de Descuento
El usuario del POS interactúa con un control estructurado de descuentos:
1. **Tipo de Descuento (Select)**:
   - "Sin descuento"
   - "Porcentaje (%)"
   - "Monto Fijo (S/.)"
2. **Valor del Descuento**:
   - **Para Vendedor**: El campo de texto de valor manual se bloquea y se muestra un selector de "Descuentos Autorizados" activos de ese tipo. El vendedor solo puede seleccionar un descuento pre-aprobado.
   - **Para Administrador**: Se muestra el selector de descuentos autorizados Y un campo numérico para ingresar un valor personalizado libre (el cual se valida en backend).

### Modelado de Clases
- `Venta` se relaciona con `DescuentoAutorizado` de manera opcional (FK `DescuentoAutorizadoId`).
- El modelo `RegistrarVentaRequest` se expande con:
  - `TipoDescuento`: string ("Ninguno", "Porcentaje", "MontoFijo")
  - `DescuentoValor`: decimal
  - `DescuentoAutorizadoId`: int?

---

## 2. Código de Barras e Integración de Lector
- **Filtro Único en Base de Datos**: Para garantizar la unicidad de los códigos de barra opcionales, se define un índice único condicional en la configuración del modelo de la entidad `Prenda`.
  ```csharp
  modelBuilder.Entity<Prenda>()
      .HasIndex(p => p.CodigoBarra)
      .IsUnique()
      .HasFilter("[CodigoBarra] IS NOT NULL AND [CodigoBarra] <> ''");
  ```
- **Foco y Escaneo Continuo**: En el POS, el campo `codigoBarraInput` detecta el evento `keydown` (tecla Enter). Al escanear, el lector simula la pulsación rápida de dígitos + `Enter`. El JS intercepta la tecla Enter, realiza una petición AJAX a `/api/buscar-codigo/{codigo}` y, en caso de éxito, añade el producto directamente. Luego limpia el input y reestablece el foco, de modo que el vendedor puede seguir escaneando sin usar el mouse.

---

## 3. Buscador Visual de Productos y Carrito
- **Visualización Enriquecida**: Los resultados de búsqueda AJAX en el POS se renderizan usando un contenedor de autocompletado estilizado con imágenes en miniatura, badges de stock y colores con contraste garantizado.
- **Carrito Rediseñado**: Estructurado como una tabla de cuadrícula amplia que muestra:
  - Imagen en miniatura del producto.
  - Nombre y Código de barra (badge gris).
  - Talla y Color en badge de alto contraste.
  - Precio unitario.
  - Spinner numérico de cantidad editable.
  - Subtotal calculado en tiempo real.
  - Botón de remoción (ícono de bote de basura en rojo).

---

## 4. Control de Permisos, Perfiles y Descargas

### Seguridad en Controladores
- Los controladores `PrendasController`, `CategoriasController`, `DescuentosController`, `ConfiguracionController` y `VendedoresController` se protegen a nivel de clase con `[Authorize(Roles = "Administrador")]`.
- El `VentasController` cuenta con acciones compartidas (`Index`, `Create`, `Details`, `Imprimir`, `DescargarExcel`), pero valida el contexto del usuario en el backend:
  - Si el usuario logueado es `Vendedor` (y no `Administrador`), el backend inyecta filtros para limitar la consulta de ventas al ID del vendedor asociado a su correo registrado.
  - Si intenta consultar una venta ajena (mediante manipulación de ID en la URL de detalles o descargas), se retorna una redirección con error.

---

## 5. Diseño de la Configuración y Branding
- **ConfiguracionSistema Mappings**:
  - En `FashionStoreDbContext.cs`, se añade Fluent API para mapear `RutaLogo` a `LogoUrl` y `RutaFavicon` a `FaviconUrl` en la tabla `Configuraciones` para evitar colisiones de esquema existentes en SQL Server.
- **Vista de Configuración**: Se organiza en pestañas temáticas (Datos de Tienda, Apariencia y Colores, Logotipos). Cuenta con un área drag-and-drop o input estilizado de archivos para cargar el logotipo con validación del tamaño y formato de la imagen en Javascript y C#.
- **Propagación en Layouts**: Los datos cargados se inyectan en el pie de página de ambos layouts (`Views/Shared/_Layout.cshtml` y `Pages/Shared/_Layout.cshtml`).

---

## 6. Cambios en Archivos del Proyecto

| Tipo de Archivo | Ruta del Archivo | Descripción del Cambio |
| --- | --- | --- |
| **Entidad** | `FashionStore.Domain/Entities/ConfiguracionSistema.cs` | Propiedades de configuración. |
| **Mapeo / DB** | `FashionStore.Infrastructure/Context/FashionStoreDbContext.cs` | Fluent API mappings para `LogoUrl`, `FaviconUrl` e índice único de código de barra. |
| **Controlador** | `FashionStore.Web/Controllers/VentasController.cs` | Añadir endpoint `api/buscar-codigo/{codigo}`, validar descuentos en backend y añadir filtros de propiedad en descargas. |
| **Controlador** | `FashionStore.Web/Controllers/PrendasController.cs` | Validación de código de barra único en acciones `Create` y `Edit` (POST). |
| **Vistas POS** | `FashionStore.Web/Views/Ventas/Create.cshtml` | Añadir lector de código de barras por Enter, tipo de descuento (porcentaje/soles) y listado visual. |
| **Vistas Config** | `FashionStore.Web/Views/Configuracion/Index.cshtml` | Rediseñar la interfaz de carga y edición de configuración del sistema. |
| **Layouts** | `FashionStore.Web/Views/Shared/_Layout.cshtml` & `Pages/Shared/_Layout.cshtml` | Añadir visualización de correo y teléfono y arreglar enlaces de perfil `/Perfil`. |

---

## 7. Riesgos y Mitigaciones
- **Riesgo**: Manipulación de precios o descuentos de venta en el frontend de JavaScript.
  - *Mitigación*: El backend vuelve a consultar el precio real de cada prenda en la base de datos y calcula el subtotal y descuento de forma transaccional e independiente de lo enviado por el navegador.
- **Riesgo**: Carga de imágenes pesadas que saturen el espacio de almacenamiento del servidor.
  - *Mitigación*: Validación rígida tanto en JS (lado del cliente) como en el controlador de la API (lado del servidor) verificando extensión (`.jpg`, `.jpeg`, `.png`, `.webp`) y tamaño máximo (5MB).
