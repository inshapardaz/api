using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.GetAuthors
{
    [TestFixture]
    public class WhenGettingAuthorsPageInMiddle : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<AuthorView> _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            AuthorBuilder.WithLibrary(LibraryId).WithBooks(3).Build(50);

            _response = await Client.GetAsync($"/library/{LibraryId}/authors?pageNumber={3}&pageSize={10}");
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
            _assert.ShouldHaveSelfLink($"/library/{LibraryId}/authors");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/library/{LibraryId}/authors", 4, 10);
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/library/{LibraryId}/authors", 2, 10);
        }

        [Test]
        public void ShouldReturnExpectedAuthors()
        {
            var expectedItems = AuthorBuilder.Authors.OrderBy(a => a.Name).Skip(2 * 10).Take(10);
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
