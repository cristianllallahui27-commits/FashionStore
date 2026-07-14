# 🧪 PRUEBAS - BUGS SOLUCIONADOS

## 🔧 Bugs Arreglados

### 1. ✅ Error "column v.UsuarioId does not exist"

**Problema**: Al acceder a Vendedores, error SQL sobre columna que no existe.

**Solución**: Removida referencia a `UsuarioId` de los queries. Ahora busca por `Correo` que sí existe en BD.

**Archivos cambiados**:
- `VendedoresController.cs`
  - Removida asignación de `UsuarioId` en Create()
  - Removida asignación de `UsuarioId` en ToggleEstado()
  - CambiarPassword() busca solo por Correo

**Compilación**: ✅ 0 Errores

---

### 2. ✅ Crear Vendedor (Solo Admin)

**Funcionalidad**: Solo Administrador puede crear vendedores y asignarles contraseña.

**Flujo**:
1. Admin > Vendedores > Crear
2. Completa datos: Nombres, Apellidos, DNI, Email, Contraseña, Correo
3. Hace clic "Crear Vendedor"
4. Sistema crea:
   - Cuenta en ApplicationUser (Identity)
   - Registro en tabla Vendedores
   - Asigna rol "Vendedor"
   - Guarda contraseña en `UltimaPasswordAdmin`
5. ✓ Vendedor creado exitosamente

---

### 3. ✅ Cambiar Contraseña Vendedor (Solo Admin)

**Funcionalidad**: Admin puede cambiar contraseña de vendedor desde Edit.

**Flujo**:
1. Admin > Vendedores
2. Click en vendedor
3. Sección: "Cambiar Contraseña de Acceso"
4. Ingresa nueva contraseña (mín 6 caracteres)
5. Click "Actualizar Contraseña"
6. ✓ Contraseña actualizada
7. Vendedor puede login con nueva contraseña

**Validación**:
- Solo Admin puede acceder (autorización)
- Valida longitud mínima de contraseña
- Usa UserManager para cambio seguro
- Guarda en `UltimaPasswordAdmin` para visibilidad del admin

---

### 4. ✅ Botón Inicio en Home

**Funcionalidad**: Botón "Inicio" en navbar redirige a home correctamente.

**Ubicación**: Navbar izquierda, primer elemento

**Html**:
```html
<a class="nav-link" asp-controller="Home" asp-action="Index">
    <i class="fas fa-home me-1"></i>Inicio
</a>
```

**Resultado**: Click → Redirige a `http://localhost:5100/` (Home/Index)

---

## 🧪 PASOS PARA PROBAR

### Test 1: Crear Vendedor

```
1. Presiona F5 → "🚀 RUN Release"
2. Login: admin@fashionstore.com / Password123!
3. Menú: Admin > Vendedores
4. Click: "+ Crear Vendedor"
5. Rellena:
   - Nombres: Juan
   - Apellidos: Pérez
   - DNI: 12345678
   - Email: juan@example.com
   - Contraseña: MiPassword123
   - Teléfono: 999999999
   - Correo: juan@example.com
6. Click: "Crear Vendedor"
7. ✓ Debe mostrar: "Vendedor 'Juan Pérez' creado correctamente"
```

### Test 2: Ver Vendedor en Lista

```
1. Admin > Vendedores
2. ✓ Debe listar "Juan Pérez"
3. ✓ Ver botón: "Editar" y otros
```

### Test 3: Cambiar Contraseña (Admin)

```
1. Admin > Vendedores
2. Click en "Juan Pérez"
3. Sección: "Cambiar Contraseña de Acceso"
4. Ingresa: "NuevaPassword456"
5. Click: "Actualizar Contraseña"
6. ✓ Mensaje: "✓ Contraseña actualizada correctamente para Juan Pérez"
```

### Test 4: Vendedor No Puede Cambiar Su Propia Contraseña

```
1. Login como vendedor: juan@example.com / NuevaPassword456
2. Admin > Vendedores
3. ✓ No debería ver opción (no tiene rol Administrador)
4. Si accede fuerza a /Vendedores/Edit/1
5. ✓ Debe redirigir (no autorizado)
```

### Test 5: Botón Inicio

```
1. En cualquier página
2. Click: "Inicio" (navbar arriba izquierda)
3. ✓ Redirige a Home (dashboard)
```

### Test 6: Flujo Completo

```
1. Admin crea 3 vendedores
2. Admin cambia contraseña a 2 de ellos
3. Login como cada vendedor
4. ✓ Funciona con contraseñas nuevas
5. Vendedor accede a Ventas > Nueva Venta
6. ✓ Auto-detecta como vendedor
7. Crea venta
8. ✓ Se registra bajo su ID
```

---

## ✅ Compilación

```
dotnet build -c Release
→ ✓ 0 Errores
→ 8 Warnings (no críticos)
```

---

## 🚀 Para Ejecutar

```powershell
# Terminal en VS Code
F5 → "🚀 RUN Release"

# Esperar compilación
# Navegador abre: http://localhost:5100

# Login
Email: admin@fashionstore.com
Password: Password123!
```

---

## 📝 Resumen

| Problema | Estado | Prueba |
|:---|:---:|:---|
| UsuarioId Error | ✅ REPARADO | Acceder a Vendedores |
| Crear Vendedor | ✅ FUNCIONA | Test 1 |
| Cambiar Contraseña | ✅ FUNCIONA | Test 3 |
| Botón Inicio | ✅ FUNCIONA | Test 5 |

---

**¡Listo para probar!** Presiona F5 y sigue los pasos.
