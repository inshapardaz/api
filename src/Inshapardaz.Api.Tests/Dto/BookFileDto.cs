using System;

namespace Inshapardaz.Api.Tests.Dto
{
    public class BookFileDto
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int FileId { get; set; }

        public string Language { get; set; }
    }

    public class BookContentDto
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int FileId { get; set; }

        public string Language { get; set; }

        public string MimeType { get; set; }

        public string FilePath { get; set; }
    }

    public class RecentBookDto
    {
        public int BookId { get; set; }

        public int AccountId { get; set; }

        public DateTime DateRead { get; set; }

        public int LibraryId { get; set; }
    }

    public class FavoriteBooksDto
    {
        public int BookId { get; set; }

        public Guid AccountId { get; set; }

        public DateTime DateAdded { get; set; }

        public int LibraryId { get; set; }
    }
}
