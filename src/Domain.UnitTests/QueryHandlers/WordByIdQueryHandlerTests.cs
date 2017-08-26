using System;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class WordByIdQueryHandlerTests
    {
        private WordByIdQueryHandler _handler;
        private DatabaseContext _database;

        public WordByIdQueryHandlerTests()
        {
            var inMemoryDataContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _database = new DatabaseContext(inMemoryDataContextOptions);
            _database.Database.EnsureCreated();

            _database.Dictionary.Add(new Dictionary { Id = 1, UserId = "1", IsPublic = false });
            _database.Dictionary.Add(new Dictionary { Id = 2, UserId = "1", IsPublic = true });
            _database.Word.Add(new Word { Id = 22, Title = "word1", DictionaryId = 1 });
            _database.Word.Add(new Word { Id = 23, Title = "word2", DictionaryId = 2 });
            _database.SaveChanges();

            _handler = new WordByIdQueryHandler(_database);
        }

        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }

        [Fact]
        public async Task WhenCallingForWordFromPublicDictionary_ShouldReturnWord()
        {
            var word = await _handler.ExecuteAsync(new WordByIdQuery { Id = 23, UserId = "1" });

            Assert.NotNull(word);
            Assert.Equal(word.Id, 23);
        }

        [Fact]
        public async Task WhenCallingForWordFromPrivateDictionary_ShouldReturnWord()
        {
            var word = await _handler.ExecuteAsync(new WordByIdQuery { Id = 22, UserId = "1" });

            Assert.NotNull(word);
            Assert.Equal(word.Id, 22);
        }

        [Fact]
        public async Task WhenCallingForWordFromPublicDictionaryAsAnonymousUser_ShouldReturnWord()
        {
            var word = await _handler.ExecuteAsync(new WordByIdQuery { Id = 23 });

            Assert.NotNull(word);
            Assert.Equal(word.Id, 23);
        }

        [Fact]
        public async Task WhenCallingForWordFromPrivateDictionaryAsAnonymousUser_ShouldNotReturnWord()
        {
            var word = await _handler.ExecuteAsync(new WordByIdQuery { Id = 22 });

            Assert.Null(word);
        }

        [Fact]
        public async Task WhenCallingForInvalidWordFromPrivateDictionaryAsAnonymousUser_ShouldNotReturnWord()
        {
            var word = await _handler.ExecuteAsync(new WordByIdQuery { Id = 223 });

            Assert.Null(word);
        }
    }
}