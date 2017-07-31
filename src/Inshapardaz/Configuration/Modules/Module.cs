using Inshapardaz.Domain;
using Inshapardaz.Domain.CommandHandlers;
using Inshapardaz.Domain.Queries;
using Inshapardaz.Domain.QueryHandlers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Api.Configuration.Modules
{
    public static class DomainModule
    {
        public static IServiceCollection ConfigureDomain(this IServiceCollection services,
            IConfigurationRoot configuration)
        {
            var connectionString = configuration["ConnectionStrings:DefaultDatabase"];

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<DatabaseContext>(
                    options => options.UseSqlServer(connectionString, o => o.UseRowNumberForPaging()));
            services.AddTransient<IDatabaseContext, DatabaseContext>();
            services.AddTransient<AddDictionaryCommandHandler>();
            services.AddTransient<AddWordCommandHandler>();
            services.AddTransient<AddWordDetailCommandHandler>();
            services.AddTransient<AddWordRelationCommandHandler>();
            services.AddTransient<AddWordTranslationCommandHandler>();
            services.AddTransient<AddWordMeaningCommandHandler>();

            services.AddTransient<UpdateDictionaryCommandHandler>();
            services.AddTransient<UpdateWordCommandHandler>();
            services.AddTransient<UpdateWordDetailCommandHandler>();
            services.AddTransient<UpdateWordRelationCommandHandler>();
            services.AddTransient<UpdateWordTranslationCommandHandler>();
            services.AddTransient<UpdateWordMeaningCommandHandler>();

            services.AddTransient<DeleteDictionaryCommandHandler>();
            services.AddTransient<DeleteWordCommandHandler>();
            services.AddTransient<DeleteWordDetailCommandHandler>();
            services.AddTransient<DeleteWordRelationCommandHandler>();
            services.AddTransient<DeleteWordTranslationCommandHandler>();
            services.AddTransient<DeleteWordMeaningCommandHandler>();

            // Darker Handlers
            services.AddTransient<GetDictionariesByUserQueryHandler>();
            services.AddTransient<GetDictionaryByIdQueryHandler>();
            services.AddTransient<WordStartingWithQueryHandler>();
            services.AddTransient<WordMeaningByWordQueryHandler>();
            services.AddTransient<WordMeaningByIdQueryHandler>();
            services.AddTransient<WordContainingTitleQuery>();
            services.AddTransient<WordIndexContainingTitleQueryHandler>();
            services.AddTransient<WordDetailsByWordQueryHandler>();
            services.AddTransient<WordDetailByIdQueryHandler>();
            services.AddTransient<GetWordsPagesQueryHandler>();
            services.AddTransient<WordByTitleQueryHandler>();
            services.AddTransient<WordByIdQueryHandler>();
            services.AddTransient<TranslationsByWordIdQueryHandler>();
            services.AddTransient<TranslationsByLanguageQueryHandler>();
            services.AddTransient<TranslationByIdQueryHandler>();
            services.AddTransient<RelationshipByWordIdQueryHandler>();
            services.AddTransient<RelationshipByIdQueryHandler>();
            services.AddTransient<GetDictionaryByWordIdQueryHandler>();
            services.AddTransient<DictionaryByWordIdQuery>();
            services.AddTransient<GetDictionaryByMeaningIdQueryHandler>();
            services.AddTransient<GetDictionaryByTranslationIdQueryHandler>();

            return services;
        }
    }
}