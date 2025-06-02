using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Inshapardaz.Api.Extensions;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.UpdateIssue
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenUpdatingIssueWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueView _newIssue;
        private IssueAssert _assert;
        private TagView[] _newTags;

        public WhenUpdatingIssueWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            var issue = IssueBuilder.WithLibrary(LibraryId).WithTags(2).Build();
            
            _newTags = new []
            {
                new TagView() { Name = RandomData.Name }, 
                new TagView() { Name = RandomData.Name }
            };
            _newIssue = new IssueView
            {
                IssueDate = RandomData.Date, 
                VolumeNumber = issue.VolumeNumber, 
                IssueNumber = issue.IssueNumber, 
                Status = RandomData.StatusType.ToDescription(),
                Tags = _newTags,
            };

            _response = await Client.PutObject($"/libraries/{LibraryId}/periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}", _newIssue);
            _assert = Services.GetService<IssueAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldHaveOKResult()
        {
            _response.ShouldBeOk();
        }

        [Test]
        public void ShouldHaveReturnedUpdatedIssue()
        {
            _assert.ShouldMatch(_newIssue);
        }

        [Test]
        public void ShouldHaveUpdatedIssue()
        {
            _assert.ShouldHaveSavedIssue();
        }
        
        
        [Test]
        public void ShouldReturnCorrectTags()
        {
            _assert.ShouldBeSameTags(_newTags);
        }
    }
}
