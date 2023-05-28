using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Inshapardaz.Api.Middleware
{
    public class LibraryConfigurationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILibraryRepository _libraryRepository;
        private readonly Settings _settings;

        public LibraryConfigurationMiddleware(RequestDelegate next, ILibraryRepository libraryRepository, Settings settings)
        {
            _next = next;
            _libraryRepository = libraryRepository;
            _settings = settings;
        }

        public async Task Invoke(HttpContext context, LibraryConfiguration libraryConfiguration)
        {
            var libraryIdValue = context.GetRouteValue("libraryId")?.ToString();
            var libraryId = 0;
            if (string.IsNullOrWhiteSpace(libraryIdValue))
            {
                libraryId = _settings.DefaultLibraryId;
            }
            else if (!int.TryParse(libraryIdValue, out libraryId))
            {
                await _next(context);
            }

            var library = await _libraryRepository.GetLibraryById(libraryId, CancellationToken.None);
            if (library is not null)
            {
                libraryConfiguration.LibraryId = libraryId;
                libraryConfiguration.ConnectionString = library.DatabaseConnection ?? _settings.DefaultConnection;
                libraryConfiguration.FileStoreType = library.FileStoreType ?? _settings.FileStoreType;
                libraryConfiguration.FileStoreSource = library.FileStoreSource;
            }
            
            await _next(context);
        }
    }
}
