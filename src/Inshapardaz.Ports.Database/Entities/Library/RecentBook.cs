using System;
using System.Collections.Generic;
using System.Text;

namespace Inshapardaz.Ports.Database.Entities.Library
{
    public class RecentBook
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public int BookId { get; set; }

        public Book Book { get; set; }

        public DateTime DateRead { get; set; }
    }
}
