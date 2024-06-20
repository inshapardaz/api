using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
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
        private ChapterAssert _assert;
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
            var contents = ChapterTestRepository.GetContentByChapter(_expected.Id);

            foreach (var content in contents)
            {
                _filePaths.Add(FileTestRepository.GetFileById(content.FileId)?.FilePath);
            }

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{_expected.BookId}/chapters/{_expected.ChapterNumber}");
            _assert = Services.GetService<ChapterAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
            _assert.ShouldHaveDeletedChapter(_expected.Id);
        }

        [Test]
        public void ShouldHaveDeletedTheChapterContents()
        {
            _assert.ThatContentsAreDeletedForChapter(_expected.Id, _filePaths);
        }
    }
}
