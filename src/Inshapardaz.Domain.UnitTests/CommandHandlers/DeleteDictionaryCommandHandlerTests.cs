using System;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.CommandHandlers
{
    public class DeleteDictionaryCommandHandlerTests : IDisposable
    {
        private readonly DeleteDictionaryCommandHandler _handler;
        private readonly DatabaseContext _database;
        private readonly Guid _userId1;

        public DeleteDictionaryCommandHandlerTests()
        {
            var inMemoryDataContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _database = new DatabaseContext(inMemoryDataContextOptions);
            _database.Database.EnsureCreated();

            _userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            
            _database.Dictionary.Add(new Dictionary {Id = 1, IsPublic = true, UserId = _userId1});
            _database.Dictionary.Add(new Dictionary {Id = 2, IsPublic = true, UserId = userId2});
            _database.Dictionary.Add(new Dictionary {Id = 3, IsPublic = false, UserId = _userId1});
            _database.Dictionary.Add(new Dictionary {Id = 4, IsPublic = false, UserId = userId2});
            _database.SaveChanges();

            _handler = new DeleteDictionaryCommandHandler(_database);
        }

        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }

        [Fact]
        public async Task WhenRemovedPrivateDictionary_ShouldDeleteFromDatabase()
        {
            await _handler.HandleAsync(new DeleteDictionaryCommand {DictionaryId = 3, UserId = _userId1 });

            Assert.Null(_database.Dictionary.SingleOrDefault(d => d.Id == 3));
        }

        [Fact]
        public async Task WhenRemovedOwnPublicDictionary_ShouldDeleteFromDatabase()
        {
            await _handler.HandleAsync(new DeleteDictionaryCommand {DictionaryId = 1, UserId = _userId1 });

            Assert.Null(_database.Dictionary.SingleOrDefault(d => d.Id == 1));
        }

        [Fact]
        public async Task WhenRemovedSomeoneElsePrivateDictionary_ShouldNotDelete()
        {
            await Assert.ThrowsAsync<RecordNotFoundException>(async () =>
                 await _handler.HandleAsync(new DeleteDictionaryCommand { DictionaryId = 4, UserId = _userId1 }));
        }

        [Fact]
        public async Task WhenRemovedSomeoneElsePublicDictionary_ShouldNotDelete()
        {
            await Assert.ThrowsAsync<RecordNotFoundException>(async () =>
                await _handler.HandleAsync(new DeleteDictionaryCommand {DictionaryId = 2, UserId = _userId1 }));
        }
    }
}