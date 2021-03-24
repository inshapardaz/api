using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Author.GetAuthors
{
    [TestFixture]
    public class WhenGettingAuthorsAsReader : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<AuthorView> _assert;

        public WhenGettingAuthorsAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            AuthorBuilder.WithLibrary(LibraryId).WithBooks(3).WithoutImage().Build(4);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/authors");
            _assert = new PagingAssert<AuthorView>(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/authors");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldNotHaveNavigationLinks()
        {
            _assert.ShouldNotHaveNextLink();
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturnExpectedAuthors()
        {
            var expectedItems = AuthorBuilder.Authors.OrderBy(a => a.Name).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                actual.ShouldMatch(item)
                            .InLibrary(LibraryId)
                            .WithBookCount(3)
                            .WithReadOnlyLinks()
                            .ShouldNotHaveImageLink();
            }
        }
    }
}
