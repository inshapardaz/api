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
        Dictionary<string, byte[]> _contents = new Dictionary<string, byte[]>();

        public FakeFileStorage()
        {
            Console.Write("");
        }
        
        public void SetupFileContents(string filePath, string content)
        {
            SetupFileContents(filePath, System.Text.UnicodeEncoding.UTF8.GetBytes(content));
        }

        public void SetupFileContents(string filePath, byte[] content)
        {
            if (!_contents.TryAdd(filePath, content))
            {
                _contents[filePath] = content;
            }
        }

        readonly List<string> _deletedFileList = new List<string>();

        public async Task<byte[]> GetFile(string filePath, CancellationToken cancellationToken)
        {
            if (_contents.ContainsKey(filePath))
            {
                return await Task.FromResult(_contents[filePath]);
            }

            throw new Exception();
        }

        public async Task<string> GetTextFile(string filePath, CancellationToken cancellationToken)
        {
            if (_contents.ContainsKey(filePath))
            {
                return await Task.FromResult(System.Text.UnicodeEncoding.UTF8.GetString(_contents[filePath]));
            }

            throw new Exception();
        }

        public async Task<string> StoreFile(string name, byte[] content, CancellationToken cancellationToken)
        {
            var url = $"http://temp.file/{name}";
            SetupFileContents(url, content);
            return url;
        }

        public async Task<string> StoreTextFile(string name, string content, CancellationToken cancellationToken)
        {
            var url = $"http://temp.file/{name}";
            SetupFileContents(url, content);
            return url;
        }

        public async Task DeleteFile(string filePath, CancellationToken cancellationToken)
        {
            await Task.Run(() => _deletedFileList.Add(filePath));
        }

        public async Task TryDeleteFile(string filePath, CancellationToken cancellationToken)
        {
            await Task.Run(() => _deletedFileList.Add(filePath));
        }

        public void AssertFileDeleted(string filePath)
        {
            Assert.That(_deletedFileList.Any(f => f == filePath), Is.True);
        }
    }
}
