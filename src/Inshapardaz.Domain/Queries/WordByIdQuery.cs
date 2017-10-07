using System;
using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class WordByIdQuery : IQuery<Word>
    {
        public long Id { get; set; }

        public Guid UserId { get; set; }
    }
}