using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.GetAuthors
{
    [TestFixture]
    public class WhenSearchingAuthorsLastPage : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<AuthorView> _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            AuthorBuilder.WithLibrary(LibraryId).WithBooks(3).WithNamePattern("SearchAuthor").Build(50);

            _response = await Client.GetAsync($"/library/{LibraryId}/authors?query=SearchAuthor&pageNumber={5}&pageSize={10}");
            _assert = new PagingAssert<AuthorView>(_response, Library);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
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
            _assert.ShouldHaveSelfLink($"/library/{LibraryId}/authors", "query", "SearchAuthor");
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
            _assert.ShouldHavePreviousLink($"/library/{LibraryId}/authors", 4, 10, "query", "SearchAuthor");
        }

        [Test]
        public void ShouldReturnExpectedAuthors()
        {
            var expectedItems = AuthorBuilder.Authors.Where(a => a.Name.Contains("SearchAuthor")).OrderBy(a => a.Name).Skip(4 * 10).Take(10);

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
