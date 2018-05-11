using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Paramore.Brighter;

namespace Inshapardaz.Ports.Elasticsearch.CommandHandlers
{
    public class AddDictionaryCommandHandler : RequestHandlerAsync<AddDictionaryCommand>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public AddDictionaryCommandHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<AddDictionaryCommand> HandleAsync(AddDictionaryCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = _clientProvider.GetClient();
            
            var count = await client.CountAsync<Dictionary<,>>(s => s.Index(Indexes.Dictionaries), cancellationToken);
            command.Dictionary.Id = (int)count.Count + 1;
            
            await client.IndexAsync(command.Dictionary, i => i
                                    .Index(Indexes.Dictionaries)
                                    .Type(DocumentTypes.Dictionary), 
                                    cancellationToken);

            var index = _indexProvider.GetIndexForDictionary(command.Dictionary.Id);
            await client.CreateIndexAsync(index, cancellationToken: cancellationToken);

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}