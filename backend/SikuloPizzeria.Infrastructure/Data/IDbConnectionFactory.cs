using System.Data.Common;

namespace SikuloPizzeria.Infrastructure.Data;

public interface IDbConnectionFactory
{
    DbConnection CreateConnection();
}