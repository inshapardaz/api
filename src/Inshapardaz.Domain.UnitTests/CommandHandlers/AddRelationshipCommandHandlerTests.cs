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
    public class AddRelationshipCommandHandlerTests : DatabaseTest
    {
        private readonly AddWordRelationCommandHandler _handler;
        private const int DictionaryId1 = 2;
        private const int DictionaryId2 = 4;
        private const long WordId1 = 653;
        private const long WordId2 = 53;
        private const long WordId3 = 34;

        public AddRelationshipCommandHandlerTests()
        {
            var word1 = Builder<Word>
                .CreateNew()
                .With(w => w.Id = WordId1)
                .With(w => w.DictionaryId = DictionaryId1)
                .Build();
            var word2 = Builder<Word>
                .CreateNew()
                .With(w => w.Id = WordId2)
                .With(w => w.DictionaryId = DictionaryId1)
                .Build();
            var word3 = Builder<Word>
                .CreateNew()
                .With(w => w.Id = WordId3)
                .With(w => w.DictionaryId = DictionaryId2)
                .Build();

            DbContext.Word.Add(word1);
            DbContext.Word.Add(word2);
            DbContext.Word.Add(word3);
            DbContext.SaveChanges();

            _handler = new AddWordRelationCommandHandler(DbContext);
        }

        [Fact]
        public async Task WhenAddingRelationshipToWord_ShouldSaveTranslationForTheWord()
        {
            var command = new AddWordRelationCommand(DictionaryId1, WordId1, WordId2, RelationType.Halat);

            await _handler.HandleAsync(command);

            var relation = DbContext.WordRelation.SingleOrDefault(w => w.Id == command.Result);

            relation.ShouldNotBeNull();
            relation.SourceWordId.ShouldBe(WordId1);
            relation.RelatedWordId.ShouldBe(WordId2);
            relation.RelationType.ShouldBe(RelationType.Halat);
        }

        [Fact]
        public async Task WhenAddingRelationshipFromNonExistingWord_ShouldThrowNotFound()
        {
            var command = new AddWordRelationCommand(DictionaryId1, 2323, WordId2, RelationType.Acronym);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task WhenAddingRelationshipToNonExistingWord_ShouldThrowBadRequest()
        {
            var command = new AddWordRelationCommand(DictionaryId1, WordId1, 23234, RelationType.Acronym);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task WhenAddingRelationshipToNonExistingDictionary_ShouldThrowNotFoundException()
        {
            var command = new AddWordRelationCommand(-1, WordId1, WordId2, RelationType.Acronym);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task WhenAddingRelationshipToItself_ShouldThrowBadRequest()
        {
            var command = new AddWordRelationCommand(DictionaryId1, WordId1, WordId1, RelationType.Acronym);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task WhenAddingRelationshipToWordInOtherDictionary_ShouldThrowBadRequest()
        {
            var command = new AddWordRelationCommand(DictionaryId1, WordId1, WordId3, RelationType.Acronym);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<BadRequestException>();
        }
    }
}
