using System.Data;

namespace Inshapardaz.Ports.Database
{
    public interface IProvideConnection
    {
        IDbConnection GetConnection();
    }
}
