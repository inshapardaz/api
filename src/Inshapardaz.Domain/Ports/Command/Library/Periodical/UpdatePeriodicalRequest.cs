using Inshapardaz.Domain.Adapters.Repositories.Library;
using Inshapardaz.Domain.Exception;
using Inshapardaz.Domain.Models;
using Inshapardaz.Domain.Models.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Command.Library.Periodical;

public class UpdatePeriodicalRequest(int libraryId, PeriodicalModel periodical) : LibraryBaseCommand(libraryId)
{
    public PeriodicalModel Periodical { get; } = periodical;

    public RequestResult Result { get; set; } = new RequestResult();

    public class RequestResult
    {
        public PeriodicalModel Periodical { get; set; }

        public bool HasAddedNew { get; set; }
    }
}

public class UpdatePeriodicalRequestHandler(
    IPeriodicalRepository periodicalRepository,
    ICategoryRepository categoryRepository)
    : RequestHandlerAsync<UpdatePeriodicalRequest>
{
    [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
    public override async Task<UpdatePeriodicalRequest> HandleAsync(UpdatePeriodicalRequest command, CancellationToken cancellationToken = new CancellationToken())
    {
        IEnumerable<CategoryModel> categories = null;
        if (command.Periodical.Categories != null && command.Periodical.Categories.Any())
        {
            categories = await categoryRepository.GetCategoriesByIds(command.LibraryId, command.Periodical.Categories.Select(c => c.Id), cancellationToken);
            if (categories.Count() != command.Periodical.Categories.Count())
            {
                throw new BadRequestException();
            }

        }
        var result = await periodicalRepository.GetPeriodicalById(command.LibraryId, command.Periodical.Id, cancellationToken);

        if (result == null)
        {
            var periodical = command.Periodical;
            periodical.Id = default;
            command.Result.Periodical = await periodicalRepository.AddPeriodical(command.LibraryId, periodical, cancellationToken);
            command.Result.HasAddedNew = true;
        }
        else
        {
            command.Result.Periodical = await periodicalRepository.UpdatePeriodical(command.LibraryId, command.Periodical, cancellationToken);
        }

        return await base.HandleAsync(command, cancellationToken);
    }
}
