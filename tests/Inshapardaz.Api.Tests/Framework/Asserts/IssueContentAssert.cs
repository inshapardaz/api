using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;
using Inshapardaz.Api.Tests.Framework.Helpers;
using Inshapardaz.Api.Views.Library;
using Microsoft.AspNetCore.Mvc;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class IssueContentAssert(
        IIssueTestRepository issueRepository,
        IFileTestRepository fileRepository,
        FakeFileStorage fileStorage)
    {
        private HttpResponseMessage _response;
        private int _libraryId;
        private IssueContentView _issueContent;
        private LibraryDto _library;

        public IssueContentAssert ForResponse(HttpResponseMessage response)
        {
            _response = response;
            _issueContent = response.GetContent<IssueContentView>().Result;
            return this;
        }

        public IssueContentAssert ForLibrary(LibraryDto library)
        {
            _libraryId = library.Id;
            _library = library;
            return this;
        }

        public IssueContentAssert ShouldHaveSelfLink()
        {
            _issueContent.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"/libraries/{_libraryId}/periodicals/{_issueContent.PeriodicalId}/volumes/{_issueContent.VolumeNumber}/issues/{_issueContent.IssueNumber}/contents/{_issueContent.Id}");

            return this;
        }

        public IssueContentAssert WithReadOnlyLinks()
        {
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            return this;
        }

        public IssueContentAssert WithWriteableLinks()
        {
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            return this;
        }

        public IssueContentAssert ShouldNotHaveIssueContent(int id)
        {
            var content = issueRepository.GetIssueContent(id);
            content.Should().BeNull();
            return this;
        }

        public IssueContentAssert ShouldHaveIssueContent(int id)
        {
            var content = issueRepository.GetIssueContent(id);
            content.Should().NotBeNull();
            content.Language.Should().Be(_issueContent.Language);
            content.MimeType.Should().Be(_issueContent.MimeType);
            content.Id.Should().Be(id);
            return this;
        }

        public IssueContentAssert ShouldHaveIssueContent(int id, string language, string mimeType)
        {
            var content = issueRepository.GetIssueContent(id);
            content.Should().NotBeNull();
            content.Language.Should().Be(language);
            content.MimeType.Should().Be(mimeType);
            return this;
        }

        public IssueContentAssert ShouldHaveIssueContent(byte[] expected)
        {
            var content = issueRepository.GetIssueContent(_issueContent.Id);
            content.Should().NotBeNull();

            content.Language.Should().Be(_issueContent.Language);
            content.MimeType.Should().Be(_issueContent.MimeType);

            // TODO: Make sure the contents are correct
            //var file = fileStore.GetFile(content.FilePath, CancellationToken.None).Result;
            //file.Should().NotBeNull().And.Should().Be(expected);
            return this;
        }

        public IssueContentAssert ShouldHaveUpdateLink()
        {
            _issueContent.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"libraries/{_libraryId}/periodicals/{_issueContent.PeriodicalId}/volumes/{_issueContent.VolumeNumber}/issues/{_issueContent.IssueNumber}/contents/{_issueContent.Id}");

            return this;
        }

        public IssueContentAssert ShouldHaveCorrectLanguage(string locale)
        {
            _issueContent.Language.Should().Be(locale);
            return this;
        }

        public IssueContentAssert ShouldHaveCorrectMimeType(string mimeType)
        {
            _issueContent.MimeType.Should().Be(mimeType);
            return this;
        }

        public IssueContentAssert ShouldNotHaveUpdateLink()
        {
            _issueContent.UpdateLink().Should().BeNull();
            return this;
        }

        public IssueContentAssert ShouldHaveDefaultLibraryLanguage()
        {
            _issueContent.Language.Should().Be(_library.Language);
            return this;
        }

        public IssueContentAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}/periodicals/{_issueContent.PeriodicalId}/volumes/{_issueContent.VolumeNumber}/issues/{_issueContent.IssueNumber}/contents/{_issueContent.Id}");
            return this;
        }

        public IssueContentAssert ShouldHaveCorrectContents(byte[] expected, string newLanguage = null, string newMimeType = null)
        {
            var filePath = issueRepository.GetIssueContentPath(_issueContent.Id, newLanguage ?? _issueContent.Language, newMimeType ?? _issueContent.MimeType);
            var content = fileStorage.GetFile(filePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.Equal(expected);
            return this;
        }

        public IssueContentAssert ShouldHaveCorrectContentsForMimeType(byte[] expected, string mimeType)
        {
            var filePath = issueRepository.GetIssueContentPath(_issueContent.Id, _issueContent.Language, mimeType);
            var content = fileStorage.GetFile(filePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.Equal(expected);
            return this;
        }

        public IssueContentAssert ShouldHaveCorrectContentsForLanguage(byte[] expected, string language)
        {
            var filePath = issueRepository.GetIssueContentPath(_issueContent.Id, language, _issueContent.MimeType);
            var content = fileStorage.GetFile(filePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.Equal(expected);
            return this;
        }

        public IssueContentAssert ShouldHavePrivateDownloadLink()
        {
            _issueContent.Link("download")
                           .ShouldBeGet();

            return this;
        }

        public IssueContentAssert ShouldHavePublicDownloadLink()
        {
            _issueContent.Link("download")
                           .ShouldBeGet();

            return this;
        }

        public IssueContentAssert ShouldHaveSavedIssueContent()
        {
            var dbContent = issueRepository.GetIssueContent(_issueContent.Id);
            dbContent.Should().NotBeNull();
            var dbFile = fileRepository.GetFileById(dbContent.FileId);
            _issueContent.Id.Should().Be(dbContent.Id);
            _issueContent.Language.Should().Be(dbContent.Language);
            _issueContent.MimeType.Should().Be(dbFile.MimeType);
            _issueContent.Checksum.Should().Be(dbFile.Checksum);

            return this;
        }

        public IssueContentAssert ShouldHaveDeleteLink()
        {
            _issueContent.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"libraries/{_libraryId}/periodicals/{_issueContent.PeriodicalId}/volumes/{_issueContent.VolumeNumber}/issues/{_issueContent.IssueNumber}/contents/{_issueContent.Id}");

            return this;
        }

        public IssueContentAssert ShouldNotHaveDeleteLink()
        {
            _issueContent.DeleteLink().Should().BeNull();
            return this;
        }

        public IssueContentAssert ShouldHavePeriodicalLink()
        {
            _issueContent.Link("periodical")
                .ShouldBeGet()
                .EndingWith($"libraries/{_libraryId}/periodicals/{_issueContent.PeriodicalId}");

            return this;
        }

        public IssueContentAssert ShouldHaveIssueLink()
        {
            _issueContent.Link("issue")
                .ShouldBeGet()
                .EndingWith($"libraries/{_libraryId}/periodicals/{_issueContent.PeriodicalId}/volumes/{_issueContent.VolumeNumber}/issues/{_issueContent.IssueNumber}");

            return this;
        }

        public IssueContentAssert ShouldHaveDeletedContent(IssueContentDto content, string mimeType)
        {
            var dbContent = issueRepository.GetIssueContent(content.Id);
            dbContent.Should().BeNull("Issue content should be deleted");

            var dbFile = fileRepository.GetFileById(content.FileId);
            dbFile.Should().BeNull("Files for content should be deleted");

            return this;
        }

        public IssueContentAssert ShouldHaveDeletedContent(IssueContentDto content, FileDto file)
        {
            var dbContent = issueRepository.GetIssueContent(content.Id);
            dbContent.Should().BeNull("Issue content should be deleted");

            var dbFile = fileRepository.GetFileById(content.FileId);
            dbFile.Should().BeNull("Files for content should be deleted");

            fileStorage.DoesFileExists(file.FilePath).Should().BeFalse();
            return this;
        }

        public IssueContentAssert ShouldHaveLocationHeader(RedirectResult result, int libraryId, int periodicalId, IssueFileDto content)
        {
            result.Url.Should().NotBeNull();
            result.Url.Should().EndWith($"libraries/{libraryId}/periodicals/{periodicalId}/volumes/{content.VolumeNumber}/issues/{content.IssueNumber}/contents");
            return this;
        }

        public IssueContentAssert ShouldMatch(IssueContentDto content, int contentId)
        {
            _issueContent.MimeType.Should().Be(content.MimeType);
            _issueContent.Id.Should().Be(contentId);
            _issueContent.Language.Should().Be(content.Language);

            var dbFile = fileRepository.GetFileById(content.FileId);
            _issueContent.MimeType.Should().Be(dbFile.MimeType);
            _issueContent.Checksum.Should().Be(dbFile.Checksum);

            return this;
        }

        public IssueContentAssert ShouldHaveChecksum(string expectedChecksum)
        {
            _issueContent.Checksum.Should().Be(expectedChecksum);
            return this;
        }
    }
}
