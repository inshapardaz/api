using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Inshapardaz.Ports.Database;
using Inshapardaz.Ports.Database.Repositories;
using Inshapardaz.Ports.Database.Repositories.Library;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Inshapardaz.Functions.Configuration
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddTransient<IProvideConnection>(sp => new SqlServerConnectionProvider(ConfigurationSettings.DatabaseConnectionString));
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<ILibraryRepository, LibraryRepository>();
            services.AddTransient<IAuthorRepository, AuthorRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IBookRepository, BookRepository>();
            services.AddTransient<IChapterRepository, ChapterRepository>();
            services.AddTransient<ISeriesRepository, SeriesRepository>();

            services.AddTransient<IPeriodicalRepository, PeriodicalRepository>();
            services.AddTransient<IIssueRepository, IssueRepository>();
            return services;
        }

        public static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory();
    }
}
