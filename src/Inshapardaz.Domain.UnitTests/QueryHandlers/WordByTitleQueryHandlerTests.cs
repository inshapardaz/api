﻿using System;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class WordByTitleQueryHandlerTests : IDisposable
    {
        private WordByTitleQueryHandler _handler;
        private DatabaseContext _database;

        public WordByTitleQueryHandlerTests()
        {
            var inMemoryDataContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _database = new DatabaseContext(inMemoryDataContextOptions);
            _database.Database.EnsureCreated();

            var userId = Guid.NewGuid();
            _database.Dictionary.Add(new Dictionary { Id = 1, UserId = userId, IsPublic = false });
            _database.Dictionary.Add(new Dictionary { Id = 2, UserId = userId, IsPublic = true });
            _database.Word.Add(new Word { Id = 22, Title = "word1", DictionaryId = 1 });
            _database.Word.Add(new Word { Id = 23, Title = "word2", DictionaryId = 2 });
            _database.SaveChanges();

            _handler = new WordByTitleQueryHandler(_database);
        }

        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }

        [Fact]
        public async Task WhenCallingForWordFromPublicDictionary_ShouldReturnWord()
        {
            var word = await _handler.ExecuteAsync(new WordByTitleQuery { Title = "word2", UserId = "1" });

            Assert.NotNull(word);
            Assert.Equal(word.Id, 23);
        }

        [Fact]
        public async Task WhenCallingForWordFromPrivateDictionary_ShouldReturnWord()
        {
            var word = await _handler.ExecuteAsync(new WordByTitleQuery { Title = "word1", UserId = "1" });

            Assert.NotNull(word);
            Assert.Equal(word.Id, 22);
        }

        [Fact]
        public async Task WhenCallingForWordFromPublicDictionaryAsAnonymousUser_ShouldReturnWord()
        {
            var word = await _handler.ExecuteAsync(new WordByTitleQuery { Title = "word2" });

            Assert.NotNull(word);
            Assert.Equal(word.Id, 23);
        }

        [Fact]
        public async Task WhenCallingForWordFromPrivateDictionaryAsAnonymousUser_ShouldNotReturnWord()
        {
            var word = await _handler.ExecuteAsync(new WordByTitleQuery { Title = "word 1" });

            Assert.Null(word);
        }

        [Fact]
        public async Task WhenCallingForInvalidWordFromPrivateDictionaryAsAnonymousUser_ShouldNotReturnWord()
        {
            var word = await _handler.ExecuteAsync(new WordByTitleQuery { Title = "somethingNew" });

            Assert.Null(word);
        }
    }
}