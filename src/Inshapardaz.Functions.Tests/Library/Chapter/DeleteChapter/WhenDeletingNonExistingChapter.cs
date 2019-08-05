using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Chapter.DeleteChapter
{
    [TestFixture]
    public class WhenDeletingNonExistingChapter : FunctionTest
    {
        NoContentResult _response;


        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var handler = Container.GetService<Functions.Library.Books.Chapters.DeleteChapter>();
            _response = (NoContentResult) await handler.Run(request, Random.Number, Random.Number, AuthenticationBuilder.WriterClaim, NullLogger.Instance, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(204));
        }
    }
}
