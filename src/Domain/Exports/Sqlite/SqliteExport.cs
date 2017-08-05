using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Domain.Exports.Sqlite
{
    public class SqliteExport
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public SqliteExport(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public byte[] ExportDictionary(int dictionaryId)
        {
            var connectionStringSqlServer = _configuration["ConnectionStrings:DefaultDatabase"];
            var sqlitePath =Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.dat" );

            SqlServerToSQLite.ConvertSqlServerToSQLiteDatabase(
                connectionStringSqlServer, 
                sqlitePath,
                null,
                Handler,
                SelectionHandler, 
                null, 
                false,
                false);

            return new byte[0];
        }
        

        private List<TableSchema> SelectionHandler(List<TableSchema> schema)
        {
            var tablesToExport = new string[]
            {
                "Word",
                "WordDetail",
                "WordRelation",
                "Meaning",
                "Translation"
            };
            return schema.Where(t => tablesToExport.Any(x => x == t.TableName)).ToList();
        }

        private void Handler(bool done, bool success, int percent, string msg)
        {
            Console.WriteLine($"Export Progress: {percent}, Status: {done},{success}, Message : {msg}");
        }
    }
}
