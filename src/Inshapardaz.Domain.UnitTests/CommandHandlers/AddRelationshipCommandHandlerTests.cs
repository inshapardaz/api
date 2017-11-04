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
        private readonly int _dictionaryId1 = 2;
        private readonly int _dictionaryId2 = 4;
        private readonly long _wordId1 = 653;
        private readonly long _wordId2 = 53;
        private readonly long _wordId3 = 34;

        public AddRelationshipCommandHandlerTests()
        {
            var word1 = Builder<Word>
                .CreateNew()
                .With(w => w.Id = _wordId1)
                .With(w => w.DictionaryId = _dictionaryId1)
                .Build();
            var word2 = Builder<Word>
                .CreateNew()
                .With(w => w.Id = _wordId2)
                .With(w => w.DictionaryId = _dictionaryId1)
                .Build();
            var word3 = Builder<Word>
                .CreateNew()
                .With(w => w.Id = _wordId3)
                .With(w => w.DictionaryId = _dictionaryId2)
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
            var command = new AddWordRelationCommand(_dictionaryId1, _wordId1, _wordId2, RelationType.Halat);

            await _handler.HandleAsync(command);

            var relation = DbContext.WordRelation.SingleOrDefault(w => w.Id == command.Result);

            relation.ShouldNotBeNull();
            relation.SourceWordId.ShouldBe(_wordId1);
            relation.RelatedWordId.ShouldBe(_wordId2);
            relation.RelationType.ShouldBe(RelationType.Halat);
        }

        [Fact]
        public async Task WhenAddingRelationshipFromNonExistingWord_ShouldThrowNotFound()
        {
            var command = new AddWordRelationCommand(_dictionaryId1, 2323, _wordId2, RelationType.Acronym);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task WhenAddingRelationshipToNonExistingWord_ShouldThrowBadRequest()
        {
            var command = new AddWordRelationCommand(_dictionaryId1, _wordId1, 23234, RelationType.Acronym);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task WhenAddingRelationshipToItself_ShouldThrowBadRequest()
        {
            var command = new AddWordRelationCommand(_dictionaryId1, _wordId1, _wordId1, RelationType.Acronym);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<BadRequestException>();
        }

        [Fact]
        public async Task WhenAddingRelationshipToWordInOtherDictionary_ShouldThrowBadRequest()
        {
            var command = new AddWordRelationCommand(_dictionaryId1, _wordId1, _wordId3, RelationType.Acronym);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<BadRequestException>();
        }
    }
}
