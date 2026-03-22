using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Chapter.AssignChapterToUser
{
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAssignChapterToSelfForWriting(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private ChapterAssert _assert;
        private ChapterDto _chapter;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _chapter = ChapterBuilder.WithLibrary(LibraryId).WithContents().WithStatus(EditingStatus.Typing).WithoutAnyAssignment().Build();
            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/assign", new { Type = "write" });
            _assert = Services.GetService<ChapterAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldAssignChapterToUser()
        {
            _assert
                .ShouldBeAssignedToUserForWriting(Account)
                .ShouldNotBeAssignedForReviewing();
        }

        [Test]
        public void ShouldUpdateDatabaseWithAssignment()
        {
            _assert
                .ShouldBeSavedAssignmentForWriting(Account)
                .ShouldBeSavedNoAssignmentForReviewing();
        }
    }
}
