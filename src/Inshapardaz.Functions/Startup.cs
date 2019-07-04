using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Configuration;
using Inshapardaz.Functions.Library.Categories;
using Inshapardaz.Storage;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
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
                            .AddDatabase();

            builder.Services.AddTransient<IFunctionAppAuthenticator, FunctionAppAuth0Authenticator>()
                   .AddTransient<IFileStorage>(sp => new FileStorage(ConfigurationSettings.FileStorageConnectionString));

            builder.Services.AddTransient<GetEntry>()
                   .AddTransient<GetLanguages>()
                   .AddTransient<GetCategories>()
                   .AddTransient<AddCategory>()
                   .AddTransient<GetCategoryById>()
                   .AddTransient<UpdateCategory>()
                   .AddTransient<DeleteCategory>();
        }
    }
}