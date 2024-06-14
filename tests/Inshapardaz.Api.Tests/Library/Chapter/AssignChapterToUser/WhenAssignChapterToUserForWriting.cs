using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.UpdateChapter
{
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAssignChapterToUserForWriting
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterDto _chapter;
        private AccountDto _writer;

        public WhenAssignChapterToUserForWriting(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _writer = AccountBuilder.InLibrary(LibraryId).As(Role.Writer).Build();
            _chapter = ChapterBuilder.WithLibrary(LibraryId).WithContents().WithStatus(EditingStatus.Typing).WithoutAnyAssignment().Build();
            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/assign", new { AccountId = _writer.Id, Type = "write" });
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
                .ShouldBeAssignedToUserForWriting(_writer)
                .ShouldNotBeAssignedForReviewing();
        }

        [Test]
        public void ShouldUpdateDatabaseWithAssignment()
        {
            ChapterAssert.FromResponse(_response, LibraryId)
                .ShouldBeSavedAssignmentForWriting(DatabaseConnection, _writer)
                .ShouldBeSavedNoAssignmentForReviewing(DatabaseConnection);
        }
    }
}
