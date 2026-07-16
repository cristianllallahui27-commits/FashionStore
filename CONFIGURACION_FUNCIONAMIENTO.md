# Configuración del Sistema - Guía de Funcionamiento

## 📋 Descripción General

La página **Configuración del Sistema** es el centro neurálgico donde los administradores pueden personalizar toda la aplicación, incluyendo:

- 🏪 Identidad de la marca (nombre, logo, favicon)
- 🎨 Colores y temas personalizados
- 📸 Imágenes de branding
- 📊 Datos del negocio
- 🔗 Redes sociales
- 👥 Gestión de usuarios y accesos

---

## 🎯 Tabs Disponibles

### 1️⃣ **Branding**
**¿Qué contiene?**
- Nombre de la tienda
- Logo (se muestra en navbar)
- Favicon (icono del navegador)
- Imagen institucional

**¿Cómo usarlo?**
- Carga imágenes en formato JPG, PNG o WEBP (máx 5MB)
- Las imágenes se guardan en `/uploads/` con nombres únicos
- Al cargar, se muestra preview inmediatamente
- Los cambios se guardan en BD al presionar "Guardar Configuración"

---

### 2️⃣ **Temas**
**¿Qué contiene?**
- 6 temas predefinidos (Fashion Store, Claro, Oscuro, Navidad, San Valentín, Black Friday)
- Botones visuales con colores

**¿Cómo usarlo?**
- Click en un tema para aplicar colores automáticamente
- Los campos de colores abajo se actualizan
- Presiona "Guardar Configuración" para guardar

**Temas disponibles:**
- 🟣 **Fashion Store**: Púrpura predeterminado
- ⚪ **Claro**: Blanco y limpio
- ⚫ **Oscuro**: Negro moderno
- 🎄 **Navidad**: Rojo y verde
- 💝 **San Valentín**: Rosa y rojo
- 🔥 **Black Friday**: Negro con amarillo
- 💻 **Cyber Days**: Azul y magenta
- 🌸 **Primavera**: Verde fresco
- 📚 **Escolar**: Azul y naranja

---

### 3️⃣ **Colores Personalizados**
**¿Qué contiene?**
- Selector de colores para cada elemento:
  - Color Primario
  - Color Secundario
  - Color del Menú
  - Color de Botones
  - Color del Dashboard
  - Color de Fondo

**¿Cómo usarlo?**
1. Click en cada cuadro de color
2. Selecciona el color que deseas
3. Vista previa se actualiza en tiempo real
4. Presiona "Guardar Configuración" para guardar

---

### 4️⃣ **Fondos e Imágenes**
**¿Qué contiene?**
- Fondo del Login
- Fondo del Dashboard
- Fondo del Menú
- Imagen del Login
- Banner Principal

**¿Cómo usarlo?**
- Click en el área de carga para seleccionar imagen
- Formatos: JPG, PNG, WEBP (máx 5MB)
- Verás preview inmediatamente
- Se guarda en `/uploads/` con nombre único
- Guardar Configuración persiste en BD

---

### 5️⃣ **Datos del Negocio**
**¿Qué contiene?**
- Nombre del Propietario
- Teléfono
- Correo
- Dirección
- Ciudad
- País
- Código Postal
- RUC/ID del Negocio
- Descripción

**¿Cómo usarlo?**
- Rellena los campos con info de tu negocio
- Se mostrará en reportes, emails, facturas
- Presiona "Guardar Configuración"

---

### 6️⃣ **Redes Sociales**
**¿Qué contiene?**
- Facebook URL
- Instagram URL
- Twitter URL
- LinkedIn URL
- TikTok URL

**¿Cómo usarlo?**
- Pega las URLs de tus perfiles
- Se mostrarán en footer y páginas públicas
- Presiona "Guardar Configuración"

---

### 7️⃣ **Usuarios y Accesos**
**¿Qué contiene?**
- Lista de todos los usuarios del sistema
- Roles: Administrador, Vendedor
- Estado: Activo/Inactivo
- Opción crear nuevos usuarios

**¿Cómo usarlo?**
- **Ver usuarios**: Tabla con todos los registrados
- **Crear usuario**: Presiona botón azul, ingresa:
  - Email
  - Contraseña (mín 6 caracteres)
  - Rol (Administrador o Vendedor)
- **Activar/Desactivar**: Toggle en la tabla
- Los cambios se guardan automáticamente

---

### 8️⃣ **Descuentos**
**¿Qué contiene?**
- Descuentos autorizados del sistema
- Porcentaje y descripción

**¿Cómo usarlo?**
- Vista solo lectura de descuentos activos
- Administrados desde la sección "Admin > Descuentos"

---

## 🔘 Botones de Acción

### 🔵 **Guardar Configuración**
**¿Qué hace?**
- Envía todos los cambios al servidor
- Valida los datos
- Guarda en base de datos
- Recarga la página para aplicar cambios

**Cambios guardados:**
- Nombre de tienda
- Colores
- Temas
- Datos del negocio
- Redes sociales
- Rutas de imágenes

**Si falla:**
- Verás mensaje de error
- Revisa que todos los campos sean válidos
- Intenta nuevamente

---

### 🟨 **Restablecer Valores Predeterminados**
**¿Qué hace?**
1. Solicita confirmación (no se puede deshacer)
2. Borra TODOS los cambios
3. Vuelve a valores originales de instalación
4. Recarga la página

**Valores por defecto:**
- Nombre: "FashionStore"
- Color primario: #667eea
- Color secundario: #764ba2
- Sin imágenes personalizadas
- Sin redes sociales

**⚠️ Advertencia**: Esta acción es irreversible. Todos los cambios se perderán.

---

### ⚙️ **Recargar**
**¿Qué hace?**
- Trae los valores actuales desde BD
- Descarta cambios no guardados
- Actualiza vista con datos de servidor

**Cuándo usarlo:**
- Realizaste cambios y quieres deshacer
- La página no refleja cambios guardados
- Para sincronizar si otra ventana modificó datos

---

## 💾 Base de Datos

### Tabla: ConfiguracionSistema
```sql
Id (int) - Siempre 1
NombreTienda (string)
RutaLogo (string)
RutaFavicon (string)
RutaImagenLogin (string)
RutaImagenInstitucional (string)
RutaBanner (string)
RutaFondoLogin (string)
RutaFondoDashboard (string)
RutaFondoMenu (string)
ColorPrimario (string) - Hex color
ColorSecundario (string)
ColorMenu (string)
ColorBotones (string)
ColorDashboard (string)
ColorFondo (string)
TemaSeleccionado (string)
TemaOscuro (bool)
NombrePropietario (string)
Telefono (string)
Correo (string)
Direccion (string)
Ciudad (string)
Pais (string)
CodigoPostal (string)
RUC (string)
Descripcion (string)
FacebookUrl (string)
InstagramUrl (string)
TwitterUrl (string)
LinkedInUrl (string)
TikTokUrl (string)
TextoPiePagina (string)
FechaCreacion (datetime)
FechaActualizacion (datetime)
```

---

## 🔐 Permisos y Autorización

**Solo Administradores** pueden acceder a esta página.

**URL de acceso:**
- `/Configuracion`
- Admin > Configuración

Si intentas sin ser admin: **Acceso Denegado**

---

## 📡 Endpoints de API

### POST `/api/configuracion/actualizar`
Guarda todos los cambios
```json
{
  "id": 1,
  "nombreTienda": "Mi Tienda",
  "colorPrimario": "#667eea",
  ...
}
```

### POST `/api/configuracion/cargar-imagen`
Sube una imagen
```
FormData:
- archivo: File
- campo: "logo" | "favicon" | "imagen_login" | etc.
```

### POST `/api/configuracion/restablecer`
Vuelve a valores por defecto
```
Respuesta: { success: true, data: ConfiguracionSistema }
```

### GET `/api/configuracion/obtener`
Obtiene configuración actual
```
Respuesta: { success: true, data: ConfiguracionSistema }
```

---

## ✅ Flujo Completo de Uso

### 1. Configurar Identidad de Marca
```
1. Tab "Branding"
2. Carga logo
3. Carga favicon
4. Presiona "Guardar Configuración"
✅ Logo aparece en navbar automáticamente
```

### 2. Personalizar Colores
```
1. Tab "Colores Personalizados"
2. Selecciona colores
3. O Tab "Temas" y elige uno predefinido
4. Presiona "Guardar Configuración"
✅ Los colores se aplican en toda la app
```

### 3. Agregar Datos del Negocio
```
1. Tab "Datos del Negocio"
2. Completa formulario
3. Tab "Redes Sociales" y agrega URLs
4. Presiona "Guardar Configuración"
✅ Se mostrarán en reportes y footer
```

### 4. Crear Nuevo Usuario (Vendedor)
```
1. Tab "Usuarios y Accesos"
2. Presiona botón "Crear Nuevo Usuario"
3. Ingresa email y contraseña
4. Selecciona rol: Vendedor
5. Presiona "Crear Usuario"
✅ El vendedor puede acceder con email + password
```

---

## 🐛 Troubleshooting

### Problema: "Error al guardar la configuración"
**Soluciones:**
- Verifica que todos los campos obligatorios estén llenos
- Recarga la página y intenta nuevamente
- Revisa la consola del navegador (F12) para más detalles

### Problema: "Imagen no carga"
**Soluciones:**
- Verifica que sea JPG, PNG o WEBP
- Verifica que el tamaño sea menor a 5MB
- Intenta con otra imagen

### Problema: "Cambios no se guardan"
**Soluciones:**
- Presiona "Guardar Configuración" (no aparece automático)
- Verifica que no haya errores de validación
- Intenta "Recargar" primero

### Problema: "Los colores no cambian en toda la app"
**Soluciones:**
- Presiona "Guardar Configuración"
- Recarga la página (F5)
- Limpia cache del navegador (Ctrl+Shift+Delete)

---

## 🔄 Sincronización con Base de Datos

- **Escritura**: Todos los cambios se guardan en BD al presionar "Guardar"
- **Lectura**: Al cargar la página, obtiene datos de BD
- **Cache**: Los datos se cachean en la aplicación para mejor performance
- **Auditoría**: FechaActualizacion se actualiza automáticamente

---

## 📌 Notas Importantes

1. ⚠️ Solo hay **UNA** configuración global (ID = 1)
2. 📁 Las imágenes se guardan en `/wwwroot/uploads/`
3. 🔒 Los datos de configuración se inyectan en todas las vistas
4. 🎨 Los cambios de color se reflejan en **tiempo real** tras guardar
5. 👥 Los usuarios creados aquí son usuarios del sistema (Identity)
6. 💾 Todos los cambios son auditados (FechaActualizacion)

---

**Status:** ✅ Completamente Funcional
**Último Update:** 2025-07-08
**Version:** 1.0
