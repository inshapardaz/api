using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Darker;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;

namespace Inshapardaz.Domain.QueryHandlers
{
    public class GetDictionaryByWordDetailIdQueryHandler : QueryHandler<GetDictionaryByWordDetailIdQuery, Model.Dictionary>
    {
        private readonly IDatabaseContext _database;

        public GetDictionaryByWordDetailIdQueryHandler(IDatabaseContext database)
        {
            _database = database;
        }

        public override Dictionary Execute(GetDictionaryByWordDetailIdQuery request)
        {
            return _database.WordDetails.SingleOrDefault(wd => wd.Id == request.WordDetailId)?.WordInstance.Dictionary;
        }
    }
}
