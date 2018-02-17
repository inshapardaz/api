using System;
using Inshapardaz.Domain.Entities;

namespace Inshapardaz.Domain.Commands
{
    public class UpdateWordRelationCommand : Command
    {
        public int DictionaryId { get; }

        public UpdateWordRelationCommand(int dictionaryId, WordRelation relation)
        {
            DictionaryId = dictionaryId;
            Relation = relation ?? throw new ArgumentNullException(nameof(relation));
        }

        public WordRelation Relation { get; }
    }
}