# 🚀 INSTRUCCIONES: EJECUTA LA PRUEBA AHORA

## ✅ APLICACIÓN LISTA

**La aplicación está ejecutándose en:**
```
http://localhost:5100
```

**Puerto:** 5100  
**Ambiente:** Development  
**Base de Datos:** PostgreSQL (Supabase)  
**Status:** ✅ Escuchando peticiones  

---

## 📝 FLUJO A EJECUTAR (En tu navegador)

### 1️⃣ PASO 1: ADMIN LOGIN
```
URL: http://localhost:5100
Usuario: admin@fashionstore.com
Contraseña: Password123!
```
✅ Clic en "Login"

---

### 2️⃣ PASO 2: IR A VENDEDORES
```
Navbar → Admin → Vendedores
```
✅ Deberías ver tabla con 6 vendedores

---

### 3️⃣ PASO 3: EDITAR ANA USUARIO
```
Busca fila: "Ana Usuario" (DNI: 82030000)
Clic en icono: ✏️ (Edit)
```
✅ Se abre formulario

---

### 4️⃣ PASO 4: CAMBIAR CONTRASEÑA DE ANA
```
Campo: "Nueva Contraseña"
Ingresa: AnaVendedor123!
Clic: "Cambiar Contraseña"
```
✅ Mensaje: "✓ Contraseña actualizada correctamente para Ana Usuario"

---

### 5️⃣ PASO 5: EDITAR SOFIA RUIZ
```
Navegar nuevamente a: Vendedores
Busca fila: "Sofia Ruiz" (DNI: 83807768)
Clic en: ✏️ (Edit)
```
✅ Se abre formulario de Sofia

---

### 6️⃣ PASO 6: CAMBIAR CONTRASEÑA DE SOFIA
```
Campo: "Nueva Contraseña"
Ingresa: SofiaVendedor456!
Clic: "Cambiar Contraseña"
```
✅ Mensaje: "✓ Contraseña actualizada correctamente para Sofia Ruiz"

---

### 7️⃣ PASO 7: LOGOUT ADMIN
```
Esquina superior derecha → Nombre de usuario
Clic: "Logout"
```
✅ Redirige a Login

---

### 8️⃣ PASO 8: LOGIN COMO ANA
```
URL: http://localhost:5100/Identity/Account/Login
Usuario: ana@fashionstore.com
Contraseña: AnaVendedor123!
```
✅ Login exitoso

---

### 9️⃣ PASO 9: VERIFICAR ACCESO LIMITADO
```
☑️ Debería VER:
  - Dashboard / Inicio
  - Ventas
  - Cambiar contraseña personal

☒ NO debería VER:
  - Menu "Admin"
  - Link a "Vendedores"
  - Link a "Descuentos"
```

---

### 🔟 PASO 10: VER VENTAS DE ANA
```
Navbar → Ventas
```
✅ Solo ve sus propias ventas (filtradas por VendedorId = 1)

---

### 1️⃣1️⃣ PASO 11: INTENTAR ACCEDER A VENDEDORES
```
URL directa: http://localhost:5100/Vendedores
```
❌ Esperado: **Acceso Denegado** (AccessDenied 403)

---

### 1️⃣2️⃣ PASO 12: LOGOUT ANA
```
Clic: Logout
```

---

### 1️⃣3️⃣ PASO 13: LOGIN COMO SOFIA
```
Usuario: sofia@fashionstore.com
Contraseña: SofiaVendedor456!
```
✅ Login exitoso

---

### 1️⃣4️⃣ PASO 14: VERIFICAR SOFIA
```
☑️ Ve Dashboard y Ventas
☒ NO ve menu Admin
☒ NO puede acceder a /Vendedores (403)
✅ Solo ve sus propias ventas
```

---

### 1️⃣5️⃣ PASO 15: LOGOUT SOFIA
```
Clic: Logout
```

---

### 1️⃣6️⃣ PASO 16: ADMIN CAMBIA CONTRASEÑA DE ANA (Nueva prueba)
```
Login: admin@fashionstore.com / Password123!
Vendedores → Edit Ana Usuario
Nueva Contraseña: AnaVendedor789!
Clic: "Cambiar Contraseña"
```

---

### 1️⃣7️⃣ PASO 17: VERIFY ANA USA CONTRASEÑA ANTIGUA (Debe fallar)
```
Logout Admin
Login: ana@fashionstore.com / AnaVendedor123!
```
❌ **Esperado: Login Falla** (contraseña antigua no funciona)

---

### 1️⃣8️⃣ PASO 18: ANA LOGIN CON CONTRASEÑA NUEVA
```
Login: ana@fashionstore.com / AnaVendedor789!
```
✅ **Esperado: Login Exitoso** (contraseña nueva funciona)

---

## ✅ CHECKLIST DE CUMPLIMIENTO

```
ADMINISTRADOR PUEDE:
[✅] Ver todos los vendedores
[✅] Editar vendedor
[✅] Cambiar contraseña de vendedor
[✅] Ver todas las ventas
[✅] Acceder a Admin panel

VENDEDOR PUEDE:
[✅] Iniciar sesión con contraseña asignada
[✅] Ver dashboard
[✅] Ver sus propias ventas
[✅] Cambiar su propia contraseña

VENDEDOR NO PUEDE:
[❌] Ver página de Vendedores
[❌] Editar otro vendedor
[❌] Ver ventas de otros vendedores
[❌] Acceder a Admin panel
[❌] Ver descuentos (si requiere Admin)

SEGURIDAD:
[✅] Contraseña antigua no funciona después del cambio
[✅] Contraseña nueva funciona
[✅] AccessDenied (403) en accesos no autorizados
[✅] Sin exposición de contraseña plaintext
[✅] Logs muestran operaciones
```

---

## 📊 RESULTADO ESPERADO

### Si TODO funciona correctamente:

```
✅ SISTEMA DE AUTENTICACIÓN
   - Admin login: EXITOSO
   - Vendedor login: EXITOSO
   - Logout: EXITOSO

✅ AUTORIZACIÓN POR ROLES
   - Admin ve Vendedores: SÍ
   - Vendedor ve Vendedores: NO (403)
   - Admin ve todas las ventas: SÍ
   - Vendedor ve solo sus ventas: SÍ

✅ GESTIÓN DE CONTRASEÑAS
   - Admin cambia contraseña: FUNCIONA
   - Contraseña antigua se invalida: FUNCIONA
   - Contraseña nueva permite login: FUNCIONA

✅ FILTRADO DE DATOS
   - Vendedor solo ve sus ventas: FUNCIONA
   - Admin ve todas: FUNCIONA

🎯 RESULTADO FINAL: LISTO PARA PRODUCCIÓN ✅
```

---

## 🆘 PROBLEMAS COMUNES

### ❌ "Login rechazado aunque contraseña es correcta"
```
Solución:
1. Verificar que ambiente está en Debug
2. Esperar que DbInitializer cree los usuarios
3. Refrescar página (F5)
4. Limpiar cookies del navegador
```

### ❌ "No ve página de cambiar contraseña"
```
Solución:
1. Verificar que está logueado como Admin
2. Verificar que vendedor tiene email (Correo)
3. Verificar en BD que usuario existe
```

### ❌ "AccessDenied incluso siendo Admin"
```
Solución:
1. Verificar que Admin está en rol "Administrador"
2. Verificar en BD: AspNetUserRoles
3. Reiniciar aplicación
```

### ❌ "Vendedor ve página de Vendedores"
```
Solución:
1. Verificar [Authorize(Roles="Administrador")] en controlador
2. Compilar: dotnet build
3. Reiniciar aplicación
```

---

## 🔧 COMANDOS ÚTILES

### Ver logs en tiempo real
```powershell
# Abre panel de logs en DevTools del navegador
F12 → Console
```

### Ver estado de la BD
```bash
# Ejecutar en Supabase SQL Editor:
SELECT COUNT(*) as total_usuarios FROM "AspNetUsers";
SELECT COUNT(*) as total_vendedores FROM "Vendedor";
SELECT COUNT(*) as total_ventas FROM "Venta";
```

### Reiniciar aplicación
```bash
# En la terminal donde corre: Ctrl+C
# Luego: dotnet run --project FashionStore.Web
```

---

## 📞 SOPORTE

**Preguntas o problemas:** Revisa `PLAN_CORRECCION_TECNICA_V2.md` para análisis técnico completo.

**Documentos disponibles:**
- `PRUEBA_FLUJO_ADMIN_VENDEDORES.md` (Detallado paso a paso)
- `CASOS_PRUEBA_VALIDACION.md` (10 casos completos)
- `QUICK_REFERENCE_CORRECCIONES.md` (Correcciones técnicas)

---

## 🎯 CONCLUSIÓN

Si completaste TODOS los 18 pasos y TODO funciona:

**✅ EL SISTEMA ESTÁ FUNCIONANDO CORRECTAMENTE**

El flujo de:
- Admin asigna contraseña ✅
- Vendedor inicia sesión ✅
- Vendedor solo ve sus datos ✅
- Solo Admin puede cambiar contraseñas ✅

**ESTÁ VERIFICADO Y OPERACIONAL** 🚀

---

**Generado:** 13 Julio 2026  
**Para:** Usuario Final  
**Duración estimada:** 15-20 minutos

¡Éxito! 🎉

