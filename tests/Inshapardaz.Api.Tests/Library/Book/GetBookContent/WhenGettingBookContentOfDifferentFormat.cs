﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.GetBookContent
{
    [TestFixture]
    public class WhenGettingBookContentOfDifferentFormat
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenGettingBookContentOfDifferentFormat()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var _book = BookBuilder.WithLibrary(LibraryId).WithContents(5).Build();
            var _expected = BookBuilder.Contents.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_book.Id}/content/{-RandomData.Number}?language={RandomData.Locale}", _expected.MimeType + "2");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNotFound()
        {
            _response.ShouldBeNotFound();
        }
    }
}
