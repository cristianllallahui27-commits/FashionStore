# 🚀 EJECUTAR CON F5 (Sin Microsoft Debugger)

## ✅ Solución Configurada

He configurado VS Code para ejecutar con **F5 sin necesidad del debugger de Microsoft**. La aplicación se ejecutará en terminal integrada.

---

## 🎯 Cómo Usar

### **Presiona F5**

Se abrirá un menú para seleccionar:

```
1. 🚀 RUN Release (F5)      ← RECOMENDADO para probar
2. 🔧 RUN Debug             ← Para desarrollo
```

---

## 📋 Qué Sucede

### **Flujo Automático:**

1. **Pre-launch task**: Compila el proyecto
   ```
   dotnet build FashionStoreSolution.sln -c Release
   ```

2. **Post-launch task**: Ejecuta la aplicación
   ```
   dotnet run --project FashionStore.Web --configuration Release --no-build
   ```

3. **Console**: Salida en terminal integrada de VS Code
   - Puedes ver logs en tiempo real
   - El servidor arranca y escucha en `http://localhost:5100`

4. **Acceso**: Abre navegador manualmente en `http://localhost:5100`

---

## 🛑 Detener Ejecución

- **Ctrl+C** en la terminal integrada
- O presiona Stop en la barra de Debug (Shift+F5)

---

## ⚙️ Variables de Entorno

Incluidas automáticamente:
```
SUPABASE_PASSWORD=MiFer2121092001
```

---

## 📁 Archivos Creados

```
✓ .vscode/launch.json     → Configuración de F5
✓ .vscode/tasks.json      → Tareas (build + run)
✓ run-release.bat         → Script de ejecución Release
✓ run-debug.bat           → Script de ejecución Debug
```

---

## 🔧 Opciones Disponibles

### Opción A: F5 → "🚀 RUN Release"
```
- Compila en Release (más rápido en ejecución)
- Ejecuta aplicación
- Terminal integrada muestra salida
- Abre navegador en http://localhost:5100
```

### Opción B: F5 → "🔧 RUN Debug"
```
- Compila en Debug (información de debug disponible)
- Ejecuta aplicación
- Terminal integrada muestra salida
- Abre navegador en http://localhost:5100
```

### Opción C: Menú Run
```
Run > Start Debugging (F5)
Run > Run Without Debugging (Ctrl+F5)
Run > Stop Debugging (Shift+F5)
```

---

## ✅ Checklist

- [x] launch.json configurado (sin Microsoft debugger)
- [x] tasks.json con tareas de build + run
- [x] Scripts batch para ejecución
- [x] Variables de entorno automáticas
- [x] Terminal integrada para salida
- [x] Sin advertencias de "Microsoft versions"

---

## 🎯 Próximos Pasos

1. **Presiona F5**
2. **Selecciona "🚀 RUN Release"**
3. **Espera a que compile y ejecute**
4. **Abre navegador en http://localhost:5100**
5. **¡Listo! La aplicación está corriendo**

---

## ⚠️ Notas

- Sin debugging de Microsoft (se evita la advertencia)
- Toda la salida se ve en terminal integrada
- Puedes ver logs en tiempo real
- Aplicación funciona 100% igual

---

**Configuración completada. ¡Presiona F5 y disfruta!**
