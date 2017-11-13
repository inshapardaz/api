using System.Collections.Generic;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    [TestFixture]
    public class GetWordTranslationByLanguageQueryHandlerTests : DatabaseTest
    {
        private readonly GetTranslationsByLanguageQueryHandler _handler;
        private readonly IList<Translation> _translations;
        private readonly Word _word;

        public GetWordTranslationByLanguageQueryHandlerTests()
        {
            var dictionary = Builder<Dictionary>
                .CreateNew()
                .Build();
            _word = Builder<Word>
                .CreateNew()
                .Build();

            _translations = Builder<Translation>
                .CreateListOfSize(4)
                .Build();

            foreach (var translation in _translations)
            {
                _word.Translation.Add(translation);
            }
            dictionary.Word.Add(_word);

            DbContext.Dictionary.Add(dictionary);
            DbContext.SaveChanges();

            _handler = new GetTranslationsByLanguageQueryHandler(DbContext);
        }

        [Test]
        public async Task WhenGettingContextMeaningFromAWord_ShouldReturnCorrectMeaning()
        {
            var translations = await _handler.ExecuteAsync(new GetTranslationsByLanguageQuery(_word.Id, _translations[2].Language));

            translations.ShouldNotBeNull();
            translations.ShouldNotBeEmpty();
            translations.ShouldContain(_translations[2]);
        }

        [Test]
        public async Task WhenGettingContextMeaningFromIncorrectWord_ShouldReturnEmpty()
        {
            var translations = await _handler.ExecuteAsync(new GetTranslationsByLanguageQuery(-1, _translations[3].Language));

            translations.ShouldNotBeNull();
            translations.ShouldBeEmpty();
        }
    }
}