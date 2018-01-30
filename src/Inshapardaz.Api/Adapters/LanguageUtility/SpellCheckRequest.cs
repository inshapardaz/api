using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Api.Renderers;
using Inshapardaz.Api.View;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Helpers;
using Inshapardaz.Domain.Queries;
using Microsoft.Extensions.Logging;
using Paramore.Brighter;
using Paramore.Darker;

namespace Inshapardaz.Api.Adapters.LanguageUtility
{
    public class SpellCheckRequest : IRequest
    {
        public Guid Id { get; set; }
        public string Sentence { get; internal set; }
        public IEnumerable<SpellCheckResultView> Response { get; internal set; }
    }

    public class SpellCheckRequestHandler : RequestHandlerAsync<SpellCheckRequest>
    {
        private readonly IQueryProcessor _queryProcessor;
        private readonly IRenderLink _linkRenderer;
        private readonly AppSettings _appSettings;
        private readonly ILogger<SpellCheckRequestHandler> _logger;

        public SpellCheckRequestHandler(IQueryProcessor queryProcessor, 
                                        IRenderLink linkRenderer,
                                        AppSettings appSettings, 
                                        ILogger<SpellCheckRequestHandler> logger)
        {
            _queryProcessor = queryProcessor;
            _linkRenderer = linkRenderer;
            _appSettings = appSettings;
            _logger = logger;
        }

        

        public override async Task<SpellCheckRequest> HandleAsync(SpellCheckRequest request, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(request.Sentence))
            {
                throw new BadRequestException();
            }

            var words = new List<SpellCheckResultView>();
            var lastIndex = 0;

            int defaultDictionaryId = _appSettings.DefaultDictionaryId;

            if (_appSettings.DefaultDictionaryId == 0)
            {
                throw new Exception("Default dictionary not found");
            }


            var requestWords = request.Sentence.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (requestWords.Length > 1000)
            {
                throw new BadRequestException();
            }

            foreach (var item in requestWords)
            {
                var sanitisedWord = item.TrimSpecialCharacters();
                var index = request.Sentence.IndexOf(sanitisedWord, lastIndex);

                var foundWord = await _queryProcessor.ExecuteAsync(new GetWordByTitleQuery(defaultDictionaryId, sanitisedWord));

                words.Add(new SpellCheckResultView
                {
                    Word = sanitisedWord,
                    IndexInString = index,
                    Correct = foundWord != null,
                    WordLink = foundWord != null ? RenderWordLink(defaultDictionaryId, foundWord) : null,
                    Corrections = foundWord != null ? null : await FindCorrectOptions(defaultDictionaryId, sanitisedWord, cancellationToken)
                });

                lastIndex = index + item.Length;
            }

            _logger.LogTrace($"Extracted {words.Count} words to check");
            request.Response = words;

            return await base.HandleAsync(request, cancellationToken);
        }

        private LinkView RenderWordLink(int dictionaryId, Word word)
        {
            return _linkRenderer.Render("GetWordById", RelTypes.Self, new { id = dictionaryId, wordId = word.Id });
        }

        private char[] urduChars = new char[]
        {
            'آ',
            'ا',
            'ب',
            'پ',
            'ت',
            'ٹ',
            'ث',
            'ج',
            'چ',
            'ح',
            'خ',
            'د',
            'ڈ',
            'ذ',
            'ر',
            'ڑ',
            'ز',
            'ژ',
            'س',
            'ش',
            'ص',
            'ض',
            'ط',
            'ظ',
            'ع',
            'غ',
            'ف',
            'ق',
            'ک',
            'گ',
            'ل',
            'م',
            'ن',
            'و',
            'ہ',
            'ء',
            'ی',
            'ئ',
            'ؤ'
        };



        private async Task<IEnumerable<SpellingOption>> FindCorrectOptions(int dictionaryId, string word, CancellationToken cancellationToken)
        {
            var replacements = new List<SpellingOption>();

            var options = new List<string>();

            // Method 0 :: Remove all movements and try again
            var wordWithoutMovements = word.RemoveMovements();
            options.Add(wordWithoutMovements);

            // Methos 1 :: replace characters in the word by other words
            for (int i = 0; i < word.Length; i++)
            {
                var ch = word[i];

                if (!Array.Exists(urduChars, c => c == ch))
                {
                    continue;
                }

                foreach (var c in urduChars)
                {
                    var newWord = word.ToCharArray();
                    newWord[i] = c;
                    options.Add(new string(newWord));
                }
            }

            // append character at each location of the word
            for (int i = 0; i < word.Length; i++)
            {
                var ch = word[i];

                if (!Array.Exists(urduChars, c => c == ch))
                {
                    continue;
                }

                foreach (var c in urduChars)
                {
                    var newWord = new List<char>(word);
                    newWord.Insert(i, c);
                    options.Add(new string(newWord.ToArray()));
                }
            }

            // drop characters from list
            for (int i = 0; i < word.Length; i++)
            {
                var ch = word[i];

                if (!Array.Exists(urduChars, c => c == ch))
                {
                    continue;
                }

                var newWord = new List<char>(word);
                newWord.RemoveAt(i);
                options.Add(new string(newWord.ToArray()));
            }

            // re-arrangecharacters from list
            for (int i = 0; i < word.Length - 1; i++)
            {
                var ch = word[i];

                if (!Array.Exists(urduChars, c => c == ch))
                {
                    continue;
                }

                var newWord = word.ToCharArray();
                char temp = newWord[i];
                newWord[i] = newWord[i + 1];
                newWord[i+1] = temp;
                options.Add(new string(newWord));
            }

            var uniqueOptions = options.Distinct();
            var optionResults = await _queryProcessor.ExecuteAsync(new GetWordsByTitlesQuery(dictionaryId, uniqueOptions));

            return optionResults.Select(r => Convert(dictionaryId, r));
        }

        private SpellingOption Convert(int dictionaryId, Word word)
        {
            var w = word.Map<Word, SpellingOption>();
            w.Links = new List<LinkView> { RenderWordLink(dictionaryId, word) };
            return w;
        }
    }
}
