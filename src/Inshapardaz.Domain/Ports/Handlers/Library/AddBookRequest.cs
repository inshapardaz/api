using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models.Library;
using Inshapardaz.Domain.Ports.Handlers.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace Inshapardaz.Domain.Ports.Library
{
    public class AddBookRequest : LibraryAuthorisedCommand
    {
        public AddBookRequest(ClaimsPrincipal claims, int libraryId, BookModel book)
        : base(claims, libraryId)
        {
            Book = book;
        }

        public BookModel Book { get; }

        public BookModel Result { get; set; }
    }

    public class AddBookRequestHandler : RequestHandlerAsync<AddBookRequest>
    {
        private readonly IBookRepository _bookRepository;
        private readonly IAuthorRepository _authorRepository;
        private readonly ISeriesRepository _seriesRepository;
        private readonly ICategoryRepository _categoryRepository;

        public AddBookRequestHandler(IBookRepository bookRepository, IAuthorRepository authorRepository,
                                     ISeriesRepository seriesRepository, ICategoryRepository categoryRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _seriesRepository = seriesRepository;
            _categoryRepository = categoryRepository;
        }

        [Authorise(step: 1, HandlerTiming.Before)]
        public override async Task<AddBookRequest> HandleAsync(AddBookRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var author = await _authorRepository.GetAuthorById(command.LibraryId, command.Book.AuthorId, cancellationToken);
            if (author == null)
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

            command.Result = await _bookRepository.AddBook(command.LibraryId, command.Book, command.UserId, cancellationToken);

            command.Result.AuthorName = author.Name;
            command.Result.SeriesName = series?.Name;
            command.Result.Categories = categories?.ToList();

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
