﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.DeletePageImage
{
    [TestFixture]
    public class WhenDeletingBookPageImageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private BookPageDto _page;
        private int _bookId;

        public WhenDeletingBookPageImageWhenNoExistingImage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).WithPages(3).Build();
            _page = BookBuilder.GetPages(book.Id).PickRandom();
            _bookId = book.Id;
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{_bookId}/pages/{_page.SequenceNumber}/image");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResponse()
        {
            _response.ShouldBeOk();
        }
    }
}
