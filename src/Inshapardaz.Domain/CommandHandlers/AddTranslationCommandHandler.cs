using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Elasticsearch;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Exception;
using Nest;
using Paramore.Brighter;

namespace Inshapardaz.Domain.CommandHandlers
{
    public class AddTranslationCommandHandler : RequestHandlerAsync<AddTranslationCommand>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public AddTranslationCommandHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<AddTranslationCommand> HandleAsync(AddTranslationCommand command, CancellationToken cancellationToken = default(CancellationToken))
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

            command.Translation.Id = word.Translation.Count;
            word.Translation.Add(command.Translation);

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