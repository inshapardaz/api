using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class FileStoreAssert
    {
        private readonly IFileTestRepository _fileRepository;
        private readonly FakeFileStorage _fileStore;

        public FileStoreAssert(IFileTestRepository fileRepository, FakeFileStorage fileStorage)
        {
            _fileRepository = fileRepository;
            _fileStore = fileStorage;
        }

        public void FileDoesnotExist(FileDto imageFile)
        {
            var file = _fileRepository.GetFileById(imageFile.Id);
            file.Should().BeNull();
            _fileStore.DoesFileExists(imageFile.FilePath);
        }

        public void FileDoesnotExist(long fileId, string filePath)
        {
            var file = _fileRepository.GetFileById(fileId);
            file.Should().BeNull();
            _fileStore.DoesFileExists(filePath);
        }
    }
}
