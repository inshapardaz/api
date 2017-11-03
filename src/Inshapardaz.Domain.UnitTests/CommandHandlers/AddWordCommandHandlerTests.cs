using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.CommandHandlers
{
    public class AddWordCommandHandlerTests : DatabaseTest
    {
        private readonly AddWordCommandHandler _handler;
        private readonly int DictionaryId = 3;

        public AddWordCommandHandlerTests()
        {
            var dictionary = Builder<Dictionary>
                        .CreateNew()
                        .With(w => w.Id = DictionaryId)
                        .Build();
            DbContext.Dictionary.Add(dictionary);
            DbContext.SaveChanges();

            _handler = new AddWordCommandHandler(DbContext);
        }

        [Fact]
        public async Task WhenAddingWordToDictionary_ShouldSaveMeaningForTheWord()
        {

            var word = Builder<Word>
                .CreateNew()
                .Build();
            var command = new AddWordCommand
            {
                DictionaryId = DictionaryId,
                Word = word
            };

            await _handler.HandleAsync(command);

            var createdWord = DbContext.Word.SingleOrDefault(w => w.Title == word.Title);

            Assert.NotNull(createdWord);
            Assert.Equal(DictionaryId, createdWord.DictionaryId);
        }

        [Fact]
        public async Task WhenAddingWordToNonExistingDictionary_ShouldThrowNotFound()
        {
            var word = Builder<Word>
                .CreateNew()
                .Build();
            var command = new AddWordCommand
            {
                DictionaryId = Builder<int>.CreateNew().Build(),
                Word = word
            };

            await Assert.ThrowsAsync<NotFoundException>(async () => await _handler.HandleAsync(command));
        }

        [Fact]
        public async Task WhenNullWord_ShouldThrowBadRequest()
        {
            var command = new AddWordCommand
            {
                DictionaryId = Builder<int>.CreateNew().Build(),
                Word = null
            };

            await Assert.ThrowsAsync<BadRequestException>(async () => await _handler.HandleAsync(command));
        }
    }
}
