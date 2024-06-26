﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.GetChapterById
{
    [TestFixture]
    public class WhenGettingChapterByIdThatDoesNotExist
        : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var expected = ChapterBuilder.WithLibrary(LibraryId).Build();
            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{expected.BookId}/chapters/{-RandomData.Number}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNotFoundResult()
        {
            _response.ShouldBeNotFound();
        }
    }
}
