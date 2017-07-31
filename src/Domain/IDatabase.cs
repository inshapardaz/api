using Inshapardaz.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Domain
{
    public interface IDatabaseContext
    {
        DbSet<AggregatedCounter> AggregatedCounter { get; set; }
        DbSet<Counter> Counter { get; set; }
        DbSet<Dictionary> Dictionary { get; set; }
        DbSet<Hash> Hash { get; set; }
        DbSet<Job> Job { get; set; }
        DbSet<JobParameter> JobParameter { get; set; }
        DbSet<JobQueue> JobQueue { get; set; }
        DbSet<List> List { get; set; }
        DbSet<Meaning> Meaning { get; set; }
        DbSet<Schema> Schema { get; set; }
        DbSet<Server> Server { get; set; }
        DbSet<Set> Set { get; set; }
        DbSet<State> State { get; set; }
        DbSet<Translation> Translation { get; set; }
        DbSet<Word> Word { get; set; }
        DbSet<WordDetail> WordDetail { get; set; }
        DbSet<WordRelation> WordRelation { get; set; }
        DbSet<DictionaryDownload> DictionaryDownload { get; set; }
        DbSet<File> File { get; set; }

        int SaveChanges();
    }
}