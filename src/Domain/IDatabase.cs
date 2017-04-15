using Inshapardaz.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain
{
    public interface IDatabaseContext
    {
        DbSet<Dictionary> Dictionaries { get; set; }
        DbSet<Word> Words { get; set; }
        DbSet<Meaning> Meanings { get; set; }
        DbSet<Translation> Translations { get; set; }
        DbSet<WordDetail> WordDetails { get; set; }
        DbSet<WordRelation> WordRelations { get; set; }

        int SaveChanges();
    }
}
