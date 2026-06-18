using Dapper;
using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Interfaces;
using SikuloPizzeria.Infrastructure.Data;

namespace SikuloPizzeria.Infrastructure.Repositories;

public sealed class CompositionProduitRepository : ICompositionProduitRepository
{
private readonly IDbConnectionFactory _connectionFactory;

public CompositionProduitRepository(IDbConnectionFactory connectionFactory)
{
    _connectionFactory = connectionFactory;
}

public async Task<IEnumerable<CompositionProduitDto>> GetAllAsync()
{
    const string sql = """
        SELECT
            cp.id AS Id,
            cp.produit_id AS ProduitId,
            p.nom AS ProduitNom,
            cp.ingredient_id AS IngredientId,
            i.nom AS IngredientNom,
            cp.quantite AS Quantite,
            cp.unite AS Unite,
            cp.ordre_affichage AS OrdreAffichage
        FROM composition_produits cp
        INNER JOIN produits p ON p.id = cp.produit_id
        INNER JOIN ingredients i ON i.id = cp.ingredient_id
        ORDER BY p.nom, cp.ordre_affichage, i.nom;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    return await connection.QueryAsync<CompositionProduitDto>(sql);
}

public async Task<IEnumerable<CompositionProduitDto>> GetByProduitIdAsync(
    int produitId)
{
    const string sql = """
        SELECT
            cp.id AS Id,
            cp.produit_id AS ProduitId,
            p.nom AS ProduitNom,
            cp.ingredient_id AS IngredientId,
            i.nom AS IngredientNom,
            cp.quantite AS Quantite,
            cp.unite AS Unite,
            cp.ordre_affichage AS OrdreAffichage
        FROM composition_produits cp
        INNER JOIN produits p ON p.id = cp.produit_id
        INNER JOIN ingredients i ON i.id = cp.ingredient_id
        WHERE cp.produit_id = @ProduitId
        ORDER BY cp.ordre_affichage, i.nom;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    return await connection.QueryAsync<CompositionProduitDto>(
        sql,
        new { ProduitId = produitId }
    );
}

public async Task<CompositionProduitDto?> GetByIdAsync(int id)
{
    const string sql = """
        SELECT
            cp.id AS Id,
            cp.produit_id AS ProduitId,
            p.nom AS ProduitNom,
            cp.ingredient_id AS IngredientId,
            i.nom AS IngredientNom,
            cp.quantite AS Quantite,
            cp.unite AS Unite,
            cp.ordre_affichage AS OrdreAffichage
        FROM composition_produits cp
        INNER JOIN produits p ON p.id = cp.produit_id
        INNER JOIN ingredients i ON i.id = cp.ingredient_id
        WHERE cp.id = @Id;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    return await connection.QuerySingleOrDefaultAsync<CompositionProduitDto>(
        sql,
        new { Id = id }
    );
}

public async Task<int> CreateAsync(CompositionProduit composition)
{
    const string sql = """
        INSERT INTO composition_produits
        (
            produit_id,
            ingredient_id,
            quantite,
            unite,
            ordre_affichage
        )
        VALUES
        (
            @ProduitId,
            @IngredientId,
            @Quantite,
            @Unite,
            @OrdreAffichage
        );

        SELECT LAST_INSERT_ID();
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    return await connection.ExecuteScalarAsync<int>(
        sql,
        composition
    );
}

public async Task<bool> DeleteAsync(int id)
{
    const string sql = """
        DELETE FROM composition_produits
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

public async Task<bool> ExistsAsync(
    int produitId,
    int ingredientId)
{
    const string sql = """
        SELECT COUNT(*)
        FROM composition_produits
        WHERE produit_id = @ProduitId
          AND ingredient_id = @IngredientId;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int count = await connection.ExecuteScalarAsync<int>(
        sql,
        new
        {
            ProduitId = produitId,
            IngredientId = ingredientId
        }
    );

    return count > 0;
}

}
