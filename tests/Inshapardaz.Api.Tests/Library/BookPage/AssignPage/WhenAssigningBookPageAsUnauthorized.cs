﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.AssignPage
{
    [TestFixture]
    public class WhenAssigningBookPageAsUnauthorized : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageDto _page;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3).Build();
            _page = BookBuilder.GetPages(book.Id).PickRandom();
            var assignment = new
            {
                Status = EditingStatus.InReview,
                AccountId = AccountId
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{book.Id}/pages/{_page.SequenceNumber}/assign", assignment);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorisedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
