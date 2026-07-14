# FLUJO DE PRUEBA: 2 VENDEDORES CON ACCESO INDEPENDIENTE
## Verificación: Solo Admin puede asignar/cambiar contraseñas

**Servidor:** http://localhost:5100  
**Fecha Prueba:** 13 Julio 2026  
**Estado:** LISTO PARA EJECUCIÓN  

---

## OBJETIVO

Verificar que:
1. ✅ Admin asigna contraseña a **Vendedor 1** con su correo
2. ✅ Admin asigna contraseña a **Vendedor 2** con su correo
3. ✅ Vendedor 1 inicia sesión y **SOLO VE** al Administrador (no otros vendedores)
4. ✅ Vendedor 2 inicia sesión y **SOLO VE** al Administrador (no otros vendedores)
5. ✅ **SOLO ADMIN** puede cambiar contraseña de vendedores (Vendedor NO puede auto-cambiar)
6. ✅ Admin cambia contraseña de ambos vendedores exitosamente

---

## PASO 1: ADMIN ACCEDE

**URL:** http://localhost:5100/Identity/Account/Login

**Credenciales:**
```
Email: admin@fashionstore.com
Password: Password123!
```

**ACCIÓN EN NAVEGADOR:**
```
1. Abre login page
2. Ingresa email: admin@fashionstore.com
3. Ingresa password: Password123!
4. Click "Iniciar sesión"
```

**RESULTADO ESPERADO:**
```
✓ Redirige a http://localhost:5100/Home/Index (Dashboard)
✓ Navbar muestra: "admin@fashionstore.com"
✓ Badge o indicador: "Administrador"
✓ Menu Admin visible (Vendedores, Reportes, Descuentos, etc)
```

**✓ PUNTO DE CONTROL 1: Admin autenticado**

---

## PASO 2: ADMIN NAVEGA A VENDEDORES

**ACCIÓN EN NAVEGADOR:**
```
1. Click Navbar → Admin → Vendedores
```

**RESULTADO ESPERADO:**
```
✓ URL: http://localhost:5100/Vendedores
✓ Tabla con 6 vendedores visible:

  # | Nombres Completos | DNI | Teléfono | Correo | Estado | Acciones
  1 | Ana Usuario | 82030000 | - | ana@fashionstore.com | ✓ Activo | [Editar] [...]
  2 | Ana Vallejo | 83051544 | 555-7000 | ana.vallejo@fashionstore.com | ✓ Activo | [Editar] [...]
  3 | Carlos Mendoza | 92031122 | 555-2001 | carlos.mendoza@fashionstore.com | ✓ Activo | [Editar] [...]
  4 | Diego Morena | 93880000 | 555-2005 | diego.morena@fashionstore.com | ✓ Activo | [Editar] [...]
  5 | Luis Castro | 92908566 | 555-2003 | luis.castro@fashionstore.com | ✓ Activo | [Editar] [...]
  6 | Sofia Ruiz | 83807768 | 555-2004 | sofia.ruiz@fashionstore.com | ✓ Activo | [Editar] [...]
```

**✓ PUNTO DE CONTROL 2: Lista de vendedores cargada**

---

## PASO 3: ADMIN EDITA VENDEDOR 1 (ANA USUARIO)

**ACCIÓN EN NAVEGADOR:**
```
1. Click en icono lápiz (Edit) en fila de "Ana Usuario"
```

**RESULTADO ESPERADO:**
```
✓ URL: http://localhost:5100/Vendedores/Edit/1
✓ Formulario abierto con datos pre-llena:
  - Nombres: "Ana"
  - Apellidos: "Usuario"
  - DNI: "82030000"
  - Teléfono: (vacío)
  - Correo: "ana@fashionstore.com"
  - Estado: Toggle ON (verde)

✓ AL FONDO: Panel amarillo
  TITULO: "Cambiar Contraseña de Acceso"
  TEXTO: "Como administrador, puedes asignar una nueva contraseña a este vendedor..."
  
  CAMPO: "Nueva Contraseña" (type=password)
  BOTÓN: Eye icon (para mostrar/ocultar)
  BOTÓN: "Actualizar Contraseña" (amarillo, grande)
```

**NOTA:** Este panel SOLO aparece si estás logueado como Admin.

**✓ PUNTO DE CONTROL 3: Panel de cambio de contraseña visible**

---

## PASO 4: ADMIN ASIGNA CONTRASEÑA A ANA USUARIO

**ACCIÓN EN NAVEGADOR:**
```
1. En campo "Nueva Contraseña" ingresa: Ana@Password123!
2. Click botón "Actualizar Contraseña" (amarillo)
3. Espera respuesta del servidor
```

**RESULTADO ESPERADO:**
```
✓ Página redirige a http://localhost:5100/Vendedores
✓ Mensaje VERDE aparece (alert success):
  "✓ Contraseña actualizada correctamente para Ana Usuario."
✓ Mensaje desaparece después de 5 segundos (auto-dismiss)
✓ Tabla de vendedores intacta

VALIDACIÓN EN BD:
✓ ApplicationUser(email=ana@fashionstore.com).PasswordHash actualizado
✓ Vendedor.UltimaPasswordAdmin = "Ana@Password123!"
```

**✓ PUNTO DE CONTROL 4: Contraseña de Vendedor 1 asignada**

---

## PASO 5: ADMIN EDITA VENDEDOR 2 (CARLOS MENDOZA)

**ACCIÓN EN NAVEGADOR:**
```
1. Click en icono lápiz (Edit) en fila de "Carlos Mendoza"
```

**RESULTADO ESPERADO:**
```
✓ URL: http://localhost:5100/Vendedores/Edit/3
✓ Formulario abierto con datos pre-llena:
  - Nombres: "Carlos"
  - Apellidos: "Mendoza"
  - DNI: "92031122"
  - Correo: "carlos.mendoza@fashionstore.com"
  - Estado: Toggle ON (verde)

✓ Panel amarillo visible (Cambiar Contraseña)
```

**✓ PUNTO DE CONTROL 5: Editar Vendedor 2 exitoso**

---

## PASO 6: ADMIN ASIGNA CONTRASEÑA A CARLOS MENDOZA

**ACCIÓN EN NAVEGADOR:**
```
1. En campo "Nueva Contraseña" ingresa: Carlos@Password123!
2. Click botón "Actualizar Contraseña" (amarillo)
3. Espera respuesta
```

**RESULTADO ESPERADO:**
```
✓ Página redirige a http://localhost:5100/Vendedores
✓ Mensaje VERDE aparece:
  "✓ Contraseña actualizada correctamente para Carlos Mendoza."
✓ Tabla de vendedores intacta

VALIDACIÓN EN BD:
✓ ApplicationUser(email=carlos.mendoza@fashionstore.com).PasswordHash actualizado
✓ Vendedor.UltimaPasswordAdmin = "Carlos@Password123!"
```

**✓ PUNTO DE CONTROL 6: Contraseña de Vendedor 2 asignada**

---

## PASO 7: ADMIN CIERRA SESIÓN

**ACCIÓN EN NAVEGADOR:**
```
1. Click en usuario (top-right) "admin@fashionstore.com"
2. Click "Logout" / "Cerrar Sesión"
```

**RESULTADO ESPERADO:**
```
✓ Redirige a http://localhost:5100/Identity/Account/Login
✓ Login form visible
✓ Campos email y password vacíos
✓ Botón "Iniciar sesión"
✓ Ningún usuario autenticado
```

**✓ PUNTO DE CONTROL 7: Admin desautenticado**

---

## PASO 8: VENDEDOR 1 INICIA SESIÓN (ANA USUARIO)

**ACCIÓN EN NAVEGADOR:**
```
1. Email: ana@fashionstore.com
2. Password: Ana@Password123!
3. Click "Iniciar sesión"
```

**RESULTADO ESPERADO:**
```
✓ Redirige a http://localhost:5100/Home/Index (Dashboard)
✓ Dashboard carga exitosamente
✓ Navbar muestra: "ana@fashionstore.com"
✓ Rol indicado: "Vendedor"
```

**✓ PUNTO DE CONTROL 8: Ana Usuario autenticado**

---

## PASO 9: VERIFICAR ANA SOLO VE ADMINISTRADOR EN DROPDOWN (VENTAS)

**ACCIÓN EN NAVEGADOR:**
```
1. Click Navbar → Ventas → Nueva Venta
2. Observa el formulario
```

**RESULTADO ESPERADO:**
```
✓ URL: http://localhost:5100/Ventas/Create
✓ Formulario tiene campo "Vendedor" (dropdown)
✓ Campo muestra: "Ana Usuario" (detectado de sesión)
✓ Indicador: "Detectado de tu sesión"

VALIDACIÓN - IMPORTANTE:
✓ SOLO debe haber 2 opciones en dropdown:
  1. "Administrador" (rol: Administrador, DNI: ADMIN0001)
  2. "Ana Usuario" (ya seleccionado)

✗ NO debe haber:
  - Carlos Mendoza
  - Diego Morena
  - Luis Castro
  - Sofia Ruiz
  
RAZÓN: Ana es Vendedor, solo ve Admin + a sí mismo
```

**⚠️ SI NO CUMPLE:** Hay problema en lógica de filtrado de Vendedores

**✓ PUNTO DE CONTROL 9: Ana solo ve Admin + ella misma**

---

## PASO 10: ANA INTENTA CAMBIAR SU PROPIA CONTRASEÑA (DEBE FALLAR)

**ACCIÓN EN NAVEGADOR:**
```
1. Barra de dirección: http://localhost:5100/Vendedores/Edit/1
2. Press Enter (intenta acceder a su propio perfil)
```

**RESULTADO ESPERADO:**
```
✓ OPCIÓN A - Acceso Denegado:
  - Redirige a http://localhost:5100/Identity/Account/AccessDenied
  - Mensaje: "Acceso denegado" / "Access Denied"
  
✓ OPCIÓN B - No muestra panel de contraseña:
  - Abre página Edit/1
  - Formulario visible PERO
  - Panel "Cambiar Contraseña" NO visible
  - Solo ve sus datos (Nombres, Apellidos, etc)
  - NO hay campo "Nueva Contraseña"

RAZÓN CORRECTA: Solo [Authorize(Roles = "Administrador")] puede cambiar
```

**✓ PUNTO DE CONTROL 10: Vendedor NO puede cambiar contraseña**

---

## PASO 11: ANA CIERRA SESIÓN

**ACCIÓN EN NAVEGADOR:**
```
1. Click usuario (top-right) "ana@fashionstore.com"
2. Click "Logout"
```

**RESULTADO ESPERADO:**
```
✓ Redirige a login
✓ Ana desautenticada
```

**✓ PUNTO DE CONTROL 11: Ana desautenticada**

---

## PASO 12: VENDEDOR 2 INICIA SESIÓN (CARLOS MENDOZA)

**ACCIÓN EN NAVEGADOR:**
```
1. Email: carlos.mendoza@fashionstore.com
2. Password: Carlos@Password123!
3. Click "Iniciar sesión"
```

**RESULTADO ESPERADO:**
```
✓ Redirige a Dashboard
✓ Navbar muestra: "carlos.mendoza@fashionstore.com"
✓ Rol indicado: "Vendedor"
```

**✓ PUNTO DE CONTROL 12: Carlos autenticado**

---

## PASO 13: VERIFICAR CARLOS SOLO VE ADMINISTRADOR EN DROPDOWN

**ACCIÓN EN NAVEGADOR:**
```
1. Navbar → Ventas → Nueva Venta
```

**RESULTADO ESPERADO:**
```
✓ Campo "Vendedor" muestra: "Carlos Mendoza" (detectado)
✓ Dropdown opciones:
  1. "Administrador"
  2. "Carlos Mendoza" (ya seleccionado)

✗ NO debe haber:
  - Ana Usuario
  - Diego Morena
  - Luis Castro
  - Sofia Ruiz

RAZÓN: Carlos es Vendedor, solo ve Admin + a sí mismo
```

**✓ PUNTO DE CONTROL 13: Carlos solo ve Admin + él mismo**

---

## PASO 14: CARLOS INTENTA CAMBIAR CONTRASEÑA (DEBE FALLAR)

**ACCIÓN EN NAVEGADOR:**
```
1. Intenta acceder: http://localhost:5100/Vendedores/Edit/3
```

**RESULTADO ESPERADO:**
```
✓ OPCIÓN A - Acceso Denegado:
  - AccessDenied page
  
✓ OPCIÓN B - Sin panel:
  - Edit abre pero no muestra "Cambiar Contraseña"
  - NO puede cambiar su contraseña
```

**✓ PUNTO DE CONTROL 14: Carlos NO puede cambiar contraseña**

---

## PASO 15: CARLOS CIERRA SESIÓN

**ACCIÓN EN NAVEGADOR:**
```
1. Click usuario → Logout
```

**RESULTADO ESPERADO:**
```
✓ Redirige a login
✓ Carlos desautenticado
```

**✓ PUNTO DE CONTROL 15: Carlos desautenticado**

---

## PASO 16: ADMIN INICIA SESIÓN DE NUEVO

**ACCIÓN EN NAVEGADOR:**
```
1. Email: admin@fashionstore.com
2. Password: Password123!
3. Click "Iniciar sesión"
```

**RESULTADO ESPERADO:**
```
✓ Dashboard carga
✓ Admin autenticado
```

**✓ PUNTO DE CONTROL 16: Admin re-autenticado**

---

## PASO 17: ADMIN CAMBIA CONTRASEÑA DE ANA

**ACCIÓN EN NAVEGADOR:**
```
1. Navbar → Admin → Vendedores
2. Click Edit "Ana Usuario"
3. En campo "Nueva Contraseña": AnaNewPass456!
4. Click "Actualizar Contraseña"
```

**RESULTADO ESPERADO:**
```
✓ Mensaje VERDE: "✓ Contraseña actualizada correctamente para Ana Usuario."
✓ Redirige a /Vendedores
✓ Tabla intacta

VALIDACIÓN BD:
✓ ApplicationUser(ana@fashionstore.com).PasswordHash actualizado
✓ Vendedor.UltimaPasswordAdmin = "AnaNewPass456!"
```

**✓ PUNTO DE CONTROL 17: Admin cambia contraseña de Ana exitosamente**

---

## PASO 18: ADMIN CAMBIA CONTRASEÑA DE CARLOS

**ACCIÓN EN NAVEGADOR:**
```
1. Navbar → Admin → Vendedores
2. Click Edit "Carlos Mendoza"
3. En campo "Nueva Contraseña": CarlosNewPass456!
4. Click "Actualizar Contraseña"
```

**RESULTADO ESPERADO:**
```
✓ Mensaje VERDE: "✓ Contraseña actualizada correctamente para Carlos Mendoza."
✓ Redirige a /Vendedores
✓ Tabla intacta

VALIDACIÓN BD:
✓ ApplicationUser(carlos.mendoza@fashionstore.com).PasswordHash actualizado
✓ Vendedor.UltimaPasswordAdmin = "CarlosNewPass456!"
```

**✓ PUNTO DE CONTROL 18: Admin cambia contraseña de Carlos exitosamente**

---

## PASO 19: ANA INICIA SESIÓN CON NUEVA CONTRASEÑA

**ACCIÓN EN NAVEGADOR:**
```
1. Admin Logout
2. Email: ana@fashionstore.com
3. Password: AnaNewPass456! (nueva)
4. Click "Iniciar sesión"
```

**RESULTADO ESPERADO:**
```
✓ Redirige a Dashboard
✓ Ana autenticado exitosamente
✓ Contraseña anterior (Ana@Password123!) ya NO funciona
```

**✓ PUNTO DE CONTROL 19: Ana login con nueva contraseña**

---

## PASO 20: CARLOS INICIA SESIÓN CON NUEVA CONTRASEÑA

**ACCIÓN EN NAVEGADOR:**
```
1. Ana Logout
2. Email: carlos.mendoza@fashionstore.com
3. Password: CarlosNewPass456! (nueva)
4. Click "Iniciar sesión"
```

**RESULTADO ESPERADO:**
```
✓ Redirige a Dashboard
✓ Carlos autenticado exitosamente
✓ Contraseña anterior (Carlos@Password123!) ya NO funciona
```

**✓ PUNTO DE CONTROL 20: Carlos login con nueva contraseña**

---

## RESUMEN DE RESULTADOS

Llenar después de ejecutar todas las pruebas:

```
TOTAL PUNTOS DE CONTROL: 20

✓ PASARON: ____ / 20
✗ FALLARON: ____ / 20

FLUJO VALIDADO:
[ ] Admin puede asignar contraseña a vendedores
[ ] Vendedor inicia sesión con contraseña asignada
[ ] Vendedor SOLO VE Administrador en dropdown
[ ] Vendedor NO PUEDE cambiar su propia contraseña
[ ] SOLO Admin puede cambiar contraseñas
[ ] Admin puede cambiar contraseña de múltiples vendedores
[ ] Vendedor puede iniciar sesión con nueva contraseña
[ ] Contraseña anterior invalida después de cambio
```

---

## PROBLEMAS ENCONTRADOS

```
Problema 1: ___________________________________
  Punto de Control: ____
  Descripción: _________________________________
  Acción requerida: ___________________________

Problema 2: ___________________________________
  Punto de Control: ____
  Descripción: _________________________________
  Acción requerida: ___________________________

(Agregar más si es necesario)
```

---

## CRITERIOS DE ÉXITO FINAL

✅ **FLUJO COMPLETO PASÓ SI:**
- [ ] 20/20 puntos de control pasaron
- [ ] Admin puede asignar contraseñas a múltiples vendedores
- [ ] Cada vendedor inicia sesión con su contraseña única
- [ ] Vendedores NO pueden auto-cambiar contraseña
- [ ] Vendedores SOLO VEN al Administrador (no otros vendedores)
- [ ] Admin puede cambiar contraseña de vendedores después de asignar
- [ ] Nuevas contraseñas funcionan inmediatamente

---

**Fecha Prueba:** ___________  
**Probador:** ___________  
**Resultado:** [ ] ÉXITO COMPLETO  [ ] FALLOS DETECTADOS  

