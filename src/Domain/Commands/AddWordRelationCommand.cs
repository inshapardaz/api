using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class AddWordRelationCommand : Command
    {
        public int SourceWordId { get; set; }
        public int RelatedWordId { get; set; }
        public RelationType RelationType { get; set; }
    }
}
