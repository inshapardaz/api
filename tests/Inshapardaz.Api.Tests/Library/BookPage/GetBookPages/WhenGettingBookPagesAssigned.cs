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

namespace Inshapardaz.Api.Tests.Library.BookPage.GetBookPages
{
    [TestFixture]
    public class WhenGettingBookPagesAssigned : TestBase
    {
        private BookDto _book;
        private HttpResponseMessage _response;
        private PagingAssert<BookPageView> _assert;

        public WhenGettingBookPagesAssigned()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var account = AccountBuilder.Build();
            _book = BookBuilder.WithLibrary(LibraryId).WithPages(20).AssignPagesToWriter(account.Id, 15).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_book.Id}/pages?pageSize=10&pageNumber=1&assignmentFilter=assigned");

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
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/books/{_book.Id}/pages",
                new KeyValuePair<string, string>("assignmentFilter", "Assigned"));
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/books/{_book.Id}/pages", 2, 10,
                new KeyValuePair<string, string>("assignmentFilter", "Assigned"));
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturExpectedBookPages()
        {
            var expectedItems = BookBuilder.GetPages(_book.Id).Where(p => p.WriterAccountId.HasValue).OrderBy(p => p.SequenceNumber).Take(10);

            _assert.ShouldHaveTotalCount(15)
                   .ShouldHavePage(1)
                   .ShouldHavePageSize(10);

            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.SequenceNumber == item.SequenceNumber);
                Services.GetService<BookPageAssert>().ForView(actual).ForLibrary(LibraryId)
                    .ShouldMatch(item)
                    .ShouldHaveSelfLink()
                    .ShouldHaveBookLink()
                    .ShouldNotHaveImageLink()
                    .ShouldNotHaveUpdateLink()
                    .ShouldNotHaveDeleteLink()
                    .ShouldNotHaveImageUpdateLink()
                    .ShouldNotHaveImageDeleteLink();
            }
        }
    }
}
