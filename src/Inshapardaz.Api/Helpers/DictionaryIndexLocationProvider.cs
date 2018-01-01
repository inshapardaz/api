using System.IO;
using Inshapardaz.Domain.IndexingService;
using Microsoft.AspNetCore.Hosting;

namespace Inshapardaz.Api.Helpers
{
    public class DictionaryIndexLocationProvider : IProvideIndexLocation
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public DictionaryIndexLocationProvider(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }
        public string GetDictionaryIndexFolder(int dictionaryId)
        {
            var root = _hostingEnvironment.ContentRootPath;
            return Path.Combine(root, $"App_Data/{dictionaryId}");
        }
    }
}
