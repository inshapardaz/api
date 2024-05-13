using Inshapardaz.Domain;
using Microsoft.Extensions.DependencyInjection;
using Paramore.Brighter.Extensions.DependencyInjection;
using Paramore.Darker.AspNetCore;

namespace Inshapardaz.Api.Configuration
{
    public static class BrighterConfiguration
    {
        public static IServiceCollection AddBrighterCommand(this IServiceCollection services)
        {
            services.AddBrighter(options =>
            {
                options.HandlerLifetime = ServiceLifetime.Scoped;
            })
                    .AutoFromAssemblies(typeof(Startup).Assembly, typeof(DomainModule).Assembly);
            return services;
        }

        public static IServiceCollection AddDarkerQuery(this IServiceCollection services)
        {
            services.AddDarker(options => options.QueryProcessorLifetime = ServiceLifetime.Scoped)
                    .AddHandlersFromAssemblies(typeof(DomainModule).Assembly);
            return services;
        }
    }
}
