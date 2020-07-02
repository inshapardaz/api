using System.Data;

namespace Inshapardaz.Database.SqlServer
{
    public interface IProvideConnection
    {
        IDbConnection GetConnection();
    }
}
