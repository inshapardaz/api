using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.Options;

namespace Inshapardaz.Api.Infrastructure.Middleware;

public class LibraryConfigurationMiddleware(
    RequestDelegate next,
    IOptions<Settings> settings,
    ILogger<LibraryConfigurationMiddleware> logger)
{
    private readonly Settings _settings = settings.Value;

    public async Task Invoke(HttpContext context, LibraryConfiguration libraryConfiguration, ILibraryRepository _libraryRepository)
    {

        var libraryIdValue = context.GetRouteValue("libraryId")?.ToString();
        logger.LogDebug("Library from route {LibraryId}", libraryIdValue);
        var libraryId = 0;
        if (string.IsNullOrWhiteSpace(libraryIdValue))
        {
            libraryId = _settings.DefaultLibraryId;
            logger.LogDebug("Using default library");
        }
        else if (!int.TryParse(libraryIdValue, out libraryId))
        {
            await next(context);
        }

        var library = await _libraryRepository.GetLibraryById(libraryId, CancellationToken.None);
        if (library is not null)
        {
            libraryConfiguration.LibraryId = libraryId;
            libraryConfiguration.ConnectionString = library.DatabaseConnection ?? _settings.Database.ConnectionString;
            libraryConfiguration.DatabaseConnectionType = library.DatabaseConnectionType ?? _settings.Database.DatabaseConnectionType.Value;
            libraryConfiguration.FileStoreType = library.FileStoreType ?? _settings.Storage.FileStoreType.Value;
            libraryConfiguration.FileStoreSource = library.FileStoreSource;
        }
        else
        {
            logger.LogWarning("No Library in context");
        }

        await next(context);
    }
}
