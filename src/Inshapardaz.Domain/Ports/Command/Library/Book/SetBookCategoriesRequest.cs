using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class SetBookCategoriesRequest(int libraryId, int bookId, IEnumerable<int> categoryIds) : LibraryBaseCommand(libraryId)
{
    public int BookId { get; } = bookId;
    public IEnumerable<int> CategoryIds { get; } = categoryIds;
    public IEnumerable<CategoryModel> Result { get; set; }
}

public class SetBookCategoriesRequestHandler(IBookRepository bookRepository, ICategoryRepository categoryRepository)
    : RequestHandlerAsync<SetBookCategoriesRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<SetBookCategoriesRequest> HandleAsync(SetBookCategoriesRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
        if (book == null)
        {
            throw new NotFoundException();
        }

        var categoryIds = (command.CategoryIds ?? Enumerable.Empty<int>()).Distinct().ToList();
        if (categoryIds.Any())
        {
            var existingCategories = await categoryRepository.GetCategoriesByIds(command.LibraryId, categoryIds, cancellationToken);
            if (existingCategories.Count() != categoryIds.Count)
            {
                throw new BadRequestException("One or more categories do not exist in this library.");
            }
        }

        await bookRepository.SetBookCategories(command.LibraryId, command.BookId, categoryIds, cancellationToken);
        command.Result = await bookRepository.GetBookCategories(command.LibraryId, command.BookId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
