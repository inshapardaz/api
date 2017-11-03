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
    public class DeleteDictionaryCommandHandlerTests : DatabaseTest
    {
        private readonly DeleteDictionaryCommandHandler _handler;

        public DeleteDictionaryCommandHandlerTests()
        {
            DbContext.Dictionary.Add(new Dictionary {Id = 1, IsPublic = true });
            DbContext.Dictionary.Add(new Dictionary {Id = 2, IsPublic = true });
            DbContext.Dictionary.Add(new Dictionary {Id = 3, IsPublic = false });
            DbContext.Dictionary.Add(new Dictionary {Id = 4, IsPublic = false });
            DbContext.SaveChanges();

            _handler = new DeleteDictionaryCommandHandler(DbContext);
        }

        [Fact]
        public async Task WhenRemovedPrivateDictionary_ShouldDeleteFromDatabase()
        {
            await _handler.HandleAsync(new DeleteDictionaryCommand {DictionaryId = 3 });

            Assert.Null(DbContext.Dictionary.SingleOrDefault(d => d.Id == 3));
        }

        [Fact]
        public async Task IfDictionaryIsNotFound_ShouldThrowException()
        {
            await Assert.ThrowsAsync<NotFoundException>(async () => await _handler.HandleAsync(new DeleteDictionaryCommand { DictionaryId = 7 }));
        }
    }
}