# 🚀 EJECUTAR CON F5 EN VS CODE

## ✅ Configuración Completada

He actualizado los archivos de configuración de VS Code para que puedas ejecutar la aplicación con **F5**:

- ✅ `.vscode/launch.json` - Configuraciones de debug
- ✅ `.vscode/tasks.json` - Tareas pre-launch

---

## 🎯 Cómo Ejecutar

### Opción 1: Press F5 (RECOMENDADO - Release)
```
1. Abre VS Code
2. Presiona F5
3. Selecciona: "🚀 FashionStore.Web (Supabase)" (primera opción)
4. ✓ Compilará en Release y ejecutará
5. Navegador se abrirá automáticamente en http://localhost:5100
```

### Opción 2: Debug Mode (Development)
```
1. Abre VS Code
2. Presiona F5
3. Selecciona: "🔧 FashionStore.Web Debug (Development)"
4. ✓ Compilará en Debug y ejecutará con breakpoints habilitados
5. Navegador se abrirá automáticamente
```

### Opción 3: Menú Debug
```
1. Menú superior: Run > Start Debugging
2. O: Run > Run Without Debugging (Ctrl+F5)
```

---

## 📋 Configuraciones Disponibles

### 1. 🚀 FashionStore.Web (Supabase) - RELEASE
- **Tipo**: Production Release
- **Entorno**: Production
- **Compilación**: Release (más rápido en ejecución)
- **Uso**: Probar funcionalidad final
- **Inicio**: http://localhost:5100

### 2. 🔧 FashionStore.Web Debug - DEVELOPMENT
- **Tipo**: Debug Mode
- **Entorno**: Development
- **Compilación**: Debug (breakpoints, logs detallados)
- **Uso**: Desarrollo y debugging
- **Inicio**: http://localhost:5100

### 3. .NET Core Attach
- **Tipo**: Attach a proceso existente
- **Uso**: Conectar debugger a proceso ya corriendo

---

## ⚙️ Variables de Entorno

Ambas configuraciones incluyen:
```
SUPABASE_PASSWORD=MiFer2121092001
```

Esto se pasa automáticamente, no necesitas hacerlo manualmente.

---

## 🔍 Qué Pasa al Presionar F5

### Flujo Automático:
1. **Pre-launch task**: Compila el proyecto
   ```
   dotnet build FashionStoreSolution.sln -c Release
   ```

2. **Launch**: Ejecuta la aplicación
   ```
   {DLL compilada}/FashionStore.Web.dll
   ```

3. **Server Ready**: Detecta cuando el servidor está listo
   - Patrón: "Now listening on:"
   - Abre navegador automáticamente

4. **Browser**: Se abre http://localhost:5100

---

## 🛑 Detener Ejecución

### Opción A: Shift+F5
- Detiene el debugger y la aplicación

### Opción B: Menú Debug
- Run > Stop Debugging

### Opción C: Ctrl+C en Terminal
- En la consola integrada de VS Code

---

## 🐛 Debugging

### Breakpoints
1. Haz clic en el número de línea (sidebar izquierdo)
2. Aparece punto rojo
3. Al ejecutar F5, se pausará en ese punto

### Variables Locales
- Cuando pausado, ver panel "Variables" (Debug sidebar)
- Inspeccionar estado en tiempo real

### Watch
- Expresiones personalizadas a monitorear
- Panel "Watch" en Debug sidebar

### Console
- Ejecutar comandos C# en contexto
- Panel "Debug Console"

---

## ⚠️ Solución de Problemas

### Problema: "Task not found"
**Solución**: Asegúrate que tasks.json existe en `.vscode/`

### Problema: "Port 5100 already in use"
```powershell
# Encuentra el proceso
Get-NetTcpConnection -LocalPort 5100

# Mata el proceso
Stop-Process -Id {PID} -Force

# Intenta F5 de nuevo
```

### Problema: "Column UsuarioId does not exist"
**Solución**: Aplicar el fix manual antes de ejecutar
- Ver: `FIX_COLUMN_USUARIOID.md`

### Problema: Debugger no detiene en breakpoints
```
1. Asegúrate usar "🔧 FashionStore.Web Debug"
2. Reconstruir proyecto
3. F5 de nuevo
```

---

## 📊 Panel de Debug (Izquierda)

Cuando ejecutas con F5, se abre panel con:

- **Variables** - Estado actual de variables
- **Watch** - Expresiones monitoreadas
- **Call Stack** - Pila de ejecución
- **Breakpoints** - Puntos de parada

---

## 🎨 Keyboard Shortcuts

| Atajo | Función |
|:---|:---|
| F5 | Iniciar debugging |
| Shift+F5 | Detener debugging |
| Ctrl+F5 | Ejecutar sin debugging |
| F9 | Toggle breakpoint |
| F10 | Step over |
| F11 | Step into |
| Shift+F11 | Step out |
| Ctrl+Shift+D | Abrir Debug sidebar |

---

## 📝 Ejemplo: Debuggear Venta

```
1. Abre: VentasController.cs
2. Línea en método Create()
3. Click en número de línea → breakpoint (punto rojo)
4. F5 → "🚀 FashionStore.Web (Supabase)"
5. Navega a: /Ventas/Create
6. ✓ Pausa en el breakpoint
7. Inspecciona variables
8. F10 para siguiente línea
9. Shift+F5 para detener
```

---

## ✅ Checklist de Configuración

- [x] launch.json actualizado
- [x] tasks.json actualizado
- [x] 2 configuraciones disponibles (Release + Debug)
- [x] SUPABASE_PASSWORD incluido
- [x] Auto-open navegador configurado
- [x] Breakpoints habilitados en Debug mode

---

## 🚀 Listo para Usar

Solo presiona **F5** y elige:
1. "🚀 FashionStore.Web (Supabase)" para Release
2. "🔧 FashionStore.Web Debug" para Development

¡La aplicación se compilará, ejecutará y abrirá en navegador automáticamente!

---

**Fecha**: Julio 2026  
**Status**: ✅ Configuración Completada
