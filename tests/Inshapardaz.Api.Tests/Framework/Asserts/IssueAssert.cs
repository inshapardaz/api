using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Api.Extensions;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class IssueAssert(
        IIssueTestRepository issueRepository,
        IIssueArticleTestRepository articleRepository,
        IIssuePageTestRepository pageRepository,
        FakeFileStorage fileStorage,
        IAuthorTestRepository authorRepository,
        ITagTestRepository tagsRepository)
    {
        private HttpResponseMessage _response;
        private IssueView _view;
        private int _libraryId;

        private readonly IAuthorTestRepository _authorRepository = authorRepository;

        public IssueAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _view = response.GetContent<IssueView>().Result;
            return this;
        }

        public IssueAssert ForView(IssueView view)
        {
            _view = view;
            return this;
        }

        public IssueAssert ForLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public IssueAssert ShouldHaveSelfLink()
        {
            _view.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.PeriodicalId}/volumes/{_view.VolumeNumber}/issues/{_view.IssueNumber}");

            return this;
        }

        public IssueAssert ShouldHavePeriodicalLink()
        {
            _view.Link("periodical")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.PeriodicalId}");

            return this;
        }

        public IssueAssert ShouldHaveCorrectImageLocationHeader(int issueId)
        {
            _response.Headers.Location.AbsoluteUri.Should().NotBeEmpty();
            return this;
        }

        public IssueAssert ShouldHaveArticlesLink()
        {
            _view.Link("articles")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.PeriodicalId}/volumes/{_view.VolumeNumber}/issues/{_view.IssueNumber}/articles");

            return this;
        }

        public IssueAssert ShouldHaveCreateArticlesLink()
        {
            _view.Link("create-article")
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.PeriodicalId}/volumes/{_view.VolumeNumber}/issues/{_view.IssueNumber}/articles");

            return this;
        }

        public IssueAssert ShouldNotHaveCreateArticleLink()
        {
            _view.Link("create-article").Should().BeNull();

            return this;
        }

        public IssueAssert ShouldHaveAddContentLink()
        {
            _view.Link("add-content")
                 .ShouldBePost()
                 .EndingWith($"libraries/{_libraryId}/periodicals/{_view.PeriodicalId}/volumes/{_view.VolumeNumber}/issues/{_view.IssueNumber}/contents");

            return this;
        }

        public IssueAssert ShouldNotHaveAddContentLink()
        {
            _view.Link("add-content").Should().BeNull();

            return this;
        }

        public IssueAssert ShouldHavePagesLink()
        {
            _view.Link("pages")
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.PeriodicalId}/volumes/{_view.VolumeNumber}/issues/{_view.IssueNumber}/pages");

            return this;
        }

        public IssueAssert ShouldHaveCreatePageLink()
        {
            _view.Link("add-pages")
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.PeriodicalId}/volumes/{_view.VolumeNumber}/issues/{_view.IssueNumber}/pages");

            return this;
        }

        public IssueAssert ShouldHaveCreateMultipleLink()
        {
            _view.Link("create-multiple")
                  .ShouldBePost()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.PeriodicalId}/volumes/{_view.VolumeNumber}/issues/{_view.IssueNumber}/pages/upload");

            return this;
        }

        public IssueAssert ShouldNotHaveCreatePageLink()
        {
            _view.Link("add-pages").Should().BeNull();

            return this;
        }

        public IssueAssert ShouldHaveUpdateLink()
        {
            _view.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.PeriodicalId}/volumes/{_view.VolumeNumber}/issues/{_view.IssueNumber}");

            return this;
        }

        public IssueAssert ShouldNotHaveUpdateLink()
        {
            _view.UpdateLink().Should().BeNull();

            return this;
        }

        public IssueAssert ShouldHaveDeleteLink()
        {
            _view.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_view.PeriodicalId}/volumes/{_view.VolumeNumber}/issues/{_view.IssueNumber}");

            return this;
        }

        public IssueAssert ShouldNotHaveDeleteLink()
        {
            _view.DeleteLink().Should().BeNull();

            return this;
        }

        public IssueAssert ShouldHaveImageLink()
        {
            _view.Link("image").ShouldBeGet();
            return this;
        }

        public IssueAssert ShouldHaveImageUpdateLink()
        {
            _view.Link("image-upload")
                   .ShouldBePut()
                   .EndingWith($"libraries/{_libraryId}/periodicals/{_view.PeriodicalId}/volumes/{_view.VolumeNumber}/issues/{_view.IssueNumber}/image");
            return this;
        }

        public IssueAssert ShouldNotHaveImageUpdateLink()
        {
            _view.Link("image-upload").Should().BeNull();

            return this;
        }

        public IssueAssert ShouldNotHaveEditLinks()
        {
            return ShouldNotHaveUpdateLink()
                .ShouldNotHaveDeleteLink()
                .ShouldNotHaveImageUpdateLink()
                .ShouldNotHaveCreatePageLink();
        }

        public IssueAssert ShouldNotHaveContentsLink()
        {
            _view.Link("contents").Should().BeNull();

            return this;
        }

        public IssueAssert ShouldHaveCorrectContentsLink()
        {
            var contents = issueRepository.GetIssueContents(_view.Id);

            contents.Should().HaveSameCount(_view.Contents);

            foreach (var content in contents)
            {
                ShouldHaveContentLink(content);
            }

            return this;
        }

        public IssueAssert ShouldHaveCorrectContents()
        {
            var contents = issueRepository.GetIssueContents(_view.Id);

            contents.Should().HaveSameCount(_view.Contents);

            foreach (var expected in contents)
            {
                var actual = _view.Contents.Single(c => c.Id == expected.Id);
                actual.Language.Should().Be(expected.Language);
                actual.MimeType.Should().Be(expected.MimeType);
                actual.SelfLink().ShouldBeGet()
                    .ShouldHaveAcceptLanguage(expected.Language)
                    .ShouldHaveAccept(expected.MimeType);
            }

            return this;
        }


        public IssueAssert ShouldHaveContentLink(IssueContentDto content)
        {
            var actual = _view.Contents.Single(x => x.Id == content.Id);
            actual.SelfLink()
                  .ShouldBeGet()
                  .ShouldHaveAcceptLanguage(content.Language);
            return this;
        }


        public IssueAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location;
            location.Should().NotBeNull();
            location.AbsoluteUri.Should().EndWith($"libraries/{_libraryId}/periodicals/{_view.PeriodicalId}/volumes/{_view.VolumeNumber}/issues/{_view.IssueNumber}");
            return this;
        }

        public IssueAssert ShouldHaveImageLocationHeader()
        {
            _response.Headers.Location.AbsoluteUri.Should().NotBeEmpty();
            return this;
        }

        public IssueAssert ShouldHaveSavedIssue()
        {
            var dbIssue = issueRepository.GetIssueById(_view.Id);
            dbIssue.Should().NotBeNull();
            _view.VolumeNumber.Should().Be(dbIssue.VolumeNumber);
            _view.IssueNumber.Should().Be(dbIssue.IssueNumber);
            _view.IssueDate.Should().BeCloseTo(dbIssue.IssueDate, TimeSpan.FromSeconds(2));

            if (_view?.Tags is not null && _view.Tags.Any())
            {
                var tags = tagsRepository.GetTagsByIssue(_view.Id);
                _view.Tags.Select(x => x.Name).ToList()
                    .Should().BeEquivalentTo(tags.Select(x => x.Name).ToList());
            }

            return this;
        }

        public IssueAssert ShouldHaveDeletedIssue(int issueId)
        {
            var dbIssue = issueRepository.GetIssueById(issueId);
            dbIssue.Should().BeNull();
            return this;
        }

        public IssueAssert ShouldHaveDeletedIssueImage(int issueId)
        {
            var issueImage = issueRepository.GetIssueImage(issueId);
            issueImage.Should().BeNull();
            return this;
        }

        public IssueAssert ShouldHaveDeletedArticlesForIssue(int issueId)
        {
            var articles = articleRepository.GetIssueArticlesByIssue(issueId);
            articles.Should().BeNullOrEmpty();
            return this;
        }

        public IssueAssert ShouldHaveDeletedPagesForIssue(int issueId)
        {
            var pages = pageRepository.GetIssuePagesByIssue(issueId);
            pages.Should().BeNullOrEmpty();
            return this;
        }

        public IssueAssert ShouldMatch(IssueView expected, int? articleCount = null, int? pageCount = null)
        {
            _view.Should().NotBeNull();
            _view.VolumeNumber.Should().Be(expected.VolumeNumber);
            _view.IssueNumber.Should().Be(expected.IssueNumber);
            _view.IssueDate.Should().BeCloseTo(expected.IssueDate, TimeSpan.FromSeconds(2));
            if (articleCount.HasValue)
            {
                _view.ArticleCount.Should().Be(articleCount);
            }

            if (pageCount.HasValue)
            {
                _view.PageCount.Should().Be(pageCount);
            }

            if (expected.Status is not null)
            {
                _view.Status.Should().Be(expected.Status.ToDescription());
            }

            if (expected.Tags is not null && expected.Tags.Any())
            {
                var tags = tagsRepository.GetTagsByIssue(_view.Id);
                _view.Tags.Should().HaveSameCount(tags);
                foreach (var tag in tags)
                {
                    var actual = expected.Tags.SingleOrDefault(a => a.Name == tag.Name);
                    actual.Name.Should().Be(tag.Name);
                }
            }
            

            return this;
        }

        public IssueAssert WithStatus(StatusType status)
        {
            _view.Status.Should().Be(status.ToDescription());
            return this;
        }

        public IssueAssert ShouldBeSameAs(IssueDto expected, int? articleCount = null, int? pageCount = null, IEnumerable<TagDto> tags = null)
        {
            _view.Should().NotBeNull();
            _view.VolumeNumber.Should().Be(expected.VolumeNumber);
            _view.IssueNumber.Should().Be(expected.IssueNumber);
            _view.IssueDate.Should().BeCloseTo(expected.IssueDate, TimeSpan.FromSeconds(2));
            if (articleCount.HasValue)
            {
                _view.ArticleCount.Should().Be(articleCount);
            }

            if (pageCount.HasValue)
            {
                _view.PageCount.Should().Be(pageCount);
            }

            _view.Status.Should().Be(expected.Status.ToDescription());
            
            if (tags is not null && tags.Any())
            {
                _view.Tags.Should().HaveSameCount(tags);
                _view.Tags.Select(c => c.Name).ToList()
                    .Should().BeEquivalentTo(tags.Select(c => c.Name).ToList());
            }
            else
            {
                _view.Tags.Should().BeNullOrEmpty();
            }

            return this;
        }

        public IssueAssert ShouldNotHaveUpdatedIssueImage(int issueId, byte[] oldImage)
        {
            var imageUrl = issueRepository.GetIssueImageUrl(issueId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().Equal(oldImage);
            return this;
        }

        public IssueAssert ShouldHaveAddedIssueImage(int issueId)
        {
            var imageUrl = issueRepository.GetIssueImageUrl(issueId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty();
            return this;
        }

        public IssueAssert ShouldHaveUpdatedIssueImage(IssueDto issue, byte[] newImage)
        {
            var imageUrl = issueRepository.GetIssueImageUrl(issue.Id);
            imageUrl.Should().NotBeNull();
            imageUrl.Should().EndWith($"periodicals/{issue.PeriodicalId}/volumes/{issue.VolumeNumber}/issues/{issue.IssueNumber}/files/issue-image.jpg");
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
            return this;
        }

        public IssueAssert ShouldHaveChecksum(byte[] contents)
        {
            var file = _response.GetContent<FileView>().Result;
            file.Checksum.Should().Be(ChecksumTestHelper.Compute(contents));
            return this;
        }

        public IssueAssert ShouldHavePublicImage(int issueId)
        {
            var image = issueRepository.GetIssueImage(issueId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
            return this;
        }

        public IssueAssert ShouldBeSameTags(IEnumerable<TagView> newTags)
        {
            var tags = tagsRepository.GetTagsByIssue(_view.Id);
            _view.Tags.Should().HaveSameCount(tags);
            _view.Tags.Select(c => c.Name).ToList()
                .Should().BeEquivalentTo(newTags.Select(c => c.Name).ToList());
            
            return this;
        }
    }
}
