namespace Inshapardaz.Database.Migrations
{
    public static class Schemas
    {
        public const string Dbo = "dbo";
        public const string Library = "Library";
        public const string Language = "Language";
    }

    public static class Tables
    {
        public const string Accounts = "Accounts";
        public const string Author = "Author";
        public const string File = "File";
        public const string Series = "Series";
        public const string Category = "Category";
        public const string Book = "Book";
        public const string SeriesCategory = "SeriesCategory";
        public const string BookCategory = "BookCategory";
        public const string Chapter = "Chapter";
        public const string ChapterContent = "ChapterContent";
        public const string BookFile = "BookFile";
        public const string BookContent = "BookContent";
        public const string RecentBooks = "RecentBooks";
        public const string FavoriteBooks = "FavoriteBooks";
        public const string PeriodicalCategory = "PeriodicalCategory";
        public const string Periodical = "Periodical";
        public const string Issue = "Issue";
        public const string Article = "Article";
        public const string ArticleText = "ArticleText";
        public const string ArticleContent = "ArticleContent";
        public const string BookPage = "BookPage";
        public const string Library = "Library";
        public const string BookShelf = "BookShelf";
        public const string BookShelfBook = "BookShelfBook";
        public const string Corrections = "Corrections";
        public const string IssueChapter = "IssueChapter";
        public const string IssuePage = "IssuePage";
        public const string IssueChapterContent = "IssueChapterContent";

    }

    public static class Columns
    {
        public const string Id = "Id";
        public const string Name = "Name";
        public const string Description = "Description";
        public const string DateCreated = "DateCreated";
        public const string BookId = "BookId";
        public const string ImageId = "ImageId";
        public const string FileId = "FileId";
        public const string IsPublic = "IsPublic";

        public const string Language = "Language";
    }
}
