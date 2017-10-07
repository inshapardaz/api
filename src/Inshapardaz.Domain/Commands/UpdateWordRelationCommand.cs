using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordRelationCommand : Command
    {
        public WordRelation Relation { get; set; }
    }
}