using System.Collections.Generic;
using Inshapardaz.Api.Tests.Framework.Asserts;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.Helpers;

namespace Inshapardaz.Api.Tests.Library.Periodical.Issue.DeleteIssue
{
    [TestFixture(Role.Admin)]
    [TestFixture(Role.LibraryAdmin)]
    [TestFixture(Role.Writer)]
    public class WhenDeletingIssueWithPermission
        : TestBase
    {
        private HttpResponseMessage _response;
        private IssueAssert _assert;
        private IssueDto _expected;
        private IEnumerable<TagDto> _expectedTags;

        public WhenDeletingIssueWithPermission(Role role)
            : base(role)
        {
        }

        [OneTimeSetUp]
        public async Task Setup()
        {
            _expected = IssueBuilder.WithLibrary(LibraryId)
                .WithArticles(2)
                .WithTags(2)
                .WithPages()
                .WithContents(2)
                .Build();

            _expectedTags = TagTestRepository.GetTagsByIssue(_expected.Id);

            _response = await Client.DeleteAsync($"/libraries/{LibraryId}/periodicals/{_expected.PeriodicalId}/volumes/{_expected.VolumeNumber}/issues/{_expected.IssueNumber}");
            _assert = Services.GetService<IssueAssert>().ForResponse(_response).ForLibrary(LibraryId);
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            Cleanup();
        }

        [Test]
        public void ShouldReturnNoContent()
        {
            _response.ShouldBeNoContent();
        }

        [Test]
        public void ShouldHaveDeletedIssue()
        {
            _assert.ShouldHaveDeletedIssue(_expected.Id);
        }

        [Test]
        public void ShouldHaveDeletedThePages()
        {
            _assert.ShouldHaveDeletedPagesForIssue(_expected.Id);
        }

        [Test]
        public void ShouldHaveDeletedTheArticles()
        {
            _assert.ShouldHaveDeletedArticlesForIssue(_expected.Id);
        }

        [Test]
        public void ShouldHaveDeletedTheImage()
        {
            _assert.ShouldHaveDeletedIssueImage(_expected.Id);
        }


        [Test]
        public void ShouldHaveDeletedContents()
        {
            var issues = IssueBuilder.Issues.Where(x => x.Id == _expected.Id).ToList();
            var contents = IssueBuilder.Contents.Where(x => x.IssueId == _expected.Id).ToList();

            foreach (var content in contents)
            {
                var file = IssueBuilder.Files.FirstOrDefault(x => x.Id == content.FileId);
                var assert = Services.GetService<IssueContentAssert>().ForLibrary(Library)
                    .ShouldHaveDeletedContent(content, file);
            }
        }

        [Test]
        public void ShouldHaveDeletedArticles()
        {
            _assert.ShouldHaveDeletedArticlesForIssue(_expected.Id);
        }
        
        
        [Test]
        public void ShouldHaveDeletedArticleTags()
        {
            var tags = TagTestRepository.GetTagsByIssue(_expected.Id);
            tags.Should().BeNullOrEmpty();
        }
        
        [Test]
        public void ShouldNotHaveDeletedTheTags()
        {
            var tags = TagTestRepository.GetTags(_expectedTags.Select(x => x.Id).ToArray());
            var tagAssert = Services.GetService<TagAssert>().ForLibrary(LibraryId);
            tags.ForEach(cat => tagAssert.ShouldNotHaveDeletedTag(cat.Id));
        }
    }
}
