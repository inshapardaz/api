using System;
using Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class DictionaryByIdQuery : IQuery<Dictionary>
    {
        public Guid UserId { get; set; }

        public int DictionaryId { get; set; }
    }
}