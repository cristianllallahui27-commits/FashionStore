# 🔧 FIX: Agregar Columna UsuarioId a Vendedores

## ⚠️ Problema
```
PostgresException: 42703: column v.UsuarioId does not exist
```

**Causa**: La migration `AddUsuarioIdToVendedor` no se aplicó a la BD PostgreSQL (Supabase).

---

## ✅ Solución Rápida (3 pasos)

### Opción A: Supabase SQL Editor (RECOMENDADO)

1. **Abrir Supabase Dashboard**
   - Ir a: https://supabase.com/dashboard
   - Seleccionar tu proyecto
   - Menú izquierdo: **SQL Editor**

2. **Ejecutar SQL**
   - Copiar contenido de `MANUAL_FIX_USUARIOID.sql` (en raíz del proyecto)
   - Pegar en SQL Editor
   - Hacer clic **Run**
   - ✓ Debe completarse sin errores

3. **Verificar**
   ```sql
   -- Ejecutar para confirmar que la columna existe:
   SELECT column_name, data_type FROM information_schema.columns 
   WHERE table_name = 'Vendedores' AND column_name = 'UsuarioId';
   ```
   - ✓ Debe retornar 1 fila con `UsuarioId` y tipo `text`

---

### Opción B: pgAdmin (Si tienes instalado)

1. Conectar a tu BD PostgreSQL
2. Navegar a: Schemas > public > Tables > Vendedores
3. Right-click > Scripts > Modify
4. Agregar columna:
   ```sql
   ALTER TABLE "Vendedores" 
   ADD COLUMN IF NOT EXISTS "UsuarioId" text;
   ```
5. Ejecutar

---

### Opción C: DBeaver (Si tienes instalado)

1. Conectar a tu BD
2. Navegar a tabla Vendedores
3. Right-click > Modify Table
4. Pestaña **Columns** > New
5. Nombre: `UsuarioId`
6. Tipo: `text`
7. Nullable: ✓ SÍ
8. Guardar

---

## 📋 SQL Manual Completo

```sql
-- 1. Agregar columna si no existe
ALTER TABLE "Vendedores" 
ADD COLUMN IF NOT EXISTS "UsuarioId" text;

-- 2. Registrar migration en historial (importante)
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260713065647_AddUsuarioIdToVendedor', '9.0.0')
ON CONFLICT ("MigrationId") DO NOTHING;

-- 3. Verificar
SELECT * FROM "Vendedores" LIMIT 1;
-- Debe mostrar columna UsuarioId
```

---

## 🔄 Después de Aplicar el Fix

### 1. Reiniciar Aplicación
```powershell
# Si está corriendo, presionar Ctrl+C

# Volver a ejecutar
$env:SUPABASE_PASSWORD='MiFer2121092001'
dotnet run --configuration Release
```

### 2. Verificar que Funciona
```
GET http://localhost:5100/Ventas/Create
```
- ✓ Debe cargar sin error
- ✓ Dropdown de métodos de pago debe tener 5 opciones

### 3. Probar Flujo Completo
1. Admin inicia sesión
2. Ventas > Nueva Venta
3. Verificar que carga correctamente
4. Seleccionar cliente y productos
5. Registrar venta
6. ✓ Debe funcionar sin errores

---

## 📍 Ubicación de Archivos

- **MANUAL_FIX_USUARIOID.sql** ← SQL a ejecutar
- **FIX_COLUMN_USUARIOID.md** ← Este documento
- **PLAN_CORRECCION_TECNICA.md** ← Documentación técnica
- **RESUMEN_CORRECCIONES_FASE1.md** ← Detalles de cambios
- **IMPLEMENTACION_COMPLETADA.md** ← Guía general

---

## ❓ Preguntas Frecuentes

**P: ¿Por qué no se aplicó la migration automáticamente?**  
R: Porque el ambiente no tiene conexión a internet, así que `dotnet ef database update` no pudo conectar a Supabase.

**P: ¿Es seguro ejecutar el SQL manual?**  
R: Sí, es completamente seguro. Solo agrega una columna nullable (vacía inicialmente).

**P: ¿Qué pasa si la columna ya existe?**  
R: La claúsula `IF NOT EXISTS` evita errores. No pasa nada si ya está.

**P: ¿Necesito hacer algo más después?**  
R: Solo reiniciar la app. El sistema funcionará normalmente.

---

## ✅ Checklist

- [ ] Abrir Supabase Dashboard
- [ ] Ir a SQL Editor
- [ ] Copiar SQL de MANUAL_FIX_USUARIOID.sql
- [ ] Ejecutar SQL
- [ ] Verificar que completa sin errores
- [ ] Reiniciar aplicación
- [ ] Probar Ventas > Nueva Venta
- [ ] ✓ Sistema funciona correctamente

---

**Tiempo estimado**: 2 minutos  
**Dificultad**: Muy fácil  
**Riesgo**: Nulo (solo agregar columna nullable)
