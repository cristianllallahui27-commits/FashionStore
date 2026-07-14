# 🚀 INSTALA SUPABASE AHORA - PASO A PASO

**Tiempo**: 5 minutos  
**Dificultad**: Muy fácil  

---

## ✅ YA ESTÁ HECHO

- ✅ `.vscode/launch.json` - F5 sin debugger
- ✅ `Program.cs` - Soporte PostgreSQL
- ✅ `appsettings.json` - Connection string Supabase
- ✅ `FashionStore.Infrastructure.csproj` - Npgsql agregado
- ✅ Build compilado sin errores

---

## 📋 TU INFORMACIÓN SUPABASE

```
Host: db.bajbvebkmacdnllnxvkv.supabase.co
Puerto: 5432
Base de datos: postgres
Usuario: postgres
Contraseña: [NECESITO ESTA]
```

---

## 🔧 PASO 1: Guardar Contraseña (2 min)

### Opción A: Variable de Entorno (SEGURA - Recomendado)

Abre **PowerShell como Administrador** y ejecuta:

```powershell
[Environment]::SetEnvironmentVariable("SUPABASE_PASSWORD", "TU_CONTRASEÑA_AQUI", "User")
```

Reemplaza `TU_CONTRASEÑA_AQUI` con tu contraseña real.

**Después**: Cierra y reabre VS Code.

### Opción B: Archivo .env (Local)

1. Copia `.env.example` a `.env`:
```bash
copy .env.example .env
```

2. Edita `.env`:
```
SUPABASE_PASSWORD=TU_CONTRASEÑA_AQUI
```

3. Guarda el archivo.

---

## 📊 PASO 2: Exportar Schema de SQL Server (2 min)

Abre **SQL Server Management Studio**:

1. Conecta a `Server=ADMIN; Database=FashionStoreDB`
2. En el Object Explorer, RIGHT-CLICK en **FashionStoreDB**
3. Selecciona **Tasks > Generate Scripts**
4. Sigue el wizard:
   - Select All Tables
   - Options: Script all objects
   - Save to file: `schema.sql`

5. Guarda ese archivo en: `c:\Users\CRISTIAN\source\repos\FashionStoreSolution\schema.sql`

---

## 🗄️ PASO 3: Restaurar Schema en Supabase (2 min)

1. Abre [https://app.supabase.com](https://app.supabase.com)
2. Login
3. Ve al proyecto FashionStore
4. Haz click en **SQL Editor** (lado izquierdo)
5. Haz click en **New Query**
6. Copia el contenido del archivo `schema.sql` que generaste
7. Pega en el editor
8. Haz click en **Run** (botón azul, abajo)
9. Espera a que termine sin errores

---

## 🚀 PASO 4: Ejecutar Aplicación (1 min)

### Opción A: F5 (Sin Debugger)

1. Abre VS Code
2. Presiona **F5**
3. Selecciona **"Run (No Debug)"** (si se pregunta)
4. Espera a que compile
5. ✅ **Se abrirá automáticamente en http://localhost:5100**

### Opción B: Terminal

```bash
cd c:\Users\CRISTIAN\source\repos\FashionStoreSolution\FashionStore.Web
dotnet run
```

Abre http://localhost:5100

---

## 🔐 PASO 5: Verifica que Funciona (1 min)

1. Abre http://localhost:5100
2. ✅ No hay error de debugger
3. Haz **Login**:
   - Email: `admin@fashionstore.com`
   - Contraseña: `Password123!`
4. ✅ Ves el **menú completo**:
   - Inicio
   - Catálogo
   - Admin
   - Ventas
   - Perfil
5. Prueba navegar a una sección (ej: Prendas)
6. ✅ Los datos se cargan desde Supabase

---

## ✅ LISTO

Si llegaste aquí:

✅ F5 funciona sin error vsdbg  
✅ App conectada a Supabase PostgreSQL  
✅ BD completamente migrada  
✅ Menú y datos operativos  

---

## 🆘 SI ALGO FALLA

### "Unable to connect to database"

```
Timeout / Connection refused
```

**Soluciones**:
1. Verifica que SUPABASE_PASSWORD está en variables de entorno
2. Reinicia VS Code
3. Verifica que la contraseña es correcta (sin espacios)

### "F5 aún muestra error del debugger"

1. Cierra VS Code completamente
2. Abre de nuevo
3. Presiona F5

### "Menú no aparece después de login"

Esto ya está reparado. Si aún falla:
1. Limpia cookies del navegador (Ctrl+Shift+Delete)
2. Recarga la página

---

## 📞 SOPORTE

Archivos importantes:
- `MIGRACION_SUPABASE_GUIA.md` - Guía técnica completa
- `EXPORT_SQL_SERVER_SCHEMA.sql` - Cómo exportar BD
- `appsettings.json` - Connection string
- `.vscode/launch.json` - Configuración F5

---

## 🎉 PRÓXIMO PASO

**Ahora mismo**:
1. Abre PowerShell como Admin
2. Ejecuta el comando SetEnvironmentVariable
3. Abre SQL Server y exporta schema
4. Pega en Supabase
5. Presiona F5

**¡Listo! Tu app está en la nube.** 🚀
