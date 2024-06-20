using Inshapardaz.Api.Extensions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.AddChapter
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAddingChapterWithPermission
        : TestBase
    {
        private ChapterView _chapter;
        private HttpResponseMessage _response;
        private ChapterAssert _assert;

        public WhenAddingChapterWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var writer = AccountBuilder.InLibrary(LibraryId).Build();
            var reviewer = AccountBuilder.InLibrary(LibraryId).Build();
            var book = BookBuilder.WithLibrary(LibraryId).Build();

            _chapter = new ChapterView {
                Title = RandomData.Name,
                ChapterNumber = 1,
                BookId = book.Id, Status =
                EditingStatus.Typing.ToDescription(),
                WriterAccountId = writer.Id,
                WriterAssignTimeStamp = RandomData.Date,
                ReviewerAccountId = reviewer.Id,
                ReviewerAssignTimeStamp = RandomData.Date
        };

            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{book.Id}/chapters", _chapter);

            _assert = Services.GetService<ChapterAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _assert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldSaveTheChapter()
        {
            _assert.ShouldHaveSavedChapter();
        }

        [Test]
        public void ShouldHaveCorrectObjectRetured()
        {
            _assert.ShouldMatch(_chapter);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHaveBookLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink()
                   .ShouldNotHaveContentsLink()
                   .ShouldHaveAddChapterContentLink();
        }
    }
}
