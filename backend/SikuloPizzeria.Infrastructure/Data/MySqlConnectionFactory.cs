using System.Data.Common;
using MySql.Data.MySqlClient;

namespace SikuloPizzeria.Infrastructure.Data;

public sealed class MySqlConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public MySqlConnectionFactory(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "La chaîne de connexion MySQL est obligatoire.",
                nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    public DbConnection CreateConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}