namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class BookGenre
    {
        public int BookId { get; set; }

        public Book Book { get; set; }

        public int GenereId { get; set; }

        public Genre Genre { get; set; }
    }
}