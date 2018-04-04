using System.Linq;
using Inshapardaz.Domain.Commands;
using Paramore.Brighter;
using Inshapardaz.Domain.Exception;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
using Nest;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddMeaningCommandHandler : RequestHandlerAsync<AddMeaningCommand>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public AddMeaningCommandHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<AddMeaningCommand> HandleAsync(AddMeaningCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            command.Meaning.WordId = command.WordId;
            var client = _clientProvider.GetClient();
            var index = _indexProvider.GetIndexForDictionary(command.DictionaryId);

            var response = await client.SearchAsync<Word>(s => s
                                        .Index(index)
                                        .Size(1)
                                        .Query(q => q
                                        .Bool(b => b
                                        .Must(m => m
                                        .Term(term => term.Field(f => f.Id).Value(command.WordId)))
                                )), cancellationToken);

            var word = response.Documents.SingleOrDefault();

            if (word == null)
            {
                throw new NotFoundException();
            }

            command.Meaning.Id = word.Meaning.Count + 1;
            word.Meaning.Add(command.Meaning);

            await client.UpdateAsync(DocumentPath<Word>.Id(command.DictionaryId),
                                     u => u
                                          .Index(index)
                                          .Type(DocumentTypes.Word)
                                          .DocAsUpsert()
                                          .Doc(word), cancellationToken);
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}