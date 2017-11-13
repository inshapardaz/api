using System.Collections.Generic;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using NUnit.Framework;
using Shouldly;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    [TestFixture]
    public class GetWordRelationshipByIdQueryHandlerTests : DatabaseTest
    {
        private readonly GetRelationshipByIdQueryHandler _handler;
        private readonly IList<WordRelation> _relationships = new List<WordRelation>();

        public GetWordRelationshipByIdQueryHandlerTests()
        {
            var dictionary = Builder<Dictionary>
                .CreateNew()
                .Build();
            var words = Builder<Word>
                .CreateListOfSize(20)
                .Build();

            foreach (var word in words)
            {
                dictionary.Word.Add(word);
            }

            _relationships.Add(new WordRelation{ SourceWord = words[2], RelatedWord = words[7],RelationType = RelationType.Compound});
            _relationships.Add(new WordRelation{ SourceWord = words[2], RelatedWord = words[10],RelationType = RelationType.Halat});
            _relationships.Add(new WordRelation{ SourceWord = words[2], RelatedWord = words[13],RelationType = RelationType.FormOfFeal});
            _relationships.Add(new WordRelation{ SourceWord = words[5], RelatedWord = words[6],RelationType = RelationType.JamaNadai });
            _relationships.Add(new WordRelation{ SourceWord = words[2], RelatedWord = words[12],RelationType = RelationType.WahidGhairNadai });
            
            DbContext.WordRelation.AddRange(_relationships);

            DbContext.Dictionary.Add(dictionary);
            DbContext.SaveChanges();

            _handler = new GetRelationshipByIdQueryHandler(DbContext);
        }

        [Test]
        public async Task WhenGettingRelationshipById_ShouldReturnCorrectRelation()
        {
            var translation = await _handler.ExecuteAsync(new GetRelationshipByIdQuery(_relationships[2].Id));

            translation.ShouldNotBeNull();
            translation.ShouldBe(_relationships[2]);
        }

        [Test]
        public async Task WhenGetRealtionshipForIncorrectId_ShouldReturnNull()
        {
            var translation = await _handler.ExecuteAsync(new GetRelationshipByIdQuery(-232));

            translation.ShouldBeNull();
        }
    }
}