using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Categories.DeleteCategory
{
    [TestFixture]
    public class WhenDeletingNonExistingCategory : FunctionTest
    {
        private NoContentResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var handler = Container.GetService<Functions.Library.Categories.DeleteCategory>();
            _response = (NoContentResult) await handler.Run(null, NullLogger.Instance, Random.Number, AuthenticationBuilder.AdminClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveNoContentResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.NoContent));
        }
    }
}
