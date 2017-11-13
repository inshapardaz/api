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
    public class GetDictionaryWordCountQueryHandlerTests : DatabaseTest
    {
        private readonly GetDictionaryWordCountQueryHandler _handler;
        private readonly IList<Word> _words;
        private readonly Dictionary _dictionary;

        public GetDictionaryWordCountQueryHandlerTests()
        {
            _dictionary = Builder<Dictionary>
                .CreateNew()
                .With(d => d.Id = 23)
                .Build();
            _words = Builder<Word>
                .CreateListOfSize(34)
                .Build();

            foreach (var word in _words)
            {
                _dictionary.Word.Add(word);
            }

            DbContext.Dictionary.Add(_dictionary);
            DbContext.SaveChanges();

            _handler = new GetDictionaryWordCountQueryHandler(DbContext);
        }

        [Test]
        public async Task WhenCalledShouldReturnTheDictionary()
        {
            var result = await _handler.ExecuteAsync(new GetDictionaryWordCountQuery
            {
                DictionaryId = _dictionary.Id
            });

            result.ShouldBe(_words.Count);
        }

        [Test]
        public async Task WhenCalleddictionaryThatDoesnotExists_ShouldReturnNull()
        {
            var result = await _handler.ExecuteAsync(new GetDictionaryWordCountQuery { DictionaryId = -9});

            result.ShouldBe(0);
        }
    }
}