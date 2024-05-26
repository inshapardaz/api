using System.Data;

namespace Inshapardaz.Domain.Adapters;

public interface IProvideConnection
{
    IDbConnection GetConnection();
    IDbConnection GetLibraryConnection();
}
