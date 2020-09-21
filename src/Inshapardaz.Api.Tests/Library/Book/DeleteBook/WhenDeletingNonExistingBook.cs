using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Book.DeleteBook
{
    [TestFixture]
    public class WhenDeletingNonExistingBook : TestBase
    {
        private HttpResponseMessage _response;

        public WhenDeletingNonExistingBook() : base(Domain.Adapters.Permission.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _response = await Client.DeleteAsync($"/library/{LibraryId}/books/{-Random.Number}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }
    }
}
