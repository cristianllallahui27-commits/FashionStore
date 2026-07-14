# 🎯 PASOS VISUALES - Guía Paso a Paso

## PASO 1: Configurar Variable de Entorno
```
┌─────────────────────────────────────────────────────────────┐
│  ABRE: PowerShell (en la carpeta del proyecto)              │
│                                                             │
│  EJECUTA:                                                   │
│  .\setup-supabase-env.ps1                                  │
│                                                             │
│  INGRESA: Tu contraseña Supabase (usuario: postgres)        │
│                                                             │
│  RESULTADO: ✅ SUPABASE_PASSWORD configurado               │
│                                                             │
│  IMPORTANTE: Reinicia Visual Studio                        │
└─────────────────────────────────────────────────────────────┘
```

**Captura de pantalla esperada:**
```
========================================
FashionStore - Configuración de Supabase
========================================

Necesitamos tu contraseña de Supabase para configurar la conexión.
📌 La encontrarás en: https://supabase.com/dashboard
   Usuario: postgres

Ingresa tu contraseña: ••••••••••••

⚙️  Configurando variable de entorno...
✅ SUPABASE_PASSWORD configurado exitosamente

📝 Próximos pasos:
   1. ⚠️  Reinicia Visual Studio
   2. 🚀 Abre la solución FashionStoreSolution
```

---

## PASO 2: Ejecutar Script SQL en Supabase
```
┌──────────────────────────────────────────────────────────────────┐
│ 1. ABRE: https://supabase.com/dashboard                          │
│                                                                  │
│ 2. SELECCIONA: Proyecto "bajbvebkmacdnllnxvkv"                 │
│                                                                  │
│ 3. VE A: SQL Editor (menú izquierdo)                           │
│                                                                  │
│ 4. HAZ CLIC: New Query                                          │
│                                                                  │
│ 5. COPIA: Todo el contenido de "supabase_init.sql"            │
│    (Ctrl+A → Ctrl+C en el editor de texto)                    │
│                                                                  │
│ 6. PEGA: En el editor SQL de Supabase (Ctrl+V)                │
│                                                                  │
│ 7. HAZ CLIC: ▶️ RUN (botón azul)                              │
│                                                                  │
│ 8. ESPERA: Hasta que termine (10-20 segundos)                 │
│                                                                  │
│ RESULTADO: ✅ Todas las tablas creadas                         │
└──────────────────────────────────────────────────────────────────┘
```

**Captura esperada en Supabase:**
```
SQL Editor

┌─ File ────────────────────────────────────────────────────────┐
│                                                                 │
│ -- =============================================================================
│ -- FASHIONSTORE - Script de Creación de Tablas para Supabase (PostgreSQL)
│ -- =============================================================================
│ 
│ -- ===== 1. TABLA: Categorias =====
│ CREATE TABLE IF NOT EXISTS "Categorias" (
│     "Id" SERIAL PRIMARY KEY,
│     ...
│
│                                      [▶️ RUN]    [Format]    [Save]
│
└─────────────────────────────────────────────────────────────────┘

✅ Query executed successfully (16 tables created)
```

---

## PASO 3: Compilar el Proyecto
```
┌──────────────────────────────────────────────────────────────────┐
│ OPCIÓN A: Visual Studio                                          │
│ ┌────────────────────────────────────────────────────────────┐   │
│ │ Presiona: Ctrl + Shift + B                                 │   │
│ │                                                            │   │
│ │ O ve a: Build → Build Solution                           │   │
│ │                                                            │   │
│ │ RESULTADO: ✅ Build succeeded                             │   │
│ └────────────────────────────────────────────────────────────┘   │
│                                                                   │
│ OPCIÓN B: PowerShell                                             │
│ ┌────────────────────────────────────────────────────────────┐   │
│ │ cd C:\Users\CRISTIAN\source\repos\FashionStoreSolution    │   │
│ │ dotnet build                                              │   │
│ │                                                            │   │
│ │ RESULTADO: ✅ Build succeeded. 0 Errors, 0 Warnings      │   │
│ └────────────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────────┘
```

---

## PASO 4: Ejecutar la Aplicación
```
┌──────────────────────────────────────────────────────────────────┐
│ OPCIÓN A: Visual Studio                                          │
│ ┌────────────────────────────────────────────────────────────┐   │
│ │ Presiona: F5                                               │   │
│ │                                                            │   │
│ │ O ve a: Debug → Start Debugging                          │   │
│ │                                                            │   │
│ │ RESULTADO: ✅ Navegador abre: https://localhost:5001      │   │
│ └────────────────────────────────────────────────────────────┘   │
│                                                                   │
│ OPCIÓN B: PowerShell                                             │
│ ┌────────────────────────────────────────────────────────────┐   │
│ │ cd FashionStore.Web                                       │   │
│ │ dotnet run                                                │   │
│ │                                                            │   │
│ │ Logs esperados:                                           │   │
│ │ info: Microsoft.Hosting.Lifetime                          │   │
│ │ Now listening on: https://localhost:5001                 │   │
│ │                                                            │   │
│ │ RESULTADO: ✅ Abre: https://localhost:5001               │   │
│ └────────────────────────────────────────────────────────────┘   │
└──────────────────────────────────────────────────────────────────┘
```

**Logs esperados:**
```
info: Microsoft.Hosting.Lifetime
  Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime
  Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime
  Application started. Press Ctrl+C to shut down.

✅ Aplicación ejecutándose!
```

---

## PASO 5: Verificar Conexión a Supabase
```
┌──────────────────────────────────────────────────────────────────┐
│ 1. ABRE: https://localhost:5001                                 │
│                                                                  │
│ 2. VE A: Cualquier página que use la base de datos             │
│    (ej: Categorías, Prendas, Clientes)                        │
│                                                                  │
│ 3. BUSCA en los LOGS: "SELECT... FROM..."                      │
│                                                                  │
│ 4. SI VES: Queries SQL = ✅ Conexión exitosa                   │
│                                                                  │
│ 5. SI VES ERROR: Revisa PASO 1 (variable de entorno)          │
│                                                                  │
│ RESULTADO ESPERADO:                                             │
│ ✅ Página se carga                                              │
│ ✅ Datos se cargan desde Supabase                              │
│ ✅ Sin errores de conexión                                      │
└──────────────────────────────────────────────────────────────────┘
```

**Logs en Visual Studio:**
```
SELECT "c"."Id", "c"."Nombre", "c"."Descripcion" 
FROM "Categorias" AS "c"

SELECT "p"."Id", "p"."Nombre", "p"."Precio", "p"."CategoriaId"
FROM "Prendas" AS "p"

✅ Queries ejecutadas correctamente en Supabase!
```

---

## ✅ CHECKLIST FINAL

```
┌───────────────────────────────────────────────────┐
│ Paso 1: Variable de Entorno                       │
│ ☐ Ejecuté setup-supabase-env.ps1                 │
│ ☐ Ingresé mi contraseña de Supabase              │
│ ☐ Reinicié Visual Studio                         │
│                                                   │
│ Paso 2: Script SQL                                │
│ ☐ Copié el contenido de supabase_init.sql        │
│ ☐ Lo pegué en SQL Editor de Supabase             │
│ ☐ Ejecuté el script (Run)                        │
│ ☐ No hay errores rojos                           │
│                                                   │
│ Paso 3: Compilación                               │
│ ☐ Presioné Ctrl+Shift+B (Build)                  │
│ ☐ Sin errores de compilación                     │
│                                                   │
│ Paso 4: Ejecución                                 │
│ ☐ Presioné F5 (Debug)                            │
│ ☐ El navegador se abre automáticamente           │
│ ☐ Página se carga sin errores                    │
│                                                   │
│ Paso 5: Verificación                              │
│ ☐ Navego a una página con datos                  │
│ ☐ Los datos se cargan desde Supabase             │
│ ☐ Veo queries SQL en los logs                    │
│                                                   │
│ ✅ TODO COMPLETADO - ¡LISTO PARA USAR!          │
└───────────────────────────────────────────────────┘
```

---

## 🎯 Casos Comunes

### ❌ Problema: "SUPABASE_PASSWORD no encontrado"
```
┌─────────────────────────────────────────────────────────────┐
│ SOLUCIÓN:                                                   │
│ 1. Abre PowerShell                                          │
│ 2. .\setup-supabase-env.ps1                               │
│ 3. Ingresa tu contraseña nuevamente                        │
│ 4. Reinicia Visual Studio                                 │
└─────────────────────────────────────────────────────────────┘
```

### ❌ Problema: "SSL connection error"
```
┌─────────────────────────────────────────────────────────────┐
│ VERIFICAR:                                                  │
│ ✅ appsettings.json tenga "SSL Mode=Require"             │
│ ✅ Supabase esté disponible (ping host)                   │
│ ✅ Contraseña sea correcta                                │
│ ✅ No haya firewall bloqueando puerto 5432                │
└─────────────────────────────────────────────────────────────┘
```

### ❌ Problema: "Tabla no existe"
```
┌─────────────────────────────────────────────────────────────┐
│ VERIFICAR:                                                  │
│ 1. ¿Ejecuté el script SQL? (PASO 2)                       │
│ 2. ¿Sin errores rojos?                                    │
│ 3. ¿En Supabase veo las tablas?                          │
│    Dashboard → Table Editor → ver listado                │
│ 4. Si no: ejecuta el script nuevamente                    │
└─────────────────────────────────────────────────────────────┘
```

---

## 📞 Ayuda Rápida

| Problema | Solución |
|----------|----------|
| "Password required" | Ejecuta `setup-supabase-env.ps1` |
| Build fails | `dotnet clean && dotnet restore && dotnet build` |
| Tabla no existe | Ejecuta `supabase_init.sql` en Supabase |
| Conexión rechazada | Verifica IP de Supabase en firewall |
| Logs no aparecen | Revisa `FashionStore.Web/logs/` |

---

## ✨ ¡HECHO! 🎉

Sigue estos 5 pasos y tu FashionStore estará conectado a Supabase.

**Próximo:** Lee `README_SUPABASE.md` para detalles adicionales.
