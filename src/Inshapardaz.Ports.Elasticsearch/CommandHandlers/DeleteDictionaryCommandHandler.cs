using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Ports.Elasticsearch.CommandHandlers
{
    public class DeleteDictionaryCommandHandler : RequestHandlerAsync<DeleteDictionaryCommand>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public DeleteDictionaryCommandHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<DeleteDictionaryCommand> HandleAsync(DeleteDictionaryCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = _clientProvider.GetClient();
            await client.DeleteAsync<Dictionary>(command.DictionaryId,
                                                 i => i
                                                        .Index(Indexes.Dictionaries)
                                                        .Type(DocumentTypes.Dictionary),
                                                cancellationToken);

            var index = _indexProvider.GetIndexForDictionary(command.DictionaryId);
            await client.DeleteIndexAsync(index, cancellationToken: cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}