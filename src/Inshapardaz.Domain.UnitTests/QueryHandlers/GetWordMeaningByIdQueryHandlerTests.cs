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

        [Test]
        public async Task WhenGettingMeaningById_ShouldReturnCorrectMeaning()
        {
            var meaning = await _handler.ExecuteAsync(new GetWordMeaningByIdQuery(_meanings[2].Id));

            meaning.ShouldNotBeNull();
            meaning.ShouldBe(_meanings[2]);
        }

        [Test]
        public async Task WhenGetMeaningForIncorrectId_ShouldReturnNull()
        {
            var meaning = await _handler.ExecuteAsync(new GetWordMeaningByIdQuery(-232));

            meaning.ShouldBeNull();
        }
    }
}