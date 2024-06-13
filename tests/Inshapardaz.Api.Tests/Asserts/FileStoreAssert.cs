using FluentAssertions;
using Inshapardaz.Api.Tests.DataHelpers;
using Inshapardaz.Api.Tests.Dto;
using Inshapardaz.Api.Tests.Fakes;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Api.Tests.Asserts
{
    public class FileStoreAssert
    {
        private readonly IProvideConnection _connectionProvider;
        private readonly FakeFileStorage _fileStore;

        public FileStoreAssert(IProvideConnection connectionProvider, IFileStorage fileStorage)
        {
            _connectionProvider = connectionProvider;
            _fileStore = fileStorage as FakeFileStorage;
        }

        internal void FileDoesnotExist(FileDto imageFile)
        {
            using (var databaseConnection = _connectionProvider.GetConnection())
            {
                var file = databaseConnection.GetFileById(imageFile.Id);
                file.Should().BeNull();
                _fileStore.DoesFileExists(imageFile.FilePath);
            }
        }

        internal void FileDoesnotExist(long fileId, string filePath)
        {
            using (var databaseConnection = _connectionProvider.GetConnection())
            {
                var file = databaseConnection.GetFileById(fileId);
                file.Should().BeNull();
                _fileStore.DoesFileExists(filePath);
            }
        }
    }
}
