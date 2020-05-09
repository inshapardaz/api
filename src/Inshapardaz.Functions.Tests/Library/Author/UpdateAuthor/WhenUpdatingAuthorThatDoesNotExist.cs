using System.Threading;
using System.Threading.Tasks;
using Bogus;
using Inshapardaz.Functions.Tests.Asserts;
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
        private AuthorAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _author = new AuthorView { Name = new Faker().Random.String() };

            _response = (CreatedResult)await handler.Run(_author, LibraryId, _author.Id, AuthenticationBuilder.AdminClaim, CancellationToken.None);
            _assert = AuthorAssert.WithResponse(_response).InLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveLocationHeader()
        {
            _assert.ShouldHaveCorrectLocationHeader();
        }

        [Test]
        public void ShouldHaveCreatedTheAuthor()
        {
            _assert.ShouldHaveSavedAuthor(DatabaseConnection);
        }
    }
}
