using System.Linq;
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
    public class WhenDeletingAuthorAsReader : FunctionTest
    {
        ForbidResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            var builder = Container.GetService<AuthorsDataBuilder>();
            var authors = builder.WithAuthors(4).Build();
            var expected = authors.First();
            
            var handler = Container.GetService<Functions.Library.Authors.DeleteAuthor>();
            _response = (ForbidResult) await handler.Run(request, NullLogger.Instance, expected.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
