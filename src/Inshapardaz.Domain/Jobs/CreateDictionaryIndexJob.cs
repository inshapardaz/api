using System.Linq;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.IndexingService;

namespace Inshapardaz.Domain.Jobs
{
    public class CreateDictionaryIndexJob
    {
        private readonly IWriteDictionaryIndex _indexWriter;
        private readonly IDatabaseContext _database;

        public CreateDictionaryIndexJob(IWriteDictionaryIndex indexWriter, IDatabaseContext database)
        {
            _indexWriter = indexWriter;
            _database = database;
        }
        public void CreateIndex(int dictionaryId)
        {
            var words = _database.Word.Where(w => w.DictionaryId == dictionaryId);
            _indexWriter.CreateIndex(dictionaryId, words.ToList());
        }
    }
}
