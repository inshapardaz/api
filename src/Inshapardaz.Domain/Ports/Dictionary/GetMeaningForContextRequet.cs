using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Repositories.Dictionary;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Dictionary
{
    public class GetMeaningForContextRequest : DictionaryRequest
    {
        public GetMeaningForContextRequest(int dictionaryId, long wordId, string context)
            : base(dictionaryId)
        {
            WordId = wordId;
            Context = context;
        }

        public string Context { get; }

        public long WordId { get; }

        public IEnumerable<Meaning> Result { get; set; }
    }

    public class GetMeaningForContextRequestHandler : RequestHandlerAsync<GetMeaningForContextRequest>
    {
        private readonly IMeaningRepository _meaningRepository;

        public GetMeaningForContextRequestHandler(IMeaningRepository meaningRepository)
        {
            _meaningRepository = meaningRepository;
        }

        [DictionaryRequestValidation(1, HandlerTiming.Before)]
        public override async Task<GetMeaningForContextRequest> HandleAsync(GetMeaningForContextRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var finalContext = string.Empty;
            if (command.Context != "default")
            {
                finalContext = command.Context;
            }

            command.Result = await _meaningRepository.GetMeaningByContext(command.DictionaryId, command.WordId, finalContext, cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
