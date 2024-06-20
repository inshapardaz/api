using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Author.GetAuthors
{
    [TestFixture]
    public class WhenSearchingAuthorsInMiddle : TestBase
    {
        private HttpResponseMessage _response;
        private PagingAssert<AuthorView> _assert;

        public WhenSearchingAuthorsInMiddle()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            AuthorBuilder.WithLibrary(LibraryId).WithBooks(3).WithNamePattern("SearchAuthor").Build(50);

            _response = await Client.GetAsync($"/libraries/{LibraryId}/authors?query=SearchAuthor&pageNumber={3}&pageSize={10}");
            _assert = Services.GetService<PagingAssert<AuthorView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/authors",
                new KeyValuePair<string, string>("query", "SearchAuthor"));
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/authors", 4, 10,
                new KeyValuePair<string, string>("query", "SearchAuthor"));
        }

        [Test]
        public void ShouldHavePreviousLinks()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/authors", 2, 10,
                new KeyValuePair<string, string>("query", "SearchAuthor"));
        }

        [Test]
        public void ShouldReturnExpectedAuthors()
        {
            var expectedItems = AuthorBuilder.Authors.Where(a => a.Name.Contains("SearchAuthor")).OrderBy(a => a.Name).Skip(2 * 10).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.Id == item.Id);
                Services.GetService<AuthorAssert>().ForView(actual)
                    .ForLibrary(LibraryId)
                    .ShouldBeSameAs(item)
                    .WithBookCount(3)
                    .WithReadOnlyLinks()
                    .ShouldHavePublicImageLink();
            }
        }
    }
}
