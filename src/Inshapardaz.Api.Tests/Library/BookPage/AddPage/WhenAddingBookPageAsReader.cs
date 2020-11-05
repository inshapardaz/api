using Bogus;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.AddPage
{
    [TestFixture]
    public class WhenAddingBookPageAsReader
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingBookPageAsReader()
            : base(Domain.Adapters.Permission.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();

            var page = new BookPageView { Text = new Faker().Random.String(), PageNumber = 1 };
            _response = await Client.PostObject($"/library/{LibraryId}/books/{book.Id}/pages", page);
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
