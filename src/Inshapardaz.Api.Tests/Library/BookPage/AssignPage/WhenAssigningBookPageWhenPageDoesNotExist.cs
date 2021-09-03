using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.BookPage.AssignPage
{
    [TestFixture]
    public class WhenAssigningBookPageWhenPageDoesNotExist : TestBase
    {
        private HttpResponseMessage _response;

        public WhenAssigningBookPageWhenPageDoesNotExist()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var book = BookBuilder.WithLibrary(LibraryId).Build();

            var assignment = new
            {
                Status = PageStatuses.Typing,
                AccountId = AccountId
            };

            _response = await Client.PostObject($"/libraries/{LibraryId}/books/{book.Id}/pages/{-RandomData.Number}/assign", assignment);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveBadRequestResponse()
        {
            _response.ShouldBeBadRequest();
        }
    }
}
