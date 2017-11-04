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
    public class UpdateWordCommandHandlerTests : DatabaseTest
    {
        private readonly UpdateWordCommandHandler _handler;
        private readonly int DictionaryId = 5;
        private readonly Word _word;
        private Dictionary _dictionary;

        public UpdateWordCommandHandlerTests()
        {
            _dictionary = Builder<Dictionary>
                .CreateNew()
                .With(d => d.Id = DictionaryId)
                .Build();
            _word = Builder<Word>
                    .CreateNew()
                    .With(w => w.Dictionary = _dictionary)
                    .Build();

            DbContext.Dictionary.Add(_dictionary);
            DbContext.Word.Add(_word);

            DbContext.SaveChanges();

            _handler = new UpdateWordCommandHandler(DbContext);
        }

        [Fact]
        public async Task WhenUpdateWord_ShouldUpdateWordFields()
        {
            var word = Builder<Word>
                .CreateNew()
                .With(w => w.Id = _word.Id)
                .With(w => w.Dictionary = _dictionary)
                .Build();
            await _handler.HandleAsync(new UpdateWordCommand(DictionaryId, word));

            var updatedWord = DbContext.Word.Single(w => w.Id == _word.Id);

            updatedWord.ShouldNotBeNull();
            updatedWord.Title.ShouldBe(word.Title);
            updatedWord.Description.ShouldBe(word.Description);
            updatedWord.Translation.ShouldBe(word.Translation);
            updatedWord.Attributes.ShouldBe(word.Attributes);
            updatedWord.Language.ShouldBe(word.Language);
            updatedWord.Pronunciation.ShouldBe(word.Pronunciation);
            updatedWord.DictionaryId.ShouldBe(DictionaryId);
        }

        [Fact]
        public async Task WhenUpdatingNonExistingWord_ShouldThrowNotFound()
        {
            var word = new Word
            {
                Id = 30203
            };

            await _handler.HandleAsync(new UpdateWordCommand(DictionaryId, word)).ShouldThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task WhenUpdatingWordInIncorrectDictionary_ShouldThrowNotFound()
        {
            await _handler.HandleAsync(new UpdateWordCommand(-6, _word)).ShouldThrowAsync<NotFoundException>();
        }
    }
}