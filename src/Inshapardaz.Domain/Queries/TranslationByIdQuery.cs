using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class TranslationByIdQuery : IQuery<Translation>
    {
        public long Id { get; set; }
    }
}