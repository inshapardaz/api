using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.GetAuthors
{
    [TestFixture]
    public class WhenSearchingAuthorsLastPage : LibraryTest<Functions.Library.Authors.GetAuthors>
    {
        private AuthorsDataBuilder _builder;
        private OkObjectResult _response;
        private PagingAssert<AuthorView> _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _builder = Container.GetService<AuthorsDataBuilder>();
            _builder.WithLibrary(LibraryId).WithBooks(3).WithNamePattern("SearchAuthor").Build(50);

            var request = new RequestBuilder()
               .WithQueryParameter("query", "SearchAuthor")
               .WithQueryParameter("pageNumber", 5)
               .WithQueryParameter("pageSize", 10)
               .Build();

            _response = (OkObjectResult)await handler.Run(request, LibraryId, AuthenticationBuilder.ReaderClaim, CancellationToken.None);

            _assert = new PagingAssert<AuthorView>(_response);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink($"/api/library/{LibraryId}/authors", "query", "SearchAuthor");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldNotHaveNextLink()
        {
            _assert.ShouldNotHaveNextLink();
        }

        [Test]
        public void ShouldHavePreviousLinks()
        {
            _assert.ShouldHavePreviousLink($"/api/library/{LibraryId}/authors", 4, 10, "query", "SearchAuthor");
        }

        [Test]
        public void ShouldReturnExpectedAuthors()
        {
            var expectedItems = _builder.Authors.Where(a => a.Name.Contains("SearchAuthor")).OrderBy(a => a.Name).Skip(4 * 10).Take(10);

            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item)
                      .InLibrary(LibraryId)
                      .WithBookCount(3)
                      .WithReadOnlyLinks()
                      .ShouldHavePublicImageLink();
            }
        }
    }
}
