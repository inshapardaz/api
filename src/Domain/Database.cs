using Inshapardaz.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Inshapardaz.Domain
{
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

        public virtual DbSet<AggregatedCounter> AggregatedCounter { get; set; }
        public virtual DbSet<Counter> Counter { get; set; }
        public virtual DbSet<Dictionary> Dictionary { get; set; }
        public virtual DbSet<Hash> Hash { get; set; }
        public virtual DbSet<Job> Job { get; set; }
        public virtual DbSet<JobParameter> JobParameter { get; set; }
        public virtual DbSet<JobQueue> JobQueue { get; set; }
        public virtual DbSet<List> List { get; set; }
        public virtual DbSet<Meaning> Meaning { get; set; }
        public virtual DbSet<Schema> Schema { get; set; }
        public virtual DbSet<Server> Server { get; set; }
        public virtual DbSet<Set> Set { get; set; }
        public virtual DbSet<State> State { get; set; }
        public virtual DbSet<Translation> Translation { get; set; }
        public virtual DbSet<Word> Word { get; set; }
        public virtual DbSet<WordDetail> WordDetail { get; set; }
        public virtual DbSet<WordRelation> WordRelation { get; set; }
        public virtual DbSet<DictionaryDownload> DictionaryDownload { get; set; }
        public virtual DbSet<File> File { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AggregatedCounter>(entity =>
            {
                entity.ToTable("AggregatedCounter", "HangFire");

                entity.HasIndex(e => new {e.Value, e.Key})
                    .HasName("UX_HangFire_CounterAggregated_Key")
                    .IsUnique();

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Counter>(entity =>
            {
                entity.ToTable("Counter", "HangFire");

                entity.HasIndex(e => new {e.Value, e.Key})
                    .HasName("IX_HangFire_Counter_Key");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Dictionary>(entity =>
            {
                entity.ToTable("Dictionary", "Inshapardaz");

                entity.Property(e => e.Name).HasMaxLength(255);

                entity.Property(e => e.UserId).HasMaxLength(50);
            });

            modelBuilder.Entity<Hash>(entity =>
            {
                entity.ToTable("Hash", "HangFire");

                entity.HasIndex(e => new {e.ExpireAt, e.Key})
                    .HasName("IX_HangFire_Hash_Key");

                entity.HasIndex(e => new {e.Id, e.ExpireAt})
                    .HasName("IX_HangFire_Hash_ExpireAt");

                entity.HasIndex(e => new {e.Key, e.Field})
                    .HasName("UX_HangFire_Hash_Key_Field")
                    .IsUnique();

                entity.Property(e => e.Field)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Job>(entity =>
            {
                entity.ToTable("Job", "HangFire");

                entity.HasIndex(e => e.StateName)
                    .HasName("IX_HangFire_Job_StateName");

                entity.HasIndex(e => new {e.Id, e.ExpireAt})
                    .HasName("IX_HangFire_Job_ExpireAt");

                entity.Property(e => e.Arguments).IsRequired();

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.InvocationData).IsRequired();

                entity.Property(e => e.StateName).HasMaxLength(20);
            });

            modelBuilder.Entity<JobParameter>(entity =>
            {
                entity.ToTable("JobParameter", "HangFire");

                entity.HasIndex(e => new {e.JobId, e.Name})
                    .HasName("IX_HangFire_JobParameter_JobIdAndName");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(40);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.JobParameter)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_JobParameter_Job");
            });

            modelBuilder.Entity<JobQueue>(entity =>
            {
                entity.ToTable("JobQueue", "HangFire");

                entity.HasIndex(e => new {e.Queue, e.FetchedAt})
                    .HasName("IX_HangFire_JobQueue_QueueAndFetchedAt");

                entity.Property(e => e.FetchedAt).HasColumnType("datetime");

                entity.Property(e => e.Queue)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<List>(entity =>
            {
                entity.ToTable("List", "HangFire");

                entity.HasIndex(e => new {e.Id, e.ExpireAt})
                    .HasName("IX_HangFire_List_ExpireAt");

                entity.HasIndex(e => new {e.ExpireAt, e.Value, e.Key})
                    .HasName("IX_HangFire_List_Key");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Meaning>(entity =>
            {
                entity.ToTable("Meaning", "Inshapardaz");

                entity.HasOne(d => d.WordDetail)
                    .WithMany(p => p.Meaning)
                    .HasForeignKey(d => d.WordDetailId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Meaning_WordDetail");
            });

            modelBuilder.Entity<Schema>(entity =>
            {
                entity.HasKey(e => e.Version)
                    .HasName("PK_HangFire_Schema");

                entity.ToTable("Schema", "HangFire");

                entity.Property(e => e.Version).ValueGeneratedNever();
            });

            modelBuilder.Entity<Server>(entity =>
            {
                entity.ToTable("Server", "HangFire");

                entity.Property(e => e.Id).HasMaxLength(100);

                entity.Property(e => e.LastHeartbeat).HasColumnType("datetime");
            });

            modelBuilder.Entity<Set>(entity =>
            {
                entity.ToTable("Set", "HangFire");

                entity.HasIndex(e => new {e.Id, e.ExpireAt})
                    .HasName("IX_HangFire_Set_ExpireAt");

                entity.HasIndex(e => new {e.Key, e.Value})
                    .HasName("UX_HangFire_Set_KeyAndValue")
                    .IsUnique();

                entity.HasIndex(e => new {e.ExpireAt, e.Value, e.Key})
                    .HasName("IX_HangFire_Set_Key");

                entity.Property(e => e.ExpireAt).HasColumnType("datetime");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<State>(entity =>
            {
                entity.ToTable("State", "HangFire");

                entity.HasIndex(e => e.JobId)
                    .HasName("IX_HangFire_State_JobId");

                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Reason).HasMaxLength(100);

                entity.HasOne(d => d.Job)
                    .WithMany(p => p.State)
                    .HasForeignKey(d => d.JobId)
                    .HasConstraintName("FK_HangFire_State_Job");
            });

            modelBuilder.Entity<Translation>(entity =>
            {
                entity.ToTable("Translation", "Inshapardaz");

                entity.HasOne(d => d.WordDetail)
                    .WithMany(p => p.Translation)
                    .HasForeignKey(d => d.WordDetailId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Translation_WordDetail");
            });

            modelBuilder.Entity<Word>(entity =>
            {
                entity.ToTable("Word", "Inshapardaz");

                entity.Property(e => e.DictionaryId).HasDefaultValueSql("1");

                entity.HasOne(d => d.Dictionary)
                    .WithMany(p => p.Word)
                    .HasForeignKey(d => d.DictionaryId)
                    .HasConstraintName("FK_Word_Dictionary");
            });

            modelBuilder.Entity<WordDetail>(entity =>
            {
                entity.ToTable("WordDetail", "Inshapardaz");

                entity.HasOne(d => d.WordInstance)
                    .WithMany(p => p.WordDetail)
                    .HasForeignKey(d => d.WordInstanceId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_WordDetail_Word");
            });

            modelBuilder.Entity<WordRelation>(entity =>
            {
                entity.ToTable("WordRelation", "Inshapardaz");

                entity.HasOne(d => d.RelatedWord)
                    .WithMany(p => p.WordRelationRelatedWord)
                    .HasForeignKey(d => d.RelatedWordId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_WordRelation_RelatedWord");

                entity.HasOne(d => d.SourceWord)
                    .WithMany(p => p.WordRelationSourceWord)
                    .HasForeignKey(d => d.SourceWordId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_WordRelation_SourceWord");
            });

            modelBuilder.Entity<File>(entity => { entity.ToTable("File", "Inshapardaz"); });

            modelBuilder.Entity<DictionaryDownload>(entity =>
            {
                entity.ToTable("DictionaryDownload", "Inshapardaz");
                entity.HasOne(d => d.Dictionary)
                    .WithMany(d => d.Downloads);
                entity.HasOne(d => d.File);
            });
        }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dictionary>(entity =>
            {
                entity.ToTable("Dictionary", "Inshapardaz");

                entity.Property(e => e.Name).HasMaxLength(255);
            });

            modelBuilder.Entity<Meaning>(entity =>
            {
                entity.ToTable("Meaning", "Inshapardaz");

                entity.HasOne(d => d.WordDetail)
                    .WithMany(p => p.Meanings)
                    .HasForeignKey(d => d.WordDetailId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Translation>(entity =>
            {
                entity.ToTable("Translation", "Inshapardaz");

                entity.HasOne(d => d.WordDetail)
                    .WithMany(p => p.Translations)
                    .HasForeignKey(d => d.WordDetailId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Word>(entity =>
            {
                entity.ToTable("Word", "Inshapardaz");

                entity.Property(e => e.DictionaryId).HasDefaultValue(1);

                entity.HasOne(d => d.Dictionary)
                    .WithMany(p => p.Words)
                    .HasForeignKey(d => d.DictionaryId);
            });

            modelBuilder.Entity<WordDetail>(entity =>
            {
                entity.ToTable("WordDetail", "Inshapardaz");

                entity.HasOne(d => d.WordInstance)
                    .WithMany(p => p.WordDetails)
                    .HasForeignKey(d => d.WordInstanceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<WordRelation>(entity =>
            {
                entity.ToTable("WordRelation", "Inshapardaz");

                entity.HasOne(d => d.RelatedWord)
                    .WithMany(p => p.WordRelations)
                    .HasForeignKey(d => d.RelatedWordId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(d => d.SourceWord)
                    .WithMany(p => p.WordRelatedTo)
                    .HasForeignKey(d => d.SourceWordId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<DictionaryDownload>(entity =>
            {
                entity.HasOne(d => d.Dictionary)
                    .WithMany(d => d.Downloads);
                entity.HasOne(d => d.File);
            });
        }
    }

    public class DataContextFactory : IDbContextFactory<DatabaseContext>
    {
        public DatabaseContext Create(DbContextFactoryOptions options)
        {
            // Used only for EF .NET Core CLI tools (update database/migrations etc.)
            var builder = new ConfigurationBuilder()
                .SetBasePath(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory()))
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            var config = builder.Build();

            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlServer(config.GetConnectionString("DefaultDatabase"));

            return new DatabaseContext(optionsBuilder.Options);
        }*/
    }
}