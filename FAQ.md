# ❓ Preguntas Frecuentes - FashionStore + Supabase

## 🔐 Seguridad

### P: ¿Cómo guardo la contraseña de forma segura?
**R:** Nunca guardes la contraseña en el código fuente. Usa variables de entorno:

```powershell
# Windows - Variable de entorno permanente
[System.Environment]::SetEnvironmentVariable(
	"SUPABASE_PASSWORD", 
	"tu_contraseña", 
	[System.EnvironmentVariableTarget]::User
)

# Luego reinicia Visual Studio
```

O usa el script que proporcionamos:
```powershell
.\setup-supabase-env.ps1
```

---

### P: ¿Qué si alguien ve mi contraseña de Supabase?
**R:** Regénérala inmediatamente:
1. Ve a https://supabase.com/dashboard
2. Selecciona tu proyecto
3. Database → Users → postgres
4. Cambia la contraseña

---

### P: ¿Por qué SSL Mode=Require?
**R:** Supabase requiere conexión cifrada. Es obligatorio para conectarse desde la nube.

---

## 🗄️ Base de Datos

### P: ¿Las tablas se crean automáticamente?
**R:** No. Debes:
1. Ejecutar el script `supabase_init.sql` manualmente en Supabase
2. Entity Framework solo VERIFICA que existan (no las crea)

Si intentas ejecutar la app sin las tablas:
```
❌ Error: Relation "Categorias" does not exist
```

---

### P: ¿Puedo usar Entity Framework para crear las tablas?
**R:** Sí, pero requiere migraciones. Después de crear el script SQL inicial:

```powershell
# Crear una migración
dotnet ef migrations add InitialCreate

# Aplicar cambios futuros
dotnet ef database update
```

---

### P: ¿Cómo elimino todas las tablas y empiezo de cero?
**R:** En SQL Editor de Supabase:

```sql
-- Eliminar todas las tablas (cuidado: datos se pierden)
DROP SCHEMA public CASCADE;
CREATE SCHEMA public;

-- Luego ejecuta nuevamente supabase_init.sql
```

---

### P: ¿Qué hago si una tabla ya existe?
**R:** El script SQL usa `CREATE TABLE IF NOT EXISTS`. Si re-ejecutas:
- ✅ Las nuevas tablas se crean
- ✅ Las existentes no se afectan
- ✅ Los índices se ignoran si ya existen

---

## 🔗 Conexión

### P: Error: "Connection refused"
**R:** Verificar:
```powershell
# 1. ¿Supabase está disponible?
ping db.bajbvebkmacdnllnxvkv.supabase.co

# 2. ¿La contraseña está configurada?
$env:SUPABASE_PASSWORD

# 3. ¿El firewall permite acceso?
# Supabase permite conexiones desde cualquier IP
```

---

### P: Error: "SSL connection error"
**R:** 
- ✅ Verifica que `SSL Mode=Require` esté en la conexión
- ✅ No necesitas certificado (Supabase lo maneja)
- ✅ `Trust Server Certificate=false` es lo correcto

---

### P: Error: "Authentication failed for user postgres"
**R:** La contraseña está mal. Prueba:
```powershell
# 1. Reconfigura la variable
.\setup-supabase-env.ps1

# 2. Si aún no funciona, regenera la contraseña en Supabase
#    Dashboard → Database → Users → postgres
```

---

## 📱 Entity Framework

### P: ¿Cómo crear una migración nueva?
**R:**
```powershell
# Editar una entidad (ej: agregar campo a Categoria)

# Crear migración
dotnet ef migrations add AddNuevoCampo --project FashionStore.Infrastructure

# Aplicar a la BD
dotnet ef database update
```

---

### P: ¿Cómo revierto una migración?
**R:**
```powershell
# Revertir la última migración
dotnet ef database update NombreMigracionAnterior

# O eliminar la migración no aplicada
dotnet ef migrations remove
```

---

### P: ¿Dónde se guardan las migraciones?
**R:** En `FashionStore.Infrastructure/Migrations/`

```
Migrations/
├── 20250101120000_InitialCreate.cs
├── 20250102150000_AddNuevoCampo.cs
└── FashionStoreDbContextModelSnapshot.cs
```

---

## 🚀 Ejecución

### P: Error al ejecutar: "dotnet run"
**R:**
```powershell
# 1. Limpiar compilaciones anteriores
dotnet clean

# 2. Restaurar paquetes
dotnet restore

# 3. Compilar
dotnet build

# 4. Ejecutar
dotnet run
```

---

### P: ¿Cómo cambio el puerto donde ejecuta la app?
**R:**
```powershell
# Opción 1: Variable de entorno
$env:ASPNETCORE_URLS = "http://localhost:5001"
dotnet run

# Opción 2: Editando launchSettings.json
# FashionStore.Web\Properties\launchSettings.json
```

---

### P: ¿Por qué tarda tanto en compilar?
**R:** Es la primera compilación. Luego será más rápido:
- Primera vez: ~30-60 segundos
- Compilaciones siguientes: ~5-10 segundos
- En release: es más lento (~2-3 minutos) pero optimiza el código

---

## 📊 Datos

### P: ¿Cómo cargo datos iniciales en las tablas?
**R:** Hay varias formas:

**Opción 1: SQL directo en Supabase**
```sql
INSERT INTO "Categorias" ("Nombre", "Descripcion") 
VALUES 
  ('Camisetas', 'Ropa casual'),
  ('Pantalones', 'Pantalones varios'),
  ('Zapatos', 'Calzado deportivo');
```

**Opción 2: Seed en Entity Framework**
```csharp
// En FashionStoreDbContext.OnModelCreating()
modelBuilder.Entity<Categoria>().HasData(
	new Categoria { Id = 1, Nombre = "Camisetas", ... },
	...
);
```

---

### P: ¿Cómo backup de mis datos?
**R:** 
1. Ve a https://supabase.com/dashboard
2. Selecciona tu proyecto
3. Database → Backups (automáticos cada día)
4. O usa PostgreSQL export:

```powershell
pg_dump -h db.bajbvebkmacdnllnxvkv.supabase.co `
		-U postgres `
		-d postgres `
		-F c > backup.dump
```

---

## 🐛 Debugging

### P: ¿Cómo veo las queries SQL que genera Entity Framework?
**R:** Ya está en el `Program.cs`:

```csharp
.LogTo(Console.WriteLine, LogLevel.Information)
```

Verás en la consola:
```sql
SELECT "c"."Id", "c"."Nombre" FROM "Categorias" AS "c"
```

---

### P: ¿Cómo activo logging más detallado?
**R:** Edita `appsettings.Development.json`:

```json
{
  "Logging": {
	"LogLevel": {
	  "Default": "Debug",
	  "Microsoft": "Information",
	  "Microsoft.EntityFrameworkCore.Database.Command": "Debug"
	}
  }
}
```

---

### P: ¿Dónde veo los logs de errores?
**R:** En `FashionStore.Web/logs/fashionstore-YYYYMMDD.txt`

---

## 🌐 Producción

### P: ¿Cómo despliego a Azure?
**R:** Usa Azure App Service + Azure Database for PostgreSQL:

```powershell
# Crear recurso en Azure
az webapp create --resource-group miGrupo --plan miPlan --name miApp

# Publicar
dotnet publish -c Release
```

---

### P: ¿Necesito cambiar la conexión para producción?
**R:** Sí, usa variables de entorno en Azure:

```
Azure Portal → Aplicación → Configuración → Variables de entorno
SUPABASE_PASSWORD = tu_contraseña_prod
```

---

## 📞 Contacto / Soporte

### P: Me aparece un error que no está aquí
**R:**
1. Revisa los logs: `FashionStore.Web/logs/`
2. Lee la guía: `GUIA_SUPABASE_SETUP.md`
3. Verifica Supabase está online: https://supabase.com/status

---

### P: ¿Hay un chat de soporte?
**R:** 
- Supabase: https://supabase.com/docs
- .NET: https://docs.microsoft.com/dotnet
- Entity Framework: https://docs.microsoft.com/ef/core/

---

## ✅ Checklist de Troubleshooting

Si algo no funciona, prueba esto en orden:

- [ ] ✅ Reiniciar Visual Studio
- [ ] ✅ `dotnet clean && dotnet restore`
- [ ] ✅ Verificar variable de entorno: `$env:SUPABASE_PASSWORD`
- [ ] ✅ Verificar que Supabase esté disponible
- [ ] ✅ Verificar credenciales en appsettings
- [ ] ✅ Ejecutar el script SQL nuevamente
- [ ] ✅ Revisar logs: `FashionStore.Web/logs/`
- [ ] ✅ Compilar sin ejecutar: `dotnet build`
- [ ] ✅ Reiniciar la máquina (último recurso)

---

¿Necesitas más ayuda? 📞 Revisa la documentación completa en `GUIA_SUPABASE_SETUP.md`
