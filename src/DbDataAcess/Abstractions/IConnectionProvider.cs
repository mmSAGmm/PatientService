using System.Data;
using System.Data.Common;

namespace Db.DataAccess.Abstractions
{
    public interface IConnectionProvider
    {
        DbConnection GetConnection();
    }
}
