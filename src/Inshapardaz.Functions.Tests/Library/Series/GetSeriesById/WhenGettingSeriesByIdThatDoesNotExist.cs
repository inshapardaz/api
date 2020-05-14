using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Asserts;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Series.GetSeriesById
{
    [TestFixture]
    public class WhenGettingSeriesByIdThatDoesNotExist : LibraryTest<Functions.Library.Series.GetSeriesById>
    {
        private NotFoundResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

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
            _response.ShouldBeNotFound();
        }
    }
}
