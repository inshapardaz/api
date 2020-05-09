using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Functions.Tests.DataBuilders;
using Inshapardaz.Functions.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.DeleteAuthor
{
    [TestFixture]
    public class WhenDeletingAuthorAsAnonymous : LibraryTest<Functions.Library.Authors.DeleteAuthor>
    {
        private AuthorsDataBuilder _builder;
        private UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var request = TestHelpers.CreateGetRequest();
            _builder = Container.GetService<AuthorsDataBuilder>();
            var authors = _builder.WithLibrary(LibraryId).Build(4);
            var expected = authors.First();

            _response = (UnauthorizedResult)await handler.Run(request, LibraryId, expected.Id, AuthenticationBuilder.Unauthorized, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            _builder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveUnauthorizedResult()
        {
            _response.Should().NotBeNull();
            _response.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
        }
    }
}
