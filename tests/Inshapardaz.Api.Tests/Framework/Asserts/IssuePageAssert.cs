using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using System;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class IssuePageAssert
    {
        public HttpResponseMessage _response;
        private IssuePageView _issuePage;
        private int _libraryId;
        private readonly IIssuePageTestRepository _issuePageRepository;
        private readonly IFileTestRepository _fileRepository;
        private readonly FakeFileStorage _fileStorage;

        public IssuePageAssert(IIssuePageTestRepository issuePageRepository,
            IFileTestRepository fileRepository,
            FakeFileStorage fileStorage)
        {
            _issuePageRepository = issuePageRepository;
            _fileRepository = fileRepository;
            _fileStorage = fileStorage;
        }

        public IssuePageAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _issuePage = response.GetContent<IssuePageView>().Result;
            return this;
        }

        public IssuePageAssert ForView(IssuePageView view)
        {
            _issuePage = view;
            return this;
        }

        public IssuePageAssert ForLibrary(int libraryId)
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

        public IssuePageAssert ShouldHaveNoIssuePage(int issueId, long pageId, int? imageId)
        {
            var page = _issuePageRepository.GetIssuePageByIssueId(issueId, pageId);
            page.Should().BeNull();

            if (imageId != null)
            {
                var image = _fileRepository.GetFileById(imageId.Value);
                image.Should().BeNull();
            }

            return this;
        }

        public IssuePageAssert ShouldHaveAssignedRecently()
        {
            _issuePage.WriterAssignTimeStamp.Should().NotBeNull();
            _issuePage.WriterAssignTimeStamp.Value.Should().BeWithin(TimeSpan.FromMinutes(1));
            return this;
        }

        public IssuePageAssert IssuePageShouldExist(int issueId, int pageNumber)
        {
            var page = _issuePageRepository.GetIssuePageByNumber(issueId, pageNumber);
            page.Should().NotBeNull();

            if (page.ImageId != null)
            {
                var image = _fileRepository.GetFileById(page.ImageId.Value);
                image.Should().NotBeNull();
            }

            return this;
        }

        public IssuePageAssert ShouldHaveNoIssuePageImage(int issueId, int pageNumber, int imageId)
        {
            var page = _issuePageRepository.GetIssuePageByNumber(issueId, pageNumber);
            page.ImageId.Should().BeNull();

            var image = _fileRepository.GetFileById(imageId);
            image.Should().BeNull();

            return this;
        }

        public IssuePageAssert ShouldNotHaveCorrectLocationHeader()
        {
            _response.Headers.Location.Should().BeNull();
            return this;
        }

        public IssuePageAssert ShouldHaveSavedPage()
        {
            _issuePageRepository.GetIssuePageByNumber(_issuePage.PeriodicalId, _issuePage.VolumeNumber, _issuePage.IssueNumber, _issuePage.SequenceNumber);
            return this;
        }

        public IssuePageAssert ShouldHaveUpdatedIssuePageImage(int issueId, int pageNumber, byte[] newImage)
        {
            var page = _issuePageRepository.GetIssuePageByNumber(issueId, pageNumber);
            page.ImageId.Should().BeGreaterThan(0);

            var image = _fileRepository.GetFileById(page.ImageId.Value);
            image.Should().NotBeNull();

            var content = _fileStorage.GetFile(image.FilePath, CancellationToken.None).Result;
            content.Should().BeEquivalentTo(newImage);

            return this;
        }

        public IssuePageAssert ShouldHaveCorrectImageLocationHeader(HttpResponseMessage response, int imageId)
        {
            string location = response.Headers.Location.AbsoluteUri;
            location.Should().NotBeEmpty();
            location.Should().EndWith($"/files/{imageId}");
            return this;
        }

        public IssuePageAssert ShouldHaveAddedIssuePageImage(int issueId, int pageNumber)
        {
            var page = _issuePageRepository.GetIssuePageByNumber(issueId, pageNumber);
            page.ImageId.Should().BeGreaterThan(0);

            var image = _fileRepository.GetFileById(page.ImageId.Value);
            image.Should().NotBeNull();

            return this;
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

        public IssuePageAssert ShouldMatch(IssuePageView view)
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
                _issuePage.WriterAssignTimeStamp.Should().BeCloseTo(view.WriterAssignTimeStamp.Value, TimeSpan.FromSeconds(3));
            }
            else
            {
                _issuePage.WriterAssignTimeStamp.Should().BeNull();
            }
            _issuePage.ReviewerAccountId.Should().Be(view.ReviewerAccountId);
            _issuePage.ReviewerAccountName.Should().Be(view.ReviewerAccountName);
            if (view.ReviewerAssignTimeStamp.HasValue)
            {
                _issuePage.ReviewerAssignTimeStamp.Should().BeCloseTo(view.ReviewerAssignTimeStamp.Value, TimeSpan.FromSeconds(3));
            }
            else
            {
                _issuePage.ReviewerAssignTimeStamp.Should().BeNull();
            }
            _issuePage.ArticleId.Should().Be(view.ArticleId);
            _issuePage.ArticleName.Should().Be(view.ArticleName);
            _issuePage.Status.Should().Be(view.Status);

            return this;
        }

        public IssuePageAssert ShouldMatch(IssuePageDto dto)
        {
            _issuePage.Text.Should().Be(dto.Text);
            _issuePage.SequenceNumber.Should().Be(dto.SequenceNumber);
            _issuePage.WriterAccountId.Should().Be(dto.WriterAccountId);

            if (dto.WriterAssignTimeStamp.HasValue)
            {
                _issuePage.WriterAssignTimeStamp.Should().BeCloseTo(dto.WriterAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _issuePage.WriterAssignTimeStamp.Should().BeNull();
            }
            _issuePage.ReviewerAccountId.Should().Be(dto.ReviewerAccountId);
            if (dto.ReviewerAssignTimeStamp.HasValue)
            {
                _issuePage.ReviewerAssignTimeStamp.Should().BeCloseTo(dto.ReviewerAssignTimeStamp.Value, TimeSpan.FromSeconds(2));
            }
            else
            {
                _issuePage.ReviewerAssignTimeStamp.Should().BeNull();
            }

            return this;
        }
    }
}
