using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.DeleteIssue
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingIssueWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueDto _expected;

        public WhenDeletingIssueWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expected = IssueBuilder.WithLibrary(LibraryId)
                .WithArticles(2)
                .WithPages()
                .WithContents(2)
                .Build();

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/periodicals/{_expected.PeriodicalId}/volumes/{_expected.VolumeNumber}/issues/{_expected.IssueNumber}");
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedIssue()
        {
            IssueAssert.ShouldHaveDeletedIssue(DatabaseConnection, _expected.Id);
        }

        [Test]
        public void ShouldHaveDeletedThePages()
        {
            IssueAssert.ShouldHaveDeletedPagesForIssue(DatabaseConnection, _expected.Id);
        }

        [Test]
        public void ShouldHaveDeletedTheArticles()
        {
            IssueAssert.ShouldHaveDeletedArticlesForIssue(DatabaseConnection, _expected.Id);
        }

        [Test]
        public void ShouldHaveDeletedTheImage()
        {
            IssueAssert.ShouldHaveDeletedIssueImage(DatabaseConnection, _expected.Id);
        }
    }
}
