using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain.Database
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
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}