using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Adapters.Repositories;
using System;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class IssueAssert
    {
        private HttpResponseMessage _response;
        private IssueView _view;
        private int _libraryId;

        public IssueAssert(HttpResponseMessage response)
        {
            _response = response;
            _view = response.GetContent<IssueView>().Result;
        }

        public IssueAssert(IssueView view)
        {
            _view = view;
        }

        public static IssueAssert FromResponse(HttpResponseMessage response)
        {
            return new IssueAssert(response);
        }

        public IssueAssert InLibrary(int libraryId)
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

        internal IssueAssert ShouldHaveCorrectImageLocationHeader(int issueId)
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

        internal IssueAssert ShouldHaveImageLink()
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

        public IssueAssert ShouldHaveCorrectContentsLink(IDbConnection db)
        {
            var contents = db.GetIssueContents(_view.Id);

            contents.Should().HaveSameCount(_view.Contents);

            foreach (var content in contents)
            {
                ShouldHaveContentLink(content);
            }

            return this;
        }

        public IssueAssert ShouldHaveCorrectContents(IDbConnection db)
        {
            var contents = db.GetIssueContents(_view.Id);

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


        internal void ShouldHaveContentLink(IssueContentDto content)
        {
            var actual = _view.Contents.Single(x => x.Id == content.Id);
            actual.SelfLink()
                  .ShouldBeGet()
                  .ShouldHaveAcceptLanguage(content.Language);
        }


        public IssueAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location;
            location.Should().NotBeNull();
            location.AbsoluteUri.Should().EndWith($"libraries/{_libraryId}/periodicals/{_view.PeriodicalId}/volumes/{_view.VolumeNumber}/issues/{_view.IssueNumber}");
            return this;
        }

        internal IssueAssert ShouldHaveImageLocationHeader()
        {
            _response.Headers.Location.AbsoluteUri.Should().NotBeEmpty();
            return this;
        }

        internal IssueAssert ShouldHaveSavedIssue(IDbConnection dbConnection)
        {
            var dbIssue = dbConnection.GetIssueById(_view.Id);
            dbIssue.Should().NotBeNull();
            _view.VolumeNumber.Should().Be(dbIssue.VolumeNumber);
            _view.IssueNumber.Should().Be(dbIssue.IssueNumber);
            _view.IssueDate.Should().BeCloseTo(dbIssue.IssueDate, TimeSpan.FromSeconds(2));

            return this;
        }

        internal static void ShouldHaveDeletedIssue(IDbConnection dbConnection, int issueId)
        {
            var dbIssue = dbConnection.GetIssueById(issueId);
            dbIssue.Should().BeNull();
        }

        internal static void ShouldHaveDeletedIssueImage(IDbConnection dbConnection, int issueId)
        {
            var issueImage = dbConnection.GetIssueImage(issueId);
            issueImage.Should().BeNull();
        }

        internal static void ShouldHaveDeletedArticlesForIssue(IDbConnection dbConnection, int issueId)
        {
            var articles = dbConnection.GetIssueArticlesByIssue(issueId);
            articles.Should().BeNullOrEmpty();
        }

        internal static void ShouldHaveDeletedPagesForIssue(IDbConnection dbConnection, int issueId)
        {
            var pages = dbConnection.GetIssuePagesByIssue(issueId);
            pages.Should().BeNullOrEmpty();
        }

        internal IssueAssert ShouldMatch(IssueView expected, int? articleCount = null, int? pageCount = null)
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

            return this;
        }

        internal IssueAssert ShouldBeSameAs(IDbConnection db, IssueDto expected, int? articleCount = null, int? pageCount = null)
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

            return this;
        }

        internal static void ShouldNotHaveUpdatedIssueImage(IDbConnection dbConnection, IFileStorage fileStorage, int issueId, byte[] oldImage)
        {
            var imageUrl = dbConnection.GetIssueImageUrl(issueId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().Equal(oldImage);
        }

        internal static void ShouldHaveAddedIssueImage(IDbConnection dbConnection, IFileStorage fileStorage, int issueId)
        {
            var imageUrl = dbConnection.GetIssueImageUrl(issueId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNullOrEmpty();
        }

        internal static void ShouldHaveUpdatedIssueImage(IDbConnection dbConnection, IFileStorage fileStorage, int issueId, byte[] newImage)
        {
            var imageUrl = dbConnection.GetIssueImageUrl(issueId);
            imageUrl.Should().NotBeNull();
            var image = fileStorage.GetFile(imageUrl, CancellationToken.None).Result;
            image.Should().NotBeNull().And.Equal(newImage);
        }

        internal static void ShouldHavePublicImage(IDbConnection dbConnection, int issueId)
        {
            var image = dbConnection.GetIssueImage(issueId);
            image.Should().NotBeNull();
            image.IsPublic.Should().BeTrue();
        }
    }

    public static class IssueAssertionExtensions
    {
        public static IssueAssert ShouldMatch(this IssueView view, IssueDto dto, IDbConnection dbConnection, int? libraryId = null, int? chapterCount = null, int? pageCount = null)
        {
            if (libraryId.HasValue)
            {
                return new IssueAssert(view)
                            .InLibrary(libraryId.Value)
                           .ShouldBeSameAs(dbConnection, dto, chapterCount, pageCount);
            }
            return new IssueAssert(view)
                               .ShouldBeSameAs(dbConnection, dto, chapterCount, pageCount);
        }
    }
}
