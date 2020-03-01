using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.DeleteAuthor
{
    [TestFixture]
    public class WhenDeletingAuthorAsReader : LibraryTest
    {
        private AuthorsDataBuilder _builder;
        private ForbidResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _builder = Container.GetService<AuthorsDataBuilder>();
            var authors = _builder.WithLibrary(LibraryId).Build(4);
            var expected = authors.First();

            var handler = Container.GetService<Functions.Library.Authors.DeleteAuthor>();
            _response = (ForbidResult)await handler.Run(request, LibraryId, expected.Id, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            Assert.That(_response, Is.Not.Null);
        }
    }
}
