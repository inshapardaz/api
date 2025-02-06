using Bogus;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.AddIssue
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenAddingIssueWithPermission
        : TestBase
    {
        private IssueView _issue;
        private HttpResponseMessage _response;
        private IssueAssert _assert;

        public WhenAddingIssueWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var periodical = PeriodicalBuilder.WithLibrary(LibraryId).Build();
            
            _issue = new IssueView
            {
                VolumeNumber = new Faker().Random.Number(1, 100),
                IssueNumber = new Faker().Random.Number(1, 100),
                IssueDate = new Faker().Date.Past()
            }; 

            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{periodical.Id}/issues", _issue);

            _assert = Services.GetService<IssueAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
        public void ShouldSaveTheIssue()
        {
            _assert.ShouldHaveSavedIssue();
        }

        [Test]
        public void ShouldHaveCorrectObjectReturned()
        {
            _assert.ShouldMatch(_issue)
                .WithStatus(EditingStatus.Available);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHavePeriodicalLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink()
                   .ShouldHaveArticlesLink()
                   .ShouldHavePagesLink()
                   .ShouldHaveCreateArticlesLink()
                   .ShouldHaveCreatePageLink()
                   .ShouldHaveImageUpdateLink();
        }
    }
}
