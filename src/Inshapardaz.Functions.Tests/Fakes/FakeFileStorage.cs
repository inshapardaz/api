using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories;

namespace Inshapardaz.Functions.Tests.Fakes
{
    public class FakeFileStorage : IFileStorage
    {
        private readonly object _lock = new object();

        private readonly Dictionary<string, byte[]> _contents = new Dictionary<string, byte[]>();

        public FakeFileStorage()
        {
            Console.Write("");
        }

        private static string GetUrl(string name) => $"{Settings.BlobRoot}/{name}";

        public void SetupFileContents(string filePath, string content)
        {
            SetupFileContents(filePath, System.Text.Encoding.UTF8.GetBytes(content));
        }

        public void SetupFileContents(string filePath, byte[] content)
        {
            lock (_lock)
            {
                if (!_contents.TryAdd(filePath, content))
                {
                    _contents[filePath] = content;
                }
            }
        }

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
                return await Task.FromResult(System.Text.Encoding.UTF8.GetString(_contents[filePath]));
            }

            return null;
        }

        public bool DoesFileExists(string filePath) => _contents.ContainsKey(filePath);

        public async Task<string> StoreFile(string name, byte[] content, CancellationToken cancellationToken)
        {
            var url = GetUrl(name);
            SetupFileContents(url, content);
            return await Task.FromResult(url);
        }

        public async Task<string> StoreTextFile(string name, string content, CancellationToken cancellationToken)
        {
            var url = GetUrl(name);
            SetupFileContents(url, content);
            return await Task.FromResult(url);
        }

        public Task DeleteFile(string filePath, CancellationToken cancellationToken)
        {
            if (_contents.ContainsKey(filePath))
            {
                _contents.Remove(filePath);
            }

            return Task.CompletedTask;
        }

        public Task TryDeleteFile(string filePath, CancellationToken cancellationToken)
        {
            if (_contents.ContainsKey(filePath))
            {
                _contents.Remove(filePath);
            }

            return Task.CompletedTask;
        }

        public async Task<string> StoreImage(string name, byte[] content, CancellationToken cancellationToken)
        {
            var url = GetUrl(name);
            SetupFileContents(url, content);
            return await Task.FromResult(url);
        }

        public Task DeleteImage(string filePath, CancellationToken cancellationToken) => DeleteFile(filePath, cancellationToken);

        public Task TryDeleteImage(string filePath, CancellationToken cancellationToken) => TryDeleteFile(filePath, cancellationToken);
    }
}
