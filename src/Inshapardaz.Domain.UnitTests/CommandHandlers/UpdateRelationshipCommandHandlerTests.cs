using System;
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
    public class UpdateRelationshipCommandHandlerTests : DatabaseTest
    {
        private readonly UpdateWordRelationCommandHandler _handler;
        private readonly int DictionaryId = 3;
        private readonly WordRelation _relation;

        public UpdateRelationshipCommandHandlerTests()
        {
            var sourceWord = Builder<Word>
                .CreateNew()
                .With(w => w.DictionaryId = DictionaryId)
                .With(w => w.Id = 2)
                .Build();
            var relatedWord = Builder<Word>
                .CreateNew()
                .With(w => w.DictionaryId = DictionaryId)
                .With(w => w.Id = 3)
                .Build();
            _relation = Builder<WordRelation>
                            .CreateNew()
                            .With(r => r.SourceWord = sourceWord)
                            .With(r => r.RelatedWord = relatedWord)
                            .Build();

            DbContext.Word.Add(sourceWord);
            DbContext.Word.Add(relatedWord);
            DbContext.WordRelation.Add(_relation);

            DbContext.SaveChanges();

            _handler = new UpdateWordRelationCommandHandler(DbContext);
        }

        [Fact]
        public async Task WhenUpdateRelation_ShouldUpdateRelationTypeAndRelatedWordFields()
        {
            var wordRelation = Builder<WordRelation>
                .CreateNew()
                .With(w => w.Id = _relation.Id)
                .Build();
            await _handler.HandleAsync(new UpdateWordRelationCommand(DictionaryId, wordRelation));

            var updatedRelation = DbContext.WordRelation.Single(w => w.Id == _relation.Id);

            updatedRelation.ShouldNotBeNull();
            updatedRelation.SourceWordId.ShouldBe(_relation.SourceWordId);
            updatedRelation.RelatedWordId.ShouldBe(wordRelation.RelatedWordId);
            updatedRelation.RelationType.ShouldBe(wordRelation.RelationType);
        }


        [Fact]
        public async Task WhenUpdatingNonExistingRelation_ShouldThrowNotFound()
        {
            var relation = new WordRelation
            {
                Id = 30203
            };

            await _handler.HandleAsync(new UpdateWordRelationCommand(DictionaryId, relation)).ShouldThrowAsync<NotFoundException>();
        }
    }
}