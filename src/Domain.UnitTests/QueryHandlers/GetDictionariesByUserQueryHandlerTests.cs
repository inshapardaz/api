using Inshapardaz.Domain;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;

namespace Domain.UnitTests.QueryHandlers
{
    public class GetDictionariesByUserQueryHandlerTests
    {
        private GetDictionariesByUserQueryHandler _handler;
        private DatabaseContext _database;

        public GetDictionariesByUserQueryHandlerTests()
        {
            var inMemoryDataContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
               .Options;

            _database = new DatabaseContext(inMemoryDataContextOptions);
            _handler = new GetDictionariesByUserQueryHandler(_database);

            _database.Dictionaries.Add(new Dictionary { Id = 1, IsPublic = true, UserId = "1" });
            _database.Dictionaries.Add(new Dictionary { Id = 2, IsPublic = true, UserId = "2" });
            _database.Dictionaries.Add(new Dictionary { Id = 3, IsPublic = false, UserId = "2" });
            _database.Dictionaries.Add(new Dictionary { Id = 4, IsPublic = false, UserId = "1" });
            _database.SaveChanges();

            _database.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }


        [Fact]
        public void WhenCallingForAnonymous_ShouldReturnAllPublicDictionaries()
        {
            var result = _handler.Execute(new GetDictionariesByUserQuery());

            Assert.Equal(result.Dictionaries.Count(), 2);
            Assert.Equal(result.Dictionaries.ElementAt(0).Id, 1);
            Assert.True(result.Dictionaries.ElementAt(0).IsPublic);
            Assert.Equal(result.Dictionaries.ElementAt(1).Id, 2);
            Assert.True(result.Dictionaries.ElementAt(1).IsPublic);
        }

        [Fact]
        public void WhenCalledForAUser_ShouldReturnPublicAndPrivateDitionaries()
        {
            var result = _handler.Execute(new GetDictionariesByUserQuery { UserId = "2" });

            Assert.Equal(result.Dictionaries.Count(), 3);

            Assert.Equal(result.Dictionaries.ElementAt(0).Id, 1);
            Assert.True(result.Dictionaries.ElementAt(0).IsPublic);

            Assert.Equal(result.Dictionaries.ElementAt(1).Id, 2);
            Assert.True(result.Dictionaries.ElementAt(1).IsPublic);

            Assert.Equal(result.Dictionaries.ElementAt(2).Id, 3);
            Assert.False(result.Dictionaries.ElementAt(2).IsPublic);
        }
    }
}
