using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Model;
using System.Linq;
using Xunit;

namespace Domain.UnitTests.CommandHandlers
{
    public class AddDictionaryCommandHandlerTests : DatabaseTestFixture
    {
        private AddDictionaryCommandHandler _handler;

        public AddDictionaryCommandHandlerTests()
        {
            _handler = new AddDictionaryCommandHandler(_database);
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
