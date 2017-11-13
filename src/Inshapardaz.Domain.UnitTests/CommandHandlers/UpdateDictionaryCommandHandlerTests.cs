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
    public class UpdateDictionaryCommandHandlerTests : DatabaseTest
    {
        private readonly UpdateDictionaryCommandHandler _handler;
        private readonly Dictionary _dictionary;

        public UpdateDictionaryCommandHandlerTests()
        {
            _dictionary = Builder<Dictionary>.CreateNew().Build();
            DbContext.Dictionary.Add(_dictionary);
            

            DbContext.SaveChanges();

            _handler = new UpdateDictionaryCommandHandler(DbContext);
        }

        [Test]
        public async Task WhenUpdatingDictionary_ShouldUpdateDictionaryFields()
        {
            var dictionary = Builder<Dictionary>
                .CreateNew()
                .With(d => d.Id = _dictionary.Id)
                .Build();

            await _handler.HandleAsync(new UpdateDictionaryCommand(dictionary));

            var createdDictionary = DbContext.Dictionary.Single(d => d.Id == dictionary.Id);

            createdDictionary.Name.ShouldBe(dictionary.Name);
            createdDictionary.Language.ShouldBe(dictionary.Language);
            createdDictionary.IsPublic.ShouldBe(dictionary.IsPublic);
        }


        [Test]
        public async Task WhenUpdatingNonExistingDictionary_ShouldThrowNotFound()
        {
            await _handler.HandleAsync(new UpdateDictionaryCommand(
                new Dictionary
                {
                    Id = 30203
                }
            )).ShouldThrowAsync<NotFoundException>();
        }
    }
}