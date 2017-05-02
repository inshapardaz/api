using System.Collections.Generic;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.QueryHandlers;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class GetDictionaryByTranslationIdQueryHandlerTests : DatabaseTestFixture
    {
        private GetDictionaryByTranslationIdQueryHandler _handler;

        public GetDictionaryByTranslationIdQueryHandlerTests()
        {
            _handler = new GetDictionaryByTranslationIdQueryHandler(_database);

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
        }

        [Fact]
        public void WhenCalledShouldReturnTheDictionary()
        {
            var result = _handler.ExecuteAsync(new Queries.GetDictionaryByTranslationIdQuery { TranslationId = 10 }).Result;

            Assert.NotNull(result);
        }

        [Fact]
        public void WhenCalledForNonExsistantId()
        {
            var result = _handler.ExecuteAsync(new Queries.GetDictionaryByTranslationIdQuery { TranslationId = 3 }).Result;

            Assert.Null(result);
        }
    }
}