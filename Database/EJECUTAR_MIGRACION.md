# 🚀 EJECUTAR MIGRACIÓN SQL Server → Supabase

## ⚡ Forma Más Rápida (Recomendada)

### Opción 1: Copiar-Pegar en Supabase (3 minutos)

1. **Abre Supabase Dashboard:**
   https://supabase.com/dashboard/project/bajbvebkmacdnllnxvkv/sql/new

2. **Click "New Query"**

3. **Copia TODOO el contenido de:**
   ```
   Database/SUPABASE_SETUP_COMPLETO.sql
   ```

4. **Pega en SQL Editor**

5. **Click "Run" (Ctrl+Enter)**

✅ **Resultado:** 9 tablas + 51 registros de ejemplo

---

## 🔧 Alternativa: Migrar Datos Reales de SQL Server

### Requisito: PostgreSQL Client (psql)

**Windows:**
```powershell
# Opción A: Descargar desde
# https://www.postgresql.org/download/windows/
# Seleccionar "Command Line Tools Only"

# Opción B: Usar WSL
wsl

# Opción C: En WSL Ubuntu
sudo apt-get install postgresql-client
```

### Comando de Migración (pgloader)

Si tienes **pgloader** instalado:

```powershell
cd "c:\Users\CRISTIAN\source\repos\FashionStoreSolution\Database"

# Ejecutar migración
pgloader config.load
```

**Tiempo:** ~2-5 minutos

---

## 📝 Si No Tienes Herramientas

### Exportar SQL Server manualmente:

```sql
-- En SQL Server Management Studio (SSMS):
-- 1. Right-click tabla → Export data
-- 2. Seleccionar "Flat File Destination"
-- 3. Exportar como CSV
-- 4. Repetir para cada tabla
```

### Importar en Supabase:

```sql
-- En Supabase SQL Editor:
COPY "Categorias" FROM STDIN WITH (FORMAT csv, DELIMITER ',');
-- Pegar datos del CSV
\.
```

---

## ✅ Validación Post-Migración

```powershell
# Verificar conexión a Supabase
$env:PGPASSWORD="MiFer2121092001"
psql -h db.bajbvebkmacdnllnxvkv.supabase.co -U postgres -d postgres -c "SELECT COUNT(*) FROM \"Categorias\";"
```

**Debería mostrar:** `count` con número de registros

---

## 🎯 Próximo Paso

Una vez migrados los datos:

```powershell
cd "c:\Users\CRISTIAN\source\repos\FashionStoreSolution\FashionStore.Web"

$env:SUPABASE_PASSWORD="MiFer2121092001"

dotnet run
```

App estará en: **http://localhost:5100**

---

## 📌 Archivos Disponibles

- `SUPABASE_SETUP_COMPLETO.sql` → Crear tablas + datos de ejemplo
- `config.load` → Configuración pgloader
- `INSTALAR_PGLOADER.md` → Guía instalación pgloader
- `MigracionSqlServerSupabase.cs` → Código C# migración
- `MigracionDirect.ps1` → PowerShell script (alternativa)

**Elige la que mejor funcione para tu ambiente.**
