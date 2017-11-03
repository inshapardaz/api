using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.CommandHandlers
{
    public class AddMeaningCommandHandlerTests : DatabaseTest
    {
        private readonly AddMeaningCommandHandler _handler;
        private readonly int WordId = 3;

        public AddMeaningCommandHandlerTests()
        {
            var word = Builder<Word>
                        .CreateNew()
                        .With(w => w.Id = WordId)
                        .Build();
            DbContext.Word.Add(word);
            DbContext.SaveChanges();

            _handler = new AddMeaningCommandHandler(DbContext);
        }

        [Fact]
        public async Task WhenAddingMeaningToWord_ShouldSaveMeaningForTheWord()
        {

            var meaning = Builder<Meaning>
                .CreateNew()
                .Build();
            var command = new AddMeaningCommand
            {
                WordId = WordId,
                Meaning = meaning
            };

            await _handler.HandleAsync(command);

            var word = DbContext.Word.Include(w => w.Meaning).SingleOrDefault(w => w.Id == WordId);

            Assert.NotNull(word);
            Assert.NotEmpty(word.Meaning);
            Assert.Equal(meaning, word.Meaning.First());
        }

        [Fact]
        public async Task WhenAddingMeaningToNonExistingWord_ShouldThrowNotFound()
        {
            var meaning = Builder<Meaning>
                .CreateNew()
                .Build();
            var command = new AddMeaningCommand
            {
                WordId = 5334,
                Meaning = meaning
            };

            await Assert.ThrowsAsync<NotFoundException>(async () => await _handler.HandleAsync(command));
        }

        [Fact]
        public async Task WhenAddingNullMeaning_ShouldThrowBadRequest()
        {
            var command = new AddMeaningCommand
            {
                WordId = 5334,
                Meaning = null
            };

            await Assert.ThrowsAsync<BadRequestException>(async () => await _handler.HandleAsync(command));
        }
    }
}
