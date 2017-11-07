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
    public class GetWordByIdQueryHandlerTests : DatabaseTest
    {
        private readonly GetWordByIdQueryHandler _handler;
        private readonly Dictionary _dictionary;
        private readonly IList<Word> _words;

        public GetWordByIdQueryHandlerTests()
        {
            _dictionary = Builder<Dictionary>
                .CreateNew()
                .Build();
            _words = Builder<Word>
                .CreateListOfSize(50)
                .Build();

            foreach (var word in _words)
            {
                _dictionary.Word.Add(word);
            }

            DbContext.Dictionary.Add(_dictionary);
            DbContext.SaveChanges();

            _handler = new GetWordByIdQueryHandler(DbContext);
        }

        [Fact]
        public async Task WhenCallingForWordFromADictionary_ShouldReturnWord()
        {
            var word = await _handler.ExecuteAsync(new GetWordByIdQuery(_dictionary.Id, _words[45].Id));

            word.ShouldNotBeNull();
            word.ShouldBe(_words[45]);
        }

        [Fact]
        public async Task WhenCallingForWordFromIncorrectDictionary_ShouldReturnNull()
        {
            var word = await _handler.ExecuteAsync(new GetWordByIdQuery(-3, _words[22].Id));

            word.ShouldBeNull();
        }

        [Fact]
        public async Task WhenCallingForNonExistingWordFromDictionary_ShouldReturnNull()
        {
            var word = await _handler.ExecuteAsync(new GetWordByIdQuery(_dictionary.Id, -12423));

            word.ShouldBeNull();
        }
    }
}