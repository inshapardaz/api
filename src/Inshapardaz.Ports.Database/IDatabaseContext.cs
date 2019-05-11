using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Ports.Database.Entities;
using Inshapardaz.Ports.Database.Entities.Dictionary;
using Inshapardaz.Ports.Database.Entities.Library;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Inshapardaz.Ports.Database
{
    public interface IDatabaseContext
    {
        DbSet<Dictionary> Dictionary { get; set; }
        DbSet<Meaning> Meaning { get; set; }
        DbSet<Translation> Translation { get; set; }
        DbSet<Word> Word { get; set; }
        DbSet<WordRelation> WordRelation { get; set; }
        DbSet<DictionaryDownload> DictionaryDownload { get; set; }
        DbSet<File> File { get; set; }

        DbSet<Category> Category { get; set; }
        DbSet<Author> Author { get; set; }
        DbSet<Book> Book { get; set; }
        DbSet<Chapter> Chapter { get; set; }

        DbSet<ChapterContent> ChapterContent { get; set; }

        DbSet<Series> Series { get; set; }

        DbSet<RecentBook> RecentBooks { get; set; }
        DbSet<FavoriteBook> FavoriteBooks { get; set; }
        DbSet <BookPage> BookPages { get; set; }
        DbSet<ChapterText> ChapterText { get; set; }

        DbSet<Magazine> Magazine { get; set; }
        DbSet<MagazineCategory> MagazineCategory { get; set; }
        DbSet<Issue> Issue { get; set; }
        DbSet<Article> Article { get; set; }
        DbSet<ArticleText> ArticleText { get; set; }


        DatabaseFacade Database { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}