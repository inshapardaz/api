﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.AssignPage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAssigningBookPageToSomeoneElse : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageAssert _assert;
        private BookPageDto _page;
        private BookPageDto _exptectedPage;
        private int _secondAccountId = RandomData.Number;

        public WhenAssigningBookPageToSomeoneElse(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _secondAccountId = AccountBuilder.Build().Id;
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3, true).WithStatus(EditingStatus.Typing, 3).Build();
            _page = BookBuilder.GetPages(book.Id).PickRandom();

            var assignment = new
            {
                Status = EditingStatus.Typed,
                AccountId = _secondAccountId
            };

            _exptectedPage = new BookPageDto(_page)
            {
                Status = assignment.Status,
                WriterAccountId = assignment.AccountId
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{book.Id}/pages/{_page.SequenceNumber}/assign", assignment);
            _assert = Services.GetService<BookPageAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveCorrectPageAssignment()
        {
            _assert.ShouldMatch(_exptectedPage);
        }

        [Test]
        public void ShouldHaveCorrectAssignmentTimeStamp()
        {
            _assert.ShouldHaveAssignedRecently();
        }
    }
}
