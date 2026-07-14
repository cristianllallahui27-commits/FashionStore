# ✅ CHECKLIST MIGRACIÓN SQL SERVER → SUPABASE
## FashionStoreDB Completa

**Estado**: 🔴 POR EJECUTAR  
**Tiempo**: 30-60 minutos  
**Dificultad**: Media  

---

## 📋 CHECKLIST PRE-MIGRACIÓN

### Verificaciones Iniciales
- [ ] SQL Server conectado y funcionando
- [ ] Base de datos FashionStoreDB accesible
- [ ] Supabase proyecto creado en app.supabase.com
- [ ] Credenciales a mano:
  - Host: db.bajbvebkmacdnllnxvkv.supabase.co
  - Usuario: postgres
  - Clave: MiFer2121092001

### Archivos Preparados
- [ ] Database/EXPORT_SQLSERVER_SCHEMA.sql ✅
- [ ] Database/SUPABASE_SCHEMA_POSTGRESQL.sql ✅
- [ ] Database/GUIA_MIGRACION_SQLSERVER_A_SUPABASE.md ✅

---

## 🔴 PASO 1: EXPORTAR DATOS DE SQL SERVER (10 minutos)

### Substep 1.1: Abrir SQL Server Management Studio
- [ ] Abre SQL Server Management Studio
- [ ] Conecta a: Server = ADMIN
- [ ] Selecciona Database = FashionStoreDB
- [ ] Click: Connect

### Substep 1.2: Ejecutar Script de Exportación
- [ ] En SQL Server, abre archivo: Database/EXPORT_SQLSERVER_SCHEMA.sql
- [ ] Ejecuta el script completo (F5)
- [ ] Espera resultados

### Substep 1.3: Verificar Conteos
- [ ] Busca en resultados: "=== CONTEOS DE DATOS ==="
- [ ] Anota los números:
  - Categorias: _____ (registros)
  - Prendas: _____
  - Clientes: _____
  - Vendedores: _____
  - MetodoPago: _____
  - Ventas: _____
  - DetalleVenta: _____
  - ConfiguracionSistema: _____

### Substep 1.4: Copiar INSERT Statements
- [ ] En SQL Server, busca sección: "-- 5. SCRIPT PARA EXPORTAR DATOS"
- [ ] Copia TODO el contenido de esa sección
- [ ] Guarda en archivo: Database/datos-sql-server.sql
- [ ] Verifica que NO esté vacío

---

## 🟡 PASO 2: CREAR SCHEMA EN SUPABASE (10 minutos)

### Substep 2.1: Acceder a Supabase
- [ ] Ve a: https://app.supabase.com
- [ ] Login con tu cuenta
- [ ] Selecciona tu proyecto FashionStore
- [ ] Click: SQL Editor

### Substep 2.2: Ejecutar Schema
- [ ] Click: "New Query"
- [ ] Abre archivo: Database/SUPABASE_SCHEMA_POSTGRESQL.sql
- [ ] Copia TODO el contenido
- [ ] Pégalo en el editor de Supabase
- [ ] Click: RUN (botón azul abajo a la derecha)
- [ ] Espera a que termine

### Substep 2.3: Verificar Creación
- [ ] Busca en resultados: "CREATE TABLE" (debe haber 8)
- [ ] Busca: "CREATE INDEX" (debe haber 7)
- [ ] NO debe haber errores en rojo
- [ ] Click: Table Editor (lado izquierdo) para ver tablas

---

## 🟠 PASO 3: IMPORTAR DATOS EN SUPABASE (15-30 minutos)

### Opción A: Copy-Paste Simple (Datos Pequeños)

#### Substep 3A.1: Preparar Datos
- [ ] Abre archivo: Database/datos-sql-server.sql
- [ ] Selecciona TODO (Ctrl+A)
- [ ] Copia (Ctrl+C)

#### Substep 3A.2: Pegar en Supabase
- [ ] Ve a Supabase SQL Editor
- [ ] Click: "New Query"
- [ ] Pega datos (Ctrl+V)
- [ ] Click: RUN
- [ ] ⚠️ Si hay error, puede ser por muchas filas (ir a Opción B)

### Opción B: Dividir en Lotes (Datos Grandes)

#### Substep 3B.1: Dividir Datos
- [ ] Abre Database/datos-sql-server.sql
- [ ] Separa por tabla:
  - INSERT Categorias (primero)
  - INSERT Prendas
  - INSERT Clientes
  - INSERT Vendedores
  - INSERT MetodoPago
  - INSERT Ventas
  - INSERT DetalleVenta

#### Substep 3B.2: Importar Tabla por Tabla
- [ ] En Supabase SQL Editor:
- [ ] Query 1: Pega INSERT de Categorias → RUN ✓
- [ ] Query 2: Pega INSERT de Prendas → RUN ✓
- [ ] Query 3: Pega INSERT de Clientes → RUN ✓
- [ ] Query 4: Pega INSERT de Vendedores → RUN ✓
- [ ] Query 5: Pega INSERT de MetodoPago → RUN ✓
- [ ] Query 6: Pega INSERT de Ventas → RUN ✓
- [ ] Query 7: Pega INSERT de DetalleVenta → RUN ✓
- [ ] Query 8: Pega INSERT de ConfiguracionSistema → RUN ✓

### Opción C: Usar psql (Terminal - Más Rápido)

#### Substep 3C.1: Configurar psql
- [ ] Descarga PostgreSQL client desde: https://www.postgresql.org/download/windows/
- [ ] Instala psql
- [ ] Abre PowerShell

#### Substep 3C.2: Conectar a Supabase
```powershell
# Copia y ejecuta en PowerShell:
$env:PGPASSWORD="MiFer2121092001"
psql -h db.bajbvebkmacdnllnxvkv.supabase.co -U postgres -d postgres -f "Database/datos-sql-server.sql"
```
- [ ] Ingresa clave cuando pida: MiFer2121092001
- [ ] Espera a que termine (Sin errores)

---

## 🟢 PASO 4: VALIDAR MIGRACIÓN (10 minutos)

### Substep 4.1: Contar Datos Migrados
- [ ] En Supabase SQL Editor → New Query
- [ ] Copia y ejecuta:

```sql
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
```

### Substep 4.2: Comparar Conteos
- [ ] Compara resultados con lo anotado en PASO 1:
  - [ ] Categorias: _____ (Supabase) vs _____ (SQL Server) ✓
  - [ ] Prendas: _____ vs _____ ✓
  - [ ] Clientes: _____ vs _____ ✓
  - [ ] Vendedores: _____ vs _____ ✓
  - [ ] MetodoPago: _____ vs _____ ✓
  - [ ] Ventas: _____ vs _____ ✓
  - [ ] DetalleVenta: _____ vs _____ ✓
  - [ ] ConfiguracionSistema: _____ vs _____ ✓

### Substep 4.3: Validar Integridad
- [ ] En Supabase SQL Editor, ejecuta:

```sql
-- Verificar foreign keys
SELECT * FROM public.Ventas WHERE "ClienteId" NOT IN (SELECT "Id" FROM public.Clientes);
-- Resultado esperado: 0 filas
```

- [ ] Resultado: 0 filas ✓

```sql
-- Verificar detalles de venta
SELECT * FROM public.DetalleVenta WHERE "VentaId" NOT IN (SELECT "Id" FROM public.Ventas);
-- Resultado esperado: 0 filas
```

- [ ] Resultado: 0 filas ✓

```sql
-- Verificar prendas
SELECT * FROM public.Prendas WHERE "CategoriaId" NOT IN (SELECT "Id" FROM public.Categorias);
-- Resultado esperado: 0 filas
```

- [ ] Resultado: 0 filas ✓

---

## 🔵 PASO 5: CONFIGURAR APLICACIÓN (5 minutos)

### Substep 5.1: Guardar Contraseña en Variable de Entorno
- [ ] Abre PowerShell **como Admin**
- [ ] Ejecuta:

```powershell
[Environment]::SetEnvironmentVariable("SUPABASE_PASSWORD", "MiFer2121092001", "User")
```

- [ ] Presiona Enter
- [ ] Resultado: Sin errores ✓

### Substep 5.2: Cerrar y Reabrir VS Code
- [ ] Cierra VS Code completamente
- [ ] Espera 5 segundos
- [ ] Reabre VS Code
- [ ] Verifica que cargó proyecto correctamente

### Substep 5.3: Verificar appsettings.json
- [ ] Abre: FashionStore.Web/appsettings.json
- [ ] Verifica que tenga:
  - [ ] "DatabaseProvider": "PostgreSQL" ✓
  - [ ] DefaultConnection apunta a Supabase ✓

---

## 🟣 PASO 6: TEST DE CONEXIÓN (5 minutos)

### Substep 6.1: Compilar
- [ ] En Terminal:

```bash
cd FashionStore.Web
dotnet build -c Release
```

- [ ] Resultado: 0 errores ✓

### Substep 6.2: Ejecutar App
- [ ] En Terminal (mismo directorio):

```bash
dotnet run
```

- [ ] Espera a ver: "Now listening on: http://localhost:5100" ✓

### Substep 6.3: Acceder a App
- [ ] Abre navegador: http://localhost:5100
- [ ] Espera a que cargue
- [ ] Página carga correctamente ✓

---

## 🟤 PASO 7: TEST DE DATOS (10 minutos)

### Substep 7.1: Login
- [ ] Click: Login / Iniciar Sesión
- [ ] Email: admin@fashionstore.com
- [ ] Password: Password123!
- [ ] Click: Login
- [ ] ✓ Ingresa exitosamente

### Substep 7.2: Ver Datos Existentes
- [ ] Navega a: Catálogo → Prendas
- [ ] ✓ Debe mostrar todas las prendas de SQL Server

- [ ] Navega a: Admin → Clientes
- [ ] ✓ Debe mostrar todos los clientes

- [ ] Navega a: Ventas → Historial de Ventas
- [ ] ✓ Debe mostrar todas las ventas

### Substep 7.3: Crear Nueva Venta (Test de Escritura)
- [ ] Click: Ventas → Nueva Venta (POS)
- [ ] Selecciona: Cliente, Vendedor, Prendas
- [ ] Click: Agregar al carrito
- [ ] Verifica carrito persiste
- [ ] Click: Guardar/Procesar Venta
- [ ] ✓ Venta se guarda exitosamente

### Substep 7.4: Verificar en Supabase
- [ ] Ve a Supabase SQL Editor
- [ ] Ejecuta:

```sql
SELECT * FROM public.Ventas ORDER BY "Fecha" DESC LIMIT 1;
```

- [ ] ✓ Debe mostrar la venta que acabas de crear

---

## ✅ PASO 8: FINALIZACIÓN

### Substep 8.1: Documentar Completado
- [ ] Todos los checkboxes anteriores ✓

### Substep 8.2: Backup de Configuración
- [ ] Guarda: appsettings.json (para referencia)
- [ ] Guarda: SUPABASE_PASSWORD (en lugar seguro)

### Substep 8.3: Verificación Final
- [ ] Build: 0 errores ✓
- [ ] Tests: dotnet test (deben pasar) ✓
- [ ] App conecta a Supabase ✓
- [ ] Datos migrados completamente ✓
- [ ] Nuevos datos se graban en Supabase ✓

---

## 🎉 RESULTADO FINAL

```
✅ Migración SQL Server → Supabase COMPLETADA
✅ Todos los datos (8 tablas) en Supabase
✅ Aplicación conectada y funcionando
✅ Nuevas ventas grabándose en Supabase
✅ Listo para COMENZAR FASE 1 del plan

PRÓXIMO: Continuar con PLAN_CORRECCION_TECNICA.md - Fase 1
```

---

## 📞 AYUDA RÁPIDA

**Si falla algo**:
1. Revisa logs en terminal
2. Verifica credenciales: MiFer2121092001
3. Verifica host: db.bajbvebkmacdnllnxvkv.supabase.co
4. Verifica que SUPABASE_PASSWORD esté en variables de entorno
5. Si todo falla, restaura de SQL Server y reintenta

**Archivos de referencia**:
- Database/GUIA_MIGRACION_SQLSERVER_A_SUPABASE.md (detalles)
- Database/EXPORT_SQLSERVER_SCHEMA.sql (exportación)
- Database/SUPABASE_SCHEMA_POSTGRESQL.sql (schema)

---

**Versión**: 1.0.0  
**Status**: 🔴 POR EJECUTAR  
**Tiempo Total**: 30-60 minutos  

**¡COMIENZA AHORA CON PASO 1!**
