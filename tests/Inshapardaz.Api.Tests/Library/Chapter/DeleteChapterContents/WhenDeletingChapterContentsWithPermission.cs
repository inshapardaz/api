using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.DeleteChapterContents
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingChapterContentsWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterContentAssert _assert;
        private ChapterContentDto _content;

        public WhenDeletingChapterContentsWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapter = ChapterBuilder.WithLibrary(LibraryId).WithContents().Build();
            _content = ChapterBuilder.Contents.Single(x => x.ChapterId == chapter.Id);

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/books/{chapter.BookId}/chapters/{chapter.ChapterNumber}/contents?language={_content.Language}");
            _assert = Services.GetService<ChapterContentAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNoContentResult()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedChapterContent()
        {
            _assert.ShouldHaveDeletedContent(_content);
        }
    }
}
