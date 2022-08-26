using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Helpers;
using Inshapardaz.Api.Views.Library;
using Inshapardaz.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Net.Http;
using System.Threading;

namespace Inshapardaz.Api.Tests.Asserts
{
    internal class IssueContentAssert
    {
        private HttpResponseMessage _response;
        private readonly int _libraryId;
        private IssueContentView _issueContent;
        private LibraryDto _library;

        public IssueContentAssert(HttpResponseMessage response, int libraryId)
        {
            _response = response;
            _libraryId = libraryId;
            _issueContent = response.GetContent<IssueContentView>().Result;
        }

        public IssueContentAssert(HttpResponseMessage response, LibraryDto library)
        {
            _response = response;
            _libraryId = library.Id;
            _library = library;
            _issueContent = response.GetContent<IssueContentView>().Result;
        }

        internal IssueContentAssert ShouldHaveSelfLink()
        {
            _issueContent.SelfLink()
                  .ShouldBeGet()
                  .EndingWith($"/libraries/{_libraryId}/periodicals/{_issueContent.PeriodicalId}/volumes/{_issueContent.VolumeNumber}/issues/{_issueContent.IssueNumber}/contents");

            return this;
        }

        internal IssueContentAssert WithReadOnlyLinks()
        {
            ShouldNotHaveUpdateLink();
            ShouldNotHaveDeleteLink();
            return this;
        }

        internal IssueContentAssert WithWriteableLinks()
        {
            ShouldHaveUpdateLink();
            ShouldHaveDeleteLink();
            return this;
        }

        internal static void ShouldNotHaveIssueContent(int id, IDbConnection db)
        {
            var content = db.GetIssueContent(id);
            content.Should().BeNull();
        }

        internal void ShouldHaveIssueContent(int id, IDbConnection db)
        {
            var content = db.GetIssueContent(id);
            content.Should().NotBeNull();
            content.Language.Should().Be(_issueContent.Language);
            content.MimeType.Should().Be(_issueContent.MimeType);
        }

        internal static void ShouldHaveIssueContent(int id, string language, string mimeType, IDbConnection db)
        {
            var content = db.GetIssueContent(id);
            content.Should().NotBeNull();
            content.Language.Should().Be(language);
            content.MimeType.Should().Be(mimeType);
        }

        internal void ShouldHaveIssueContent(byte[] expected, IDbConnection db, IFileStorage fileStore)
        {
            var content = db.GetIssueContent(_issueContent.Id);
            content.Should().NotBeNull();

            content.Language.Should().Be(_issueContent.Language);
            content.MimeType.Should().Be(_issueContent.MimeType);

            // TODO: Make sure the contents are correct
            //var file = fileStore.GetFile(content.FilePath, CancellationToken.None).Result;
            //file.Should().NotBeNull().And.Should().Be(expected);
        }

        internal IssueContentAssert ShouldHaveUpdateLink()
        {
            _issueContent.UpdateLink()
                 .ShouldBePut()
                 .EndingWith($"libraries/{_libraryId}/periodicals/{_issueContent.PeriodicalId}/volumes/{_issueContent.VolumeNumber}/issues/{_issueContent.IssueNumber}/contents");

            return this;
        }

        internal IssueContentAssert ShouldHaveCorrectLanguage(string locale)
        {
            _issueContent.Language.Should().Be(locale);
            return this;
        }

        internal IssueContentAssert ShouldHaveCorrectMimeType(string mimeType)
        {
            _issueContent.MimeType.Should().Be(mimeType);
            return this;
        }

        internal IssueContentAssert ShouldNotHaveUpdateLink()
        {
            _issueContent.UpdateLink().Should().BeNull();
            return this;
        }

        internal IssueContentAssert ShouldHaveDefaultLibraryLanguage()
        {
            _issueContent.Language.Should().Be(_library.Language);
            return this;
        }

        internal IssueContentAssert ShouldHaveCorrectLocationHeader()
        {
            var location = _response.Headers.Location.AbsoluteUri;
            location.Should().NotBeNull();
            location.Should().EndWith($"libraries/{_libraryId}/periodicals/{_issueContent.PeriodicalId}/volumes/{_issueContent.VolumeNumber}/issues/{_issueContent.IssueNumber}/contents");
            return this;
        }

        internal IssueContentAssert ShouldHaveCorrectContents(byte[] expected, IFileStorage fileStorage, IDbConnection dbConnection, string newLanguage = null, string newMimeType = null)
        {
            var filePath = dbConnection.GetIssueContentPath(_issueContent.Id, newLanguage ?? _issueContent.Language, newMimeType ?? _issueContent.MimeType);
            var content = fileStorage.GetFile(filePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.Equal(expected);
            return this;
        }

        internal IssueContentAssert ShouldHaveCorrectContentsForMimeType(byte[] expected, string mimeType, IFileStorage fileStorage, IDbConnection dbConnection)
        {
            var filePath = dbConnection.GetIssueContentPath(_issueContent.Id, _issueContent.Language, mimeType);
            var content = fileStorage.GetFile(filePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.Equal(expected);
            return this;
        }

        internal IssueContentAssert ShouldHaveCorrectContentsForLanguage(byte[] expected, string language, IFileStorage fileStorage, IDbConnection dbConnection)
        {
            var filePath = dbConnection.GetIssueContentPath(_issueContent.Id, language, _issueContent.MimeType);
            var content = fileStorage.GetFile(filePath, CancellationToken.None).Result;
            content.Should().NotBeNull().And.Equal(expected);
            return this;
        }

        internal IssueContentAssert ShouldHavePrivateDownloadLink()
        {
            _issueContent.Link("download")
                           .ShouldBeGet();

            return this;
        }

        internal IssueContentAssert ShouldHavePublicDownloadLink()
        {
            _issueContent.Link("download")
                           .ShouldBeGet();

            return this;
        }

        internal IssueContentAssert ShouldHaveSavedIssueContent(IDbConnection dbConnection)
        {
            var dbContent = dbConnection.GetIssueContent(_issueContent.Id);
            dbContent.Should().NotBeNull();
            var dbFile = dbConnection.GetFileById(dbContent.FileId);
            _issueContent.Id.Should().Be(dbContent.Id);
            _issueContent.Language.Should().Be(dbContent.Language);
            _issueContent.MimeType.Should().Be(dbFile.MimeType);

            return this;
        }

        internal IssueContentAssert ShouldHaveDeleteLink()
        {
            _issueContent.DeleteLink()
                 .ShouldBeDelete()
                 .EndingWith($"libraries/{_libraryId}/periodicals/{_issueContent.PeriodicalId}/volumes/{_issueContent.VolumeNumber}/issues/{_issueContent.IssueNumber}/contents");

            return this;
        }

        internal IssueContentAssert ShouldNotHaveDeleteLink()
        {
            _issueContent.DeleteLink().Should().BeNull();
            return this;
        }

        internal IssueContentAssert ShouldHavePeriodicalLink()
        {
            _issueContent.Link("periodical")
                .ShouldBeGet()
                .EndingWith($"libraries/{_libraryId}/periodicals/{_issueContent.PeriodicalId}");

            return this;
        }

        internal IssueContentAssert ShouldHaveIssueLink()
        {
            _issueContent.Link("issue")
                .ShouldBeGet()
                .EndingWith($"libraries/{_libraryId}/periodicals/{_issueContent.PeriodicalId}/volumes/{_issueContent.VolumeNumber}/issues/{_issueContent.IssueNumber}");

            return this;
        }
        internal static void ShouldHaveDeletedContent(IDbConnection dbConnection, IssueFileDto content, string mimeType)
        {
            var dbContent = dbConnection.GetIssueContent(content.Id);
            dbContent.Should().BeNull("Issue content should be deleted");

            var dbFile = dbConnection.GetFileById(content.FileId);
            dbFile.Should().BeNull("Files for content should be deleted");
        }

        internal static void ShouldHaveLocationHeader(RedirectResult result, int libraryId, int periodicalId, IssueFileDto content)
        {
            var response = result as RedirectResult;
            response.Url.Should().NotBeNull();
            response.Url.Should().EndWith($"libraries/{libraryId}/periodicals/{periodicalId}/volumes/{content.VolumeNumber}/issues/{content.IssueNumber}/contents");
        }

        internal IssueContentAssert ShouldMatch(IssueContentDto content, int contentId, IDbConnection dbConnection)
        {
            _issueContent.MimeType.Should().Be(content.MimeType);
            _issueContent.Id.Should().Be(contentId);
            _issueContent.Language.Should().Be(content.Language);

            var dbFile = dbConnection.GetFileById(content.FileId);
            _issueContent.MimeType.Should().Be(dbFile.MimeType);

            return this;
        }
    }
}
