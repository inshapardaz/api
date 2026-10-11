using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Helpers.Spelling;

namespace Inshapardaz.Api.Infrastructure.Configuration;

// Services behind the /tools language features (spelling, dictionaries, transliteration).
public static class LanguageToolsConfiguration
{
    public static IServiceCollection AddLanguageTools(this IServiceCollection services) =>
        services.AddSingleton<SpellingIndexCache>()
            .AddScoped<ISpellingReplacementRepository>(DatabaseFactory.GetSpellingReplacementRepository);
}
