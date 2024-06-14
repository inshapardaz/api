using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.BookPage.GetBookPages
{
    [TestFixture]
    public class WhenGettingBookPagesAssignedToMe : TestBase
    {
        private BookDto _book;
        private HttpResponseMessage _response;
        private PagingAssert<BookPageView> _assert;
        private AccountDto _account;

        public WhenGettingBookPagesAssignedToMe()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _account = AccountBuilder.Build();
            _book = BookBuilder.WithLibrary(LibraryId).WithPages(20)
                .AssignPagesToWriter(_account.Id, 7)
                .AssignPagesToWriter(AccountId, 11)
                .Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_book.Id}/pages?pageSize=10&pageNumber=1&assignmentFilter=assignedtome");

            _assert = new PagingAssert<BookPageView>(_response);
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
                new KeyValuePair<string, string>("assignmentFilter", "AssignedToMe"));
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
                new KeyValuePair<string, string>("assignmentFilter", "AssignedToMe"));
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturExpectedBookPages()
        {
            var expectedItems = BookBuilder.GetPages(_book.Id).Where(p => p.WriterAccountId == AccountId).OrderBy(p => p.SequenceNumber).Take(10);
            _assert.ShouldHaveTotalCount(11)
                   .ShouldHavePage(1)
                   .ShouldHavePageSize(10);

            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.SequenceNumber == item.SequenceNumber);
                actual.ShouldMatch(item)
                    .InLibrary(LibraryId)
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
