using Inshapardaz.Domain.Repositories;
using Inshapardaz.Functions.Authentication;
using Inshapardaz.Functions.Configuration;
using Inshapardaz.Functions.Library.Categories;
using Inshapardaz.Functions.Library.Series;
using Inshapardaz.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Inshapardaz.Functions.Library.Authors;
using Inshapardaz.Functions.Library.Books;

[assembly: WebJobsStartup(typeof(Inshapardaz.Functions.Startup))]
namespace Inshapardaz.Functions
{
    public class Startup : IWebJobsStartup
    {
        public void Configure(IWebJobsBuilder  builder)
        {
            builder.AddAccessTokenBinding();
            builder.Services.AddHttpClient()
                            .AddRenderers()
                            .AddBrighterCommand()
                            .AddDatabase()
                            .AddTransient<IFileStorage>(sp => new FileStorage(ConfigurationSettings.FileStorageConnectionString));

            builder.Services.AddTransient<GetEntry>()
                   .AddTransient<GetLanguages>()
                   .AddTransient<GetCategories>()
                   .AddTransient<AddCategory>()
                   .AddTransient<GetCategoryById>()
                   .AddTransient<UpdateCategory>()
                   .AddTransient<DeleteCategory>()
                   .AddTransient<GetSeries>()
                   .AddTransient<AddSeries>()
                   .AddTransient<GetSeriesById>()
                   .AddTransient<UpdateSeries>()
                   .AddTransient<DeleteSeries>()
                   .AddTransient<GetAuthors>()
                   .AddTransient<GetAuthorById>()
                   .AddTransient<AddAuthor>()
                   .AddTransient<UpdateAuthor>()
                   .AddTransient<DeleteAuthor>()
                   .AddTransient<GetBooks>()
                   .AddTransient<GetBookById>()
                   .AddTransient<AddBook>()
                   .AddTransient<UpdateBook>()
                   .AddTransient<DeleteBook>();
        }
    }
}
