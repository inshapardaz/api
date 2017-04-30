using System.Linq;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Model;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.CommandHandlers
{
    public class DeleteDictionaryCommandHandlerTests : DatabaseTestFixture
    {
        private DeleteDictionaryCommandHandler _handler;

        public DeleteDictionaryCommandHandlerTests()
        {
            _handler = new DeleteDictionaryCommandHandler(_database);
            _database.Dictionaries.Add(new Dictionary { Id = 1, IsPublic = true, UserId = "1" });
            _database.Dictionaries.Add(new Dictionary { Id = 2, IsPublic = true, UserId = "2" });
            _database.Dictionaries.Add(new Dictionary { Id = 3, IsPublic = false, UserId = "1" });
            _database.Dictionaries.Add(new Dictionary { Id = 4, IsPublic = false, UserId = "2" });
            _database.SaveChanges();
        }

        [Fact]
        public void WhenRemovedPrivateDictionary_ShouldDeleteFromDatabase()
        {
            _handler.Handle(new DeleteDictionaryCommand { DictionaryId = 3, UserId = "1" });

            Assert.Null(_database.Dictionaries.SingleOrDefault(d => d.Id == 3));
        }

        [Fact]
        public void WhenRemovedOwnPublicDictionary_ShouldDeleteFromDatabase()
        {
            _handler.Handle(new DeleteDictionaryCommand { DictionaryId = 1, UserId = "1" });

            Assert.Null(_database.Dictionaries.SingleOrDefault(d => d.Id == 1));
        }

        [Fact]
        public void WhenRemovedSomeoneElsePrivateDictionary_ShouldNotDelete()
        {
            Assert.Throws<RecordNotFoundException>(() => 
                    _handler.Handle(new DeleteDictionaryCommand { DictionaryId = 4, UserId = "1" }));
        }

        [Fact]
        public void WhenRemovedSomeoneElsePublicDictionary_ShouldNotDelete()
        {
            Assert.Throws<RecordNotFoundException>(() =>
                    _handler.Handle(new DeleteDictionaryCommand { DictionaryId = 2, UserId = "1" }));
        }
    }
}
