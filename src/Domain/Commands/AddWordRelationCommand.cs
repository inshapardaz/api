using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class AddWordRelationCommand : Command
    {
        public long SourceWordId { get; set; }
        public long RelatedWordId { get; set; }
        public RelationType RelationType { get; set; }
    }
}