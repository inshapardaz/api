using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class WordByTitleQueryHandlerTests : DatabaseTestFixture
    {
        private WordByTitleQueryHandler _handler;

        public WordByTitleQueryHandlerTests()
        {
            _handler = new WordByTitleQueryHandler(_database);

            _database.Dictionaries.Add(new Dictionary { Id = 1, UserId = "1", IsPublic = false });
            _database.Dictionaries.Add(new Dictionary { Id = 2, UserId = "1", IsPublic = true });
            _database.Words.Add(new Word { Id = 22, Title = "word1", DictionaryId = 1 });
            _database.Words.Add(new Word { Id = 23, Title = "word2", DictionaryId = 2 });
            _database.SaveChanges();
        }

        [Fact]
        public void WhenCallingForWordFromPublicDictionary_ShouldReturnWord()
        {
            var word = _handler.Execute(new WordByTitleQuery { Title = "word2", UserId = "1" });

            Assert.NotNull(word);
            Assert.Equal(word.Id, 23);
        }

        [Fact]
        public void WhenCallingForWordFromPrivateDictionary_ShouldReturnWord()
        {
            var word = _handler.Execute(new WordByTitleQuery { Title = "word1", UserId = "1" });

            Assert.NotNull(word);
            Assert.Equal(word.Id, 22);
        }

        [Fact]
        public void WhenCallingForWordFromPublicDictionaryAsAnonymousUser_ShouldReturnWord()
        {
            var word = _handler.Execute(new WordByTitleQuery { Title = "word2" });

            Assert.NotNull(word);
            Assert.Equal(word.Id, 23);
        }

        [Fact]
        public void WhenCallingForWordFromPrivateDictionaryAsAnonymousUser_ShouldNotReturnWord()
        {
            var word = _handler.Execute(new WordByTitleQuery { Title = "word 1" });

            Assert.Null(word);
        }

        [Fact]
        public void WhenCallingForInvalidWordFromPrivateDictionaryAsAnonymousUser_ShouldNotReturnWord()
        {
            var word = _handler.Execute(new WordByTitleQuery { Title = "somethingNew" });

            Assert.Null(word);
        }

    }
}
