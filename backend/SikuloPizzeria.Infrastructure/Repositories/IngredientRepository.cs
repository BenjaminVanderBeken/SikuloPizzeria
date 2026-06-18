using Dapper;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Interfaces;
using SikuloPizzeria.Infrastructure.Data;

namespace SikuloPizzeria.Infrastructure.Repositories;

public sealed class IngredientRepository : IIngredientRepository
{
private readonly IDbConnectionFactory _connectionFactory;

public IngredientRepository(IDbConnectionFactory connectionFactory)
{
    _connectionFactory = connectionFactory;
}

public async Task<IEnumerable<Ingredient>> GetAllAsync()
{
    const string sql = """
        SELECT
            id AS Id,
            nom AS Nom,
            type AS Type,
            stock_actuel AS StockActuel,
            unite_mesure AS UniteMesure,
            prix_unitaire AS PrixUnitaire,
            allergenes AS Allergenes,
            actif AS Actif
        FROM ingredients
        ORDER BY nom;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    return await connection.QueryAsync<Ingredient>(sql);
}

public async Task<Ingredient?> GetByIdAsync(int id)
{
    const string sql = """
        SELECT
            id AS Id,
            nom AS Nom,
            type AS Type,
            stock_actuel AS StockActuel,
            unite_mesure AS UniteMesure,
            prix_unitaire AS PrixUnitaire,
            allergenes AS Allergenes,
            actif AS Actif
        FROM ingredients
        WHERE id = @Id;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    return await connection.QuerySingleOrDefaultAsync<Ingredient>(
        sql,
        new { Id = id }
    );
}

public async Task<int> CreateAsync(Ingredient ingredient)
{
    const string sql = """
        INSERT INTO ingredients (
            nom,
            type,
            stock_actuel,
            unite_mesure,
            prix_unitaire,
            allergenes,
            actif
        )
        VALUES (
            @Nom,
            @Type,
            @StockActuel,
            @UniteMesure,
            @PrixUnitaire,
            @Allergenes,
            @Actif
        );

        SELECT LAST_INSERT_ID();
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    return await connection.ExecuteScalarAsync<int>(sql, ingredient);
}

public async Task<bool> UpdateAsync(Ingredient ingredient)
{
    const string sql = """
        UPDATE ingredients
        SET
            nom = @Nom,
            type = @Type,
            stock_actuel = @StockActuel,
            unite_mesure = @UniteMesure,
            prix_unitaire = @PrixUnitaire,
            allergenes = @Allergenes,
            actif = @Actif
        WHERE id = @Id;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int affectedRows = await connection.ExecuteAsync(sql, ingredient);
    return affectedRows > 0;
}

public async Task<bool> DisableAsync(int id)
{
    const string sql = """
        UPDATE ingredients
        SET actif = FALSE
        WHERE id = @Id
          AND actif = TRUE;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
    return affectedRows > 0;
}

public async Task<bool> ReactivateAsync(int id)
{
    const string sql = """
        UPDATE ingredients
        SET actif = TRUE
        WHERE id = @Id
          AND actif = FALSE;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
    return affectedRows > 0;
}

public async Task<bool> IsUsedInCompositionAsync(int id)
{
    const string sql = """
        SELECT COUNT(*)
        FROM composition_produits
        WHERE ingredient_id = @Id;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int count = await connection.ExecuteScalarAsync<int>(
        sql,
        new { Id = id }
    );

    return count > 0;
}

public async Task<bool> DeletePermanentlyAsync(int id)
{
    const string sql = """
        DELETE FROM ingredients
        WHERE id = @Id;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
    return affectedRows > 0;
}

public async Task<bool> ExistsByNameAsync(
    string nom,
    int? excludedId = null)
{
    const string sql = """
        SELECT COUNT(*)
        FROM ingredients
        WHERE LOWER(nom) = LOWER(@Nom)
          AND (@ExcludedId IS NULL OR id <> @ExcludedId);
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int count = await connection.ExecuteScalarAsync<int>(
        sql,
        new
        {
            Nom = nom,
            ExcludedId = excludedId
        }
    );

    return count > 0;
}


}
