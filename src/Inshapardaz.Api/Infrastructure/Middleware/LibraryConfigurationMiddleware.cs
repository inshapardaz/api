using Inshapardaz.Domain.Adapters.Configuration;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Models;
using Microsoft.Extensions.Options;

namespace Inshapardaz.Api.Infrastructure.Middleware;

public class LibraryConfigurationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly Settings _settings;
    private readonly ILogger<LibraryConfigurationMiddleware> _logger;

    public LibraryConfigurationMiddleware(RequestDelegate next, IOptions<Settings> settings, ILogger<LibraryConfigurationMiddleware> logger)
    {
        _next = next;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, LibraryConfiguration libraryConfiguration, ILibraryRepository _libraryRepository)
    {

        var libraryIdValue = context.GetRouteValue("libraryId")?.ToString();
        _logger.LogDebug("Library from route {LibraryId}", libraryIdValue);
        var libraryId = 0;
        if (string.IsNullOrWhiteSpace(libraryIdValue))
        {
            libraryId = _settings.DefaultLibraryId;
            _logger.LogDebug("Using default library");
        }
        else if (!int.TryParse(libraryIdValue, out libraryId))
        {
            await _next(context);
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
            _logger.LogWarning("No Library in context");
        }

        await _next(context);
    }
}
