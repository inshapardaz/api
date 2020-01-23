using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.DeleteBook
{
    [TestFixture]
    public class WhenDeletingNonExistingBook : FunctionTest
    {
        private NoContentResult _response;


        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            
            var handler = Container.GetService<Functions.Library.Books.DeleteBook>();
            _response = (NoContentResult) await handler.Run(request, NullLogger.Instance, Random.Number, AuthenticationBuilder.WriterClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo(204));
        }
    }
}
