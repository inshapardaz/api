using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using System;
using System.Data;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Asserts
{
    public class IssuePageAssert
    {
        internal static IssuePageAssert FromResponse(HttpResponseMessage response, int libraryId)
        {
            return new IssuePageAssert(response) { _libraryId = libraryId };
        }

        public static IssuePageAssert FromObject(IssuePageView view)
        {
            return new IssuePageAssert(view);
        }

        public HttpResponseMessage _response;
        private IssuePageView _issuePage;
        private int _libraryId;

        public IssuePageAssert(HttpResponseMessage response)
        {
            _response = response;
            _issuePage = response.GetContent<IssuePageView>().Result;
        }

        public IssuePageAssert(IssuePageView view)
        {
            _issuePage = view;
        }

        public IssuePageAssert InLibrary(int libraryId)
        {
            _libraryId = libraryId;
            return this;
        }

        public IssuePageAssert ShouldHaveCorrectLocationHeader()
        {
            string location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeEmpty();
            location.Should().EndWith($"libraries/{_libraryId}/periodicals/{_issuePage.PeriodicalId}/volumes/{_issuePage.VolumeNumber}/issues/{_issuePage.IssueNumber}/pages/{_issuePage.SequenceNumber}");
            return this;
        }

        internal static void ShouldHaveNoIssuePage(int issueId, long pageId, int? imageId, IDbConnection databaseConnection, FakeFileStorage fileStore)
        {
            var page = databaseConnection.GetIssuePageByIssueId(issueId, pageId);
            page.Should().BeNull();

            if (imageId != null)
            {
                var image = databaseConnection.GetFileById(imageId.Value);
                image.Should().BeNull();
            }
        }

        internal IssuePageAssert ShouldHaveAssignedRecently()
        {
            _issuePage.WriterAssignTimeStamp.Should().NotBeNull();
            _issuePage.WriterAssignTimeStamp.Value.Should().BeWithin(TimeSpan.FromMinutes(1));
            return this;
        }

        internal static void IssuePageShouldExist(int issueId, int pageNumber, IDbConnection databaseConnection)
        {
            var page = databaseConnection.GetIssuePageByNumber(issueId, pageNumber);
            page.Should().NotBeNull();

            if (page.ImageId != null)
            {
                var image = databaseConnection.GetFileById(page.ImageId.Value);
                image.Should().NotBeNull();
            }
        }

        internal static void ShouldHaveNoIssuePageImage(int issueId, int pageNumber, int imageId, IDbConnection databaseConnection, FakeFileStorage fileStore)
        {
            var page = databaseConnection.GetIssuePageByNumber(issueId, pageNumber);
            page.ImageId.Should().BeNull();

            var image = databaseConnection.GetFileById(imageId);
            image.Should().BeNull();
        }

        public IssuePageAssert ShouldNotHaveCorrectLocationHeader()
        {
            _response.Headers.Location.Should().BeNull();
            return this;
        }

        public void ShouldHaveSavedPage(IDbConnection databaseConnection)
        {
            databaseConnection.GetIssuePageByNumber(_issuePage.PeriodicalId, _issuePage.VolumeNumber, _issuePage.IssueNumber, _issuePage.SequenceNumber);
        }

        internal static void ShouldHaveUpdatedIssuePageImage(int issueId, int pageNumber, byte[] newImage, IDbConnection databaseConnection, FakeFileStorage fileStore)
        {
            var page = databaseConnection.GetIssuePageByNumber(issueId, pageNumber);
            page.ImageId.Should().BeGreaterThan(0);

            var image = databaseConnection.GetFileById(page.ImageId.Value);
            image.Should().NotBeNull();

            var content = fileStore.GetFile(image.FilePath, CancellationToken.None).Result;
            content.Should().BeEquivalentTo(newImage);
        }

        internal static void ShouldHaveCorrectImageLocationHeader(HttpResponseMessage response, int imageId)
        {
            string location = response.Headers.Location.AbsoluteUri;
            location.Should().NotBeEmpty();
            location.Should().EndWith($"/files/{imageId}");
        }

        internal static void ShouldHaveAddedIssuePageImage(int issueId, int pageNumber, IDbConnection databaseConnection, FakeFileStorage fileStore)
        {
            var page = databaseConnection.GetIssuePageByNumber(issueId, pageNumber);
            page.ImageId.Should().BeGreaterThan(0);

            var image = databaseConnection.GetFileById(page.ImageId.Value);
            image.Should().NotBeNull();
        }

        public IssuePageAssert ShouldHaveSelfLink()
        {
            _issuePage.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issuePage.PeriodicalId}/volumes/{_issuePage.VolumeNumber}/issues/{_issuePage.IssueNumber}/pages/{_issuePage.SequenceNumber}");
            return this;
        }

        public IssuePageAssert ShouldHavePeriodicalLink()
        {
            _issuePage.Link("periodical")
                .ShouldBeGet()
                .EndingWith($"libraries/{_libraryId}/periodicals/{_issuePage.PeriodicalId}");

            return this;
        }

        public IssuePageAssert ShouldHaveIssueLink()
        {
            _issuePage.Link("issue")
                .ShouldBeGet()
                .EndingWith($"libraries/{_libraryId}/periodicals/{_issuePage.PeriodicalId}/volumes/{_issuePage.VolumeNumber}/issues/{_issuePage.IssueNumber}");

            return this;
        }
        public IssuePageAssert ShouldHaveUpdateLink()
        {
            _issuePage.UpdateLink()
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issuePage.PeriodicalId}/volumes/{_issuePage.VolumeNumber}/issues/{_issuePage.IssueNumber}/pages/{_issuePage.SequenceNumber}");
            return this;
        }

        public IssuePageAssert ShouldNotHaveUpdateLink()
        {
            _issuePage.UpdateLink().Should().BeNull();
            return this;
        }

        public IssuePageAssert ShouldHaveDeleteLink()
        {
            _issuePage.DeleteLink()
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issuePage.PeriodicalId}/volumes/{_issuePage.VolumeNumber}/issues/{_issuePage.IssueNumber}/pages/{_issuePage.SequenceNumber}");
            return this;
        }

        public IssuePageAssert ShouldNotHaveDeleteLink()
        {
            _issuePage.DeleteLink().Should().BeNull();
            return this;
        }

        public IssuePageAssert ShouldHaveNoNextLink()
        {
            _issuePage.Link("next").Should().BeNull();
            return this;
        }

        public IssuePageAssert ShouldHaveNextLinkForPageNumber(int pageNumber)
        {
            _issuePage.Link("next")
                .ShouldBeGet()
                .EndingWith($"libraries/{_libraryId}/periodicals/{_issuePage.PeriodicalId}/volumes/{_issuePage.VolumeNumber}/issues/{_issuePage.IssueNumber}/pages/{pageNumber}");
            return this;
        }

        public IssuePageAssert ShouldHaveNoPreviousLink()
        {
            _issuePage.Link("previous").Should().BeNull();
            return this;
        }

        public IssuePageAssert ShouldHavePreviousLinkForPageNumber(int pageNumber)
        {
            _issuePage.Link("previous")
                .ShouldBeGet()
                .EndingWith($"libraries/{_libraryId}/periodicals/{_issuePage.PeriodicalId}/volumes/{_issuePage.VolumeNumber}/issues/{_issuePage.IssueNumber}/pages/{pageNumber}");
            return this;
        }

        public IssuePageAssert ShouldHaveImageLink(int imageId)
        {
            _issuePage.Link("image")
                  .ShouldBeGet()
                  .EndingWith($"files/{imageId}");
            return this;
        }

        public IssuePageAssert ShouldNotHaveImageLink()
        {
            _issuePage.Link("image").Should().BeNull();
            return this;
        }

        public IssuePageAssert ShouldHaveImageUpdateLink()
        {
            _issuePage.Link("image-upload")
                  .ShouldBePut()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issuePage.PeriodicalId}/volumes/{_issuePage.VolumeNumber}/issues/{_issuePage.IssueNumber}/pages/{_issuePage.SequenceNumber}/image");
            return this;
        }

        public IssuePageAssert ShouldNotHaveImageUpdateLink()
        {
            _issuePage.Link("image-update").Should().BeNull();
            return this;
        }

        public IssuePageAssert ShouldHaveImageDeleteLink()
        {
            _issuePage.Link("image-delete")
                  .ShouldBeDelete()
                  .EndingWith($"libraries/{_libraryId}/periodicals/{_issuePage.PeriodicalId}/volumes/{_issuePage.VolumeNumber}/issues/{_issuePage.IssueNumber}/pages/{_issuePage.SequenceNumber}/image");
            return this;
        }

        public IssuePageAssert ShouldNotHaveImageDeleteLink()
        {
            _issuePage.Link("image-delete").Should().BeNull();
            return this;
        }

        public void ShouldMatch(IssuePageView view)
        {
            _issuePage.Text.Should().Be(view.Text);
            _issuePage.PeriodicalId.Should().Be(view.PeriodicalId);
            _issuePage.VolumeNumber.Should().Be(view.VolumeNumber);
            _issuePage.IssueNumber.Should().Be(view.IssueNumber);
            _issuePage.SequenceNumber.Should().Be(view.SequenceNumber);
            _issuePage.WriterAccountId.Should().Be(view.WriterAccountId);
            _issuePage.WriterAccountName.Should().Be(view.WriterAccountName);
            if (view.WriterAssignTimeStamp.HasValue)
            {
                _issuePage.WriterAssignTimeStamp.Should().BeCloseTo(view.WriterAssignTimeStamp.Value, 3000);
            }
            else
            {
                _issuePage.WriterAssignTimeStamp.Should().BeNull();
            }
            _issuePage.ReviewerAccountId.Should().Be(view.ReviewerAccountId);
            _issuePage.ReviewerAccountName.Should().Be(view.ReviewerAccountName);
            if (view.ReviewerAssignTimeStamp.HasValue)
            {
                _issuePage.ReviewerAssignTimeStamp.Should().BeCloseTo(view.ReviewerAssignTimeStamp.Value, 3000);
            }
            else
            {
                _issuePage.ReviewerAssignTimeStamp.Should().BeNull();
            }
            _issuePage.ArticleNumber.Should().Be(view.ArticleNumber);
            _issuePage.ArticleName.Should().Be(view.ArticleName);
            _issuePage.Status.Should().Be(view.Status);
        }

        public IssuePageAssert ShouldMatch(IssuePageDto dto)
        {
            _issuePage.Text.Should().Be(dto.Text);
            _issuePage.SequenceNumber.Should().Be(dto.SequenceNumber);
            _issuePage.WriterAccountId.Should().Be(dto.WriterAccountId);

            if (dto.WriterAssignTimeStamp.HasValue)
            {
                _issuePage.WriterAssignTimeStamp.Should().BeCloseTo(dto.WriterAssignTimeStamp.Value, 2000);
            }
            else
            {
                _issuePage.WriterAssignTimeStamp.Should().BeNull();
            }
            _issuePage.ReviewerAccountId.Should().Be(dto.ReviewerAccountId);
            if (dto.ReviewerAssignTimeStamp.HasValue)
            {
                _issuePage.ReviewerAssignTimeStamp.Should().BeCloseTo(dto.ReviewerAssignTimeStamp.Value, 2000);
            }
            else
            {
                _issuePage.ReviewerAssignTimeStamp.Should().BeNull();
            }

            return this;
        }
    }

    public static class IssuePageAssertionExtensions
    {
        public static IssuePageAssert ShouldMatch(this IssuePageView view, IssuePageDto dto)
        {
            return IssuePageAssert.FromObject(view)
                               .ShouldMatch(dto);
        }
    }
}
