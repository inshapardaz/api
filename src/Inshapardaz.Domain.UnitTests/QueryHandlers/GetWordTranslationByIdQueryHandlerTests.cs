using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using Shouldly;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class GetWordTranslationByIdQueryHandlerTests : DatabaseTest
    {
        private readonly GetTranslationByIdQueryHandler _handler;
        private readonly IList<Translation> _translations;

        public GetWordTranslationByIdQueryHandlerTests()
        {
            var dictionary = Builder<Dictionary>
                .CreateNew()
                .Build();
            var word = Builder<Word>
                .CreateNew()
                .Build();

            _translations = Builder<Translation>
                .CreateListOfSize(3)
                .Build();

            foreach (var translation in _translations)
            {
                word.Translation.Add(translation);
            }

            dictionary.Word.Add(word);

            DbContext.Dictionary.Add(dictionary);
            DbContext.SaveChanges();

            _handler = new GetTranslationByIdQueryHandler(DbContext);
        }

        [Fact]
        public async Task WhenGettingTranslationById_ShouldReturnCorrectTranslation()
        {
            var translation = await _handler.ExecuteAsync(new GetTranslationByIdQuery(_translations[2].Id));

            translation.ShouldNotBeNull();
            translation.ShouldBe(_translations[2]);
        }

        [Fact]
        public async Task WhenGetTranslationForIncorrectId_ShouldReturnNull()
        {
            var translation = await _handler.ExecuteAsync(new GetTranslationByIdQuery(-232));

            translation.ShouldBeNull();
        }
    }
}