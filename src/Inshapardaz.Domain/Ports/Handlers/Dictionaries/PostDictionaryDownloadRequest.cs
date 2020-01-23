﻿using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Dictionaries;
using Inshapardaz.Domain.Repositories.Dictionaries;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionaries
{
    public class PostDictionaryDownloadRequest : DictionaryRequest
    {
        public PostDictionaryDownloadRequest(int dictionaryId)
            : base(dictionaryId)
        {
        }

        public DictionaryDownload Result { get; set; } 
    }

    public class PostDictionaryDownloadRequestHandler : RequestHandlerAsync<PostDictionaryDownloadRequest>
    {
        private readonly IDictionaryRepository _dictionaryRepository;

        public PostDictionaryDownloadRequestHandler(IDictionaryRepository dictionaryRepository)
        {
            _dictionaryRepository = dictionaryRepository;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<PostDictionaryDownloadRequest> HandleAsync(PostDictionaryDownloadRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var newDownload = await _dictionaryRepository.AddDictionaryDownload(command.DictionaryId, MimeTypes.SqlLite,  cancellationToken: cancellationToken);

            command.Result = newDownload;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}