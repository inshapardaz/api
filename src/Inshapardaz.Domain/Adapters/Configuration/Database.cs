using Inshapardaz.Domain.Models.Library;

namespace Inshapardaz.Domain.Adapters.Configuration
{
    public record Database
    {
        public DatabaseTypes? DatabaseConnectionType { get; init; }
        public string ConnectionString { get; init; }
    }
}
