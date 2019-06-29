using AutoMapper;
using Inshapardaz.Ports.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Functions.Configuration
{
    public static class MapperConfiguration
    {
    public static IServiceCollection AddMappings(this IServiceCollection services)
        {
            Mapper.Initialize(c =>
                {
                    c.AddProfile(new MappingProfile());
                    c.AddProfile(new DatabaseMappingProfile());
                }
            );
            Mapper.AssertConfigurationIsValid();

            return services;
        }
    }
}