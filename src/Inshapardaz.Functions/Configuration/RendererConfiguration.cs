using Inshapardaz.Functions.Adapters;
using Inshapardaz.Functions.Adapters.Library;
using Microsoft.Extensions.DependencyInjection;

namespace Inshapardaz.Functions.Configuration
{
    public static class RendererConfiguration
    {
        public static IServiceCollection AddRenderers(this IServiceCollection services)
        {
            services.AddTransient<IRenderEnum, EnumRenderer>();
            services.AddTransient<IRenderEntry, EntryRenderer>();
            // services.AddTransient<IRenderDictionaries, DictionariesRenderer>();
            // services.AddTransient<IRenderDictionary, DictionaryRenderer>();
            // services.AddTransient<IRenderWord, WordRenderer>();
            // services.AddTransient<IRenderWordPage, WordPageRenderer>();
            // services.AddTransient<IRenderMeaning, MeaningRenderer>();
            // services.AddTransient<IRenderTranslation, TranslationRenderer>();
            // services.AddTransient<IRenderRelation, RelationRenderer>();
            // services.AddTransient<IRenderDictionaryDownload, DictionaryDownloadRenderer>();
            // services.AddTransient<IRenderJobStatus, JobStatusRenderer>();
            // services.AddTransient<IRenderFile, FileRenderer>();

            services.AddTransient<IRenderCategory, CategoryRenderer>();
            services.AddTransient<IRenderCategories, CategoriesRenderer>();
            // services.AddTransient<IRenderAuthors, AuthorsRenderer>();
            // services.AddTransient<IRenderAuthor, AuthorRenderer>();
            // services.AddTransient<IRenderBooks, BooksRenderer>();
            // services.AddTransient<IRenderBook, BookRenderer>();
            // services.AddTransient<IRenderChapters, ChaptersRenderer>();
            // services.AddTransient<IRenderChapter, ChapterRenderer>();
            // services.AddTransient<IRenderChapterContent, ChapterContentRenderer>();
            services.AddTransient<IRenderSeries, SeriesRenderer>();
            services.AddTransient<IRenderSeriesList, SeriesListRenderer>();
            // services.AddTransient<IRenderBookFiles, BookFilesRenderer>();
            // services.AddTransient<IRenderBookFile, BookFileRenderer>();
            // services.AddTransient<IRenderPeriodical, PeriodicalRenderer>();
            // services.AddTransient<IRenderPeriodicals, PeriodicalsRenderer>();
            // services.AddTransient<IRenderIssues, IssuesRenderer>();
            // services.AddTransient<IRenderIssue, IssueRenderer>();

            return services;
        }
    }
}