Resumen
======
Instrucciones para migrar la base de datos SQL Server a Supabase (PostgreSQL) y dejar Supabase como base de datos principal para la aplicación.

Importante
---------
- NO subir la contraseña real a GitHub ni al appsettings.json.
- Usar la variable de entorno SUPABASE_PASSWORD para la contraseña.
- Estas instrucciones asumen que la aplicación usa la cadena en appsettings.json y Program.cs ya preparada para reemplazar ${SUPABASE_PASSWORD}.

Pasos rápidos
-------------
1) Exportar/convertir datos de SQL Server a PostgreSQL (recomendado: pgloader).
2) Ajustar la aplicación para usar Npgsql (ya aplicado en el proyecto).
3) Generar migración EF Core para PostgreSQL y/o generar script SQL:
   - dotnet ef migrations add InitialSupabase --project FashionStore.Infrastructure --startup-project FashionStore.Web
   - dotnet ef migrations script -o database/schema_postgresql.sql --project FashionStore.Infrastructure --startup-project FashionStore.Web
4) Establecer variable de entorno SUPABASE_PASSWORD (PowerShell):
   - $env:SUPABASE_PASSWORD = "TuPasswordAqui"
   - Para permanente: setx SUPABASE_PASSWORD "TuPasswordAqui"
5) Aplicar migraciones (actualiza esquema en Supabase):
   - dotnet ef database update --project FashionStore.Infrastructure --startup-project FashionStore.Web
6) Importar datos convertidos a Supabase (pgloader o psql)
7) Verificar tablas y conteos (verificacion_migracion_supabase.md)

Herramientas recomendadas
-------------------------
- pgloader (migración SQL Server -> PostgreSQL automática)
- psql (cliente Postgres)
- DBeaver / Azure Data Studio / Supabase GUI para inspección visual

Ejemplos de comandos
--------------------
# 1) Establecer variable de entorno (temporal en PowerShell):
$env:SUPABASE_PASSWORD = "TuPasswordSeguro"

# 2) Generar script SQL del modelo EF Core (genera schema_postgresql.sql en carpeta database):
dotnet ef migrations add InitialSupabase --project FashionStore.Infrastructure --startup-project FashionStore.Web
dotnet ef migrations script -o database/schema_postgresql.sql --project FashionStore.Infrastructure --startup-project FashionStore.Web

# 3) Aplicar migraciones directamente contra Supabase (requiere SUPABASE_PASSWORD):
dotnet ef database update --project FashionStore.Infrastructure --startup-project FashionStore.Web

# 4) Migración de datos desde SQL Server con pgloader (instalar pgloader primero)
# Reemplace SOURCE_* y DEST_* con valores reales
pgloader "mssql://SA:SqlServerPassword@localhost:1433/YourSqlServerDatabase" "postgresql://postgres:${SUPABASE_PASSWORD}@db.bajbvebkmacdnllnxvkv.supabase.co:5432/postgres"

# 5) Si ya tiene un dump SQL compatible con Postgres, importar con psql:
psql "postgresql://postgres:${SUPABASE_PASSWORD}@db.bajbvebkmacdnllnxvkv.supabase.co:5432/postgres" -f database/schema_postgresql.sql
psql "postgresql://postgres:${SUPABASE_PASSWORD}@db.bajbvebkmacdnllnxvkv.supabase.co:5432/postgres" -f database/seed_data.sql

Notas y recomendaciones
----------------------
- Conversión de tipos: SQL Server (decimal, nvarchar, datetime) pueden requerir ajuste a Postgres (numeric, varchar, timestamp).
- Identity: tablas AspNet* funcionan con Npgsql pero los hashes de contraseña siguen siendo válidos si los datos se migran correctamente.
- Si no quiere migrar usuarios manualmente, usar la clase DbInitializer incluida para crear roles y admin automáticamente al arrancar la app.
- Verifique constraints, secuencias y columnas identity (postgres usa serial/identity).
- Hacer copia de seguridad antes de cualquier operación destructiva.

Soporte
------
Si quieres, puedo:
- Generar el script EF Core y guardarlo en database/schema_postgresql.sql (si ejecutas dotnet ef migrations localmente y me pegas el resultado puedo integrarlo).
- Sugerir script pgloader más detallado tras ver la conexión SQL Server origen.
- Ejecutar las migraciones y actualizar la base si me pides ejecutar comandos en terminal (te daré los comandos exactos para PowerShell).