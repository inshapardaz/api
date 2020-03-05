using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Configuration;
using Inshapardaz.Storage;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

[assembly: WebJobsStartup(typeof(Inshapardaz.Functions.Startup))]

namespace Inshapardaz.Functions
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder builder)
        {
            builder //.AddAccessTokenBinding()
                    //.AddBinders()
                .Services.AddTransient<IReadClaims, ClaimsReader>()
                         .AddHttpClient()
                         .AddBrighterCommand()
                         .AddDarkerQuery()
                         .AddDatabase();

            if (!builder.Services.Any(x => x.ServiceType == typeof(IFileStorage)))
            {
                builder.Services.AddTransient<IFileStorage>(sp => new FileStorage(ConfigurationSettings.FileStorageConnectionString));
            }
        }
    }
}
