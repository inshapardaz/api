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
    [TestFixture]
    public class WhenDeletingIssuePageWhenNoExistingImage : TestBase
    {
        private HttpResponseMessage _response;
        private IssuePageAssert _assert;
        private IssuePageDto _page;
        private int _issueId;

        public WhenDeletingIssuePageWhenNoExistingImage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithPages(3, true).Build();
            _page = IssueBuilder.GetPages(issue.Id).PickRandom();
            _issueId = issue.Id;
            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/pages/{-RandomData.Number}");
            _assert = Services.GetService<IssuePageAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            BookBuilder.CleanUp();
            Cleanup();
        }

        [Test]
        public void ShouldHaveOkResponse()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldDeletePage()
        {
            _assert.IssuePageShouldExist(_issueId, _page.SequenceNumber);
        }
    }
}
