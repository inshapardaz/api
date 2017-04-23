using Darker;
using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Queries
{
    public class TranslationByIdQuery : IQuery<Translation>
    {
        public long Id { get; set; }
    }
}