using System;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class GetDictionariesByUserQueryHandlerTests : IDisposable
    {
        private readonly GetDictionariesByUserQueryHandler _handler;
        private readonly DatabaseContext _database;
        private readonly Guid _userId1;
        private readonly Guid _userId2;

        public GetDictionariesByUserQueryHandlerTests()
        {
            var inMemoryDataContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _database = new DatabaseContext(inMemoryDataContextOptions);
            _database.Database.EnsureCreated();

            _userId1 = Guid.NewGuid();
            _userId2 = Guid.NewGuid();
            _database.Dictionary.Add(new Dictionary {Id = 1, IsPublic = true, UserId = _userId1});
            _database.Dictionary.Add(new Dictionary {Id = 2, IsPublic = true, UserId = _userId2});
            _database.Dictionary.Add(new Dictionary {Id = 3, IsPublic = false, UserId = _userId2});
            _database.Dictionary.Add(new Dictionary {Id = 4, IsPublic = false, UserId = _userId1});
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
            var result = await _handler.ExecuteAsync(new DictionariesByUserQuery());

            Assert.Equal(result.Count(), 2);
            Assert.Equal(result.ElementAt(0).Id, 1);
            Assert.True(result.ElementAt(0).IsPublic);
            Assert.Equal(result.ElementAt(1).Id, 2);
            Assert.True(result.ElementAt(1).IsPublic);
        }

        [Fact]
        public async Task WhenCalledForAUser_ShouldReturnPublicAndPrivateDictionaries()
        {
            var result = await _handler.ExecuteAsync(new DictionariesByUserQuery {UserId = _userId2 });

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