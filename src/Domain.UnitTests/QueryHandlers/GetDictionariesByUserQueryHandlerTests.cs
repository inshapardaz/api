using System.Linq;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class GetDictionariesByUserQueryHandlerTests : DatabaseTestFixture
    {
        private GetDictionariesByUserQueryHandler _handler;

        public GetDictionariesByUserQueryHandlerTests()
        {
            _handler = new GetDictionariesByUserQueryHandler(_database);

            _database.Dictionaries.Add(new Dictionary { Id = 1, IsPublic = true, UserId = "1" });
            _database.Dictionaries.Add(new Dictionary { Id = 2, IsPublic = true, UserId = "2" });
            _database.Dictionaries.Add(new Dictionary { Id = 3, IsPublic = false, UserId = "2" });
            _database.Dictionaries.Add(new Dictionary { Id = 4, IsPublic = false, UserId = "1" });
            _database.SaveChanges();
        }

        [Fact]
        public void WhenCallingForAnonymous_ShouldReturnAllPublicDictionaries()
        {
            var result = _handler.Execute(new GetDictionariesByUserQuery());

            Assert.Equal(result.Count(), 2);
            Assert.Equal(result.ElementAt(0).Id, 1);
            Assert.True(result.ElementAt(0).IsPublic);
            Assert.Equal(result.ElementAt(1).Id, 2);
            Assert.True(result.ElementAt(1).IsPublic);
        }

        [Fact]
        public void WhenCalledForAUser_ShouldReturnPublicAndPrivateDitionaries()
        {
            var result = _handler.Execute(new GetDictionariesByUserQuery { UserId = "2" });

            Assert.Equal(result.Count(), 3);

            Assert.Equal(result.ElementAt(0).Id, 1);
            Assert.True(result.ElementAt(0).IsPublic);

            Assert.Equal(result.ElementAt(1).Id, 2);
            Assert.True(result.ElementAt(1).IsPublic);

            Assert.Equal(result.ElementAt(2).Id, 3);
            Assert.False(result.ElementAt(2).IsPublic);
        }
    }
}
