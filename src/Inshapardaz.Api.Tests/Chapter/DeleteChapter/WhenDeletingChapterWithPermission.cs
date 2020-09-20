using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Domain.Adapters;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.DeleteChapter
{
    [TestFixture(Permission.Admin)]
    [TestFixture(Permission.LibraryAdmin)]
    [TestFixture(Permission.Writer)]
    public class WhenDeletingChapterWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterDto _expected;

        public WhenDeletingChapterWithPermission(Permission permission)
            : base(permission)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expected = ChapterBuilder.WithLibrary(LibraryId).WithContents().Build();

            _response = await Client.DeleteAsync($"/library/{LibraryId}/books/{_expected.BookId}/chapters/{_expected.Id}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedChapter()
        {
            ChapterAssert.ShouldHaveDeletedChapter(_expected.Id, DatabaseConnection);
        }

        [Test]
        public void ShouldHaveDeletedTheChapterContents()
        {
            ChapterAssert.ThatFilesAreDeletedForChapter(_expected.Id, DatabaseConnection);
        }
    }
}
