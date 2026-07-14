# 📚 Guía: Ejecutar el Script SQL en Supabase

## ✅ Pasos para crear las tablas en tu base de datos Supabase

### **1. Acceder a SQL Editor en Supabase**
- Ve a: https://supabase.com/dashboard/project/bajbvebkmacdnllnxvkv/sql/new
- O en el Dashboard: SQL Editor → New Query

### **2. Copiar y pegar el script**
- Copia todo el contenido de `supabase_init.sql`
- Pégalo en el SQL Editor de Supabase
- Haz clic en ▶️ "Run" (o presiona Ctrl+Enter)

### **3. Verificar la creación**
- En el SQL Editor, ejecuta esta consulta para confirmar:
```sql
SELECT table_name FROM information_schema.tables 
WHERE table_schema = 'public';
```

Deberías ver todas estas tablas:
- ✅ Categorias
- ✅ Prendas
- ✅ Clientes
- ✅ Vendedores
- ✅ MetodosPago (con datos iniciales)
- ✅ DescuentosAutorizados
- ✅ Ventas
- ✅ DetalleVentas
- ✅ ConfiguracionSistema
- ✅ AspNetUsers (Identity)
- ✅ AspNetRoles (Identity)
- ✅ AspNetUserRoles
- ✅ AspNetUserClaims
- ✅ AspNetUserLogins
- ✅ AspNetRoleClaims
- ✅ AspNetUserTokens

---

## 🔐 Configuración de Credenciales (IMPORTANTE)

### **Opción 1: Variables de Entorno (RECOMENDADO)**

En PowerShell (Windows):
```powershell
$env:SUPABASE_PASSWORD = "tu_contraseña_aqui"
```

Para hacerla permanente (Windows):
```powershell
[System.Environment]::SetEnvironmentVariable("SUPABASE_PASSWORD", "tu_contraseña_aqui", [System.EnvironmentVariableTarget]::User)
```

Luego reinicia Visual Studio.

### **Opción 2: Archivo appsettings.Development.json**

1. Crea o edita `FashionStore.Web/appsettings.Development.json`
2. Agrégale esto:
```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Host=db.bajbvebkmacdnllnxvkv.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=tu_contraseña_aqui;SSL Mode=Require;Trust Server Certificate=false;"
  },
  "SUPABASE_PASSWORD": "tu_contraseña_aqui"
}
```

3. **⚠️ IMPORTANTE:** Agrega `appsettings.Development.json` a `.gitignore` para no exponer contraseñas

### **Opción 3: secrets.json (Desarrollo Local)**

```powershell
dotnet user-secrets init --project FashionStore.Web
dotnet user-secrets set "SUPABASE_PASSWORD" "tu_contraseña_aqui" --project FashionStore.Web
```

---

## 🚀 Ejecutar Migraciones en el Proyecto

Una vez que las tablas existan en Supabase:

### **1. Desde Visual Studio**
```powershell
# Abre Package Manager Console
# Navega al proyecto FashionStore.Infrastructure

# Crear una migración inicial
Add-Migration InitialCreate

# Aplicar la migración
Update-Database
```

### **2. Desde CLI (.NET)**
```powershell
cd FashionStore.Web

# Crear migración
dotnet ef migrations add InitialCreate --project FashionStore.Infrastructure

# Aplicar migración
dotnet ef database update
```

---

## ✔️ Validar la Conexión

Ejecuta tu aplicación:
```powershell
cd FashionStore.Web
dotnet run
```

Si ves esto en los logs:
```
✅ Database connection successful
```

¡Todo está configurado correctamente! 🎉

---

## 🐛 Solución de Problemas

### Error: "SUPABASE_PASSWORD no encontrado"
- ✅ Asegúrate de que la variable de entorno está configurada
- ✅ Reinicia Visual Studio después de configurarla

### Error: "SSL connection error"
- ✅ Verifica que `SSL Mode=Require` esté en la conexión
- ✅ Supabase requiere conexión SSL

### Error: "Tabla ya existe"
- ✅ Las tablas pueden existir. Ejecuta el script y ignora los errores de "already exists"
- ✅ O elimina manualmente las tablas en SQL Editor antes de ejecutar

### Error de permisos en Supabase
- ✅ Asegúrate de usar el usuario `postgres`
- ✅ Usa la contraseña correcta (la que ingresaste al crear el proyecto)

---

## 📞 Próximos Pasos

1. ✅ Ejecutar el script SQL en Supabase
2. ✅ Configurar la contraseña de Supabase
3. ✅ Compilar el proyecto
4. ✅ Ejecutar la aplicación
5. ✅ Validar que la BD está conectada

¡Necesitas algo más? Pregúntame! 🚀
