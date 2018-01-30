using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Queries;
using Microsoft.Extensions.Configuration;
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
        private readonly IQueryProcessor _queryProcessor;
        private readonly ILogger<TranspileRequestHandler> _logger;
        private readonly AppSettings _appSettings;

        public TranspileRequestHandler(IQueryProcessor queryProcessor, 
                    ILogger<TranspileRequestHandler> logger,
                    AppSettings appSettings)
        {
            _queryProcessor = queryProcessor;
            _logger = logger;
            _appSettings = appSettings;
        }

        public override async Task<TranspileRequest> HandleAsync(TranspileRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(request.Source))
            {
                throw new BadRequestException();
            }

            int dictionaryId = _appSettings.DefaultDictionaryId;
            var dictionary = await _queryProcessor.ExecuteAsync(new GetDictionaryByIdQuery {DictionaryId = dictionaryId }, cancellationToken);
            
            string result = string.Empty;

            if (request.FromLanguage == dictionary.Language)
            {
                var requestStrings = request.Source.SplitIntoWords().PreserveSpecialCharacters();
                var translationQuery = new GetTranslationsForWordsByLanguageQuery(requestStrings, request.ToLanguage) { IsTranspiling = true };
                var translations = await _queryProcessor.ExecuteAsync(translationQuery, cancellationToken);
                result = Transpile(requestStrings, translations);
            }
            else if (request.ToLanguage == dictionary.Language)
            {
                var requestStrings = request.Source.SplitIntoWords().PreserveSpecialCharacters();
                var translationQuery = new GetWordsForTranslationsByLanguageQuery(requestStrings, request.FromLanguage) { IsTranspiling = true };
                var translations = await _queryProcessor.ExecuteAsync(translationQuery, cancellationToken);
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
    }
}
