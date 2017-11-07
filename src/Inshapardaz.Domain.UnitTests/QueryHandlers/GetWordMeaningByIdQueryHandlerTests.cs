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
    public class GetWordMeaningByIdQueryHandlerTests : DatabaseTest
    {
        private readonly GetWordMeaningByIdQueryHandler _handler;
        private readonly IList<Meaning> _meanings;

        public GetWordMeaningByIdQueryHandlerTests()
        {
            var dictionary = Builder<Dictionary>
                .CreateNew()
                .Build();
            var word = Builder<Word>
                .CreateNew()
                .Build();

            _meanings = Builder<Meaning>
                .CreateListOfSize(3)
                .Build();

            foreach (var meaning in _meanings)
            {
                word.Meaning.Add(meaning);
            }
            dictionary.Word.Add(word);

            DbContext.Dictionary.Add(dictionary);
            DbContext.SaveChanges();

            _handler = new GetWordMeaningByIdQueryHandler(DbContext);
        }

        [Fact]
        public async Task WhenGettingMeaningById_ShouldReturnCorrectMeaning()
        {
            var word = await _handler.ExecuteAsync(new GetWordMeaningByIdQuery(_meanings[2].Id));

            word.ShouldNotBeNull();
            word.ShouldBe(_meanings[2]);
        }

        [Fact]
        public async Task WhenGetMeaningForIncorrectId_ShouldReturnNull()
        {
            var word = await _handler.ExecuteAsync(new GetWordMeaningByIdQuery(-232));

            word.ShouldBeNull();
        }
    }
}