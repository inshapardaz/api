using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.GetRecentReadBooks
{
    [TestFixture]
    public class WhenGettingRecentBooksAsUnauthorised : FunctionTest
    {
        private UnauthorizedResult _response;
        
        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var handler = Container.GetService<Functions.Library.Books.GetRecentReadBooks>();
            _response = (UnauthorizedResult) await handler.Run(request, NullLogger.Instance, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [Test]
        public void ShouldHaveUnauthorisedResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
