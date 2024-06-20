using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.DeletePage
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingIssuePageWithPermissions : TestBase
    {
        private HttpResponseMessage _response;
        private IssuePageAssert _assert;
        private IssuePageDto _page;
        private int _issueId;

        public WhenDeletingIssuePageWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithPages(3, true).Build();
            _page = IssueBuilder.GetPages(issue.Id).PickRandom();
            _issueId = issue.Id;
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/pages/{_page.SequenceNumber}");
            _assert = Services.GetService<IssuePageAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldDeletePage()
        {
            _assert.ShouldHaveNoIssuePage(_issueId, _page.Id, _page.ImageId);
        }
    }
}
