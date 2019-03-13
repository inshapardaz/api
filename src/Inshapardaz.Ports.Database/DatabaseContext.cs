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

        public DbSet<Category> Category { get; set; }
        public DbSet<Author> Author { get; set; }
        public DbSet<Book> Book { get; set; }
        public DbSet<Chapter> Chapter { get; set; }
        public DbSet<ChapterText> ChapterText { get; set; }
        public DbSet<BookPage> BookPages { get; set; }
        public DbSet<RecentBook> RecentBooks { get; set; }
        public DbSet<FavoriteBook> FavoriteBooks { get; set; }

        public DbSet<Series> Series { get; set; }

        public DbSet<Magazine> Magazine { get; set; }
        public DbSet<MagazineCategory> MagazineCategory { get; set; }
        public DbSet<Issue> Issue { get; set; }
        

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

            modelBuilder.Entity<BookCategory>(entity =>
            {
                entity.ToTable("BookCategory", "Library")
                      .HasKey(t => new { t.BookId, t.CategoryId });
            });

            modelBuilder.Entity<Series>(entity =>
            {
                entity.ToTable("Series", "Library");
            });

            modelBuilder.Entity<SeriesCategory>(entity =>
            {
                entity.ToTable("SeriesCategory", "Library")
                      .HasKey(t => new { t.SeriesId, t.CategoryId });
            });

            modelBuilder.Entity<Chapter>(entity =>
            {
                entity.ToTable("Chapter", "Library");
            });

            modelBuilder.Entity<ChapterText>(entity =>
            {
                entity.ToTable("ChapterText", "Library");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category", "Library");
            });

            modelBuilder.Entity<BookPage>(entity =>
            {
                entity.ToTable("BookPage", "Library");
            });

            modelBuilder.Entity<RecentBook>(entity =>
                entity.ToTable("RecentBooks", "Library")
            );

            modelBuilder.Entity<FavoriteBook>(entity =>
                entity.ToTable("FavoriteBooks", "Library")
            );

            modelBuilder.Entity<Magazine>(entity =>
                entity.ToTable("Magazine", "Library")
            );

            modelBuilder.Entity<MagazineCategory>(entity =>
                entity.ToTable("MagazineCategory", "Library")
            );

            modelBuilder.Entity<Issue>(entity =>
                entity.ToTable("Issue", "Library")
            );
        }
    }
}