# 📋 QUÉ PEGAR EN SUPABASE - PASO A PASO

## 🎯 RESUMEN RÁPIDO

**En el Notepad que está abierto:**
```
Selecciona TODO: Ctrl+A
Copia: Ctrl+C
```

**En Supabase (siguiente):**
```
Pega TODO: Ctrl+V
Click "Run": Ctrl+Enter
```

---

## 📖 EXPLICACIÓN DEL ARCHIVO

El archivo `SUPABASE_SETUP_COMPLETO.sql` contiene:

### 1️⃣ CREAR TABLAS (10 tablas)
```sql
CREATE TABLE IF NOT EXISTS public.Categorias (...)
CREATE TABLE IF NOT EXISTS public.Prendas (...)
CREATE TABLE IF NOT EXISTS public.Clientes (...)
CREATE TABLE IF NOT EXISTS public.Vendedores (...)
CREATE TABLE IF NOT EXISTS public.MetodoPago (...)
CREATE TABLE IF NOT EXISTS public.Ventas (...)
CREATE TABLE IF NOT EXISTS public.DetalleVenta (...)
CREATE TABLE IF NOT EXISTS public.ConfiguracionSistema (...)
CREATE TABLE IF NOT EXISTS public.ConfiguracionAuditoria (...)
```

### 2️⃣ CREAR ÍNDICES (7 índices)
```sql
CREATE INDEX IF NOT EXISTS idx_prendas_categoria ON public.Prendas(...)
... más índices
```

### 3️⃣ INSERTAR DATOS INICIALES (51 registros)
```sql
INSERT INTO public.Categorias VALUES (...)  -- 5 categorías
INSERT INTO public.Prendas VALUES (...)     -- 18 prendas
INSERT INTO public.Clientes VALUES (...)    -- 10 clientes
INSERT INTO public.Vendedores VALUES (...) -- 5 vendedores
INSERT INTO public.MetodoPago VALUES (...)  -- 5 métodos pago
INSERT INTO public.Ventas VALUES (...)      -- 2 ventas ejemplo
... más inserts
```

---

## ✅ RESULTADO FINAL

Después de ejecutar TODO en Supabase:

| Tabla | Registros |
|-------|-----------|
| Categorias | 5 |
| Prendas | 18 |
| Clientes | 10 |
| Vendedores | 5 |
| MetodoPago | 5 |
| DescuentosAutorizados | 0 |
| Ventas | 2+ |
| DetalleVentas | 4+ |
| ConfiguracionSistema | 1 |
| ConfiguracionAuditoria | 1 |

**TOTAL: 10 tablas, 51+ registros**

---

## 🚀 INSTRUCCIONES EXACTAS

### Paso 1: En Notepad (AHORA ABIERTO)
```
1. Ctrl+A  (seleccionar TODO)
2. Ctrl+C  (copiar)
```

### Paso 2: Ir a Supabase
```
https://supabase.com/dashboard/project/bajbvebkmacdnllnxvkv/sql/new
```

### Paso 3: En Supabase SQL Editor
```
1. Click botón "New Query" (arriba a la derecha)
2. En el editor blanco: Ctrl+V (pegar)
3. Click botón "Run" (arriba derecha)
   O: Ctrl+Enter
```

### Paso 4: Esperar
```
Tiempo: 30 segundos a 1 minuto
Verás: "✅ Query executed successfully"
```

### Paso 5: Verificar
```
Click "Table Editor" (lado izquierdo)
Deberías ver 10 tablas con datos
```

---

## 📸 VISUAL

```
┌─────────────────────────────────────────┐
│ NOTEPAD (abierto)                       │
│ ┌───────────────────────────────────┐   │
│ │ -- SUPABASE - SETUP COMPLETO     │   │
│ │ CREATE TABLE public.Categorias...│   │
│ │ CREATE TABLE public.Prendas...   │   │
│ │ ... (TODO el SQL)                │   │
│ └───────────────────────────────────┘   │
│                                         │
│ Ctrl+A → Ctrl+C (copiar TODO)          │
└─────────────────────────────────────────┘

              ⬇️

┌─────────────────────────────────────────┐
│ SUPABASE SQL EDITOR                     │
│ https://supabase.com/dashboard/...      │
│                                         │
│ [New Query]  [Run] ⚙️                   │
│ ┌───────────────────────────────────┐   │
│ │ (Ctrl+V pegar aquí)               │   │
│ │                                   │   │
│ │ -- SUPABASE - SETUP COMPLETO     │   │
│ │ CREATE TABLE public.Categorias...│   │
│ │ ...                               │   │
│ └───────────────────────────────────┘   │
│                                         │
│ Click [Run] o Ctrl+Enter               │
└─────────────────────────────────────────┘

              ⬇️

┌─────────────────────────────────────────┐
│ ✅ Query executed successfully         │
│ Rows affected: 51                       │
│                                         │
│ [Table Editor] ← Ver tablas creadas    │
└─────────────────────────────────────────┘
```

---

## 🎯 LISTO

Después de esto, veremos cómo ejecutar la app:

```powershell
$env:SUPABASE_PASSWORD="MiFer2121092001"
dotnet run
```

---

**¿Necesitas ayuda en algún paso? Avísame exactamente dónde te atascas.**
