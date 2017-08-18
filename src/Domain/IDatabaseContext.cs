using Inshapardaz.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain
{
    public interface IDatabaseContext
    {
        DbSet<Dictionary> Dictionary { get; set; }
        DbSet<Meaning> Meaning { get; set; }
        DbSet<Translation> Translation { get; set; }
        DbSet<Word> Word { get; set; }
        DbSet<WordDetail> WordDetail { get; set; }
        DbSet<WordRelation> WordRelation { get; set; }
        DbSet<DictionaryDownload> DictionaryDownload { get; set; }
        DbSet<File> File { get; set; }

        int SaveChanges();
    }
}