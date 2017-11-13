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
    public class GetWordTranslationByWordQueryHandlerTests : DatabaseTest
    {
        private readonly GetTranslationsByWordIdQueryHandler _handler;
        private readonly Word _word;

        public GetWordTranslationByWordQueryHandlerTests()
        {
            _word = Builder<Word>
                .CreateNew()
                .Build();

            var translations = Builder<Translation>
                .CreateListOfSize(4)
                .Build();
            foreach (var translation in translations)
            {
                _word.Translation.Add(translation);
            }

            DbContext.Word.Add(_word);
            DbContext.SaveChanges();

            _handler = new GetTranslationsByWordIdQueryHandler(DbContext);
        }

        [Test]
        public async Task WhenGettingTranslationForAWord_ShouldReturnCorrectTranslation()
        {
            var translations = await _handler.ExecuteAsync(new GetTranslationsByWordIdQuery(_word.Id));

            translations.ShouldNotBeNull();
            translations.ShouldNotBeEmpty();
            translations.ShouldBe(_word.Translation);
        }

        [Test]
        public async Task WhenGettingIncorrectTranslation_ShouldReturnEmpty()
        {
            var translations = await _handler.ExecuteAsync(new GetTranslationsByWordIdQuery(-1));

            translations.ShouldNotBeNull();
            translations.ShouldBeEmpty();
        }
    }
}