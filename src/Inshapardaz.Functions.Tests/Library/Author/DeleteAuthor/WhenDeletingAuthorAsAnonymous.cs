using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.DeleteAuthor
{
    [TestFixture]
    public class WhenDeletingAuthorAsAnonymous : FunctionTest
    {
        private UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            var builder = Container.GetService<AuthorsDataBuilder>();
            var authors = builder.Build(4);
            var expected = authors.First();
            
            var handler = Container.GetService<Functions.Library.Authors.DeleteAuthor>();
            _response = (UnauthorizedResult) await handler.Run(request, NullLogger.Instance, expected.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.Unauthorized));
        }
    }
}
