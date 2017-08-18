using Inshapardaz.Data.Dictionary;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Inshapardaz.Data
{
    public class DictionaryDatabase : DbContext, IDictionaryDatabase
    {
        public virtual DbSet<Dictionary.Dictionary> Dictionary { get; set; }
        public virtual DbSet<Meaning> Meaning { get; set; }
        public virtual DbSet<Translation> Translation { get; set; }
        public virtual DbSet<Word> Word { get; set; }
        public virtual DbSet<WordDetail> WordDetail { get; set; }
        public virtual DbSet<WordRelation> WordRelation { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=dictionary.dat");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dictionary.Dictionary>(entity =>
            {
                entity.ToTable("Dictionary");
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.UserId).HasMaxLength(50);
            });

            modelBuilder.Entity<Meaning>(entity =>
            {
                entity.ToTable("Meaning");

                entity.HasOne(d => d.WordDetail)
                    .WithMany(p => p.Meaning)
                    .HasForeignKey(d => d.WordDetailId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Meaning_WordDetail");
            });

            modelBuilder.Entity<Translation>(entity =>
            {
                entity.ToTable("Translation");

                entity.HasOne(d => d.WordDetail)
                    .WithMany(p => p.Translation)
                    .HasForeignKey(d => d.WordDetailId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_Translation_WordDetail");
            });
            modelBuilder.Entity<Word>(entity =>
            {
                entity.ToTable("Word");

                entity.Property(e => e.DictionaryId).HasDefaultValueSql("1");

                entity.HasOne(d => d.Dictionary)
                    .WithMany(p => p.Word)
                    .HasForeignKey(d => d.DictionaryId)
                    .HasConstraintName("FK_Word_Dictionary");
            });

            modelBuilder.Entity<WordDetail>(entity =>
            {
                entity.ToTable("WordDetail");

                entity.HasOne(d => d.WordInstance)
                    .WithMany(p => p.WordDetail)
                    .HasForeignKey(d => d.WordInstanceId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_WordDetail_Word");
            });

            modelBuilder.Entity<WordRelation>(entity =>
            {
                entity.ToTable("WordRelation");

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
        }
    }
}