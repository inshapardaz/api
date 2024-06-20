using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.AddChapterContents
{
    [TestFixture]
    public class WhenAddingChapterContentsWithoutLanguage
        : TestBase
    {
        private HttpResponseMessage _response;
        private ChapterDto _chapter;
        private ChapterContentAssert _assert;

        public WhenAddingChapterContentsWithoutLanguage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _chapter = ChapterBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.PostString($"/libraries/{LibraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/contents", RandomData.String);
            _assert = Services.GetService<ChapterContentAssert>().ForResponse(_response).ForLibrary(Library);
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
        public void ShouldHaveUsedLibraryLanguageForContent()
        {
            _assert.ShouldHaveDefaultLibraryLanguage();
        }
    }
}
