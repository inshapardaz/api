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

namespace Inshapardaz.Domain.Ports.Handlers.Library.Periodical
{
    public class UpdatePeriodicalRequest : LibraryBaseCommand
    {
        public UpdatePeriodicalRequest(int libraryId, PeriodicalModel periodical)
            : base(libraryId)
        {
            Periodical = periodical;
        }

        public PeriodicalModel Periodical { get; }

        public RequestResult Result { get; set; } = new RequestResult();

        public class RequestResult
        {
            public PeriodicalModel Periodical { get; set; }

            public bool HasAddedNew { get; set; }
        }
    }

    public class UpdatePeriodicalRequestHandler : RequestHandlerAsync<UpdatePeriodicalRequest>
    {
        private readonly IPeriodicalRepository _periodicalRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UpdatePeriodicalRequestHandler(IPeriodicalRepository periodicalRepository, ICategoryRepository categoryRepository)
        {
            _periodicalRepository = periodicalRepository;
            _categoryRepository = categoryRepository;
        }

        [LibraryAuthorize(1, Role.LibraryAdmin, Role.Writer)]
        public override async Task<UpdatePeriodicalRequest> HandleAsync(UpdatePeriodicalRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            IEnumerable<CategoryModel> categories = null;
            if (command.Periodical.Categories != null && command.Periodical.Categories.Any())
            {
                categories = await _categoryRepository.GetCategoriesByIds(command.LibraryId, command.Periodical.Categories.Select(c => c.Id), cancellationToken);
                if (categories.Count() != command.Periodical.Categories.Count())
                {
                    throw new BadRequestException();
                }

            }
            var result = await _periodicalRepository.GetPeriodicalById(command.LibraryId, command.Periodical.Id, cancellationToken);

            if (result == null)
            {
                var periodical = command.Periodical;
                periodical.Id = default;
                command.Result.Periodical = await _periodicalRepository.AddPeriodical(command.LibraryId, periodical, cancellationToken);
                command.Result.HasAddedNew = true;
            }
            else
            {
                command.Result.Periodical = await _periodicalRepository.UpdatePeriodical(command.LibraryId, command.Periodical, cancellationToken);
            }

            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
