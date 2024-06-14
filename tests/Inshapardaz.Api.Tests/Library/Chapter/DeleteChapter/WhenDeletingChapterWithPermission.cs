using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.DeleteChapter
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingChapterWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterDto _expected;

        private List<string> _filePaths = new();

        public WhenDeletingChapterWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expected = ChapterBuilder.WithLibrary(LibraryId).WithContents().WithPages().Build();
            var contents = DatabaseConnection.GetContentByChapter(_expected.Id);

            foreach (var content in contents)
            {
                _filePaths.Add(DatabaseConnection.GetFileById(content.FileId)?.FilePath);
            }

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{_expected.BookId}/chapters/{_expected.ChapterNumber}");
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
            ChapterAssert.ThatContentsAreDeletedForChapter(_expected.Id, _filePaths, DatabaseConnection, FileStore);
        }
    }
}
