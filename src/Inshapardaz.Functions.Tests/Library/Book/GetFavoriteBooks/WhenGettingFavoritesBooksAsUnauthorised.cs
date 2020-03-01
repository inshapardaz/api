using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Book.GetFavoriteBooks
{
    [TestFixture]
    public class WhenGettingFavoritesBooksAsUnauthorised : LibraryTest
    {
        private UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();

            var handler = Container.GetService<Functions.Library.Books.GetFavoriteBooks>();
            _response = (UnauthorizedResult)await handler.Run(request, LibraryId, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [Test]
        public void ShouldHaveUnauthorisedResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
