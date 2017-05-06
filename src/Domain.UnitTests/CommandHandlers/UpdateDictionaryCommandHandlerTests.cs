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
    public class UpdateDictionaryCommandHandlerTests
    {
        private UpdateDictionaryCommandHandler _handler;
        private DatabaseContext _database;

        public UpdateDictionaryCommandHandlerTests()
        {
            var inMemoryDataContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                               .Options;

            _database = new DatabaseContext(inMemoryDataContextOptions);
            _database.Database.EnsureCreated();

            _database.Dictionaries.Add(new Dictionary { Id = 1, IsPublic = true, UserId = "1", Language = 1 });
            _database.Dictionaries.Add(new Dictionary { Id = 2, IsPublic = true, UserId = "2", Language = 2 });
            _database.Dictionaries.Add(new Dictionary { Id = 3, IsPublic = false, UserId = "1", Language = 3 });
            _database.Dictionaries.Add(new Dictionary { Id = 4, IsPublic = false, UserId = "2", Language = 4 });

            _database.SaveChanges();

            _handler = new UpdateDictionaryCommandHandler(_database);
        }

        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }

        [Fact]
        public void WhenUpdatePrivateDictionary_ShouldUpdateDictionaryFields()
        {
            _handler.Handle(new UpdateDictionaryCommand { Dictionary = new Dictionary { Id = 3, UserId = "1", Language = 33, Name = "Some Name", IsPublic = true } });

            var dictionary = _database.Dictionaries.Single(d => d.Id == 3);

            Assert.NotNull(dictionary);
            Assert.Equal(dictionary.Name, "Some Name", true);
            Assert.Equal(dictionary.Language, 33);
            Assert.Equal(dictionary.UserId, "1");
            Assert.True(dictionary.IsPublic);
        }

        [Fact]
        public void WhenRemovedOwnPublicDictionary_ShouldDeleteFromDatabase()
        {
            _handler.Handle(new UpdateDictionaryCommand { Dictionary = new Dictionary { Id = 1, UserId = "1", Language = 11 } });

            Assert.Equal(_database.Dictionaries.Single(d => d.Id == 1).Language, 11);
        }

        [Fact]
        public void WhenRemovedSomeoneElsePrivateDictionary_ShouldNotDelete()
        {
            Assert.Throws<RecordNotFoundException>(() =>
                    _handler.Handle(new UpdateDictionaryCommand { Dictionary = new Dictionary { Id = 4, UserId = "1", Language = 44 } }));
        }

        [Fact]
        public void WhenRemovedSomeoneElsePublicDictionary_ShouldNotDelete()
        {
            Assert.Throws<RecordNotFoundException>(() =>
                    _handler.Handle(new UpdateDictionaryCommand { Dictionary = new Dictionary { Id = 2, UserId = "1", Language = 22 } }));
        }
    }
}