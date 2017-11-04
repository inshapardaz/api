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
    public class AddMeaningCommandHandlerTests : DatabaseTest
    {
        private readonly AddMeaningCommandHandler _handler;
        private readonly int DictionaryId = 3;
        private readonly long WordId = 332;

        public AddMeaningCommandHandlerTests()
        {
            var word = Builder<Word>
                        .CreateNew()
                        .With(w => w.Id = WordId)
                        .With(w => w.DictionaryId = DictionaryId)
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
            var command = new AddMeaningCommand(DictionaryId, WordId, meaning);

            await _handler.HandleAsync(command);

            var word = DbContext.Word.Include(w => w.Meaning).SingleOrDefault(w => w.Id == WordId);

            word.ShouldNotBeNull();
            word.Meaning.ShouldNotBeEmpty();
            var createdMeaning = word.Meaning.FirstOrDefault();
            createdMeaning.ShouldNotBeNull();
            createdMeaning.ShouldBeSameAs(meaning);
        }

        [Fact]
        public async Task WhenAddingMeaningToNonExistingWord_ShouldThrowNotFound()
        {
            var meaning = Builder<Meaning>
                .CreateNew()
                .Build();
            var command = new AddMeaningCommand(DictionaryId, 5334, meaning);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task WhenAddingMeaningToNonExistingDictionary_ShouldThrowNotFound()
        {
            var meaning = Builder<Meaning>
                .CreateNew()
                .Build();
            var command = new AddMeaningCommand(32, WordId, meaning);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<NotFoundException>();
        }
    }
}
