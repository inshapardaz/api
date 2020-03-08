using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Tests.Helpers;
using Inshapardaz.Functions.Views.Library;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Library.Author.UpdateAuthor
{
    [TestFixture]
    public class WhenUpdatingAuthorThatDoesNotExist : LibraryTest<Functions.Library.Authors.UpdateAuthor>
    {
        private CreatedResult _response;
        private AuthorView _author;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = new AuthorView { Name = new Faker().Random.String() };

            _response = (CreatedResult)await handler.Run(_author, LibraryId, _author.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
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
            var returned = _response.Value as AuthorView;
            Assert.That(returned, Is.Not.Null);

            var actual = DatabaseConnection.GetAuthorById(returned.Id);
            Assert.That(actual, Is.Not.Null, "Author should be created.");
        }
    }
}
