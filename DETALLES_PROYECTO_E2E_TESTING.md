# FashionStore - Detalles del Proyecto para Pruebas E2E con Playwright

## 📋 INFORMACIÓN GENERAL DEL PROYECTO

### 1. Stack Tecnológico

**Backend:**
- **Framework:** ASP.NET Core 9.0
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Auth:** ASP.NET Identity (Identity Framework)
- **Architecture:** N-Tier (Domain, Infrastructure, Web)
- **Patrón:** Repository Pattern + Unit of Work

**Frontend:**
- **View Engine:** Razor Pages & MVC Views
- **CSS:** Bootstrap 5.3.3
- **JavaScript:** jQuery + Vanilla JS
- **UI Framework:** Bootstrap components

**Testing:**
- **xUnit:** 285 tests unitarios
- **Code Coverage:** ~85%+
- **E2E Tools:** Playwright (a implementar)

---

## 🔐 ACCESOS Y ROLES DEL SISTEMA

### 1. Roles Disponibles

| Rol | Descripción | Permisos Principales |
|-----|-------------|----------------------|
| **Administrador** | Control total del sistema | Ver todo, crear/editar/eliminar, gestionar usuarios |
| **Vendedor** | Puede crear y ver sus propias ventas | Crear ventas, ver solo sus ventas |
| **Cliente** | Solo acceso de consulta (no implementado aún) | Ver catálogo, historial de compras |

### 2. Credenciales de Prueba

#### **Usuario Admin (Producción)**
```
Email:    admin@fashionstore.com
Password: Password123!
Rol:      Administrador
Permisos: Acceso completo a todas las secciones
```

#### **Usuarios Vendedores (para crear durante pruebas)**
```
Formato Email:  vendedor_[nombre]@fashionstore.com
Password:       Password123! (o la que asignes)
Rol:            Vendedor
Permisos:       Crear ventas, ver sus propias ventas
```

#### **Cliente Genérico**
```
ID:    1
Nombre: Cliente Genérico
Email:  generico@fashionstore.com
Rol:    Comprador (sin login directo)
```

---

## 🌐 URLS PRINCIPALES DEL SISTEMA

### Base URL
```
Local:      http://localhost:5000
Staging:    https://fashionstore-staging.azurewebsites.net
Production: https://fashionstore.azurewebsites.net
```

### Rutas Principales

| Sección | URL | Rol Requerido | Descripción |
|---------|-----|---------------|-------------|
| **Inicio** | `/Home/Index` | Cualquiera (Autenticado) | Panel principal |
| **Login** | `/Identity/Account/Login` | Público | Inicio de sesión |
| **Logout** | `/Identity/Account/Logout` | Autenticado | Cerrar sesión |
| **Prendas** | `/Prendas` | Admin | Gestión de inventario |
| **Categorías** | `/Categorias` | Admin | Gestión de categorías |
| **Clientes** | `/Clientes` | Admin | Gestión de clientes |
| **Vendedores** | `/Vendedores` | Admin | Gestión de vendedores |
| **Ventas - Crear** | `/Ventas/Create` | Admin/Vendedor | POS (Punto de Venta) |
| **Ventas - Lista** | `/Ventas` | Admin/Vendedor | Historial de ventas |
| **Configuración** | `/Configuracion` | Admin | Sistema settings |
| **Descuentos** | `/Descuentos` | Admin | Gestión de descuentos |
| **Reportes** | `/Reportes` | Admin | Análisis y reportes |

---

## 🔄 FLUJOS PRINCIPALES PARA PRUEBAS E2E

### Flujo 1: Autenticación Admin

```
1. Acceder a /Identity/Account/Login
2. Ingresar email: admin@fashionstore.com
3. Ingresar password: Password123!
4. Click "Iniciar Sesión"
5. Validar redirección a /Home/Index
6. Validar que aparezca badge "Admin" en navbar
```

### Flujo 2: Autenticación Vendedor

```
1. Acceder a /Identity/Account/Login
2. Ingresar email vendedor (ej: vendedor_juan@fashionstore.com)
3. Ingresar password
4. Click "Iniciar Sesión"
5. Validar redirección a /Home/Index
6. Validar que aparezca badge "Vendedor" en navbar
7. Validar que menú solo muestre "Ventas" (no Admin)
```

### Flujo 3: Crear Nueva Prenda (Admin)

```
1. Autenticarse como Admin
2. Navegar a Admin > Catálogo > Prendas
3. Click "Crear Nueva Prenda"
4. Llenar formulario:
   - Nombre: "Test Camiseta"
   - Categoría: Seleccionar una
   - Precio: 99.99
   - Stock: 50
   - Descripción: "Prenda de prueba"
5. Click "Guardar"
6. Validar que aparezca en listado
7. Validar que el stock sea correcto
```

### Flujo 4: Crear Nueva Venta (Vendedor)

```
1. Autenticarse como Vendedor
2. Navegar a Ventas > Nueva Venta (POS)
3. Validar que vendedor sea READ-ONLY (no seleccionable)
4. Seleccionar Cliente del dropdown
5. Buscar producto por código de barras o nombre
6. Agregar producto al carrito
7. Establecer cantidad
8. Seleccionar Método de Pago
9. Click "Registrar Venta"
10. Validar confirmación y número de venta
11. Validar que stock disminuya
```

### Flujo 5: Gestionar Usuarios (Admin)

```
1. Autenticarse como Admin
2. Navegar a Admin > Usuarios y Accesos
3. Click "Crear Nuevo Usuario"
4. Ingresar:
   - Email: vendedor_test@fashionstore.com
   - Password: Test123456
   - Rol: Vendedor
5. Click "Crear Usuario"
6. Validar que aparezca en listado
7. Toggle Activo/Inactivo
8. Validar cambio de estado
```

### Flujo 6: Configurar Sistema (Admin)

```
1. Autenticarse como Admin
2. Navegar a Admin > Configuración
3. En tab "Branding":
   - Cargar logo (JPG/PNG/WEBP)
   - Cargar favicon
4. En tab "Temas":
   - Seleccionar tema (Dark, Blue, etc.)
5. En tab "Colores":
   - Cambiar color primario
   - Cambiar color secundario
6. En tab "Datos del Negocio":
   - Llenar información
7. Click "Guardar Configuración"
8. Validar que cambios persistan al recargar
```

### Flujo 7: Cambiar Tema (Cualquier usuario)

```
1. Estar autenticado
2. Ver navbar
3. Click "Temas" (dropdown con paleta)
4. Seleccionar tema (Dark, Blue, Green, etc.)
5. Validar que se aplique inmediatamente
6. Recargar página
7. Validar que tema se mantenga (localStorage)
```

---

## 🗄️ MODELOS Y DATOS IMPORTANTES

### Entidades Principales

#### Cliente
```csharp
public class Cliente {
    public int Id { get; set; }
    public string NombreCompleto { get; set; }
    public string DNI { get; set; }
    public string? Email { get; set; }
    public string? Telefono { get; set; }
}
```

#### Prenda
```csharp
public class Prenda {
    public int Id { get; set; }
    public string Nombre { get; set; }
    public decimal Precio { get; set; }
    public int Stock { get; set; }
    public int CategoriaId { get; set; }
    public Categoria Categoria { get; set; }
}
```

#### Venta
```csharp
public class Venta {
    public int Id { get; set; }
    public int ClienteId { get; set; }
    public int VendedorId { get; set; }
    public int MetodoPagoId { get; set; }
    public decimal Total { get; set; }
    public DateTime Fecha { get; set; }
    public List<DetalleVenta> Detalles { get; set; }
}
```

#### Vendedor
```csharp
public class Vendedor {
    public int Id { get; set; }
    public string Nombres { get; set; }
    public string Apellidos { get; set; }
    public string DNI { get; set; }
    public string Correo { get; set; }
    public bool Estado { get; set; }
}
```

---

## 📱 ELEMENTOS DE UI CLAVE PARA SELECCIONADORES

### Navbar
```html
<!-- Logo/Home -->
<a class="navbar-brand" href="/Home/Index">FashionStore</a>

<!-- Link Inicio -->
<a class="nav-link" href="/Home/Index">
    <i class="fas fa-home me-1"></i>Inicio
</a>

<!-- Dropdown Catálogo -->
<a class="nav-link dropdown-toggle" href="#" data-bs-toggle="dropdown">
    <i class="fas fa-tshirt me-1"></i>Catálogo
</a>

<!-- Dropdown Admin -->
<a class="nav-link dropdown-toggle" href="#" data-bs-toggle="dropdown">
    <i class="fas fa-users-cog me-1"></i>Admin
</a>

<!-- Dropdown Ventas -->
<a class="nav-link dropdown-toggle" href="#" data-bs-toggle="dropdown">
    <i class="fas fa-cash-register me-1"></i>Ventas
</a>

<!-- User Dropdown -->
<a class="nav-link dropdown-toggle" href="#" id="userDropdown" data-bs-toggle="dropdown">
    <i class="fas fa-user"></i>
</a>

<!-- Logout -->
<a class="dropdown-item text-danger" href="#" onclick="document.getElementById('logoutForm').submit();">
    <i class="fas fa-sign-out-alt me-2"></i>Cerrar Sesión
</a>
```

### Formularios Login
```html
<!-- Email Input -->
<input id="Input_Email" name="Input.Email" type="email" class="form-control" required>

<!-- Password Input -->
<input id="Input_Password" name="Input.Password" type="password" class="form-control" required>

<!-- Login Button -->
<button type="submit" class="btn btn-primary">Iniciar Sesión</button>
```

### Botones de Acción Común
```html
<!-- Crear -->
<a href="/Prendas/Create" class="btn btn-primary">
    <i class="fas fa-plus me-1"></i>Crear Prenda
</a>

<!-- Editar -->
<a href="/Prendas/Edit/@prenda.Id" class="btn btn-warning btn-sm">
    <i class="fas fa-edit me-1"></i>Editar
</a>

<!-- Eliminar -->
<button onclick="confirmarEliminar('@item.Id', '@item.Nombre')" class="btn btn-danger btn-sm">
    <i class="fas fa-trash me-1"></i>Eliminar
</button>

<!-- Guardar Configuración -->
<button onclick="guardarConfiguracion()" class="btn btn-primary btn-lg">
    <i class="fas fa-save me-2"></i>Guardar Configuración
</button>
```

### Selects y Dropdowns
```html
<!-- Select Cliente -->
<select class="form-select" id="cliente" required>
    <option value="">-- Selecciona cliente --</option>
    <!-- Options populated dynamically -->
</select>

<!-- Select Método de Pago -->
<select class="form-select" id="metodoPago" required>
    <option value="">-- Selecciona método --</option>
    <!-- Options populated dynamically -->
</select>

<!-- Select Categoría -->
<select class="form-select" id="categoria" required>
    <option value="">-- Selecciona categoría --</option>
    <!-- Options populated dynamically -->
</select>
```

---

## 📊 VALIDACIONES IMPORTANTES

### Validaciones de Entrada

| Campo | Validación | Ejemplo |
|-------|-----------|---------|
| Email | Formato válido | user@example.com |
| Password | Mínimo 6 caracteres | Password123! |
| DNI | 8 dígitos | 12345678 |
| Teléfono | Formato numérico | 987654321 |
| Precio | Número positivo | 99.99 |
| Stock | Número entero ≥ 0 | 50 |
| Cantidad Venta | Número > 0 | 1-999 |

### Validaciones de Negocio

1. **Stock insuficiente:** No se puede vender más de lo disponible
2. **Vendedor activo:** Solo vendedores con Estado=true pueden vender
3. **Usuario único:** No se pueden duplicar emails
4. **Categoría requerida:** Toda prenda debe tener categoría
5. **Cliente requerido:** Toda venta necesita cliente
6. **Método de pago requerido:** Toda venta necesita forma de pago

---

## 🔗 ENDPOINTS DE API (si necesitas pruebas de API)

### Configuración
```
GET  /api/configuracion/obtener
POST /api/configuracion/actualizar
POST /api/configuracion/cargar-imagen
POST /api/configuracion/restablecer
POST /api/configuracion/guardar-tema
```

### Ventas
```
POST /Ventas/api/registrar-venta
POST /Ventas/api/validar-venta
GET  /Ventas
GET  /Ventas/Details/{id}
```

### Productos
```
GET  /Prendas
GET  /Prendas/Details/{id}
POST /Prendas/Create
POST /Prendas/Edit/{id}
```

---

## 📝 PATRONES DE SELECCIÓN PARA PLAYWRIGHT

### Selectores CSS Comunes

```javascript
// Inputs de texto
const emailInput = page.locator('input[id="Input_Email"]');
const passwordInput = page.locator('input[id="Input_Password"]');
const nombreInput = page.locator('input[name="Nombre"]');

// Selects
const clienteSelect = page.locator('select#cliente');
const metodoPagoSelect = page.locator('select#metodoPago');

// Botones
const loginBtn = page.locator('button[type="submit"]:has-text("Iniciar Sesión")');
const crearBtn = page.locator('a:has-text("Crear")');
const guardarBtn = page.locator('button:has-text("Guardar")');

// Links
const inicioLink = page.locator('a[href="/Home/Index"]');
const prendaLink = page.locator('a[href="/Prendas"]');

// Elementos dinámicos
const successAlert = page.locator('.alert-success');
const errorAlert = page.locator('.alert-danger');
const badge = page.locator('span.badge');

// Navbar
const navbar = page.locator('.navbar');
const userDropdown = page.locator('#userDropdown');
const logoutLink = page.locator('a:has-text("Cerrar Sesión")');
```

---

## 🎯 CASOS DE PRUEBA SUGERIDOS (minuciosos)

### TC001: Autenticación Exitosa (Admin)
- Pre: Usuario en página login
- Steps: 1-5 (ver Flujo 1)
- Expected: Redirigido a Home, aparece badge Admin

### TC002: Autenticación Fallida
- Pre: Usuario en página login
- Steps: Ingresar email inválido o password incorrecto
- Expected: Mensaje de error visible

### TC003: Crear Prenda Nueva
- Pre: Admin autenticado
- Steps: 1-7 (ver Flujo 3)
- Expected: Prenda aparece en listado

### TC004: Crear Venta Completa
- Pre: Vendedor autenticado
- Steps: 1-11 (ver Flujo 4)
- Expected: Venta registrada, stock actualizado

### TC005: Cambiar Tema
- Pre: Usuario autenticado
- Steps: 1-7 (ver Flujo 7)
- Expected: Tema persiste tras recargar

### TC006: Crear Usuario
- Pre: Admin autenticado
- Steps: 1-8 (ver Flujo 5)
- Expected: Usuario creado, puede iniciar sesión

### TC007: Validar Stock Insuficiente
- Pre: Vendedor intentando vender más que stock disponible
- Steps: Agregar cantidad mayor a stock en carrito
- Expected: Error: "Stock insuficiente"

### TC008: Logout
- Pre: Usuario autenticado
- Steps: Click logout
- Expected: Redirigido a login, sesión cerrada

---

## 🛠️ SETUP PARA PLAYWRIGHT

### Instalación Inicial

```bash
# Crear proyecto Playwright
npm create @playwright/test@latest fashionstore-e2e

# Instalar Playwright
npm install @playwright/test

# Instalar browsers
npx playwright install
```

### Estructura de Carpetas Recomendada

```
fashionstore-e2e/
├── tests/
│   ├── auth/
│   │   ├── login.spec.ts
│   │   └── logout.spec.ts
│   ├── prendas/
│   │   ├── create-prenda.spec.ts
│   │   └── list-prendas.spec.ts
│   ├── ventas/
│   │   ├── create-venta.spec.ts
│   │   └── list-ventas.spec.ts
│   ├── configuracion/
│   │   ├── themes.spec.ts
│   │   └── settings.spec.ts
│   └── fixtures/
│       ├── auth.fixture.ts
│       └── data.fixture.ts
├── utils/
│   ├── helpers.ts
│   └── selectors.ts
├── playwright.config.ts
└── package.json
```

---

## 📋 CHECKLIST PRE-TESTING

- [ ] Proyecto compilado sin errores
- [ ] 285 tests unitarios pasando
- [ ] Base de datos limpia o con data de prueba
- [ ] Usuarios Admin y Vendedor creados
- [ ] URL base accesible
- [ ] Playwright instalado (npm install)
- [ ] Browsers descargados (npx playwright install)
- [ ] playwright.config.ts configurado
- [ ] Variables de entorno configuradas

---

## 🎬 ÁREAS A CUBRIR EN VIDEO

1. **Introducción** (1-2 min)
   - Presentar proyecto
   - Explicar roles y accesos
   - Mostrar estructura

2. **Setup y Configuración** (3-5 min)
   - Instalación de Playwright
   - Configuración inicial
   - Estructura de carpetas

3. **Ejecución de Tests** (10-15 min)
   - Test de login (Admin y Vendedor)
   - Test de crear prenda
   - Test de crear venta
   - Test de cambiar tema
   - Test de crear usuario

4. **Análisis de Resultados** (5-8 min)
   - Cobertura alcanzada
   - Resultados exitosos
   - Casos con falla (si los hay)
   - Screenshots en caso de error

5. **Conclusiones** (2-3 min)
   - Resumen de pruebas
   - Recomendaciones
   - Próximos pasos

---

**Status:** 📋 DOCUMENTO LISTO PARA E2E TESTING  
**Última actualización:** 2025-07-08  
**Próximo paso:** Implementar tests con Playwright basado en esta documentación
