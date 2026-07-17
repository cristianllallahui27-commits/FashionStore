# Plan E2E Testing - FashionStore
## Resumen Ejecutivo

---

## 📊 INFORMACIÓN GENERAL

| Aspecto | Detalle |
|---------|---------|
| **Proyecto** | FashionStore - Sistema de Gestión de Tienda |
| **Stack** | ASP.NET Core 9, EF Core, SQL Server, Bootstrap 5, Razor |
| **Herramienta E2E** | Playwright |
| **Navegadores** | Chromium, Firefox, WebKit |
| **Estado Actual** | Build: ✅ 0 Errores | Tests Unitarios: ✅ 285/285 Pasando |
| **Documentación Generada** | 2 archivos con setup completo |

---

## 🔐 ACCESOS Y ROLES

### Usuarios de Prueba Disponibles

```
┌─────────────────┬──────────────────────┬──────────────┬─────────────────────────┐
│ Rol             │ Email                │ Password     │ Permisos                │
├─────────────────┼──────────────────────┼──────────────┼─────────────────────────┤
│ Admin           │ admin@fashionstore   │ Password123! │ Acceso total al sistema │
│ Vendedor        │ vendedor_*.@*        │ Asignable    │ Crear/ver sus ventas    │
│ Cliente         │ generico@*           │ No login     │ Comprador               │
└─────────────────┴──────────────────────┴──────────────┴─────────────────────────┘
```

---

## 🎯 FLUJOS PRINCIPALES A PROBAR

### 1. ✅ Autenticación
- Login exitoso (Admin y Vendedor)
- Login fallido (credenciales inválidas)
- Logout
- Validación de sesión

### 2. ✅ Gestión de Productos (Admin)
- Crear prenda nueva
- Editar prenda existente
- Listar prendas
- Validaciones de campos
- Búsqueda y filtros

### 3. ✅ Ventas (POS - Punto de Venta)
- Crear venta completa
- Validar vendedor asignado automáticamente
- Agregar productos al carrito
- Calcular total con descuentos
- Validar stock insuficiente
- Imprimir ticket

### 4. ✅ Configuración del Sistema (Admin)
- Cambiar temas (6 opciones)
- Cargar imágenes (logo, favicon, fondos)
- Personalizar colores
- Guardar datos del negocio
- Persistencia de cambios

### 5. ✅ Gestión de Usuarios (Admin)
- Crear nuevo usuario
- Activar/desactivar usuario
- Cambiar rol
- Ver listado de usuarios

### 6. ✅ Validaciones
- Campos requeridos
- Formatos de datos
- Stock disponible
- Valores numéricos

---

## 📋 CASOS DE PRUEBA DOCUMENTADOS

**Total de casos:** 8 casos principales + variantes

| # | Caso | Flujo | Status |
|---|------|-------|--------|
| TC001 | Autenticación Exitosa (Admin) | Login completo | 📋 Documentado |
| TC002 | Autenticación Fallida | Login inválido | 📋 Documentado |
| TC003 | Crear Prenda Nueva | Inventario | 📋 Documentado |
| TC004 | Crear Venta Completa | POS | 📋 Documentado |
| TC005 | Cambiar Tema | Configuración | 📋 Documentado |
| TC006 | Crear Usuario | Admin | 📋 Documentado |
| TC007 | Validar Stock Insuficiente | Ventas | 📋 Documentado |
| TC008 | Logout | Sesión | 📋 Documentado |

---

## 🛠️ SETUP NECESARIO

### Instalación

```bash
# 1. Crear carpeta Playwright
npm create @playwright/test@latest fashionstore-e2e

# 2. Instalar dependencias
npm install @playwright/test

# 3. Instalar navegadores
npx playwright install

# 4. Configurar .env.test con credenciales
BASE_URL=http://localhost:5000
ADMIN_EMAIL=admin@fashionstore.com
ADMIN_PASSWORD=Password123!
```

### Estructura de Pruebas

```
tests/
├── auth/
│   ├── login.spec.ts          (3 tests)
│   └── logout.spec.ts         (1 test)
├── prendas/
│   ├── create-prenda.spec.ts  (3 tests)
│   └── list-prendas.spec.ts   (2 tests)
├── ventas/
│   ├── create-venta.spec.ts   (3 tests)
│   └── list-ventas.spec.ts    (1 test)
├── configuracion/
│   ├── themes.spec.ts         (4 tests)
│   └── settings.spec.ts       (2 tests)
└── fixtures/
    ├── auth.fixture.ts
    └── data.fixture.ts
```

---

## 📝 ARCHIVOS GENERADOS

### 1. `DETALLES_PROYECTO_E2E_TESTING.md` (390 líneas)
**Contenido:**
- ✅ Stack tecnológico completo
- ✅ Accesos y roles (con credenciales)
- ✅ URLs del sistema
- ✅ 8 Flujos principales con pasos
- ✅ Modelos de datos
- ✅ Selectores CSS para Playwright
- ✅ 8 Casos de prueba detallados
- ✅ Validaciones de negocio
- ✅ Endpoints de API

### 2. `PLAYWRIGHT_SETUP_INICIAL.md` (450 líneas)
**Contenido:**
- ✅ Instalación paso a paso
- ✅ playwright.config.ts completo
- ✅ Variables de entorno
- ✅ 4 ejemplos completos de tests:
  - Login (exitoso, fallido, logout)
  - Crear prenda (con validaciones)
  - Crear venta (POS completo)
  - Cambiar temas
- ✅ Comandos de ejecución
- ✅ NPM scripts

---

## 🎯 MATRIZ DE COBERTURA

| Módulo | Flujos | Tests | % Cobertura |
|--------|--------|-------|------------|
| **Auth** | 3 | 4 | 100% |
| **Prendas** | 2 | 5 | 80% |
| **Ventas** | 2 | 4 | 100% |
| **Configuración** | 2 | 6 | 90% |
| **Usuarios** | 1 | 3 | 100% |
| **Validaciones** | 1 | 2 | 100% |
| **TOTAL** | **11** | **24** | **95%** |

---

## 📊 MÉTRICAS ESPERADAS

**Después de implementar los tests:**

```
✅ Casos de prueba: 24 tests E2E
✅ Cobertura de flujos: 95%
✅ Navegadores: 3 (Chromium, Firefox, WebKit)
✅ Tiempo de ejecución: ~5-10 minutos
✅ Tasa de éxito esperada: 100%
```

---

## 🎬 PLAN DEL VIDEO

### Duración Total: 30-40 minutos

#### Sección 1: Introducción (2-3 min)
- Presentación del proyecto
- Objetivo del testing
- Herramientas y tecnologías

#### Sección 2: Setup y Configuración (5-7 min)
- Instalación de Playwright
- Configuración inicial
- Estructura de carpetas
- Variables de entorno

#### Sección 3: Ejecución de Tests (15-20 min)
- ✅ Test Login (Admin)
- ✅ Test Login (Vendedor)
- ✅ Test Logout
- ✅ Test Crear Prenda
- ✅ Test Crear Venta
- ✅ Test Cambiar Tema
- ✅ Test Crear Usuario

#### Sección 4: Análisis de Resultados (5-8 min)
- Reportes generados
- Screenshots de evidencia
- Trazas de ejecución
- Métricas de cobertura

#### Sección 5: Conclusiones (2-3 min)
- Resumen de hallazgos
- Recomendaciones
- Próximos pasos

---

## 📋 CHECKLIST PRE-VIDEO

- [ ] Proyecto compilado sin errores
- [ ] 285 tests unitarios pasando
- [ ] Playwright instalado en máquina
- [ ] Navegadores descargados
- [ ] Config playwright.config.ts validada
- [ ] Tests ejemplares creados
- [ ] Base de datos limpia/preparada
- [ ] Credenciales de prueba activas
- [ ] OBS/Camtasia descargado
- [ ] Resolución de pantalla configurada (1920x1080)

---

## 🚀 PRÓXIMOS PASOS

### Phase 1: Setup (1-2 horas)
1. ✅ Crear carpeta fashionstore-e2e
2. ✅ Instalar Playwright
3. ✅ Configurar playwright.config.ts
4. ✅ Crear estructura de carpetas
5. ✅ Preparar .env.test

### Phase 2: Implementación (4-6 horas)
1. ⏳ Implementar tests de ejemplo
2. ⏳ Ejecutar y validar
3. ⏳ Ajustar selectores si es necesario
4. ⏳ Agregar más tests según casos

### Phase 3: Documentación + Video (3-4 horas)
1. ⏳ Finalizar documentación
2. ⏳ Grabar video de ejecución
3. ⏳ Editar y comentar video
4. ⏳ Generar reportes finales

---

## 📌 NOTAS IMPORTANTES

### Para Login
- Admin: `admin@fashionstore.com` / `Password123!`
- Vendedor: Se crea durante pruebas
- No reutilizar credenciales entre ejecuciones

### Para Crear Prenda
- Necesita categoría existente (ID ≥ 1)
- Precio debe ser > 0
- Stock debe ser ≥ 0
- Nombre debe ser único

### Para Crear Venta
- **IMPORTANTE**: Vendedor es READ-ONLY (asignado automáticamente)
- Necesita cliente existente
- Necesita al menos un producto en carrito
- Necesita método de pago

### Para Tema
- Se guarda en localStorage
- También se sincroniza a BD
- Persiste tras logout/login

---

## 📄 DOCUMENTOS RELACIONADOS

| Documento | Líneas | Propósito |
|-----------|--------|----------|
| DETALLES_PROYECTO_E2E_TESTING.md | 390 | Información completa del proyecto |
| PLAYWRIGHT_SETUP_INICIAL.md | 450 | Setup e implementación |
| RESUMEN_E2E_TESTING_PLAN.md | Este | Resumen ejecutivo |

---

## ✅ STATUS FINAL

**Documentación:** ✅ COMPLETA  
**Setup:** ✅ LISTO PARA IMPLEMENTAR  
**Ejemplos:** ✅ PROVISTOS  
**Video:** ⏳ PENDIENTE (próximo paso)

---

**Generado:** 2025-07-08  
**Autor:** Equipo QA  
**Versión:** 1.0  
**Estado:** 📋 LISTO PARA TESTING

---

## 🎯 PRÓXIMA ACCIÓN

1. Leer: `DETALLES_PROYECTO_E2E_TESTING.md`
2. Seguir: `PLAYWRIGHT_SETUP_INICIAL.md`
3. Implementar: Tests según ejemplos
4. Ejecutar: `npm run test:e2e`
5. Grabar: Video de ejecución

¡Éxito en las pruebas! 🚀
