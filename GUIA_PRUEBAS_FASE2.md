# 🧪 GUÍA DE PRUEBAS - FASE 2: AUTENTICACIÓN PROFESIONAL

## Requisitos Previos

✅ Solución compilada correctamente
✅ Base de datos actualizada (si hay cambios de migración)
✅ Visual Studio 2026 Community abierto
✅ Usuario de prueba creado en Identity DB

---

## 🔍 Caso de Prueba 1: Usuario NO Autenticado - Acceso a Raíz

### Pasos:
1. Abrir navegador incógnito / privado (sin sesión)
2. Acceder a `https://localhost:7000/` (o puerto de tu app)

### Resultado Esperado:
```
✅ Redirección 302 a /Identity/Account/Login
✅ Muestra Login moderno con:
   - Logo (shopping-bag icon)
   - "FashionStore"
   - "Gestión de Tienda de Ropa"
   - Campos Email y Contraseña
   - Checkbox "Recordar sesión"
   - Link "¿Olvidó su contraseña?"
   - Botón "Iniciar Sesión"
```

### Pantalla Esperada:
```
┌─────────────────────────────────────┐
│     FASHIONSTORE (púrpura)          │
│                                     │
│     🛍️ FashionStore                │
│  Gestión de Tienda de Ropa          │
│                                     │
│  Email: [____________________]      │
│  Contraseña: [_____________]        │
│  ☑ Recordar sesión                  │
│              [Iniciar Sesión]       │
│  ¿Olvidó su contraseña?             │
│  ¿No tiene cuenta? Registrarse      │
└─────────────────────────────────────┘
```

---

## 🔍 Caso de Prueba 2: Acceso a /Home/Index sin Autenticar

### Pasos:
1. Navegador incógnito
2. Acceder a `https://localhost:7000/Home/Index`

### Resultado Esperado:
```
✅ Redirección 302 a /Identity/Account/Login
✅ ReturnUrl contiene: ?returnUrl=%2FHome%2FIndex
✅ Después de login, vuelve a /Home/Index (si está en ReturnUrl)
```

---

## 🔍 Caso de Prueba 3: Login Exitoso

### Pasos:
1. Estar en página de Login
2. Ingresar credenciales válidas:
   - Email: `admin@fashionstore.com` (o usuario válido)
   - Contraseña: `[tu_contraseña_valida]`
3. Click en "Iniciar Sesión"

### Resultado Esperado:
```
✅ Validación en cliente (sin errores)
✅ POST a /Identity/Account/Login
✅ SignInManager autentica
✅ Cookie creada
✅ Redirección 302 a /Home/Index
✅ Muestra Dashboard con:
   - Total Categorías
   - Total Prendas
   - Total Clientes
   - Total Ventas
   - Total Usuarios
   - Gráfico de ventas últimos 6 meses
```

---

## 🔍 Caso de Prueba 4: Usuario Autenticado Accede a Login

### Pasos:
1. Usuario autenticado (sesión activa)
2. Acceder directamente a `https://localhost:7000/Identity/Account/Login`

### Resultado Esperado:
```
✅ OnGetAsync detecta IsSignedIn(User) = true
✅ Redirección 302 a /Home/Index
✅ NO muestra Login page
✅ Va directo al Dashboard
```

---

## 🔍 Caso de Prueba 5: Usuario Autenticado Accede a Raíz

### Pasos:
1. Usuario autenticado (sesión activa)
2. Acceder a `https://localhost:7000/`

### Resultado Esperado:
```
✅ Middleware valida
✅ User.IsAuthenticated = true
✅ Redirección 302 a /Home/Index
✅ Muestra Dashboard
```

---

## 🔍 Caso de Prueba 6: Recordar Sesión

### Pasos:
1. En Login, marcar checkbox "Recordar sesión"
2. Ingresar credenciales y hacer login
3. Cerrar navegador completamente
4. Abrir navegador nuevamente
5. Acceder a `https://localhost:7000/Home/Index`

### Resultado Esperado:
```
✅ Sesión se mantiene por 30 días
✅ Cookie SlidingExpiration renovada
✅ NO redirige a Login
✅ Muestra Dashboard
```

---

## 🔍 Caso de Prueba 7: Sin Marcar "Recordar Sesión"

### Pasos:
1. En Login, NO marcar checkbox
2. Ingresar credenciales y hacer login
3. Cerrar navegador completamente
4. Abrir navegador nuevamente
5. Acceder a `https://localhost:7000/`

### Resultado Esperado:
```
✅ Sesión expira al cerrar navegador (cookies de sesión)
✅ Redirige a /Identity/Account/Login
```

---

## 🔍 Caso de Prueba 8: Recuperar Contraseña

### Pasos:
1. En Login, click en "¿Olvidó su contraseña?"
2. Debería ir a /Identity/Account/ForgotPassword

### Resultado Esperado:
```
✅ Navega a ForgotPassword.cshtml
✅ Muestra formulario para ingresar email
✅ (Si email service configurado) Envía link de reseteo
```

---

## 🔍 Caso de Prueba 9: Credenciales Inválidas

### Pasos:
1. En Login, ingresar email incorrecto o contraseña incorrecta
2. Click en "Iniciar Sesión"

### Resultado Esperado:
```
✅ ModelState.IsValid = false O result.Succeeded = false
✅ Muestra mensaje de error:
   "Invalid login attempt."
✅ NO redirige
✅ Permanece en Login page
✅ Campos se limpian (excepto email si se pide)
✅ Toastr muestra error en rojo
```

---

## 🔍 Caso de Prueba 10: Email Vacío

### Pasos:
1. En Login, dejar Email vacío
2. Click en "Iniciar Sesión"

### Resultado Esperado:
```
✅ Validación HTML5 o server-side
✅ Mensaje: "Campo requerido"
✅ NO envía POST
```

---

## 🔍 Caso de Prueba 11: Email Inválido

### Pasos:
1. En Login, ingresar "invalidemail" (sin @)
2. Click en "Iniciar Sesión"

### Resultado Esperado:
```
✅ Validación server-side ([EmailAddress])
✅ Muestra error de formato
✅ NO redirige
```

---

## 🔍 Caso de Prueba 12: Logout / Sign Out

### Pasos:
1. Usuario autenticado
2. Click en logout (generalmente en navbar)
3. Acceder a `https://localhost:7000/`

### Resultado Esperado:
```
✅ Sesión destruida
✅ Cookie eliminada
✅ Redirección a /Identity/Account/Login
```

---

## 🔍 Caso de Prueba 13: Responsive Design - Móvil

### Pasos:
1. Abrir DevTools (F12)
2. Device Toolbar → iPhone 12
3. Acceder a Login

### Resultado Esperado:
```
✅ Login se adapta a pantalla móvil
✅ Campos tienen tamaño adecuado
✅ Botón es clickeable
✅ No hay overflow horizontal
✅ Fuentes legibles
✅ Espaciado apropiado
```

---

## 🔍 Caso de Prueba 14: Validación en Cliente

### Pasos:
1. Abrir DevTools → Console
2. Desactivar JavaScript (simular)
3. O dejar campos vacíos y submit

### Resultado Esperado:
```
✅ Validación server-side sigue funcionando
✅ Si JS desactivado, se valida en servidor
✅ Mensajes de error apropiados
```

---

## 🔍 Caso de Prueba 15: Acceso a Rutas Restringidas

### Pasos:
1. Sin autenticar, intentar acceder a:
   - /Categorias
   - /Prendas
   - /Ventas
   - etc.

### Resultado Esperado:
```
✅ Redirige a Login (si tienen [Authorize])
O
✅ Muestra AccessDenied page (si [Authorize] y restricción de rol)
```

---

## 📊 Checklist de Validación

```
[ ] ✅ Login page muestra correctamente
[ ] ✅ Redirección "/" → Login funciona
[ ] ✅ Redirección "/Home/Index" → Login funciona
[ ] ✅ Login exitoso → Dashboard funciona
[ ] ✅ Usuario autenticado en Login → Dashboard funciona
[ ] ✅ Usuario autenticado en "/" → Dashboard funciona
[ ] ✅ Recordar sesión funciona
[ ] ✅ Recuperar contraseña link funciona
[ ] ✅ Credenciales inválidas muestran error
[ ] ✅ Logout funciona correctamente
[ ] ✅ Mobile responsive funciona
[ ] ✅ Validación cliente/servidor funciona
[ ] ✅ AccessDenied page funciona
[ ] ✅ Cookies configuradas correctamente
[ ] ✅ Compilación correcta
```

---

## 🐛 Problemas Comunes y Soluciones

### Problema: Redirección infinita a Login
```
Causa: Middleware en "/" + otro middleware en "/Identity/Account/Login"
Solución: Verificar que middleware solo actúe en "/" y "/index.html"
```

### Problema: CSS no carga en Login
```
Causa: Layout = null sin rutas correctas a CDN
Solución: Verificar URLs de Bootstrap y FontAwesome
```

### Problema: Recordar sesión no funciona
```
Causa: Input.RememberMe no se vincula
Solución: Verificar asp-for="Input.RememberMe" en HTML
```

### Problema: Page Model no ve cambios
```
Causa: Cache del navegador
Solución: Ctrl+F5 o Clear browser cache
```

---

## 🎯 Criterios de Aceptación

✅ **Funcional:**
- La app NUNCA muestra Home sin autenticación
- Login page es moderno y profesional
- Redirecciones funcionan correctamente

✅ **Seguridad:**
- Múltiples niveles de validación
- ASP.NET Identity funcional
- Cookies seguras

✅ **Experiencia:**
- UI responsivo
- Mensajes claros
- Navegación intuitiva

✅ **Técnico:**
- Compila sin errores
- Sin warnings críticos
- Performance aceptable

---

## 📋 Ejecutar Todas las Pruebas

Tiempo estimado: **30 minutos**

1. Caso 1 (1 min)
2. Caso 2 (1 min)
3. Caso 3 (3 min)
4. Caso 4 (1 min)
5. Caso 5 (1 min)
6. Caso 6 (5 min)
7. Caso 7 (5 min)
8. Caso 8 (1 min)
9. Caso 9 (2 min)
10. Caso 10 (1 min)
11. Caso 11 (1 min)
12. Caso 12 (2 min)
13. Caso 13 (3 min)
14. Caso 14 (2 min)
15. Caso 15 (2 min)

**Total: ~30 minutos**

---

## 📝 Reporte de Pruebas

Usar este formato:

```
Caso 1: Usuario NO Autenticado - Acceso a Raíz
Status: ✅ PASS / ❌ FAIL / ⚠️ WARNING
Notas: [descripción]

Caso 2: Acceso a /Home/Index sin Autenticar
Status: ✅ PASS / ❌ FAIL / ⚠️ WARNING
Notas: [descripción]

...

RESUMEN FINAL: ✅ TODAS LAS PRUEBAS PASARON
```

---

**Guía de Pruebas - Fase 2 Completada**
**Fecha: 2025-01-15**
