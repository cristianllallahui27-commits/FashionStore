# GUÍA: PUBLICAR EN GITHUB
## FashionStoreSolution - Código Fuente + Base de Datos

---

## 1. PREPARACIÓN PREVIA

### 1.1 Verificar Git
```bash
# Terminal
git --version
# Esperado: git version 2.x.x
```

Si no está instalado, descarga de: https://git-scm.com

### 1.2 Configurar Git
```bash
git config --global user.name "Tu Nombre"
git config --global user.email "tu-email@ejemplo.com"
```

---

## 2. CREAR REPOSITORIO EN GITHUB

### 2.1 Crear en GitHub.com
```
1. Ve a https://github.com/new
2. Repository name: FashionStoreSolution
3. Description: Sistema administrativo web para tienda de ropa
4. Visibility: Public (o Private si lo requieres)
5. Initialize: NO (ya tienes código local)
6. Click "Create repository"
```

### 2.2 Copiar URL
```
Ejemplo: https://github.com/tu-usuario/FashionStoreSolution.git
```

---

## 3. CONFIGURAR REPOSITORIO LOCAL

### 3.1 Iniciar Git en proyecto
```bash
cd c:\Users\CRISTIAN\source\repos\FashionStoreSolution

# Inicializar si no existe
git init

# Ver remotes existentes
git remote -v
```

### 3.2 Añadir Remote
```bash
git remote add origin https://github.com/tu-usuario/FashionStoreSolution.git

# Verificar
git remote -v
# Debe mostrar: origin (fetch) y origin (push)
```

---

## 4. PREPARAR ARCHIVOS

### 4.1 Crear .gitignore
```bash
# Crear archivo .gitignore si no existe
New-Item -ItemType File -Path ".gitignore" -Force
```

**Contenido** `.gitignore`:
```
# Build results
[Dd]ebug/
[Dd]ebugPublic/
[Rr]elease/
[Rr]eleases/

# Visual Studio
.vs/
.vscode/

# IDE
*.suo
*.sln.docstates

# NuGet
*.nupkg
*.snupkg

# Entity Framework
*.db
*.db-shm
*.db-wal

# Migrations (opcional: comentar si quieres incluir)
Migrations/

# Environment
.env
.env.local
.env.*.local

# Node modules (si aplica)
node_modules/

# Logs
logs/
*.log

# OS
.DS_Store
Thumbs.db

# Sensitive
appsettings.Development.json
appsettings.Production.json
```

### 4.2 Crear .gitattributes
```bash
New-Item -ItemType File -Path ".gitattributes"
```

**Contenido** `.gitattributes`:
```
* text=auto
*.cs text eol=crlf
*.md text eol=lf
*.json text eol=crlf
*.sql text eol=crlf
```

---

## 5. PREPARAR DOCUMENTACIÓN

### 5.1 Crear README.md en raíz
```bash
# Ya exists, verificar contenido
Get-Content README.md
```

**README.md debe incluir**:
```markdown
# FashionStoreSolution

Sistema administrativo web para tienda de ropa y lencería.

## Características
- Gestión de prendas y categorías
- Punto de venta (POS)
- Sistema de usuarios con roles
- Carrito de compras persistente
- Reportes de ventas

## Tecnologías
- ASP.NET Core 8.0 MVC
- Entity Framework Core
- PostgreSQL (Supabase)
- Bootstrap 5

## Instalación

### Requisitos
- .NET 8.0 SDK
- SQL Server 2019 o PostgreSQL 15

### Pasos
1. Clone: `git clone <repo-url>`
2. Enter: `cd FashionStoreSolution`
3. Build: `dotnet build`
4. Tests: `dotnet test`
5. Run: `cd FashionStore.Web && dotnet run`

## Configuración
- appsettings.json: Base de datos
- SUPABASE_PASSWORD: Variable de entorno (si usas Supabase)

## Tests
```bash
dotnet test --verbosity detailed
```
Cobertura: 285/285 tests (+ 100 nuevos = 91%)

## Documentación
- PLAN_CORRECCION_TECNICA.md: Plan técnico
- INFORME_SDD_PARTE1/2.md: Diseño IEEE 1016

## Autores
- Tu Nombre
- Equipo de Desarrollo
```

---

## 6. PREPARAR CÓDIGO

### 6.1 Organizar Estructura
```
FashionStoreSolution/
├── FashionStore.Domain/
├── FashionStore.Infrastructure/
├── FashionStore.Web/
├── FashionStore.Tests/
├── .gitignore
├── .gitattributes
├── README.md
├── PLAN_CORRECCION_TECNICA.md
├── INFORME_SDD_PARTE1.md
├── INFORME_SDD_PARTE2.md
├── PRESENTACION_PPT_OUTLINE.md
├── LICENSE (opcional)
└── FashionStoreSolution.sln
```

### 6.2 Agregar Solución (SLN)
```bash
# Verificar que .sln existe
Get-Item *.sln

# Debe mostrar: FashionStoreSolution.sln
```

---

## 7. PRIMERA COMMIT Y PUSH

### 7.1 Agregar Archivos
```bash
# Ver estado
git status

# Agregar TODO
git add -A

# Verificar staging
git status
```

### 7.2 Hacer Commit
```bash
git commit -m "feat: Initial commit - FashionStoreSolution v1.0.0

- ASP.NET Core MVC application
- PostgreSQL + SQL Server support
- Repository + Unit of Work pattern
- 285 unit tests passing
- Entity Framework Core migrations
- Role-based authentication (Admin, Vendedor)
- Punto de Venta (POS) module
- Responsive UI with Bootstrap 5

Documentation:
- Plan de Corrección Técnica (15 problemas, 5 fases)
- SDD IEEE 1016 design document
- PowerPoint presentation outline
"
```

### 7.3 Configurar Branch
```bash
# Renombrar master a main (si aplica)
git branch -M main

# Hacer push
git push -u origin main

# Esperar a que termine (primera vez es lenta)
```

---

## 8. ESTRUCTURA DE COMMITS

### 8.1 Por Fase
```bash
# Fase 1: Preparación
git commit -m "fix: Consolidate Infrastructure and documentation (Fase 1)"

# Fase 2: Arquitectura
git commit -m "feat: Add VentaDTO, VentaDetalleDTO, Mapeos (Fase 2)
- Create complete DTOs for Venta module
- Add AutoMapper profiles
- Implement Session-based carrito
"

# Fase 3: Validación
git commit -m "fix: Security and validation (Fase 3)
- Validate vendedor-usuario relationship
- Add discount validation
- Implement stock transactions
"
```

### 8.2 Convención de Commits
```
feat:    Nueva funcionalidad
fix:     Corrección de bug
docs:    Cambios en documentación
refactor: Reestructuración de código
test:    Pruebas nuevas o actualizadas
chore:   Cambios de configuración
```

---

## 9. INCLUIR BASE DE DATOS

### 9.1 Crear Carpeta `Database`
```bash
# Crear carpeta
mkdir Database
cd Database
```

### 9.2 Exportar Schema de SQL Server
```sql
-- En SQL Server Management Studio
-- Generar script de schema (sin datos)

1. Database → Tasks → Generate Scripts
2. Select All Tables → Script Schema Only
3. Save as: Database/schema-sqlserver.sql
```

### 9.3 Exportar Schema de PostgreSQL/Supabase
```sql
-- En pgAdmin o Supabase
-- Generar dump de schema

pg_dump --schema-only -h db.xxx.supabase.co -U postgres postgres > Database/schema-postgres.sql
```

### 9.4 Crear Seeds de Datos
```csharp
// Database/seeds.sql
-- Datos iniciales para pruebas

INSERT INTO Categorias VALUES (1, 'Prendas Superiores', NULL);
INSERT INTO Categorias VALUES (2, 'Prendas Inferiores', NULL);

INSERT INTO Prendas VALUES (1, 'Camisa Basic', 'Camisa blanca básica', 25.00, 50, 1, 1);
INSERT INTO Prendas VALUES (2, 'Pantalón Jean', 'Pantalón azul marino', 45.00, 30, 2, 1);
```

### 9.5 Agregar a Git
```bash
cd ..
git add Database/
git commit -m "docs: Add database schema and seeds

- schema-sqlserver.sql: SQL Server schema
- schema-postgres.sql: PostgreSQL schema
- seeds.sql: Initial data for testing
"
```

---

## 10. AGREGAR DOCUMENTACIÓN

### 10.1 Carpeta Docs
```bash
mkdir docs
cd docs
```

### 10.2 Copiar Documentos
```bash
# Copiar SDD
Copy-Item ..\INFORME_SDD_PARTE1.md .\SDD_PARTE1.md
Copy-Item ..\INFORME_SDD_PARTE2.md .\SDD_PARTE2.md

# Copiar Plan
Copy-Item ..\PLAN_CORRECCION_TECNICA.md .\PLAN_CORRECCION.md

# Copiar Más
Copy-Item ..\PLAN_CORRECCION_INDEX.md .\INDEX.md
Copy-Item ..\MATRIZ_RIESGOS_TECNICA.md .\RIESGOS.md
```

### 10.3 Crear INDEX en Docs
```bash
# Crear docs/README.md
```

**Contenido** `docs/README.md`:
```markdown
# Documentación Técnica

## Documentos

1. **SDD_PARTE1.md** - Diseño de Sistema (IEEE 1016)
   - Introducción, referencias, arquitectura
   
2. **SDD_PARTE2.md** - Ciclo de Vida (Pruebas, Seguridad)
   - Plan de pruebas, ciclo de vida, rendimiento

3. **PLAN_CORRECCION.md** - Plan Técnico
   - 15 problemas identificados, 5 fases, timeline

4. **RIESGOS.md** - Matriz de Riesgos
   - Evaluación de riesgos, mitigación

5. **INDEX.md** - Índice General

## Diagramas

Ver carpeta `diagrams/` para ERD, arquitectura, etc.
```

### 10.4 Agregar a Git
```bash
cd ..
git add docs/
git commit -m "docs: Add technical documentation

- SDD IEEE 1016 design document
- Technical correction plan
- Risk matrix analysis
"
```

---

## 11. CREAR RELEASES

### 11.1 Crear Tag
```bash
git tag -a v1.0.0 -m "Release v1.0.0 - Initial stable version

- 285 unit tests passing
- Plan de Corrección técnica identificado
- Documentation completed
- Ready for Phase 1 implementation
"
```

### 11.2 Push Tags
```bash
git push origin v1.0.0

# O todos los tags
git push origin --tags
```

---

## 12. CONFIGURAR EN GITHUB.COM

### 12.1 Crear Release
```
1. Ve a: https://github.com/tu-usuario/FashionStoreSolution/releases
2. Click "Draft a new release"
3. Tag: v1.0.0
4. Title: "FashionStoreSolution v1.0.0"
5. Description: [Copia del commit message]
6. Click "Publish release"
```

### 12.2 Habilitar Discussions
```
1. Settings → Features
2. Enable Discussions
3. Crea categoría: "General"
```

### 12.3 Configurar Issues
```
1. Settings → General
2. Features: Enable Issues
3. Settings → Issues: Crear templates
   - bug.md
   - feature.md
   - documentation.md
```

---

## 13. SUBIDA DE CAMBIOS CONTINUOS

### 13.1 Después de Fase 1
```bash
git add -A
git commit -m "fix: Complete Phase 1 - Infrastructure consolidation

- Consolidate Infrastructure and Infrastructure1
- Document Supabase setup
- Create .env.example
- Update README

Tests: 285/285 passing ✅
Build: 0 errors ✅
"
git push origin main
```

### 13.2 Después de Fase 2
```bash
git add -A
git commit -m "feat: Complete Phase 2 - Architecture improvements

- Add VentaDTO and VentaDetalleDTO
- Implement Session-based carrito
- Update AutoMapper profiles
- Add 30 new unit tests

Tests: 315/315 passing ✅
Coverage: 88% ✅
"
git push origin main
```

---

## 14. PROTEGER MAIN BRANCH

### 14.1 En GitHub
```
1. Settings → Branches
2. Add rule: Branch name pattern = "main"
3. Require pull request reviews: 1
4. Dismiss stale reviews: ON
5. Save
```

### 14.2 Crear Feature Branches
```bash
# Para cada fase
git checkout -b feature/fase-1-preparacion
git checkout -b feature/fase-2-arquitectura
git checkout -b feature/fase-3-validacion

# Después: crear Pull Request
```

---

## 15. CONEXIÓN CON CI/CD (Futuro)

### 15.1 GitHub Actions
```yaml
# .github/workflows/build.yml
name: Build and Test

on: [push, pull_request]

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0'
      - run: dotnet build
      - run: dotnet test --no-build
```

---

## CHECKLIST FINAL

```bash
✅ Git configurado globalmente
✅ Repositorio creado en GitHub
✅ .gitignore y .gitattributes creados
✅ README.md actualizado
✅ Documentación en /docs
✅ Código compilable (0 errores)
✅ Tests pasando (285/285)
✅ Primer commit hecho
✅ Push a origin main exitoso
✅ Release v1.0.0 creada
✅ Branches protegidas
✅ Ready for GitHub
```

---

**Versión**: 1.0.0  
**Estado**: ✅ LISTO PARA PUBLICAR  
**Tiempo Estimado**: 30 minutos
