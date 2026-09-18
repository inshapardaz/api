using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class UpsertBookRatingRequest(int libraryId, int accountId, int bookId, RatingModel rating)
    : LibraryBaseCommand(libraryId)
{
    public int AccountId { get; } = accountId;
    public int BookId { get; } = bookId;
    public RatingModel Rating { get; } = rating;

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public RatingModel Rating { get; set; }
    }
}

public class UpsertBookRatingRequestHandler(IBookRepository bookRepository)
    : RequestHandlerAsync<UpsertBookRatingRequest>
{
    [LibraryAuthorize(1, Role.Reader, Role.Writer, Role.Admin, Role.LibraryAdmin)]
    public override async Task<UpsertBookRatingRequest> HandleAsync(UpsertBookRatingRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        if (command.Rating.Value < 1 || command.Rating.Value > 5)
        {
            throw new BadRequestException("Rating value must be between 1 and 5");
        }

        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);

        if (book != null)
        {
            var result = await bookRepository.UpsertBookRating(
                command.LibraryId, command.AccountId, command.BookId, command.Rating, cancellationToken);
            command.Result = new UpsertBookRatingRequest.RequestResult
            {
                Rating = result
            };
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
