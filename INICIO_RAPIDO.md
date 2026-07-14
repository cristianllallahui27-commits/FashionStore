# ⚡ INICIO RÁPIDO - FashionStore

## 🎯 En 30 Segundos

### 1. Abre Terminal
Presiona **Ctrl+`** en VS Code

### 2. Copia y Pega:
```bash
cd c:\Users\CRISTIAN\source\repos\FashionStoreSolution\FashionStore.Web
dotnet run
```

### 3. Espera a esto:
```
Now listening on: http://localhost:5100
Application started. Press Ctrl+C to stop the application.
```

### 4. Abre en Navegador:
**http://localhost:5100**

---

## 🔐 Login de Prueba

**Admin**:
```
Email: admin@fashionstore.com
Password: Password123!
```

---

## ✅ Funcionalidades Disponibles

Una vez logueado como **Admin**, verás en el menú:

1. **Inicio** - Dashboard
2. **Catálogo**
   - Prendas
   - Categorías
3. **Admin**
   - Clientes
   - Vendedores
   - Descuentos
   - Configuración
4. **Ventas**
   - Nueva Venta (POS)
   - Historial de Ventas
5. **Perfil** - Mi Perfil (dropdown usuario)

---

## 🛑 Detener la Aplicación

Presiona **Ctrl+C** en la terminal

---

## 📊 Estado del Proyecto

✅ Build: 0 errores
✅ Tests: 285/285 pasando
✅ Menú: 100% funcional
✅ Controladores: 9/9 implementados
✅ BD: SQL Server conectada

---

## 🐛 Si hay problemas

### Opción 1: Rebuild completo
```bash
cd c:\Users\CRISTIAN\source\repos\FashionStoreSolution
dotnet clean
dotnet build FashionStoreSolution.sln
cd FashionStore.Web
dotnet run
```

### Opción 2: Usar el script PowerShell
```powershell
cd c:\Users\CRISTIAN\source\repos\FashionStoreSolution
PowerShell -ExecutionPolicy Bypass -File run-fashionstore.ps1
```

### Opción 3: Con tests
```powershell
PowerShell -ExecutionPolicy Bypass -File run-fashionstore.ps1 -Test -Rebuild
```

---

## 📖 Documentación Completa

- `GUIA_EJECUCION_SIN_DEBUGGER.md` - Guía detallada
- `VERIFICACION_FUNCIONALIDADES_FINAL.md` - Checklist completo
- `PLAN_CORRECCION_TECNICA.md` - Problemas resueltos

---

## 🎉 ¡Listo!

La aplicación está **LISTA PARA USAR** sin debugger. ¡Disfruta! 🚀
