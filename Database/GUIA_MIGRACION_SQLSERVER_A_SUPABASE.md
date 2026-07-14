# GUÍA: MIGRACIÓN SQL SERVER → SUPABASE
## FashionStoreDB Completa

**Status**: 🔴 EN PROGRESO  
**Clave Supabase**: MiFer2121092001  
**Conexión SQL Server**: ADMIN | FashionStoreDB  

---

## PASO 1: PREPARAR SQL SERVER

### 1.1 Abrir SQL Server Management Studio

```
1. Abre SQL Server Management Studio
2. Server: ADMIN (local)
3. Database: FashionStoreDB
4. Connect
```

### 1.2 Ejecutar Script de Exportación

```sql
-- En SQL Server: Ejecutar EXPORT_SQLSERVER_SCHEMA.sql
-- Copiar resultados de:
-- - Conteos de datos
-- - INSERT statements
```

**Resultado esperado**: Listado de TODAS las tablas con datos

```
=== CONTEOS DE DATOS ===
Categorias:            10
Prendas:               50
Clientes:              25
Vendedores:            8
MetodoPago:            3
Ventas:                100
DetalleVenta:          250
ConfiguracionSistema:  1
AspNetUsers:           5
AspNetRoles:           3
```

---

## PASO 2: CREAR SUPABASE

### 2.1 Acceder a Supabase

```
1. Ve a: https://app.supabase.com
2. Login con tu cuenta
3. Selecciona proyecto (o crea nuevo)
```

### 2.2 Abrir SQL Editor

```
1. Click: SQL Editor
2. New Query
```

### 2.3 Ejecutar Script de Schema

```sql
-- Copiar SUPABASE_SCHEMA_POSTGRESQL.sql completo
-- Pegarlo en SQL Editor
-- Click: RUN
```

**Resultado esperado**: ✅ Todas las tablas creadas sin errores

```
CREATE TABLE
CREATE TABLE
CREATE TABLE
... (8 tablas totales)
CREATE INDEX
CREATE INDEX
... (7 índices)
```

---

## PASO 3: EXPORTAR DATOS DE SQL SERVER

### 3.1 Generar INSERT Statements

**En SQL Server Management Studio**:

```sql
-- Copiar cada tabla una por una
-- Ejemplo: Categorias

SELECT 'INSERT INTO Categorias (Id, Nombre, Descripcion, FechaCreacion) VALUES (' + 
       CAST(Id AS VARCHAR) + ', ''' + ISNULL(Nombre, '') + ''', ''' + ISNULL(Descripcion, '') + ''', ''' + 
       CONVERT(VARCHAR, ISNULL(FechaCreacion, GETDATE()), 121) + ''');'
FROM Categorias;

-- Resultado: Una línea INSERT por cada fila
```

### 3.2 Exportar a Archivo

```
1. Results → Save to file (.sql)
2. Guardar en: Database/datos-export.sql
```

---

## PASO 4: IMPORTAR DATOS EN SUPABASE

### 4.1 Método A: Copy-Paste en SQL Editor

```
1. Ve a Supabase SQL Editor
2. New Query
3. Copia los INSERT statements
4. Pega en el editor
5. RUN

⚠️ NOTA: Si hay muchos datos, dividir en lotes (100 filas c/u)
```

### 4.2 Método B: Usar psql (Recomendado para datos grandes)

```bash
# En Terminal PowerShell

# 1. Descargar psql (PostgreSQL client)
# Ve a: https://www.postgresql.org/download/windows/

# 2. Conectar a Supabase
$env:PGPASSWORD="MiFer2121092001"
psql -h db.bajbvebkmacdnllnxvkv.supabase.co -U postgres -d postgres -f Database/datos-export.sql

# 3. Ingresar contraseña: MiFer2121092001
```

### 4.3 Método C: Usar DBeaver (GUI)

```
1. Abre DBeaver (descarga gratis)
2. New Database Connection → PostgreSQL
3. Host: db.bajbvebkmacdnllnxvkv.supabase.co
4. Port: 5432
5. Database: postgres
6. User: postgres
7. Password: MiFer2121092001
8. Conecta
9. File → Import → Database → datos-export.sql
10. Execute
```

---

## PASO 5: VALIDAR MIGRACIÓN

### 5.1 Verificar Tablas en Supabase

**En Supabase SQL Editor**:

```sql
-- Listar todas las tablas
SELECT * FROM information_schema.tables 
WHERE table_schema = 'public' 
ORDER BY table_name;

-- Resultado esperado: 8 tablas
-- - Categorias
-- - Prendas
-- - Clientes
-- - Vendedores
-- - MetodoPago
-- - Ventas
-- - DetalleVenta
-- - ConfiguracionSistema
```

### 5.2 Contar Datos Migrados

```sql
-- Verificar que TODOS los datos están

SELECT 'Categorias' AS tabla, COUNT(*) AS registros FROM public.Categorias
UNION ALL
SELECT 'Prendas', COUNT(*) FROM public.Prendas
UNION ALL
SELECT 'Clientes', COUNT(*) FROM public.Clientes
UNION ALL
SELECT 'Vendedores', COUNT(*) FROM public.Vendedores
UNION ALL
SELECT 'MetodoPago', COUNT(*) FROM public.MetodoPago
UNION ALL
SELECT 'Ventas', COUNT(*) FROM public.Ventas
UNION ALL
SELECT 'DetalleVenta', COUNT(*) FROM public.DetalleVenta
UNION ALL
SELECT 'ConfiguracionSistema', COUNT(*) FROM public.ConfiguracionSistema;

-- Resultado esperado: Mismos números que SQL Server
```

### 5.3 Validar Foreign Keys

```sql
-- Verificar integridad referencial

-- Ventas sin cliente válido
SELECT * FROM public.Ventas WHERE "ClienteId" NOT IN (SELECT "Id" FROM public.Clientes);
-- Resultado: 0 filas

-- DetalleVenta sin venta válida
SELECT * FROM public.DetalleVenta WHERE "VentaId" NOT IN (SELECT "Id" FROM public.Ventas);
-- Resultado: 0 filas

-- Prendas sin categoría válida
SELECT * FROM public.Prendas WHERE "CategoriaId" NOT IN (SELECT "Id" FROM public.Categorias);
-- Resultado: 0 filas
```

---

## PASO 6: ACTUALIZAR APLICACIÓN

### 6.1 Cambiar Connection String

**En appsettings.json**:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=db.bajbvebkmacdnllnxvkv.supabase.co;Port=5432;Database=postgres;Username=postgres;Password=${SUPABASE_PASSWORD};SSL Mode=Require;Trust Server Certificate=false;",
    "SqlServerConnection": "Server=ADMIN;Database=FashionStoreDB;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "DatabaseProvider": "PostgreSQL"
}
```

### 6.2 Configurar Variable de Entorno

**PowerShell como Admin**:

```powershell
[Environment]::SetEnvironmentVariable("SUPABASE_PASSWORD", "MiFer2121092001", "User")
```

**Luego**: Cierra y reabre VS Code

### 6.3 Verificar Conexión

```bash
cd FashionStore.Web
dotnet run

# Esperado: "Now listening on: http://localhost:5100"
# Si hay error de BD, revisar logs
```

---

## PASO 7: PRUEBA FUNCIONAL

### 7.1 Acceder a la Aplicación

```
1. Abre http://localhost:5100
2. Login: admin@fashionstore.com / Password123!
3. Navega a cada módulo:
   - Prendas: Debe mostrar 50 productos
   - Clientes: Debe mostrar 25 clientes
   - Vendedores: Debe mostrar 8 vendedores
   - Ventas: Debe mostrar 100 transacciones
```

### 7.2 Crear Nueva Venta (Test de Escritura)

```
1. Click: Ventas → Nueva Venta (POS)
2. Selecciona cliente, vendedor, prendas
3. Click: Guardar
4. ✅ Debe guardar en Supabase exitosamente
```

### 7.3 Verificar en Supabase

```sql
-- En Supabase SQL Editor
SELECT * FROM public.Ventas ORDER BY "Fecha" DESC LIMIT 1;

-- Debe mostrar la venta que creaste
```

---

## TROUBLESHOOTING

### Problema: "Connection refused"

```
Solución:
1. Verificar SUPABASE_PASSWORD está configurada
2. Verificar host/puerto correcto
3. Verificar credenciales (usuario: postgres)
4. Revisar firewall
```

### Problema: "Foreign key constraint failed"

```
Solución:
1. Verificar que datos referenciados existen
2. Ejecutar validación de integridad
3. Revisar orden de inserción (tablas padre primero)
```

### Problema: "Duplicate key value violates unique constraint"

```
Solución:
1. Verificar que Primary Keys no duplicados
2. Resetear sequences:
   SELECT setval('public.Categorias_Id_seq', (SELECT MAX("Id") FROM public.Categorias));
   SELECT setval('public.Prendas_Id_seq', (SELECT MAX("Id") FROM public.Prendas));
   -- Repetir para cada tabla
```

### Problema: "Type conversion error"

```
Solución:
1. Revisar tipos de datos SQL Server vs PostgreSQL
2. Converter fechas al formato TIMESTAMP
3. Converter booleans: 1 → TRUE, 0 → FALSE
```

---

## CHECKLIST FINAL

```bash
✅ PREPARACIÓN
   ✓ SQL Server conectado
   ✓ Datos contados en SQL Server

✅ SUPABASE
   ✓ Proyecto creado
   ✓ Schema creado
   ✓ Índices creados

✅ MIGRACIÓN
   ✓ INSERT statements generados
   ✓ Datos importados en Supabase
   ✓ Sin errores

✅ VALIDACIÓN
   ✓ Tablas verificadas (8 totales)
   ✓ Datos completos (conteos iguales)
   ✓ Integridad referencial OK
   ✓ Sequences resetadas

✅ APLICACIÓN
   ✓ Connection string actualizado
   ✓ Variable de entorno configurada
   ✓ App se conecta exitosamente
   ✓ Datos visibles en UI

✅ TEST
   ✓ Nueva venta creada
   ✓ Venta grabada en Supabase
   ✓ Todo funciona correctamente
```

---

## ARCHIVOS DE REFERENCIA

```
Database/
├── EXPORT_SQLSERVER_SCHEMA.sql (cómo exportar de SQL Server)
├── SUPABASE_SCHEMA_POSTGRESQL.sql (schema para Supabase)
├── datos-export.sql (generado después de exportar)
└── GUIA_MIGRACION_SQLSERVER_A_SUPABASE.md (este archivo)
```

---

## RESUMEN RÁPIDO

```
1. SQL Server: Ejecutar EXPORT_SQLSERVER_SCHEMA.sql
2. Copiar INSERT statements
3. Supabase: Ejecutar SUPABASE_SCHEMA_POSTGRESQL.sql
4. Supabase: Pegar INSERT statements
5. Validar: Contar datos y verificar FK
6. App: Actualizar connection string y env var
7. Test: Nueva venta → Guardar → Verificar Supabase
8. ✅ Listo para Fase 1
```

---

**Tiempo Total**: 30-60 minutos  
**Dificultad**: Media (SQL + BD)  
**Status**: 🔴 POR EJECUTAR  

**Próximo paso**: Ejecutar PASO 1 ahora.
