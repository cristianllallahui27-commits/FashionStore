# 📊 Esquema de Base de Datos - FashionStore

## Diagrama de Relaciones (ER Diagram)

```
┌─────────────────────────────────────────────────────────────────────────┐
│                         FASHIONSTORE DATABASE                            │
└─────────────────────────────────────────────────────────────────────────┘

TABLAS DE REFERENCIA
═══════════════════════════════════════════════════════════════════════════

┌──────────────────────┐         ┌──────────────────────┐
│   CATEGORIAS         │         │   METODOS_PAGO       │
├──────────────────────┤         ├──────────────────────┤
│ PK: Id (serial)      │         │ PK: Id (serial)      │
│    Nombre (varchar)  │         │    Nombre (varchar)  │
│    Descripcion       │         │    Descripcion       │
└──────────────────────┘         └──────────────────────┘
		  △                               △
		  │ 1:N                          │ 1:N
		  │ (CategoriaId)                │ (MetodoPagoId)
		  │                              │
		  │              ┌──────────────────────────────────┐
		  │              │        VENTAS                    │
		  │              ├──────────────────────────────────┤
		  └──────────────┤ PK: Id (serial)                  │
						 │    Fecha (timestamp)             │
						 │    FK: ClienteId (int)           │
						 │    FK: VendedorId (int)          │
						 │    FK: MetodoPagoId (int)        │
						 │    FK: DescuentoAutId (int)      │
						 │    Total (numeric 10,2)          │
						 │    MontoRecibido (numeric)       │
						 │    Vuelto (numeric)              │
						 │    Descuento (numeric)           │
						 └──────────────────────────────────┘
									│ 1:N
									│ (VentaId)
									▼
						 ┌──────────────────────────────────┐
						 │   DETALLEVENTAS                  │
						 ├──────────────────────────────────┤
						 │ PK: Id (serial)                  │
						 │ FK: VentaId (int)                │
						 │ FK: PrendaId (int)       ────────┤─────────┐
						 │    Cantidad (int)                │         │
						 │    Precio (numeric 10,2)         │         │
						 │    Subtotal (numeric 10,2)       │         │
						 └──────────────────────────────────┘         │
																	  │
																  1:N │
																	  │
						 ┌──────────────────────┐                     │
						 │     PRENDAS          │◄────────────────────┘
						 ├──────────────────────┤
						 │ PK: Id (serial)      │
						 │    Nombre (varchar)  │
						 │    Descripcion       │
						 │    Talla (varchar)   │
						 │    Color (varchar)   │
						 │    Precio (numeric)  │
						 │    Stock (int)       │
						 │    ImagenUrl         │
						 │    CodigoBarra       │
						 │ FK: CategoriaId      │
						 └──────────────────────┘


TABLAS PRINCIPALES
═══════════════════════════════════════════════════════════════════════════

┌──────────────────────┐
│    CLIENTES          │
├──────────────────────┤
│ PK: Id (serial)      │
│    NombreCompleto    │
│    DNI (unique)      │
│    Telefono          │
│    Email             │
│    Direccion         │
└──────────────────────┘
		△
		│ 1:N
		│ (ClienteId)
		│
		└─────────────┬─────────────┬──────────────┤ VENTAS


┌──────────────────────┐
│    VENDEDORES        │
├──────────────────────┤
│ PK: Id (serial)      │
│    Nombres           │
│    Apellidos         │
│    DNI (unique)      │
│    Telefono          │
│    Correo            │
│    Estado (boolean)  │
│    UltimaPasswordAdm │
└──────────────────────┘
		△
		│ 1:N
		│ (VendedorId)
		│
		└─────────────┤ VENTAS


┌──────────────────────────────────────────┐
│   DESCUENTOS_AUTORIZADOS                 │
├──────────────────────────────────────────┤
│ PK: Id (serial)                          │
│    Descripcion (varchar 150)             │
│    Porcentaje (numeric 5,2) CHECK > 0    │
│    FechaCreacion (timestamp)             │
│    FechaVencimiento (timestamp)          │
└──────────────────────────────────────────┘
		△
		│ 1:N (opcional)
		│ (DescuentoAutId)
		│
		└─────────────┤ VENTAS


┌──────────────────────────────────────────┐
│   CONFIGURACION_SISTEMA                  │
├──────────────────────────────────────────┤
│ PK: Id (serial)                          │
│    TiendaNombre (varchar)                │
│    TiendaDescripcion (varchar)           │
│    TiendaTelefono (varchar)              │
│    TiendaEmail (varchar)                 │
│    TiendaDireccion (varchar)             │
│    DescuentoMaxAutoSinAutorizacion       │
│    FechaActualizacion (timestamp)        │
└──────────────────────────────────────────┘


TABLAS DE IDENTIDAD (ASP.NET Core Identity)
═══════════════════════════════════════════════════════════════════════════

┌──────────────────────────────────────────┐
│   AspNetUsers                            │
├──────────────────────────────────────────┤
│ PK: Id (varchar 36)                      │
│    UserName (varchar 256)                │
│    Email (varchar 256)                   │
│    PasswordHash (text)                   │
│    PhoneNumber (varchar 20)              │
│    EmailConfirmed (boolean)              │
│    PhoneNumberConfirmed (boolean)        │
│    TwoFactorEnabled (boolean)            │
│    LockoutEnd (timestamp)                │
│    LockoutEnabled (boolean)              │
│    AccessFailedCount (int)               │
│    FechaRegistro (timestamp)             │
└──────────────────────────────────────────┘
		△                     △
		│ N:N                 │ N:N
		│                     │
		└─────────────────────┴─────────────┐
											│
┌──────────────────────────────────────────┼────────────────────────┐
│   AspNetRoles                            │                        │
├──────────────────────────────────────────┤                        │
│ PK: Id (varchar 36)                      │                        │
│    Name (varchar 256)                    │                        │
└──────────────────────────────────────────┘                        │
																	│
				   ┌────────────────────────────────────────────────┘
				   │
		┌──────────────────────────────────────────┐
		│   AspNetUserRoles                        │
		├──────────────────────────────────────────┤
		│ PK: UserId, RoleId                       │
		│ FK: UserId → AspNetUsers                 │
		│ FK: RoleId → AspNetRoles                 │
		└──────────────────────────────────────────┘


ESTADÍSTICAS DE TABLAS
═══════════════════════════════════════════════════════════════════════════

Total de Tablas: 16
│
├─ De Dominio: 9
│  ├─ Categorias
│  ├─ Prendas
│  ├─ Clientes
│  ├─ Vendedores
│  ├─ MetodosPago
│  ├─ Ventas
│  ├─ DetalleVentas
│  ├─ DescuentosAutorizados
│  └─ ConfiguracionSistema
│
├─ De Identidad: 7
│  ├─ AspNetUsers
│  ├─ AspNetRoles
│  ├─ AspNetUserRoles
│  ├─ AspNetUserClaims
│  ├─ AspNetUserLogins
│  ├─ AspNetRoleClaims
│  └─ AspNetUserTokens


ÍNDICES PRINCIPALES
═══════════════════════════════════════════════════════════════════════════

Tabla              │ Índices
─────────────────────────────────────────────────────────────────────────
Categorias         │ Nombre
Prendas            │ Nombre, CategoriaId, CodigoBarra
Clientes           │ DNI, NombreCompleto, Email
Vendedores         │ DNI, Correo, Estado
Ventas             │ Fecha, ClienteId, VendedorId
DetalleVentas      │ VentaId, PrendaId
DescuentosAut.     │ FechaCreacion
AspNetUsers        │ UserName, Email
AspNetRoles        │ NormalizedName
AspNetUserRoles    │ RoleId


RESTRICCIONES PRINCIPALES
═══════════════════════════════════════════════════════════════════════════

Tabla              │ Restricción
─────────────────────────────────────────────────────────────────────────
Prendas            │ Precio > 0, Stock >= 0
Vendedores         │ DNI único, Estado booleano
Clientes           │ DNI único
DescuentosAut.     │ Porcentaje > 0 AND <= 100
Ventas             │ Total >= 0, Descuento >= 0
DetalleVentas      │ Cantidad > 0, Precio > 0
MetodosPago        │ Nombre único
Claves Foráneas    │ ON DELETE RESTRICT (protege integridad)
─────────────────────────────────────────────────────────────────────────


FLUJO DE DATOS PRINCIPALES
═════════════════════════════════════════════════════════════════════════

[Cliente] → [Venta] → [DetalleVenta] → [Prenda] → [Categoria]
						 │
						 └──→ [MetodoPago]
						 │
						 └──→ [DescuentoAutorizado] (opcional)

[Vendedor] → [Venta]

[Usuario] ← (ASP.NET Identity) → [Rol]


INTEGRIDAD REFERENCIAL
═════════════════════════════════════════════════════════════════════════

- Prendas.CategoriaId → Categorias.Id (ON DELETE RESTRICT)
- Ventas.ClienteId → Clientes.Id (ON DELETE RESTRICT)
- Ventas.VendedorId → Vendedores.Id (ON DELETE RESTRICT)
- Ventas.MetodoPagoId → MetodosPago.Id (ON DELETE RESTRICT)
- DetalleVentas.VentaId → Ventas.Id (ON DELETE CASCADE)
- DetalleVentas.PrendaId → Prendas.Id (ON DELETE RESTRICT)
- AspNetUserRoles.UserId → AspNetUsers.Id (ON DELETE CASCADE)
- AspNetUserRoles.RoleId → AspNetRoles.Id (ON DELETE CASCADE)
- (y más relaciones de Identity)
