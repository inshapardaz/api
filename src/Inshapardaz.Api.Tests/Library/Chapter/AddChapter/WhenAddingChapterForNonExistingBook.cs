using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.AddChapter
{
    [TestFixture]
    public class WhenAddingChapterForNonExistingBook
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingChapterForNonExistingBook()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var chapter = new ChapterView { Title = Random.Name, ChapterNumber = 1 };

            _response = await Client.PostObject($"/library/{LibraryId}/books/{-Random.Number}/chapters", chapter);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
