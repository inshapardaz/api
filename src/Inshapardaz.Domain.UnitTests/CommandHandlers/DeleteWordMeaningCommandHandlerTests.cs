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
    public class DeleteWordMeaningCommandHandlerTests : DatabaseTest
    {
        private readonly DeleteWordMeaningCommandHandler _handler;
        private readonly int DictionaryId = 3;
        private readonly long WordId = 12;
        private readonly long MeaningId = 323;

        public DeleteWordMeaningCommandHandlerTests()
        {
            var dictionary = Builder<Dictionary>
                        .CreateNew()
                        .With(d => d.Id = DictionaryId)
                        .Build();

            var word = Builder<Word>
                .CreateNew()
                .With(w => w.Id = WordId)
                .Build();
            var meaning = Builder<Meaning>
                .CreateNew()
                .With(m => m.Id = MeaningId)
                .Build();
            word.Meaning.Add(meaning);
            dictionary.Word.Add(word);
            DbContext.Dictionary.Add(dictionary);

            DbContext.SaveChanges();

            _handler = new DeleteWordMeaningCommandHandler(DbContext);
        }

        [Fact]
        public async Task WhenDeletinWordToDictionary_ShouldDeleteWord()
        {
            var command = new DeleteWordMeaningCommand(DictionaryId, MeaningId);

            await _handler.HandleAsync(command);

            var parentWord = DbContext.Word.SingleOrDefault(w => w.Id == WordId);
            var removedMeaning = DbContext.Meaning.SingleOrDefault(m => m.Id == MeaningId);

            parentWord.ShouldNotBeNull();
            removedMeaning.ShouldBeNull();
        }

        [Fact]
        public async Task WhenDeletingNonExistingWord_ShouldThrowNotFound()
        {
            var command = new DeleteWordMeaningCommand(DictionaryId, 532532);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<NotFoundException>();
        }
    }
}
