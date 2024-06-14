using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.AssignArticleToUser
{
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAssignArticleToUserForWriting
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleAssert _assert;
        private ArticleDto _article;
        private AccountDto _writer;

        public WhenAssignArticleToUserForWriting(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _writer = AccountBuilder.InLibrary(LibraryId).As(Role.Writer).Build();
            _article = ArticleBuilder.WithLibrary(LibraryId).WithContent().WithStatus(EditingStatus.Typing).Build();
            _response = await Client.PostObject($"/libraries/{LibraryId}/articles/{_article.Id}/assign", new { AccountId = _writer.Id, Type = "write" });
            _assert = ArticleAssert.FromResponse(_response, LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldAssignArticleToUser()
        {
           _assert
                .ShouldBeAssignedToUserForWriting(_writer)
                .ShouldNotBeAssignedForReviewing();
        }

        [Test]
        public void ShouldUpdateDatabaseWithAssignment()
        {
            _assert
                .ShouldBeSavedAssignmentForWriting(DatabaseConnection, _writer)
                .ShouldBeSavedNoAssignmentForReviewing(DatabaseConnection);
        }
    }
}
