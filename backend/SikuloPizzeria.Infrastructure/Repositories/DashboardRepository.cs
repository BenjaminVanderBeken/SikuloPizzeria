using Dapper;
using SikuloPizzeria.Core.DTOs;
using SikuloPizzeria.Core.Interfaces;
using SikuloPizzeria.Infrastructure.Data;

namespace SikuloPizzeria.Infrastructure.Repositories;

public sealed class DashboardRepository : IDashboardRepository
{
private readonly IDbConnectionFactory _connectionFactory;


public DashboardRepository(IDbConnectionFactory connectionFactory)
{
    _connectionFactory = connectionFactory;
}

public async Task<DashboardStatsDto> GetStatsAsync()
{
    const string sql = """
        SELECT
            (
                SELECT COUNT(*)
                FROM produits
                WHERE actif = 1
            ) AS ProduitsActifs,

            (
                SELECT COUNT(*)
                FROM clients
                WHERE actif = 1
            ) AS ClientsActifs,

            (
                SELECT COUNT(*)
                FROM commandes
            ) AS CommandesTotales,

            (
                SELECT COUNT(*)
                FROM commandes
                WHERE DATE(date_commande) = CURRENT_DATE()
            ) AS CommandesDuJour,

            (
                SELECT COUNT(*)
                FROM commandes
                WHERE statut = 'EN_ATTENTE'
            ) AS CommandesEnAttente,

            (
                SELECT COALESCE(SUM(montant_total), 0)
                FROM commandes
                WHERE statut_paiement = 'PAYEE'
            ) AS ChiffreAffairesPaye,

            (
                SELECT COUNT(*)
                FROM ingredients
                WHERE actif = 1
            ) AS IngredientsActifs,

            (
                SELECT COUNT(*)
                FROM composition_produits
            ) AS Compositions;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    return await connection.QuerySingleAsync<DashboardStatsDto>(sql);
}


}
