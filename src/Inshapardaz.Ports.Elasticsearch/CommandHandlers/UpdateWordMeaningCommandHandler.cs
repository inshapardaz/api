using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Ports.Elasticsearch.CommandHandlers
{
    public class UpdateWordMeaningCommandHandler : RequestHandlerAsync<UpdateWordMeaningCommand>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public UpdateWordMeaningCommandHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<UpdateWordMeaningCommand> HandleAsync(UpdateWordMeaningCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
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

            var meaning = word.Meaning.SingleOrDefault(m => m.Id == command.Meaning.Id);
            if (meaning== null)
            {
                throw new NotFoundException();
            }

            meaning.Context = command.Meaning.Context;
            meaning.Value = command.Meaning.Value;
            meaning.Example = command.Meaning.Example;

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