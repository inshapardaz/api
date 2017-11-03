using System;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.CommandHandlers
{
    public class UpdateDictionaryCommandHandlerTests : DatabaseTest
    {
        private readonly UpdateDictionaryCommandHandler _handler;
        private readonly Guid _userId1;

        public UpdateDictionaryCommandHandlerTests()
        {
            _userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            
            DbContext.Dictionary.Add(new Dictionary
            {
                Id = 1,
                IsPublic = true,
                UserId = _userId1,
                Language = Languages.Avestan
            });
            DbContext.Dictionary.Add(new Dictionary
            {
                Id = 2,
                IsPublic = true,
                UserId = userId2,
                Language = Languages.Chinese
            });
            DbContext.Dictionary.Add(new Dictionary
            {
                Id = 3,
                IsPublic = false,
                UserId = _userId1,
                Language = Languages.English
            });
            DbContext.Dictionary.Add(new Dictionary
            {
                Id = 4,
                IsPublic = false,
                UserId = userId2,
                Language = Languages.German
            });

            DbContext.SaveChanges();

            _handler = new UpdateDictionaryCommandHandler(DbContext);
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

            var dictionary = DbContext.Dictionary.Single(d => d.Id == 3);

            Assert.NotNull(dictionary);
            Assert.Equal("Some Name", dictionary.Name, true);
            Assert.Equal(Languages.Hindi, dictionary.Language);
            Assert.Equal(dictionary.UserId, _userId1);
            Assert.True(dictionary.IsPublic);
        }


        [Fact]
        public async Task WhenUpdatingNonExistingDictionary_ShouldThrowNotFound()
        {
            await Assert.ThrowsAsync<NotFoundException>(async () => 
            await _handler.HandleAsync(new UpdateDictionaryCommand
            {
                Dictionary = new Dictionary
                {
                    Id = 30203
                }
            }));
        }
    }
}