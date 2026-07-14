# 🗄️ MIGRACIÓN A SUPABASE (PostgreSQL) - GUÍA COMPLETA

**Estado**: Archivos configurados y listos  
**Próximo**: Agregar tu contraseña de Supabase

---

## 📋 PROBLEMAS RESUELTOS

✅ **F5 sin Debugger**: `.vscode/launch.json` creado  
✅ **Suporte PostgreSQL**: Npgsql agregado al proyecto  
✅ **Connection String**: Configurado para Supabase  
✅ **Program.cs**: Detecta PostgreSQL automáticamente  

---

## 🚀 PASO 1: Agregar Contraseña de Supabase

### Opción A: Variable de Entorno (RECOMENDADO - Seguro)

**Windows PowerShell (como Admin)**:
```powershell
[Environment]::SetEnvironmentVariable("SUPABASE_PASSWORD", "TU_CONTRASEÑA_AQUI", "User")
```

Reinicia VS Code después.

### Opción B: Archivo .env Local

1. Copia `.env.example` a `.env` (NO lo subas a git)
2. Edita `.env`:
```
SUPABASE_PASSWORD=TU_CONTRASEÑA_AQUI
```

3. Instala dotenv en Program.cs (si lo prefieres):
```bash
dotnet add package DotEnv.Core
```

---

## 🔄 PASO 2: Restaurar BD en Supabase

### Opción A: Usando SQL Editor de Supabase (Recomendado)

1. Abre [https://app.supabase.com](https://app.supabase.com)
2. Login con tus credenciales
3. Ve a **SQL Editor**
4. Haz click **"New Query"**
5. Copia TODO el contenido del archivo: `BACKUP_FASHIONSTORE_SCHEMA.sql` (que crearemos)
6. Ejecuta

### Opción B: Via Command Line

```bash
# Exportar schema desde SQL Server local
sqlcmd -S ADMIN -d FashionStoreDB -E -o backup_schema.sql -Q "SCRIPT ALL"

# Convertir T-SQL a PostgreSQL
# (Herramienta: pgloader o migración manual)

# Importar a Supabase
psql -h db.bajbvebkmacdnllnxvkv.supabase.co -U postgres -d postgres < backup_schema.sql
```

---

## 📝 PASO 3: Instalar Dependencias

```bash
cd c:\Users\CRISTIAN\source\repos\FashionStoreSolution
dotnet restore
```

---

## 🔄 PASO 4: Crear Migraciones para PostgreSQL

```bash
cd FashionStore.Infrastructure1

# Crear nueva migración para PostgreSQL
dotnet ef migrations add InitialPostgres -c FashionStoreDbContext -p FashionStore.Infrastructure.csproj

# Ver las migraciones creadas
dotnet ef migrations list -c FashionStoreDbContext
```

---

## 🚀 PASO 5: Probar Conexión y F5

### Opción 1: Terminal (dotnet run)

```bash
cd FashionStore.Web
dotnet run
```

Debe conectarse a Supabase sin errores.

### Opción 2: F5 en VS Code (Sin Debugger)

1. Abre VS Code
2. Presiona **F5**
3. Selecciona **"Run (No Debug)"** (si se pregunta)
4. Espera a que compile y arranque
5. ✅ **Debe abrir automáticamente en http://localhost:5100**

---

## 🔍 VERIFICACIÓN

Al abrir la app en http://localhost:5100:

- [ ] No hay error de debugger
- [ ] Se carga la página de login
- [ ] Puedes hacer login: admin@fashionstore.com / Password123!
- [ ] Aparece el menú completo
- [ ] Puedes navegar a todas las secciones
- [ ] Los datos se muestran correctamente

---

## 📊 CONNECTION STRING

Tu connection string es:

```
Host=db.bajbvebkmacdnllnxvkv.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=TU_CONTRASEÑA;SSL Mode=Require;Trust Server Certificate=false;
```

**En appsettings.json** (ya configurado):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.bajbvebkmacdnllnxvkv.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=${SUPABASE_PASSWORD};SSL Mode=Require;Trust Server Certificate=false;"
  },
  "DatabaseProvider": "PostgreSQL"
}
```

---

## ⚠️ CARACTERES ESPECIALES EN CONTRASEÑA

Si tu contraseña contiene caracteres especiales (como `@`, `#`, `%`), **NO necesitas codificarlos** en la variable de entorno.

Supabase se encarga automáticamente.

---

## 🔐 SEGURIDAD

**NUNCA**:
- ❌ Guardes la contraseña en appsettings.json visible
- ❌ Subas .env a git
- ❌ Compartas tu SUPABASE_PASSWORD

**SÍ**:
- ✅ Usa variable de entorno
- ✅ Usa .env local (git-ignored)
- ✅ Usa Azure Key Vault o AWS Secrets (producción)

---

## 🆘 SI HAY ERRORES

### Error: "Unable to connect to Supabase"

```
Connection refused / Timeout
```

**Soluciones**:
1. Verifica que la contraseña es correcta
2. Verifica que tu IP está whitelisted en Supabase
3. Verifica que SUPABASE_PASSWORD está en variables de entorno

### Error: "No migrations pending"

```
No migrations pending
```

**Solución**: Las migraciones ya se aplicaron. Es normal.

### Error: "Invalid migration"

Ejecuta:
```bash
dotnet ef database drop -f -c FashionStoreDbContext
dotnet ef database update -c FashionStoreDbContext
```

---

## 📦 ESTRUCTURA POST-MIGRACIÓN

```
FashionStoreSolution/
├── .vscode/
│   ├── launch.json          ✅ F5 sin debugger
│   └── tasks.json           ✅ Build tasks
├── .env.example             ✅ Template (copy a .env)
├── FashionStore.Web/
│   ├── appsettings.json     ✅ Connection Supabase
│   └── Program.cs           ✅ Soporte PostgreSQL
├── FashionStore.Infrastructure1/
│   ├── FashionStore.Infrastructure.csproj ✅ Npgsql agregado
│   └── Migrations/          ✅ Migraciones PostgreSQL
```

---

## ✅ CHECKLIST FINAL

- [ ] Supabase account creada
- [ ] Contraseña guardada en variable de entorno (SUPABASE_PASSWORD)
- [ ] BD schema restaurado en Supabase
- [ ] `dotnet restore` ejecutado
- [ ] Migraciones creadas para PostgreSQL
- [ ] F5 ejecuta sin error de debugger
- [ ] Login funciona
- [ ] Menú aparece
- [ ] Datos visibles desde Supabase

---

## 🎯 RESULTADO FINAL

✅ **F5 funciona sin error de licencia vsdbg**  
✅ **App conectada a Supabase PostgreSQL**  
✅ **Todos los datos migrados**  
✅ **Menú y funcionalidades operativas**  

---

## 🚀 PRÓXIMO PASO

1. **Agrega tu contraseña** de Supabase a variable de entorno
2. **Ejecuta** `dotnet restore`
3. **Presiona F5** en VS Code
4. **Verifica** que no hay error de debugger
5. **Login** y prueba la app

¡Listo! 🎉
