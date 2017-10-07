using System;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.CommandHandlers
{
    public class AddDictionaryCommandHandlerTests : IDisposable
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
        public async Task WhenAdded_ShouldSaveToDatabase()
        {
            var name = "Test";
            await _handler.HandleAsync(new AddDictionaryCommand
            {
                Dictionary = new Dictionary() { UserId = Guid.NewGuid(), IsPublic = false, Name = name, Language = Languages.Avestan }
            });

            Assert.Equal(_database.Dictionary.Count(), 1);
            Assert.Equal(_database.Dictionary.First().Name, name);
            Assert.Equal(_database.Dictionary.First().Language, Languages.Avestan);
            Assert.False(_database.Dictionary.First().IsPublic);
        }
    }
}