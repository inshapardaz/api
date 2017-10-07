using System;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.QueryHandlers;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class GetDictionaryByIdQueryHandlerTests : IDisposable
    {
        private GetDictionaryByIdQueryHandler _handler;
        private DatabaseContext _database;
        private readonly Guid _userId1;
        private readonly Guid _userId2;

        public GetDictionaryByIdQueryHandlerTests()
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

            _handler = new GetDictionaryByIdQueryHandler(_database);
        }

        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }

        [Fact]
        public async Task WhenCalledAsAnonymousAnId_ShouldReturPublicDictionaryForOtherUser()
        {
            var result = await _handler.ExecuteAsync(new Queries.DictionaryByIdQuery { DictionaryId = 2 });

            Assert.NotNull(result);
            Assert.True(result.IsPublic);
        }

        [Fact]
        public async Task WhenCalledAsAnonymousForPrivateDictionary_ShouldNotReutrnMatchingDictionary()
        {
            var result = await _handler.ExecuteAsync(new Queries.DictionaryByIdQuery { DictionaryId = 3 });

            Assert.Null(result);
        }

        [Fact]
        public async Task WhenCalledForUser_ShouldReturnPrivateDictionary()
        {
            var result = await _handler.ExecuteAsync(new Queries.DictionaryByIdQuery { UserId = _userId2, DictionaryId = 3 });

            Assert.NotNull(result);
            Assert.False(result.IsPublic);
        }

        [Fact]
        public async Task WhenCalledForUser_ShouldReturnPublicDictionary()
        {
            var result = await _handler.ExecuteAsync(new Queries.DictionaryByIdQuery { UserId = _userId2, DictionaryId = 1 });

            Assert.NotNull(result);
            Assert.True(result.IsPublic);
            Assert.NotEqual(result.UserId, _userId2);
        }
    }
}