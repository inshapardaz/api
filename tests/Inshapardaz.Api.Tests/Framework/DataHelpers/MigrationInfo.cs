using System.Collections.Generic;
using FluentMigrator.Infrastructure;

namespace Inshapardaz.Api.Tests.Framework.DataHelpers
{
    public class MigrationInfo
    {
        public IEnumerable<KeyValuePair<long, IMigrationInfo>> InAssembly { get; set; }
        public IEnumerable<KeyValuePair<long, IMigrationInfo>> InDatabase { get; set; }
        public IEnumerable<KeyValuePair<long, IMigrationInfo>> NotInDatabase { get; set; }

        public long LatestDatabaseVersion { get; set; }
        public long LatestAssemblyVersion { get; set; }
    }
}