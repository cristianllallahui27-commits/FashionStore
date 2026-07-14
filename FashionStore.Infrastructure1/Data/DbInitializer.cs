using FashionStore.Domain.Entities;
using FashionStore.Infrastructure.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FashionStore.Infrastructure.Data
{
    public static class DbInitializer
    {
        // Credenciales del administrador por defecto
        private const string AdminEmail    = "admin@fashionstore.com";
        private const string AdminPassword = "Password123!";

        public static async Task Initialize(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var context = services.GetRequiredService<FashionStoreDbContext>();
                var logger      = services.GetRequiredService<ILogger<DbInitializerMarker>>();

                await SeedRoles(roleManager, logger);
                await SeedAdminUser(userManager, logger);
                await SeedAdminVendedor(context, userManager, logger);
                await SeedMetodosPago(context, logger);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<DbInitializerMarker>>();
                logger.LogError(ex, "Error en DbInitializer.Initialize");
            }
        }

        // ──────────────────────────────────────────────
        // ROLES
        // ──────────────────────────────────────────────
        private static async Task SeedRoles(
            RoleManager<IdentityRole> roleManager,
            ILogger logger)
        {
            string[] roleNames = { "Administrador", "Vendedor" };
            foreach (var role in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    if (result.Succeeded)
                        logger.LogInformation("Rol '{Role}' creado.", role);
                    else
                        logger.LogWarning("No se pudo crear rol '{Role}'.", role);
                }
            }
        }

        // ──────────────────────────────────────────────
        // USUARIO ADMINISTRADOR POR DEFECTO
        // Crea admin@fashionstore.com / Admin123! si no existe.
        // Si existe pero no tiene el rol, se lo asigna.
        // NO modifica usuarios existentes ni sus contraseñas.
        // ──────────────────────────────────────────────
        private static async Task SeedAdminUser(
            UserManager<ApplicationUser> userManager,
            ILogger logger)
        {
            var admin = await userManager.FindByEmailAsync(AdminEmail);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = AdminEmail,
                    Email    = AdminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, AdminPassword);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                        logger.LogWarning("Error creando admin: {Code} - {Desc}", err.Code, err.Description);
                    return;
                }
                logger.LogInformation("Usuario admin '{Email}' creado.", AdminEmail);
            }

            // Asegurar que tiene el rol Administrador
            if (!await userManager.IsInRoleAsync(admin, "Administrador"))
            {
                await userManager.AddToRoleAsync(admin, "Administrador");
                logger.LogInformation("Rol Administrador asignado a '{Email}'.", AdminEmail);
            }

            // Asegurar que también tiene el rol Vendedor (para acceder a ventas)
            if (!await userManager.IsInRoleAsync(admin, "Vendedor"))
            {
                await userManager.AddToRoleAsync(admin, "Vendedor");
                logger.LogInformation("Rol Vendedor asignado a '{Email}'.", AdminEmail);
            }
        }

        // ──────────────────────────────────────────────
        // VENDEDOR PARA ADMINISTRADOR
        // Crea un registro Vendedor para el admin si no existe
        // Permite que admin haga ventas sin tabla separada
        // ──────────────────────────────────────────────
        private static async Task SeedAdminVendedor(
            FashionStoreDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger logger)
        {
            // Buscar si ya existe un vendedor con DNI especial para admin
            var vendedorAdmin = await context.Vendedores.FirstOrDefaultAsync(v => v.DNI == "ADMIN0001");

            if (vendedorAdmin == null)
            {
                // Obtener el usuario admin
                var admin = await userManager.FindByEmailAsync(AdminEmail);
                if (admin == null)
                {
                    logger.LogWarning("Admin user not found. Cannot create admin vendor.");
                    return;
                }

                // Crear vendedor para el administrador
                vendedorAdmin = new Vendedor
                {
                    Nombres = "Administrador",
                    Apellidos = "Sistema",
                    DNI = "ADMIN0001",
                    Correo = admin.Email,
                    // UsuarioId NOT MAPPED (no se asigna aquí)
                    Estado = true,
                    UltimaPasswordAdmin = null
                };

                await context.Vendedores.AddAsync(vendedorAdmin);
                await context.SaveChangesAsync();

                logger.LogInformation("Admin vendor created. VendedorId: {VendedorId}, Correo: {Correo}", vendedorAdmin.Id, admin.Email);
            }
            else
            {
                // UsuarioId is [NotMapped], no need to update it
                // Just ensure the vendor record exists
                logger.LogInformation("Admin vendor already exists. VendedorId: {VendedorId}", vendedorAdmin.Id);
            }
        }

        // ──────────────────────────────────────────────
        // MÉTODOS DE PAGO
        // Crea 5 métodos de pago estándar si no existen
        // ──────────────────────────────────────────────
        private static async Task SeedMetodosPago(
            FashionStoreDbContext context,
            ILogger logger)
        {
            // Verificar si ya existen métodos de pago
            if (await context.MetodosPago.AnyAsync())
            {
                logger.LogInformation("Métodos de pago ya existen en la BD.");
                return;
            }

            var metodosPago = new List<MetodoPago>
            {
                new MetodoPago
                {
                    Nombre = "Efectivo",
                    Descripcion = "Pago en efectivo (dinero en mano)",
                    Activo = true
                },
                new MetodoPago
                {
                    Nombre = "Tarjeta de Crédito",
                    Descripcion = "Pago con tarjeta de crédito",
                    Activo = true
                },
                new MetodoPago
                {
                    Nombre = "Tarjeta de Débito",
                    Descripcion = "Pago con tarjeta de débito",
                    Activo = true
                },
                new MetodoPago
                {
                    Nombre = "Transferencia Bancaria",
                    Descripcion = "Pago mediante transferencia bancaria",
                    Activo = true
                },
                new MetodoPago
                {
                    Nombre = "Cheque",
                    Descripcion = "Pago con cheque personal o de empresa",
                    Activo = true
                }
            };

            await context.MetodosPago.AddRangeAsync(metodosPago);
            await context.SaveChangesAsync();

            logger.LogInformation("5 métodos de pago creados exitosamente.");
        }
    }

    // Clase marcadora para ILogger<T> sin exponer el tipo interno
    internal class DbInitializerMarker { }
}
