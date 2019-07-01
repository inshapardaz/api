using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(Inshapardaz.Functions.Startup))]
namespace Inshapardaz.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient()
                            .AddRenderers()
                            .AddBrighterCommand()
                            .AddDatabase()
                            .AddMappings();

            builder.Services.AddTransient<IFunctionAppAuthenticator, FunctionAppAuth0Authenticator>();

            builder.Services.AddTransient<GetEntry>();
        }
    }
}