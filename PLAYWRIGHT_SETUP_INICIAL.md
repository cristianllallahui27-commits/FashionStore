# Setup Inicial de Playwright para FashionStore

## 📦 Instalación Paso a Paso

### 1. Crear Proyecto Playwright

```bash
# En directorio raíz del proyecto
npm create @playwright/test@latest fashionstore-e2e

# O navegar a carpeta existente
cd fashionstore-e2e
npm install @playwright/test
npx playwright install
```

### 2. Estructura Básica de Directorios

```bash
mkdir -p tests/{auth,prendas,ventas,configuracion,fixtures}
mkdir -p utils
```

### 3. Configuración playwright.config.ts

```typescript
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './tests',
  fullyParallel: false,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 1 : undefined,
  reporter: 'html',
  
  use: {
    baseURL: 'http://localhost:5000',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },

  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },

    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },

    {
      name: 'webkit',
      use: { ...devices['Desktop Safari'] },
    },
  ],

  webServer: {
    command: 'dotnet run --project FashionStore.Web',
    url: 'http://localhost:5000',
    reuseExistingServer: !process.env.CI,
  },
});
```

---

## 🔑 Archivo de Variables de Entorno (.env.test)

```env
BASE_URL=http://localhost:5000
ADMIN_EMAIL=admin@fashionstore.com
ADMIN_PASSWORD=Password123!
VENDEDOR_EMAIL=vendedor_test@fashionstore.com
VENDEDOR_PASSWORD=Password123!
```

---

## 📝 Ejemplo 1: Test de Login (auth/login.spec.ts)

```typescript
import { test, expect } from '@playwright/test';

const BASE_URL = process.env.BASE_URL || 'http://localhost:5000';
const ADMIN_EMAIL = 'admin@fashionstore.com';
const ADMIN_PASSWORD = 'Password123!';

test.describe('Autenticación - Login', () => {
  test('Login exitoso como Admin', async ({ page }) => {
    // 1. Navegar a página de login
    await page.goto(`${BASE_URL}/Identity/Account/Login`);
    
    // 2. Verificar que página cargó correctamente
    await expect(page.locator('h1:has-text("Iniciar Sesión")')).toBeVisible();
    
    // 3. Llenar email
    await page.locator('input#Input_Email').fill(ADMIN_EMAIL);
    
    // 4. Llenar password
    await page.locator('input#Input_Password').fill(ADMIN_PASSWORD);
    
    // 5. Click en botón login
    await page.locator('button:has-text("Iniciar Sesión")').click();
    
    // 6. Esperar redirección a Home
    await page.waitForURL(`${BASE_URL}/Home/Index`);
    
    // 7. Validar que está en Home
    await expect(page).toHaveURL(`${BASE_URL}/Home/Index`);
    
    // 8. Validar que aparezca badge Admin
    await expect(page.locator('span.badge:has-text("Admin")')).toBeVisible();
  });

  test('Login fallido con credenciales inválidas', async ({ page }) => {
    await page.goto(`${BASE_URL}/Identity/Account/Login`);
    
    // Llenar con credenciales incorrectas
    await page.locator('input#Input_Email').fill('invalid@example.com');
    await page.locator('input#Input_Password').fill('wrongpassword');
    
    // Click login
    await page.locator('button:has-text("Iniciar Sesión")').click();
    
    // Validar mensaje de error
    const errorMsg = page.locator('div.alert-danger, span.text-danger');
    await expect(errorMsg).toBeVisible();
  });

  test('Logout exitoso', async ({ page, context }) => {
    // 1. Login primero
    await page.goto(`${BASE_URL}/Identity/Account/Login`);
    await page.locator('input#Input_Email').fill(ADMIN_EMAIL);
    await page.locator('input#Input_Password').fill(ADMIN_PASSWORD);
    await page.locator('button:has-text("Iniciar Sesión")').click();
    await page.waitForURL(`${BASE_URL}/Home/Index`);
    
    // 2. Hacer logout
    await page.locator('#userDropdown').click();
    await page.locator('a:has-text("Cerrar Sesión")').click();
    
    // 3. Validar redirección a login
    await page.waitForURL(`${BASE_URL}/Identity/Account/Login`);
    await expect(page).toHaveURL(/Login/);
  });
});
```

---

## 📝 Ejemplo 2: Test de Crear Prenda (prendas/create-prenda.spec.ts)

```typescript
import { test, expect } from '@playwright/test';

const BASE_URL = 'http://localhost:5000';
const ADMIN_EMAIL = 'admin@fashionstore.com';
const ADMIN_PASSWORD = 'Password123!';

test.describe('Gestión de Prendas - Crear', () => {
  
  // Fixture: Login automático
  test.beforeEach(async ({ page }) => {
    await page.goto(`${BASE_URL}/Identity/Account/Login`);
    await page.locator('input#Input_Email').fill(ADMIN_EMAIL);
    await page.locator('input#Input_Password').fill(ADMIN_PASSWORD);
    await page.locator('button:has-text("Iniciar Sesión")').click();
    await page.waitForURL(`${BASE_URL}/Home/Index`);
  });

  test('Crear nueva prenda exitosamente', async ({ page }) => {
    // 1. Navegar a Prendas
    await page.locator('a:has-text("Catálogo")').click();
    await page.locator('a:has-text("Prendas")').click();
    await page.waitForURL(/Prendas/);
    
    // 2. Click en "Crear Prenda"
    await page.locator('a:has-text("Crear Nueva Prenda")').click();
    await page.waitForURL(/Prendas\/Create/);
    
    // 3. Llenar formulario
    const timestamp = Date.now();
    const nombrePrenda = `Test Camiseta ${timestamp}`;
    
    await page.locator('input[name="Nombre"]').fill(nombrePrenda);
    await page.locator('select[name="CategoriaId"]').selectOption('1'); // Seleccionar primera categoría
    await page.locator('input[name="Precio"]').fill('99.99');
    await page.locator('input[name="Stock"]').fill('50');
    await page.locator('textarea[name="Descripcion"]').fill('Prenda de prueba Playwright');
    
    // 4. Click Guardar
    await page.locator('button:has-text("Guardar")').click();
    
    // 5. Validar que se guardó (esperar mensaje de éxito)
    await expect(page.locator('.alert-success, .toastr')).toBeVisible();
    
    // 6. Validar redirección a listado
    await page.waitForURL(/Prendas$/, { timeout: 10000 });
    
    // 7. Buscar la prenda creada en el listado
    await expect(page.locator(`text=${nombrePrenda}`)).toBeVisible();
  });

  test('Validar campos requeridos', async ({ page }) => {
    // 1. Navegar a crear prenda
    await page.locator('a:has-text("Catálogo")').click();
    await page.locator('a:has-text("Prendas")').click();
    await page.locator('a:has-text("Crear Nueva Prenda")').click();
    
    // 2. Intentar guardar sin llenar campos
    await page.locator('button:has-text("Guardar")').click();
    
    // 3. Validar que muestre errores de validación
    const errorMessages = page.locator('span.text-danger, .field-validation-error');
    await expect(errorMessages).toBeTruthy();
  });

  test('Validar precio positivo', async ({ page }) => {
    await page.locator('a:has-text("Catálogo")').click();
    await page.locator('a:has-text("Prendas")').click();
    await page.locator('a:has-text("Crear Nueva Prenda")').click();
    
    // Llenar con precio negativo
    await page.locator('input[name="Nombre"]').fill('Test Prenda');
    await page.locator('input[name="Precio"]').fill('-99.99');
    
    // Intentar guardar
    await page.locator('button:has-text("Guardar")').click();
    
    // Validar error
    await expect(page.locator('.text-danger, .alert-danger')).toBeVisible();
  });
});
```

---

## 📝 Ejemplo 3: Test de Crear Venta (ventas/create-venta.spec.ts)

```typescript
import { test, expect } from '@playwright/test';

const BASE_URL = 'http://localhost:5000';
const VENDEDOR_EMAIL = 'vendedor_test@fashionstore.com';
const VENDEDOR_PASSWORD = 'Password123!';

test.describe('Ventas - Crear (POS)', () => {
  
  test.beforeEach(async ({ page }) => {
    // Login como vendedor
    await page.goto(`${BASE_URL}/Identity/Account/Login`);
    await page.locator('input#Input_Email').fill(VENDEDOR_EMAIL);
    await page.locator('input#Input_Password').fill(VENDEDOR_PASSWORD);
    await page.locator('button:has-text("Iniciar Sesión")').click();
    await page.waitForURL(`${BASE_URL}/Home/Index`);
  });

  test('Crear venta completa', async ({ page }) => {
    // 1. Navegar a Nueva Venta
    await page.locator('a:has-text("Ventas")').click();
    await page.locator('a:has-text("Nueva Venta")').click();
    await page.waitForURL(/Ventas\/Create/);
    
    // 2. Validar que vendedor es READ-ONLY
    const vendedorInput = page.locator('input[readonly][value*="Vendedor"]');
    await expect(vendedorInput).toBeVisible();
    await expect(vendedorInput).toHaveAttribute('readonly', 'readonly');
    
    // 3. Seleccionar cliente
    await page.locator('select#cliente').selectOption('1');
    
    // 4. Buscar y agregar producto por código de barras
    const barcodeInput = page.locator('input#campoCodBarra');
    await barcodeInput.fill('1'); // Código de barras de ejemplo
    await barcodeInput.press('Enter');
    
    // 5. Esperar que se agregue a carrito
    await page.waitForTimeout(1000);
    
    // 6. Establecer cantidad
    const cantidadInputs = page.locator('input[name*="cantidad"]');
    if (await cantidadInputs.count() > 0) {
      await cantidadInputs.first().clear();
      await cantidadInputs.first().fill('2');
    }
    
    // 7. Seleccionar método de pago
    await page.locator('select#metodoPago').selectOption('1');
    
    // 8. Click Registrar Venta
    await page.locator('button:has-text("Registrar Venta")').click();
    
    // 9. Validar confirmación
    await expect(page.locator('.alert-success, .toastr')).toBeVisible();
    
    // 10. Validar redirección
    await page.waitForURL(/Ventas/, { timeout: 10000 });
  });

  test('Validar que no se puede cambiar vendedor', async ({ page }) => {
    await page.goto(`${BASE_URL}/Ventas/Create`);
    
    // Verificar que el campo vendedor no existe como select o está deshabilitado
    const vendedorSelect = page.locator('select#vendedor');
    const vendedorInput = page.locator('input#vendedor[readonly]');
    
    // Debe ser input readonly, no select
    await expect(vendedorInput).toBeVisible();
    await expect(vendedorSelect).not.toBeVisible();
  });

  test('Validar error con stock insuficiente', async ({ page }) => {
    await page.goto(`${BASE_URL}/Ventas/Create`);
    
    // Seleccionar cliente
    await page.locator('select#cliente').selectOption('1');
    
    // Intentar agregar cantidad mayor al stock
    // (necesitarás identificar un producto con stock bajo)
    
    // Click registrar
    await page.locator('button:has-text("Registrar Venta")').click();
    
    // Validar mensaje de error
    await expect(page.locator('.alert-danger, .toastr.ng-trigger-toastrInOut')).toBeVisible();
    await expect(page.locator('text=/[Ss]tock insuficiente/')).toBeVisible();
  });
});
```

---

## 📝 Ejemplo 4: Test de Temas (configuracion/themes.spec.ts)

```typescript
import { test, expect } from '@playwright/test';

const BASE_URL = 'http://localhost:5000';
const ADMIN_EMAIL = 'admin@fashionstore.com';
const ADMIN_PASSWORD = 'Password123!';

test.describe('Cambiar Tema', () => {
  
  test.beforeEach(async ({ page }) => {
    await page.goto(`${BASE_URL}/Identity/Account/Login`);
    await page.locator('input#Input_Email').fill(ADMIN_EMAIL);
    await page.locator('input#Input_Password').fill(ADMIN_PASSWORD);
    await page.locator('button:has-text("Iniciar Sesión")').click();
    await page.waitForURL(`${BASE_URL}/Home/Index`);
  });

  test('Cambiar a tema Oscuro', async ({ page }) => {
    // 1. Click en dropdown Temas
    await page.locator('button:has-text("Temas")').click();
    
    // 2. Seleccionar tema Oscuro
    await page.locator('a[data-theme="theme-dark"]').click();
    
    // 3. Validar que cambió el body class
    const body = page.locator('body');
    const hasThemeDark = await body.evaluate(() => {
      return document.body.classList.contains('theme-dark');
    });
    expect(hasThemeDark).toBe(true);
  });

  test('Tema persiste tras recargar página', async ({ page, context }) => {
    // 1. Cambiar a tema Dark
    await page.locator('button:has-text("Temas")').click();
    await page.locator('a[data-theme="theme-dark"]').click();
    
    // 2. Recargar página
    await page.reload();
    
    // 3. Esperar que cargue
    await page.waitForLoadState('networkidle');
    
    // 4. Validar que tema sigue siendo Dark
    const hasThemeDark = await page.locator('body').evaluate(() => {
      return document.body.classList.contains('theme-dark');
    });
    expect(hasThemeDark).toBe(true);
    
    // 5. Validar localStorage
    const localStorage = await context.storageState();
    const themeStored = await page.evaluate(() => localStorage.getItem('fashionstore_theme'));
    expect(themeStored).toBe('theme-dark');
  });

  test('Cambiar entre múltiples temas', async ({ page }) => {
    const temas = ['theme-blue', 'theme-green', 'theme-orange', 'theme-red'];
    
    for (const tema of temas) {
      // Click dropdown
      await page.locator('button:has-text("Temas")').click();
      
      // Seleccionar tema
      await page.locator(`a[data-theme="${tema}"]`).click();
      
      // Validar cambio
      const hasTheme = await page.locator('body').evaluate((el) => {
        return el.classList.contains(tema);
      });
      expect(hasTheme).toBe(true);
    }
  });
});
```

---

## 🎬 Ejecutar Tests

### Comandos Básicos

```bash
# Ejecutar todos los tests
npx playwright test

# Ejecutar test específico
npx playwright test tests/auth/login.spec.ts

# Ejecutar en modo watch
npx playwright test --watch

# Ejecutar en modo UI (visual)
npx playwright test --ui

# Ejecutar en navegador específico
npx playwright test --project=chromium

# Ver reportes HTML
npx playwright show-report
```

### Con npm scripts (package.json)

```json
{
  "scripts": {
    "test:e2e": "playwright test",
    "test:e2e:ui": "playwright test --ui",
    "test:e2e:watch": "playwright test --watch",
    "test:e2e:headed": "playwright test --headed",
    "test:e2e:debug": "playwright test --debug",
    "test:e2e:report": "playwright show-report"
  }
}
```

---

## 📊 Parámetros para Video

```bash
# Grabar con vídeo en caso de fallo
npx playwright test --trace on

# Ejecutar con screenshots
npx playwright test --screenshot only-on-failure

# Ejecutar en modo headed (ver navegador)
npx playwright test --headed

# Ejecutar con salida detallada
npx playwright test --reporter=list
```

---

## ✅ Próximos Pasos

1. ✅ Crear carpeta fashionstore-e2e
2. ✅ Instalar Playwright
3. ✅ Configurar playwright.config.ts
4. ✅ Crear tests de ejemplo
5. ✅ Ejecutar tests: `npm run test:e2e`
6. ✅ Ver reportes: `npm run test:e2e:report`
7. ✅ Grabar video de ejecución

---

**Estado:** 📋 SETUP LISTO PARA IMPLEMENTAR  
**Próximo:** Implementar los tests y grabar ejecución
