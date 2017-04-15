using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class TranslationByIdQuery : IQueryRequest<TranslationByIdQuery.Response>
    {
        public long Id { get; set; }

        public class Response : IQueryResponse
        {
            public Translation Translation { get; set; }
        }
    }
}