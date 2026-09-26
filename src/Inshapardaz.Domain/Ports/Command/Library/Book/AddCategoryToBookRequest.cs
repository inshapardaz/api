using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Book;

public class AddCategoryToBookRequest(int libraryId, int bookId, int categoryId) : LibraryBaseCommand(libraryId)
{
    public int BookId { get; } = bookId;
    public int CategoryId { get; } = categoryId;
    public IEnumerable<CategoryModel> Result { get; set; }
}

public class AddCategoryToBookRequestHandler(IBookRepository bookRepository, ICategoryRepository categoryRepository)
    : RequestHandlerAsync<AddCategoryToBookRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<AddCategoryToBookRequest> HandleAsync(AddCategoryToBookRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        var book = await bookRepository.GetBookById(command.LibraryId, command.BookId, null, cancellationToken);
        if (book == null)
        {
            throw new NotFoundException();
        }

        var category = await categoryRepository.GetCategoryById(command.LibraryId, command.CategoryId, cancellationToken);
        if (category == null)
        {
            throw new NotFoundException();
        }

        await bookRepository.AddCategoryToBook(command.LibraryId, command.BookId, command.CategoryId, cancellationToken);
        command.Result = await bookRepository.GetBookCategories(command.LibraryId, command.BookId, cancellationToken);

        return await base.HandleAsync(command, cancellationToken);
    }
}
