using System.Collections.Generic;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.QueryHandlers;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class GetDictionaryByWordIdQueryHandlerTests : DatabaseTestFixture
    {
        private GetDictionaryByWordIdQueryHandler _handler;

        public GetDictionaryByWordIdQueryHandlerTests()
        {
            _handler = new GetDictionaryByWordIdQueryHandler(_database);

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
        }

        [Fact]
        public void WhenCalledShouldReturnTheDictionary()
        {
            var result = _handler.Execute(new Queries.GetDictionaryByWordIdQuery { WordId = 2 });

            Assert.NotNull(result);
        }

        [Fact]
        public void WhenCalledForNonExsistantId()
        {
            var result = _handler.Execute(new Queries.GetDictionaryByWordIdQuery { WordId = 3 });

            Assert.Null(result);
        }
    }
}
