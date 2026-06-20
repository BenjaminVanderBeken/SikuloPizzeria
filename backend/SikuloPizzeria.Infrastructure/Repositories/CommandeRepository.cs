using Dapper;
using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Interfaces;
using SikuloPizzeria.Infrastructure.Data;

namespace SikuloPizzeria.Infrastructure.Repositories;

public sealed class CommandeRepository : ICommandeRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public CommandeRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<CommandeDto>> GetAllAsync()
    {
        const string commandesSql = """
            SELECT
                c.id AS Id,
                c.numero_commande AS NumeroCommande,
                c.client_id AS ClientId,
                CASE
                    WHEN c.client_id IS NULL THEN 'Vente comptoir'
                    ELSE TRIM(CONCAT(cl.nom, ' ', COALESCE(cl.prenom, '')))
                END AS ClientNomComplet,
                c.date_commande AS DateCommande,
                c.date_livraison_prevue AS DateLivraisonPrevue,
                c.date_livraison_reelle AS DateLivraisonReelle,
                c.type_commande AS TypeCommande,
                c.statut AS Statut,
                c.sous_total AS SousTotal,
                c.tva_montant AS TvaMontant,
                c.frais_livraison AS FraisLivraison,
                c.reduction_montant AS ReductionMontant,
                c.montant_total AS MontantTotal,
                c.mode_paiement AS ModePaiement,
                c.statut_paiement AS StatutPaiement,
                c.notes_client AS NotesClient,
                c.notes_cuisine AS NotesCuisine,
                c.demandes_speciales AS DemandesSpeciales
            FROM commandes c
            LEFT JOIN clients cl ON cl.id = c.client_id
            ORDER BY c.date_commande DESC, c.id DESC;
            """;

        const string detailsSql = """
            SELECT
                d.commande_id AS CommandeId,
                d.id AS Id,
                d.produit_id AS ProduitId,
                p.nom AS ProduitNom,
                d.variante_id AS VarianteId,
                d.nom_variante AS NomVariante,
                d.quantite AS Quantite,
                d.prix_unitaire AS PrixUnitaire,
                d.prix_total AS PrixTotal,
                d.notes_particulieres AS NotesParticulieres,
                d.cout_supplements AS CoutSupplements
            FROM details_commandes d
            INNER JOIN produits p ON p.id = d.produit_id
            ORDER BY d.commande_id, d.id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        List<CommandeDto> commandes =
            (await connection.QueryAsync<CommandeDto>(commandesSql)).ToList();

        IEnumerable<DetailCommandeRow> lignes =
            await connection.QueryAsync<DetailCommandeRow>(detailsSql);

        Dictionary<int, List<DetailCommandeDto>> detailsParCommande =
            lignes
                .GroupBy(ligne => ligne.CommandeId)
                .ToDictionary(
                    groupe => groupe.Key,
                    groupe => groupe.Select(ConvertirDetail).ToList()
                );

        foreach (CommandeDto commande in commandes)
        {
            commande.Details = detailsParCommande.GetValueOrDefault(
                commande.Id,
                []
            );
        }

        return commandes;
    }

    public async Task<CommandeDto?> GetByIdAsync(int id)
    {
        const string sql = """
            SELECT
                c.id AS Id,
                c.numero_commande AS NumeroCommande,
                c.client_id AS ClientId,
                CASE
                    WHEN c.client_id IS NULL THEN 'Vente comptoir'
                    ELSE TRIM(CONCAT(cl.nom, ' ', COALESCE(cl.prenom, '')))
                END AS ClientNomComplet,
                c.date_commande AS DateCommande,
                c.date_livraison_prevue AS DateLivraisonPrevue,
                c.date_livraison_reelle AS DateLivraisonReelle,
                c.type_commande AS TypeCommande,
                c.statut AS Statut,
                c.sous_total AS SousTotal,
                c.tva_montant AS TvaMontant,
                c.frais_livraison AS FraisLivraison,
                c.reduction_montant AS ReductionMontant,
                c.montant_total AS MontantTotal,
                c.mode_paiement AS ModePaiement,
                c.statut_paiement AS StatutPaiement,
                c.notes_client AS NotesClient,
                c.notes_cuisine AS NotesCuisine,
                c.demandes_speciales AS DemandesSpeciales
            FROM commandes c
            LEFT JOIN clients cl ON cl.id = c.client_id
            WHERE c.id = @Id;

            SELECT
                d.id AS Id,
                d.produit_id AS ProduitId,
                p.nom AS ProduitNom,
                d.variante_id AS VarianteId,
                d.nom_variante AS NomVariante,
                d.quantite AS Quantite,
                d.prix_unitaire AS PrixUnitaire,
                d.prix_total AS PrixTotal,
                d.notes_particulieres AS NotesParticulieres,
                d.cout_supplements AS CoutSupplements
            FROM details_commandes d
            INNER JOIN produits p ON p.id = d.produit_id
            WHERE d.commande_id = @Id
            ORDER BY d.id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        using SqlMapper.GridReader resultats =
            await connection.QueryMultipleAsync(sql, new { Id = id });

        CommandeDto? commande =
            await resultats.ReadSingleOrDefaultAsync<CommandeDto>();

        if (commande is null)
        {
            return null;
        }

        commande.Details =
            (await resultats.ReadAsync<DetailCommandeDto>()).ToList();

        return commande;
    }

    public async Task<int> CreateAsync(
        Commande commande,
        IReadOnlyCollection<DetailCommande> details)
    {
        const string commandeSql = """
            INSERT INTO commandes (
                numero_commande,
                client_id,
                promotion_id,
                date_livraison_prevue,
                date_livraison_reelle,
                type_commande,
                statut,
                sous_total,
                tva_montant,
                frais_livraison,
                reduction_montant,
                montant_total,
                mode_paiement,
                statut_paiement,
                notes_client,
                notes_cuisine,
                demandes_speciales
            )
            VALUES (
                @NumeroCommande,
                @ClientId,
                @PromotionId,
                @DateLivraisonPrevue,
                @DateLivraisonReelle,
                @TypeCommande,
                @Statut,
                @SousTotal,
                @TvaMontant,
                @FraisLivraison,
                @ReductionMontant,
                @MontantTotal,
                @ModePaiement,
                @StatutPaiement,
                @NotesClient,
                @NotesCuisine,
                @DemandesSpeciales
            );

            SELECT LAST_INSERT_ID();
            """;

        const string detailSql = """
            INSERT INTO details_commandes (
                commande_id,
                produit_id,
                variante_id,
                quantite,
                prix_unitaire,
                prix_total,
                nom_variante,
                notes_particulieres,
                cout_supplements
            )
            VALUES (
                @CommandeId,
                @ProduitId,
                @VarianteId,
                @Quantite,
                @PrixUnitaire,
                @PrixTotal,
                @NomVariante,
                @NotesParticulieres,
                @CoutSupplements
            );
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            int commandeId = await connection.ExecuteScalarAsync<int>(
                commandeSql,
                commande,
                transaction
            );

            foreach (DetailCommande detail in details)
            {
                detail.CommandeId = commandeId;
            }

            await connection.ExecuteAsync(
                detailSql,
                details,
                transaction
            );

            await transaction.CommitAsync();
            return commandeId;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateAsync(
        Commande commande,
        IReadOnlyCollection<DetailCommande> details)
    {
        const string updateCommandeSql = """
            UPDATE commandes
            SET
                client_id = @ClientId,
                date_livraison_prevue = @DateLivraisonPrevue,
                type_commande = @TypeCommande,
                sous_total = @SousTotal,
                tva_montant = @TvaMontant,
                frais_livraison = @FraisLivraison,
                reduction_montant = @ReductionMontant,
                montant_total = @MontantTotal,
                mode_paiement = @ModePaiement,
                notes_client = @NotesClient,
                notes_cuisine = @NotesCuisine,
                demandes_speciales = @DemandesSpeciales,
                date_modification = CURRENT_TIMESTAMP
            WHERE id = @Id
              AND statut IN ('EN_ATTENTE', 'CONFIRMEE');
            """;

        const string deleteDetailsSql = """
            DELETE FROM details_commandes
            WHERE commande_id = @Id;
            """;

        const string insertDetailSql = """
            INSERT INTO details_commandes (
                commande_id,
                produit_id,
                variante_id,
                quantite,
                prix_unitaire,
                prix_total,
                nom_variante,
                notes_particulieres,
                cout_supplements
            )
            VALUES (
                @CommandeId,
                @ProduitId,
                @VarianteId,
                @Quantite,
                @PrixUnitaire,
                @PrixTotal,
                @NomVariante,
                @NotesParticulieres,
                @CoutSupplements
            );
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            int lignesModifiees = await connection.ExecuteAsync(
                updateCommandeSql,
                commande,
                transaction
            );

            if (lignesModifiees == 0)
            {
                await transaction.RollbackAsync();
                return false;
            }

            await connection.ExecuteAsync(
                deleteDetailsSql,
                new { commande.Id },
                transaction
            );

            foreach (DetailCommande detail in details)
            {
                detail.CommandeId = commande.Id;
            }

            await connection.ExecuteAsync(
                insertDetailSql,
                details,
                transaction
            );

            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<bool> UpdateStatusAsync(int id, string statut)
    {
        const string sql = """
            UPDATE commandes
            SET
                statut = @Statut,
                date_livraison_reelle = CASE
                    WHEN @Statut = 'LIVREE' THEN CURRENT_TIMESTAMP
                    ELSE date_livraison_reelle
                END,
                date_modification = CURRENT_TIMESTAMP
            WHERE id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        int affectedRows = await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                Statut = statut
            }
        );

        return affectedRows > 0;
    }

    public async Task<bool> UpdatePaymentAsync(
        int id,
        string modePaiement,
        string statutPaiement)
    {
        const string sql = """
            UPDATE commandes
            SET
                mode_paiement = @ModePaiement,
                statut_paiement = @StatutPaiement,
                date_modification = CURRENT_TIMESTAMP
            WHERE id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        int affectedRows = await connection.ExecuteAsync(
            sql,
            new
            {
                Id = id,
                ModePaiement = modePaiement,
                StatutPaiement = statutPaiement
            }
        );

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        const string sql = """
            DELETE FROM commandes
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

    public async Task<bool> ExistsByNumberAsync(string numeroCommande)
    {
        const string sql = """
            SELECT COUNT(*)
            FROM commandes
            WHERE numero_commande = @NumeroCommande;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        int count = await connection.ExecuteScalarAsync<int>(
            sql,
            new { NumeroCommande = numeroCommande }
        );

        return count > 0;
    }

    private static DetailCommandeDto ConvertirDetail(
        DetailCommandeRow ligne)
    {
        return new DetailCommandeDto
        {
            Id = ligne.Id,
            ProduitId = ligne.ProduitId,
            ProduitNom = ligne.ProduitNom,
            VarianteId = ligne.VarianteId,
            NomVariante = ligne.NomVariante,
            Quantite = ligne.Quantite,
            PrixUnitaire = ligne.PrixUnitaire,
            PrixTotal = ligne.PrixTotal,
            NotesParticulieres = ligne.NotesParticulieres,
            CoutSupplements = ligne.CoutSupplements
        };
    }

    private sealed class DetailCommandeRow
    {
        public int CommandeId { get; set; }
        public int Id { get; set; }
        public int ProduitId { get; set; }
        public string ProduitNom { get; set; } = string.Empty;
        public int? VarianteId { get; set; }
        public string? NomVariante { get; set; }
        public int Quantite { get; set; }
        public decimal PrixUnitaire { get; set; }
        public decimal PrixTotal { get; set; }
        public string? NotesParticulieres { get; set; }
        public decimal CoutSupplements { get; set; }
    }
}
