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
    public class GetWordMeaningByContextQueryHandlerTests : DatabaseTest
    {
        private readonly GetWordMeaningsByContextQueryHandler _handler;
        private readonly IList<Meaning> _meanings;
        private readonly Word _word;

        public GetWordMeaningByContextQueryHandlerTests()
        {
            var dictionary = Builder<Dictionary>
                .CreateNew()
                .Build();
            _word = Builder<Word>
                .CreateNew()
                .Build();

            _meanings = Builder<Meaning>
                .CreateListOfSize(4)
                .Build();

            foreach (var meaning in _meanings)
            {
                _word.Meaning.Add(meaning);
            }
            dictionary.Word.Add(_word);

            DbContext.Dictionary.Add(dictionary);
            DbContext.SaveChanges();

            _handler = new GetWordMeaningsByContextQueryHandler(DbContext);
        }

        [Test]
        public async Task WhenGettingContextMeaningFromAWord_ShouldReturnCorrectMeaning()
        {
            var meanings = await _handler.ExecuteAsync(new GetWordMeaningsByContextQuery(_word.Id, _meanings[2].Context));

            meanings.ShouldNotBeNull();
            meanings.ShouldNotBeEmpty();
            meanings.ShouldContain(_meanings[2]);
        }

        [Test]
        public async Task WhenGettingContextMeaningFromIncorrectWord_ShouldReturnEmpty()
        {
            var meanings = await _handler.ExecuteAsync(new GetWordMeaningsByContextQuery(-1, _meanings[3].Context));

            meanings.ShouldNotBeNull();
            meanings.ShouldBeEmpty();
        }
    }
}