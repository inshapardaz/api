using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters.Repositories;

namespace Inshapardaz.Api.Tests.Fakes
{
    public class FakeFileStorage : IFileStorage
    {
        private readonly object _lock = new object();

        private readonly Dictionary<string, byte[]> _contents = new Dictionary<string, byte[]>();

        private string GetUrl(string name) => $"http://localhost.blob/{name}";

        public bool SupportsPublicLink => false;

        public string  SetupFileContents(string filePath, string content)
         =>
            SetupFileContents(filePath, System.Text.Encoding.UTF8.GetBytes(content));

        public string SetupFileContents(string filePath, byte[] content)
        {
            lock (_lock)
            {
                var url = GetUrl(filePath);
                if (!_contents.TryAdd(url, content))
                {
                    _contents[url] = content;
                }
                return filePath;
            }
        }

        public async Task<byte[]> GetFile(string filePath, CancellationToken cancellationToken)
        {
            var url = GetUrl(filePath);
            if (_contents.ContainsKey(url))
            {
                return await Task.FromResult(_contents[url]);
            }

            throw new Exception();
        }

        public async Task<string> GetTextFile(string filePath, CancellationToken cancellationToken)
        {
            var url = GetUrl(filePath);
            if (_contents.ContainsKey(url))
            {
                return await Task.FromResult(System.Text.Encoding.UTF8.GetString(_contents[url]));
            }

            return null;
        }

        public bool DoesFileExists(string filePath) => _contents.ContainsKey(GetUrl(filePath));

        public async Task<string> StoreFile(string name, byte[] content, CancellationToken cancellationToken)
        {
            return await Task.FromResult(SetupFileContents(name, content));
        }

        public async Task<string> StoreTextFile(string name, string content, CancellationToken cancellationToken)
        {
            return await Task.FromResult(SetupFileContents(name, content));
        }

        public Task DeleteFile(string filePath, CancellationToken cancellationToken)
        {
            var url = GetUrl(filePath);
            if (_contents.ContainsKey(url))
            {
                _contents.Remove(url);
            }

            return Task.CompletedTask;
        }

        public Task TryDeleteFile(string filePath, CancellationToken cancellationToken)
        {
            return DeleteFile(filePath, cancellationToken);
        }

        public async Task<string> StoreImage(string name, byte[] content, string mimeType, CancellationToken cancellationToken)
        {
            return await Task.FromResult(SetupFileContents(name, content));
        }

        public Task DeleteImage(string filePath, CancellationToken cancellationToken) => DeleteFile(filePath, cancellationToken);

        public Task TryDeleteImage(string filePath, CancellationToken cancellationToken) => DeleteFile(filePath, cancellationToken);

        public string GetPublicUrl(string filePath)
        {
            return filePath;
        }
    }
}
