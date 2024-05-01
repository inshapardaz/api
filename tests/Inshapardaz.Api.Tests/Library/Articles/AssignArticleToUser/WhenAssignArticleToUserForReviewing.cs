using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Articles.AssignArticleToUser
{
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAssignArticleToUserForReviewing
        : TestBase
    {
        private HttpResponseMessage _response;
        private ArticleAssert _assert;
        private ArticleDto _article;
        private AccountDto _reviewer;

        public WhenAssignArticleToUserForReviewing(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _reviewer = AccountBuilder.InLibrary(LibraryId).As(Role.Writer).Build();
            _article = ArticleBuilder.WithLibrary(LibraryId).WithContent().WithStatus(EditingStatus.InReview).Build();
            _response = await Client.PostObject($"/libraries/{LibraryId}/articles/ {_article.Id}/assign", new { AccountId = _reviewer.Id, Type = "review" });
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
                .ShouldNotBeAssignedForWriting()
                .ShouldBeAssignedToUserForReviewing(_reviewer);
        }

        [Test]
        public void ShouldUpdateDatabaseWithAssignment()
        {
            _assert
                .ShouldBeSavedNoAssignmentForWriting(DatabaseConnection)
                .ShouldBeSavedAssignmentForReviewing(DatabaseConnection, _reviewer);
        }
    }
}
