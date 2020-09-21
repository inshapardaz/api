using Bogus;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.AddChapter
{
    [TestFixture]
    public class WhenAddingChapterAsReader
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingChapterAsReader()
            : base(Domain.Adapters.Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();

            var chapter = new ChapterView { Title = new Faker().Random.String(), ChapterNumber = 1 };

            _response = await Client.PostObject($"/library/{LibraryId}/books/{book.Id}/chapters", chapter);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.ShouldBeForbidden();
        }
    }
}
