using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Repositories;
using NUnit.Framework;

namespace Inshapardaz.Functions.Tests.Fakes
{
    public class FakeFileStorage : IFileStorage
    {
        readonly List<string> _deletedFileList = new List<string>();

        public async Task<byte[]> GetFile(string filePath, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetTextFile(string filePath, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> StoreFile(string name, byte[] content, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> StoreTextFile(string name, string content, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteFile(string filePath, CancellationToken cancellationToken)
        {
            _deletedFileList.Add(filePath);
        }

        public async Task TryDeleteFile(string filePath, CancellationToken cancellationToken)
        {
            _deletedFileList.Add(filePath);
        }

        public void AssertFileDeleted(string filePath)
        {
            Assert.That(_deletedFileList.Any(f => f == filePath), Is.True);
        }
    }
}
