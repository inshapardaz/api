using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Library;
using Inshapardaz.Database.SqlServer;
using Inshapardaz.Database.SqlServer.Repositories;
using Inshapardaz.Database.SqlServer.Repositories.Library;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Models;

namespace Inshapardaz.Api.Configuration
{
    public static class DatabaseConfiguration
    {
        public static IServiceCollection AddDatabaseConnection(this IServiceCollection services)
        {
            services.AddTransient<IProvideConnection>(sp => new SqlServerConnectionProvider(sp.GetService<Settings>().DefaultConnection, sp.GetService<LibraryConfiguration>()));
            return services;
        }

        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddScoped<IFileRepository, FileRepository>();
            services.AddScoped<ILibraryRepository, LibraryRepository>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            services.AddScoped<IChapterRepository, ChapterRepository>();
            services.AddScoped<ISeriesRepository, SeriesRepository>();
            services.AddScoped<IBookPageRepository, BookPageRepository>();
            services.AddScoped<IPeriodicalRepository, PeriodicalRepository>();
            services.AddScoped<IIssueRepository, IssueRepository>();
            services.AddScoped<IIssueArticleRepository, IssueArticleRepository>();
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ICorrectionRepository, CorrectionRepository>();
            services.AddScoped<IIssuePageRepository, IssuePageRepository>();
            services.AddScoped<IBookShelfRepository, BookShelfRepository>();
            return services;
        }

        public static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory();
    }
}
