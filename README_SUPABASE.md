# 🎯 INSTRUCCIONES RÁPIDAS - FashionStore + Supabase

## 📋 Resumen de lo que se ha preparado

✅ **Script SQL completo** (`supabase_init.sql`)
- Crea todas las tablas necesarias
- Incluye índices y restricciones
- Inserta métodos de pago iniciales

✅ **Configuración de Entity Framework** (actualizado)
- `appsettings.Development.json` configurado
- `Program.cs` listo para conectar con Supabase

✅ **Script PowerShell** (`setup-supabase-env.ps1`)
- Configura la variable de entorno automáticamente

---

## 🚀 Pasos a Ejecutar (EN ORDEN)

### **Paso 1️⃣: Ejecutar el Script de Configuración de Entorno**

Abre PowerShell en la carpeta del proyecto y ejecuta:

```powershell
.\setup-supabase-env.ps1
```

📝 **Te pedirá:** Contraseña de Supabase (usuario `postgres`)

⚠️ **Después:** Reinicia Visual Studio

---

### **Paso 2️⃣: Ejecutar el Script SQL en Supabase**

1. Ve a: https://supabase.com/dashboard
2. Selecciona tu proyecto: **bajbvebkmacdnllnxvkv**
3. Ve a **SQL Editor** → **New Query**
4. Copia TODO el contenido de `supabase_init.sql`
5. Pégalo en el editor
6. Haz clic en ▶️ **Run**
7. Verifica que no haya errores rojos

---

### **Paso 3️⃣: Compila el Proyecto**

En Visual Studio:
```
Ctrl + Shift + B
```

O desde PowerShell:
```powershell
cd FashionStore.Web
dotnet build
```

✅ No debería haber errores de compilación

---

### **Paso 4️⃣: Ejecuta la Aplicación**

En Visual Studio:
```
F5
```

O desde PowerShell:
```powershell
cd FashionStore.Web
dotnet run
```

✅ Deberías ver en los logs:
```
✅ Database connection successful
```

---

## 🔐 Variables de Entorno Configuradas

| Variable | Valor | Dónde |
|----------|-------|-------|
| `SUPABASE_PASSWORD` | Tu contraseña PostgreSQL | Variables de Entorno de Windows |
| `DefaultConnection` | Connection string completo | `appsettings.Development.json` |

---

## 🛠️ Solución Rápida de Problemas

### ❌ "SUPABASE_PASSWORD no encontrado"
```powershell
# Ejecuta nuevamente:
.\setup-supabase-env.ps1

# Luego reinicia Visual Studio
```

### ❌ "SSL connection error"
- Verifica que `SSL Mode=Require` esté en `appsettings.json`
- Supabase REQUIERE conexión segura

### ❌ "Tabla ya existe" en Supabase
- Es normal, significa que ejecutaste el script dos veces
- Las tablas se crearon exitosamente ✅

### ❌ Error de compilación con Npgsql
```powershell
# Limpia y reconstruye:
dotnet clean
dotnet restore
dotnet build
```

---

## 📚 Archivos Importantes

```
C:\Users\CRISTIAN\source\repos\FashionStoreSolution\
├── supabase_init.sql           ← Script SQL para crear tablas
├── setup-supabase-env.ps1      ← Script de configuración
├── GUIA_SUPABASE_SETUP.md      ← Guía detallada (si necesitas más info)
├── .env.example                ← Ejemplo de variables de entorno
└── FashionStore.Web/
	├── appsettings.json                 ← Config general
	└── appsettings.Development.json     ← Config desarrollo (ACTUALIZADO)
```

---

## ✨ Checklist Final

- [ ] Ejecuté `setup-supabase-env.ps1`
- [ ] Reinicié Visual Studio
- [ ] Ejecuté el script SQL en Supabase
- [ ] Compilé el proyecto sin errores
- [ ] Ejecuté la aplicación y funcionó

¿Listo? ¡Vamos a empezar! 🚀

---

## 📞 ¿Necesitas Ayuda?

Si algo no funciona:
1. Revisa `GUIA_SUPABASE_SETUP.md` para más detalles
2. Verifica los logs de la aplicación
3. Comprueba que Supabase está accesible
