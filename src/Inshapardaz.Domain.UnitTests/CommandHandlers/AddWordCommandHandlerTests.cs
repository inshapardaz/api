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
            var command = new AddWordCommand(DictionaryId, word);

            await _handler.HandleAsync(command);

            var createdWord = DbContext.Word.SingleOrDefault(w => w.Title == word.Title);

            createdWord.ShouldNotBeNull();
            createdWord.ShouldBeSameAs(word);
            createdWord.DictionaryId.ShouldBe(DictionaryId);
        }

        [Fact]
        public async Task WhenAddingWordToNonExistingDictionary_ShouldThrowNotFound()
        {
            var word = Builder<Word>
                .CreateNew()
                .Build();
            var command = new AddWordCommand(Builder<int>.CreateNew().Build(), word);

            await _handler.HandleAsync(command).ShouldThrowAsync<NotFoundException>();
        }
    }
}
