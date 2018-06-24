using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Inshapardaz.Domain.Entities.Library;
using Inshapardaz.Domain.Repositories.Library;
using Paramore.Brighter;

namespace Inshapardaz.Domain.Ports.Library
{
    public class GetGenresRequest : RequestBase
    {
        public IEnumerable<Genre> Result { get; set; }
    }

    public class GetGenresRequestHandler : RequestHandlerAsync<GetGenresRequest>
    {
        private readonly IGenreRepository _genreRepository;

        public GetGenresRequestHandler(IGenreRepository genreRepository)
        {
            _genreRepository = genreRepository;
        }

        public override async Task<GetGenresRequest> HandleAsync(GetGenresRequest command, CancellationToken cancellationToken = new CancellationToken())
        {
            var results = await _genreRepository.GetGenres(cancellationToken);
            command.Result = results;
            return await base.HandleAsync(command, cancellationToken);
        }
    }
}
