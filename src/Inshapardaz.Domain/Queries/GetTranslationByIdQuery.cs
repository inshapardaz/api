using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class GetTranslationByIdQuery : IQuery<Translation>
    {
        public long Id { get; set; }
    }
}