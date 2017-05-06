using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.QueryHandlers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class GetDictionaryByWordIdQueryHandlerTests
    {
        private GetDictionaryByWordIdQueryHandler _handler;
        private DatabaseContext _database;

        public GetDictionaryByWordIdQueryHandlerTests()
        {
            var inMemoryDataContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                               .Options;

            _database = new DatabaseContext(inMemoryDataContextOptions);
            _database.Database.EnsureCreated();

            var data = new Dictionary
            {
                Id = 1,
                IsPublic = true,
                UserId = "1",
                Words = new List<Word>
                {
                    new Word
                    {
                        Id = 2,
                        Title = "something"
                    }
                }
            };
            _database.Add(data);
            _database.SaveChanges();

            _handler = new GetDictionaryByWordIdQueryHandler(_database);
        }

        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }

        [Fact]
        public async Task WhenCalledShouldReturnTheDictionary()
        {
            var result = await _handler.ExecuteAsync(new Queries.GetDictionaryByWordIdQuery { WordId = 2 });

            Assert.NotNull(result);
        }

        [Fact]
        public async Task WhenCalledForNonExsistantId()
        {
            var result = await _handler.ExecuteAsync(new Queries.GetDictionaryByWordIdQuery { WordId = 3 });

            Assert.Null(result);
        }
    }
}