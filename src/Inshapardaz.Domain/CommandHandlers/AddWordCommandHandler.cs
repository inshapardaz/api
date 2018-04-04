using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Exception;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddWordCommandHandler : RequestHandlerAsync<AddWordCommand>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public AddWordCommandHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<AddWordCommand> HandleAsync(AddWordCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (command.Word == null)
            {
                throw new BadRequestException();
            }

            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(command.DictionaryId);

            var count = await client.CountAsync<Word>(i => i.Index(index), cancellationToken);
            command.Word.Id = count.Count + 1;
            command.Word.DictionaryId = command.DictionaryId;

            await client.IndexAsync(command.Word, i => i.Index(index).Type(DocumentTypes.Word), cancellationToken);

            return await  base.HandleAsync(command, cancellationToken);
        }
    }
}