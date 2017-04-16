// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Database.cs" company="Inshapardaz">
//   Muhammad Umer Farooq
// </copyright>
// <summary>
//   Defines the Database type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Inshapardaz.Domain.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Inshapardaz.Domain
{
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        public DatabaseContext()
        {
        }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }
                
        public virtual DbSet<Dictionary> Dictionaries { get; set; }
        public virtual DbSet<Meaning> Meanings { get; set; }
        public virtual DbSet<Translation> Translations { get; set; }
        public virtual DbSet<Word> Words { get; set; }
        public virtual DbSet<WordDetail> WordDetails { get; set; }
        public virtual DbSet<WordRelation> WordRelations { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
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
        }
    }
}
