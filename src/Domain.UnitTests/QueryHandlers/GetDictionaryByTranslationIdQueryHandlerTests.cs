using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.QueryHandlers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class GetDictionaryByTranslationIdQueryHandlerTests
    {
        private GetDictionaryByTranslationIdQueryHandler _handler;
        private DatabaseContext _database;

        public GetDictionaryByTranslationIdQueryHandlerTests()
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
                        Id = 1,
                        Title = "something",
                        WordDetails = new List<WordDetail>
                        {
                            new WordDetail {
                                Id = 1,
                                Translations = new [] { new Translation { Id = 9 } }
                            },
                            new WordDetail {
                                Id = 2,
                                Translations = new [] { new Translation { Id = 10 } }
                            }
                        }
                    }
                }
            };
            _database.Add(data);
            _database.SaveChanges();

            _handler = new GetDictionaryByTranslationIdQueryHandler(_database);
        }

        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }

        [Fact]
        public async Task WhenCalledShouldReturnTheDictionary()
        {
            var result = await _handler.ExecuteAsync(new Queries.DictionaryByTranslationIdQuery { TranslationId = 10 });

            Assert.NotNull(result);
        }

        [Fact]
        public async Task WhenCalledForNonExsistantId()
        {
            var result = await _handler.ExecuteAsync(new Queries.DictionaryByTranslationIdQuery { TranslationId = 3 });

            Assert.Null(result);
        }
    }
}