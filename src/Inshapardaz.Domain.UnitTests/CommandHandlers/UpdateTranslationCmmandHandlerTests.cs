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
    public class UpdateTranslationCmmandHandlerTests : DatabaseTest
    {
        private readonly UpdateWordTranslationCommandHandler _handler;
        private readonly int DictionaryId = 4;
        private readonly Translation _translation;

        public UpdateTranslationCmmandHandlerTests()
        {
            var word = Builder<Word>
                .CreateNew()
                .With(w => w.DictionaryId = DictionaryId)
                .Build();

            _translation = Builder<Translation>
                .CreateNew()
                .With(t => t.Word = word)
                .Build();
            DbContext.Word.Add(word);
            DbContext.Translation.Add(_translation);

            DbContext.SaveChanges();

            _handler = new UpdateWordTranslationCommandHandler(DbContext);
        }

        [Fact]
        public async Task WhenUpdateTranslation_ShouldUpdateTranslationFields()
        {
            var translation = Builder<Translation>
                .CreateNew()
                .With(w => w.Id = _translation.Id)
                .Build();
            await _handler.HandleAsync(new UpdateWordTranslationCommand(DictionaryId, translation));

            var updatedTranslation = DbContext.Translation.Single(w => w.Id == _translation.Id);

            updatedTranslation.ShouldNotBeNull();
            updatedTranslation.Language.ShouldBe(translation.Language);
            updatedTranslation.Value.ShouldBe(translation.Value);
            updatedTranslation.WordId.ShouldBe(translation.WordId);
        }

        [Fact]
        public async Task WhenUpdatingNonExistingTranslation_ShouldThrowNotFound()
        {
            var meaning = new Translation
            {
                Id = 30203
            };

            await _handler.HandleAsync(new UpdateWordTranslationCommand(DictionaryId, meaning))
                          .ShouldThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task WhenUpdatingTranslationFromIncorrectDictionary_ShouldThrowNotFound()
        {
            await _handler.HandleAsync(new UpdateWordTranslationCommand(-5, _translation))
                          .ShouldThrowAsync<NotFoundException>();
        }
    }
}