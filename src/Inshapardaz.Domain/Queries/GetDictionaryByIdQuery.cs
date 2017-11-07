using System;
using Paramore.Darker;
using Inshapardaz.Domain.Database.Entities;

namespace Inshapardaz.Domain.Queries
{
    public class GetDictionaryByIdQuery : IQuery<Dictionary>
    {

        public int DictionaryId { get; set; }
    }
}