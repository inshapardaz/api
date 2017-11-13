using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Commands;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Exception;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Domain.UnitTests.CommandHandlers
{
    [TestFixture]
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

        [Test]
        public async Task WhenDeletingMeaningFromWord_ShouldDeleteMeaning()
        {
            var command = new DeleteWordMeaningCommand(DictionaryId, MeaningId);

            await _handler.HandleAsync(command);

            var parentWord = DbContext.Word.SingleOrDefault(w => w.Id == WordId);
            var removedMeaning = DbContext.Meaning.SingleOrDefault(m => m.Id == MeaningId);

            parentWord.ShouldNotBeNull();
            removedMeaning.ShouldBeNull();
        }

        [Test]
        public async Task WhenDeletingNonExistingMeaning_ShouldThrowNotFound()
        {
            var command = new DeleteWordMeaningCommand(DictionaryId, 532532);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<NotFoundException>();
        }

        [Test]
        public async Task WhenDeletingMeaningFromIncorrectDictionary_ShouldThrowNotFound()
        {
            var command = new DeleteWordMeaningCommand(-4, MeaningId);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<NotFoundException>();
        }
    }
}
