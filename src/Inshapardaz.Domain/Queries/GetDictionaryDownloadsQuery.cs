using System;
using System.Collections.Generic;
using Inshapardaz.Domain.Database.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetDictionaryDownloadsQuery : IQuery<IEnumerable<DictionaryDownload>>
    {
        public Guid UserId { get; set; }

        public int DictionaryId { get; set; }
    }
}