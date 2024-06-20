using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.IssuePage.GetIssuePages
{
    [TestFixture]
    public class WhenGettingAllIssuePages : TestBase
    {
        private IssueDto _issue;
        private HttpResponseMessage _response;
        private PagingAssert<IssuePageView> _assert;

        public WhenGettingAllIssuePages()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).WithPages(20)
                .WithStatus(EditingStatus.Typing, 7)
                .WithStatus(EditingStatus.Available, 3)
                .WithStatus(EditingStatus.Typed, 5)
                .WithStatus(EditingStatus.InReview, 3)
                .WithStatus(EditingStatus.Completed, 2)
                .Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages?pageSize=10&pageNumber=1&status=all");

            _assert = Services.GetService<PagingAssert<IssuePageView>>().ForResponse(_response);
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
        public void ShouldHaveSelfLink()
        {
            _assert.ShouldHaveSelfLink($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _assert.ShouldNotHaveCreateLink();
        }

        [Test]
        public void ShouldHaveNextLink()
        {
            _assert.ShouldHaveNextLink($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/pages", 2, 10);
        }

        [Test]
        public void ShouldNotHavePreviousLink()
        {
            _assert.ShouldNotHavePreviousLink();
        }

        [Test]
        public void ShouldReturExpectedBookPages()
        {
            var expectedItems = IssueBuilder.GetPages(_issue.Id)
                                            .OrderBy(p => p.SequenceNumber).Take(10);
            _assert.ShouldHaveTotalCount(20)
                   .ShouldHavePage(1)
                   .ShouldHavePageSize(10);

            foreach (var item in expectedItems)
            {
                var actual = _assert.Data.FirstOrDefault(x => x.SequenceNumber == item.SequenceNumber);
                Services.GetService<IssuePageAssert>().ForResponse(_response).ForLibrary(LibraryId)
                        .ShouldMatch(item)
                        .ShouldHaveSelfLink()
                        .ShouldHavePeriodicalLink()
                        .ShouldHaveIssueLink()
                        .ShouldNotHaveImageLink()
                        .ShouldNotHaveUpdateLink()
                        .ShouldNotHaveDeleteLink()
                        .ShouldNotHaveImageUpdateLink()
                        .ShouldNotHaveImageDeleteLink();
            }
        }
    }
}
