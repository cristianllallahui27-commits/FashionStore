# ✅ PASOS DESPUÉS DE CONFIGURAR SUPABASE_PASSWORD

Una vez que hayas ejecutado `.\setup-supabase-env.ps1` y reiniciado Visual Studio, sigue estos pasos:

---

## 1️⃣ COMPILAR EL PROYECTO

```
Ctrl + Shift + B
```

**Resultado esperado:**
```
Build succeeded. 0 Errors
```

Si ves errores:
- Limpia: `dotnet clean`
- Restaura: `dotnet restore`
- Compila: `dotnet build`

---

## 2️⃣ EJECUTAR CON F5

```
F5
```

**El navegador debería abrir:** https://localhost:5001

---

## 3️⃣ VERIFICAR LOGS

En la ventana **Output** de Visual Studio busca estos logs:

### ✅ Logs Esperados (significan conexión exitosa):

```
info: Microsoft.Hosting.Lifetime
  Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime
  Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime
  Application started. Press Ctrl+C to shut down.

info: Microsoft.EntityFrameworkCore.Database.Command
  SELECT u."Email", u."Id", length(u."PasswordHash"::text) AS password_hash_length, 
		 left(u."PasswordHash"::text, 30) AS password_hash_preview
  FROM "AspNetUsers" u
  WHERE lower(u."Email") = lower('Admin@gmail.com');
```

### ❌ Errores Comunes y Soluciones:

```
Error: SUPABASE_PASSWORD requerido
→ Solución: No reiniciaste Visual Studio después de configurar
  Reinicia Visual Studio completamente
```

```
Error: SSL connection error
→ Solución: Verifica que SSL Mode=Require esté en appsettings.json
```

```
Error: Could not connect to postgres
→ Solución: Verifica que Supabase esté disponible
  Intenta: ping db.bajbvebkmacdnllnxvkv.supabase.co
```

---

## 4️⃣ PROBAR LOGIN CON ADMIN

1. En la página inicial, haz clic en **Login** (o ve a `/Identity/Account/Login`)
2. Ingresa:
   - **Email:** Admin@gmail.com
   - **Contraseña:** (la que ingresaste al crear el admin)

**Resultado esperado:**
- ✅ Login exitoso
- ✅ Redirige al dashboard
- ✅ En los logs verás queries SELECT de AspNetUsers

---

## 5️⃣ VERIFICAR CONEXIÓN A SUPABASE

Para confirmar que realmente estás usando Supabase:

### Opción A: Revisar Logs SQL
1. Abre Visual Studio Output
2. Busca "SELECT" en los logs
3. Si ves queries SELECT con nombres de columna en mayúsculas y comillas dobles = ✅ PostgreSQL (Supabase)

### Opción B: Ejecutar SQL en Supabase Dashboard
1. Ve a https://supabase.com/dashboard
2. Tu proyecto → SQL Editor
3. Ejecuta:
```sql
SELECT "Email", "Id", COUNT(*) 
FROM "AspNetUsers" 
GROUP BY "Email", "Id";
```

Si ves Admin@gmail.com en los resultados = ✅ Conexión funciona

### Opción C: Navegar por la Aplicación
1. Intenta crear una nueva categoría
2. En los logs deberías ver:
```
INSERT INTO "Categorias" ("Nombre", "Descripcion")
VALUES (@p0, @p1)
```

---

## 6️⃣ CHECKLIST FINAL

- [ ] Ejecuté `.\setup-supabase-env.ps1`
- [ ] Reinicié Visual Studio
- [ ] Compilé sin errores (Ctrl+Shift+B)
- [ ] Ejecuté F5
- [ ] El navegador se abrió en https://localhost:5001
- [ ] Veo logs de conexión en Output
- [ ] Hice login con Admin@gmail.com
- [ ] Vi queries SQL SELECT en los logs
- [ ] La aplicación funciona correctamente

---

## 7️⃣ PRÓXIMOS PASOS

✅ **Conexión funcionando:**
- Usa la aplicación normalmente
- Las queries se ejecutan en Supabase
- Los datos se guardan en PostgreSQL

✅ **Hacer cambios en la base de datos:**
```powershell
# Crear migración
dotnet ef migrations add MiCambio --project FashionStore.Infrastructure

# Aplicar
dotnet ef database update
```

✅ **Ver todo en Supabase:**
- Dashboard → Table Editor
- Ve todas tus tablas y datos

---

## 📞 ¿ALGO NO FUNCIONA?

1. Revisa `CONFIGURAR_SUPABASE_PASSWORD.md`
2. Revisa `FAQ.md`
3. Verifica logs: `FashionStore.Web/logs/`
4. Comprueba que Supabase esté online

---

**¡Tu FashionStore está conectada a Supabase! 🎉**
