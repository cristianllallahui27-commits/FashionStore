# ✅ VALIDAR MIGRACIÓN - Supabase

## 🔍 Paso 1: Verificar Tablas Creadas

### En Supabase Dashboard:

1. Abre: **https://supabase.com/dashboard**
2. Selecciona proyecto: `bajbvebkmacdnllnxvkv`
3. **Table Editor** (lado izquierdo)

Deberías ver estas 10 tablas:
- ✅ Categorias
- ✅ Prendas
- ✅ Clientes
- ✅ Vendedores
- ✅ MetodoPago
- ✅ DescuentosAutorizados
- ✅ Ventas
- ✅ DetalleVentas
- ✅ ConfiguracionSistema
- ✅ ConfiguracionAuditoria

---

## 📊 Paso 2: Contar Registros

### Opción A: En Supabase SQL Editor

1. **SQL Editor** → **New Query**
2. Ejecuta este script:

```sql
SELECT 
  'Categorias' AS tabla, COUNT(*) AS registros FROM "Categorias"
UNION ALL SELECT 'Prendas', COUNT(*) FROM "Prendas"
UNION ALL SELECT 'Clientes', COUNT(*) FROM "Clientes"
UNION ALL SELECT 'Vendedores', COUNT(*) FROM "Vendedores"
UNION ALL SELECT 'MetodoPago', COUNT(*) FROM "MetodoPago"
UNION ALL SELECT 'DescuentosAutorizados', COUNT(*) FROM "DescuentosAutorizados"
UNION ALL SELECT 'Ventas', COUNT(*) FROM "Ventas"
UNION ALL SELECT 'DetalleVentas', COUNT(*) FROM "DetalleVentas"
UNION ALL SELECT 'ConfiguracionSistema', COUNT(*) FROM "ConfiguracionSistema"
UNION ALL SELECT 'ConfiguracionAuditoria', COUNT(*) FROM "ConfiguracionAuditoria"
ORDER BY tabla;
```

**Resultado esperado:**

```
tabla                      | registros
========================+==========
Categorias                | 5
Prendas                   | 18
Clientes                  | 10
Vendedores                | 5
MetodoPago                | 5
DescuentosAutorizados     | 0 (o más)
Ventas                    | 2+
DetalleVentas             | 4+
ConfiguracionSistema      | 1
ConfiguracionAuditoria    | 1
```

### Opción B: Desde PowerShell

```powershell
$env:PGPASSWORD="MiFer2121092001"

# Contar Categorias
psql -h db.bajbvebkmacdnllnxvkv.supabase.co -U postgres -d postgres `
  -c "SELECT COUNT(*) FROM \"Categorias\";"

# Ver todas las tablas
psql -h db.bajbvebkmacdnllnxvkv.supabase.co -U postgres -d postgres `
  -c "SELECT tablename FROM pg_tables WHERE schemaname='public';"
```

---

## 🔗 Paso 3: Validar Integridad de Datos

### Ejecutar en Supabase SQL Editor:

```sql
-- Verificar que no hay datos huérfanos
SELECT 'Prendas sin categoria' as check_name, COUNT(*) as count 
FROM "Prendas" 
WHERE "CategoriaId" NOT IN (SELECT "Id" FROM "Categorias");

SELECT 'Ventas sin cliente' as check_name, COUNT(*) as count 
FROM "Ventas" 
WHERE "ClienteId" IS NOT NULL AND "ClienteId" NOT IN (SELECT "Id" FROM "Clientes");

SELECT 'DetalleVentas sin venta' as check_name, COUNT(*) as count 
FROM "DetalleVentas" 
WHERE "VentaId" NOT IN (SELECT "Id" FROM "Ventas");
```

**Resultado esperado:** Todas las búsquedas retornan `0`

---

## 🧪 Paso 4: Probar Conexión desde App

### Verificar en PowerShell:

```powershell
# Asegurarse de que el password está configurado
$env:SUPABASE_PASSWORD="MiFer2121092001"

# Ir a la carpeta de la app
cd "c:\Users\CRISTIAN\source\repos\FashionStoreSolution\FashionStore.Web"

# Ejecutar app
dotnet run
```

**En consola debería ver:**

```
[15:XX:XX INF] Now listening on: http://localhost:5100
```

Si ves este mensaje: ✅ **Conexión exitosa**

Si ves error de conexión: ❌ Verificar:
- Password correcto en `$env:SUPABASE_PASSWORD`
- Firewall permite Puerto 5432
- Supabase está online

---

## 📱 Paso 5: Probar en Navegador

1. Abre: **http://localhost:5100**
   - Debería redirigir a: **http://localhost:5100/Identity/Account/Login**

2. Clic en **"Sign Up"** (Registrarse)

3. Crear usuario de prueba:
   - Email: `admin@fashionstore.com`
   - Contraseña: `Test123!`
   - Confirmar contraseña

4. Click **"Register"**

**Resultado esperado:**
- ✅ Usuario creado exitosamente
- ✅ Redirige a login
- ✅ Puedes iniciar sesión

---

## 🎯 Paso 6: Probar Funcionalidades

Una vez logueado, verificar:

### Dashboard
- [ ] Mostrar gráficos (ventas, ingresos)
- [ ] Contar usuarios, clientes

### Catálogo
- [ ] Ver categorías (5)
- [ ] Ver prendas (18)

### Administración
- [ ] Listar clientes (10)
- [ ] Listar vendedores (5)

### Ventas
- [ ] Nueva venta funciona
- [ ] Historial de ventas visible

---

## ✅ Checklist de Validación

- [ ] 10 tablas en Supabase
- [ ] Registros migrados correctamente
- [ ] Sin datos huérfanos
- [ ] App conecta a Supabase
- [ ] Login/Registro funciona
- [ ] Dashboard carga
- [ ] Catálogo visible
- [ ] CRUD de ventas funciona

---

## 🚨 Troubleshooting

### Error: "Connection refused"
```powershell
# Verificar que Supabase está en línea
# Ir a: https://supabase.com/dashboard
# Proyecto debe estar "Running"
```

### Error: "Password authentication failed"
```powershell
# Revisar contraseña
$env:SUPABASE_PASSWORD="MiFer2121092001"  # Exacto

# En appsettings.json
# Password=${SUPABASE_PASSWORD}
```

### Error: "Table does not exist"
```sql
-- En Supabase SQL Editor, verificar tablas
SELECT * FROM information_schema.tables 
WHERE table_schema = 'public';
```

### App no muestra datos
1. Verificar que tablas tienen registros
2. Revisar logs de app (consola)
3. Intentar `dotnet clean && dotnet build`

---

## 📌 Próximo Paso

Si todo está ✅ **validado y funcionando**:

1. **Proceder con correcciones técnicas** (si las hay)
2. **Documentar SSD IEEE 1016**
3. **Crear presentación PPT**
4. **Preparar para deployment**

---

**CHECKLIST FINAL:**

- ✅ Tablas creadas
- ✅ Datos migrados
- ✅ Integridad validada
- ✅ App conectada
- ✅ Funcionalidades probadas

**Sistema listo para producción.**
