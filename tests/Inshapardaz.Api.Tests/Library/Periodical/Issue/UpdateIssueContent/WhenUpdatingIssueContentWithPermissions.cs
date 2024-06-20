using Bogus;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.UpdateIssueContent
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingIssueContentWithPermissions
        : TestBase
    {
        private HttpResponseMessage _response;

        private IssueDto _issue;
        private IssueContentDto _content;
        private byte[] _expected;
        private IssueContentAssert _assert;

        public WhenUpdatingIssueContentWithPermissions(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _issue = IssueBuilder.WithLibrary(LibraryId).WithContents(2).Build();
            _content = IssueBuilder.Contents.PickRandom();

            _expected = new Faker().Image.Random.Bytes(50);
            _response = await Client.PutFile($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/contents/ {_content.Id} ?language={_content.Language}", _expected, _content.MimeType);
            _assert = Services.GetService<IssueContentAssert>().ForResponse(_response).ForLibrary(Library);
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
        public void ShouldHaveUpdatedFileContents()
        {
            _assert.ShouldHaveIssueContent(_expected);
        }
    }
}
