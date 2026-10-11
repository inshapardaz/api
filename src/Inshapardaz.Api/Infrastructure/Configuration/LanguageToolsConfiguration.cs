using Inshapardaz.Adapters.Dictionary.Formats;
using Inshapardaz.Adapters.Transliteration;
using Inshapardaz.Api.Converters;
using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Adapters.Repositories;
using Inshapardaz.Domain.Helpers.Spelling;
using Inshapardaz.Domain.Helpers.Transliteration;

namespace Inshapardaz.Api.Infrastructure.Configuration;

// Services behind the /tools language features (spelling, dictionaries, transliteration).
public static class LanguageToolsConfiguration
{
    public static IServiceCollection AddLanguageTools(this IServiceCollection services) =>
        services.AddSingleton<SpellingIndexCache>()
            .AddScoped<ISpellingReplacementRepository>(DatabaseFactory.GetSpellingReplacementRepository)
            .AddScoped<IDictionaryRepository>(DatabaseFactory.GetDictionaryRepository)
            .AddTransient<IReadDictionaryFiles, DictionaryFileReader>()
            .AddTransient<IWriteDictionaryFiles, DictionaryFileWriter>()
            .AddTransient<IRenderDictionary, DictionaryRenderer>()
            .AddSingleton<TransliterationDictionaryCache>()
            .AddSingleton<ITransliterate, Transliterator>()
            .AddScoped<ITransliterationRepository>(DatabaseFactory.GetTransliterationRepository)
            .AddTransient<IRenderTransliteration, TransliterationRenderer>();
}
