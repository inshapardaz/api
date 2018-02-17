using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
using Nest;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class UpdateWordCommandHandler : RequestHandlerAsync<UpdateWordCommand>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public UpdateWordCommandHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<UpdateWordCommand> HandleAsync(UpdateWordCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(command.DictionaryId);

            await client.UpdateAsync(DocumentPath<Word>.Id(command.Word.Id),
                                     u => u
                                          .Index(index)
                                          .Type(DocumentTypes.Word)
                                          .DocAsUpsert()
                                          .Doc(command.Word), cancellationToken);
            
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}