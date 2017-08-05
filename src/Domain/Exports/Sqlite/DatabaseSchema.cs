using System.Collections.Generic;

namespace Inshapardaz.Domain.Exports.Sqlite
{
    public class DatabaseSchema
    {
        public List<TableSchema> Tables = new List<TableSchema>();
        public List<ViewSchema> Views = new List<ViewSchema>();
    }
}
