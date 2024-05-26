using Db.DataAccess.Abstractions;
using Db.DataAccess.Options;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace Db.DataAccess.Implementation
{
    public class MySqlConnectionProvider : IConnectionProvider
    {
        private readonly IOptions<MySqlOption> _options;

        public MySqlConnectionProvider(IOptions<MySqlOption> options)
        {
            _options = options;
        }

        public DbConnection GetConnection()
        {
            return new MySqlConnection(_options.Value.ConnectionString);
        }
    }
}
