using Dapper;
using SikuloPizzeria.Core.Entities;
using SikuloPizzeria.Core.Interfaces;
using SikuloPizzeria.Infrastructure.Data;

namespace SikuloPizzeria.Infrastructure.Repositories;

public sealed class ClientRepository : IClientRepository
{
private readonly IDbConnectionFactory _connectionFactory;

public ClientRepository(IDbConnectionFactory connectionFactory)
{
    _connectionFactory = connectionFactory;
}

public async Task<IEnumerable<Client>> GetAllAsync()
{
    const string sql = """
        SELECT
            id AS Id,
            nom AS Nom,
            prenom AS Prenom,
            email AS Email,
            telephone AS Telephone,
            adresse_rue AS AdresseRue,
            adresse_numero AS AdresseNumero,
            adresse_boite AS AdresseBoite,
            adresse_code_postal AS AdresseCodePostal,
            adresse_ville AS AdresseVille,
            adresse_pays AS AdressePays,
            notes AS Notes,
            client_vip AS ClientVip,
            actif AS Actif,
            date_creation AS DateCreation,
            date_modification AS DateModification
        FROM clients
        ORDER BY nom, prenom;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    return await connection.QueryAsync<Client>(sql);
}

public async Task<Client?> GetByIdAsync(int id)
{
    const string sql = """
        SELECT
            id AS Id,
            nom AS Nom,
            prenom AS Prenom,
            email AS Email,
            telephone AS Telephone,
            adresse_rue AS AdresseRue,
            adresse_numero AS AdresseNumero,
            adresse_boite AS AdresseBoite,
            adresse_code_postal AS AdresseCodePostal,
            adresse_ville AS AdresseVille,
            adresse_pays AS AdressePays,
            notes AS Notes,
            client_vip AS ClientVip,
            actif AS Actif,
            date_creation AS DateCreation,
            date_modification AS DateModification
        FROM clients
        WHERE id = @Id;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    return await connection.QuerySingleOrDefaultAsync<Client>(
        sql,
        new { Id = id }
    );
}

public async Task<int> CreateAsync(Client client)
{
    const string sql = """
        INSERT INTO clients (
            nom,
            prenom,
            email,
            telephone,
            adresse_rue,
            adresse_numero,
            adresse_boite,
            adresse_code_postal,
            adresse_ville,
            adresse_pays,
            notes,
            client_vip,
            actif
        )
        VALUES (
            @Nom,
            @Prenom,
            @Email,
            @Telephone,
            @AdresseRue,
            @AdresseNumero,
            @AdresseBoite,
            @AdresseCodePostal,
            @AdresseVille,
            @AdressePays,
            @Notes,
            @ClientVip,
            @Actif
        );

        SELECT LAST_INSERT_ID();
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    return await connection.ExecuteScalarAsync<int>(sql, client);
}

public async Task<bool> UpdateAsync(Client client)
{
    const string sql = """
        UPDATE clients
        SET
            nom = @Nom,
            prenom = @Prenom,
            email = @Email,
            telephone = @Telephone,
            adresse_rue = @AdresseRue,
            adresse_numero = @AdresseNumero,
            adresse_boite = @AdresseBoite,
            adresse_code_postal = @AdresseCodePostal,
            adresse_ville = @AdresseVille,
            adresse_pays = @AdressePays,
            notes = @Notes,
            client_vip = @ClientVip,
            actif = @Actif
        WHERE id = @Id;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int affectedRows = await connection.ExecuteAsync(sql, client);
    return affectedRows > 0;
}

public async Task<bool> DisableAsync(int id)
{
    const string sql = """
        UPDATE clients
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
        UPDATE clients
        SET actif = TRUE
        WHERE id = @Id
          AND actif = FALSE;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
    return affectedRows > 0;
}

public async Task<bool> HasOrdersAsync(int id)
{
    const string sql = """
        SELECT COUNT(*)
        FROM commandes
        WHERE client_id = @Id;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int count = await connection.ExecuteScalarAsync<int>(sql, new { Id = id });
    return count > 0;
}

public async Task<bool> DeletePermanentlyAsync(int id)
{
    const string sql = """
        DELETE FROM clients
        WHERE id = @Id;
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
    return affectedRows > 0;
}

public async Task<bool> ExistsByEmailAsync(
    string email,
    int? excludedId = null)
{
    const string sql = """
        SELECT COUNT(*)
        FROM clients
        WHERE LOWER(email) = LOWER(@Email)
          AND (@ExcludedId IS NULL OR id <> @ExcludedId);
        """;

    await using var connection = _connectionFactory.CreateConnection();
    await connection.OpenAsync();

    int count = await connection.ExecuteScalarAsync<int>(
        sql,
        new
        {
            Email = email,
            ExcludedId = excludedId
        }
    );

    return count > 0;
}


}
