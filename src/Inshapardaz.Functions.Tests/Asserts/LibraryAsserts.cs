using FluentAssertions;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Tests.DataHelpers;
using Inshapardaz.Functions.Views;
using Inshapardaz.Ports.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Functions.Tests.Asserts
{
    public class LibraryAsserts
    {
        private readonly IDbConnection _connection;
        private readonly IFileStorage _fileStorage;

        public LibraryAsserts(IProvideConnection connectionProvider, IFileStorage fileStorage)
        {
            _connection = connectionProvider.GetConnection();
            _fileStorage = fileStorage;
        }

        public void ThatBookHaveMatchingFiles(int bookId, IEnumerable<FileView> files)
        {
            throw new NotImplementedException();
        }

        public void ThatFileMatch(FileView fileView)
        {
            var file = _connection.GetFileById(fileView.Id);

            file.Should().NotBeNull();

            file.FileName.Should().Be(fileView.FileName, "File Name should match");
            file.MimeType.Should().Be(fileView.MimeType, "File MimeType should match");
        }

        public void ThatAuthorExists(int authorId)
        {
            var author = _connection.GetAuthorById(authorId);

            author.Should().NotBeNull();
        }

        public void ThatSeriesExists(int seriesId)
        {
            var series = _connection.GetSeriesById(seriesId);

            series.Should().NotBeNull();
        }

        public void ThatFileExists(int fileId)
        {
            var file = _connection.GetFileById(fileId);

            file.Should().NotBeNull();
        }

        public void ThatFileIsDeleted(int fileId)
        {
            var file = _connection.GetFileById(fileId);

            file.Should().BeNull();
        }

        internal async Task ThatFileContentsMatch(int id, byte[] expected)
        {
            var file = _connection.GetFileById(id);
            var contents = await _fileStorage.GetFile(file.FilePath, CancellationToken.None);
            contents.Should().BeSameAs(expected, "File contents should match.");
        }
    }
}
