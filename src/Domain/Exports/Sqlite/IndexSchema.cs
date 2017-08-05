using System.Collections.Generic;

namespace Inshapardaz.Domain.Exports.Sqlite
{
    public class IndexSchema
    {
        public string IndexName;

        public bool IsUnique;

        public List<IndexColumn> Columns;
    }
}
