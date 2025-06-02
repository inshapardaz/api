using System.Collections.Generic;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.GetIssueById
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenGettingIssueByIdHavingContentsWithWritePermissions
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueDto _expected;
        private IssueAssert _assert;
        private IEnumerable<TagDto> _tags;

        public WhenGettingIssueByIdHavingContentsWithWritePermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _tags = TagBuilder.WithLibrary(LibraryId)
                .Build(2);
            _expected = IssueBuilder.WithLibrary(LibraryId)
                .WithContent()
                .WithAuthors(2)
                .WithTags(_tags)
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
        public void ShouldHaveCorrectObjectReturned()
        {
            _assert.ShouldBeSameAs(_expected, tags: _tags);
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
        public void ShouldHaveEditLinks()
        {
            _assert.ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink()
                   .ShouldHaveCreatePageLink()
                   .ShouldHaveCreateArticlesLink()
                   .ShouldHaveCreateMultipleLink();
        }

        [Test]
        public void ShouldHaveCorrectContents()
        {
            _assert.ShouldHaveCorrectContentsLink();
        }
    }
}
