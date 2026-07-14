-- MANUAL FIX: Agregar columna UsuarioId a tabla Vendedores
-- Ejecutar esto en Supabase SQL Editor si la migration no se aplicó automáticamente

-- 1. Agregar columna UsuarioId si no existe
ALTER TABLE "Vendedores" 
ADD COLUMN IF NOT EXISTS "UsuarioId" text;

-- 2. Registrar la migration como aplicada en la tabla __EFMigrationsHistory
-- (Esto es importante para que EF Core no intente aplicarla de nuevo)
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260713065647_AddUsuarioIdToVendedor', '9.0.0')
ON CONFLICT ("MigrationId") DO NOTHING;

-- RESULTADO:
-- Si todo está correcto, la columna UsuarioId ya debe existir en Vendedores
-- y el sistema debería funcionar sin error "column v.UsuarioId does not exist"

-- VERIFICAR (ejecutar después):
-- SELECT column_name, data_type FROM information_schema.columns 
-- WHERE table_name = 'Vendedores' AND column_name = 'UsuarioId';
