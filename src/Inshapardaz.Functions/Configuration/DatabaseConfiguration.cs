using System.Linq;
using Inshapardaz.Domain.Repositories;
using Inshapardaz.Domain.Repositories.Dictionary;
using Inshapardaz.Domain.Repositories.Library;
using Inshapardaz.Ports.Database;
using Inshapardaz.Ports.Database.Repositories;
using Inshapardaz.Ports.Database.Repositories.Dictionary;
using Inshapardaz.Ports.Database.Repositories.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Inshapardaz.Functions.Configuration
{
    public static class DatabaseConfiguration 
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            if (!services.Any(x => x.ServiceType == typeof(IDatabaseContext)))
            {
                var connectionString = ConfigurationSettings.DatabaseConnectionString;

                services.AddEntityFrameworkSqlServer()
                        .AddDbContext<DatabaseContext>(
                            options =>
                            {
                                options.UseLoggerFactory(MyLoggerFactory);
                                options.UseSqlServer(connectionString, o => o.UseRowNumberForPaging());
                            });

                services.AddTransient<IDatabaseContext, DatabaseContext>();
            }

            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<IDictionaryRepository, DictionaryRepository>();
            services.AddTransient<IWordRepository, WordRepository>();
            services.AddTransient<IMeaningRepository, MeaningRepository>();
            services.AddTransient<ITranslationRepository, TranslationRepository>();
            services.AddTransient<IRelationshipRepository, RelationshipRepository>();

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
            = new LoggerFactory(new[] { new ConsoleLoggerProvider((_, __) => true, true) });
    }
}