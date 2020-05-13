using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.GetBookById
{
    [TestFixture]
    public class WhenGettingBookByIdThatDoesNotExist : LibraryTest<Functions.Library.Books.GetBookById>
    {
        private NotFoundResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            _response = (NotFoundResult)await handler.Run(request, LibraryId, new Faker().Random.Int(), AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNotFoundResult()
        {
            _response.ShouldBeNotFound();
        }
    }
}
