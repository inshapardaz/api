using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.UpdateChapter
{
    [TestFixture]
    public class WhenAssignChapterToReader
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterDto _chapter;

        public WhenAssignChapterToReader()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var reader = AccountBuilder.InLibrary(LibraryId).As(Role.Reader).Build();
            _chapter = ChapterBuilder.WithLibrary(LibraryId).WithContents().WithoutAnyAssignment().Build();
            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/assign", new { AccountId = reader.Id, Type = "write" });
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
