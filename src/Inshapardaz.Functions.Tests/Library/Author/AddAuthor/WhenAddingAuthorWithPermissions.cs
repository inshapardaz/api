using System.Net;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.AddAuthor
{
    [TestFixture(AuthenticationLevel.Administrator)]
    [TestFixture(AuthenticationLevel.Writer)]
    public class WhenAddingAuthorWithPermissions : LibraryTest<Functions.Library.Authors.AddAuthor>
    {
        private CreatedResult _response;
        private readonly ClaimsPrincipal _claim;

        public WhenAddingAuthorWithPermissions(AuthenticationLevel authenticationLevel)
        {
            _claim = AuthenticationBuilder.CreateClaim(authenticationLevel);
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var author = new AuthorView { Name = new Faker().Random.String() };

            _response = (CreatedResult)await handler.Run(author, LibraryId, _claim, CancellationToken.None);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            Assert.That(_response, Is.Not.Null);
            Assert.That(_response.StatusCode, Is.EqualTo((int)HttpStatusCode.Created));
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            Assert.That(_response.Location, Is.Not.Empty);
        }

        [Test]
        public void ShouldHaveCreatedTheAuthor()
        {
            var actual = _response.Value as AuthorView;
            Assert.That(actual, Is.Not.Null);

            var dbAuthor = DatabaseConnection.GetAuthorById(actual.Id);
            Assert.That(dbAuthor, Is.Not.Null, "Author should be created.");
        }
    }
}
