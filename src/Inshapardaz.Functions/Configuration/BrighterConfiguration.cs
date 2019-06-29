using Inshapardaz.Domain;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter.AspNetCore;

namespace Inshapardaz.Functions.Configuration
{
    public static class BrighterConfiguration
    {
        public static IServiceCollection AddBrighterCommand(this IServiceCollection services)
        {
            services.AddBrighter()
                    .AsyncHandlersFromAssemblies(typeof(Startup).Assembly)
                    .AsyncHandlersFromAssemblies(typeof(DomainModule).Assembly);
            return services;
        }
    }
}