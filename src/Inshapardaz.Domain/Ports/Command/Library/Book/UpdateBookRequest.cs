using Inshapardaz.Domain.Adapters;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Handlers.Library;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Command;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Handlers.Library.Book
{
    public class UpdateBookRequest : LibraryBaseCommand
    {
        public UpdateBookRequest(int libraryId, int? accountId, BookModel book)
            : base(libraryId)
        {
            AccountId = accountId;
            Book = book;
        }

        public int? AccountId { get; }
        public BookModel Book { get; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public BookModel Book { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdateBookRequestHandler : RequestHandlerAsync<UpdateBookRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly ISeriesRepository _seriesRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateBookRequestHandler(IBookRepository bookRepository, IAuthorRepository authorRepository,
                                        ISeriesRepository seriesRepository, ICategoryRepository categoryRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _seriesRepository = seriesRepository;
            _categoryRepository = categoryRepository;
        }

        [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
        public override async Task<UpdateBookRequest> HandleAsync(UpdateBookRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            IEnumerable<AuthorModel> authors = null;
            if (command.Book.Authors != null && command.Book.Authors.Any())
            {
                authors = await _authorRepository.GetAuthorByIds(command.LibraryId, command.Book.Authors.Select(a => a.Id), cancellationToken);
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
                series = await _seriesRepository.GetSeriesById(command.LibraryId, command.Book.SeriesId.Value, cancellationToken);
                if (series == null)
                {
                    throw new BadRequestException();
                }
            }

            IEnumerable<CategoryModel> categories = null;
            if (command.Book.Categories != null && command.Book.Categories.Any())
            {
                categories = await _categoryRepository.GetCategoriesByIds(command.LibraryId, command.Book.Categories.Select(c => c.Id), cancellationToken);
                if (categories.Count() != command.Book.Categories.Count())
                {
                    throw new BadRequestException();
                }
            }

            var result = await _bookRepository.GetBookById(command.LibraryId, command.Book.Id, command.AccountId, cancellationToken);

            if (result == null)
            {
                var book = command.Book;
                book.Id = default;
                command.Result.Book = await _bookRepository.AddBook(command.LibraryId, book, command.AccountId, cancellationToken);
                command.Result.Book.SeriesName = series?.Name;
                command.Result.Book.Categories = categories?.ToList();
                command.Result.HasAddedNew = true;
            }
            else
            {
                await _bookRepository.UpdateBook(command.LibraryId, command.Book, cancellationToken);

                command.Result.Book = command.Book;
                command.Result.Book.SeriesName = series?.Name;
                command.Result.Book.Categories = categories?.ToList();
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
