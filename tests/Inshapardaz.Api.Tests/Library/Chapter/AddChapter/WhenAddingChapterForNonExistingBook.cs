using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Chapter.AddChapter
{
    [TestFixture]
    public class WhenAddingChapterForNonExistingBook() : TestBase(Role.Writer)
    {
        private HttpResponseMessage _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapter = new ChapterView { Title = RandomData.Name, ChapterNumber = 1 };

            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{-RandomData.Number}/chapters", chapter);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveBadRequestResult() => _response.ShouldBeBadRequest();
    }
}
