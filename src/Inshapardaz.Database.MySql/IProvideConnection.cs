using System.Data;

namespace Inshapardaz.Database.MySql
{
    public interface IProvideConnection
    {
        IDbConnection GetConnection();
        IDbConnection GetLibraryConnection();
    }
}
