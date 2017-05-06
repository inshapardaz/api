using System;
using System.Linq;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Model;
using Microsoft.EntityFrameworkCore;

using Xunit;

namespace Inshapardaz.Domain.UnitTests.CommandHandlers
{
    public class DeleteDictionaryCommandHandlerTests
    {
        private DeleteDictionaryCommandHandler _handler;
        private DatabaseContext _database;

        public DeleteDictionaryCommandHandlerTests()
        {
            var inMemoryDataContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                               .Options;

            _database = new DatabaseContext(inMemoryDataContextOptions);
            _database.Database.EnsureCreated();

            _database.Dictionaries.Add(new Dictionary { Id = 1, IsPublic = true, UserId = "1" });
            _database.Dictionaries.Add(new Dictionary { Id = 2, IsPublic = true, UserId = "2" });
            _database.Dictionaries.Add(new Dictionary { Id = 3, IsPublic = false, UserId = "1" });
            _database.Dictionaries.Add(new Dictionary { Id = 4, IsPublic = false, UserId = "2" });
            _database.SaveChanges();

            _handler = new DeleteDictionaryCommandHandler(_database);
        }

        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }

        [Fact]
        public void WhenRemovedPrivateDictionary_ShouldDeleteFromDatabase()
        {
            _handler.Handle(new DeleteDictionaryCommand { DictionaryId = 3, UserId = "1" });

            Assert.Null(_database.Dictionaries.SingleOrDefault(d => d.Id == 3));
        }

        [Fact]
        public void WhenRemovedOwnPublicDictionary_ShouldDeleteFromDatabase()
        {
            _handler.Handle(new DeleteDictionaryCommand { DictionaryId = 1, UserId = "1" });

            Assert.Null(_database.Dictionaries.SingleOrDefault(d => d.Id == 1));
        }

        [Fact]
        public void WhenRemovedSomeoneElsePrivateDictionary_ShouldNotDelete()
        {
            Assert.Throws<RecordNotFoundException>(() =>
                    _handler.Handle(new DeleteDictionaryCommand { DictionaryId = 4, UserId = "1" }));
        }

        [Fact]
        public void WhenRemovedSomeoneElsePublicDictionary_ShouldNotDelete()
        {
            Assert.Throws<RecordNotFoundException>(() =>
                    _handler.Handle(new DeleteDictionaryCommand { DictionaryId = 2, UserId = "1" }));
        }
    }
}