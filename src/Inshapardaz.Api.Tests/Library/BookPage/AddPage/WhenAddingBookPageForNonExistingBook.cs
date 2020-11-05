using Inshapardaz.Api.Tests;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.AddPage
{
    [TestFixture]
    public class WhenAddingBookPageForNonExistingBook
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingBookPageForNonExistingBook()
            : base(Domain.Adapters.Permission.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var page = new BookPageView { Text = Random.Text, PageNumber = 1 };
            _response = await Client.PostObject($"/library/{LibraryId}/books/{-Random.Number}/pages", page);
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
