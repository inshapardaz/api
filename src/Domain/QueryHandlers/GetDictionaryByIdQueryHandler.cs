using System.Collections.Generic;
using System.Linq;

using Inshapardaz.Domain.Queries;

using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryByIdQueryHandler : QueryHandler<GetDictionaryByIdQuery, Dictionary>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryByIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override Dictionary Execute(GetDictionaryByIdQuery request)
        {
            IQueryable<Dictionary> result;
            if (!string.IsNullOrWhiteSpace(request.UserId))
            {
                result = _database.Dictionaries.Where(d => d.Id == request.DictionaryId && (d.IsPublic || (d.UserId == request.UserId)));
            }
            else
            {
                result = _database.Dictionaries.Where(d => d.Id == request.DictionaryId && d.IsPublic);
            }

            return result.SingleOrDefault();
        }
    }
}
