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
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<ILibraryRepository, LibraryRepository>();
            services.AddTransient<IAuthorRepository, AuthorRepository>();
            services.AddTransient<IArticleRepository, ArticleRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IBookRepository, BookRepository>();
            services.AddTransient<IChapterRepository, ChapterRepository>();
            services.AddTransient<ISeriesRepository, SeriesRepository>();
            services.AddTransient<IBookPageRepository, BookPageRepository>();
            services.AddTransient<IPeriodicalRepository, PeriodicalRepository>();
            services.AddTransient<IIssueRepository, IssueRepository>();
            services.AddTransient<IIssueArticleRepository, IssueArticleRepository>();
            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<ICorrectionRepository, CorrectionRepository>();
            services.AddTransient<IIssuePageRepository, IssuePageRepository>();
            services.AddTransient<IBookShelfRepository, BookShelfRepository>();
            return services;
        }

        public static readonly LoggerFactory MyLoggerFactory
            = new LoggerFactory();
    }
}
