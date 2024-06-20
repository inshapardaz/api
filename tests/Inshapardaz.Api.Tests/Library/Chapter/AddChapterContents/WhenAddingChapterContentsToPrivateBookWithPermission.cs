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
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAddingChapterContentsToPrivateBookWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private string _contents;
        private ChapterDto _chapter;
        private ChapterContentAssert _assert;

        public WhenAddingChapterContentsToPrivateBookWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var locale = RandomData.Locale;
            _chapter = ChapterBuilder.WithLibrary(LibraryId).Private().WithContentLanguage(locale).Build();
            _contents = RandomData.String;

            _response = await Client.PostString($"/libraries/{LibraryId}/books/{_chapter.BookId}/chapters/{_chapter.ChapterNumber}/contents?language={locale}", _contents);
            _assert = Services.GetService<ChapterContentAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
        public void ShouldSaveTheChapterContent()
        {
            _assert.ShouldHaveSavedChapterContent();
        }

        [Test]
        public void ShouldHaveTextReturened()
        {
            _assert.ShouldHaveText(_contents);
        }

        [Test]
        public void ShouldHaveCorrectTextSaved()
        {
            _assert.ShouldHaveSavedCorrectText(_contents);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHaveBookLink()
                   .ShouldHaveChapterLink()
                   .ShouldHaveContentLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink();
        }
    }
}
