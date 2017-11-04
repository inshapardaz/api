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
    public class UpdateMeaningCommandHandlerTests : DatabaseTest
    {
        private readonly UpdateWordMeaningCommandHandler _handler;
        private readonly int DictionaryId = 2;
        private readonly Meaning _meaning;

        public UpdateMeaningCommandHandlerTests()
        {
            var word = Builder<Word>
                .CreateNew()
                .With(w => w.DictionaryId = DictionaryId)
                .Build();
            _meaning = Builder<Meaning>
                        .CreateNew()
                        .With(m => m.Word = word)
                        .Build();
            DbContext.Word.Add(word);
            DbContext.Meaning.Add(_meaning);

            DbContext.SaveChanges();

            _handler = new UpdateWordMeaningCommandHandler(DbContext);
        }

        [Fact]
        public async Task WhenUpdateMeaning_ShouldUpdateMeaningFields()
        {
            var meaning = Builder<Meaning>
                .CreateNew()
                .With(w => w.Id = _meaning.Id)
                .Build();
            await _handler.HandleAsync(new UpdateWordMeaningCommand(DictionaryId, meaning));

            var updatedMeaning = DbContext.Meaning.Single(w => w.Id == _meaning.Id);

            updatedMeaning.ShouldNotBeNull();
            updatedMeaning.Context.ShouldBe(meaning.Context);
            updatedMeaning.Example.ShouldBe(meaning.Example);
            updatedMeaning.Value.ShouldBe(meaning.Value);
            updatedMeaning.WordId.ShouldBe(meaning.WordId);
        }
        
        [Fact]
        public async Task WhenUpdatingNonExistingMeaning_ShouldThrowNotFound()
        {
            var meaning = new Meaning
            {
                Id = 30203
            };

            await _handler.HandleAsync(new UpdateWordMeaningCommand(DictionaryId, meaning)).ShouldThrowAsync<NotFoundException>();
        }

        [Fact]
        public async Task WhenUpdatingMeaningFromIncorrectDictionary_ShouldThrowNotFound()
        {
            var meaning = new Meaning{ Id = 30203 };

            await _handler.HandleAsync(new UpdateWordMeaningCommand(-4, meaning)).ShouldThrowAsync<NotFoundException>();
        }
    }
}