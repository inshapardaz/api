using System.Collections.Generic;
using System.Linq;
using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class WordDetailsByWordQueryHandler : QueryHandler<WordDetailsByWordQuery, IEnumerable<WordDetail>>
    {
        private readonly IDatabaseContext _database;

        public WordDetailsByWordQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override IEnumerable<WordDetail> Execute(WordDetailsByWordQuery query)
        {
            IEnumerable<WordDetail> result;

            if (query.IncludeDetails)
            {
                result = _database.WordDetails
                    .Include(w => w.Meanings)
                    .Include(w => w.Translations)
                    .Where(w => w.WordInstanceId == query.WordId)
                    .OrderBy(x => x.Id)
                    .ToList();
            }
            else
            {
                result = _database.WordDetails
                    .Where(w => w.WordInstanceId == query.WordId)
                    .OrderBy(x => x.Id)
                    .ToList();
            }
            
            return result;
        }
    }
}
