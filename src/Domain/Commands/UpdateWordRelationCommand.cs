using Inshapardaz.Domain.Model;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordRelationCommand : Command
    {
        public WordRelation Relation { get; set; }
    }
}