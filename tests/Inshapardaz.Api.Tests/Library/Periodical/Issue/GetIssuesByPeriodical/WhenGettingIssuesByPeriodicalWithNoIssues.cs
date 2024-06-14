using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.GetIssuesByPeriodical
{
    [TestFixture]
    public class WhenGettingIssuesByPeriodicalWithNoIssues
        : TestBase
    {
        private PeriodicalDto _periodical;
        private HttpResponseMessage _response;
        private PageView<IssueView> _view;

        public WhenGettingIssuesByPeriodicalWithNoIssues()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _periodical = PeriodicalBuilder.WithLibrary(LibraryId).Build();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_periodical.Id}/issues");
            _view = _response.GetContent<PageView<IssueView>>().Result;
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
            _view.SelfLink()
                .ShouldBeGet()
                .EndingWith($"libraries/{LibraryId}/periodicals/{_periodical.Id}/issues");
        }

        [Test]
        public void ShouldNotHaveCreateLink()
        {
            _view.CreateLink()
                .ShouldBePost()
                .EndingWith($"libraries/{LibraryId}/periodicals/{_periodical.Id}/issues");
        }

        [Test]
        public void ShouldHaveNoIssues()
        {
            _view.Data.Should().BeEmpty();
            _view.PageCount.Should().Be(1);
            _view.CurrentPageIndex.Should().Be(1);
        }
    }
}
