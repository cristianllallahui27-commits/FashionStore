# 📦 Instalar pgloader en Windows

## Opción 1: LIBREOFFICE (+ pgloader)

pgloader está incluido en algunas distribuciones. En Windows, la forma más simple es:

### Paso 1: Descargar pgloader pre-compilado

```powershell
# Descargar binario pgloader para Windows
$url = "https://github.com/dimitri/pgloader/releases/download/v3.6.9/pgloader.exe"
$dest = "C:\Program Files\pgloader\pgloader.exe"

# Crear carpeta
New-Item -ItemType Directory -Path "C:\Program Files\pgloader" -Force

# Descargar
Invoke-WebRequest -Uri $url -OutFile $dest
```

### Paso 2: Agregar a PATH

```powershell
# Agregar pgloader a PATH (permanente)
[Environment]::SetEnvironmentVariable(
    "Path",
    [Environment]::GetEnvironmentVariable("Path", "User") + ";C:\Program Files\pgloader",
    "User"
)
```

### Paso 3: Verificar instalación

```powershell
pgloader --version
```

---

## Opción 2: Docker (recomendado)

Si tienes Docker Desktop instalado:

```bash
docker run --rm \
  -v $(pwd)/config.load:/config.load \
  dimitri/pgloader:latest \
  pgloader /config.load
```

---

## Opción 3: WSL (Windows Subsystem for Linux)

```bash
# En WSL (Bash/Ubuntu)
sudo apt-get update
sudo apt-get install -y pgloader

# Ejecutar
pgloader config.load
```

---

## Opción 4: PowerShell Script (sin pgloader)

Si no puedes instalar pgloader, usar script PowerShell:

```powershell
cd "c:\Users\CRISTIAN\source\repos\FashionStoreSolution\Database"
.\MIGRACION_BCP_A_SUPABASE.ps1
```

**Requiere:**
- SQL Server Client Tools (bcp)
- PostgreSQL Client (psql)

---

## Instalación de PostgreSQL Client (psql) en Windows

Si no tienes psql:

```powershell
# Opción A: Descargar standalone PostgreSQL client
$url = "https://www.postgresql.org/download/windows/"
# → Descargar "PostgreSQL Installer"
# → Solo seleccionar "Command Line Tools"

# Opción B: Usar Chocolatey
choco install postgresql

# Opción C: Verificar versión
psql --version
```

---

## Instalación de SQL Server Client Tools (bcp)

Si no tienes bcp:

```powershell
# Opción A: Instalar SQLCMD
choco install sqlserver-cmdline-utilities

# Opción B: Usar SQL Server Management Studio (SSMS)
# Incluye bcp automáticamente

# Opción C: Verificar
bcp -v
```

---

## Comando de Ejecución

### Con pgloader:

```powershell
cd "c:\Users\CRISTIAN\source\repos\FashionStoreSolution\Database"
pgloader config.load
```

### Con Script PowerShell:

```powershell
cd "c:\Users\CRISTIAN\source\repos\FashionStoreSolution\Database"
powershell -ExecutionPolicy Bypass -File ".\MIGRACION_BCP_A_SUPABASE.ps1"
```

---

## Troubleshooting

### Error: "pgloader no encontrado"
```powershell
# Verificar PATH
$env:Path -split ";"

# Reinstalar PATH
[Environment]::SetEnvironmentVariable("Path", "$env:Path;C:\Program Files\pgloader", "User")

# Reiniciar PowerShell
```

### Error: "Connection refused"
```powershell
# Verificar SQL Server
sqlcmd -S ADMIN -Q "SELECT @@VERSION;"

# Verificar Supabase
psql -h db.bajbvebkmacdnllnxvkv.supabase.co -U postgres -d postgres -c "SELECT 1;"
```

### Error: "Authentication failed"
```powershell
# Revisar credenciales en config.load:
# - Usuario SQL Server: ADMIN (Trusted Connection)
# - Usuario Supabase: postgres
# - Password: MiFer2121092001
```

---

## Tiempo Esperado

- **Descarga**: 2-3 min
- **Migración**: 1-5 min (depende de volumen)
- **Validación**: 30 seg

**Total**: ~5-10 minutos

---

## Post-Migración

Una vez completada:

```powershell
# 1. Verificar datos en Supabase
psql -h db.bajbvebkmacdnllnxvkv.supabase.co \
  -U postgres -d postgres \
  -c "SELECT COUNT(*) FROM \"Categorias\";"

# 2. Reiniciar app
$env:SUPABASE_PASSWORD="MiFer2121092001"
dotnet run
```

---

## Referencia

- [pgloader GitHub](https://github.com/dimitri/pgloader)
- [PostgreSQL Client Download](https://www.postgresql.org/download/)
- [SQL Server Client Tools](https://learn.microsoft.com/en-us/sql/tools/sqlcmd-utility)
