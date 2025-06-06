using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.GetIssuesByPeriodical
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingIssuesByPeriodicalWithWritePermissions
        : TestBase
    {
        private HttpResponseMessage _response;
        private PageView<IssueView> _view;
        private IEnumerable<IssueDto> _issues;
        private PeriodicalDto _periodical;

        public WhenGettingIssuesByPeriodicalWithWritePermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _periodical = PeriodicalBuilder.WithLibrary(LibraryId).Build();
            _issues = IssueBuilder.WithLibrary(LibraryId).WithPeriodical(_periodical.Id).WithTags(2).WithContent().Build(5);

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
        public void ShouldHaveCreateLink()
        {
            _view.CreateLink()
                .ShouldBePost()
                .EndingWith($"libraries/{LibraryId}/periodicals/{_periodical.Id}/issues");
        }

        [Test]
        public void ShouldHaveCorrectNumberOfIssues()
        {
            Assert.That(_view.Data.Count(), Is.EqualTo(_issues.Count()));
        }

        [Test]
        public void ShouldHaveCorrectIssuesData()
        {
            foreach (var expected in _issues)
            {
                var actual = _view.Data.FirstOrDefault(x => x.Id == expected.Id);
                var expectedTags = IssueBuilder.IssueTags[expected.Id];
                var pages = IssueBuilder.GetPages(expected.Id);
                var assert = Services.GetService<IssueAssert>().ForView(actual).ForLibrary(LibraryId)
                    .ShouldBeSameAs(expected, tags: expectedTags.AsEnumerable())
                    .ShouldHaveUpdateLink()
                    .ShouldHaveDeleteLink()
                    .ShouldHaveCreateArticlesLink()
                    .ShouldHaveCreatePageLink()
                    .ShouldHaveAddContentLink()
                    .ShouldHaveCreateMultipleLink()
                    .ShouldHaveCorrectContents();
            }
        }


        [Test]
        public void ShouldHaveCorrectPaginationData()
        {
            _view.PageCount.Should().Be(1);
            _view.PageSize.Should().Be(10);
            _view.CurrentPageIndex.Should().Be(1);
        }
    }
}
