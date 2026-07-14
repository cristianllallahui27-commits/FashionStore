using FashionStore.Domain.Entities;
using FashionStore.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FashionStore.Infrastructure.Data
{
    public static class DbInitializer
    {
        private const string Admin1Email    = "admin@fashionstore.com";
        private const string Admin1Password = "Password123!";
        private const string Admin2Email    = "Admin@gmail.com";
        private const string Admin2Password = "Admin123!";

        public static async Task Initialize(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var logger      = services.GetRequiredService<ILogger<DbInitializerMarker>>();

                await SeedRoles(roleManager, logger);
                await SeedUser(userManager, Admin1Email, Admin1Password, logger);
                await SeedUser(userManager, Admin2Email, Admin2Password, logger);
                
                // Seed métodos de pago
                var unitOfWork = services.GetRequiredService<IUnitOfWork>();
                await SeedMetodosPago(unitOfWork, logger);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<DbInitializerMarker>>();
                logger.LogError(ex, "Error en DbInitializer.Initialize");
            }
        }

        private static async Task SeedRoles(
            RoleManager<IdentityRole> roleManager,
            ILogger logger)
        {
            string[] roleNames = { "Administrador", "Vendedor", "Gerente" };
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

        private static async Task SeedUser(
            UserManager<ApplicationUser> userManager,
            string email,
            string password,
            ILogger logger)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email    = email,
                    EmailConfirmed = true,
                    Activo = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                        logger.LogWarning("Error creando usuario '{Email}': {Code} - {Desc}", email, err.Code, err.Description);
                    return;
                }
                logger.LogInformation("Usuario '{Email}' creado con éxito.", email);
            }
            else
            {
                // Asegurar que esté activo
                if (!user.Activo)
                {
                    user.Activo = true;
                    await userManager.UpdateAsync(user);
                    logger.LogInformation("Usuario '{Email}' marcado como Activo.", email);
                }
            }

            if (!await userManager.IsInRoleAsync(user, "Administrador"))
            {
                await userManager.AddToRoleAsync(user, "Administrador");
                logger.LogInformation("Rol Administrador asignado a '{Email}'.", email);
            }

            if (!await userManager.IsInRoleAsync(user, "Vendedor"))
            {
                await userManager.AddToRoleAsync(user, "Vendedor");
                logger.LogInformation("Rol Vendedor asignado a '{Email}'.", email);
            }
        }

        private static async Task SeedMetodosPago(
            IUnitOfWork unitOfWork,
            ILogger logger)
        {
            try
            {
                var metodos = await unitOfWork.MetodosPago.GetAllAsync();
                
                if (metodos.Count() == 0)
                {
                    var metodosDefault = new[]
                    {
                        new MetodoPago { Nombre = "Efectivo" },
                        new MetodoPago { Nombre = "Tarjeta de Crédito" },
                        new MetodoPago { Nombre = "Tarjeta de Débito" },
                        new MetodoPago { Nombre = "Transferencia Bancaria" },
                        new MetodoPago { Nombre = "Cheque" }
                    };

                    foreach (var metodo in metodosDefault)
                    {
                        await unitOfWork.MetodosPago.AddAsync(metodo);
                    }

                    await unitOfWork.CommitAsync();
                    logger.LogInformation("✓ Insertados {Count} métodos de pago", metodosDefault.Length);
                }
                else
                {
                    logger.LogInformation("✓ Métodos de pago ya existen ({Count})", metodos.Count());
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error al seedear métodos de pago");
            }
        }
    }

    internal class DbInitializerMarker { }
}
