using System.Collections.Generic;

using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetDictionaryByIdQuery : IQueryRequest<GetDictionaryByIdQuery.Response>
    {
        public string UserId { get; set; }

        public int DictionaryId { get; set; }
        public class Response : IQueryResponse
        {
            public Model.Dictionary Dictionary { get; }

            public Response(Model.Dictionary dictionary)
            {
                Dictionary = dictionary;
            }
        }
    }
}