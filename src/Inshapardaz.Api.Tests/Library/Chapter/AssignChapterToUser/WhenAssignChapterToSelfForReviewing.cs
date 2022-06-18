using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.UpdateChapter
{
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAssignChapterToSelfForReviewing
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterDto _chapter;

        public WhenAssignChapterToSelfForReviewing(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _chapter = ChapterBuilder.WithLibrary(LibraryId).WithContents().WithoutAnyAssignment().Build();
            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/assign", new { Type = "review" });
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldAssignChapterToUser()
        {
            ChapterAssert.FromResponse(_response, LibraryId)
                .ShouldNotBeAssignedForWriting()
                .ShouldBeAssignedToUserForReviewing(Account);
        }

        [Test]
        public void ShouldUpdateDatabaseWithAssignment()
        {
            ChapterAssert.FromResponse(_response, LibraryId)
                .ShouldBeSavedNoAssignmentForWriting(DatabaseConnection)
                .ShouldBeSavedAssignmentForReviewing(DatabaseConnection, Account);
        }
    }
}
