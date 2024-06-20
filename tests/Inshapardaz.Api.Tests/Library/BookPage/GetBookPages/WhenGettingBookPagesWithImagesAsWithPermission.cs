using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookPage.GetBookPages
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingBookPagesWithImagesAsWithPermission : TestBase
    {
        private BookDto _book;
        private HttpResponseMessage _response;
        private PagingAssert<BookPageView> _assert;

        public WhenGettingBookPagesWithImagesAsWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _book = BookBuilder.WithLibrary(LibraryId).WithPages(9, true).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_book.Id}/pages");

            _assert = Services.GetService<PagingAssert<BookPageView>>().ForResponse(_response);
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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books/{_book.Id}/pages");
        }

        [Test]
        public void ShouldHaveCreateLink()
        {
            _assert.ShouldHaveCreateLink($"/libraries/{LibraryId}/books/{_book.Id}/pages");
        }

        [Test]
        public void ShouldNotHaveNavigationLinks()
        {
            _assert.ShouldNotHaveNextLink();
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturExpectedBookPages()
        {
            var expectedItems = BookBuilder.GetPages(_book.Id).OrderBy(p => p.SequenceNumber).Take(10);
            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.SequenceNumber == item.SequenceNumber);
                Services.GetService<BookPageAssert>().ForView(actual).ForLibrary(LibraryId)
                    .ShouldMatch(item)
                    .ShouldHaveSelfLink()
                    .ShouldHaveBookLink()
                    .ShouldHaveImageLink(item.ImageId.Value)
                    .ShouldHaveUpdateLink()
                    .ShouldHaveDeleteLink()
                    .ShouldHaveImageUpdateLink()
                    .ShouldHaveImageDeleteLink();
            }
        }
    }
}
