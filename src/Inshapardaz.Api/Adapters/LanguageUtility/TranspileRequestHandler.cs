using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.View.Dictionary;
using Inshapardaz.Domain;
using Inshapardaz.Domain.Entities;
using Inshapardaz.Domain.Entities.Dictionary;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Repositories.Dictionary;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.LanguageUtility
{
    public class TranspileRequest : IRequest
    {
        public Guid Id { get; set; }

        public Languages FromLanguage { get; set; }

        public Languages ToLanguage { get; set; }

        public string Source { get; set; }

        public TranspileResponseView Response { get; internal set; }
    }

    public class TranspileRequestHandler : RequestHandlerAsync<TranspileRequest>
    {
        private readonly IDictionaryRepository _dictionaryRepository;
        private readonly IWordRepository _wordRepository;
        private readonly ITranslationRepository _translationRepository;
        private readonly ILogger<TranspileRequestHandler> _logger;
        private readonly Settings _settings;

        public TranspileRequestHandler(IDictionaryRepository dictionaryRepository,
                                       IWordRepository wordRepository,
                                       ITranslationRepository translationRepository,
                    ILogger<TranspileRequestHandler> logger,
                    Settings settings)
        {
            _dictionaryRepository = dictionaryRepository;
            _wordRepository = wordRepository;
            _translationRepository = translationRepository;
            _logger = logger;
            _settings = settings;
        }

        public override async Task<TranspileRequest> HandleAsync(TranspileRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(request.Source))
            {
                throw new BadRequestException();
            }

            int dictionaryId = _settings.DefaultDictionaryId;
            var dictionary = await _dictionaryRepository.GetDictionaryById (dictionaryId, cancellationToken);
            
            string result = string.Empty;

            if (request.FromLanguage == dictionary.Language)
            {
                var requestStrings = request.Source.SplitIntoWords().PreserveSpecialCharacters();
                var translations = await GetTranslationsForWordsByLanguage(dictionaryId, requestStrings, request.ToLanguage, true, cancellationToken);
                result = Transpile(requestStrings, translations);
            }
            else if (request.ToLanguage == dictionary.Language)
            {
                var requestStrings = request.Source.SplitIntoWords().PreserveSpecialCharacters();
                var translations = await GetTranslationsForWordsByLanguage(dictionaryId, requestStrings, request.FromLanguage, true, cancellationToken);
                result = Transpile(requestStrings, translations);
            }
            else
            {
                throw new BadRequestException();
            }

            request.Response = new TranspileResponseView
            {
                FromLanguage = request.FromLanguage,
                ToLanguage = request.ToLanguage,
                Result = result
            };

            return await base.HandleAsync(request, cancellationToken);
        }

        private string Transpile(IEnumerable<string> source, Dictionary<string, Translation> translations)
        {
            var builder = new StringBuilder();
            builder.AppendJoin(' ', source.Select(s => translations.ContainsKey(s) ? translations[s].Value : s));
            return builder.ToString();
        }

        private string Transpile(IEnumerable<string> source, Dictionary<string, Word> translations)
        {
            var builder = new StringBuilder();
            builder.AppendJoin(' ', source.Select(s => translations.ContainsKey(s) ? translations[s].Title : s));
            return builder.ToString();
        }

        private async Task<Dictionary<string, Word>> GetTranslationsForWordsByLanguage(int dictionaryId, string[] titles, Languages languages, bool isTranspiling, CancellationToken cancellationToken)
        {
            /*List<Translation> translations;
            if (isTranspiling)
            {
                translations = await _translationRepository
                                      .Where(x => x.Language == query.Language &&
                                                  x.IsTrasnpiling == query.IsTranspiling &&
                                                  query.Words.Any(w => w == x.Value))
                                                  .ToListAsync(cancellationToken);
            }

            else
            {
                translations = await _database.Translation
                                      .Where(x => x.Language == query.Language &&
                                                  x.IsTrasnpiling == query.IsTranspiling &&
                                                  query.Words.Any(w => w == x.Value))
                                      .ToListAsync(cancellationToken);
            }

            var result = new Dictionary<string, Word>();
            foreach (var translation in translations)
            {
                if (result.ContainsKey(translation.Value))
                {
                    // IMPORVE : We have mulitple words for this translation. 
                    continue;
                }
                else
                {
                    var word = _database.Word.SingleOrDefault(w => w.Id == translation.WordId);
                    result.Add(translation.Value, word);
                }
            }*/
            throw new NotImplementedException();
        }
    }
}
