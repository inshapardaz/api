using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FizzWare.NBuilder;
using Inshapardaz.Domain.Database.Entities;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using Shouldly;
using Xunit;

namespace Inshapardaz.Domain.UnitTests.QueryHandlers
{
    public class GetWordsPagesQueryHandlerTests : DatabaseTest
    {
        private readonly GetWordsPagesQueryHandler _handler;
        private readonly Dictionary _dictionary;
        private readonly IList<Word> _words;

        public GetWordsPagesQueryHandlerTests()
        {
            _dictionary = Builder<Dictionary>
                .CreateNew()
                .Build();

            _words = Builder<Word>
                .CreateListOfSize(10)
                .Build().OrderBy(w => w.Title).ToList();

            foreach (var word in _words)
            {
                DbContext.Word.Add(word);
                _dictionary.Word.Add(word);
            }

            DbContext.Dictionary.Add(_dictionary);
            DbContext.SaveChanges();
            _handler = new GetWordsPagesQueryHandler(DbContext);
        }

        [Fact]
        public async Task WhenGettingFirstPageOfWords_ShouldReturnCorrectPage()
        {
            var page = await _handler.ExecuteAsync(new GetWordPageQuery(_dictionary.Id, 1, 3));

            page.ShouldNotBeNull();
            page.PageNumber.ShouldBe(1);
            page.TotalCount.ShouldBe(4);
            page.PageSize.ShouldBe(3);
            page.Data.ShouldNotBeEmpty();
            page.Data.Count().ShouldBe(3);
            page.Data.ElementAt(0).ShouldBe(_words[0]);
            page.Data.ElementAt(1).ShouldBe(_words[1]);
            page.Data.ElementAt(2).ShouldBe(_words[2]);
        }

        [Fact]
        public async Task WhenGettingSecondPageOfWords_ShouldReturnCorrectPage()
        {
            var page = await _handler.ExecuteAsync(new GetWordPageQuery(_dictionary.Id, 2, 3));

            page.ShouldNotBeNull();
            page.PageNumber.ShouldBe(2);
            page.TotalCount.ShouldBe(4);
            page.PageSize.ShouldBe(3);
            page.Data.ShouldNotBeEmpty();
            page.Data.Count().ShouldBe(3);
            page.Data.ElementAt(0).ShouldBe(_words[3]);
            page.Data.ElementAt(1).ShouldBe(_words[4]);
            page.Data.ElementAt(2).ShouldBe(_words[5]);
        }

        [Fact]
        public async Task WhenGettingLastPartialPageOfWords_ShouldReturnCorrectPage()
        {
            var page = await _handler.ExecuteAsync(new GetWordPageQuery(_dictionary.Id, 4, 3));

            page.ShouldNotBeNull();
            page.PageNumber.ShouldBe(4);
            page.TotalCount.ShouldBe(4);
            page.PageSize.ShouldBe(3);
            page.Data.ShouldNotBeEmpty();
            page.Data.Count().ShouldBe(1);
            page.Data.ElementAt(0).ShouldBe(_words[9]);
        }

        [Fact]
        public async Task WhenGettingNonExistantPageOfWords_ShouldReturnEmptyCollection()
        {
            var page = await _handler.ExecuteAsync(new GetWordPageQuery(_dictionary.Id, 20, 10));

            page.ShouldNotBeNull();
            page.PageNumber.ShouldBe(20);
            page.TotalCount.ShouldBe(1);
            page.PageSize.ShouldBe(10);
            page.Data.ShouldBeEmpty();
        }
        [Fact]
        public async Task WhenGettingWordPageFromNonExistantDictionary_ShouldReturnEmptyCollection()
        {
            var page = await _handler.ExecuteAsync(new GetWordPageQuery(-1, 1, 4));
            
            page.ShouldNotBeNull();
            page.PageNumber.ShouldBe(1);
            page.TotalCount.ShouldBe(0);
            page.PageSize.ShouldBe(4);
            page.Data.ShouldBeEmpty();
        }
    }
}