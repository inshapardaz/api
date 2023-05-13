using FluentAssertions;
using Inshapardaz.Api.Tests.Asserts;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.GetIssuesByPeriodical
{
    [TestFixture]
    public class WhenGettingIssuessByPeriodicalAsReader : TestBase
    {
        private PeriodicalDto _periodical;
        private IEnumerable<IssueDto> _issues;
        private HttpResponseMessage _response;
        private PageView<IssueView> _view;

        public WhenGettingIssuessByPeriodicalAsReader()
            : base(Role.Reader)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _periodical = PeriodicalBuilder.WithLibrary(LibraryId).Build();
            _issues = IssueBuilder.WithLibrary(LibraryId).WithPeriodical(_periodical.Id)
                        .WithArticles(RandomData.NumberBetween(1, 3))
                        .WithPages(RandomData.NumberBetween(1, 5)).Build(5);

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
            _view.CreateLink().Should().BeNull();
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
                var assert = new IssueAssert(actual).InLibrary(LibraryId);
                var pages = IssueBuilder.GetPages(expected.Id);
                var articles = IssueBuilder.GetArticles(expected.Id);
                assert.ShouldBeSameAs(DatabaseConnection, expected, articles.Count(), pages.Count())
                   .ShouldHaveSelfLink()
                   .ShouldHavePeriodicalLink()
                   .ShouldHaveArticlesLink()
                   .ShouldHavePagesLink()
                   .ShouldNotHaveUpdateLink()
                   .ShouldNotHaveDeleteLink()
                   .ShouldNotHaveCreateArticleLink()
                   .ShouldNotHaveCreatePageLink()
                   .ShouldNotHaveAddContentLink();
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
