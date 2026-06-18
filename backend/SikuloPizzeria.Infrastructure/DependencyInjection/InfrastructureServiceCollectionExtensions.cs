using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SikuloPizzeria.Core.Interfaces;
using SikuloPizzeria.Infrastructure.Data;
using SikuloPizzeria.Infrastructure.Repositories;

namespace SikuloPizzeria.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string? connectionString =
            configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "La chaîne de connexion 'DefaultConnection' est absente.");
        }

        services.AddSingleton<IDbConnectionFactory>(
            new MySqlConnectionFactory(connectionString));

        services.AddScoped<IProduitRepository, ProduitRepository>();
        services.AddScoped<ICategorieRepository, CategorieRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();

        return services;
    }
}