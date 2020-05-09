using System.Threading;
using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.AddAuthor
{
    [TestFixture]
    public class WhenAddingAuthorAsReader : LibraryTest<Functions.Library.Authors.AddAuthor>
    {
        private ForbidResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = new AuthorView { Name = Random.Name };

            _response = (ForbidResult)await handler.Run(author, LibraryId, AuthenticationBuilder.ReaderClaim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveForbiddenResult()
        {
            _response.Should().NotBeNull();
        }
    }
}
