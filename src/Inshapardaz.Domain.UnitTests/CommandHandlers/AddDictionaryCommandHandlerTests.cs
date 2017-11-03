using System;
using System.Linq;
using System.Threading.Tasks;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
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
            var name = "Test";
            await _handler.HandleAsync(new AddDictionaryCommand
            {
                Dictionary = new Dictionary { UserId = Guid.NewGuid(), IsPublic = false, Name = name, Language = Languages.Avestan }
            });

            Assert.Equal(DbContext.Dictionary.Count(), 1);
            Assert.Equal(DbContext.Dictionary.First().Name, name);
            Assert.Equal(DbContext.Dictionary.First().Language, Languages.Avestan);
            Assert.False(DbContext.Dictionary.First().IsPublic);
        }
    }
}