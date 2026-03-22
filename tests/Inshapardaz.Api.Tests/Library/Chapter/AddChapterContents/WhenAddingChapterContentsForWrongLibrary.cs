using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Chapter.Contents.AddChapterContents
{
    [TestFixture]
    public class WhenAddingChapterContentsForWrongLibrary() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            var chapter = ChapterBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.PostString($"/libraries/{-RandomData.Number}/books/{book.Id}/chapters/{chapter.Id}/contents?language={RandomData.Locale}", RandomData.String);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveForbiddenResult() => _response.ShouldBeForbidden();
    }
}
