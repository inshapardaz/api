using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Shouldly;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.CommandHandlers
{
    public class DeleteWordCommandHandlerTests : DatabaseTest
    {
        private readonly DeleteWordCommandHandler _handler;
        private readonly int DictionaryId = 3;
        private readonly long WordId = 323;

        public DeleteWordCommandHandlerTests()
        {
            var dictionary = Builder<Dictionary>
                        .CreateNew()
                        .With(d => d.Id = DictionaryId)
                        .Build();

            var word = Builder<Word>
                .CreateNew()
                .With(w => w.Id = WordId)
                .Build();
            dictionary.Word.Add(word);
            DbContext.Dictionary.Add(dictionary);

            DbContext.SaveChanges();

            _handler = new DeleteWordCommandHandler(DbContext);
        }

        [Fact]
        public async Task WhenDeletinWordToDictionary_ShouldDeleteWord()
        {
            var command = new DeleteWordCommand(DictionaryId, WordId);

            await _handler.HandleAsync(command);

            var createdWord = DbContext.Word.SingleOrDefault(w => w.Id == WordId);

            createdWord.ShouldBeNull();
        }

        [Fact]
        public async Task WhenDeletingNonExistingWord_ShouldThrowNotFound()
        {
            var command = new DeleteWordCommand(DictionaryId, 532532);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<NotFoundException>();
        }
    }
}
