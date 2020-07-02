using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Extensions;
using Inshapardaz.Functions.Configuration;
using Inshapardaz.Storage.Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Inshapardaz.Functions.Startup))]

namespace Inshapardaz.Functions
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddTransient<IReadClaims, ClaimsReader>()
                         .AddHttpClient()
                         .AddBrighterCommand()
                         .AddDarkerQuery();
            AddDatabaseConnection(builder.Services)
             .AddDatabase();

            builder.Services.AddTransient<IFileStorage, FileStorage>();

            AddCustomServices(builder.Services);
        }

        protected virtual IServiceCollection AddDatabaseConnection(IServiceCollection services)
        {
            services.AddDatabaseConnection();
            return services;
        }

        protected virtual IServiceCollection AddCustomServices(IServiceCollection services)
        {
            return services;
        }
    }
}
