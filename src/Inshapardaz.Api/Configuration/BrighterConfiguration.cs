using Inshapardaz.Domain;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter.AspNetCore;
using Paramore.Darker.AspNetCore;

namespace Inshapardaz.Api.Configuration
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

        public static IServiceCollection AddDarkerQuery(this IServiceCollection services)
        {
            services.AddDarker()
                    .AddHandlersFromAssemblies(typeof(DomainModule).Assembly);
            return services;
        }
    }
}
