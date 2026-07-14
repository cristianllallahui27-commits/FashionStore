# 🚀 EJECUTAR SETUP SUPABASE

## ✅ REQUISITOS

1. **Acceso a Supabase**: https://supabase.com/
2. **Proyecto creado**: `db.bajbvebkmacdnllnxvkv.supabase.co`
3. **Password**: `MiFer2121092001`
4. **SQL Editor disponible**

## 📝 PASOS

### 1. Abre SQL Editor en Supabase

```
https://supabase.com/dashboard/project/[YOUR-PROJECT]/sql/new
```

### 2. Copia TODO el contenido de:

```
Database/SUPABASE_SETUP_COMPLETO.sql
```

### 3. Pega en SQL Editor de Supabase

### 4. Ejecuta (Ctrl+Enter o botón "Run")

✅ Debería ver confirmación:

```
Categorias      | 5
Prendas         | 18
Clientes        | 10
Vendedores      | 5
MetodoPago      | 5
Ventas          | 2
DetalleVenta    | 4
ConfiguracionSistema    | 1
ConfiguracionAuditoria  | 1
```

## 🔧 VERIFICAR CONEXIÓN

Ejecuta en Terminal (con SUPABASE_PASSWORD definida):

```powershell
cd "c:\Users\CRISTIAN\source\repos\FashionStoreSolution\FashionStore.Web"
$env:SUPABASE_PASSWORD="MiFer2121092001"
dotnet run
```

✅ Si ves `Now listening on: http://localhost:5100/` → Supabase conectado ✓

## 📊 CHEQUEAR DATOS EN SUPABASE

1. Dashboard → Table Editor
2. Selecciona cada tabla para ver registros:
   - Categorias (5)
   - Prendas (18)
   - Clientes (10)
   - Vendedores (5)
   - MetodoPago (5)
   - Ventas (2 con detalles)
   - ConfiguracionSistema (1)
   - ConfiguracionAuditoria (1)

## ❌ TROUBLESHOOTING

**Error**: `Connection refused`
- Verificar password en environment
- Verificar Host en appsettings.json

**Error**: `Duplicate key`
- Las inserciones usan `ON CONFLICT DO NOTHING`
- Seguro ejecutar múltiples veces

**Error**: `Foreign key constraint`
- Asegurar que Categorías existen antes de Prendas
- Script está en orden correcto

## 📌 NOTA IMPORTANTE

- **App trabaja SOLO con Supabase**
- **Sin fallback a SQL Server**
- **Password requerido en: $env:SUPABASE_PASSWORD**

---

Listo. El setup Supabase completo (esquema + datos) se ejecuta en 1-2 minutos.
