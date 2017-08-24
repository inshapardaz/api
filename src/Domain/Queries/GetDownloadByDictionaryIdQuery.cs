using Darker;

namespace Inshapardaz.Domain.Queries
{
    public class GetDownloadByDictionaryIdQuery : IQuery<Model.File>
    {
        public string UserId { get; set; }

        public int DictionaryId { get; set; }

        public string MimeType { get; set; }
    }
}