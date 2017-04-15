using System.Collections.Generic;

using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetDictionariesByUserQuery : IQueryRequest<GetDictionariesByUserQuery.Response>
    {
        public string UserId { get; set; }

        public class Response : IQueryResponse
        {
            public IEnumerable<Model.Dictionary> Dictionaries { get; }

            public Response(IReadOnlyList<Model.Dictionary> dictionaries)
            {
                Dictionaries = dictionaries;
            }
        }
    }
}