using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.UpdatePageSequenceNumber
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingPageSequenceNumber : TestBase
    {
        private HttpResponseMessage _response;
        private IssuePageAssert _assert;
        private IssuePageDto _page;
        private int _issueId, _newSequenceNumber;
        private string _text;

        public WhenUpdatingPageSequenceNumber(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithPages(5, true).Build();
            _page = IssueBuilder.GetPages(issue.Id).ElementAt(2);
            _text = RandomData.Text;

            var oldSequenceNumber = _page.SequenceNumber;
            _newSequenceNumber = 1;
            _page.SequenceNumber = _newSequenceNumber;
            
            _response = await Client.PostObject($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/pages/{oldSequenceNumber}/sequenceNumber", new { SequenceNumber = _page.SequenceNumber });
            
            
            _issueId = issue.Id;
            _assert = Services.GetService<IssuePageAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnOk()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveUpdatedThePageSequenceNumber()
        {
            var savedPage = IssuePageTestRepository.GetIssuePageByNumber(_issueId, _page.SequenceNumber);
            savedPage.Should().NotBeNull();
            savedPage.SequenceNumber.Should().Be(1);
        }
    }
}
