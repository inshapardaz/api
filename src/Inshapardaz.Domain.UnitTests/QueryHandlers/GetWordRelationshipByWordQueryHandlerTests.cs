using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using Shouldly;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class GetWordRelationshipByWordQueryHandlerTests : DatabaseTest
    {
        private readonly GetRelationshipsByWordQueryHandler _handler;
        private readonly IList<WordRelation> _relationships = new List<WordRelation>();
        private readonly IList<Word> _words;

        public GetWordRelationshipByWordQueryHandlerTests()
        {
            var dictionary = Builder<Dictionary>
                .CreateNew()
                .Build();
            _words = Builder<Word>
                .CreateListOfSize(20)
                .Build();

            foreach (var word in _words)
            {
                dictionary.Word.Add(word);
            }

            _relationships.Add(new WordRelation{ SourceWord = _words[2], RelatedWord = _words[7],RelationType = RelationType.Compound});
            _relationships.Add(new WordRelation{ SourceWord = _words[2], RelatedWord = _words[10],RelationType = RelationType.Halat});
            _relationships.Add(new WordRelation{ SourceWord = _words[2], RelatedWord = _words[13],RelationType = RelationType.FormOfFeal});
            _relationships.Add(new WordRelation{ SourceWord = _words[5], RelatedWord = _words[6],RelationType = RelationType.JamaNadai });
            _relationships.Add(new WordRelation{ SourceWord = _words[2], RelatedWord = _words[12],RelationType = RelationType.WahidGhairNadai });
            
            DbContext.WordRelation.AddRange(_relationships);

            DbContext.Dictionary.Add(dictionary);
            DbContext.SaveChanges();

            _handler = new GetRelationshipsByWordQueryHandler(DbContext);
        }

        [Fact]
        public async Task WhenGettingTranslationById_ShouldReturnCorrectTranslation()
        {
            var relations = await _handler.ExecuteAsync(new GetRelationshipsByWordQuery(_words[2].Id));

            relations.ShouldNotBeNull();
            relations.ShouldBe(_words[2].WordRelationSourceWord);
        }

        [Fact]
        public async Task WhenGetTranslationForIncorrectId_ShouldReturnNull()
        {
            var relations = await _handler.ExecuteAsync(new GetRelationshipsByWordQuery(-232));

            relations.ShouldBeEmpty();
        }
    }
}