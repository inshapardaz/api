using System;
using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetDownloadByDictionaryIdQuery : IQuery<File>
    {
        public Guid UserId { get; set; }

        public int DictionaryId { get; set; }

        public string MimeType { get; set; }
    }
}