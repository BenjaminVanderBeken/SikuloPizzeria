using Dapper;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Interfaces;
using SikuloPizzeria.Infrastructure.Data;

namespace SikuloPizzeria.Infrastructure.Repositories;

public sealed class CategorieRepository : ICategorieRepository
{
private readonly IDbConnectionFactory _connectionFactory;


public CategorieRepository(IDbConnectionFactory connectionFactory)
{
    _connectionFactory = connectionFactory;
}

public async Task<IEnumerable<Categorie>> GetAllAsync()
{
    const string sql = """
        SELECT
            id AS Id,
            nom AS Nom,
            description AS Description,
            ordre_affichage AS OrdreAffichage,
            actif AS Actif
        FROM categories
        ORDER BY ordre_affichage, nom;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    return await connection.QueryAsync<Categorie>(sql);
}

public async Task<Categorie?> GetByIdAsync(int id)
{
    const string sql = """
        SELECT
            id AS Id,
            nom AS Nom,
            description AS Description,
            ordre_affichage AS OrdreAffichage,
            actif AS Actif
        FROM categories
        WHERE id = @Id;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    return await connection.QuerySingleOrDefaultAsync<Categorie>(
        sql,
        new { Id = id }
    );
}

public async Task<int> CreateAsync(Categorie categorie)
{
    const string sql = """
        INSERT INTO categories (
            nom,
            description,
            ordre_affichage,
            actif
        )
        VALUES (
            @Nom,
            @Description,
            @OrdreAffichage,
            @Actif
        );

        SELECT LAST_INSERT_ID();
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    return await connection.ExecuteScalarAsync<int>(sql, categorie);
}

public async Task<bool> UpdateAsync(Categorie categorie)
{
    const string sql = """
        UPDATE categories
        SET
            nom = @Nom,
            description = @Description,
            ordre_affichage = @OrdreAffichage,
            actif = @Actif
        WHERE id = @Id;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int affectedRows = await connection.ExecuteAsync(sql, categorie);

    return affectedRows > 0;
}

public async Task<bool> DeleteAsync(int id)
{
    const string sql = """
        UPDATE categories
        SET actif = FALSE
        WHERE id = @Id
          AND actif = TRUE;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int affectedRows = await connection.ExecuteAsync(
        sql,
        new { Id = id }
    );

    return affectedRows > 0;
}

public async Task<bool> ReactivateAsync(int id)
{
    const string sql = """
        UPDATE categories
        SET actif = TRUE
        WHERE id = @Id
          AND actif = FALSE;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int affectedRows = await connection.ExecuteAsync(
        sql,
        new { Id = id }
    );

    return affectedRows > 0;
}

public async Task<bool> HasProductsAsync(int id)
{
    const string sql = """
        SELECT COUNT(*)
        FROM produits
        WHERE categorie_id = @Id;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int productCount = await connection.ExecuteScalarAsync<int>(
        sql,
        new { Id = id }
    );

    return productCount > 0;
}

public async Task<bool> DeletePermanentlyAsync(int id)
{
    const string sql = """
        DELETE FROM categories
        WHERE id = @Id;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int affectedRows = await connection.ExecuteAsync(
        sql,
        new { Id = id }
    );

    return affectedRows > 0;
}

public async Task<bool> ExistsByNameAsync(
    string nom,
    int? excludedId = null
)
{
    const string sql = """
        SELECT COUNT(*)
        FROM categories
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
