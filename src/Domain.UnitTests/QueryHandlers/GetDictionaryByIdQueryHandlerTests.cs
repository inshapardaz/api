using Inshapardaz.Domain;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.QueryHandlers;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit;

namespace Domain.UnitTests.QueryHandlers
{
    public class GetDictionaryByIdQueryHandlerTests : DatabaseTestFixture
    {
        private GetDictionaryByIdQueryHandler _handler;

        public GetDictionaryByIdQueryHandlerTests()
        {
            _handler = new GetDictionaryByIdQueryHandler(_database);

            _database.Dictionaries.Add(new Dictionary { Id = 1, IsPublic = true, UserId = "1" });
            _database.Dictionaries.Add(new Dictionary { Id = 2, IsPublic = true, UserId = "2" });
            _database.Dictionaries.Add(new Dictionary { Id = 3, IsPublic = false, UserId = "2" });
            _database.Dictionaries.Add(new Dictionary { Id = 4, IsPublic = false, UserId = "1" });
            _database.SaveChanges();
        }

        [Fact]
        public void WhenCalledAsAnonymousAnId_ShouldReturPublicDictionaryForOtherUser()
        {
            var result = _handler.Execute(new Inshapardaz.Domain.Queries.GetDictionaryByIdQuery { DictionaryId = 2 });

            Assert.NotNull(result);
            Assert.True(result.IsPublic);
        }

        [Fact]
        public void WhenCalledAsAnonymousForPrivateDictionary_ShouldNotReutrnMatchingDictionary()
        {
            var result = _handler.Execute(new Inshapardaz.Domain.Queries.GetDictionaryByIdQuery { DictionaryId = 3 });

            Assert.Null(result);
        }


        [Fact]
        public void WhenCalledForUser_ShouldReturnPrivateDictionary()
        {
            var result = _handler.Execute(new Inshapardaz.Domain.Queries.GetDictionaryByIdQuery { UserId = "2", DictionaryId = 3 });

            Assert.NotNull(result);
            Assert.False(result.IsPublic);
        }

        [Fact]
        public void WhenCalledForUser_ShouldReturnPublicDictionary()
        {
            var result = _handler.Execute(new Inshapardaz.Domain.Queries.GetDictionaryByIdQuery { UserId = "2", DictionaryId = 1 });

            Assert.NotNull(result);
            Assert.True(result.IsPublic);
            Assert.NotEqual(result.UserId, "2");
        }
    }
}
