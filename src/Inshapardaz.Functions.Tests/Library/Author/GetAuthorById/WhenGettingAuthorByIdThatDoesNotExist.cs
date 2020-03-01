using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.GetAuthorById
{
    [TestFixture]
    public class WhenGettingAuthorByIdThatDoesNotExist : LibraryTest
    {
        private NotFoundResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var handler = Container.GetService<Functions.Library.Authors.GetAuthorById>();
            _response = (NotFoundResult)await handler.Run(request, LibraryId, Random.Number, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNotFoundResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.NotFound));
        }
    }
}
