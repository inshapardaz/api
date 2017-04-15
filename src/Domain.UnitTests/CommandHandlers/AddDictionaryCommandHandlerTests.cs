using Inshapardaz.Domain;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Domain.UnitTests.CommandHandlers
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
            _handler = new AddDictionaryCommandHandler(_database);
            _database.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }

        [Fact]
        public void WhenAdded_ShouldSaveToDatabase()
        {
            var name = "Test";
            _handler.Handle(new AddDictionaryCommand { UserId = "2", Dictionary = new Dictionary() { UserId = "2", IsPublic = false, Name = name, Language = 3 } });

            Assert.Equal(_database.Dictionaries.Count(), 1);
            Assert.Equal(_database.Dictionaries.First().Name, name);
            Assert.Equal(_database.Dictionaries.First().Language, 3);
            Assert.False(_database.Dictionaries.First().IsPublic);
        }
    }
}
