using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.QueryHandlers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class GetDictionaryByMeaningIdQueryHandlerTests
    {
        private GetDictionaryByMeaningIdQueryHandler _handler;
        private DatabaseContext _database;

        public GetDictionaryByMeaningIdQueryHandlerTests()
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
                Word = new List<Word>
                {
                    new Word
                    {
                        Id = 1,
                        Title = "something",
                        WordDetail = new List<WordDetail>
                        {
                            new WordDetail
                            {
                                Id = 1,
                                Meaning = new[] {new Meaning {Id = 9}}
                            },
                            new WordDetail
                            {
                                Id = 2,
                                Meaning = new[] {new Meaning {Id = 10}}
                            }
                        }
                    }
                }
            };
            _database.Add(data);
            _database.SaveChanges();

            _handler = new GetDictionaryByMeaningIdQueryHandler(_database);
        }

        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }

        [Fact]
        public async Task WhenCalledShouldReturnTheDictionary()
        {
            var result = await _handler.ExecuteAsync(new Queries.DictionaryByMeaningIdQuery { MeaningId = 10 });

            Assert.NotNull(result);
        }

        [Fact]
        public async Task WhenCalledForNonExsistantId()
        {
            var result = await _handler.ExecuteAsync(new Queries.DictionaryByMeaningIdQuery { MeaningId = 3 });

            Assert.Null(result);
        }
    }
}