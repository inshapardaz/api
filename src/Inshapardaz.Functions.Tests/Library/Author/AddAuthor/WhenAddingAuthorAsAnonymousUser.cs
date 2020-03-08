using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.AddAuthor
{
    [TestFixture]
    public class WhenAddingAuthorAsAnonymousUser : LibraryTest<Functions.Library.Authors.AddAuthor>
    {
        private UnauthorizedResult _response;

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = new AuthorView { Name = new Faker().Random.String() };

            _response = (UnauthorizedResult)await handler.Run(author, LibraryId, AuthenticationBuilder.Unauthorized, CancellationToken.None);
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
        }
    }
}
