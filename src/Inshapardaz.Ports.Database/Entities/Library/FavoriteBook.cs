using System;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class FavoriteBook
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public int BookId { get; set; }

        public Book Book { get; set; }

        public DateTime DateAdded { get; set; }
    }
}
