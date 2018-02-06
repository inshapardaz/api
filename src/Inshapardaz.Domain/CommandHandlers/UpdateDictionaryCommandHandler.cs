using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Nest;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
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