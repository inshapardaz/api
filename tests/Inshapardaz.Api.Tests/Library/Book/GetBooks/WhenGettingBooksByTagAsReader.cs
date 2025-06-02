using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.GetBooks
{
    [TestFixture]
    public class WhenGettingBooksByTagAsReader
        : TestBase

    {
        private HttpResponseMessage _response;
        private PagingAssert<BookView> _assert;
        private TagDto _tag1, _tag2;
        private IEnumerable<BookDto> _tagBooks2;

        public WhenGettingBooksByTagAsReader() : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _tag1 = TagBuilder.WithLibrary(LibraryId).Build();
            _tag2 = TagBuilder.WithLibrary(LibraryId).Build();
            BookBuilder.WithLibrary(LibraryId).WithTag(_tag1).IsPublic().Build(30);
            _tagBooks2 = BookBuilder.WithLibrary(LibraryId).WithTag(_tag2).IsPublic().Build(15);
            CategoryBuilder.WithLibrary(LibraryId).WithBooks(3).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books?pageNumber=2&pageSize=5&tagId={_tag2.Id}");

            _assert = Services.GetService<PagingAssert<BookView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books",
                new KeyValuePair<string, string>("tagId", _tag2.Id.ToString()));
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/books", 3, 5, 
                new KeyValuePair<string, string>("tagId", _tag2.Id.ToString()));
        }

        [Test]
        public void ShouldHavePreviousLink()
        {
            _assert.ShouldHavePreviousLink($"/libraries/{LibraryId}/books", 1, 5,
                new KeyValuePair<string, string>("tagId", _tag2.Id.ToString()));
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldReturnCorrectPage()
        {
            _assert.ShouldHavePage(2)
                   .ShouldHavePageSize(5)
                   .ShouldHaveTotalCount(_tagBooks2.Count())
                   .ShouldHaveItems(5);
        }

        [Test]
        public void ShouldReturnExpectedBooks()
        {
            var expectedItems = _tagBooks2.OrderBy(a => a.Title).Skip(5).Take(5).ToArray();
            for (int i = 0; i < _assert.Data.Count(); i++)
            {
                var actual = _assert.Data.ElementAt(i);
                var expected = expectedItems[i];
                Services.GetService<BookAssert>().ForView(actual).ForLibrary(LibraryId)
                            .ShouldBeSameAs(expected)
                            .ShouldHaveCorrectLinks()
                            .ShouldNotHaveEditLinks()
                            .ShouldNotHaveImageUpdateLink()
                            .ShouldNotHaveCreateChaptersLink()
                            .ShouldNotHaveAddContentLink()
                            .ShouldHaveChaptersLink()
                            .ShouldHavePublicImageLink();
            }
        }
    }
}
