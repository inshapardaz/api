﻿using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.AssignPage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAssigningBookPageToSelf : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageAssert _assert;
        private BookPageDto _page;
        private BookPageDto _exptectedPage;

        public WhenAssigningBookPageToSelf(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3, true).Build();
            _page = BookBuilder.GetPages(book.Id).PickRandom();

            var assignment = new
            {
                Status = PageStatuses.AssignedToReview,
                AccountId = AccountId
            };

            _exptectedPage = new BookPageDto(_page)
            {
                Status = assignment.Status,
                AccountId = assignment.AccountId
            };

            _response = await Client.PostObject($"/library/{LibraryId}/books/{book.Id}/pages/{_page.SequenceNumber}/assign", assignment);
            _assert = BookPageAssert.FromResponse(_response, LibraryId);
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
