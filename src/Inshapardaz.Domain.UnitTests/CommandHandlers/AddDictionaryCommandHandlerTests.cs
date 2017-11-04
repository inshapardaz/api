using System;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Shouldly;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.CommandHandlers
{
    public class AddDictionaryCommandHandlerTests : DatabaseTest
    {
        private readonly AddDictionaryCommandHandler _handler;

        public AddDictionaryCommandHandlerTests()
        {
            _handler = new AddDictionaryCommandHandler(DbContext);
        }

        [Fact]
        public async Task WhenAdded_ShouldSaveToDatabase()
        {
            var dictionary = new Dictionary
            {
                UserId = Guid.NewGuid(),
                IsPublic = false,
                Name = "Test",
                Language = Languages.Avestan
            };

            await _handler.HandleAsync(new AddDictionaryCommand(dictionary));

            DbContext.Dictionary.ShouldNotBeEmpty();

            var createdDictionary = DbContext.Dictionary.First(d => d.Id == dictionary.Id);
            createdDictionary.ShouldNotBeNull();
            createdDictionary.ShouldBeSameAs(dictionary);
        }
    }
}