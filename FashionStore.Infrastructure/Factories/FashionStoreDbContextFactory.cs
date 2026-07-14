using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using FashionStore.Infrastructure.Context;

namespace FashionStore.Infrastructure.Factories;

/// <summary>
/// Factory para crear instancias de FashionStoreDbContext
/// Necesaria para las migraciones de Entity Framework Core
/// </summary>
public class FashionStoreDbContextFactory : IDesignTimeDbContextFactory<FashionStoreDbContext>
{
    public FashionStoreDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FashionStoreDbContext>();

        // Usar variable de entorno SUPABASE_PASSWORD si está disponible para no incrustar secretos
        var password = Environment.GetEnvironmentVariable("SUPABASE_PASSWORD") ?? string.Empty;
        var connectionString = $"Host=db.bajbvebkmacdnllnxvkv.supabase.co;Port=5432;Database=postgres;Username=postgres;Password={password};SSL Mode=Require;Trust Server Certificate=false;";

        optionsBuilder.UseNpgsql(connectionString, options =>
        {
            options.MigrationsHistoryTable("__EFMigrationsHistory", "public");
        });

        return new FashionStoreDbContext(optionsBuilder.Options);
    }
}
