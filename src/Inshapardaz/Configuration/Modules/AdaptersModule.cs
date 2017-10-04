using Inshapardaz.Api.Adapter;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Api.Configuration.Modules
{
    public static class AdaptersModule
    {
        public static IServiceCollection ConfigureAdapters(this IServiceCollection services)
        {
            services.AddTransient<GetEntryCommandHandler>();

            return services;
        }
    }
}
