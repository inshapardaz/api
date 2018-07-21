﻿using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionary
{
    public class GetMeaningByIdRequest : DictionaryRequest
    {
        public GetMeaningByIdRequest(int dictionaryId, long wordId, long meaningId)
            : base(dictionaryId)
        {
            MeaningId = meaningId;
            WordId = wordId;
        }

        public long MeaningId { get; }

        public long WordId { get; }

        public Meaning Result { get; set;  }
    }
    
    public class GetMeaningByIdRequestHandler : RequestHandlerAsync<GetMeaningByIdRequest>
    {
        private readonly IMeaningRepository _meaningRepository;

        public GetMeaningByIdRequestHandler(IMeaningRepository meaningRepository)
        {
            _meaningRepository = meaningRepository;
        }
        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetMeaningByIdRequest> HandleAsync(GetMeaningByIdRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var meaning = await _meaningRepository.GetMeaningById(command.DictionaryId, command.WordId, command.MeaningId, cancellationToken);

            if (meaning == null)
            {
                throw new NotFoundException();
            }

            command.Result = meaning;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}