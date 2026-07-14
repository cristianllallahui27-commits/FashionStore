# ✅ MIGRACIÓN SIN WSL (Windows Directo)

## 🎯 3 OPCIONES DISPONIBLES

### OPCIÓN 1: Supabase SQL Editor (MÁS SIMPLE - 3 minutos)

**RECOMENDADA** - Sin instalar nada

1. Abre: https://supabase.com/dashboard/project/bajbvebkmacdnllnxvkv/sql/new

2. Click **"New Query"**

3. Abre este archivo en Notepad:
   ```
   c:\Users\CRISTIAN\source\repos\FashionStoreSolution\
   Database\SUPABASE_SETUP_COMPLETO.sql
   ```

4. **Copia TODO** (Ctrl+A, Ctrl+C)

5. **Pega en Supabase** (Ctrl+V)

6. Click **"Run"** (Ctrl+Enter)

✅ **LISTO:** 10 tablas + 51 registros

---

### OPCIÓN 2: pgloader vía Docker (si tienes Docker Desktop)

```powershell
# Ejecutar en PowerShell
docker run --rm `
  -v "c:\Users\CRISTIAN\source\repos\FashionStoreSolution\Database\config.load:/config.load" `
  dimitri/pgloader:latest `
  pgloader /config.load
```

✅ Tiempo: 2-5 minutos

---

### OPCIÓN 3: pgloader ejecutable directo

Si descargaste pgloader.exe:

```powershell
cd "c:\Users\CRISTIAN\source\repos\FashionStoreSolution\Database"

C:\pgloader\pgloader.exe config.load
```

✅ Tiempo: 2-5 minutos

---

## 🎯 RECOMENDACIÓN

**Usa OPCIÓN 1 (Supabase Editor)** - Es la más simple:

1. No requiere instalar nada
2. Sin dependencias
3. 3 minutos total
4. 100% confiable

---

## ✅ DESPUÉS DE MIGRAR

Verificar en Supabase Dashboard → **Table Editor**:

Deberías ver 10 tablas con datos:
```
✅ Categorias (5+)
✅ Prendas (18+)
✅ Clientes (10+)
✅ Vendedores (5+)
✅ MetodoPago (5+)
✅ DescuentosAutorizados
✅ Ventas
✅ DetalleVentas
✅ ConfiguracionSistema (1)
✅ ConfiguracionAuditoria (1)
```

---

## 🚀 LUEGO EJECUTAR APP

```powershell
cd "c:\Users\CRISTIAN\source\repos\FashionStoreSolution\FashionStore.Web"

$env:SUPABASE_PASSWORD="MiFer2121092001"

dotnet run
```

Abre: **http://localhost:5100**

---

**Yo te espero aquí si necesitas ayuda con cualquiera de las 3 opciones.**
