using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class TranslationByIdQuery : IQuery<TranslationByIdQuery.Response>
    {
        public long Id { get; set; }

        public class Response
        {
            public Translation Translation { get; set; }
        }
    }
}