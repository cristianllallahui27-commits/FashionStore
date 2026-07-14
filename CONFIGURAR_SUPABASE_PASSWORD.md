# 🔑 CONFIGURACIÓN CRÍTICA: Variable de Entorno SUPABASE_PASSWORD

## ⚠️ ESTADO ACTUAL: NO CONFIGURADO

La aplicación NO puede conectarse a Supabase sin esta variable.

---

## 🚀 SOLUCIÓN RÁPIDA

### Opción 1: Script Automático (RECOMENDADO)

```powershell
# Abre PowerShell en la carpeta del proyecto y ejecuta:
.\setup-supabase-env.ps1
```

Te pedirá tu contraseña de Supabase (usuario: postgres)
Luego **REINICIA Visual Studio**

---

### Opción 2: Configurar Manualmente (Windows)

```powershell
# Abre PowerShell y ejecuta:
[System.Environment]::SetEnvironmentVariable(
	"SUPABASE_PASSWORD", 
	"tu_contraseña_aqui", 
	[System.EnvironmentVariableTarget]::User
)
```

Reemplaza `tu_contraseña_aqui` con tu contraseña real de Supabase

Luego **REINICIA Visual Studio**

---

### Opción 3: Verificar que se Configuró

```powershell
# Después de cualquier opción anterior, verifica:
$env:SUPABASE_PASSWORD

# Debería mostrar: tu_contraseña
# Si no muestra nada, reinicia PowerShell
```

---

## 📍 ¿DÓNDE OBTENGO MI CONTRASEÑA DE SUPABASE?

1. Ve a: https://supabase.com/dashboard
2. Selecciona tu proyecto: **bajbvebkmacdnllnxvkv**
3. Ve a: **Database** → **Users** → **postgres**
4. Ahí verás tu contraseña (o puedes cambiarla)

---

## ⚡ PASOS COMPLETOS:

### 1️⃣ Configurar Variable
```powershell
.\setup-supabase-env.ps1
# O configurar manualmente
```

### 2️⃣ Reiniciar Visual Studio
```
Cierra Visual Studio completamente
Vuelve a abrirlo
```

### 3️⃣ Compilar
```
Ctrl + Shift + B
```

### 4️⃣ Ejecutar
```
F5
```

### 5️⃣ Verificar en Logs
Deberías ver:
```
Now listening on: https://localhost:5001
SELECT ... FROM "AspNetUsers" ...
```

---

## ✅ CHECKLIST

- [ ] Obtuve mi contraseña de Supabase
- [ ] Ejecuté .\setup-supabase-env.ps1 O configuré manualmente
- [ ] Reinicié Visual Studio
- [ ] Compilé el proyecto
- [ ] Ejecuté F5
- [ ] Veo logs de conexión exitosa

---

## 🎯 PRÓXIMO PASO

👉 **Ejecuta ahora:**
```powershell
.\setup-supabase-env.ps1
```

Luego vuelve aquí cuando esté configurado.
