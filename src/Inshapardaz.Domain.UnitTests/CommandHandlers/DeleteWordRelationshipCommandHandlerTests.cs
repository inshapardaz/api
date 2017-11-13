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
    public class DeleteWordRelationshipCommandHandlerTests : DatabaseTest
    {
        private readonly DeleteWordRelationshipCommandHandler _handler;
        private readonly int DictionaryId = 3;
        private readonly long WordId = 12;
        private readonly long RelationshipId = 323;

        public DeleteWordRelationshipCommandHandlerTests()
        {
            var dictionary = Builder<Dictionary>
                        .CreateNew()
                        .With(d => d.Id = DictionaryId)
                        .Build();

            var sourceWord = Builder<Word>
                .CreateNew()
                .With(w => w.Id = WordId)
                .With(w => w.Dictionary = dictionary)
                .Build();
            var destinationWord = Builder<Word>
                .CreateNew()
                .With(w => w.Id = WordId)
                .With(w => w.Dictionary = dictionary)
                .Build();
            var relationship = Builder<WordRelation>
                .CreateNew()
                .With(r => r.Id = RelationshipId)
                .With(r => r.SourceWord = sourceWord)
                .With(r => r.RelatedWord = destinationWord)
                .Build();
            sourceWord.WordRelationRelatedWord.Add(relationship);
            dictionary.Word.Add(sourceWord);
            DbContext.Dictionary.Add(dictionary);

            DbContext.SaveChanges();

            _handler = new DeleteWordRelationshipCommandHandler(DbContext);
        }

        [Test]
        public async Task WhenDeletingRelationship_ShouldDeleteRelation()
        {
            var command = new DeleteWordRelationshipCommand(DictionaryId, RelationshipId);

            await _handler.HandleAsync(command);

            var parentWord = DbContext.Word.SingleOrDefault(w => w.Id == WordId);
            var removedRelationship = DbContext.WordRelation.SingleOrDefault(m => m.Id == RelationshipId);
            parentWord.ShouldNotBeNull();
            removedRelationship.ShouldBeNull();
        }

        [Test]
        public async Task WhenDeletingNonExistingRelation_ShouldThrowNotFound()
        {
            var command = new DeleteWordRelationshipCommand(DictionaryId, 532532);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<NotFoundException>();
        }

        [Test]
        public async Task WhenDeletingRelationFromIncorrectDictionary_ShouldThrowNotFound()
        {
            var command = new DeleteWordRelationshipCommand(-2, RelationshipId);

            await _handler.HandleAsync(command)
                          .ShouldThrowAsync<NotFoundException>();
        }
    }
}
