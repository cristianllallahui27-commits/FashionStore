# GUÍA: CONVERTIR DOCUMENTOS A WORD (.DOCX)
## FashionStoreSolution

---

## OPCIÓN 1: USAR PANDOC (RECOMENDADO)

### 1.1 Instalar Pandoc
```bash
# Descargar desde: https://pandoc.org/installing.html

# Verificar instalación
pandoc --version
```

### 1.2 Convertir Markdown a Word
```bash
cd c:\Users\CRISTIAN\source\repos\FashionStoreSolution

# Informe SDD - Parte 1
pandoc INFORME_SDD_PARTE1.md -o INFORME_SDD_PARTE1.docx \
  --toc \
  --number-sections \
  --template=default

# Informe SDD - Parte 2
pandoc INFORME_SDD_PARTE2.md -o INFORME_SDD_PARTE2.docx \
  --toc \
  --number-sections \
  --template=default

# Combinar ambas partes
pandoc INFORME_SDD_PARTE1.md INFORME_SDD_PARTE2.md \
  -o INFORME_SDD_COMPLETO.docx \
  --toc \
  --number-sections \
  --reference-doc=custom-style.docx
```

### 1.3 Script de Conversión (PowerShell)
```powershell
# Crear archivo: convert-to-docx.ps1

$files = @(
    "INFORME_SDD_PARTE1.md",
    "INFORME_SDD_PARTE2.md",
    "PLAN_CORRECCION_TECNICA.md",
    "MATRIZ_RIESGOS_TECNICA.md",
    "GUIA_EJECUCION_FASES.md"
)

foreach ($file in $files) {
    $output = $file -replace '.md', '.docx'
    Write-Host "Convirtiendo $file → $output..."
    & pandoc $file -o $output `
        --toc `
        --number-sections `
        --from markdown `
        --to docx
}

Write-Host "✅ Conversión completada"
```

Ejecutar:
```bash
PowerShell -ExecutionPolicy Bypass -File convert-to-docx.ps1
```

---

## OPCIÓN 2: USAR WORD ONLINE

### 2.1 Pasos
```
1. Ve a https://office.com
2. Abre Word Online
3. Crea documento nuevo
4. Copy & Paste contenido markdown
5. Formatea manualmente
6. File → Save as
7. Descarga como .docx
```

**Desventaja**: Manual y lento

---

## OPCIÓN 3: USAR GOOGLE DOCS

### 3.1 Pasos
```
1. Ve a https://docs.google.com
2. Crea nuevo documento
3. Copy & Paste markdown
4. Formatea
5. File → Download → Microsoft Word (.docx)
```

---

## OPCIÓN 4: USAR TYPORA + EXPORT

### 4.1 Typora (Editor Markdown)
```
1. Descarga Typora: https://typora.io
2. Abre archivo .md
3. File → Export → Microsoft Word (.docx)
4. Selecciona estilo
5. Exporta
```

**Ventaja**: Mantiene formato markdown

---

## FORMATEO FINAL EN WORD

### 4.1 Estilo Académico
```
Después de convertir, en Word:

1. Márgenes: 2.5 cm todos lados
2. Fuente: Calibri 11pt
3. Espaciado: 1.5 líneas
4. Párrafos: 0.5" primera línea
5. Encabezados: Arial 14pt Bold
```

### 4.2 Portada
```
Crear página 1:

FASHIONSTORE SOLUTION
Sistema Administrativo Web para Tienda de Ropa y Lencería

Proyecto de Ingeniería de Software
Versión 1.0.0
Julio 2026

Software Design Description (SDD)
Basado en IEEE 1016-2009

Autor: Tu Nombre
Empresa: Tu Empresa
Fecha: Julio 7, 2026
```

### 4.3 Índice Automático
```
En Word:
1. References tab
2. Click: Table of Contents
3. Choose: Automatic Table
4. Word generará TOC automático
```

### 4.4 Numeración de Secciones
```
En Word:
1. Select all text (Ctrl+A)
2. Home → Multilevel List
3. Choose: 1, A, i, a format
4. Apply to paragraphs
```

---

## ESTRUCTURA FINAL DEL DOCUMENTO WORD

```
┌────────────────────────────────┐
│ PORTADA                        │
├────────────────────────────────┤
│ TABLA DE CONTENIDOS (TOC)      │
├────────────────────────────────┤
│ INTRODUCCIÓN                   │
│ 1. Propósito                   │
│ 2. Alcance                     │
│ 3. Definiciones                │
├────────────────────────────────┤
│ DESCRIPCIÓN GENERAL            │
│ 1. Funcionalidad               │
│ 2. Usuarios                    │
│ 3. Restricciones               │
├────────────────────────────────┤
│ ARQUITECTURA                   │
│ 1. Alto Nivel                  │
│ 2. Componentes                 │
│ 3. Decisiones                  │
├────────────────────────────────┤
│ CICLO DE VIDA                  │
│ 1. Fases                       │
│ 2. Pruebas (91%)               │
│ 3. Seguridad                   │
├────────────────────────────────┤
│ APÉNDICES                      │
│ A. Diagrama ERD                │
│ B. Tablas de BD                │
│ C. Convenciones de código      │
└────────────────────────────────┘
```

---

## AGREGAR IMÁGENES Y DIAGRAMAS

### 5.1 Insertar en Word
```
1. Insert → Pictures
2. Selecciona imagen
3. Tamaño: Ancho máximo 6 inches
4. Alineación: Centered
5. Caption: "Figura X. [Descripción]"
```

### 5.2 Crear Diagramas
```
Opciones:
1. Visio (professional)
2. Draw.io (free, online)
3. Lucidchart (collaboration)
4. Miro (whiteboard)

Exportar como PNG/PDF → Insert en Word
```

---

## TABLA: COMPARACIÓN DE MÉTODOS

| Método | Tiempo | Calidad | Automatización | Recomendado |
|--------|--------|---------|----------------|------------|
| Pandoc | 5 min | 85% | ✅ Sí | ✅ ✅ ✅ |
| Word Online | 20 min | 70% | ❌ No | ✅ |
| Google Docs | 15 min | 75% | ❌ No | ✅ |
| Typora | 10 min | 90% | ✅ Sí | ✅ ✅ |
| Manual | 60+ min | 100% | ❌ No | ❌ |

**Recomendación**: Pandoc + Typora

---

## CHECKLIST DE CONVERSIÓN

```bash
✅ Pandoc instalado
✅ Markdown files preparados
✅ Script de conversión creado
✅ Conversión completada
✅ Archivos .docx generados
✅ Revisión de formato
✅ Portada agregada
✅ TOC automático
✅ Secciones numeradas
✅ Imágenes insertadas
✅ Revisión final
✅ Listo para entrega
```

---

## COMANDO FINAL (COMPLETO)

```bash
# Convertir TODAS las partes
pandoc INFORME_SDD_PARTE1.md INFORME_SDD_PARTE2.md \
  PLAN_CORRECCION_TECNICA.md \
  -o "FASHIONSTORE_SOLUTION_INFORME_COMPLETO.docx" \
  --toc \
  --number-sections \
  --from markdown \
  --to docx \
  --variable title="FashionStore Solution - Informe Técnico" \
  --variable author="Tu Nombre" \
  --variable date="Julio 7, 2026"
```

Resultado:
```
✅ FASHIONSTORE_SOLUTION_INFORME_COMPLETO.docx (150-200 páginas)
```

---

## POST-PROCESAMIENTO

### 7.1 En Word
```
1. Abre documento convertido
2. Review → Check Spelling
3. Design → Choose color scheme
4. Review → Track Changes ON
5. Envía para revisión
```

### 7.2 Compartir
```
1. File → Share
2. Get a Link (Share Link)
3. Comparte con stakeholders
4. Recibe feedback
5. Colabora en tiempo real
```

---

**Versión**: 1.0.0  
**Método Recomendado**: Pandoc  
**Tiempo Total**: 10-15 minutos  
**Status**: ✅ LISTO PARA CONVERTIR
