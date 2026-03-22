using Inshapardaz.Api.Infrastructure.Factories;
using Inshapardaz.Domain.Adapters.Configuration;
using Microsoft.Extensions.Options;

namespace Inshapardaz.Api.Infrastructure.Services;

public class MigrationService(IServiceProvider services) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = services.CreateScope();
        var settings = scope.ServiceProvider.GetRequiredService<IOptions<Settings>>().Value;
        var migrationFactory = scope.ServiceProvider.GetRequiredService<DatabaseMigrationFactory>();

        var migrator = migrationFactory.CreateMigrator(settings.Database.DatabaseConnectionType);
        migrator.UpdateDatabase(settings.Database.ConnectionString);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
