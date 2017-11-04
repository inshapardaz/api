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
    public class DeleteWordTranslationCommandHandlerTests : DatabaseTest
    {
        private readonly DeleteWordTranslationCommandHandler _handler;
        private readonly int DictionaryId = 3;
        private readonly long WordId = 12;
        private readonly long TranslationId = 323;

        public DeleteWordTranslationCommandHandlerTests()
        {
            var dictionary = Builder<Dictionary>
                        .CreateNew()
                        .With(d => d.Id = DictionaryId)
                        .Build();

            var word = Builder<Word>
                .CreateNew()
                .With(w => w.Id = WordId)
                .Build();
            var translation = Builder<Translation>
                .CreateNew()
                .With(m => m.Id = TranslationId)
                .Build();
            word.Translation.Add(translation);
            dictionary.Word.Add(word);
            DbContext.Dictionary.Add(dictionary);

            DbContext.SaveChanges();

            _handler = new DeleteWordTranslationCommandHandler(DbContext);
        }

        [Fact]
        public async Task WhenDeletingTranslationFromWord_ShouldDeleteTranlation()
        {
            var command = new DeleteWordTranslationCommand(DictionaryId, TranslationId);

            await _handler.HandleAsync(command);

            var parentWord = DbContext.Word.SingleOrDefault(w => w.Id == WordId);
            var removedTranslation = DbContext.Translation.SingleOrDefault(m => m.Id == TranslationId);
            parentWord.ShouldNotBeNull();
            removedTranslation.ShouldBeNull();
        }

        [Fact]
        public async Task WhenDeletingNonExistingWord_ShouldThrowNotFound()
        {
            var command = new DeleteWordTranslationCommand(DictionaryId, 532532);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<NotFoundException>();
        }
    }
}
