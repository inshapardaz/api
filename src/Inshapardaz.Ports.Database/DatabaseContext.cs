using Inshapardaz.Ports.Database.Entities;
using Inshapardaz.Ports.Database.Entities.Dictionary;
using Inshapardaz.Ports.Database.Entities.Library;
using Microsoft.EntityFrameworkCore;

namespace Inshapardaz.Ports.Database
{
    public sealed class DatabaseContext : DbContext, IDatabaseContext
    {
        public DatabaseContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Dictionary> Dictionary { get; set; }
        public DbSet<Meaning> Meaning { get; set; }
        public DbSet<Translation> Translation { get; set; }
        public DbSet<Word> Word { get; set; }
        public DbSet<WordRelation> WordRelation { get; set; }
        public DbSet<DictionaryDownload> DictionaryDownload { get; set; }
        public DbSet<File> File { get; set; }

        public DbSet<Genre> Genere { get; set; }
        public DbSet<Author> Author { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Chapter> Chapter { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Inshapardaz");

            modelBuilder.Entity<Dictionary>(entity =>
            {
                entity.ToTable("Dictionary", "Inshapardaz");
                entity.Property(e => e.Name).HasMaxLength(255);
                entity.Property(e => e.UserId).HasMaxLength(50);
            });

            modelBuilder.Entity<Word>(entity =>
            {
                entity.ToTable("Word", "Inshapardaz");
                entity.HasOne(d => d.Dictionary)
                      .WithMany(p => p.Word)
                      .HasForeignKey(d => d.DictionaryId)
                      .HasConstraintName("FK_Word_Dictionary")
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => e.Title);
            });

            modelBuilder.Entity<Meaning>(entity =>
            {
                entity.ToTable("Meaning", "Inshapardaz");

                entity.HasOne(d => d.Word)
                    .WithMany(p => p.Meaning)
                    .HasForeignKey(d => d.WordId)
                    .HasConstraintName("FK_Meaning_Word")
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(m => m.WordId);
            });

            modelBuilder.Entity<Translation>(entity =>
            {
                entity.ToTable("Translation", "Inshapardaz");

                entity.HasOne(d => d.Word)
                    .WithMany(p => p.Translation)
                    .HasForeignKey(d => d.WordId)
                    .HasConstraintName("FK_Translation_Word")
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(t => t.WordId);
            });
            
            modelBuilder.Entity<WordRelation>(entity =>
            {
                entity.ToTable("WordRelation", "Inshapardaz");

                entity.HasOne(d => d.RelatedWord)
                    .WithMany(p => p.WordRelationRelatedWord)
                    .HasForeignKey(d => d.RelatedWordId)
                    .HasConstraintName("FK_WordRelation_RelatedWord")
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(d => d.SourceWord)
                    .WithMany(p => p.WordRelationSourceWord)
                    .HasForeignKey(d => d.SourceWordId)
                    .HasConstraintName("FK_WordRelation_SourceWord")
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(t => t.SourceWordId);
            });

            modelBuilder.Entity<File>(entity =>
            {
                entity.ToTable("File", "Inshapardaz");
            });

            modelBuilder.Entity<DictionaryDownload>(entity =>
            {
                entity.ToTable("DictionaryDownload", "Inshapardaz");
                entity.HasOne(d => d.Dictionary)
                    .WithMany(d => d.Downloads)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.File);
                entity.HasIndex(f => f.DictionaryId);
            });


            modelBuilder.Entity<Author>(entity =>
            {
                entity.ToTable("Author", "Library");
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Book", "Library");
            });

            modelBuilder.Entity<BookImage>(entity =>
            {
                entity.ToTable("BookImage", "Library");
            });

            modelBuilder.Entity<BookGenre>(entity =>
            {
                entity.ToTable("BookGenre", "Library");
            });

            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.ToTable("Chapter", "Library");
            });

            modelBuilder.Entity<ChapterText>(entity =>
            {
                entity.ToTable("ChapterText", "Library");
            });

            modelBuilder.Entity<Genre>(entity =>
            {
                entity.ToTable("Genre", "Library");
            });

            modelBuilder.Entity<BookGenre>()
                        .HasKey(t => new { t.BookId, t.GenreId });

            modelBuilder.Entity<BookGenre>()
                        .HasOne(bg => bg.Book)
                        .WithMany(b => b.Generes)
                        .HasForeignKey(pt => pt.GenreId);

            modelBuilder.Entity<BookGenre>()
                        .HasOne(pt => pt.Genre)
                        .WithMany(t => t.BookGeneres)
                        .HasForeignKey(pt => pt.GenreId);
        }
    }
}