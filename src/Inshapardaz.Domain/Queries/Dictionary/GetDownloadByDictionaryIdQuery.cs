using System;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities;
using Paramore.Darker;

namespace Inshapardaz.Domain.Queries.Dictionary
{
    public class GetDownloadByDictionaryIdQuery : IQuery<File>
    {
        public Guid UserId { get; set; }

        public int DictionaryId { get; set; }

        public string MimeType { get; set; }
    }

    public class GetDownloadByDictionaryIdQueryHandler : QueryHandlerAsync<GetDownloadByDictionaryIdQuery, File>
    {
        //private readonly IClientProvider _clientProvider;

        //public GetDownloadByDictionaryIdQueryHandler(IClientProvider clientProvider)
        //{
        //    _clientProvider = clientProvider;
        //}

        public override async Task<File> ExecuteAsync(GetDownloadByDictionaryIdQuery query,
                                                      CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}