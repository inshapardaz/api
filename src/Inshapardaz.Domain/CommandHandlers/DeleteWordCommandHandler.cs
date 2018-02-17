using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class DeleteWordCommandHandler : RequestHandlerAsync<DeleteWordCommand>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public DeleteWordCommandHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<DeleteWordCommand> HandleAsync(DeleteWordCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(command.DictionaryId);

            await client.DeleteAsync<Word>(command.DictionaryId,
                                                 i => i
                                                      .Index(index)
                                                      .Type(DocumentTypes.Word),
                                                 cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}