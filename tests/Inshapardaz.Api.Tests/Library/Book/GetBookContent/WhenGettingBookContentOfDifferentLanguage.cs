﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.GetBookContent
{
    [TestFixture]
    public class WhenGettingBookContentOfDifferentLanguage
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenGettingBookContentOfDifferentLanguage()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var _book = BookBuilder.WithLibrary(LibraryId).WithContents(5).Build();
            var _expected = BookBuilder.Contents.PickRandom();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{_book.Id}/content/{-RandomData.Number}?language={_expected.Language + "a"}", _expected.MimeType);
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
