using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.GetIssueById
{
    [TestFixture]
    public class WhenGettingIssueByIdAsAnonymous
        : TestBase
    {
        private HttpResponseMessage _response;

        private IssueDto _expected;

        private IssueAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expected = IssueBuilder.WithLibrary(LibraryId)
                                    .WithPages(10)
                                    .WithArticles(3)
                                    .WithContents(2)
                                    .Build(4)
                                    .First();

            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_expected.PeriodicalId}/volumes/{_expected.VolumeNumber}/issues/{_expected.IssueNumber}");
            _assert = Services.GetService<IssueAssert>().ForResponse(_response).ForLibrary(LibraryId);
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
        public void ShouldHaveCorrectObjectRetured()
        {
            _assert.ShouldBeSameAs(_expected, 3, 10);
        }

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHavePeriodicalLink()
                   .ShouldHaveArticlesLink()
                   .ShouldHavePagesLink();
        }

        [Test]
        public void ShouldNotHaveEditLinks()
        {
            _assert.ShouldNotHaveUpdateLink()
                   .ShouldNotHaveDeleteLink()
                   .ShouldNotHaveCreateArticleLink()
                   .ShouldNotHaveCreatePageLink();
        }

        [Test]
        public void ShouldHaveNoContentsLink()
        {
            _assert.ShouldNotHaveContentsLink();
        }
    }
}
