using System;

namespace Inshapardaz.Functions.Tests.Dto
{
    public class BookFileDto
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        public int FileId { get; set; }
    }

    public class RecentBookDto
    {
        public int BookId { get; set; }

        public Guid UserId { get; set; }

        public DateTime DateRead { get; set; }

        public int LibraryId { get; set; }
    }

    public class FavoriteBooksDto
    {
        public int BookId { get; set; }

        public Guid UserId { get; set; }

        public DateTime DateAdded { get; set; }

        public int LibraryId { get; set; }
    }
}
