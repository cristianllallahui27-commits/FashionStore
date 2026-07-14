# 🚀 SQL PARA EJECUTAR EN SUPABASE - SIN CLI

**Método:** SQL Editor directo (sin necesidad de Supabase CLI)  
**Tiempo:** ~3-5 minutos  
**Status:** Listo para ejecutar AHORA MISMO

---

## 📋 INSTRUCCIONES PASO A PASO

### PASO 1: Abrir Supabase SQL Editor

**URL:**
```
https://supabase.com/dashboard/project/bajbvebkmacdnllnxvkv/sql/new
```

**O manualmente:**
1. Ve a https://supabase.com/dashboard
2. Click en tu proyecto: `bajbvebkmacdnllnxvkv`
3. Click en **"SQL Editor"** (lado izquierdo)
4. Click en **"New Query"** (arriba derecha)

---

### PASO 2: COPIAR EL SQL

**Abre el archivo:**
```
c:\Users\CRISTIAN\source\repos\FashionStoreSolution\Database\MIGRACION_COMPLETA_SUPABASE.sql
```

**Pasos:**
1. Click derecho → "Abrir con" → "Notepad"
2. Ctrl+A (seleccionar TODO)
3. Ctrl+C (copiar)

---

### PASO 3: PEGAR EN SUPABASE

**En el editor SQL de Supabase:**
1. Click en el área blanca (editor)
2. Ctrl+V (pegar)

**Deberías ver:**
```sql
-- ==========================================
-- MIGRACIÓN COMPLETA: SQL Server → Supabase
-- ==========================================
-- Fashion Store - Sistema Administrativo
...
```

---

### PASO 4: EJECUTAR

**Opciones:**
- Click botón **"Run"** (arriba derecha) 
- O presiona **Ctrl+Enter**

**Espera a ver:**
```
✅ Query executed successfully
```

**Tiempo:** 20-40 segundos

---

### PASO 5: VALIDAR

**En Supabase, click en "Table Editor" (lado izquierdo)**

Deberías ver estas tablas:

#### AspNetUsers (ASP.NET Identity)
```
✅ AspNetUsers             (1 registro: admin)
✅ AspNetRoles             (3 registros: Roles)
✅ AspNetUserRoles         (1 registro)
✅ AspNetUserClaims        (0 registros)
✅ AspNetUserLogins        (0 registros)
✅ AspNetUserTokens        (0 registros)
✅ AspNetRoleClaims        (0 registros)
```

#### Tablas de Negocio
```
✅ Categorias              (5 registros)
✅ MetodoPago              (5 registros)
✅ Prendas                 (18 registros)
✅ Clientes                (10 registros)
✅ Vendedores              (5 registros)
✅ Ventas                  (2 registros)
✅ DetalleVenta            (4 registros)
✅ ConfiguracionSistema    (1 registro)
✅ ConfiguracionAuditoria  (1 registro)
✅ DescuentosAutorizados   (0 registros)
```

**Total:** ~51+ registros en 17 tablas ✅

---

## ✅ COMANDOS DE VALIDACIÓN

**Después de ejecutar el SQL, puedes ejecutar estos comandos en Supabase para verificar:**

### Validación 1: Contar tablas
```sql
SELECT COUNT(*) as "Total_Tablas"
FROM information_schema.tables 
WHERE table_schema = 'public'
AND table_name NOT LIKE 'pg_%';
```
**Resultado esperado:** 17

### Validación 2: Usuario administrador
```sql
SELECT "UserName", "Email", "NormalizedUserName" 
FROM public."AspNetUsers" 
WHERE "UserName" = 'admin';
```
**Resultado esperado:** 1 registro (admin, admin@fashionstore.com, ADMIN)

### Validación 3: Roles
```sql
SELECT "Name" FROM public."AspNetRoles" ORDER BY "Name";
```
**Resultado esperado:** Administrador, Gerente, Vendedor

### Validación 4: Total de registros por tabla
```sql
SELECT 
  'AspNetUsers' as tabla, COUNT(*) as registros FROM public."AspNetUsers"
UNION ALL SELECT 'Categorias', COUNT(*) FROM public."Categorias"
UNION ALL SELECT 'Prendas', COUNT(*) FROM public."Prendas"
UNION ALL SELECT 'Clientes', COUNT(*) FROM public."Clientes"
UNION ALL SELECT 'Vendedores', COUNT(*) FROM public."Vendedores"
UNION ALL SELECT 'MetodoPago', COUNT(*) FROM public."MetodoPago"
UNION ALL SELECT 'Ventas', COUNT(*) FROM public."Ventas"
UNION ALL SELECT 'DetalleVenta', COUNT(*) FROM public."DetalleVenta"
ORDER BY tabla;
```

---

## 🎯 USUARIO DE PRUEBA

**Para acceder a la aplicación después:**

| Campo | Valor |
|-------|-------|
| **Username** | `admin` |
| **Password** | `Admin123!` |
| **Email** | `admin@fashionstore.com` |
| **Rol** | Administrador |

---

## 🔄 SI ALGO FALLA

### Error: "Query executed failed"
**Causa:** SQL incompleto o error de sintaxis

**Solución:**
1. Verificar que copiaste TODO el archivo (sin líneas cortadas)
2. Revisar que no hay caracteres especiales rotos
3. Intentar de nuevo

### Error: "Public schema already exists"
**Causa:** Esquema ya existe (normal)

**Solución:** Ignorar, es OK. Los `IF NOT EXISTS` lo manejan.

### Error: "Duplicate key"
**Causa:** Datos ya existen

**Solución:** Ejecutar de nuevo (los `ON CONFLICT DO NOTHING` evitan duplicados)

### Error: "Permission denied"
**Causa:** Usuario sin permisos

**Solución:**
1. Verificar que usas usuario `postgres` (no anon)
2. Cambiar role a postgres:
```sql
SET ROLE postgres;
```

---

## 📊 ESTRUCTURA DEL SQL

El archivo `MIGRACION_COMPLETA_SUPABASE.sql` contiene:

### Sección 1: ASP.NET Identity (7 tablas)
```sql
CREATE TABLE "AspNetUsers" (...)
CREATE TABLE "AspNetRoles" (...)
CREATE TABLE "AspNetUserRoles" (...)
-- ... etc (7 tablas total)
INSERT INTO "AspNetRoles" VALUES (...)
INSERT INTO "AspNetUsers" VALUES (...)
INSERT INTO "AspNetUserRoles" VALUES (...)
```

### Sección 2: Tablas de Negocio (10 tablas)
```sql
CREATE TABLE "Categorias" (...)
CREATE TABLE "Prendas" (...)
CREATE TABLE "Clientes" (...)
-- ... etc (10 tablas total)
```

### Sección 3: Índices (12 índices)
```sql
CREATE INDEX idx_prendas_categoria (...)
CREATE INDEX idx_ventas_fecha (...)
-- ... etc (12 índices total)
```

### Sección 4: Datos Iniciales
```sql
INSERT INTO "Categorias" VALUES (...)
INSERT INTO "Prendas" VALUES (...)
INSERT INTO "Clientes" VALUES (...)
-- ... datos iniciales (~51 registros)
```

### Sección 5: Validación Final
```sql
SELECT COUNT(*) FROM "Categorias"
SELECT COUNT(*) FROM "Prendas"
-- ... validación de que todo se creó correctamente
```

---

## 🚀 PASOS DESPUÉS DE MIGRACIÓN

### 1. Reiniciar App
```powershell
$env:SUPABASE_PASSWORD="MiFer2121092001"
cd FashionStore.Web
dotnet run
```

### 2. Acceder a http://localhost:5100

### 3. Login
- Usuario: `admin`
- Password: `Admin123!`

### 4. Validar Funcionalidad
- [ ] Dashboard carga
- [ ] Inventario visible
- [ ] Ventas funcional
- [ ] Crear registros funciona

---

## ✅ CHECKLIST FINAL

**Antes de ejecutar SQL:**
- [ ] Archivo copiado completamente
- [ ] Navegador abierto en Supabase
- [ ] SQL Editor abierto (New Query)

**Durante ejecución:**
- [ ] Click "Run" o Ctrl+Enter
- [ ] Esperar 20-40 segundos
- [ ] Ver "✅ Query executed successfully"

**Después:**
- [ ] Click "Table Editor"
- [ ] Verificar 17 tablas creadas
- [ ] Confirmar ~51+ registros
- [ ] Ejecutar comandos de validación

**Reiniciar app:**
- [ ] `dotnet run` con SUPABASE_PASSWORD
- [ ] Navegar a http://localhost:5100
- [ ] Login con admin/Admin123!
- [ ] Validar funcionalidad

---

## 💡 TIPS

1. **Si es muy lento:** Supabase puede estar ocupado. Esperar 1-2 minutos y reintentar.

2. **Si necesitas borrar todo:** Ejecutar en SQL Editor:
```sql
DROP SCHEMA public CASCADE;
CREATE SCHEMA public;
```
Luego ejecutar el SQL nuevamente.

3. **Para ver el progreso:** Mientras se ejecuta, click en "Executing..." para ver detalle.

4. **Para cancelar:** Click en botón "X" o "Stop" si aparece.

---

## 📞 VALIDACIÓN RÁPIDA

```powershell
# Después de ejecutar SQL en Supabase

# 1. Build app
dotnet build -c Release
# ✅ Debe completar sin errores

# 2. Ejecutar app
$env:SUPABASE_PASSWORD="MiFer2121092001"
cd FashionStore.Web
dotnet run
# ✅ Escucha en http://localhost:5100

# 3. Abrir navegador
# http://localhost:5100
# Login: admin / Admin123!
# ✅ Redirige a Dashboard
```

---

**¿Listo? ¡Adelante con la migración! 🚀**

