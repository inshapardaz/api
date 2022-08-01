using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.AddPage
{
    [TestFixture]
    public class WhenAddingBookPageForNonExistingBook
        : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAddingBookPageForNonExistingBook()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var page = new BookPageView { Text = RandomData.Text, SequenceNumber = 1 };
            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{-RandomData.Number}/pages", page);
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
