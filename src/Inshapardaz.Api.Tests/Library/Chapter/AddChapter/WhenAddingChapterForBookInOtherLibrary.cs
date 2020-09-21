using Inshapardaz.Api.Tests;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataBuilders;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Chapter.AddChapter
{
    [TestFixture]
    public class WhenAddingChapterForBookInOtherLibrary
        : TestBase
    {
        private HttpResponseMessage _response;
        private LibraryDataBuilder _libBuilder;

        public WhenAddingChapterForBookInOtherLibrary()
            : base(Domain.Adapters.Permission.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _libBuilder = Services.GetService<LibraryDataBuilder>();

            var library2 = _libBuilder.Build();
            var book = BookBuilder.WithLibrary(library2.Id).Build();

            var chapter = new ChapterView { Title = Random.Name, ChapterNumber = 1, BookId = book.Id };

            _response = await Client.PostObject($"/library/{LibraryId}/books/{book.Id}/chapters", chapter);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _libBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResult()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
