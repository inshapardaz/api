using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataBuilders;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Book.Contents.GetBookContent
{
    [TestFixture]
    public class WhenGettingBookContentForBookWithNoContents
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenGettingBookContentForBookWithNoContents()
            : base(Domain.Adapters.Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            _response = await Client.GetAsync($"/library/{LibraryId}/books/{book.Id}/content", Random.Locale, MimeTypes.Pdf);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeNotFound();
        }
    }
}
