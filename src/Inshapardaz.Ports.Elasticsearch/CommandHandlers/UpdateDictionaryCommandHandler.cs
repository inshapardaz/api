using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Ports.Elasticsearch.CommandHandlers
{
    public class UpdateDictionaryCommandHandler : RequestHandlerAsync<UpdateDictionaryCommand>
    {
        private readonly IClientProvider _clientProvider;

        public UpdateDictionaryCommandHandler(IClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public override async Task<UpdateDictionaryCommand> HandleAsync(UpdateDictionaryCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var client = _clientProvider.GetClient();
            await client.UpdateAsync(DocumentPath<Dictionary>.Id(command.Dictionary.Id), 
                                     u => u
                                        .Index(Indexes.Dictionaries)
                                        .Type(DocumentTypes.Dictionary)
                                        .DocAsUpsert()
                                        .Doc(command.Dictionary), cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}