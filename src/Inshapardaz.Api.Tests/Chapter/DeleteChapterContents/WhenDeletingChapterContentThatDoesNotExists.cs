﻿using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.DeleteChapterContents
{
    [TestFixture]
    public class WhenDeletingChapterContentThatDoesNotExists
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenDeletingChapterContentThatDoesNotExists()
            : base(Permission.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var _newContents = Random.Words(12);
            var chapter = ChapterBuilder.WithLibrary(LibraryId).WithoutContents().Build();

            _response = await Client.DeleteAsync($"/library/{LibraryId}/books/{chapter.BookId}/chapters/{chapter.Id}/contents", Random.Locale, Random.MimeType);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContentResult()
        {
            _response.ShouldBeNoContent();
        }
    }
}
