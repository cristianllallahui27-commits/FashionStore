# 🔴 MATRIZ DE RIESGOS TÉCNICOS

**Versión**: 1.0.0  
**Fecha**: Julio 7, 2026  
**Metodología**: Risk Matrix (Probabilidad × Impacto)  

---

## 📊 ESCALA DE EVALUACIÓN

### Probabilidad
- 🟢 **Bajo** (1): Raro que ocurra
- 🟡 **Medio** (2): Puede ocurrir
- 🔴 **Alto** (3): Muy probable que ocurra

### Impacto
- 🟢 **Bajo** (1): Molestia menor
- 🟡 **Medio** (2): Funcionalidad afectada
- 🔴 **Alto** (3): Sistema inutilizable
- 🟣 **Crítico** (4): Pérdida de datos / Seguridad comprometida

### Riesgo = Probabilidad × Impacto
- 1-3: Bajo (🟢)
- 4-6: Medio (🟡)
- 8-12: Alto (🔴)

---

## 🎯 MATRIZ ACTUAL

```
           IMPACTO
           1    2    3    4
           ↓    ↓    ↓    ↓
PROB 1 →  [B ]  [B ]  [M ]  [M ]
     2 →  [B ]  [M ]  [A ]  [A ]
     3 →  [M ]  [A ]  [A ]  [CR]
```

---

## 📋 RIESGOS IDENTIFICADOS (Ordenados por Severidad)

### 🟣 CRÍTICO (Riesgo ≥ 8)

#### [CR1] Carrito Pierde Datos en Producción
| Aspecto | Valor |
|---------|-------|
| **ID** | C3 |
| **Descripción** | Items del carrito se pierden entre peticiones |
| **Ubicación** | `CarritoService.cs` línea 8 |
| **Probabilidad** | 🔴 Alta (3) - Ocurre en load-balancing |
| **Impacto** | 🟣 Crítico (4) - Pérdida de datos del cliente |
| **Riesgo** | 3 × 4 = **12 (CRÍTICO)** |
| **Estado Actual** | 🔴 Activo |
| **Mitigación** | Implementar Session o BD para carrito |
| **Plazo** | Fase 2 - URGENTE |

---

#### [CR2] Acceso No Autorizado a Ventas
| Aspecto | Valor |
|---------|-------|
| **ID** | A1 |
| **Descripción** | Vendedor sin registro ve TODAS las ventas |
| **Ubicación** | `VentasController.cs` línea 52 |
| **Probabilidad** | 🟡 Media (2) - Si usuario no está sincronizado |
| **Impacto** | 🟣 Crítico (4) - Brecha de seguridad |
| **Riesgo** | 2 × 4 = **8 (CRÍTICO)** |
| **Estado Actual** | 🔴 Activo |
| **Mitigación** | Validar vendedor obligatoriamente |
| **Plazo** | Fase 3 - URGENTE |

---

#### [CR3] Descuentos Fraudulentos
| Aspecto | Valor |
|---------|-------|
| **ID** | A3 |
| **Descripción** | Vendedor aplica descuentos sin validación |
| **Ubicación** | `VentasController.cs` POST Create |
| **Probabilidad** | 🟡 Media (2) - Si vendedor es deshonesto |
| **Impacto** | 🟣 Crítico (4) - Pérdida financiera |
| **Riesgo** | 2 × 4 = **8 (CRÍTICO)** |
| **Estado Actual** | 🔴 Activo |
| **Mitigación** | Validar descuento ≤ máximo y ≤ total |
| **Plazo** | Fase 3 - URGENTE |

---

#### [CR4] Race Condition en Stock
| Aspecto | Valor |
|---------|-------|
| **ID** | A5 |
| **Descripción** | Dos vendedores venden último ítem simultáneamente |
| **Ubicación** | `CarritoService.cs` + `ServicioVentas.cs` |
| **Probabilidad** | 🟡 Media (2) - Depende de concurrencia |
| **Impacto** | 🟣 Crítico (4) - Overbooking e inconsistencia |
| **Riesgo** | 2 × 4 = **8 (CRÍTICO)** |
| **Estado Actual** | 🔴 Activo |
| **Mitigación** | Transacciones DB con lock |
| **Plazo** | Fase 3 - URGENTE |

---

### 🔴 ALTO (Riesgo 6-7)

#### [A1] Duplicación de Infraestructura
| Aspecto | Valor |
|---------|-------|
| **ID** | C1 |
| **Descripción** | Dos carpetas Infrastructure + Infrastructure1 |
| **Ubicación** | Raíz del proyecto |
| **Probabilidad** | 🔴 Alta (3) - Mantenedor toca wrong folder |
| **Impacto** | 🟡 Medio (2) - Bug difícil de debuggear |
| **Riesgo** | 3 × 2 = **6 (ALTO)** |
| **Estado Actual** | 🔴 Activo |
| **Mitigación** | Consolidar a una carpeta |
| **Plazo** | Fase 1 - PRIORITARIO |

---

#### [A2] BD Supabase Sin Variable Entorno
| Aspecto | Valor |
|---------|-------|
| **ID** | C2 |
| **Descripción** | SUPABASE_PASSWORD no configurable → App CRASH |
| **Ubicación** | `Program.cs` línea 42-48 |
| **Probabilidad** | 🔴 Alta (3) - Falta documentación |
| **Impacto** | 🟡 Medio (2) - Difícil onboarding |
| **Riesgo** | 3 × 2 = **6 (ALTO)** |
| **Estado Actual** | 🔴 Activo |
| **Mitigación** | Crear `.env.example`, fallback a SQL Server |
| **Plazo** | Fase 1 - PRIORITARIO |

---

#### [A3] Controlador Usa DbContext Directo
| Aspecto | Valor |
|---------|-------|
| **ID** | A2 |
| **Descripción** | VentasController accede BD via `_context` y `_unitOfWork` |
| **Ubicación** | `VentasController.cs` línea 18-19 |
| **Probabilidad** | 🟡 Media (2) - Error de arquitectura |
| **Impacto** | 🟡 Medio (2) - Difícil testear, inconsistencia |
| **Riesgo** | 2 × 2 = **4 (MEDIO)** |
| **Estado Actual** | 🔴 Activo |
| **Mitigación** | Remover `_context`, usar solo UnitOfWork |
| **Plazo** | Fase 2 |

---

#### [A4] Admin Tiene Múltiples Roles
| Aspecto | Valor |
|---------|lantán|
| **ID** | A4 |
| **Descripción** | Admin tiene rol "Administrador" + "Vendedor" |
| **Ubicación** | `DbInitializer.cs` línea 93-96 |
| **Probabilidad** | 🟡 Media (2) - Lógica confusa puede causar bugs |
| **Impacto** | 🟡 Medio (2) - Comportamiento inesperado |
| **Riesgo** | 2 × 2 = **4 (MEDIO)** |
| **Estado Actual** | 🔴 Activo |
| **Mitigación** | Decidir: Admin = Administrador XOR Vendedor |
| **Plazo** | Fase 3 |

---

### 🟡 MEDIO (Riesgo 4-6)

#### [M1] DTOs Incompletos
| Aspecto | Valor |
|---------|-------|
| **ID** | M2 |
| **Descripción** | Falta VentaDTO, VentaDetalleDTO |
| **Ubicación** | `FashionStore.Domain/DTOs/` |
| **Probabilidad** | 🟡 Media (2) - Se necesitan para APIs |
| **Impacto** | 🟡 Medio (2) - Mapeos manuales |
| **Riesgo** | 2 × 2 = **4 (MEDIO)** |
| **Estado Actual** | 🟡 Activo |
| **Mitigación** | Crear DTOs faltantes |
| **Plazo** | Fase 2 |

---

#### [M2] DetalleVentaDTO en Lugar Incorrecto
| Aspecto | Valor |
|---------|-------|
| **ID** | M1 |
| **Descripción** | DetalleVentaDTO definido en Interfaces/ no en DTOs/ |
| **Ubicación** | `IServicioVentas.cs` |
| **Probabilidad** | 🟢 Bajo (1) - No afecta funcionalidad |
| **Impacto** | 🟡 Medio (2) - Confusión de proyecto |
| **Riesgo** | 1 × 2 = **2 (BAJO)** |
| **Estado Actual** | 🟡 Activo |
| **Mitigación** | Mover a DTOs/ |
| **Plazo** | Fase 2 |

---

#### [M3] Mapeos Incompletos en AutoMapper
| Aspecto | Valor |
|---------|-------|
| **ID** | M3 |
| **Descripción** | MappingProfile falta mapeos para Venta, Configuración |
| **Ubicación** | `MappingProfile.cs` |
| **Probabilidad** | 🟡 Media (2) - Se necesitan para APIs |
| **Impacto** | 🟡 Medio (2) - Conversión manual |
| **Riesgo** | 2 × 2 = **4 (MEDIO)** |
| **Estado Actual** | 🟡 Activo |
| **Mitigación** | Agregar mapeos faltantes |
| **Plazo** | Fase 2 |

---

#### [M4] Entidades Sin Campos Críticos
| Aspecto | Valor |
|---------|-------|
| **ID** | M4 |
| **Descripción** | Prenda falta StockMinimo, Venta falta CreadoPor, etc |
| **Ubicación** | Entidades en `Domain/Entities/` |
| **Probabilidad** | 🟡 Media (2) - Necesarios para reportes |
| **Impacto** | 🟡 Medio (2) - Reportes incompletos |
| **Riesgo** | 2 × 2 = **4 (MEDIO)** |
| **Estado Actual** | 🟡 Activo |
| **Mitigación** | Agregar campos, migración EF |
| **Plazo** | Fase 4 |

---

### 🟢 BAJO (Riesgo 1-3)

#### [B1] Autorización Granular Falta
| Aspecto | Valor |
|---------|-------|
| **ID** | B1 |
| **Descripción** | Solo validación de rol, falta permisos específicos |
| **Ubicación** | Controladores |
| **Probabilidad** | 🟢 Bajo (1) - Mejora futura |
| **Impacto** | 🟡 Medio (2) - Seguridad mejorable |
| **Riesgo** | 1 × 2 = **2 (BAJO)** |
| **Estado Actual** | 🟢 Activo pero no crítico |
| **Mitigación** | Implementar policy-based authorization |
| **Plazo** | Fase 5 |

---

#### [B2] Validación Client-Side Incompleta
| Aspecto | Valor |
|---------|-------|
| **ID** | B2 |
| **Descripción** | Vistas no validan antes de enviar |
| **Ubicación** | Vistas `.cshtml` |
| **Probabilidad** | 🟢 Bajo (1) - Server-side valida |
| **Impacto** | 🟢 Bajo (1) - UX improvement |
| **Riesgo** | 1 × 1 = **1 (BAJO)** |
| **Estado Actual** | 🟢 No crítico |
| **Mitigación** | Agregar validación HTML5 |
| **Plazo** | Fase 5 |

---

#### [B3] Rutas de Imágenes Inconsistentes
| Aspecto | Valor |
|---------|-------|
| **ID** | B3 |
| **Descripción** | `.csproj` declara `images/` pero código usa `uploads/` |
| **Ubicación** | `.csproj`, `ConfiguracionController.cs` |
| **Probabilidad** | 🟢 Bajo (1) - Organizacional |
| **Impacto** | 🟢 Bajo (1) - Confusión |
| **Riesgo** | 1 × 1 = **1 (BAJO)** |
| **Estado Actual** | 🟢 No crítico |
| **Mitigación** | Estandarizar ruta |
| **Plazo** | Fase 5 |

---

## 🎯 PRIORIZACIÓN POR RIESGO

### Debe Hacerse ANTES de Producción

```
RIESGO    ID    PROBLEMA                              FASE
12        C3    Carrito pierde datos                  2
8         A1    Vendedor acceso no autorizado         3
8         A3    Descuentos fraudulentos               3
8         A5    Race condition stock                  3
6         C1    Duplicación Infrastructure            1
6         C2    BD Supabase sin variable entorno      1
4         A2    DbContext directo en VentasController 2
4         A4    Admin múltiples roles                 3
4         M2    DTOs incompletos                      2
4         M3    Mapeos incompletos                    2
4         M4    Campos entidades faltantes            4
```

### Puede Hacerse POST-Producción

```
RIESGO    ID    PROBLEMA                              FASE
2         M1    DetalleVentaDTO en lugar incorrecto   2
2         B1    Autorización granular                 5
1         B2    Validación client-side                5
1         B3    Rutas imágenes                        5
```

---

## 📈 EVOLUCIÓN DE RIESGOS

### ANTES (Ahora)
```
Riesgos Críticos: 4 (C3, A1, A3, A5)
Riesgos Altos: 4 (C1, C2, A2, A4)
Riesgos Medios: 4 (M1, M2, M3, M4)
Riesgos Bajos: 3 (B1, B2, B3)
TOTAL: 15 riesgos
```

### DESPUÉS (Objetivo: Fase 3)
```
Riesgos Críticos: 0 ✅
Riesgos Altos: 0 ✅
Riesgos Medios: 4 (Mejoras)
Riesgos Bajos: 3 (Pulido)
TOTAL: 7 riesgos (solo mejoras)
```

---

## 🛡️ PLAN DE MITIGACIÓN

### Semana 1: Mitigación de Críticos
| Día | Tarea | Riesgo Reducido |
|-----|-------|-----------------|
| 1-2 | Consolidar Infrastructure (C1) | CR1: 12→9 |
| 1-2 | Configurar var entorno (C2) | CR2: 6→3 |
| 3-5 | Carrito en Session (C3) | CR1: 12→2 |
| 3-5 | Validar vendedor (A1) | CR2: 8→2 |
| 3-5 | Validar descuentos (A3) | CR3: 8→2 |
| 3-5 | Stock transaccional (A5) | CR4: 8→2 |

**Resultado**: Riesgo promedio baja de 7.5 → 1.8

---

## 📊 HEATMAP FINAL

```
                    BAJO        MEDIO       ALTO        CRÍTICO
BAJO PROB          [B2,B3]      [M1]         []          []
PROB MEDIA          [B1]        [M3,M4]     [A2,A4]     [A1,A3,A5]
PROB ALTA           []          [C1]        [C2]        [C3]
```

---

## ✅ CHECKLIST DE RIESGOS

### Fase 1
- [ ] C1: Infrastructure consolidada
- [ ] C2: SUPABASE_PASSWORD documentada
- [ ] Riesgo: C1=6, C2=6 → 3 cada uno

### Fase 2
- [ ] A2: DbContext removido
- [ ] C3: Carrito en Session
- [ ] M1-M3: DTOs y mapeos
- [ ] Riesgo: C3=12→2, A2=4→1, M1-3=4→1 cada uno

### Fase 3
- [ ] A1: Validación vendedor-usuario
- [ ] A3: Validación descuentos
- [ ] A4: Rol Admin decisión
- [ ] A5: Stock transaccional
- [ ] Riesgo: A1-5=8→2, A4=4→1 cada uno

### Fase 4
- [ ] M4: Campos entidades
- [ ] Riesgo: M4=4→0

### Fase 5
- [ ] B1: Autorización granular
- [ ] B2-B3: UX y organización
- [ ] Riesgo: B1-3=2-1→0

---

**Generado**: 7 Julio 2026  
**Estado**: 🔴 15 RIESGOS IDENTIFICADOS - PLAN EN EJECUCIÓN  
**Próximo Review**: Después de Fase 1
