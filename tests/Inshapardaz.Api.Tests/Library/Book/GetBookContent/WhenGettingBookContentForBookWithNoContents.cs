using Inshapardaz.Api.Tests.Asserts;
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
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();
            _response = await Client.GetAsync($"/libraries/{LibraryId}/books/{book.Id}/content/{-RandomData.Number}?language={RandomData.Locale}", MimeTypes.Pdf);
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
