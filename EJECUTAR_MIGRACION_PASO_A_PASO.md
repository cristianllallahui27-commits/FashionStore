# 🚀 EJECUTAR MIGRACIÓN A SUPABASE - GUÍA PASO A PASO

**Status:** ✅ Código compilado  
**Fecha:** 7 de Julio, 2026  
**Duración:** ~3 minutos

---

## 📋 TAREAS A EJECUTAR

### ✅ #1 - ABRIR SUPABASE SQL EDITOR

**URL:**
```
https://supabase.com/dashboard/project/bajbvebkmacdnllnxvkv/sql/new
```

**Pasos:**
1. Abre el navegador
2. Pega la URL
3. Inicia sesión si es necesario
4. Verás editor SQL blanco

---

### ✅ #2 - COPIAR SCRIPT SQL COMPLETO

**Archivo a abrir:**
```
c:\Users\CRISTIAN\source\repos\FashionStoreSolution\Database\MIGRACION_COMPLETA_SUPABASE.sql
```

**Pasos:**
1. Abre con Notepad (Click derecho → Abrir con → Notepad)
2. Ctrl+A (seleccionar todo)
3. Ctrl+C (copiar)

**Qué contiene el script:**
- ✅ 8 tablas ASP.NET Identity (Users, Roles, UserRoles, UserClaims, etc.)
- ✅ Usuario Administrador de prueba (admin / Admin123!)
- ✅ 3 Roles (Administrador, Vendedor, Gerente)
- ✅ 10 tablas de negocio (Categorias, Prendas, Clientes, etc.)
- ✅ 5 Categorías
- ✅ 18 Prendas
- ✅ 10 Clientes
- ✅ 5 Vendedores
- ✅ 5 Métodos de Pago
- ✅ 2 Ventas de ejemplo con detalles
- ✅ Configuración del sistema
- ✅ Configuración de auditoría

**Total: ~80 sentencias SQL**

---

### ✅ #3 - PEGAR EN SUPABASE

**En el editor SQL (campo blanco):**
1. Click en el área blanca
2. Ctrl+V (pegar)

**Deberías ver:**
```sql
-- ==========================================
-- MIGRACIÓN COMPLETA: SQL Server → Supabase
-- ==========================================
...
```

---

### ✅ #4 - EJECUTAR SCRIPT

**Opciones:**
- Click botón **"Run"** (arriba derecha)
- O presiona **Ctrl+Enter**

**Espera a ver:**
```
✅ Query executed successfully
```

**Tiempo:** 20-40 segundos

---

### ✅ #5 - VERIFICAR TABLAS CREADAS

**En Supabase, click en "Table Editor" (lado izquierdo)**

Deberías ver estas tablas:

#### ASP.NET Identity (8 tablas)
- ✅ AspNetUsers (1 registro: admin)
- ✅ AspNetRoles (3 registros: Administrador, Vendedor, Gerente)
- ✅ AspNetUserRoles (1 registro: admin → Administrador)
- ✅ AspNetUserClaims
- ✅ AspNetUserLogins
- ✅ AspNetUserTokens
- ✅ AspNetRoleClaims
- ✅ (Otra tabla de Identity si aparece)

#### Tablas de Negocio (10+ tablas)
- ✅ Categorias (5 registros)
- ✅ MetodoPago (5 registros)
- ✅ Prendas (18 registros)
- ✅ Clientes (10 registros)
- ✅ Vendedores (5 registros)
- ✅ Ventas (2 registros)
- ✅ DetalleVenta (4 registros)
- ✅ ConfiguracionSistema (1 registro)
- ✅ ConfiguracionAuditoria (1 registro)
- ✅ DescuentosAutorizados (0 registros, pero tabla existe)

**Total esperado: ~51+ registros**

---

### ✅ #6 - VERIFICAR INTEGRIDAD REFERENCIAL

**En Supabase SQL Editor, ejecuta:**
```sql
-- Verificar foreign keys
SELECT constraint_name, table_name, column_name
FROM information_schema.key_column_usage
WHERE table_schema = 'public'
ORDER BY table_name;
```

Deberías ver todas las relaciones:
- Prendas.CategoriaId → Categorias.Id
- Ventas.ClienteId → Clientes.Id
- Ventas.VendedorId → Vendedores.Id
- Ventas.MetodoPagoId → MetodoPago.Id
- DetalleVenta.VentaId → Ventas.Id
- DetalleVenta.PrendaId → Prendas.Id
- etc.

---

## 🔑 USUARIO DE PRUEBA

**Para ingresar al sistema después:**

| Campo | Valor |
|-------|-------|
| **Username** | `admin` |
| **Email** | `admin@fashionstore.com` |
| **Password** | `Admin123!` |
| **Rol** | Administrador |

---

## 🏃 PASOS RESTANTES (después de migración)

### Paso 7: Reiniciar la App

```powershell
# En PowerShell
$env:SUPABASE_PASSWORD="MiFer2121092001"
cd "c:\Users\CRISTIAN\source\repos\FashionStoreSolution\FashionStore.Web"
dotnet run
```

**Resultado esperado:**
```
info: Microsoft.AspNetCore.Server.Kestrel
      Listening on http://localhost:5100
```

### Paso 8: Acceder a la App

```
http://localhost:5100
```

### Paso 9: Hacer Login

1. Click en "Login" o acceso directo
2. Usuario: `admin`
3. Password: `Admin123!`
4. Click "Sign In"

### Paso 10: Validar Funcionalidad

#### Dashboard
- [ ] Carga sin errores
- [ ] Muestra estadísticas
- [ ] Gráficos visibles

#### Inventario
- [ ] Categorías visibles (5)
- [ ] Prendas visibles (18)
- [ ] Precios correctos
- [ ] Stock correcto

#### Ventas
- [ ] Ventas anteriores visibles (2)
- [ ] Detalles de venta visibles (4)
- [ ] Totales correctos

#### Gestión
- [ ] Crear nueva categoría ✓
- [ ] Crear nueva prenda ✓
- [ ] Registrar nuevo cliente ✓
- [ ] Registrar nueva venta ✓

---

## 🆘 SOLUCIÓN DE PROBLEMAS

### ❌ Error: "Query executed failed"

**Causa:** SQL incompleto o malformado

**Solución:**
1. Verificar que copiaste TODO el archivo
2. No dejar líneas parciales
3. Intentar de nuevo

### ❌ Error: "Public schema already exists"

**Causa:** Esquema ya existe (OK)

**Solución:** Ignorar, es normal en Supabase

### ❌ Error: "Duplicate key value"

**Causa:** Datos ya existen

**Solución:** Ejecutar de nuevo (los `ON CONFLICT DO NOTHING` lo manejan)

### ❌ App no conecta a Supabase

**Causa:** Contraseña incorrecta o sin variable env

**Solución:**
```powershell
$env:SUPABASE_PASSWORD="MiFer2121092001"
# Verificar que esté configurada
$env:SUPABASE_PASSWORD
# Debe mostrar: MiFer2121092001
```

### ❌ Login no funciona

**Causa:** Usuario admin no migrado correctamente

**Solución:**
```sql
-- En Supabase SQL Editor, ejecutar:
SELECT "UserName", "Email", "NormalizedUserName", "NormalizedEmail" 
FROM public."AspNetUsers" 
WHERE "UserName" = 'admin';
```

Debe retornar 1 registro con:
- UserName: admin
- Email: admin@fashionstore.com
- NormalizedUserName: ADMIN
- NormalizedEmail: ADMIN@FASHIONSTORE.COM

---

## ✅ CHECKLIST FINAL

**Migración SQL:**
- [ ] Script ejecutado en Supabase
- [ ] Query executed successfully ✓
- [ ] 12 tablas creadas ✓
- [ ] ~51 registros insertados ✓

**Verificación Supabase:**
- [ ] Tablas visibles en Table Editor
- [ ] AspNetUsers tiene 1 registro (admin)
- [ ] AspNetRoles tiene 3 registros
- [ ] Categorias tiene 5 registros
- [ ] Prendas tiene 18 registros
- [ ] Clientes tiene 10 registros
- [ ] Vendedores tiene 5 registros
- [ ] Ventas tiene 2 registros
- [ ] DetalleVenta tiene 4 registros

**App Funcional:**
- [ ] Build sin errores
- [ ] App ejecuta sin excepciones
- [ ] Login funciona con admin/Admin123!
- [ ] Dashboard carga datos
- [ ] Inventario visible
- [ ] Ventas visible
- [ ] Crear nuevo registro funciona

**🎉 Migración Completada Exitosamente**

---

## 📞 ¿NECESITAS AYUDA?

Si algo no funciona:
1. Verifica que el SQL se ejecutó completamente
2. Confirma que las tablas existen en Table Editor
3. Reinicia la app
4. Borra cookies del navegador
5. Intenta login nuevamente

---

**¡Adelante con la migración!**

