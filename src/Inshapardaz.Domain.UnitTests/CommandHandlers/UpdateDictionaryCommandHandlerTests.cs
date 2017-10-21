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
    public class UpdateDictionaryCommandHandlerTests : IDisposable
    {
        private readonly UpdateDictionaryCommandHandler _handler;
        private readonly DatabaseContext _database;
        private readonly Guid _userId1;

        public UpdateDictionaryCommandHandlerTests()
        {
            var inMemoryDataContextOptions = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _database = new DatabaseContext(inMemoryDataContextOptions);
            _database.Database.EnsureCreated();

            _userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            
            _database.Dictionary.Add(new Dictionary
            {
                Id = 1,
                IsPublic = true,
                UserId = _userId1,
                Language = Languages.Avestan
            });
            _database.Dictionary.Add(new Dictionary
            {
                Id = 2,
                IsPublic = true,
                UserId = userId2,
                Language = Languages.Chinese
            });
            _database.Dictionary.Add(new Dictionary
            {
                Id = 3,
                IsPublic = false,
                UserId = _userId1,
                Language = Languages.English
            });
            _database.Dictionary.Add(new Dictionary
            {
                Id = 4,
                IsPublic = false,
                UserId = userId2,
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
                    UserId = _userId1,
                    Language = Languages.Hindi,
                    Name = "Some Name",
                    IsPublic = true
                }
            });

            var dictionary = _database.Dictionary.Single(d => d.Id == 3);

            Assert.NotNull(dictionary);
            Assert.Equal("Some Name", dictionary.Name, true);
            Assert.Equal(Languages.Hindi, dictionary.Language);
            Assert.Equal(dictionary.UserId, _userId1);
            Assert.True(dictionary.IsPublic);
        }

        [Fact]
        public async Task WhenRemovedOwnPublicDictionary_ShouldDeleteFromDatabase()
        {
            await _handler.HandleAsync(new UpdateDictionaryCommand
            {
                Dictionary = new Dictionary { Id = 1, UserId = _userId1, Language = Languages.Japanese }
            });

            Assert.Equal(_database.Dictionary.Single(d => d.Id == 1).Language, Languages.Japanese);
        }

        [Fact]
        public async Task WhenRemovedSomeoneElsePrivateDictionary_ShouldNotDelete()
        {
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.HandleAsync(new UpdateDictionaryCommand
                {
                    Dictionary = new Dictionary { Id = 4, UserId = _userId1, Language = Languages.Persian }
                }));
        }

        [Fact]
        public async Task WhenRemovedSomeoneElsePublicDictionary_ShouldNotDelete()
        {
            await Assert.ThrowsAsync<NotFoundException>(async () =>
                await _handler.HandleAsync(new UpdateDictionaryCommand
                {
                    Dictionary = new Dictionary { Id = 2, UserId = _userId1, Language = Languages.Persian }
                }));
        }
    }
}