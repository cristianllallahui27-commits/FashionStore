# 🚀 EJECUTAR PGLOADER EN WSL (Windows)

## ✅ Archivo config.load YA CREADO

El archivo está en:
```
Database/config.load
```

## 📝 PASOS PARA EJECUTAR

### Paso 1: Abrir WSL (Bash)

```powershell
# En PowerShell o CMD ejecuta:
wsl
```

Debería entrar a Ubuntu/Debian bash.

### Paso 2: Instalar pgloader (si no existe)

```bash
sudo apt-get update
sudo apt-get install -y pgloader postgresql-client
```

### Paso 3: Copiar config.load a WSL

```bash
# Desde WSL bash:
cp /mnt/c/Users/CRISTIAN/source/repos/FashionStoreSolution/Database/config.load ~/config.load
```

### Paso 4: Ejecutar pgloader

```bash
# Ejecutar migración
pgloader ~/config.load
```

**Debería mostrar:**
```
Processing the log file 'pgloader.log'
Creating index for 'Categorias' ...
Creating index for 'Prendas' ...
...
COPY "DetalleVentas" (...)
...
```

### Paso 5: Salir de WSL

```bash
exit
```

---

## ⏱️ TIEMPO ESPERADO

- Instalación: 2-3 minutos
- Migración: 1-5 minutos
- **TOTAL: 5-8 minutos**

---

## ✅ VERIFICAR MIGRACIÓN

Después de ejecutar pgloader:

```bash
# Desde WSL bash:
export PGPASSWORD="MiFer2121092001"

psql -h db.bajbvebkmacdnllnxvkv.supabase.co -U postgres -d postgres -c "
SELECT COUNT(*) FROM \"Categorias\";
SELECT COUNT(*) FROM \"Prendas\";
SELECT COUNT(*) FROM \"Clientes\";
"
```

Debería mostrar números > 0 para cada tabla.

---

## 🐛 TROUBLESHOOTING

### Error: "pgloader: command not found"
```bash
# Instalar pgloader
sudo apt-get install -y pgloader
```

### Error: "Cannot connect to SQL Server"
```bash
# Verificar que ADMIN (SQL Server) está corriendo
# Desde PowerShell (no WSL):
sqlcmd -S ADMIN -Q "SELECT @@VERSION;"
```

### Error: "Cannot connect to Supabase"
```bash
# Verificar credenciales en config.load
# Password: MiFer2121092001
# Host: db.bajbvebkmacdnllnxvkv.supabase.co
```

---

## 📌 ALTERNATIVA: Si WSL no funciona

Usar el SQL script manual:
```
Database/SUPABASE_SETUP_COMPLETO.sql
```

Ir a Supabase Editor → Copiar-Pegar → Run

---

## 🎯 SIGUIENTE

Una vez completada la migración:

```powershell
# Desde PowerShell (fuera de WSL):
cd "c:\Users\CRISTIAN\source\repos\FashionStoreSolution\FashionStore.Web"

$env:SUPABASE_PASSWORD="MiFer2121092001"

dotnet run
```

Abre: **http://localhost:5100**

---

**LISTO PARA MIGRAR CON PGLOADER**
