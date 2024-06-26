﻿using Inshapardaz.Api.Tests.Framework.Asserts;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.DeleteChapter
{
    [TestFixture]
    public class WhenDeletingChapterAsUnauthorized
         : TestBase
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapter = ChapterBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{chapter.BookId}/chapters/{chapter.ChapterNumber}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.ShouldBeUnauthorized();
        }
    }
}
