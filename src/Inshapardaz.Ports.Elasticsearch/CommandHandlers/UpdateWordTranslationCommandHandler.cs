﻿using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Ports.Elasticsearch.CommandHandlers
{
    public class UpdateWordTranslationCommandHandler : RequestHandlerAsync<UpdateWordTranslationCommand>
    {
        private readonly IClientProvider _clientProvider;
        private readonly IProvideIndex _indexProvider;

        public UpdateWordTranslationCommandHandler(IClientProvider clientProvider, IProvideIndex indexProvider)
        {
            _clientProvider = clientProvider;
            _indexProvider = indexProvider;
        }

        public override async Task<UpdateWordTranslationCommand> HandleAsync(UpdateWordTranslationCommand command, CancellationToken cancellationToken = default(CancellationToken))
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

            var translation = word.Translation.SingleOrDefault(m => m.Id == command.Translation.Id);
            if (translation == null)
            {
                throw new NotFoundException();
            }

            translation.Language = command.Translation.Language;
            translation.Value = command.Translation.Value;
            translation.IsTrasnpiling = command.Translation.IsTrasnpiling;

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
