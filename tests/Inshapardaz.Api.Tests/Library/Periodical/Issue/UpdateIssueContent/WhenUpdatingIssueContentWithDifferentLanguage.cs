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
    [TestFixture]
    public class WhenUpdatingIssueContentWithDifferentLanguage
        : TestBase
    {
        private HttpResponseMessage _response;
        private string _newLanguage;
        private IssueDto _issue;
        private IssueContentDto _content;
        private byte[] _text;
        private IssueContentAssert _assert;

        public WhenUpdatingIssueContentWithDifferentLanguage()
            : base(Role.Writer)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _newLanguage = RandomData.Locale;

            _issue = IssueBuilder.WithLibrary(LibraryId).WithContents(2).WithContentLanguage($"{_newLanguage}_old").Build();
            _content = IssueBuilder.Contents.PickRandom();

            _text = new Faker().Image.Random.Bytes(50);

            _response = await Client.PutFile($"/libraries/{LibraryId}/periodicals/{_issue.PeriodicalId}/volumes/{_issue.VolumeNumber}/issues/{_issue.IssueNumber}/contents/{_content.Id}?language={_newLanguage}",_text, _content.MimeType);
            _assert = Services.GetService<IssueContentAssert>().ForResponse(_response).ForLibrary(Library);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveCreatedResult()
        {
            _response.ShouldBeCreated();
        }

        [Test]
        public void ShouldHaveCorrectLink()
        {
            _assert.ShouldHaveSelfLink()
                   .ShouldHavePeriodicalLink()
                   .ShouldHaveIssueLink()
                   .ShouldHaveUpdateLink()
                   .ShouldHaveDeleteLink();
        }

        [Test]
        public void ShouldHaveCorrectLanguage()
        {
            _assert.ShouldHaveCorrectLanguage(_newLanguage);
        }

        [Test]
        public void ShouldHaveCorrectMimeType()
        {
            _assert.ShouldHaveCorrectMimeType(_content.MimeType);
        }

        [Test]
        public void ShouldHaveCorrectContentSaved()
        {
            _assert.ShouldHaveCorrectContents(_text, newLanguage: _newLanguage);
        }
    }
}
