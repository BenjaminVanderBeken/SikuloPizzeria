using Dapper;
using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Interfaces;
using SikuloPizzeria.Infrastructure.Data;

namespace SikuloPizzeria.Infrastructure.Repositories;

public sealed class ProduitRepository : IProduitRepository
{
private readonly IDbConnectionFactory _connectionFactory;

public ProduitRepository(IDbConnectionFactory connectionFactory)
{
    _connectionFactory = connectionFactory;
}

public async Task<IEnumerable<ProduitDto>> GetAllAsync()
{
    const string sql = """
        SELECT
            p.id AS Id,
            p.categorie_id AS CategorieId,
            c.nom AS CategorieNom,
            p.nom AS Nom,
            p.description AS Description,
            p.prix_base AS PrixBase,
            p.prix_promotion AS PrixPromotion,
            p.image_url AS ImageUrl,
            p.permet_supplement AS PermetSupplement,
            p.actif AS Actif,
            p.populaire AS Populaire,
            p.ordre_affichage AS OrdreAffichage,

            vp.id AS VarianteId,
            vp.produit_id AS ProduitId,
            vp.nom AS VarianteNom,
            vp.prix AS VariantePrix,
            vp.actif AS VarianteActif,
            vp.ordre_affichage AS VarianteOrdreAffichage

        FROM produits p
        INNER JOIN categories c
            ON c.id = p.categorie_id
        LEFT JOIN variantes_produits vp
            ON vp.produit_id = p.id

        ORDER BY
            p.ordre_affichage,
            p.nom,
            vp.ordre_affichage,
            vp.nom;
        """;

    await using var connection =
        _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    var produitsParId = new Dictionary<int, ProduitDto>();

    await connection.QueryAsync<
        ProduitDto,
        VarianteProduitLigne,
        ProduitDto>(
        sql,
        (produit, variante) =>
        {
            if (!produitsParId.TryGetValue(
                    produit.Id,
                    out ProduitDto? produitExistant))
            {
                produitExistant = produit;
                produitExistant.Variantes = [];

                produitsParId.Add(
                    produitExistant.Id,
                    produitExistant);
            }

            if (variante is not null &&
                variante.VarianteId > 0)
            {
                produitExistant.Variantes.Add(
                    new VarianteProduit
                    {
                        Id = variante.VarianteId,
                        ProduitId = variante.ProduitId,
                        Nom = variante.VarianteNom,
                        Prix = variante.VariantePrix,
                        Actif = variante.VarianteActif,
                        OrdreAffichage =
                            variante.VarianteOrdreAffichage
                    });
            }

            return produitExistant;
        },
        splitOn: "VarianteId");

    return produitsParId.Values;
}

public async Task<ProduitDto?> GetByIdAsync(int id)
{
    const string sql = """
        SELECT
            p.id AS Id,
            p.categorie_id AS CategorieId,
            c.nom AS CategorieNom,
            p.nom AS Nom,
            p.description AS Description,
            p.prix_base AS PrixBase,
            p.prix_promotion AS PrixPromotion,
            p.image_url AS ImageUrl,
            p.permet_supplement AS PermetSupplement,
            p.actif AS Actif,
            p.populaire AS Populaire,
            p.ordre_affichage AS OrdreAffichage,

            vp.id AS VarianteId,
            vp.produit_id AS ProduitId,
            vp.nom AS VarianteNom,
            vp.prix AS VariantePrix,
            vp.actif AS VarianteActif,
            vp.ordre_affichage AS VarianteOrdreAffichage

        FROM produits p
        INNER JOIN categories c
            ON c.id = p.categorie_id
        LEFT JOIN variantes_produits vp
            ON vp.produit_id = p.id

        WHERE p.id = @Id

        ORDER BY
            vp.ordre_affichage,
            vp.nom;
        """;

    await using var connection =
        _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    ProduitDto? produitTrouve = null;

    await connection.QueryAsync<
        ProduitDto,
        VarianteProduitLigne,
        ProduitDto>(
        sql,
        (produit, variante) =>
        {
            if (produitTrouve is null)
            {
                produitTrouve = produit;
                produitTrouve.Variantes = [];
            }

            if (variante is not null &&
                variante.VarianteId > 0)
            {
                produitTrouve.Variantes.Add(
                    new VarianteProduit
                    {
                        Id = variante.VarianteId,
                        ProduitId = variante.ProduitId,
                        Nom = variante.VarianteNom,
                        Prix = variante.VariantePrix,
                        Actif = variante.VarianteActif,
                        OrdreAffichage =
                            variante.VarianteOrdreAffichage
                    });
            }

            return produitTrouve;
        },
        new { Id = id },
        splitOn: "VarianteId");

    return produitTrouve;
}

public async Task<int> CreateAsync(
    Produit produit,
    IReadOnlyCollection<VarianteProduit> variantes)
{
    const string sqlProduit = """
        INSERT INTO produits (
            categorie_id,
            nom,
            description,
            prix_base,
            prix_promotion,
            image_url,
            permet_supplement,
            actif,
            populaire,
            ordre_affichage
        )
        VALUES (
            @CategorieId,
            @Nom,
            @Description,
            @PrixBase,
            @PrixPromotion,
            @ImageUrl,
            @PermetSupplement,
            @Actif,
            @Populaire,
            @OrdreAffichage
        );

        SELECT LAST_INSERT_ID();
        """;

    const string sqlVariante = """
        INSERT INTO variantes_produits (
            produit_id,
            nom,
            prix,
            actif,
            ordre_affichage
        )
        VALUES (
            @ProduitId,
            @Nom,
            @Prix,
            @Actif,
            @OrdreAffichage
        );
        """;

    await using var connection =
        _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    await using var transaction =
        await connection.BeginTransactionAsync();

    try
    {
        int produitId =
            await connection.ExecuteScalarAsync<int>(
                sqlProduit,
                produit,
                transaction);

        foreach (VarianteProduit variante in variantes)
        {
            variante.ProduitId = produitId;

            await connection.ExecuteAsync(
                sqlVariante,
                variante,
                transaction);
        }

        await transaction.CommitAsync();

        return produitId;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}

public async Task<bool> UpdateAsync(
    Produit produit,
    IReadOnlyCollection<VarianteProduit> variantes)
{
    const string sqlProduit = """
        UPDATE produits
        SET
            categorie_id = @CategorieId,
            nom = @Nom,
            description = @Description,
            prix_base = @PrixBase,
            prix_promotion = @PrixPromotion,
            image_url = @ImageUrl,
            permet_supplement = @PermetSupplement,
            actif = @Actif,
            populaire = @Populaire,
            ordre_affichage = @OrdreAffichage
        WHERE id = @Id;
        """;

    const string sqlDesactiverVariantes = """
        UPDATE variantes_produits
        SET actif = FALSE
        WHERE produit_id = @ProduitId;
        """;

    const string sqlModifierVariante = """
        UPDATE variantes_produits
        SET
            nom = @Nom,
            prix = @Prix,
            actif = @Actif,
            ordre_affichage = @OrdreAffichage
        WHERE id = @Id
          AND produit_id = @ProduitId;
        """;

    const string sqlAjouterVariante = """
        INSERT INTO variantes_produits (
            produit_id,
            nom,
            prix,
            actif,
            ordre_affichage
        )
        VALUES (
            @ProduitId,
            @Nom,
            @Prix,
            @Actif,
            @OrdreAffichage
        );
        """;

    await using var connection =
        _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    await using var transaction =
        await connection.BeginTransactionAsync();

    try
    {
        int lignesModifiees =
            await connection.ExecuteAsync(
                sqlProduit,
                produit,
                transaction);

        if (lignesModifiees == 0)
        {
            await transaction.RollbackAsync();
            return false;
        }

        await connection.ExecuteAsync(
            sqlDesactiverVariantes,
            new { ProduitId = produit.Id },
            transaction);

        foreach (VarianteProduit variante in variantes)
        {
            variante.ProduitId = produit.Id;

            if (variante.Id > 0)
            {
                await connection.ExecuteAsync(
                    sqlModifierVariante,
                    variante,
                    transaction);
            }
            else
            {
                await connection.ExecuteAsync(
                    sqlAjouterVariante,
                    variante,
                    transaction);
            }
        }

        await transaction.CommitAsync();

        return true;
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}

public async Task<bool> DisableAsync(int id)
{
    const string sql = """
        UPDATE produits
        SET actif = FALSE
        WHERE id = @Id
          AND actif = TRUE;
        """;

    await using var connection =
        _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    int lignesModifiees =
        await connection.ExecuteAsync(
            sql,
            new { Id = id });

    return lignesModifiees > 0;
}

public async Task<bool> ReactivateAsync(int id)
{
    const string sql = """
        UPDATE produits
        SET actif = TRUE
        WHERE id = @Id
          AND actif = FALSE;
        """;

    await using var connection =
        _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    int lignesModifiees =
        await connection.ExecuteAsync(
            sql,
            new { Id = id });

    return lignesModifiees > 0;
}

public async Task<bool> IsUsedInOrderAsync(int id)
{
    const string sql = """
        SELECT COUNT(*)
        FROM details_commandes
        WHERE produit_id = @Id;
        """;

    await using var connection =
        _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    int nombreUtilisations =
        await connection.ExecuteScalarAsync<int>(
            sql,
            new { Id = id });

    return nombreUtilisations > 0;
}

public async Task<bool> DeleteAsync(int id)
{
    const string sql = """
        DELETE FROM produits
        WHERE id = @Id;
        """;

    await using var connection =
        _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    int lignesSupprimees =
        await connection.ExecuteAsync(
            sql,
            new { Id = id });

    return lignesSupprimees > 0;
}

public async Task<bool> ExistsByNameAsync(
    int categorieId,
    string nom,
    int? excludedId = null)
{
    const string sql = """
        SELECT COUNT(*)
        FROM produits
        WHERE categorie_id = @CategorieId
          AND LOWER(nom) = LOWER(@Nom)
          AND (
              @ExcludedId IS NULL
              OR id <> @ExcludedId
          );
        """;

    await using var connection =
        _connectionFactory.CreateConnection();

    await connection.OpenAsync();

    int nombre =
        await connection.ExecuteScalarAsync<int>(
            sql,
            new
            {
                CategorieId = categorieId,
                Nom = nom,
                ExcludedId = excludedId
            });

    return nombre > 0;
}

private sealed class VarianteProduitLigne
{
    public int VarianteId { get; set; }

    public int ProduitId { get; set; }

    public string VarianteNom { get; set; } = string.Empty;

    public decimal VariantePrix { get; set; }

    public bool VarianteActif { get; set; }

    public int VarianteOrdreAffichage { get; set; }
}


}
