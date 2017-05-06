using System;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Domain.Model;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
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
            _database.Database.EnsureCreated();

            _database.Dictionaries.Add(new Dictionary { Id = 1, IsPublic = true, UserId = "1" });
            _database.Dictionaries.Add(new Dictionary { Id = 2, IsPublic = true, UserId = "2" });
            _database.Dictionaries.Add(new Dictionary { Id = 3, IsPublic = false, UserId = "2" });
            _database.Dictionaries.Add(new Dictionary { Id = 4, IsPublic = false, UserId = "1" });
            _database.SaveChanges();

            _handler = new GetDictionariesByUserQueryHandler(_database);
        }

        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }

        [Fact]
        public async Task WhenCallingForAnonymous_ShouldReturnAllPublicDictionaries()
        {
            var result = await _handler.ExecuteAsync(new GetDictionariesByUserQuery());

            Assert.Equal(result.Count(), 2);
            Assert.Equal(result.ElementAt(0).Id, 1);
            Assert.True(result.ElementAt(0).IsPublic);
            Assert.Equal(result.ElementAt(1).Id, 2);
            Assert.True(result.ElementAt(1).IsPublic);
        }

        [Fact]
        public async Task WhenCalledForAUser_ShouldReturnPublicAndPrivateDitionaries()
        {
            var result = await _handler.ExecuteAsync(new GetDictionariesByUserQuery { UserId = "2" });

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