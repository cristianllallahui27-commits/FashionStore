# QUICK REFERENCE - Correcciones Fase 1 (Seguridad Crítica)
## FashionStoreSolution - 24-48 horas

---

## 1️⃣ REMOVER CREDENCIALES DE ARCHIVOS JSON

### Archivo: `FashionStore.Web/appsettings.json`

**Antes:**
```json
{
  "SUPABASE_PASSWORD": "MiFer2121092001",
  "ConnectionStrings": {
    "DefaultConnection": "Host=aws-1-us-east-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.bajbvebkmacdnllnxvkv;Password=${SUPABASE_PASSWORD};SSL Mode=Require;Trust Server Certificate=true;Timeout=30;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Después:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=aws-1-us-east-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.bajbvebkmacdnllnxvkv;Password=${SUPABASE_PASSWORD};SSL Mode=Require;Trust Server Certificate=true;Timeout=30;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Archivo: `FashionStore.Web/appsettings.Development.json`

**Antes:**
```json
{
  "SUPABASE_PASSWORD": "MiFer2121092001",
  "ConnectionStrings": {
    "DefaultConnection": "Host=aws-1-us-east-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.bajbvebkmacdnllnxvkv;Password=${SUPABASE_PASSWORD};SSL Mode=Require;Trust Server Certificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**Después:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=aws-1-us-east-2.pooler.supabase.com;Port=5432;Database=postgres;Username=postgres.bajbvebkmacdnllnxvkv;Password=${SUPABASE_PASSWORD};SSL Mode=Require;Trust Server Certificate=true;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

---

## 2️⃣ ACTUALIZAR .GITIGNORE

**Agregar línea:**
```
# Application Settings (contains sensitive data)
appsettings*.json
!appsettings.example.json
```

---

## 3️⃣ REMOVER CAMPO UltimaPasswordAdmin

### Archivo: `FashionStore.Domain/Entities/Vendedor.cs`

**Línea 34 - REMOVER:**
```csharp
/// <summary>Última contraseña asignada por el administrador (visible solo para Admin)</summary>
[StringLength(100)]
public string? UltimaPasswordAdmin { get; set; }
```

**Resultado esperado:** Campo no existe en clase

---

## 4️⃣ REMOVER ASIGNACIÓN EN CONTROLADOR

### Archivo: `FashionStore.Web/Controllers/VendedoresController.cs`

**Línea ~162 - REMOVER:**
```csharp
// Guardar contraseña para visibilidad del administrador
vendedor.UltimaPasswordAdmin = NuevaPassword;
_unitOfWork.Vendedores.Update(vendedor);
```

**Quedar con solo:**
```csharp
// No guardar contraseña plaintext en BD
_unitOfWork.Vendedores.Update(vendedor);  // Vendedor sin cambios
await _unitOfWork.CommitAsync();
```

---

## 5️⃣ REFACTORIZAR DbInitializer

### Archivo: `FashionStore.Infrastructure/Data/DbInitializer.cs`

**Línea 14-17 - REMOVER CONSTANTES:**
```csharp
private const string Admin1Email    = "admin@fashionstore.com";
private const string Admin1Password = "Password123!";    // ← REMOVER
private const string Admin2Email    = "Admin@gmail.com";
private const string Admin2Password = "Admin123!";       // ← REMOVER
```

**REEMPLAZAR CON:**
```csharp
private const string Admin1Email = "admin@fashionstore.com";
private const string Admin2Email = "Admin@gmail.com";

// En método Initialize, ANTES de SeedUser:
var admin1Pwd = Environment.GetEnvironmentVariable("ADMIN1_PASSWORD") 
    ?? GenerateSecurePassword();
var admin2Pwd = Environment.GetEnvironmentVariable("ADMIN2_PASSWORD") 
    ?? GenerateSecurePassword();

logger.LogWarning("⚠️ Admin credentials (save securely): {Email1}:{Pwd1} | {Email2}:{Pwd2}", 
    Admin1Email, admin1Pwd, Admin2Email, admin2Pwd);

await SeedUser(userManager, Admin1Email, admin1Pwd, logger);
await SeedUser(userManager, Admin2Email, admin2Pwd, logger);
```

**AGREGAR MÉTODO:**
```csharp
private static string GenerateSecurePassword()
{
    // Generar contraseña aleatoria: 12 caracteres, A-Z, a-z, 0-9, !@#
    var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#";
    var random = new Random();
    return new string(Enumerable.Range(0, 12)
        .Select(_ => chars[random.Next(chars.Length)])
        .ToArray());
}
```

---

## 6️⃣ AGREGAR AUTORIZACIÓN EN DescuentosController

### Archivo: `FashionStore.Web/Controllers/DescuentosController.cs`

**Línea 38 - AGREGAR:**
```csharp
// GET: Descuentos/Edit/5
[Authorize(Roles = "Administrador")]  // ← AGREGAR ESTA LÍNEA
public async Task<IActionResult> Edit(int? id)
{
    // resto del código
}
```

**Y también en POST:**
```csharp
// POST: Descuentos/Edit/5
[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Administrador")]  // ← AGREGAR AQUÍ TAMBIÉN
public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Tipo,Valor,Activo,FechaCreacion")] DescuentoAutorizado descuentoAutorizado)
{
    // resto del código
}
```

---

## 7️⃣ VALIDACIÓN DE RANGO EN DESCUENTOS

### Archivo: `FashionStore.Web/Controllers/DescuentosController.cs`

**En método Edit POST - AGREGAR VALIDACIÓN:**
```csharp
if (ModelState.IsValid)
{
    // Validar rango de Valor
    if (descuentoAutorizado.Tipo == TipoDescuento.Porcentaje)
    {
        if (descuentoAutorizado.Valor < 0 || descuentoAutorizado.Valor > 100)
        {
            ModelState.AddModelError("Valor", "Porcentaje debe estar entre 0 y 100");
            return View(descuentoAutorizado);
        }
    }
    else if (descuentoAutorizado.Valor < 0)
    {
        ModelState.AddModelError("Valor", "Monto debe ser positivo");
        return View(descuentoAutorizado);
    }
    
    try
    {
        _context.Update(descuentoAutorizado);
        await _context.SaveChangesAsync();
    }
    // resto del código
}
```

---

## 8️⃣ VERIFICACIÓN POST-CAMBIOS

### Paso 1: Compilación
```bash
cd "c:\Users\CRISTIAN\source\repos\FashionStoreSolution"
dotnet clean
dotnet build -c Release
# ✓ Esperado: 0 Errores, 2 Advertencias
```

### Paso 2: Tests
```bash
dotnet test -c Release
# ✓ Esperado: 285/285 tests passed
```

### Paso 3: Verificar Archivos
```bash
# Credenciales NO deben estar
grep "MiFer2121092001" FashionStore.Web/appsettings.json
# ✓ Esperado: Sin resultados

grep "MiFer2121092001" FashionStore.Web/appsettings.Development.json
# ✓ Esperado: Sin resultados

# .gitignore actualizado
grep "appsettings" .gitignore
# ✓ Esperado: Match encontrado

# UltimaPasswordAdmin removido
grep "UltimaPasswordAdmin" FashionStore.Domain/Entities/Vendedor.cs
# ✓ Esperado: Sin resultados

# [Authorize] en DescuentosController
grep -A1 "Edit.*int" FashionStore.Web/Controllers/DescuentosController.cs | grep Authorize
# ✓ Esperado: Match encontrado
```

### Paso 4: Git
```bash
# Verificar que credenciales serán removidas
git status | grep "modified:"
# Deberían aparecer: appsettings.json, appsettings.Development.json, Vendedor.cs, etc.

# Hacer commit
git add .
git commit -m "SECURITY: Remove hardcoded credentials and plaintext passwords

- Remove SUPABASE_PASSWORD from appsettings*.json
- Use environment variables only
- Remove UltimaPasswordAdmin field (plaintext risk)
- Refactor DbInitializer to use env vars for admin passwords
- Add [Authorize] to DescuentosController.Edit()
- Add validation for discount value ranges
- Update .gitignore to exclude appsettings*.json

CRITICAL SECURITY FIXES - DO NOT PUBLISH WITHOUT THIS"
```

---

## ⚙️ ENVIRONMENT VARIABLES (Producción)

### Windows (PowerShell)
```powershell
$env:SUPABASE_PASSWORD = "MiFer2121092001"
$env:ADMIN1_PASSWORD = "Password123!"
$env:ADMIN2_PASSWORD = "Admin123!"

# Verificar
$env:SUPABASE_PASSWORD
$env:ADMIN1_PASSWORD
```

### Linux/Mac (Bash)
```bash
export SUPABASE_PASSWORD="MiFer2121092001"
export ADMIN1_PASSWORD="Password123!"
export ADMIN2_PASSWORD="Admin123!"

# Verificar
echo $SUPABASE_PASSWORD
echo $ADMIN1_PASSWORD
```

### Docker (.env)
```
SUPABASE_PASSWORD=MiFer2121092001
ADMIN1_PASSWORD=Password123!
ADMIN2_PASSWORD=Admin123!
```

---

## ✅ CHECKLIST FINAL

```
REMOVER CREDENCIALES
[ ] appsettings.json - SUPABASE_PASSWORD removido
[ ] appsettings.Development.json - SUPABASE_PASSWORD removido
[ ] .gitignore - appsettings*.json agregado

REMOVER PLAINTEXT PASSWORDS
[ ] Vendedor.cs - UltimaPasswordAdmin removido
[ ] VendedoresController.cs - Asignación removida

REFACTORIZAR INICIALIZACIÓN
[ ] DbInitializer.cs - Constantes removidas
[ ] DbInitializer.cs - Usa env vars (ADMIN1_PASSWORD, ADMIN2_PASSWORD)
[ ] DbInitializer.cs - GenerateSecurePassword() implementado

AUTORIZACIÓN
[ ] DescuentosController.cs - [Authorize] en Edit (GET)
[ ] DescuentosController.cs - [Authorize] en Edit (POST)
[ ] DescuentosController.cs - Validación rango de valores

VALIDACIÓN
[ ] dotnet build -c Release → 0 Errores
[ ] dotnet test -c Release → 285/285 pasados
[ ] grep no encuentra credenciales en archivos
[ ] git status muestra cambios esperados

SEGURIDAD
[ ] Credenciales rotadas en Supabase (MANUAL)
[ ] Environment variables configuradas en servidor
[ ] Git history verificado (sin credenciales antiguas)
```

---

## 📞 SOPORTE RÁPIDO

**"Compilación falla después de cambios"**
```bash
dotnet clean
dotnet restore
dotnet build
```

**"UltimaPasswordAdmin causa error en migraciones"**
→ Crear migración: `dotnet ef migrations add RemoveUltimaPasswordAdmin`

**"Environment variable no funciona"**
→ Verificar: `echo $env:SUPABASE_PASSWORD` (PowerShell)

**"Tests fallan después de remover UltimaPasswordAdmin"**
→ Actualizar test: Remover cualquier referencia a `UltimaPasswordAdmin`

---

**Documento Quick Reference:** Kiro AI  
**Duración Estimada:** 2-3 horas  
**Complejidad:** BAJA  
**Riesgo:** BAJO (cambios localizados)

