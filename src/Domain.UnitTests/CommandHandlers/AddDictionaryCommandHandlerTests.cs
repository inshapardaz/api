using System;
using System.Linq;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.CommandHandlers
{
    public class AddDictionaryCommandHandlerTests
    {
        private AddDictionaryCommandHandler _handler;
        private DatabaseContext _database;

        public AddDictionaryCommandHandlerTests()
        {
            var inMemoryDataContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                               .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                               .Options;

            _database = new DatabaseContext(inMemoryDataContextOptions);
            _database.Database.EnsureCreated();
            _handler = new AddDictionaryCommandHandler(_database);
        }

        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }

        [Fact]
        public void WhenAdded_ShouldSaveToDatabase()
        {
            var name = "Test";
            _handler.Handle(new AddDictionaryCommand { Dictionary = new Dictionary() { UserId = "2", IsPublic = false, Name = name, Language = 3 } });

            Assert.Equal(_database.Dictionaries.Count(), 1);
            Assert.Equal(_database.Dictionaries.First().Name, name);
            Assert.Equal(_database.Dictionaries.First().Language, 3);
            Assert.False(_database.Dictionaries.First().IsPublic);
        }
    }
}