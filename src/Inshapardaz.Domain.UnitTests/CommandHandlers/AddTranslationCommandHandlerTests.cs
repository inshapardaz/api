using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.CommandHandlers
{
    public class AddTranslationCommandHandlerTests : DatabaseTest
    {
        private readonly AddTranslationCommandHandler _handler;
        private const int DictionaryId = 3;
        private const long WordId = 3232;

        public AddTranslationCommandHandlerTests()
        {
            var word = Builder<Word>
                        .CreateNew()
                        .With(w => w.Id = WordId)
                        .With(w => w.DictionaryId = DictionaryId)
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
            var command = new AddTranslationCommand(DictionaryId, WordId, translation);

            await _handler.HandleAsync(command);

            var word = DbContext.Word.Include(w => w.Translation).SingleOrDefault(w => w.Id == WordId);

            word.ShouldNotBeNull();
            word.Translation.ShouldNotBeEmpty();
            word.Translation.First().ShouldBeSameAs(translation);
        }

        [Fact]
        public async Task WhenAddingTranslationToNonExistingWord_ShouldThrowNotFound()
        {
            var translation = Builder<Translation>
                .CreateNew()
                .Build();
            var command = new AddTranslationCommand(DictionaryId, 2323, translation);

            await _handler.HandleAsync(command).ShouldThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task WhenAddingTranslationToNonExistingDictionary_ShouldThrowNotFound()
        {
            var translation = Builder<Translation>
                .CreateNew()
                .Build();
            var command = new AddTranslationCommand(-2, WordId, translation);

            await _handler.HandleAsync(command).ShouldThrowAsync<NotFoundException>();
        }
    }
}
