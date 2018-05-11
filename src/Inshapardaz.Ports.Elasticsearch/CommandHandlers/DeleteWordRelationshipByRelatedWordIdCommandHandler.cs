using System;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Ports.Elasticsearch.CommandHandlers
{
    public class DeleteWordRelationshipByRelatedWordIdCommandHandler : RequestHandlerAsync<DeleteWordRelationshipByRelatedWordIdCommand>
    {
        public override async Task<DeleteWordRelationshipByRelatedWordIdCommand> HandleAsync(DeleteWordRelationshipByRelatedWordIdCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}