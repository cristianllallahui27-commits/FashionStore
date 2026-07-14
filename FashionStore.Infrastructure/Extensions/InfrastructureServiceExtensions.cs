using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using FashionStore.Infrastructure.Context;

namespace FashionStore.Infrastructure.Extensions;

/// <summary>
/// Extensiones para registrar servicios de infraestructura
/// </summary>
public static class InfrastructureServiceExtensions
{
    /// <summary>
    /// Registra FashionStoreDbContext en el contenedor de servicios
    /// </summary>
    public static IServiceCollection AddFashionStoreInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<FashionStoreDbContext>(options =>
        {
            options.UseNpgsql(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "public");
            });
        });

        return services;
    }
}
