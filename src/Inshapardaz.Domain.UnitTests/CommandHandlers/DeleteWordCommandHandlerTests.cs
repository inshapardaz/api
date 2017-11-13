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

        [Test]
        public async Task WhenDeletingWordFromDictionary_ShouldDeleteWord()
        {
            var command = new DeleteWordCommand(DictionaryId, WordId);

            await _handler.HandleAsync(command);

            var createdWord = DbContext.Word.SingleOrDefault(w => w.Id == WordId);

            createdWord.ShouldBeNull();
        }

        [Test]
        public async Task WhenDeletingNonExistingWord_ShouldThrowNotFound()
        {
            var command = new DeleteWordCommand(DictionaryId, 532532);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<NotFoundException>();
        }


        [Test]
        public async Task WhenDeletingWordFromIncorrectDictionary_ShouldThrowNotFound()
        {
            var command = new DeleteWordCommand(-3, WordId);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<NotFoundException>();
        }
    }
}
