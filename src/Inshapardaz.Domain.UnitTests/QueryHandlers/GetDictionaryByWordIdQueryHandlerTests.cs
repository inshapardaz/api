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
    public class GetDictionaryByWordIdQueryHandlerTests : DatabaseTest
    {
        private readonly GetDictionaryByWordIdQueryHandler _handler;
        private readonly IList<Word> _words;
        private readonly Dictionary _dictionary;

        public GetDictionaryByWordIdQueryHandlerTests()
        {
            _dictionary = Builder<Dictionary>
                .CreateNew()
                .With(d => d.Id = 23)
                .Build();
            _words = Builder<Word>
                .CreateListOfSize(3)
                .Build();

            foreach (var word in _words)
            {
                _dictionary.Word.Add(word);
            }

            DbContext.Dictionary.Add(_dictionary);
            DbContext.SaveChanges();

            _handler = new GetDictionaryByWordIdQueryHandler(DbContext);
        }

        [Fact]
        public async Task WhenCalledShouldReturnTheDictionary()
        {
            var result = await _handler.ExecuteAsync(new DictionaryByWordIdQuery(_words[1].Id));

            result.ShouldNotBeNull();
            result.ShouldBe(_dictionary);
        }

        [Fact]
        public async Task WhenCalleddictionaryThatDoesnotExists_ShouldReturnNull()
        {
            var result = await _handler.ExecuteAsync(new DictionaryByWordIdQuery(-9));

            result.ShouldBeNull();
        }
    }
}