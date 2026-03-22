using FluentAssertions;
using Inshapardaz.Api.Tests.Framework.DataHelpers;
using Inshapardaz.Api.Tests.Framework.Dto;
using Inshapardaz.Api.Tests.Framework.Fakes;

namespace Inshapardaz.Api.Tests.Framework.Asserts
{
    public class FileStoreAssert(IFileTestRepository fileRepository, FakeFileStorage fileStorage)
    {
        public void FileDoesnotExist(FileDto imageFile)
        {
            var file = fileRepository.GetFileById(imageFile.Id);
            file.Should().BeNull();
            fileStorage.DoesFileExists(imageFile.FilePath);
        }

        public void FileDoesnotExist(long fileId, string filePath)
        {
            var file = fileRepository.GetFileById(fileId);
            file.Should().BeNull();
            fileStorage.DoesFileExists(filePath);
        }
    }
}
