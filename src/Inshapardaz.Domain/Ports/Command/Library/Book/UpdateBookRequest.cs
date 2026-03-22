using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class UpdateBookRequest(int libraryId, int? accountId, BookModel book) : LibraryBaseCommand(libraryId)
{
    public int? AccountId { get; } = accountId;
    public BookModel Book { get; } = book;

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public BookModel Book { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdateBookRequestHandler(
    IBookRepository bookRepository,
    IAuthorRepository authorRepository,
    ISeriesRepository seriesRepository,
    ICategoryRepository categoryRepository)
    : RequestHandlerAsync<UpdateBookRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdateBookRequest> HandleAsync(UpdateBookRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        IEnumerable<AuthorModel> authors = null;
        if (command.Book.Authors != null && command.Book.Authors.Any())
        {
            authors = await authorRepository.GetAuthorByIds(command.LibraryId, command.Book.Authors.Select(a => a.Id), cancellationToken);
            if (authors.Count() != command.Book.Authors.Count())
            {
                throw new BadRequestException();
            }
        }

        if (authors == null || authors.FirstOrDefault() == null)
        {
            throw new BadRequestException();
        }

        SeriesModel series = null;
        if (command.Book.SeriesId.HasValue)
        {
            series = await seriesRepository.GetSeriesById(command.LibraryId, command.Book.SeriesId.Value, cancellationToken);
            if (series == null)
            {
                throw new BadRequestException();
            }
        }

        IEnumerable<CategoryModel> categories = null;
        if (command.Book.Categories != null && command.Book.Categories.Any())
        {
            categories = await categoryRepository.GetCategoriesByIds(command.LibraryId, command.Book.Categories.Select(c => c.Id), cancellationToken);
            if (categories.Count() != command.Book.Categories.Count())
            {
                throw new BadRequestException();
            }
        }

        var result = await bookRepository.GetBookById(command.LibraryId, command.Book.Id, command.AccountId, cancellationToken);

        if (result == null)
        {
            var book = command.Book;
            book.Id = default;
            command.Result.Book = await bookRepository.AddBook(command.LibraryId, book, command.AccountId, cancellationToken);
            command.Result.Book.SeriesName = series?.Name;
            command.Result.Book.Categories = categories?.ToList();
            command.Result.HasAddedNew = true;
        }
        else
        {
            await bookRepository.UpdateBook(command.LibraryId, command.Book, cancellationToken);

            command.Result.Book = command.Book;
            command.Result.Book.SeriesName = series?.Name;
            command.Result.Book.Categories = categories?.ToList();
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
