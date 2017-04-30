using System.Linq;
using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryByWordIdQueryHandler : QueryHandler<GetDictionaryByWordIdQuery, Dictionary>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryByWordIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override Dictionary Execute(GetDictionaryByWordIdQuery request)
        {
            return _database.Words.SingleOrDefault(wd => wd.Id == request.WordId)?.Dictionary;
        }
    }
}
