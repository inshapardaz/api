using System;
using System.Linq;
using System.Threading.Tasks;
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

            _database.Dictionary.Add(new Dictionary
            {
                Id = 1,
                IsPublic = true,
                UserId = "1",
                Language = Languages.Avestan
            });
            _database.Dictionary.Add(new Dictionary
            {
                Id = 2,
                IsPublic = true,
                UserId = "2",
                Language = Languages.Chinese
            });
            _database.Dictionary.Add(new Dictionary
            {
                Id = 3,
                IsPublic = false,
                UserId = "1",
                Language = Languages.English
            });
            _database.Dictionary.Add(new Dictionary
            {
                Id = 4,
                IsPublic = false,
                UserId = "2",
                Language = Languages.German
            });

            _database.SaveChanges();

            _handler = new UpdateDictionaryCommandHandler(_database);
        }

        public void Dispose()
        {
            _database.Database.EnsureDeleted();
        }

        [Fact]
        public async Task WhenUpdatePrivateDictionary_ShouldUpdateDictionaryFields()
        {
            await _handler.HandleAsync(new UpdateDictionaryCommand
            {
                Dictionary = new Dictionary
                {
                    Id = 3,
                    UserId = "1",
                    Language = Languages.Hindi,
                    Name = "Some Name",
                    IsPublic = true
                }
            });

            var dictionary = _database.Dictionary.Single(d => d.Id == 3);

            Assert.NotNull(dictionary);
            Assert.Equal(dictionary.Name, "Some Name", true);
            Assert.Equal(dictionary.Language, Languages.Hindi);
            Assert.Equal(dictionary.UserId, "1");
            Assert.True(dictionary.IsPublic);
        }

        [Fact]
        public async Task WhenRemovedOwnPublicDictionary_ShouldDeleteFromDatabase()
        {
            await _handler.HandleAsync(new UpdateDictionaryCommand
            {
                Dictionary = new Dictionary { Id = 1, UserId = "1", Language = Languages.Japanese }
            });

            Assert.Equal(_database.Dictionary.Single(d => d.Id == 1).Language, Languages.Japanese);
        }

        [Fact]
        public async Task WhenRemovedSomeoneElsePrivateDictionary_ShouldNotDelete()
        {
            await Assert.ThrowsAsync<RecordNotFoundException>(async () =>
                await _handler.HandleAsync(new UpdateDictionaryCommand
                {
                    Dictionary = new Dictionary { Id = 4, UserId = "1", Language = Languages.Persian }
                }));
        }

        [Fact]
        public async Task WhenRemovedSomeoneElsePublicDictionary_ShouldNotDelete()
        {
            await Assert.ThrowsAsync<RecordNotFoundException>(async () =>
                await _handler.HandleAsync(new UpdateDictionaryCommand
                {
                    Dictionary = new Dictionary { Id = 2, UserId = "1", Language = Languages.Persian }
                }));
        }
    }
}