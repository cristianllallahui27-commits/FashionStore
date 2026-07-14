Matriz de verificación de migración
===================================

Instrucciones:
- Para cada tabla original en SQL Server, anote el conteo de registros.
- Después de migrar a Supabase, conecte con psql o GUI y cuente los registros.
- Indique OK si coinciden o ERROR si faltan.

Ejemplo:
| Tabla origen | Conteo origen | Tabla destino | Conteo destino | Estado |
|--------------|---------------|---------------|----------------|--------|
| AspNetUsers  | 10            | AspNetUsers   | 10             | OK     |

Tablas esperadas:
- AspNetUsers
- AspNetRoles
- AspNetUserRoles
- Categorias
- Prendas
- Clientes
- Vendedores
- MetodoPago
- Ventas
- DetalleVenta
- Configuraciones
- ConfiguracionesAuditoria
- DescuentosAutorizados

Proceso de verificación:
1) Conectarse a Supabase con psql:
   psql "postgresql://postgres:${SUPABASE_PASSWORD}@db.bajbvebkmacdnllnxvkv.supabase.co:5432/postgres"
2) Para cada tabla ejecutar: SELECT COUNT(*) FROM public."TableName";
3) Rellenar la tabla de verificación con los resultados.

Notas:
- Si hay discrepancias, revisar claves foráneas, constraints y errores en el log de pgloader.
- Para tablas con columnas JSON o tipos especiales, validar manualmente los datos.
