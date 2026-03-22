using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.GetIssueById
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingIssueByIdWithoutContentsAndWithWritePermissions(Role role) : TestBase(role)
    {
        private HttpResponseMessage _response;
        private IssueDto _expected;
        private IssueAssert _assert;

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expected = IssueBuilder.WithLibrary(LibraryId).WithContent().Build(4).First();
            _response = await Client.GetAsync($"/libraries/{LibraryId}/periodicals/{_expected.PeriodicalId}/volumes/{_expected.VolumeNumber}/issues/{_expected.IssueNumber}");
            _assert = Services.GetService<IssueAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown() => Cleanup();

        [Test]
        public void ShouldHaveOkResult() => _response.ShouldBeOk();

        [Test]
        public void ShouldHaveCorrectObjectReturned() => _assert.ShouldBeSameAs(_expected);

        [Test]
        public void ShouldHaveLinks()
        {
            _assert.ShouldHaveSelfLink()
                  .ShouldHavePeriodicalLink()
                  .ShouldHaveArticlesLink()
                  .ShouldHavePagesLink();
        }

        [Test]
        public void ShouldHaveEditLinks()
        {
            _assert.ShouldHaveUpdateLink()
                  .ShouldHaveDeleteLink()
                  .ShouldHaveCreatePageLink()
                  .ShouldHaveCreateArticlesLink();
        }
    }
}
