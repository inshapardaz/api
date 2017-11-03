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
    public class AddTranslationCommandHandlerTests : DatabaseTest
    {
        private readonly AddTranslationCommandHandler _handler;
        private readonly int WordId = 3;

        public AddTranslationCommandHandlerTests()
        {
            var word = Builder<Word>
                        .CreateNew()
                        .With(w => w.Id = WordId)
                        .Build();
            DbContext.Word.Add(word);
            DbContext.SaveChanges();

            _handler = new AddTranslationCommandHandler(DbContext);
        }

        [Fact]
        public async Task WhenAddingTranslationToWord_ShouldSaveTranslationForTheWord()
        {

            var translation = Builder<Translation>
                .CreateNew()
                .Build();
            var command = new AddTranslationCommand(WordId, translation);

            await _handler.HandleAsync(command);

            var word = DbContext.Word.Include(w => w.Translation).SingleOrDefault(w => w.Id == WordId);

            Assert.NotNull(word);
            Assert.NotEmpty(word.Translation);
            Assert.Equal(translation, word.Translation.First());
        }

        [Fact]
        public async Task WhenAddingTranslationToNonExistingWord_ShouldThrowNotFound()
        {
            var translation = Builder<Translation>
                .CreateNew()
                .Build();
            var command = new AddTranslationCommand(2323, translation);

            await Assert.ThrowsAsync<NotFoundException>(async () => await _handler.HandleAsync(command));
        }
    }
}
